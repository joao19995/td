using Godot;
using System.Collections.Generic;

public partial class Tower : Node2D
{
    private TowerData _data;
    private List<Enemy> _enemiesInRange = new();
    private float _cooldownRemaining = 0f;

    public override void _Ready()
    {
        var detectionArea = GetNode<Area2D>("DetectionArea");
        detectionArea.AreaEntered += OnAreaEntered;
        detectionArea.AreaExited += OnAreaExited;

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
    }

    public override void _Process(double delta)
    {
        if (_data == null) return;

        if (_cooldownRemaining > 0f)
        {
            _cooldownRemaining -= (float)delta;
            return;
        }

        var target = SelectTarget();
        if (target != null)
        {
            Shoot(target);
            _cooldownRemaining = 1f / _data.FireRate;
        }
    }

    // Targeting strategy: FIFO (first enemy to enter range).
    // Swap this method to change to Closest / Strongest / Last.
    private Enemy SelectTarget()
    {
        CleanUpInvalidTargets();
        return _enemiesInRange.Count > 0 ? _enemiesInRange[0] : null;
    }

    private void CleanUpInvalidTargets()
    {
        _enemiesInRange.RemoveAll(e => e == null || !IsInstanceValid(e));
    }

    private void Shoot(Enemy target)
    {
        if (_data.ProjectileScene == null)
        {
            GD.PrintErr("Tower: ProjectileScene not set in TowerData.");
            return;
        }

        var projectile = _data.ProjectileScene.Instantiate<Projectile>();
        projectile.GlobalPosition = GlobalPosition;
        projectile.Initialize(target, _data.Damage);
        GetTree().CurrentScene.CallDeferred(Node.MethodName.AddChild, projectile);
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area is Enemy enemy)
            _enemiesInRange.Add(enemy);
    }

    private void OnAreaExited(Area2D area)
    {
        if (area is Enemy enemy)
            _enemiesInRange.Remove(enemy);
    }
}
