using Godot;

/// <summary>
/// Centralises enemy instantiation. When object pooling is added, only this class changes.
/// </summary>
public static class EnemyFactory
{
    /// <summary>
    /// Instantiates an enemy from the given scene, wires its data and path, and returns it
    /// <b>without</b> adding it to the scene tree. The caller is responsible for AddChild.
    /// </summary>
    public static Enemy Create(PackedScene enemyScene, EnemyData data, Curve2D path)
    {
        var enemy = enemyScene.Instantiate<Enemy>();
        enemy.Initialize(data, path);
        return enemy;
    }
}
