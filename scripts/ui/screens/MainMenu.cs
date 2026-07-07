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
    private Button _continueButton;
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

        if (SaveManager.Instance.HasRunState())
        {
            _continueButton = new Button();
            _continueButton.Text = "Continue Run";
            _continueButton.Pressed += OnContinueRunPressed;
            var vbox = _startRunButton.GetParent<VBoxContainer>();
            vbox.AddChild(_continueButton);
            vbox.MoveChild(_continueButton, vbox.GetChildren().Count - 1);
        }
    }

    private void OnContinueRunPressed()
    {
        var data = SaveManager.Instance.LoadRunState();
        if (data == null) return;

        RunState.Instance.TryResumeRun(data);
        LevelManager.Instance.PickRandomLevel();
        UIManager.Instance.PushScreen(UIManager.Instance.BriefingData);
    }

    private void OnBestiaryPressed()
    {
        UIManager.Instance.PushScreen(UIManager.Instance.BestiaryData);
    }

    private void OnStartRunPressed()
    {
        GD.Print("[MainMenu] Start Run pressed — loading LoadoutScreen.");
        SceneManager.Instance.LoadLevel("res://scenes/ui/screens/LoadoutScreen.tscn",
            LevelManager.Instance.LevelContainer);
    }

    private void OnMetaShopPressed()
    {
        UIManager.Instance.PushScreen(UIManager.Instance.MetaShopData);
    }
}
