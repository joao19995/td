using Godot;

public partial class Enemy : Area2D
{
    private EnemyData _data;
    private Curve2D _curve;
    private float _distanceTraveled;
    private float _pathLength;
    private Health _health;

    public override void _Ready()
    {
        _health = GetNode<Health>("Health");
        _health.Died += OnDied;

        if (_data != null)
            ApplyData();
    }

    public void Initialize(EnemyData data, Curve2D curve)
    {
        _data = data;
        _curve = curve;
        _pathLength = _curve.GetBakedLength();
        _distanceTraveled = 0f;
        GlobalPosition = _curve.SampleBaked(0f);

        // _health is only available after _Ready; ApplyData() is called from _Ready.
        if (_health != null)
            ApplyData();
    }

    private void ApplyData()
    {
        _health.Setup(_data.MaxHealth);

        if (_data.Sprite != null)
            GetNode<Sprite2D>("Sprite2D").Texture = _data.Sprite;
    }

    public void TakeDamage(float amount)
    {
        _health.TakeDamage(amount);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_curve == null || _data == null) return;

        _distanceTraveled += _data.MoveSpeed * (float)delta;

        if (_distanceTraveled >= _pathLength)
        {
            ReachedEnd();
            return;
        }

        GlobalPosition = _curve.SampleBaked(_distanceTraveled);
    }

    private void ReachedEnd()
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
