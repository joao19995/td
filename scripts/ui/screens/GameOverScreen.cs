using Godot;

public partial class GameOverScreen : Control
{
    [Export] private NodePath _waveLabelPath = new NodePath("VBox/WaveLabel");
    [Export] private NodePath _tokenLabelPath = new NodePath("VBox/TokenLabel");
    [Export] private NodePath _menuButtonPath = new NodePath("VBox/MenuButton");

    private Label _waveLabel;
    private Label _tokenLabel;
    private Button _menuButton;

    public override void _Ready()
    {
        _waveLabel = GetNode<Label>(_waveLabelPath);
        _tokenLabel = GetNode<Label>(_tokenLabelPath);
        _menuButton = GetNode<Button>(_menuButtonPath);

        if (LevelManager.Instance.CurrentLevelNode is BaseLevel level && level.Spawner != null)
            _waveLabel.Text = $"Wave: {level.Spawner.CurrentWaveDisplay}";

        int tokensFromProgress = SaveManager.Instance.MetaTokensPerRun;
        float ratio = SlotManager.Instance != null
            ? (float)RunState.Instance.FightsCompleted / Mathf.Max(1, SlotManager.Instance.FightsPerRun)
            : 0f;
        int earned = Mathf.RoundToInt(tokensFromProgress * (GameBalance.TokenRewardBaseMultiplier + ratio));
        _tokenLabel.Text = $"+{earned} tokens (will be awarded on return)";

        _menuButton.Pressed += OnMenuPressed;
    }

    private static void OnMenuPressed()
    {
        UIManager.NavigateToMainMenu();
    }
}
