using Godot;
using System;
using System.Collections.Generic;

public partial class AttackComponent : Node
{
    private TowerData _data;
    private EquipData _equipData;
    private float _effectiveDamage;
    private float _effectiveFireRate;
    private float _effectiveCritChance;
    private float _effectiveCritMultiplier;
    private float _cooldown;
    private float _splashRadiusMultiplier = 1f;
    private float _slowDurationMultiplier = 1f;
    private float _poisonDurationMultiplier = 1f;
    private int _extraChainBounces;
    private float _judgmentSealCooldown;
    private float _judgmentProtocolCooldown;
    private Action<Enemy, Vector2> _precomputedHitEffects;
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

    public void SetEquipData(EquipData data)
    {
        _equipData = data;
    }

    public void SetEquipModifiers(float splashRadiusMult, float slowDurationMult, float poisonDurationMult, int extraChainBounces)
    {
        _splashRadiusMultiplier = splashRadiusMult;
        _slowDurationMultiplier = slowDurationMult;
        _poisonDurationMultiplier = poisonDurationMult;
        _extraChainBounces = extraChainBounces;
    }

    public void Refresh()
    {
        RebuildEffects();
    }

    private void RebuildEffects()
    {
        var effects = new List<Action<Enemy, Vector2>>();

        // Todas as structs de efeito (PoisonEffectData, SlowEffectData) são criadas com
        // "new" dentro do closure abaixo — uma instância nova por invocação, nunca
        // partilhada entre alvos. Os fields de duração/multiplicador (ex: _slowDurationMultiplier,
        // _poisonDurationMultiplier, _effectiveDamage) são lidos através de this no momento da
        // invocação, não no momento do RebuildEffects. Zero risco de referência partilhada.

        if (_data != null)
        {
            if (_data.HasPoison)
                effects.Add((mainEnemy, _) => ApplyEffect(mainEnemy, new PoisonEffectData
                {
                    Duration = _data.PoisonDuration * _poisonDurationMultiplier,
                    DamagePerTick = _data.PoisonDamagePerTick
                        * (1f + (RunState.Instance?.TrinketStatusStrengthBonusPercent ?? 0f)
                               + (_equipData?.PoisonDamagePercentBonus ?? 0f)),
                }));

            if (_data.HasSlow)
                effects.Add((mainEnemy, _) => ApplyEffect(mainEnemy, new SlowEffectData
                {
                    Duration = _data.SlowDuration * _slowDurationMultiplier,
                    SpeedMultiplier = _data.SlowMultiplier
                        / (1f + (RunState.Instance?.TrinketStatusStrengthBonusPercent ?? 0f)),
                }));
        }

        _precomputedHitEffects = effects.Count > 0
            ? (mainEnemy, hitPosition) =>
            {
                foreach (var e in effects)
                    e(mainEnemy, hitPosition);
            }
            : null;
    }

    public override void _Process(double delta)
    {
        if (_cooldown > 0f)
            _cooldown -= (float)delta;
        if (_judgmentSealCooldown > 0f)
            _judgmentSealCooldown -= (float)delta;
        if (_judgmentProtocolCooldown > 0f)
            _judgmentProtocolCooldown -= (float)delta;
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
        bool wasCrit = RollCrit(ref damage);

        if (TryInstaKill(target, wasCrit)) return;

        ApplyDamageModifiers(target, ref damage);

        SpawnProjectile(target, towerPos, damage, wasCrit);
        TrySpreadPoison(target, towerPos);
    }

    private bool RollCrit(ref float damage)
    {
        if (!_data.HasCrit || !(_rng.NextDouble() < _effectiveCritChance))
            return false;
        damage *= _effectiveCritMultiplier;
        return true;
    }

    private bool TryInstaKill(Enemy target, bool wasCrit)
    {
        if (wasCrit && _judgmentProtocolCooldown <= 0f
            && SynergyManager.Instance?.IsSynergyActive("crust_judgment_protocol") == true
            && target.HealthPercent <= 0.5f)
        {
            target.TakeDamage(9999f);
            _judgmentProtocolCooldown = GameBalance.JudgmentProtocolCooldown;
            return true;
        }

        float execThreshold = _equipData?.ExecuteThresholdPercent ?? 0f;
        if (execThreshold > 0f && _judgmentSealCooldown <= 0f && target.HealthPercent <= execThreshold)
        {
            target.TakeDamage(9999f);
            _judgmentSealCooldown = _equipData.ExecuteCooldownSeconds > 0f ? _equipData.ExecuteCooldownSeconds : GameBalance.JudgmentSealCooldown;
            return true;
        }

        return false;
    }

    private void ApplyDamageModifiers(Enemy target, ref float damage)
    {
        if (_data.HasExecute && target.HealthPercent <= _data.ExecuteThresholdHPPercent)
            damage *= _data.ExecuteMultiplier;

        if (_data.HasExecute && (target.IsBoss || target.IsHeavy))
            damage *= _data.EliteBonusMultiplier;

        float eliteDmgBonus = _equipData?.EliteDamagePercentBonus ?? 0f;
        if (eliteDmgBonus > 0f && (target.IsBoss || target.IsHeavy))
            damage *= 1f + eliteDmgBonus;

        float shopHeavyPct = RunState.Instance?.ShopHeavyDamageBonusPercent ?? 0f;
        if (shopHeavyPct > 0f && (target.IsBoss || target.IsHeavy))
            damage *= 1f + shopHeavyPct;

        float basicDmgPct = RunState.Instance?.TrinketBasicDamagePercentBonus ?? 0f;
        if (basicDmgPct > 0f && !target.IsBoss && !target.IsHeavy)
            damage *= 1f + basicDmgPct;
    }

    private void SpawnProjectile(Enemy target, Vector2 towerPos, float damage, bool wasCrit)
    {
        var projectile = ProjectileFactory.Create(_data.ProjectileScene, damage, target, towerPos);
        projectile.WasCrit = wasCrit;
        projectile.PierceCount = _equipData?.PierceBonus ?? 0;

        Action<Enemy, Vector2> hitEffects = _precomputedHitEffects;
        float capturedDamage = damage;

        if (_data.HasSplash)
        {
            var prev = hitEffects;
            hitEffects = (mainEnemy, hitPosition) =>
            {
                prev?.Invoke(mainEnemy, hitPosition);
                TriggerSplashDamage(mainEnemy, hitPosition, capturedDamage);
            };
        }

        if (_data.HasChain)
        {
            var prev = hitEffects;
            hitEffects = (mainEnemy, hitPosition) =>
            {
                prev?.Invoke(mainEnemy, hitPosition);
                TriggerChain(mainEnemy, capturedDamage * _data.ChainBounceDamageMultiplier);
            };
        }

        float splashCritRadius = _equipData?.SplashOnCritRadius ?? 0f;
        if (splashCritRadius > 0f)
        {
            var prev = hitEffects;
            float capturedRadius = splashCritRadius;
            hitEffects = (mainEnemy, hitPosition) =>
            {
                prev?.Invoke(mainEnemy, hitPosition);
                if (projectile.WasCrit)
                    TriggerSplashAt(mainEnemy, hitPosition, capturedRadius, capturedDamage);
            };
        }

        projectile.OnHitEffect = hitEffects;
        GetProjectilesContainer().CallDeferred(Node.MethodName.AddChild, projectile);
    }

    private void TrySpreadPoison(Enemy target, Vector2 towerPos)
    {
        float poisonSpreadRadius = _equipData?.PoisonSpreadRadius ?? 0f;
        if (poisonSpreadRadius <= 0f || !_data.HasPoison) return;

        var spaceState = GetParent<Node2D>().GetWorld2D().DirectSpaceState;
        var query = new PhysicsShapeQueryParameters2D();
        var circle = new CircleShape2D { Radius = poisonSpreadRadius };
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
        if (spreadTarget == null) return;

        float strengthMult = 1f + (RunState.Instance?.TrinketStatusStrengthBonusPercent ?? 0f)
                               + (_equipData?.PoisonDamagePercentBonus ?? 0f);
        ApplyEffect(spreadTarget, new PoisonEffectData
        {
            Duration = _data.PoisonDuration * _poisonDurationMultiplier,
            DamagePerTick = _data.PoisonDamagePerTick * strengthMult,
        });
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
        int totalBounces = _data.ChainBounceCount + _extraChainBounces;
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
            {
                float strengthMult = 1f + (RunState.Instance?.TrinketStatusStrengthBonusPercent ?? 0f)
                                       + (_equipData?.PoisonDamagePercentBonus ?? 0f);
                ApplyEffect(nearest, new PoisonEffectData
                {
                    Duration = _data.PoisonDuration * _poisonDurationMultiplier,
                    DamagePerTick = _data.PoisonDamagePerTick * strengthMult,
                });
            }
            if (_data.HasSlow)
                ApplyEffect(nearest, new SlowEffectData
                {
                    Duration = _data.SlowDuration * _slowDurationMultiplier,
                    SpeedMultiplier = _data.SlowMultiplier
                        / (1f + (RunState.Instance?.TrinketStatusStrengthBonusPercent ?? 0f)),
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

    private void TriggerSplashDamage(Enemy mainEnemy, Vector2 hitPosition, float damage)
    {
        TriggerSplashAt(mainEnemy, hitPosition, _data.SplashRadius * _splashRadiusMultiplier, damage);
    }

    private void TriggerSplashAt(Enemy mainEnemy, Vector2 hitPosition, float radius, float damage)
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
                surroundingEnemy.TakeDamage(damage);
            }
        }
    }
}