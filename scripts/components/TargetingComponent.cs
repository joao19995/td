using Godot;
using System.Collections.Generic;

public enum TargetingStrategy
{
    First,     // Furthest along the path (closest to finish)
    Closest,   // Enemy closest to the tower
    Strongest, // Enemy with the most remaining health
    Last       // Enemy that entered range most recently (LIFO)
}

public partial class TargetingComponent : Node
{
    /// <summary>NodePath to the Area2D that tracks enemies in range.</summary>
    [Export] public NodePath DetectionAreaPath { get; set; } = "../DetectionArea";

    [Export] public TargetingStrategy Strategy { get; set; } = TargetingStrategy.First;

    private readonly List<Enemy> _enemiesInRange = new();

    public override void _Ready()
    {
        if (GetParent() is not Node2D)
            GD.PushError($"TargetingComponent: parent must be a Node2D (got {GetParent()?.GetClass()}).");

        var detectionArea = GetNodeOrNull<Area2D>(DetectionAreaPath);
        if (detectionArea == null)
        {
            GD.PushError($"TargetingComponent: no Area2D found at DetectionAreaPath='{DetectionAreaPath}'. Targeting will not work.");
            return;
        }

        detectionArea.AreaEntered += OnAreaEntered;
        detectionArea.AreaExited += OnAreaExited;
    }

    /// <summary>Returns the best target according to the current Strategy, or null if none.</summary>
    public Enemy SelectTarget()
    {
        CleanUpInvalidTargets();
        if (_enemiesInRange.Count == 0) return null;

        return Strategy switch
        {
            TargetingStrategy.First    => GetFurthestEnemy(),
            TargetingStrategy.Last     => _enemiesInRange[_enemiesInRange.Count - 1],
            TargetingStrategy.Closest  => GetClosestEnemy(),
            TargetingStrategy.Strongest => GetStrongestEnemy(),
            _                          => GetFurthestEnemy(),
        };
    }

    private void CleanUpInvalidTargets()
    {
        _enemiesInRange.RemoveAll(e => e == null || !IsInstanceValid(e) || e.IsDead || !e.IsInsideTree());
    }

    private Enemy GetClosestEnemy()
    {
        var origin = GetParent<Node2D>().GlobalPosition;
        Enemy closest = null;
        float minDist = float.MaxValue;

        foreach (var enemy in _enemiesInRange)
        {
            float dist = origin.DistanceTo(enemy.GlobalPosition);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }

        return closest;
    }

    private Enemy GetStrongestEnemy()
    {
        Enemy strongest = null;
        float maxHealth = -1f;

        foreach (var enemy in _enemiesInRange)
        {
            float health = enemy.GetCurrentHealth();
            if (health > maxHealth)
            {
                maxHealth = health;
                strongest = enemy;
            }
        }

        return strongest;
    }

    private Enemy GetFurthestEnemy()
    {
        Enemy furthest = null;
        float maxProgress = -1f;

        foreach (var enemy in _enemiesInRange)
        {
            var movement = enemy.GetNodeOrNull<MovementComponent>("MovementComponent");
            float progress = movement != null ? movement.GetProgressRatio() : 0f;
            if (progress > maxProgress)
            {
                maxProgress = progress;
                furthest = enemy;
            }
        }

        return furthest;
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
