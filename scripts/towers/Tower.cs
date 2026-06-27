using Godot;

public partial class Tower : Node2D
{
    private TowerData _data;
    private TargetingComponent _targeting;
    private AttackComponent _attack;
    public override void _Ready()
    {
        _targeting = GetNode<TargetingComponent>("TargetingComponent");
        _attack = GetNode<AttackComponent>("AttackComponent");

        if (_data != null)
            ApplyData();
    }

    public void Setup(TowerData data)
    {
        _data = data;

        if (IsInsideTree())
            ApplyData();
    }

    private void ApplyData()
    {
        var detectionArea = GetNode<Area2D>("DetectionArea");
        var shape = detectionArea.GetNode<CollisionShape2D>("CollisionShape2D");

        // Duplicate so each tower gets its own CircleShape2D instead of sharing
        // the one baked into Tower.tscn.
        var circle = (CircleShape2D)((CircleShape2D)shape.Shape).Duplicate();
        circle.Radius = _data.Range;
        shape.Shape = circle;

        if (_data.Sprite != null)
            GetNode<Sprite2D>("Sprite2D").Texture = _data.Sprite;

        _attack.Setup(_data.ProjectileScene, _data.Damage, _data.FireRate, _data.HasSplash, _data.SplashRadius);
    }

    public override void _Process(double delta)
    {
        if (_data == null) return;

        var target = _targeting.SelectTarget();
        _attack.TryAttack(target);
    }
}
