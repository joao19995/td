using Godot;

/// <summary>
/// Centralises tower instantiation. When object pooling is added, only this class changes.
/// </summary>
public static class TowerFactory
{
    /// <summary>
    /// Instantiates a tower from the given scene, positions it, injects its data, and returns it
    /// <b>without</b> adding it to the scene tree. The caller is responsible for AddChild.
    /// </summary>
    public static Tower Create(PackedScene towerScene, TowerData data, Vector2 position)
    {
        var tower = towerScene.Instantiate<Tower>();
        tower.GlobalPosition = position;
        tower.Setup(data);
        return tower;
    }
}
