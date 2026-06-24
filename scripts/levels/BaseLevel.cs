using Godot;

/// <summary>
/// Abstract base class for all level scenes.
/// Subclass this instead of Node2D for every map.
/// Exposes the tilemap, spawner, and camera so systems such as HUD and LevelManager
/// can bind to them without knowing the concrete level type.
///
/// By default child nodes are discovered by the conventional names
/// "TileMapLayer", "EnemySpawner", and "Camera2D".
/// Override <see cref="TileMapNodePath"/>, <see cref="SpawnerNodePath"/>, or
/// <see cref="CameraNodePath"/> in the Inspector when a map uses different names.
/// Override <see cref="OnLevelReady"/> to do additional per-map setup.
/// </summary>
public abstract partial class BaseLevel : Node2D
{
    [Export] public NodePath TileMapNodePath { get; set; } = "TileMapLayer";
    [Export] public NodePath SpawnerNodePath { get; set; } = "EnemySpawner";
    [Export] public NodePath CameraNodePath { get; set; } = "Camera2D";

    /// <summary>The TileMapLayer used for tower placement validation.</summary>
    public TileMapLayer BuildableTileMap { get; protected set; }

    /// <summary>The spawner that drives wave logic for this map.</summary>
    public EnemySpawner Spawner { get; protected set; }

    public override void _Ready()
    {
        BuildableTileMap = GetNodeOrNull<TileMapLayer>(TileMapNodePath);
        Spawner = GetNodeOrNull<EnemySpawner>(SpawnerNodePath);

        if (BuildableTileMap == null)
            GD.PushWarning($"BaseLevel ({Name}): no TileMapLayer found at path '{TileMapNodePath}'.");

        if (Spawner == null)
            GD.PushWarning($"BaseLevel ({Name}): no EnemySpawner found at path '{SpawnerNodePath}'.");

        OnLevelReady();
    }

    /// <summary>Returns the Camera2D for this level, or null if not found.</summary>
    public Camera GetCamera() => GetNodeOrNull<Camera>(CameraNodePath);

    /// <summary>Override for per-map setup that runs after discovery.</summary>
    protected virtual void OnLevelReady() { }
}
