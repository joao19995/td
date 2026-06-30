using Godot;
using Godot.Collections;

public partial class LoadoutScreen : CanvasLayer
{
    [Export] public Array<TowerData> AllTowers;

    private bool[] _selected;
    private Button _startButton;
    private Label _infoLabel;
    private const int MaxTowers = 4;

    public override void _Ready()
    {
        _infoLabel = GetNode<Label>("VBox/InfoLabel");
        _startButton = GetNode<Button>("VBox/StartRunButton");
        _selected = new bool[AllTowers.Count];
        _startButton.Pressed += OnStartRunPressed;

        int i = 0;
        foreach (var data in AllTowers)
        {
            var btn = new Button();
            btn.Text = $"{data.TowerName} ({data.Cost}g)";
            btn.ToggleMode = true;
            int idx = i;
            btn.Toggled += (toggledOn) => OnTowerToggled(idx, toggledOn);
            GetNode<VBoxContainer>("VBox/TowerList").AddChild(btn);
            i++;
        }

        UpdateStartButton();
    }

    private void OnTowerToggled(int index, bool toggledOn)
    {
        if (toggledOn)
        {
            int count = 0;
            foreach (bool s in _selected) { if (s) count++; }
            if (count >= MaxTowers)
            {
                var btn = GetNode<VBoxContainer>("VBox/TowerList").GetChild<Button>(index);
                btn.ButtonPressed = false;
                return;
            }
        }

        _selected[index] = toggledOn;
        UpdateStartButton();
    }

    private void UpdateStartButton()
    {
        int count = 0;
        for (int i = 0; i < _selected.Length; i++)
            if (_selected[i]) count++;

        _startButton.Text = count > 0 ? $"Start Run ({count})" : "Start Run";
        _startButton.Disabled = count == 0;
    }

    private void OnStartRunPressed()
    {
        var ids = new Godot.Collections.Array<string>();
        for (int i = 0; i < _selected.Length; i++)
        {
            if (_selected[i])
                ids.Add(AllTowers[i].Id);
        }

        RunState.Instance.StartRun(200, 20, ids);
        LevelManager.Instance.LoadRandomLevel();
    }
}
