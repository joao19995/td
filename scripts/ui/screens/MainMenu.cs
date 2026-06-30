using Godot;

public partial class MainMenu : CanvasLayer
{
    public override void _Ready()
    {
        var btn = GetNode<Button>("VBox/StartRunButton");
        btn.Pressed += OnStartRunPressed;
    }

    private void OnStartRunPressed()
    {
        SceneManager.Instance.LoadLevel("res://scenes/ui/screens/LoadoutScreen.tscn",
            LevelManager.Instance.LevelContainer);
    }
}
