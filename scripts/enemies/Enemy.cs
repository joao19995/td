using Godot;

public partial class Enemy : Area2D
{
    private EnemyData _data;
    private Health _health;
    private MovementComponent _movement;
    private Timer _poisonTimer;
    private float _poisonTimeLeft;
    private float _poisonDamagePerTick;
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

    public void TakeDamage(float amount) => _health?.TakeDamage(amount);

    /// <summary>Returns the enemy's current health (used by TargetingComponent for Strongest strategy).</summary>
    public float GetCurrentHealth() => _health?.GetCurrentHealth() ?? 0f;

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
    public void ApplyPoison(float damagePerTick, float duration)
    {
        var sprite = GetNode<Sprite2D>("Sprite2D");

        // 1. Fica verde IMEDIATAMENTE
        if (sprite != null)
        {
            sprite.Modulate = Colors.Green;
        }

        // Atualizamos os dados do veneno atual
        _poisonDamagePerTick = damagePerTick;
        _poisonTimeLeft = duration;

        // 2. Se o Timer JÁ EXISTIR, não criamos outro! Apenas damos "refresh" no tempo
        if (_poisonTimer != null && IsInstanceValid(_poisonTimer))
        {
            return; // O tempo já foi reiniciado acima através do _poisonTimeLeft
        }

        // 3. Se NÃO existir, criamos o Timer pela primeira vez
        _poisonTimer = new Timer();
        _poisonTimer.Name = "PoisonTimer";
        _poisonTimer.WaitTime = 1.0f;
        _poisonTimer.Autostart = true;
        AddChild(_poisonTimer);

        _poisonTimer.Timeout += () =>
        {
            if (!IsInstanceValid(this)) return;

            // Aplica o dano a cada segundo
            TakeDamage(_poisonDamagePerTick);

            _poisonTimeLeft -= 1.0f;

            // Se o tempo acabou, limpa o efeito e desliga o timer
            if (_poisonTimeLeft <= 0f)
            {
                if (IsInstanceValid(sprite))
                {
                    sprite.Modulate = Colors.White; // Volta à cor original com toda a certeza
                }

                // Destrói o timer e limpa a variável da memória
                _poisonTimer.QueueFree();
                _poisonTimer = null;
            }
        };
    }
}

