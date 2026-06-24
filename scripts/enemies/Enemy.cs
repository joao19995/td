using Godot;

public partial class Enemy : Area2D
{
    [Export] public float Speed = 60f;
    [Export] public int RewardOnDeath = 5;
    [Export] public int DamageToPlayer = 1;
    private Curve2D _curve;
    private float _distanceTraveled;
    private float _pathLength;
    private Health _health;

    public override void _Ready()
    {
        _health = GetNode<Health>("Health");
        _health.Died += OnDied;
    }

    public void Initialize(Curve2D curve)
    {
        _curve = curve;
        _pathLength = _curve.GetBakedLength();
        _distanceTraveled = 0f;
        GlobalPosition = _curve.SampleBaked(0f);
    }

    public void TakeDamage(float amount)
    {
        _health.TakeDamage(amount);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_curve == null) return;

        _distanceTraveled += Speed * (float)delta;

        if (_distanceTraveled >= _pathLength)
        {
            ReachedEnd();
            return;
        }

        GlobalPosition = _curve.SampleBaked(_distanceTraveled);
    }

    private void ReachedEnd()
    {
        EventBus.Instance.EmitSignal(EventBus.SignalName.EnemyReachedEnd, DamageToPlayer);
        QueueFree();
    }

    private void OnDied()
    {
        EventBus.Instance.EmitSignal(EventBus.SignalName.EnemyDied, RewardOnDeath);
        QueueFree();
    }
}