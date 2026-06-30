using Godot;

public partial class MainMenu : CanvasLayer
{
    public override void _Ready()
    {
        GetNode<Button>("VBox/StartRunButton").Pressed += OnStartRunPressed;
        GetNode<Button>("VBox/MetaShopButton").Pressed += OnMetaShopPressed;
        GetNode<Label>("VBox/TokenLabel").Text = $"Tokens: {SaveManager.Instance.MetaTokens}";
    }

    private void OnStartRunPressed()
    {
        SceneManager.Instance.LoadLevel("res://scenes/ui/screens/LoadoutScreen.tscn",
            LevelManager.Instance.LevelContainer);
    }

    private void OnMetaShopPressed()
    {
        UIManager.Instance.PushScreen(UIManager.Instance.MetaShopData);
    }
}
