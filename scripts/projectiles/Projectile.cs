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
        GD.Print($"[Projectile] Initialize — target={target?.Name}, dmg={damage}");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Target == null || !IsInstanceValid(Target))
        {
            GD.Print($"[Projectile] Target lost — returning");
            ReturnToPool();
            return;
        }

        Vector2 direction = (Target.GlobalPosition - GlobalPosition).Normalized();
        GlobalPosition += direction * Speed * (float)delta;
        Rotation = direction.Angle();
    }

    private void OnAreaEntered(Area2D area)
    {
        GD.Print($"[Projectile] AreaEntered — area={area.Name}, isTarget={area == Target}, _returning={_returningToPool}");
        if (area == Target)
        {
            OnHitTarget(Target);
        }
    }

    protected virtual void OnHitTarget(Enemy mainEnemy)
    {
        GD.Print($"[Projectile] OnHitTarget — dmg={Damage}");
        mainEnemy.TakeDamage(Damage);
        OnHitEffect?.Invoke(mainEnemy, GlobalPosition);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (_returningToPool)
        {
            GD.Print($"[Projectile] ReturnToPool blocked — already returning");
            return;
        }
        _returningToPool = true;
        GD.Print($"[Projectile] ReturnToPool — deferred");
        CallDeferred(nameof(DeferredReturnToPool));
    }

    private void DeferredReturnToPool()
    {
        _returningToPool = false;
        GD.Print($"[Projectile] DeferredReturnToPool — poolAvailable={PoolManager.Instance != null}");
        if (PoolManager.Instance != null)
            PoolManager.Instance.Return(this);
        else
            QueueFree();
    }
}