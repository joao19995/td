using Godot;

public partial class Main : Node2D
{
    public override void _Ready()
    {
        SceneManager.Instance.LevelLoaded += OnLevelLoaded;
        LevelManager.Instance.LoadLevel(0, this);
    }

    public override void _ExitTree()
    {
        if (SceneManager.Instance != null)
            SceneManager.Instance.LevelLoaded -= OnLevelLoaded;
    }

    private void OnLevelLoaded(Node levelNode)
    {
        if (levelNode is not BaseLevel level)
        {
            GD.PushWarning("Main: Loaded level does not extend BaseLevel.");
            return;
        }

        var hud = GetNode<HUD>("HUD");
        hud.SetActiveMap(level.BuildableTileMap, level.Spawner);
    }
}