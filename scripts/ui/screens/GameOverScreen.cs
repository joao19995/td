using Godot;

public partial class GameOverScreen : Control
{
    public override void _Ready()
    {
        if (LevelManager.Instance.CurrentLevelNode is BaseLevel level && level.Spawner != null)
            GetNode<Label>("VBox/WaveLabel").Text = $"Wave: {level.Spawner.CurrentWaveDisplay}";

        GetNode<Button>("VBox/RetryButton").Pressed += OnRetryPressed;
        GetNode<Button>("VBox/MenuButton").Pressed += OnMenuPressed;
    }

    private void OnRetryPressed()
    {
        UIManager.Instance.PopScreen();
        LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentLevelIndex);
    }

    private static void OnMenuPressed()
    {
        UIManager.Instance.PopAll();
        SceneManager.Instance.LoadLevel("res://scenes/ui/screens/MainMenu.tscn",
            LevelManager.Instance.LevelContainer);
    }
}
