using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class LoadoutScreen : CanvasLayer
{
    private List<TowerData> _allTowers;
    private List<SynergyData> _allSynergies;
    private bool[] _selected;
    private List<Button> _towerButtons;
    private bool _isBatchUpdating;

    private Button _startButton;
    private Label _infoLabel;
    private Button _randomButton;
    private Button[] _slotButtons;
    private Label _saveHelpLabel;
    private VBoxContainer _towerList;
    private VBoxContainer _previewPanel;
    private TextureRect _previewSprite;
    private Label _previewName;
    private Label _previewStats;
    private Label _previewSpecial;
    private Label _synergyLabel;

    [Export] private int MaxTowers = 4;

    public override void _Ready()
    {
        _infoLabel = GetNode<Label>("VBox/TopRow/InfoLabel");
        _startButton = GetNode<Button>("VBox/BottomRow/StartRunButton");
        _randomButton = GetNode<Button>("VBox/TopRow/RandomButton");
        _towerList = GetNode<VBoxContainer>("VBox/ContentHBox/TowerScroll/TowerList");
        _previewPanel = GetNode<VBoxContainer>("VBox/ContentHBox/PreviewPanel");
        _previewSprite = GetNode<TextureRect>("VBox/ContentHBox/PreviewPanel/PreviewHBox/PreviewSprite");
        _previewName = GetNode<Label>("VBox/ContentHBox/PreviewPanel/PreviewHBox/PreviewName");
        _previewStats = GetNode<Label>("VBox/ContentHBox/PreviewPanel/PreviewStats");
        _previewSpecial = GetNode<Label>("VBox/ContentHBox/PreviewPanel/PreviewSpecial");
        _synergyLabel = GetNode<Label>("VBox/SynergyLabel");
        _saveHelpLabel = GetNode<Label>("VBox/BottomRow/SaveHelpLabel");
        _slotButtons = new Button[]
        {
            GetNode<Button>("VBox/BottomRow/Slot1"),
            GetNode<Button>("VBox/BottomRow/Slot2"),
            GetNode<Button>("VBox/BottomRow/Slot3"),
        };

        _startButton.Pressed += OnStartRunPressed;
        _randomButton.Pressed += OnRandomPressed;

        for (int i = 0; i < 3; i++)
        {
            int slot = i;
            _slotButtons[i].GuiInput += (@event) =>
            {
                if (@event is InputEventMouseButton mb && mb.Pressed)
                {
                    if (mb.ButtonIndex == MouseButton.Right)
                    {
                        SaveSlot(slot);
                    }
                    else if (mb.ButtonIndex == MouseButton.Left)
                    {
                        var existing = SaveManager.Instance.GetLoadoutSlot(slot);
                        if (existing != null && existing.Count > 0)
                            LoadSlot(slot);
                        else
                            SaveSlot(slot);
                    }
                }
            };
            _slotButtons[i].MouseEntered += () => ShowSlotHint(slot);
            _slotButtons[i].MouseExited += () => { if (_saveHelpLabel.Text.StartsWith("Slot")) _saveHelpLabel.Text = ""; };
        }

        _allTowers = LoadAllTowers();
        _allSynergies = LoadAllSynergies();
        _selected = new bool[_allTowers.Count];
        _towerButtons = new List<Button>();

        GetNode<Label>("VBox/Title").Text = $"SELECT TOWERS (max {MaxTowers})";

        int idx = 0;
        foreach (var data in _allTowers)
        {
            bool unlocked = SaveManager.Instance.IsTowerUnlocked(data.Id);
            var btn = new Button();
            btn.Text = unlocked ? $" {data.TowerName} ({data.Cost}g)" : $" {data.TowerName} (LOCKED)";
            btn.Disabled = !unlocked;
            btn.ToggleMode = unlocked;
            btn.Icon = data.Sprite;
            btn.CustomMinimumSize = new Vector2(0, 20);

            int i = idx;
            btn.Toggled += (toggledOn) => OnTowerToggled(i, toggledOn);
            btn.MouseEntered += () => ShowPreview(i);
            btn.MouseExited += () => HidePreview();

            _towerList.AddChild(btn);
            _towerButtons.Add(btn);
            idx++;
        }

        UpdateLabels();
        UpdateSlotTooltips();
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

    private static List<SynergyData> LoadAllSynergies()
    {
        var list = new List<SynergyData>();
        var dir = DirAccess.Open("res://resources/synergy_data/");
        if (dir == null) return list;
        foreach (var file in dir.GetFiles())
        {
            if (!file.EndsWith(".tres") && !file.EndsWith(".res"))
                continue;
            var res = ResourceLoader.Load<Resource>("res://resources/synergy_data/" + file, "", ResourceLoader.CacheMode.Replace);
            if (res is SynergyData s)
                list.Add(s);
        }
        return list;
    }

    private void OnTowerToggled(int index, bool toggledOn)
    {
        if (toggledOn && !_isBatchUpdating)
        {
            int count = 0;
            foreach (bool s in _selected) { if (s) count++; }
            if (count >= MaxTowers)
            {
                _towerButtons[index].ButtonPressed = false;
                return;
            }
        }

        _selected[index] = toggledOn;
        UpdateLabels();
        UpdateSynergyHints();
    }

    private void ShowPreview(int index)
    {
        var data = _allTowers[index];
        _previewSprite.Texture = data.Sprite;
        _previewName.Text = data.TowerName;
        _previewStats.Text = $"DMG:{data.Damage}  SPD:{data.FireRate:F1}  RNG:{data.Range:F0}";

        var specials = new List<string>();
        if (data.HasSplash) specials.Add("SPLASH");
        if (data.HasPoison) specials.Add("POISON");
        if (data.HasSlow) specials.Add("SLOW");
        if (data.HasAura) specials.Add("AURA");
        if (data.HasChain) specials.Add("CHAIN");
        if (data.HasCrit) specials.Add("CRIT");
        if (data.HasExecute) specials.Add("EXECUTE");
        if (data.HasGlobalAura) specials.Add("GLOBAL");

        _previewSpecial.Text = specials.Count > 0 ? string.Join(" ", specials) : "";
        _previewPanel.Modulate = new Color(1, 1, 1, 1);
        _previewPanel.MouseFilter = Control.MouseFilterEnum.Pass;
    }

    private void HidePreview()
    {
        _previewPanel.Modulate = new Color(1, 1, 1, 0);
        _previewPanel.MouseFilter = Control.MouseFilterEnum.Ignore;
    }

    private void UpdateLabels()
    {
        int count = 0;
        for (int i = 0; i < _selected.Length; i++)
            if (_selected[i]) count++;

        _infoLabel.Text = $"{count} / {MaxTowers} selected";
        _startButton.Text = count > 0 ? $"Start Run ({count})" : "Start Run";
        _startButton.Disabled = count == 0;
    }

    private void UpdateSynergyHints()
    {
        var selectedIds = new List<string>();
        for (int i = 0; i < _selected.Length; i++)
            if (_selected[i]) selectedIds.Add(_allTowers[i].Id);

        var active = GetPreviewSynergies(selectedIds);
        if (active.Count > 0)
        {
            var parts = new List<string>();
            foreach (var s in active)
                parts.Add($"{s.DisplayName}");
            _synergyLabel.Text = "Synergies: " + string.Join(", ", parts);
            _synergyLabel.Visible = true;
        }
        else
        {
            _synergyLabel.Visible = false;
        }
    }

    private List<SynergyData> GetPreviewSynergies(List<string> selectedIds)
    {
        var active = new List<SynergyData>();
        foreach (var synergy in _allSynergies)
        {
            if (synergy == null) continue;

            if (!SaveManager.Instance.IsDiscovered("synergy_" + synergy.Id))
                continue;

            bool allRequiredUnlocked = true;
            foreach (var reqId in synergy.RequiredTowerIds)
            {
                if (!SaveManager.Instance.IsTowerUnlocked(reqId))
                {
                    allRequiredUnlocked = false;
                    break;
                }
            }
            if (!allRequiredUnlocked) continue;

            bool allRequired = true;
            foreach (var reqId in synergy.RequiredTowerIds)
            {
                if (!selectedIds.Contains(reqId))
                {
                    allRequired = false;
                    break;
                }
            }
            if (allRequired && selectedIds.Count >= synergy.MinTowerCount)
                active.Add(synergy);
        }
        return active;
    }

    private void OnRandomPressed()
    {
        _isBatchUpdating = true;

        for (int i = 0; i < _towerButtons.Count; i++)
            _towerButtons[i].ButtonPressed = false;

        var unlocked = new List<int>();
        for (int i = 0; i < _allTowers.Count; i++)
            if (SaveManager.Instance.IsTowerUnlocked(_allTowers[i].Id))
                unlocked.Add(i);

        var rng = new System.Random();
        for (int i = unlocked.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            int tmp = unlocked[i];
            unlocked[i] = unlocked[j];
            unlocked[j] = tmp;
        }

        int take = Mathf.Min(MaxTowers, unlocked.Count);
        for (int i = 0; i < take; i++)
        {
            int idx = unlocked[i];
            _towerButtons[idx].ButtonPressed = true;
        }

        _isBatchUpdating = false;
        UpdateLabels();
        UpdateSynergyHints();

        _saveHelpLabel.Text = $"Random: {take} towers";
    }

    private void SaveSlot(int slot)
    {
        var ids = new Array<string>();
        for (int i = 0; i < _selected.Length; i++)
            if (_selected[i]) ids.Add(_allTowers[i].Id);

        SaveManager.Instance.SetLoadoutSlot(slot, ids);
        UpdateSlotTooltips();
        _saveHelpLabel.Text = $"Saved slot {slot + 1}!";
    }

    private void LoadSlot(int slot)
    {
        var ids = SaveManager.Instance.GetLoadoutSlot(slot);
        if (ids == null || ids.Count == 0) return;

        _isBatchUpdating = true;

        for (int i = 0; i < _towerButtons.Count; i++)
            _towerButtons[i].ButtonPressed = false;

        foreach (var id in ids)
        {
            for (int i = 0; i < _allTowers.Count; i++)
            {
                if (_allTowers[i].Id == id && SaveManager.Instance.IsTowerUnlocked(id))
                {
                    _towerButtons[i].ButtonPressed = true;
                    break;
                }
            }
        }

        _isBatchUpdating = false;
        UpdateLabels();
        UpdateSynergyHints();
        _saveHelpLabel.Text = $"Loaded slot {slot + 1}!";
    }

    private void ShowSlotHint(int slot)
    {
        var ids = SaveManager.Instance.GetLoadoutSlot(slot);
        if (ids != null && ids.Count > 0)
            _saveHelpLabel.Text = $"Slot {slot + 1}: {ids.Count} towers | Click=Load";
        else
            _saveHelpLabel.Text = $"Slot {slot + 1}: empty | Click=Save";
    }

    private void UpdateSlotTooltips()
    {
        for (int s = 0; s < 3; s++)
        {
            var ids = SaveManager.Instance.GetLoadoutSlot(s);
            if (ids == null || ids.Count == 0)
            {
                _slotButtons[s].TooltipText = "Empty slot\nLeft-click to save current loadout";
                continue;
            }

            var names = new List<string>();
            foreach (var id in ids)
            {
                var data = _allTowers.Find(t => t.Id == id);
                if (data != null) names.Add(data.TowerName);
            }
            _slotButtons[s].TooltipText = string.Join("\n", names) + "\nLeft-click: Load | Right-click: Overwrite";
        }
    }

    private void OnStartRunPressed()
    {
        var ids = new Array<string>();
        for (int i = 0; i < _selected.Length; i++)
            if (_selected[i]) ids.Add(_allTowers[i].Id);

        RunState.Instance.StartRun(EconomyManager.Instance.StartingMoney, GameManager.Instance.StartingLives, ids);
        LevelManager.Instance.PickRandomLevel();
        UIManager.Instance.PushScreen(UIManager.Instance.BriefingData);
    }
}
