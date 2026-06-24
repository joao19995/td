using Godot;

public partial class Enemy : Area2D
{
    private EnemyData _data;
    private Health _health;
    private MovementComponent _movement;

    // Held until _Ready() wires up _movement (Initialize may be called before _Ready).
    private Curve2D _pendingCurve;

    public override void _Ready()
    {
        _health = GetNode<Health>("Health");
        _health.Died += OnDied;

        _movement = GetNode<MovementComponent>("MovementComponent");
        _movement.ReachedEnd += OnReachedEnd;

        if (_data != null)
            ApplyData();
    }

    public void Initialize(EnemyData data, Curve2D curve)
    {
        _data = data;

        if (_movement != null)
        {
            _movement.Initialize(curve, data.MoveSpeed);
        }
        else
        {
            // _Ready hasn't run yet; store for later.
            _pendingCurve = curve;
        }

        if (_health != null)
            ApplyData();
    }

    private void ApplyData()
    {
        _health.Setup(_data.MaxHealth);

        if (_pendingCurve != null)
        {
            _movement.Initialize(_pendingCurve, _data.MoveSpeed);
            _pendingCurve = null;
        }

        if (_data.Sprite != null)
            GetNode<Sprite2D>("Sprite2D").Texture = _data.Sprite;
    }

    public void TakeDamage(float amount) => _health.TakeDamage(amount);

    /// <summary>Returns the enemy's current health (used by TargetingComponent for Strongest strategy).</summary>
    public float GetCurrentHealth() => _health.GetCurrentHealth();

    private void OnReachedEnd()
    {
        EventBus.Instance.EmitSignal(EventBus.SignalName.EnemyReachedEnd, _data.DamageToPlayer);
        QueueFree();
    }

    private void OnDied()
    {
        EventBus.Instance.EmitSignal(EventBus.SignalName.EnemyDied, _data.RewardGold);
        QueueFree();
    }
}
