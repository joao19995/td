using Godot;

public partial class VictoryScreen : Control
{
    [Export] private NodePath _tokenLabelPath = new NodePath("VBox/TokenLabel");
    [Export] private NodePath _statsLabelPath = new NodePath("VBox/StatsLabel");
    [Export] private NodePath _menuButtonPath = new NodePath("VBox/MenuButton");
    [Export] private NodePath _newRunButtonPath = new NodePath("VBox/NewRunButton");

    private Label _tokenLabel;
    private Label _statsLabel;
    private Button _menuButton;
    private Button _newRunButton;

    public override void _Ready()
    {
        _tokenLabel = GetNode<Label>(_tokenLabelPath);
        _statsLabel = GetNode<Label>(_statsLabelPath);
        _menuButton = GetNode<Button>(_menuButtonPath);
        _newRunButton = GetNode<Button>(_newRunButtonPath);

        var run = RunState.Instance;
        int totalFights = SlotManager.Instance?.FightsPerRun ?? 1;

        _tokenLabel.Text = $"Tokens earned: +{run.LastTokenReward}";

        _statsLabel.Text = $"Fights won: {run.LastFightsCompleted} / {totalFights}\n"
            + $"Enemies converted: {run.LastEnemiesKilled}\n"
            + $"Gold earned: {run.LastGoldEarned}\n"
            + $"Gold spent: {run.LastGoldSpent}";

        _menuButton.Pressed += OnMenuPressed;
        _newRunButton.Pressed += OnNewRunPressed;
    }

    private static void OnMenuPressed()
    {
        UIManager.NavigateToMainMenu(true);
    }

    private static void OnNewRunPressed()
    {
        UIManager.NavigateToMainMenu(true);
    }
}
