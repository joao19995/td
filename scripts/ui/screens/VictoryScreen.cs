using Godot;

public partial class VictoryScreen : Control
{
    [Export] private NodePath _menuButtonPath = new NodePath("VBox/MenuButton");

    private Button _menuButton;

    public override void _Ready()
    {
        _menuButton = GetNode<Button>(_menuButtonPath);
        _menuButton.Pressed += OnMenuPressed;
    }

    private static void OnMenuPressed()
    {
        RunState.Instance.EndRun(true);
        UIManager.Instance.PopAll();
        SceneManager.Instance.LoadLevel("res://scenes/ui/screens/MainMenu.tscn",
            LevelManager.Instance.LevelContainer);
    }
}
