using Godot;
using System.Collections.Generic;

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
    private float _antiBuffScanTimer;
    private readonly HashSet<Tower> _affectedTowers = new();
    public bool IsDead { get; private set; }
    public bool IsBoss => _data?.IsBoss ?? false;
    public bool IsHeavy => _data?.IsHeavy ?? false;

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
    public float MaxHealth => _health?.MaxHealth ?? 1f;
    public float HealthPercent => GetCurrentHealth() / MaxHealth;

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

    public override void _Process(double delta)
    {
        if (_data == null || !_data.HasAntiBuffAura || IsDead) return;

        _antiBuffScanTimer -= (float)delta;
        if (_antiBuffScanTimer > 0f) return;
        _antiBuffScanTimer = 0.5f;

        var myPos = GlobalPosition;
        var allTowers = GetTree().GetNodesInGroup("towers");
        var currentInRange = new HashSet<Tower>();

        foreach (var node in allTowers)
        {
            if (node is Tower tower && GodotObject.IsInstanceValid(tower))
            {
                float dist = tower.GlobalPosition.DistanceTo(myPos);
                if (dist <= _data.AntiBuffAuraRadius)
                    currentInRange.Add(tower);
            }
        }

        foreach (var tower in currentInRange)
        {
            if (!_affectedTowers.Contains(tower) && GodotObject.IsInstanceValid(tower))
            {
                _affectedTowers.Add(tower);
                Tower.AddAntiBuff(tower);
                tower.RefreshStats();
            }
        }

        var left = new List<Tower>();
        foreach (var tower in _affectedTowers)
        {
            if (!currentInRange.Contains(tower))
                left.Add(tower);
        }
        foreach (var tower in left)
        {
            _affectedTowers.Remove(tower);
            Tower.RemoveAntiBuff(tower);
            if (GodotObject.IsInstanceValid(tower))
                tower.RefreshStats();
        }
    }

    private void RemoveAllAntiBuffEffects()
    {
        foreach (var tower in _affectedTowers)
        {
            Tower.RemoveAntiBuff(tower);
            if (IsInstanceValid(tower))
                tower.RefreshStats();
        }
        _affectedTowers.Clear();
    }

    private void OnReachedEnd()
    {
        IsDead = true;
        RemoveAllAntiBuffEffects();
        EventBus.Instance.EmitSignal(EventBus.SignalName.EnemyReachedEnd, Mathf.RoundToInt(_data.DamageToPlayer * _statMultiplier));
        ReturnToPool();
    }

    private void OnDied()
    {
        IsDead = true;
        RemoveAllAntiBuffEffects();
        EventBus.Instance.EmitSignal(EventBus.SignalName.EnemyDied, Mathf.RoundToInt(_data.RewardGold * _statMultiplier));
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (_returningToPool)
            return;
        _returningToPool = true;
        RemoveAllAntiBuffEffects();
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
