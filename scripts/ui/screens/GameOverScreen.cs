using Godot;

public partial class GameOverScreen : Control
{
    [Export] private NodePath _waveLabelPath = new NodePath("VBox/WaveLabel");
    [Export] private NodePath _menuButtonPath = new NodePath("VBox/MenuButton");

    private Label _waveLabel;
    private Button _menuButton;

    public override void _Ready()
    {
        _waveLabel = GetNode<Label>(_waveLabelPath);
        _menuButton = GetNode<Button>(_menuButtonPath);

        if (LevelManager.Instance.CurrentLevelNode is BaseLevel level && level.Spawner != null)
            _waveLabel.Text = $"Wave: {level.Spawner.CurrentWaveDisplay}";

        _menuButton.Pressed += OnMenuPressed;
    }

    private static void OnMenuPressed()
    {
        RunState.Instance.EndRun();
        UIManager.Instance.PopAll();
        SceneManager.Instance.LoadLevel("res://scenes/ui/screens/MainMenu.tscn",
            LevelManager.Instance.LevelContainer);
    }
}
