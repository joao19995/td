using Godot;

public partial class MainMenu : CanvasLayer
{
    public override void _Ready()
    {
        var btn = GetNode<Button>("VBox/PlayButton");
        GD.Print($"[MainMenu] _Ready — button found={btn != null}");
        btn.Pressed += OnPlayPressed;
    }

    private void OnPlayPressed()
    {
        GD.Print("[MainMenu] Play pressed");
        var lm = LevelManager.Instance;
        GD.Print($"[MainMenu] LevelManager.Instance={lm != null}, Levels.Count={lm?.Levels?.Count}, CurrentLevelNode={lm?.CurrentLevelNode?.Name}");
        lm.LoadLevel(0);
    }
}
