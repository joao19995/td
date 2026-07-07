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
    private const int _enemyCollisionMask = 1;
    private CollisionShape2D _collisionShape;

    [Export] private NodePath _collisionShapePath = new NodePath("CollisionShape2D");

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
        _collisionShape = GetNode<CollisionShape2D>(_collisionShapePath);
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

            if (Target != null && !_returningToPool && IsOverlappingTarget())
                OnHitTarget(Target);
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
        var query = GetPhysicsQuery(new CircleShape2D { Radius = 200f });
        var results = GetWorld2D().DirectSpaceState.IntersectShape(query);
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

    private PhysicsShapeQueryParameters2D GetPhysicsQuery(CircleShape2D shape)
    {
        return new PhysicsShapeQueryParameters2D
        {
            Shape = shape,
            Transform = new Transform2D(0, GlobalPosition),
            CollideWithAreas = true,
            CollideWithBodies = false,
            CollisionMask = _enemyCollisionMask,
        };
    }

    private bool IsOverlappingTarget()
    {
        if (_collisionShape?.Shape is not CircleShape2D circle) return false;

        var query = GetPhysicsQuery(circle);
        var results = GetWorld2D().DirectSpaceState.IntersectShape(query);
        foreach (var result in results)
        {
            if (result["collider"].AsGodotObject() is Enemy enemy && enemy == Target)
                return true;
        }
        return false;
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
