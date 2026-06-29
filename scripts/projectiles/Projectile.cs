using Godot;
using System;

public partial class Projectile : Area2D
{
    [Export] public float Speed = 300f;
    [Export] public float Damage = 5f;

    public Enemy Target { get; set; }
    public Action<Enemy, Vector2> OnHitEffect { get; set; }
    private bool _returningToPool;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }

    public void Initialize(Enemy target, float damage)
    {
        Target = target;
        Damage = damage;
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

        SetDeferred("monitoring", false);
        OnHitTarget(Target);
    }

    protected virtual void OnHitTarget(Enemy mainEnemy)
    {
        SetPhysicsProcess(false);
        Visible = false;
        mainEnemy.TakeDamage(Damage);
        OnHitEffect?.Invoke(mainEnemy, mainEnemy.GlobalPosition);
        ReturnToPool();
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
