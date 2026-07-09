using Godot;
using System.Collections.Generic;

public partial class Enemy : Area2D
{
    private EnemyData _data;
    private Health _health;
    private MovementComponent _movement;
    private StatusEffectComponent _statusEffects;
    private HealthBar _healthBar;
    private Label _healthLabel;
    private Sprite2D _sprite;
    private Curve2D _pendingCurve;
    private bool _signalsConnected;
    private bool _returningToPool;
    private float _hpMultiplier = 1f;
    private float _dmgMultiplier = 1f;
    private float _goldMultiplier = 1f;
    private float _speedMultiplier = 1f;
    private bool _isElite;
    private float _antiBuffScanTimer;
    private readonly HashSet<Tower> _affectedTowers = new();
    public bool IsDead { get; private set; }
    public bool IsBoss => _data?.IsBoss ?? false;
    public bool IsHeavy => _data?.IsHeavy ?? false;
    public bool IsElite => _isElite;

    public override void _Ready()
    {
        _health = GetNode<Health>("Health");
        _movement = GetNode<MovementComponent>("MovementComponent");
        _statusEffects = GetNode<StatusEffectComponent>("StatusEffectComponent");
        _sprite = GetNode<Sprite2D>("Sprite2D");
        _statusEffects.Setup(_health, _sprite, _movement);
        _healthBar = new HealthBar();
        _healthBar.Name = "HealthBar";
        AddChild(_healthBar);

        if (OS.IsDebugBuild())
        {
            _healthLabel = new Label();
            _healthLabel.Name = "HealthLabel";
            _healthLabel.LabelSettings = new LabelSettings { FontSize = 5, OutlineSize = 1, OutlineColor = Colors.Black };
            _healthLabel.HorizontalAlignment = HorizontalAlignment.Center;
            _healthLabel.Position = new Vector2(-8f, -20f);
            _healthLabel.CustomMinimumSize = new Vector2(16f, 0f);
            AddChild(_healthLabel);
        }

        ConnectSignals();

        if (_data != null)
            ApplyData();

        if (_speedMultiplier != 1f)
            _movement?.SetBaseSpeedMultiplier(_speedMultiplier);
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

    public void Initialize(EnemyData data, Curve2D curve, float statMultiplier = 1f, bool isElite = false)
    {
        _returningToPool = false;
        IsDead = false;
        _data = data;
        _hpMultiplier = statMultiplier;
        _dmgMultiplier = statMultiplier;
        _goldMultiplier = statMultiplier;
        _speedMultiplier = 1f;
        _isElite = isElite;
        ConnectSignals();

        if (_movement != null)
        {
            _movement.Initialize(curve, data.MoveSpeed);
            _movement.SetBaseSpeedMultiplier(_speedMultiplier);
        }
        else
            _pendingCurve = curve;

        if (_health != null)
            ApplyData();
    }

    public void SetModifierMultipliers(float hpMult = 1f, float dmgMult = 1f, float goldMult = 1f)
    {
        _hpMultiplier *= hpMult;
        _dmgMultiplier *= dmgMult;
        _goldMultiplier *= goldMult;

        if (_health != null)
            ApplyData();
    }

    public void SetSpeedMultiplier(float mult)
    {
        _speedMultiplier = mult;

        if (_movement != null)
            _movement.SetBaseSpeedMultiplier(_speedMultiplier);
    }

    private void ApplyData()
    {
        float eliteHpMult = _isElite ? GameBalance.EliteHpMultiplier : 1f;
        _health.Setup(_data.MaxHealth * _hpMultiplier * eliteHpMult);
        float initialHp = _health.GetCurrentHealth();
        _healthBar.SetHealth(initialHp, _health.MaxHealth);
        if (_healthLabel != null)
            _healthLabel.Text = Mathf.RoundToInt(initialHp).ToString();

        if (_pendingCurve != null)
        {
            _movement.Initialize(_pendingCurve, _data.MoveSpeed);
            _pendingCurve = null;
        }

        _movement?.SetBaseSpeedMultiplier(_speedMultiplier);

        if (_data.Sprite != null && _sprite != null)
            _sprite.Texture = _data.Sprite;

        Visible = true;
        SetProcess(true);
        SetPhysicsProcess(true);
    }

    public void TakeDamage(float amount) => _health?.TakeDamage(amount);

    private void OnHealthChanged(float current, float max)
    {
        _healthBar?.SetHealth(current, max);
        if (_healthLabel != null)
            _healthLabel.Text = Mathf.RoundToInt(current).ToString();
    }

    public float GetCurrentHealth() => _health?.GetCurrentHealth() ?? 0f;
    public float MaxHealth => _health?.MaxHealth ?? 1f;
    public float HealthPercent => GetCurrentHealth() / MaxHealth;
    public float GetProgressRatio() => _movement?.GetProgressRatio() ?? 0f;

    public void ApplyStatusEffect(object effectData)
    {
        if (effectData is StatusEffectData sd)
            _statusEffects?.ApplyEffect(sd);
    }

    public void ResetForPool()
    {
        _statusEffects?.ClearEffects();
        _healthBar?.Reset();
    }

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
        _antiBuffScanTimer = GameBalance.AuraScanInterval;

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
        float eliteDmgMult = _isElite ? GameBalance.EliteDamageMultiplier : 1f;
        EventBus.Instance.EmitSignal(EventBus.SignalName.EnemyReachedEnd, Mathf.RoundToInt(_data.DamageToPlayer * _dmgMultiplier * eliteDmgMult));
        ReturnToPool();
    }

    private void OnDied()
    {
        IsDead = true;
        RemoveAllAntiBuffEffects();
        float eliteGoldMult = _isElite ? GameBalance.EliteGoldMultiplier : 1f;
        EventBus.Instance.EmitSignal(EventBus.SignalName.EnemyDied, Mathf.RoundToInt(_data.RewardGold * _goldMultiplier * eliteGoldMult));
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
