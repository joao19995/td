using Godot;
using System.Collections.Generic;

public partial class Tower : Node2D
{
    [Export] public float Range = 100f;
    [Export] public float FireRate = 1f; // disparos por segundo
    [Export] public PackedScene ProjectileScene;
    [Export] public float Damage = 5f;

    private List<Enemy> _enemiesInRange = new();
    private float _cooldownRemaining = 0f;

    public override void _Ready()
    {
        var detectionArea = GetNode<Area2D>("DetectionArea");
        var shape = detectionArea.GetNode<CollisionShape2D>("CollisionShape2D");

        if (shape.Shape is CircleShape2D circle)
        {
            circle.Radius = Range;
        }

        detectionArea.AreaEntered += OnAreaEntered;
        detectionArea.AreaExited += OnAreaExited;
    }

    public override void _Process(double delta)
    {
        if (_cooldownRemaining > 0f)
        {
            _cooldownRemaining -= (float)delta;
            return;
        }

        var target = SelectTarget();
        if (target != null)
        {
            Shoot(target);
            _cooldownRemaining = 1f / FireRate;
        }
    }

    // Critério atual: FIFO (primeiro a entrar no raio).
    // Trocar este método é o único ponto a alterar para mudar de estratégia.
    private Enemy SelectTarget()
    {
        CleanUpInvalidTargets();
        return _enemiesInRange.Count > 0 ? _enemiesInRange[0] : null;
    }

    private void CleanUpInvalidTargets()
    {
        // Remove inimigos que já morreram (QueueFree) mas ainda não dispararam AreaExited
        _enemiesInRange.RemoveAll(e => e == null || !IsInstanceValid(e));
    }

private void Shoot(Enemy target)
{
    if (ProjectileScene == null)
    {
        GD.PrintErr("Tower: ProjectileScene não está atribuído.");
        return;
    }

    var projectile = ProjectileScene.Instantiate<Projectile>();
    projectile.GlobalPosition = GlobalPosition;
    projectile.Initialize(target, Damage); // precisa de [Export] Damage na Tower também
    GetTree().CurrentScene.CallDeferred(Node.MethodName.AddChild, projectile);
}

    private void OnAreaEntered(Area2D area)
    {
        if (area is Enemy enemy)
        {
            _enemiesInRange.Add(enemy);
        }
    }

    private void OnAreaExited(Area2D area)
    {
        if (area is Enemy enemy)
        {
            _enemiesInRange.Remove(enemy);
        }
    }
}