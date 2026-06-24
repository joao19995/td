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

        if (shape.Shape is CircleShape2D circle)
            circle.Radius = _data.Range;

        if (_data.Sprite != null)
            GetNode<Sprite2D>("Sprite2D").Texture = _data.Sprite;

        _attack.Setup(_data.ProjectileScene, _data.Damage, _data.FireRate);
    }

    public override void _Process(double delta)
    {
        if (_data == null) return;

        var target = _targeting.SelectTarget();
        _attack.TryAttack(target);
    }
}
