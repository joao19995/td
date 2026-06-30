using Godot;

public partial class FightCompleteScreen : Control
{
    public override void _Ready()
    {
        GetNode<Label>("VBox/GoldLabel").Text = $"Gold: {EconomyManager.Instance.CurrentMoney}";
        GetNode<Label>("VBox/LivesLabel").Text = $"Lives: {GameManager.Instance.CurrentLives}";
        GetNode<Button>("VBox/NextFightButton").Pressed += OnNextFightPressed;
        GetNode<Button>("VBox/EndRunButton").Pressed += OnEndRunPressed;
    }

    private void OnNextFightPressed()
    {
        UIManager.Instance.PopScreen();
        LevelManager.Instance.LoadRandomLevel();
    }

    private static void OnEndRunPressed()
    {
        RunState.Instance.EndRun();
        UIManager.Instance.PopScreen();
        UIManager.Instance.PopAll();
        SceneManager.Instance.LoadLevel("res://scenes/ui/screens/MainMenu.tscn",
            LevelManager.Instance.LevelContainer);
    }
}
