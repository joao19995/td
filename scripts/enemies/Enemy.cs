using Godot;

public partial class Enemy : Area2D
{
    private EnemyData _data;
    private Health _health;
    private MovementComponent _movement;
    private StatusEffectComponent _statusEffects;
    private Curve2D _pendingCurve;
    private bool _signalsConnected;
    private bool _returningToPool;

    public override void _Ready()
    {
        _health = GetNode<Health>("Health");
        _movement = GetNode<MovementComponent>("MovementComponent");
        _statusEffects = GetNode<StatusEffectComponent>("StatusEffectComponent");
        ConnectSignals();

        if (_data != null)
            ApplyData();
    }

    private void ConnectSignals()
    {
        if (_signalsConnected) return;
        if (_health == null || _movement == null) return;
        _health.Died += OnDied;
        _movement.ReachedEnd += OnReachedEnd;
        _signalsConnected = true;
    }

    private void DisconnectSignals()
    {
        if (!_signalsConnected) return;
        _health.Died -= OnDied;
        _movement.ReachedEnd -= OnReachedEnd;
        _signalsConnected = false;
    }

    public void Initialize(EnemyData data, Curve2D curve)
    {
        _returningToPool = false;
        _data = data;
        GD.Print($"[Enemy] Initialize — {data.EnemyName}, sigConnected={_signalsConnected}, hasParent={GetParent() != null}");
        ConnectSignals();

        if (_movement != null)
            _movement.Initialize(curve, data.MoveSpeed);
        else
            _pendingCurve = curve;

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

        Visible = true;
        SetProcess(true);
        SetPhysicsProcess(true);
    }

    public void TakeDamage(float amount) => _health?.TakeDamage(amount);

    public float GetCurrentHealth() => _health?.GetCurrentHealth() ?? 0f;

    private void OnReachedEnd()
    {
        GD.Print($"[Enemy] OnReachedEnd — {_data?.EnemyName}, returning={_returningToPool}");
        EventBus.Instance.EmitSignal(EventBus.SignalName.EnemyReachedEnd, _data.DamageToPlayer);
        ReturnToPool();
    }

    private void OnDied()
    {
        GD.Print($"[Enemy] OnDied — {_data?.EnemyName}, returning={_returningToPool}");
        EventBus.Instance.EmitSignal(EventBus.SignalName.EnemyDied, _data.RewardGold);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (_returningToPool)
        {
            GD.Print($"[Enemy] ReturnToPool blocked — already returning");
            return;
        }
        _returningToPool = true;
        _statusEffects?.ClearEffects();
        DisconnectSignals();
        GD.Print($"[Enemy] ReturnToPool — deferred");
        CallDeferred(nameof(DeferredReturnToPool));
    }

    private void DeferredReturnToPool()
    {
        _returningToPool = false;
        GD.Print($"[Enemy] DeferredReturnToPool — poolAvailable={PoolManager.Instance != null}");
        if (PoolManager.Instance != null)
            PoolManager.Instance.Return(this);
        else
            QueueFree();
    }
}

