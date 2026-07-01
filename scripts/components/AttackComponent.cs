using Godot;
using System;
using System.Collections.Generic;

public partial class AttackComponent : Node
{
    private TowerData _data;
    private float _effectiveDamage;
    private float _effectiveFireRate;
    private float _effectiveCritChance;
    private float _effectiveCritMultiplier;
    private float _cooldown;
    private string _equipId;
    private float _splashRadiusMultiplier = 1f;
    private float _slowDurationMultiplier = 1f;
    private float _poisonDurationMultiplier = 1f;
    private int _extraChainBounces;
    private float _judgmentCooldown;
    private static readonly Random _rng = new();

    public override void _Ready()
    {
        if (GetParent() is not Node2D)
            GD.PushError($"AttackComponent: parent must be a Node2D (got {GetParent()?.GetClass()}).");
    }

    public void Setup(TowerData data)
    {
        _data = data;
        _effectiveDamage = data.Damage;
        _effectiveFireRate = data.FireRate;
        _cooldown = 0f;
    }

    public void SetEffectiveStats(float damage, float fireRate)
    {
        _effectiveDamage = damage;
        _effectiveFireRate = fireRate;
    }

    public void SetCritStats(float chance, float multiplier)
    {
        _effectiveCritChance = chance;
        _effectiveCritMultiplier = multiplier;
    }

    public void SetEquipId(string id)
    {
        _equipId = id;
    }

    public void SetEquipModifiers(float splashRadiusMult, float slowDurationMult, float poisonDurationMult, int extraChainBounces)
    {
        _splashRadiusMultiplier = splashRadiusMult;
        _slowDurationMultiplier = slowDurationMult;
        _poisonDurationMultiplier = poisonDurationMult;
        _extraChainBounces = extraChainBounces;
    }

    public override void _Process(double delta)
    {
        if (_cooldown > 0f)
            _cooldown -= (float)delta;
        if (_judgmentCooldown > 0f)
            _judgmentCooldown -= (float)delta;
    }

    public bool TryAttack(Enemy target)
    {
        if (target == null || _cooldown > 0f || _data == null) return false;

        Fire(target);
        _cooldown = 1f / _effectiveFireRate;
        return true;
    }

    private void Fire(Enemy target)
    {
        if (_data.ProjectileScene == null)
        {
            GD.PrintErr("AttackComponent: ProjectileScene not set in TowerData.");
            return;
        }

        var towerPos = GetParent<Node2D>().GlobalPosition;
        float damage = _effectiveDamage;
        bool wasCrit = false;

        if (_data.HasCrit && _rng.NextDouble() < _effectiveCritChance)
        {
            damage *= _effectiveCritMultiplier;
            wasCrit = true;

            bool judgmentActive = SynergyManager.Instance?.IsSynergyActive("crust_judgment_protocol") == true;
            if (judgmentActive && _judgmentCooldown <= 0f && target.HealthPercent <= 0.5f)
            {
                target.TakeDamage(9999f);
                _judgmentCooldown = 10f;
                return;
            }
        }

        if (_data.HasExecute && target.HealthPercent <= _data.ExecuteThresholdHPPercent)
            damage *= _data.ExecuteMultiplier;

        if (_data.HasExecute && (target.IsBoss || target.IsHeavy))
            damage *= _data.EliteBonusMultiplier;

        // Judgment Seal: execute below 15% HP with 5s cooldown
        if (_equipId == "judgment_seal" && _judgmentCooldown <= 0f && target.HealthPercent <= 0.15f)
        {
            target.TakeDamage(9999f);
            _judgmentCooldown = 5f;
            return;
        }

        // Holy Flour Pouch: +20% vs elites/boss
        if (_equipId == "holy_flour_pouch" && (target.IsBoss || target.IsHeavy))
            damage *= 1.2f;

        var projectile = ProjectileFactory.Create(_data.ProjectileScene, damage, target, towerPos);
        projectile.WasCrit = wasCrit;

        // Messenger Crate: pierce +1
        if (_equipId == "messenger_crate")
            projectile.PierceCount = 1;

        var effects = new List<Action<Enemy, Vector2>>();

        if (_data.HasSplash)
            effects.Add((mainEnemy, hitPosition) => TriggerSplashDamage(mainEnemy, hitPosition));

        if (_data.HasPoison)
            effects.Add((mainEnemy, _) => ApplyEffect(mainEnemy, new PoisonEffectData
            {
                Duration = _data.PoisonDuration * _poisonDurationMultiplier,
                DamagePerTick = _data.PoisonDamagePerTick,
            }));

        if (_data.HasSlow)
            effects.Add((mainEnemy, _) => ApplyEffect(mainEnemy, new SlowEffectData
            {
                Duration = _data.SlowDuration * _slowDurationMultiplier,
                SpeedMultiplier = _data.SlowMultiplier,
            }));

        if (_data.HasChain)
        {
            float chainDamage = _effectiveDamage * _data.ChainBounceDamageMultiplier;
            effects.Add((mainEnemy, hitPosition) => TriggerChain(mainEnemy, chainDamage));
        }

        // Blessed Crunch Seal: crits make mini-splash (radius 30)
        if (_equipId == "blessed_crunch_seal")
        {
            effects.Add((mainEnemy, hitPosition) =>
            {
                if (projectile.WasCrit)
                    TriggerSplashAt(mainEnemy, hitPosition, 30f);
            });
        }

        if (effects.Count > 0)
        {
            projectile.OnHitEffect = (mainEnemy, hitPosition) =>
            {
                foreach (var e in effects)
                    e(mainEnemy, hitPosition);
            };
        }

        GetProjectilesContainer().CallDeferred(Node.MethodName.AddChild, projectile);

        // Double Sampling: apply poison to nearest extra target within 30px
        if (_equipId == "double_sampling" && _data.HasPoison)
        {
            var spaceState = GetParent<Node2D>().GetWorld2D().DirectSpaceState;
            var query = new PhysicsShapeQueryParameters2D();
            var circle = new CircleShape2D { Radius = 30f };
            query.Shape = circle;
            query.Transform = new Transform2D(0, towerPos);
            query.CollideWithAreas = true;
            query.CollideWithBodies = false;
            query.CollisionMask = 1;
            var results = spaceState.IntersectShape(query);
            Enemy spreadTarget = null;
            float nearestDist = float.MaxValue;
            foreach (var result in results)
            {
                if (result["collider"].AsGodotObject() is Enemy enemy && enemy != target && !enemy.IsDead)
                {
                    float dist = enemy.GlobalPosition.DistanceTo(towerPos);
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        spreadTarget = enemy;
                    }
                }
            }
            if (spreadTarget != null)
            {
                ApplyEffect(spreadTarget, new PoisonEffectData
                {
                    Duration = _data.PoisonDuration * _poisonDurationMultiplier,
                    DamagePerTick = _data.PoisonDamagePerTick,
                });
            }
        }
    }

    private static void ApplyEffect(Enemy enemy, object effectData)
    {
        var ec = enemy.GetNode<StatusEffectComponent>("StatusEffectComponent");
        if (ec == null) return;
        if (effectData is PoisonEffectData p)
            ec.ApplyEffect(p);
        else if (effectData is SlowEffectData s)
            ec.ApplyEffect(s);
    }

    private void TriggerChain(Enemy mainEnemy, float chainDamage)
    {
        Enemy current = mainEnemy;
        int totalBounces = 1 + _extraChainBounces;
        float bounceRangeMult = SynergyManager.Instance?.IsSynergyActive("holy_fermentation_network") == true ? 1.3f : 1f;

        for (int b = 0; b < totalBounces; b++)
        {
            var spaceState = GetParent<Node2D>().GetWorld2D().DirectSpaceState;
            var query = new PhysicsShapeQueryParameters2D();
            var circle = new CircleShape2D { Radius = _data.ChainBounceRange * bounceRangeMult };

            query.Shape = circle;
            query.Transform = new Transform2D(0, current.GlobalPosition);
            query.CollideWithAreas = true;
            query.CollideWithBodies = false;
            query.CollisionMask = 1;

            var results = spaceState.IntersectShape(query);
            Enemy nearest = null;
            float nearestDist = float.MaxValue;

            foreach (var result in results)
            {
                if (result["collider"].AsGodotObject() is Enemy enemy && enemy != current && enemy != mainEnemy && !enemy.IsDead)
                {
                    float dist = enemy.GlobalPosition.DistanceTo(current.GlobalPosition);
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        nearest = enemy;
                    }
                }
            }

            if (nearest == null) break;

            nearest.TakeDamage(chainDamage);
            current = nearest;

            if (_data.HasPoison)
                ApplyEffect(nearest, new PoisonEffectData
                {
                    Duration = _data.PoisonDuration * _poisonDurationMultiplier,
                    DamagePerTick = _data.PoisonDamagePerTick,
                });
            if (_data.HasSlow)
                ApplyEffect(nearest, new SlowEffectData
                {
                    Duration = _data.SlowDuration * _slowDurationMultiplier,
                    SpeedMultiplier = _data.SlowMultiplier,
                });
        }
    }

    private Node GetProjectilesContainer()
    {
        if (LevelManager.Instance.CurrentLevelNode is BaseLevel level && level.ProjectilesContainer != null)
            return level.ProjectilesContainer;

        GD.PushWarning("AttackComponent: ProjectilesContainer not found, falling back to level root.");
        return LevelManager.Instance.CurrentLevelNode;
    }

    private void TriggerSplashDamage(Enemy mainEnemy, Vector2 hitPosition)
    {
        TriggerSplashAt(mainEnemy, hitPosition, _data.SplashRadius * _splashRadiusMultiplier);
    }

    private void TriggerSplashAt(Enemy mainEnemy, Vector2 hitPosition, float radius)
    {
        var spaceState = GetParent<Node2D>().GetWorld2D().DirectSpaceState;
        var query = new PhysicsShapeQueryParameters2D();
        var circle = new CircleShape2D { Radius = radius };

        query.Shape = circle;
        query.Transform = new Transform2D(0, hitPosition);
        query.CollideWithAreas = true;
        query.CollideWithBodies = false;
        query.CollisionMask = 1;

        var results = spaceState.IntersectShape(query);

        var effect = new SplashEffect();
        effect.Initialize(radius);
        effect.GlobalPosition = hitPosition;
        GetProjectilesContainer().AddChild(effect);

        foreach (var result in results)
        {
            if (result["collider"].AsGodotObject() is Enemy surroundingEnemy)
            {
                if (surroundingEnemy == mainEnemy) continue;
                surroundingEnemy.TakeDamage(_effectiveDamage);
            }
        }
    }
}