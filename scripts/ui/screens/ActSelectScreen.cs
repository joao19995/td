using Godot;
using System.Collections.Generic;

public partial class ActSelectScreen : CanvasLayer
{
    [Export] private NodePath _titlePath = new NodePath("VBox/Title");
    [Export] private NodePath _backButtonPath = new NodePath("VBox/BackButton");
    [Export] private NodePath _actListPath = new NodePath("VBox/ActScroll/ActList");
    [Export] private NodePath _descLabelPath = new NodePath("VBox/DescLabel");

    private Label _title;
    private Button _backButton;
    private VBoxContainer _actList;
    private Label _descLabel;

    private Dictionary<string, ActData> _actsById = new();

    public override void _Ready()
    {
        _title = GetNode<Label>(_titlePath);
        _backButton = GetNode<Button>(_backButtonPath);
        _actList = GetNode<VBoxContainer>(_actListPath);
        _descLabel = GetNode<Label>(_descLabelPath);

        _backButton.Pressed += OnBackPressed;

        var allActs = ResourceLoaderHelper.LoadFromDir<ActData>("res://resources/act_data/");
        _title.Text = "SELECT ACT";

        if (allActs.Count == 0)
        {
            _descLabel.Text = "No acts found — create ActData resources in resources/act_data/";
            return;
        }

        foreach (var act in allActs)
        {
            _actsById[act.Id] = act;

            bool unlocked = SaveManager.Instance.IsActUnlocked(act.Id);
            var btn = new Button();
            btn.Text = unlocked
                ? $"  {act.ActName}"
                : $"  {act.ActName} (LOCKED)";
            btn.Disabled = !unlocked;
            btn.ToggleMode = false;
            if (act.PreviewTexture != null)
                btn.Icon = act.PreviewTexture;
            btn.CustomMinimumSize = new Vector2(0, 24);

            string actId = act.Id;
            btn.MouseEntered += () => ShowDescription(actId);
            btn.MouseExited += () => _descLabel.Text = "";

            if (unlocked)
            {
                btn.Pressed += () => OnActSelected(actId);
            }

            _actList.AddChild(btn);
        }
    }

    private void ShowDescription(string actId)
    {
        if (!_actsById.TryGetValue(actId, out var act)) return;
        string status = SaveManager.Instance.IsActUnlocked(actId) ? "UNLOCKED" : "LOCKED";
        _descLabel.Text = $"[{status}] {act.Description}";
    }

    private void OnActSelected(string actId)
    {
        if (!_actsById.TryGetValue(actId, out var act)) return;
        GD.Print($"[ActSelect] Selected act: {act.Id} ({act.ActName}).");
        RunState.Instance.SetAct(act);
        SceneManager.Instance.LoadLevel("res://scenes/ui/screens/LoadoutScreen.tscn",
            LevelManager.Instance.LevelContainer);
    }

    private void OnBackPressed()
    {
        SceneManager.Instance.LoadLevel("res://scenes/ui/screens/MainMenu.tscn",
            LevelManager.Instance.LevelContainer);
    }
}
