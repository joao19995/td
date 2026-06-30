using Godot;

/// <summary>
/// Abstract base class for all level scenes.
/// Subclass this instead of Node2D for every map.
/// Exposes the tilemap, spawner, camera, and entity containers so systems
/// can bind to them without knowing the concrete level type.
///
/// Towers, Enemies, and Projectiles container nodes give every spawning
/// system (TowerPlacementManager, EnemySpawner, AttackComponent) a single,
/// predictable place to AddChild into — instead of dumping every entity as
/// a direct child of the level node. This keeps the level's scene tree
/// organized by entity type and is required for any future per-type
/// operations (pooling, pausing one category, bulk cleanup, debug counts).
///
/// By default child nodes are discovered by the conventional names.
/// Override the NodePath exports in the Inspector when a map uses
/// different names. Override <see cref="OnLevelReady"/> for per-map setup.
/// </summary>
public abstract partial class BaseLevel : Node2D
{
    [Export] public NodePath TileMapNodePath { get; set; } = "TileMapLayer";
    [Export] public NodePath SpawnerNodePath { get; set; } = "EnemySpawner";
    [Export] public NodePath TowersContainerPath { get; set; } = "TowersContainer";
    [Export] public NodePath EnemiesContainerPath { get; set; } = "EnemiesContainer";
    [Export] public NodePath ProjectilesContainerPath { get; set; } = "ProjectilesContainer";

    public TileMapLayer BuildableTileMap { get; protected set; }
    public EnemySpawner Spawner { get; protected set; }
    public Node2D TowersContainer { get; protected set; }
    public Node2D EnemiesContainer { get; protected set; }
    public Node2D ProjectilesContainer { get; protected set; }

    public override void _Ready()
    {
        BuildableTileMap = GetNodeOrNull<TileMapLayer>(TileMapNodePath);
        Spawner = GetNodeOrNull<EnemySpawner>(SpawnerNodePath);
        TowersContainer = GetNodeOrNull<Node2D>(TowersContainerPath);
        EnemiesContainer = GetNodeOrNull<Node2D>(EnemiesContainerPath);
        ProjectilesContainer = GetNodeOrNull<Node2D>(ProjectilesContainerPath);

        if (BuildableTileMap == null)
            GD.PushWarning($"BaseLevel ({Name}): no TileMapLayer found at path '{TileMapNodePath}'.");

        if (Spawner == null)
            GD.PushWarning($"BaseLevel ({Name}): no EnemySpawner found at path '{SpawnerNodePath}'.");

        if (TowersContainer == null)
            GD.PushWarning($"BaseLevel ({Name}): no TowersContainer found at path '{TowersContainerPath}'.");

        if (EnemiesContainer == null)
            GD.PushWarning($"BaseLevel ({Name}): no EnemiesContainer found at path '{EnemiesContainerPath}'.");

        if (ProjectilesContainer == null)
            GD.PushWarning($"BaseLevel ({Name}): no ProjectilesContainer found at path '{ProjectilesContainerPath}'.");

        OnLevelReady();
    }

    public void ConfigureForRun()
    {
        Spawner?.ConfigureForRun(
            RunState.Instance.IsBossFight,
            RunState.Instance.IsMiniboss,
            SlotManager.Instance.MinibossStatMultiplier
        );
    }

    protected virtual void OnLevelReady() { }
}
