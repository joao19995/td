using Godot;

public partial class Main : Node2D
{
    public override void _Ready()
    {
        SceneManager.Instance.LevelLoaded += OnLevelLoaded;
        SceneManager.Instance.LoadLevel("res://scenes/levels/Map1.tscn", this);
    }

    public override void _ExitTree()
    {
        if (SceneManager.Instance != null)
            SceneManager.Instance.LevelLoaded -= OnLevelLoaded;
    }

    private void OnLevelLoaded(Node levelNode)
    {
        var hud = GetNode<HUD>("HUD");
        var tileMap = levelNode.GetNode<TileMapLayer>("TileMapLayer");
        var spawner = levelNode.GetNode<EnemySpawner>("EnemySpawner");
        hud.SetActiveMap(tileMap, spawner);
    }
}