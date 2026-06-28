using Godot;

public partial class PauseScreen : Control
{
    public override void _Ready()
    {
        GetNode<Button>("VBox/ResumeButton").Pressed += () => UIManager.Instance.PopScreen();
        GetNode<Button>("VBox/MenuButton").Pressed += OnMenuPressed;
    }

    private static void OnMenuPressed()
    {
        UIManager.Instance.PopAll();
        SceneManager.Instance.LoadLevel("res://scenes/ui/screens/MainMenu.tscn",
            LevelManager.Instance.LevelContainer);
    }
}
