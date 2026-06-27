using Godot;
using System;

public partial class AttackComponent : Node
{
    private TowerData _data;
    private float _cooldown;
    public override void _Ready()
    {
        if (GetParent() is not Node2D)
            GD.PushError($"AttackComponent: parent must be a Node2D (got {GetParent()?.GetClass()}).");
    }

    public void Setup(TowerData data)
    {
        _data = data;
        _cooldown = 0f;
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
        _cooldown = 1f / _data.FireRate;
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
        GD.Print($"[AttackComponent] Fire — tower={_data.TowerName}, target={target.Name}, pos={towerPos}");
        var projectile = ProjectileFactory.Create(_data.ProjectileScene, _data.Damage, target, towerPos);

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

        LevelManager.Instance.CurrentLevelNode.CallDeferred(Node.MethodName.AddChild, projectile);
    }


    private void TriggerSplashDamage(Enemy mainEnemy, Vector2 hitPosition)
    {
        var spaceState = GetParent<Node2D>().GetWorld2D().DirectSpaceState;
        var query = new PhysicsShapeQueryParameters2D();
        var circle = new CircleShape2D { Radius = _data.SplashRadius };

        query.Shape = circle;
        query.Transform = new Transform2D(0, hitPosition);
        query.CollideWithAreas = true;
        query.CollideWithBodies = false;
        query.CollisionMask = 1;

        var results = spaceState.IntersectShape(query);

        foreach (var result in results)
        {
            if (result["collider"].AsGodotObject() is Enemy surroundingEnemy)
            {
                if (surroundingEnemy == mainEnemy) continue;
                surroundingEnemy.TakeDamage(_data.Damage);
            }
        }
    }
}