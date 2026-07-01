using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class LoadoutScreen : CanvasLayer
{
    private List<TowerData> _allTowers;
    private bool[] _selected;
    private Button _startButton;
    private Label _infoLabel;
    [Export] private int MaxTowers = 4;

    public override void _Ready()
    {
        _infoLabel = GetNode<Label>("VBox/InfoLabel");
        _startButton = GetNode<Button>("VBox/StartRunButton");
        _startButton.Pressed += OnStartRunPressed;

        _allTowers = LoadAllTowers();
        _selected = new bool[_allTowers.Count];

        GetNode<Label>("VBox/Title").Text = $"SELECT TOWERS (max {MaxTowers})";

        int i = 0;
        foreach (var data in _allTowers)
        {
            bool unlocked = SaveManager.Instance.IsTowerUnlocked(data.Id);
            var btn = new Button();
            btn.Text = unlocked ? $"{data.TowerName} ({data.Cost}g)" : $"{data.TowerName} (LOCKED)";
            btn.Disabled = !unlocked;
            btn.ToggleMode = unlocked;
            int idx = i;
            btn.Toggled += (toggledOn) => OnTowerToggled(idx, toggledOn);
            GetNode<VBoxContainer>("VBox/TowerScroll/TowerList").AddChild(btn);
            i++;
        }

        UpdateStartButton();
    }

    private static List<TowerData> LoadAllTowers()
    {
        var list = new List<TowerData>();
        var dir = DirAccess.Open("res://resources/tower_data/");
        if (dir == null) return list;
        foreach (var file in dir.GetFiles())
        {
            if (!file.EndsWith(".tres") && !file.EndsWith(".res"))
                continue;
            var res = ResourceLoader.Load<Resource>("res://resources/tower_data/" + file, "", ResourceLoader.CacheMode.Replace);
            if (res is TowerData t)
                list.Add(t);
        }
        return list;
    }

    private void OnTowerToggled(int index, bool toggledOn)
    {
        if (toggledOn)
        {
            int count = 0;
            foreach (bool s in _selected) { if (s) count++; }
            if (count >= MaxTowers)
            {
                var btn = GetNode<VBoxContainer>("VBox/TowerScroll/TowerList").GetChild<Button>(index);
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
                ids.Add(_allTowers[i].Id);
        }

        RunState.Instance.StartRun(EconomyManager.Instance.StartingMoney, GameManager.Instance.StartingLives, ids);
        LevelManager.Instance.PickRandomLevel();
        UIManager.Instance.PushScreen(UIManager.Instance.BriefingData);
    }
}
