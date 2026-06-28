using Godot;

public partial class Main : Node2D
{
    public override void _Ready()
    {
        SceneManager.Instance.LevelLoaded += OnLevelLoaded;
        LevelManager.Instance.SetContainer(this);
        SceneManager.Instance.LoadLevel("res://scenes/ui/screens/MainMenu.tscn", this);
    }

    public override void _ExitTree()
    {
        if (SceneManager.Instance != null)
            SceneManager.Instance.LevelLoaded -= OnLevelLoaded;
    }

    private void OnLevelLoaded(Node levelNode)
    {
        if (levelNode is not BaseLevel level)
            return;

        var hud = GetNode<HUD>("HUD");
        hud.SetActiveMap(level.BuildableTileMap, level.Spawner);
    }
}