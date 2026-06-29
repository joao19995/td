using Godot;

public partial class MainMenu : CanvasLayer
{
    public override void _Ready()
    {
        var btn = GetNode<Button>("VBox/PlayButton");
        btn.Pressed += OnPlayPressed;
    }

    private void OnPlayPressed()
    {
        LevelManager.Instance.LoadLevel(0);
    }
}
