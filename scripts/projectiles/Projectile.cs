using Godot;
using System;

public partial class Projectile : Area2D
{
    [Export] public float Speed = 300f;
    [Export] public float Damage = 5f;

    public Enemy Target { get; set; }
    public Action<Enemy, Vector2> OnHitEffect { get; set; }
    public bool WasCrit { get; set; }
    public int PierceCount { get; set; }
    private bool _returningToPool;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }

    public void Initialize(Enemy target, float damage)
    {
        Target = target;
        Damage = damage;
        WasCrit = false;
        PierceCount = 0;
        _returningToPool = false;
        Visible = true;
        Monitoring = true;
        Monitorable = true;
        SetPhysicsProcess(true);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Target == null || !IsInstanceValid(Target) || Target.IsDead)
        {
            SetPhysicsProcess(false);
            Visible = false;
            ReturnToPool();
            return;
        }

        Vector2 direction = (Target.GlobalPosition - GlobalPosition).Normalized();
        GlobalPosition += direction * Speed * (float)delta;
        Rotation = direction.Angle();
    }

    private void OnAreaEntered(Area2D area)
    {
        if (_returningToPool || area != Target) return;

        OnHitTarget(Target);
    }

    protected virtual void OnHitTarget(Enemy mainEnemy)
    {
        SetPhysicsProcess(false);
        mainEnemy.TakeDamage(Damage);
        OnHitEffect?.Invoke(mainEnemy, mainEnemy.GlobalPosition);

        if (PierceCount > 0)
        {
            PierceCount--;
            FindNextTarget(mainEnemy);
        }
        else
        {
            SetDeferred("monitoring", false);
            Visible = false;
            ReturnToPool();
        }
    }

    private void FindNextTarget(Enemy exclude)
    {
        var spaceState = GetWorld2D().DirectSpaceState;
        var query = new PhysicsShapeQueryParameters2D();
        var circle = new CircleShape2D { Radius = 200f };
        query.Shape = circle;
        query.Transform = new Transform2D(0, GlobalPosition);
        query.CollideWithAreas = true;
        query.CollideWithBodies = false;
        query.CollisionMask = 1;

        var results = spaceState.IntersectShape(query);
        Enemy nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var result in results)
        {
            if (result["collider"].AsGodotObject() is Enemy enemy && enemy != exclude && !enemy.IsDead)
            {
                float dist = enemy.GlobalPosition.DistanceTo(GlobalPosition);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = enemy;
                }
            }
        }

        if (nearest != null)
        {
            Target = nearest;
            SetPhysicsProcess(true);
        }
        else
        {
            Visible = false;
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (_returningToPool)
            return;
        _returningToPool = true;
        CallDeferred(nameof(DeferredReturnToPool));
    }

    private void DeferredReturnToPool()
    {
        _returningToPool = false;
        if (PoolManager.Instance != null)
            PoolManager.Instance.Return(this);
        else
            QueueFree();
    }
}
