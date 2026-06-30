using Godot;

public partial class GameOverScreen : Control
{
    public override void _Ready()
    {
        if (LevelManager.Instance.CurrentLevelNode is BaseLevel level && level.Spawner != null)
            GetNode<Label>("VBox/WaveLabel").Text = $"Wave: {level.Spawner.CurrentWaveDisplay}";

        GetNode<Button>("VBox/MenuButton").Pressed += OnMenuPressed;
    }

    private static void OnMenuPressed()
    {
        RunState.Instance.EndRun();
        UIManager.Instance.PopAll();
        SceneManager.Instance.LoadLevel("res://scenes/ui/screens/MainMenu.tscn",
            LevelManager.Instance.LevelContainer);
    }
}
