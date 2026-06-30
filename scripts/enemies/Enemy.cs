using Godot;

public partial class Enemy : Area2D
{
    private EnemyData _data;
    private Health _health;
    private MovementComponent _movement;
    private StatusEffectComponent _statusEffects;
    private HealthBar _healthBar;
    private Curve2D _pendingCurve;
    private bool _signalsConnected;
    private bool _returningToPool;
    private float _statMultiplier = 1f;
    public bool IsDead { get; private set; }

    public override void _Ready()
    {
        _health = GetNode<Health>("Health");
        _movement = GetNode<MovementComponent>("MovementComponent");
        _statusEffects = GetNode<StatusEffectComponent>("StatusEffectComponent");
        _healthBar = new HealthBar();
        _healthBar.Name = "HealthBar";
        AddChild(_healthBar);
        ConnectSignals();

        if (_data != null)
            ApplyData();
    }

    private void ConnectSignals()
    {
        if (_signalsConnected) return;
        if (_health == null || _movement == null) return;
        _health.HealthChanged += OnHealthChanged;
        _health.Died += OnDied;
        _health.DamageTaken += OnDamageTaken;
        _movement.ReachedEnd += OnReachedEnd;
        _signalsConnected = true;
    }

    private void DisconnectSignals()
    {
        if (!_signalsConnected) return;
        _health.HealthChanged -= OnHealthChanged;
        _health.Died -= OnDied;
        _health.DamageTaken -= OnDamageTaken;
        _movement.ReachedEnd -= OnReachedEnd;
        _signalsConnected = false;
    }

    public void Initialize(EnemyData data, Curve2D curve, float statMultiplier = 1f)
    {
        _returningToPool = false;
        IsDead = false;
        _data = data;
        _statMultiplier = statMultiplier;
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
        _health.Setup(_data.MaxHealth * _statMultiplier);
        _healthBar.SetHealth(_health.GetCurrentHealth(), _health.MaxHealth);

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

    private void OnHealthChanged(float current, float max)
    {
        _healthBar?.SetHealth(current, max);
    }

    public float GetCurrentHealth() => _health?.GetCurrentHealth() ?? 0f;

    private void OnDamageTaken(float amount)
    {
        var container = LevelManager.Instance.CurrentLevelNode is BaseLevel bl && bl.ProjectilesContainer != null
            ? bl.ProjectilesContainer
            : LevelManager.Instance.CurrentLevelNode;

        if (container == null) return;

        var popup = new DamagePopup();
        popup.GlobalPosition = GlobalPosition;
        container.AddChild(popup);
        popup.ShowDamage(amount, Colors.White);
    }

    private void OnReachedEnd()
    {
        IsDead = true;
        EventBus.Instance.EmitSignal(EventBus.SignalName.EnemyReachedEnd, Mathf.RoundToInt(_data.DamageToPlayer * _statMultiplier));
        ReturnToPool();
    }

    private void OnDied()
    {
        IsDead = true;
        EventBus.Instance.EmitSignal(EventBus.SignalName.EnemyDied, Mathf.RoundToInt(_data.RewardGold * _statMultiplier));
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (_returningToPool)
            return;
        _returningToPool = true;
        _statusEffects?.ClearEffects();
        DisconnectSignals();
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
