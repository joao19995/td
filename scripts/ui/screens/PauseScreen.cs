using Godot;

public partial class PauseScreen : Control
{
    [Export] private NodePath _resumeButtonPath = new NodePath("VBox/ResumeButton");
    [Export] private NodePath _bestiaryButtonPath = new NodePath("VBox/BestiaryButton");
    [Export] private NodePath _menuButtonPath = new NodePath("VBox/MenuButton");
    [Export] private NodePath _endRunButtonPath = new NodePath("VBox/EndRunButton");

    private Button _resumeButton;
    private Button _bestiaryButton;
    private Button _menuButton;
    private Button _endRunButton;

    public override void _Ready()
    {
        _resumeButton = GetNode<Button>(_resumeButtonPath);
        _bestiaryButton = GetNode<Button>(_bestiaryButtonPath);
        _menuButton = GetNode<Button>(_menuButtonPath);
        _endRunButton = GetNode<Button>(_endRunButtonPath);

        _resumeButton.Pressed += () => UIManager.Instance.PopScreen();
        _bestiaryButton.Pressed += () => UIManager.Instance.PushScreen(UIManager.Instance.BestiaryData);
        _menuButton.Pressed += () => UIManager.NavigateToMainMenuKeepRun();
        _endRunButton.Pressed += () => UIManager.NavigateToMainMenu();
    }
}
