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
        UIManager.NavigateToMainMenu(true);
    }
}
