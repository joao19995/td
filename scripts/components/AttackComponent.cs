using Godot;
using System;

public partial class AttackComponent : Node
{
    private TowerData _data;
    private float _effectiveDamage;
    private float _effectiveFireRate;
    private float _cooldown;
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

    public override void _Process(double delta)
    {
        if (_cooldown > 0f)
            _cooldown -= (float)delta;
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
        var projectile = ProjectileFactory.Create(_data.ProjectileScene, _effectiveDamage, target, towerPos);

        if (_data.HasSplash)
        {
            projectile.OnHitEffect = (mainEnemy, hitPosition) => TriggerSplashDamage(mainEnemy, hitPosition);
        }
        else if (_data.HasPoison)
        {
            projectile.OnHitEffect = (mainEnemy, hitPosition) =>
            {
                var effectComponent = mainEnemy.GetNode<StatusEffectComponent>("StatusEffectComponent");
                if (effectComponent != null)
                {
                    var poisonData = new PoisonEffectData
                    {
                        Duration = _data.PoisonDuration,
                        DamagePerTick = _data.PoisonDamagePerTick,
                    };
                    effectComponent.ApplyEffect(poisonData);
                }
            };
        }
        else if (_data.HasSlow)
        {
            projectile.OnHitEffect = (mainEnemy, hitPosition) =>
            {
                var effectComponent = mainEnemy.GetNode<StatusEffectComponent>("StatusEffectComponent");
                if (effectComponent != null)
                {
                    var slowData = new SlowEffectData
                    {
                        Duration = _data.SlowDuration,
                        SpeedMultiplier = _data.SlowMultiplier,
                    };
                    effectComponent.ApplyEffect(slowData);
                }
            };
        }

        GetProjectilesContainer().CallDeferred(Node.MethodName.AddChild, projectile);
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
        GD.Print($"[Splash] TriggerSplashDamage — pos={hitPosition}, radius={_data.SplashRadius}");

        var spaceState = GetParent<Node2D>().GetWorld2D().DirectSpaceState;
        var query = new PhysicsShapeQueryParameters2D();
        var circle = new CircleShape2D { Radius = _data.SplashRadius };

        query.Shape = circle;
        query.Transform = new Transform2D(0, hitPosition);
        query.CollideWithAreas = true;
        query.CollideWithBodies = false;
        query.CollisionMask = 1;

        var results = spaceState.IntersectShape(query);
        GD.Print($"[Splash] IntersectShape — results={results.Count}");

        var effect = new SplashEffect();
        effect.Initialize(_data.SplashRadius);
        effect.GlobalPosition = hitPosition;
        GetProjectilesContainer().AddChild(effect);

        int hitCount = 0;
        foreach (var result in results)
        {
            if (result["collider"].AsGodotObject() is Enemy surroundingEnemy)
            {
                if (surroundingEnemy == mainEnemy) continue;
                surroundingEnemy.TakeDamage(_data.Damage);
                hitCount++;
            }
        }
        GD.Print($"[Splash] Hit {hitCount} surrounding enemies, main={mainEnemy.Name}");
    }
}