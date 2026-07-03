using Godot;

public partial class MainMenu : CanvasLayer
{
    [Export] private NodePath _startRunButtonPath = new NodePath("VBox/StartRunButton");
    [Export] private NodePath _metaShopButtonPath = new NodePath("VBox/MetaShopButton");
    [Export] private NodePath _bestiaryButtonPath = new NodePath("VBox/BestiaryButton");
    [Export] private NodePath _tokenLabelPath = new NodePath("VBox/TokenLabel");

    private Button _startRunButton;
    private Button _metaShopButton;
    private Button _bestiaryButton;
    private Label _tokenLabel;

    public override void _Ready()
    {
        _startRunButton = GetNode<Button>(_startRunButtonPath);
        _metaShopButton = GetNode<Button>(_metaShopButtonPath);
        _bestiaryButton = GetNode<Button>(_bestiaryButtonPath);
        _tokenLabel = GetNode<Label>(_tokenLabelPath);

        _startRunButton.Pressed += OnStartRunPressed;
        _metaShopButton.Pressed += OnMetaShopPressed;
        _bestiaryButton.Pressed += OnBestiaryPressed;
        _tokenLabel.Text = $"Tokens: {SaveManager.Instance.MetaTokens}";
    }

    private void OnBestiaryPressed()
    {
        UIManager.Instance.PushScreen(UIManager.Instance.BestiaryData);
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
