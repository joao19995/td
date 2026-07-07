using Godot;

public partial class GameOverScreen : Control
{
    [Export] private NodePath _waveLabelPath = new NodePath("VBox/WaveLabel");
    [Export] private NodePath _tokenLabelPath = new NodePath("VBox/TokenLabel");
    [Export] private NodePath _statsLabelPath = new NodePath("VBox/StatsLabel");
    [Export] private NodePath _menuButtonPath = new NodePath("VBox/MenuButton");

    private Label _waveLabel;
    private Label _tokenLabel;
    private Label _statsLabel;
    private Button _menuButton;

    public override void _Ready()
    {
        _waveLabel = GetNode<Label>(_waveLabelPath);
        _tokenLabel = GetNode<Label>(_tokenLabelPath);
        _statsLabel = GetNode<Label>(_statsLabelPath);
        _menuButton = GetNode<Button>(_menuButtonPath);

        if (LevelManager.Instance.CurrentLevelNode is BaseLevel level && level.Spawner != null)
            _waveLabel.Text = $"Wave: {level.Spawner.CurrentWaveDisplay}";

        var run = RunState.Instance;
        int totalFights = SlotManager.Instance != null ? SlotManager.Instance.FightsPerRun : 1;
        int earned = run.PreviewTokenReward();
        _tokenLabel.Text = $"+{earned} tokens (will be awarded on return)";

        _statsLabel.Text = $"Fights won: {run.FightsCompleted} / {totalFights}\n"
            + $"Enemies converted: {run.TotalEnemiesKilled}\n"
            + $"Gold earned: {run.TotalGoldEarned}\n"
            + $"Gold spent: {run.TotalGoldSpent}";

        _menuButton.Pressed += OnMenuPressed;
    }

    private static void OnMenuPressed()
    {
        UIManager.NavigateToMainMenu();
    }
}
