using Godot;
using System.Collections.Generic;
using System.Text;

public partial class BestiaryScreen : Control
{
    private enum Category { Towers, Enemies, Equipment, Trinkets, Synergies }

    private VBoxContainer _contentVBox;
    private Button _backButton;
    private readonly List<Button> _categoryButtons = new();
    private Category _currentCategory = Category.Towers;

    private List<TowerData> _towers = new();
    private List<EnemyData> _enemies = new();
    private List<EquipData> _equips = new();
    private List<TrinketData> _trinkets = new();
    private List<SynergyData> _synergies = new();

    public override void _Ready()
    {
        var bg = new ColorRect();
        bg.LayoutMode = 1;
        bg.AnchorRight = 1f;
        bg.AnchorBottom = 1f;
        bg.Color = new Color(0, 0, 0, 0.85f);
        AddChild(bg);

        var vbox = new VBoxContainer();
        vbox.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        vbox.OffsetLeft = 6f;
        vbox.OffsetTop = 4f;
        vbox.OffsetRight = -6f;
        vbox.OffsetBottom = -4f;
        AddChild(vbox);

        var topBar = new HBoxContainer();
        var title = new Label();
        title.Text = "BESTIARY";
        title.HorizontalAlignment = HorizontalAlignment.Center;
        title.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        _backButton = new Button();
        _backButton.Text = "X";
        _backButton.Pressed += () => UIManager.Instance?.PopScreen();
        topBar.AddChild(title);
        topBar.AddChild(_backButton);
        vbox.AddChild(topBar);

        var catBar = new HBoxContainer();
        foreach (Category cat in System.Enum.GetValues<Category>())
        {
            var btn = new Button();
            btn.Text = cat switch
            {
                Category.Towers => "Towers",
                Category.Enemies => "Enemies",
                Category.Equipment => "Equip",
                Category.Trinkets => "Trinkets",
                Category.Synergies => "Synergies",
                _ => cat.ToString()
            };
            btn.ToggleMode = true;
            var captured = cat;
            btn.Pressed += () => SelectCategory(captured);
            _categoryButtons.Add(btn);
            catBar.AddChild(btn);
        }
        vbox.AddChild(catBar);

        var scroll = new ScrollContainer();
        scroll.SizeFlagsVertical = SizeFlags.ExpandFill;
        _contentVBox = new VBoxContainer();
        _contentVBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        scroll.AddChild(_contentVBox);
        vbox.AddChild(scroll);

        LoadAllData();
        SelectCategory(Category.Towers);
    }

    private void LoadAllData()
    {
        _towers = LoadFromDir<TowerData>("res://resources/tower_data/");
        _enemies = LoadFromDir<EnemyData>("res://resources/enemy_data/");
        _equips = LoadFromDir<EquipData>("res://resources/equip_data/");
        _trinkets = LoadFromDir<TrinketData>("res://resources/trinket_data/");
        _synergies = LoadFromDir<SynergyData>("res://resources/synergy_data/");
    }

    private static List<T> LoadFromDir<T>(string dirPath) where T : Resource
    {
        var items = new List<T>();
        var dir = DirAccess.Open(dirPath);
        if (dir == null) return items;

        foreach (var file in dir.GetFiles())
        {
            if (!file.EndsWith(".tres") && !file.EndsWith(".res"))
                continue;
            var item = ResourceLoader.Load<T>(dirPath + file, "", ResourceLoader.CacheMode.Replace);
            if (item != null)
                items.Add(item);
        }

        return items;
    }

    private void SelectCategory(Category cat)
    {
        _currentCategory = cat;
        for (int i = 0; i < _categoryButtons.Count; i++)
            _categoryButtons[i].ButtonPressed = i == (int)cat;

        foreach (var child in _contentVBox.GetChildren())
            child.QueueFree();

        switch (cat)
        {
            case Category.Towers: PopulateTowers(); break;
            case Category.Enemies: PopulateEnemies(); break;
            case Category.Equipment: PopulateEquipment(); break;
            case Category.Trinkets: PopulateTrinkets(); break;
            case Category.Synergies: PopulateSynergies(); break;
        }
    }

    private void AddEntry(string text, bool locked = false)
    {
        var label = new Label();
        label.Text = text;
        if (locked)
            label.Modulate = new Color(0.4f, 0.4f, 0.4f);
        _contentVBox.AddChild(label);
    }

    private void PopulateTowers()
    {
        foreach (var t in _towers)
        {
            bool unlocked = SaveManager.Instance?.IsTowerUnlocked(t.Id) ?? true;
            if (unlocked)
            {
                var sb = new StringBuilder();
                sb.Append($"{t.TowerName}  DMG:{t.Damage} SPD:{t.FireRate} RNG:{t.Range} Cost:{t.Cost}");
                if (t.HasSplash) sb.Append(" [SPLASH]");
                if (t.HasPoison) sb.Append(" [POISON]");
                if (t.HasSlow) sb.Append(" [SLOW]");
                AddEntry(sb.ToString());
            }
            else
            {
                AddEntry($"{t.TowerName} (LOCKED)  ???", true);
            }
        }
    }

    private void PopulateEnemies()
    {
        foreach (var e in _enemies)
        {
            bool discovered = SaveManager.Instance?.IsDiscovered($"enemy_{e.Id}") ?? true;
            if (discovered)
                AddEntry($"{e.EnemyName}  HP:{e.MaxHealth} SPD:{e.MoveSpeed} Gold:{e.RewardGold} DMG:{e.DamageToPlayer}");
            else
                AddEntry("???  HP:??? SPD:??? Gold:??? DMG:???", true);
        }
    }

    private void PopulateEquipment()
    {
        foreach (var eq in _equips)
        {
            bool discovered = SaveManager.Instance?.IsDiscovered($"equip_{eq.Id}") ?? true;
            if (discovered)
            {
                var sb = new StringBuilder();
                sb.Append($"{eq.Name}  [{eq.TargetTowerId.ToUpper()}] {eq.Cost}g");
                if (eq.DamagePercentBonus > 0) sb.Append($" +{eq.DamagePercentBonus * 100f:F0}%DMG");
                if (eq.FireRatePercentBonus > 0) sb.Append($" +{eq.FireRatePercentBonus * 100f:F0}%SPD");
                if (eq.RangePercentBonus > 0) sb.Append($" +{eq.RangePercentBonus * 100f:F0}%RNG");
                AddEntry(sb.ToString());
            }
            else
            {
                AddEntry("???  [???] ???g", true);
            }
        }
    }

    private void PopulateTrinkets()
    {
        foreach (var tr in _trinkets)
        {
            bool discovered = SaveManager.Instance?.IsDiscovered($"trinket_{tr.Id}") ?? true;
            if (discovered)
            {
                var sb = new StringBuilder();
                sb.Append($"{tr.Name}  ");
                if (tr.DamagePercentBonus > 0) sb.Append($"+{tr.DamagePercentBonus * 100f:F0}%DMG ");
                if (tr.HealAmount > 0) sb.Append($"+{tr.HealAmount}Lives ");
                if (tr.GoldAmount > 0) sb.Append($"+{tr.GoldAmount}Gold");
                AddEntry(sb.ToString().Trim());
            }
            else
            {
                AddEntry("???  +???", true);
            }
        }
    }

    private void PopulateSynergies()
    {
        foreach (var s in _synergies)
        {
            bool discovered = SaveManager.Instance?.IsDiscovered($"synergy_{s.Id}") ?? true;
            if (discovered)
            {
                var sb = new StringBuilder();
                sb.Append($"{s.DisplayName}  {s.Description}");
                AddEntry(sb.ToString());
            }
            else
            {
                AddEntry("???  ???", true);
            }
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("pause_game"))
        {
            GetViewport().SetInputAsHandled();
            UIManager.Instance?.PopScreen();
        }
    }
}
