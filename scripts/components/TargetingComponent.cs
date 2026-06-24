using Godot;
using System.Collections.Generic;

/// <summary>
/// Selects an attack target from enemies currently inside the tower's detection area.
/// Strategy is configurable via the exported property.
/// Parent must be a Node2D.
/// </summary>
public enum TargetingStrategy
{
    First,     // First enemy to enter range (FIFO)
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
            TargetingStrategy.First    => _enemiesInRange[0],
            TargetingStrategy.Last     => _enemiesInRange[_enemiesInRange.Count - 1],
            TargetingStrategy.Closest  => GetClosestEnemy(),
            TargetingStrategy.Strongest => GetStrongestEnemy(),
            _                          => _enemiesInRange[0],
        };
    }

    private void CleanUpInvalidTargets()
    {
        _enemiesInRange.RemoveAll(e => e == null || !IsInstanceValid(e));
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
