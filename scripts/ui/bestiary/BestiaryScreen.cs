using Godot;
using System.Collections.Generic;
using System.Text;

public partial class BestiaryScreen : Control
{
    private enum Category { Towers, Enemies, Equipment, Trinkets, Synergies }

    private VBoxContainer _contentVBox;
    private Button _backButton;
    private Label _progressLabel;
    private readonly List<Button> _categoryButtons = new();
    private Category _currentCategory = Category.Towers;

    private List<TowerData> _towers = new();
    private List<EnemyData> _enemies = new();
    private List<EquipData> _equips = new();
    private List<TrinketData> _trinkets = new();
    private List<SynergyData> _synergies = new();

    private float _maxTowerDamage, _maxTowerFireRate, _maxTowerRange;
    private float _maxEnemyHP, _maxEnemySpeed, _maxEnemyGold, _maxEnemyDamage;

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

        _progressLabel = new Label();
        _progressLabel.HorizontalAlignment = HorizontalAlignment.Center;
        _progressLabel.Modulate = new Color(0.7f, 0.7f, 0.7f);
        vbox.AddChild(_progressLabel);

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

        ComputeMaxStats();
    }

    private void ComputeMaxStats()
    {
        _maxTowerDamage = 0; _maxTowerFireRate = 0; _maxTowerRange = 0;
        foreach (var t in _towers)
        {
            if (t.Damage > _maxTowerDamage) _maxTowerDamage = t.Damage;
            if (t.FireRate > _maxTowerFireRate) _maxTowerFireRate = t.FireRate;
            if (t.Range > _maxTowerRange) _maxTowerRange = t.Range;
        }

        _maxEnemyHP = 0; _maxEnemySpeed = 0; _maxEnemyGold = 0; _maxEnemyDamage = 0;
        foreach (var e in _enemies)
        {
            if (e.MaxHealth > _maxEnemyHP) _maxEnemyHP = e.MaxHealth;
            if (e.MoveSpeed > _maxEnemySpeed) _maxEnemySpeed = e.MoveSpeed;
            if (e.RewardGold > _maxEnemyGold) _maxEnemyGold = e.RewardGold;
            if (e.DamageToPlayer > _maxEnemyDamage) _maxEnemyDamage = e.DamageToPlayer;
        }
    }

    private static List<T> LoadFromDir<T>(string dirPath) where T : Resource
    {
        return ResourceLoaderHelper.LoadFromDir<T>(dirPath);
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

    private void UpdateProgress(int found, int total)
    {
        _progressLabel.Text = $"Found: {found} / {total}";
    }

    private HBoxContainer MakeStatBar(string label, float value, float maxValue, int barMaxWidth = 72)
    {
        var hbox = new HBoxContainer();

        var lbl = new Label();
        lbl.Text = label;
        lbl.CustomMinimumSize = new Vector2(32, 0);
        lbl.HorizontalAlignment = HorizontalAlignment.Right;
        hbox.AddChild(lbl);

        var barContainer = new Control();
        barContainer.CustomMinimumSize = new Vector2(barMaxWidth, 8);
        barContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        hbox.AddChild(barContainer);

        var bg = new ColorRect();
        bg.Color = new Color(0.12f, 0.12f, 0.12f);
        bg.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        barContainer.AddChild(bg);

        var fill = new ColorRect();
        fill.Color = new Color(0.35f, 0.75f, 1.0f);
        float ratio = maxValue > 0 ? Mathf.Clamp(value / maxValue, 0, 1) : 0;
        fill.AnchorLeft = 0;
        fill.AnchorRight = ratio;
        fill.AnchorTop = 0;
        fill.AnchorBottom = 1;
        barContainer.AddChild(fill);

        var val = new Label();
        if (value == Mathf.Floor(value))
            val.Text = value.ToString("F0");
        else
            val.Text = value.ToString("F1");
        val.CustomMinimumSize = new Vector2(36, 0);
        hbox.AddChild(val);

        return hbox;
    }

    private VBoxContainer MakeCenteredSprite(Texture2D tex, int size)
    {
        var outer = new HBoxContainer();
        outer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        outer.AddChild(new Control() { SizeFlagsHorizontal = SizeFlags.ExpandFill });

        var texRect = new TextureRect();
        texRect.Texture = tex;
        texRect.CustomMinimumSize = new Vector2(size, size);
        texRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        texRect.ExpandMode = TextureRect.ExpandModeEnum.FitWidth;
        outer.AddChild(texRect);

        outer.AddChild(new Control() { SizeFlagsHorizontal = SizeFlags.ExpandFill });
        var vbox = new VBoxContainer();
        vbox.AddChild(outer);
        return vbox;
    }

    private VBoxContainer MakeEntry(string name, Texture2D icon, Control detailContent, bool locked)
    {
        var entry = new VBoxContainer();
        entry.AddThemeConstantOverride("separation", 0);

        var header = new HBoxContainer();
        header.MouseFilter = MouseFilterEnum.Stop;
        header.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        header.CustomMinimumSize = new Vector2(0, 20);

        var texRect = new TextureRect();
        texRect.Texture = icon;
        texRect.CustomMinimumSize = new Vector2(18, 18);
        texRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        texRect.MouseFilter = MouseFilterEnum.Ignore;
        header.AddChild(texRect);

        var nameLabel = new Label();
        nameLabel.Text = name;
        nameLabel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        nameLabel.MouseFilter = MouseFilterEnum.Ignore;
        if (locked)
            nameLabel.Modulate = new Color(0.4f, 0.4f, 0.4f);
        header.AddChild(nameLabel);

        var arrow = new Label();
        arrow.Text = "▼";
        arrow.MouseFilter = MouseFilterEnum.Ignore;
        arrow.CustomMinimumSize = new Vector2(16, 0);
        arrow.HorizontalAlignment = HorizontalAlignment.Center;
        arrow.Modulate = new Color(0.6f, 0.6f, 0.6f);
        header.AddChild(arrow);

        var detail = new VBoxContainer();
        detail.Visible = false;
        if (detailContent != null)
            detail.AddChild(detailContent);

        header.GuiInput += (@event) =>
        {
            if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left && mb.Pressed && !locked)
            {
                detail.Visible = !detail.Visible;
                arrow.Text = detail.Visible ? "▲" : "▼";
            }
        };

        entry.AddChild(header);
        entry.AddChild(detail);

        if (!locked)
        {
            var sep = new ColorRect();
            sep.Color = new Color(0.2f, 0.2f, 0.2f);
            sep.CustomMinimumSize = new Vector2(0, 1);
            entry.AddChild(sep);
        }

        return entry;
    }

    private VBoxContainer MakeTowerDetail(TowerData t)
    {
        var vbox = new VBoxContainer();
        vbox.AddThemeConstantOverride("separation", 2);

        vbox.AddChild(MakeCenteredSprite(t.Sprite, 48));

        var nameLabel = new Label();
        nameLabel.Text = t.TowerName;
        nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
        nameLabel.AddThemeFontSizeOverride("font_size", 14);
        vbox.AddChild(nameLabel);

        vbox.AddChild(MakeStatBar("DMG", t.Damage, _maxTowerDamage));
        vbox.AddChild(MakeStatBar("SPD", t.FireRate, _maxTowerFireRate));
        vbox.AddChild(MakeStatBar("RNG", t.Range, _maxTowerRange));

        var costLabel = new Label();
        costLabel.Text = $"Cost: {t.Cost}g";
        vbox.AddChild(costLabel);

        var tags = TowerTagHelper.GetTags(t, "GLOBAL AURA");
        if (tags.Count > 0)
        {
            var tagLabel = new Label();
            tagLabel.Text = string.Join("  ", tags);
            tagLabel.Modulate = new Color(0.8f, 0.8f, 0.3f);
            vbox.AddChild(tagLabel);
        }

        if (!string.IsNullOrEmpty(t.FlavorText))
        {
            var flavor = new Label();
            flavor.Text = $"\"{t.FlavorText}\"";
            flavor.Modulate = new Color(0.65f, 0.65f, 0.65f);
            flavor.AutowrapMode = TextServer.AutowrapMode.Word;
            vbox.AddChild(flavor);
        }

        return vbox;
    }

    private VBoxContainer MakeEnemyDetail(EnemyData e)
    {
        var vbox = new VBoxContainer();
        vbox.AddThemeConstantOverride("separation", 2);

        vbox.AddChild(MakeCenteredSprite(e.Sprite, 48));

        var nameLabel = new Label();
        nameLabel.Text = e.EnemyName;
        nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
        nameLabel.AddThemeFontSizeOverride("font_size", 14);
        vbox.AddChild(nameLabel);

        vbox.AddChild(MakeStatBar("HP", e.MaxHealth, _maxEnemyHP));
        vbox.AddChild(MakeStatBar("SPD", e.MoveSpeed, _maxEnemySpeed));
        vbox.AddChild(MakeStatBar("Gold", e.RewardGold, _maxEnemyGold));
        vbox.AddChild(MakeStatBar("DMG", e.DamageToPlayer, _maxEnemyDamage));

        var tags = new List<string>();
        if (e.IsBoss) tags.Add("BOSS");
        if (e.IsHeavy) tags.Add("HEAVY");
        if (e.HasAntiBuffAura) tags.Add("ANTI-BUFF AURA");
        if (tags.Count > 0)
        {
            var tagLabel = new Label();
            tagLabel.Text = string.Join("  ", tags);
            tagLabel.Modulate = new Color(0.8f, 0.3f, 0.3f);
            vbox.AddChild(tagLabel);
        }

        if (!string.IsNullOrEmpty(e.FlavorText))
        {
            var flavor = new Label();
            flavor.Text = $"\"{e.FlavorText}\"";
            flavor.Modulate = new Color(0.65f, 0.65f, 0.65f);
            flavor.AutowrapMode = TextServer.AutowrapMode.Word;
            vbox.AddChild(flavor);
        }

        return vbox;
    }

    private VBoxContainer MakeEquipDetail(EquipData eq)
    {
        var vbox = new VBoxContainer();
        vbox.AddThemeConstantOverride("separation", 2);

        vbox.AddChild(MakeCenteredSprite(eq.Icon, 32));

        var nameLabel = new Label();
        nameLabel.Text = eq.Name;
        nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
        nameLabel.AddThemeFontSizeOverride("font_size", 14);
        vbox.AddChild(nameLabel);

        if (!string.IsNullOrEmpty(eq.Description))
        {
            var desc = new Label();
            desc.Text = eq.Description;
            desc.AutowrapMode = TextServer.AutowrapMode.Word;
            vbox.AddChild(desc);
        }

        var stats = new List<string>();
        if (eq.DamagePercentBonus > 0) stats.Add($"+{eq.DamagePercentBonus * 100f:F0}% DMG");
        if (eq.FireRatePercentBonus > 0) stats.Add($"+{eq.FireRatePercentBonus * 100f:F0}% SPD");
        if (eq.RangePercentBonus > 0) stats.Add($"+{eq.RangePercentBonus * 100f:F0}% RNG");
        if (eq.ExtraChainBounces > 0) stats.Add($"+{eq.ExtraChainBounces} Bounce");
        if (eq.CritChanceBonus > 0) stats.Add($"+{eq.CritChanceBonus * 100f:F0}% Crit");
        if (eq.PierceBonus > 0) stats.Add($"+{eq.PierceBonus} Pierce");
        if (stats.Count > 0)
        {
            var statLabel = new Label();
            statLabel.Text = string.Join("  ", stats);
            vbox.AddChild(statLabel);
        }

        var infoLabel = new Label();
        infoLabel.Text = $"For: {eq.TargetTowerId}  |  Cost: {eq.Cost}g";
        infoLabel.Modulate = new Color(0.7f, 0.7f, 0.7f);
        vbox.AddChild(infoLabel);

        if (!string.IsNullOrEmpty(eq.FlavorText))
        {
            var flavor = new Label();
            flavor.Text = $"\"{eq.FlavorText}\"";
            flavor.Modulate = new Color(0.65f, 0.65f, 0.65f);
            flavor.AutowrapMode = TextServer.AutowrapMode.Word;
            vbox.AddChild(flavor);
        }

        return vbox;
    }

    private VBoxContainer MakeTrinketDetail(TrinketData tr)
    {
        var vbox = new VBoxContainer();
        vbox.AddThemeConstantOverride("separation", 2);

        vbox.AddChild(MakeCenteredSprite(tr.Icon, 32));

        var nameLabel = new Label();
        nameLabel.Text = tr.Name;
        nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
        nameLabel.AddThemeFontSizeOverride("font_size", 14);
        vbox.AddChild(nameLabel);

        if (!string.IsNullOrEmpty(tr.Description))
        {
            var desc = new Label();
            desc.Text = tr.Description;
            desc.AutowrapMode = TextServer.AutowrapMode.Word;
            vbox.AddChild(desc);
        }

        var stats = new List<string>();
        if (tr.DamagePercentBonus > 0) stats.Add($"+{tr.DamagePercentBonus * 100f:F0}% DMG");
        if (tr.FireRateBonusPercent > 0) stats.Add($"+{tr.FireRateBonusPercent * 100f:F0}% SPD");
        if (tr.RangePercentBonus > 0) stats.Add($"+{tr.RangePercentBonus * 100f:F0}% RNG");
        if (tr.HealAmount > 0) stats.Add($"+{tr.HealAmount} Lives");
        if (tr.GoldAmount > 0) stats.Add($"+{tr.GoldAmount} Gold");
        if (tr.CritDamageBonusPercent > 0) stats.Add($"+{tr.CritDamageBonusPercent * 100f:F0}% Crit DMG");
        if (tr.StatusDurationBonusPercent > 0) stats.Add($"+{tr.StatusDurationBonusPercent * 100f:F0}% Status Dur");
        if (tr.PassiveGoldPerInterval > 0) stats.Add($"{tr.PassiveGoldPerInterval}g / {tr.PassiveGoldInterval:F1}s");
        if (stats.Count > 0)
        {
            var statLabel = new Label();
            statLabel.Text = string.Join("  ", stats);
            vbox.AddChild(statLabel);
        }

        if (!string.IsNullOrEmpty(tr.FlavorText))
        {
            var flavor = new Label();
            flavor.Text = $"\"{tr.FlavorText}\"";
            flavor.Modulate = new Color(0.65f, 0.65f, 0.65f);
            flavor.AutowrapMode = TextServer.AutowrapMode.Word;
            vbox.AddChild(flavor);
        }

        return vbox;
    }

    private VBoxContainer MakeSynergyDetail(SynergyData s)
    {
        var vbox = new VBoxContainer();
        vbox.AddThemeConstantOverride("separation", 2);

        vbox.AddChild(MakeCenteredSprite(s.Icon, 32));

        var nameLabel = new Label();
        nameLabel.Text = s.DisplayName;
        nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
        nameLabel.AddThemeFontSizeOverride("font_size", 14);
        vbox.AddChild(nameLabel);

        if (!string.IsNullOrEmpty(s.Description))
        {
            var desc = new Label();
            desc.Text = s.Description;
            desc.AutowrapMode = TextServer.AutowrapMode.Word;
            vbox.AddChild(desc);
        }

        var reqLabel = new Label();
        var reqText = string.Join(", ", s.RequiredTowerIds);
        reqLabel.Text = $"{s.MinTowerCount}+ towers: {reqText}";
        reqLabel.Modulate = new Color(0.7f, 0.7f, 0.7f);
        vbox.AddChild(reqLabel);

        if (!string.IsNullOrEmpty(s.FlavorText))
        {
            var flavor = new Label();
            flavor.Text = $"\"{s.FlavorText}\"";
            flavor.Modulate = new Color(0.65f, 0.65f, 0.65f);
            flavor.AutowrapMode = TextServer.AutowrapMode.Word;
            vbox.AddChild(flavor);
        }

        return vbox;
    }

    private void PopulateTowers()
    {
        int found = 0;
        foreach (var t in _towers)
        {
            bool unlocked = SaveManager.Instance?.IsTowerUnlocked(t.Id) ?? true;
            if (unlocked) found++;

            var sb = new StringBuilder();
            sb.Append($"{t.TowerName}");
            if (!unlocked) sb.Append(" (LOCKED)");
            var detail = unlocked ? MakeTowerDetail(t) : null;
            var entry = MakeEntry(sb.ToString(), t.Sprite, detail, !unlocked);
            _contentVBox.AddChild(entry);
        }
        UpdateProgress(found, _towers.Count);
    }

    private void PopulateEnemies()
    {
        int found = 0;
        foreach (var e in _enemies)
        {
            bool discovered = SaveManager.Instance?.IsDiscovered($"enemy_{e.Id}") ?? true;
            if (discovered) found++;

            string name = discovered ? e.EnemyName : "???";
            var detail = discovered ? MakeEnemyDetail(e) : null;
            var entry = MakeEntry(name, e.Sprite, detail, !discovered);
            _contentVBox.AddChild(entry);
        }
        UpdateProgress(found, _enemies.Count);
    }

    private void PopulateEquipment()
    {
        int found = 0;
        foreach (var eq in _equips)
        {
            bool discovered = SaveManager.Instance?.IsDiscovered($"equip_{eq.Id}") ?? true;
            if (discovered) found++;

            string name = discovered ? eq.Name : "???";
            var detail = discovered ? MakeEquipDetail(eq) : null;
            var entry = MakeEntry(name, eq.Icon, detail, !discovered);
            _contentVBox.AddChild(entry);
        }
        UpdateProgress(found, _equips.Count);
    }

    private void PopulateTrinkets()
    {
        int found = 0;
        foreach (var tr in _trinkets)
        {
            bool discovered = SaveManager.Instance?.IsDiscovered($"trinket_{tr.Id}") ?? true;
            if (discovered) found++;

            string name = discovered ? tr.Name : "???";
            var detail = discovered ? MakeTrinketDetail(tr) : null;
            var entry = MakeEntry(name, tr.Icon, detail, !discovered);
            _contentVBox.AddChild(entry);
        }
        UpdateProgress(found, _trinkets.Count);
    }

    private void PopulateSynergies()
    {
        int found = 0;
        foreach (var s in _synergies)
        {
            bool discovered = SaveManager.Instance?.IsDiscovered($"synergy_{s.Id}") ?? true;
            if (discovered) found++;

            string name = discovered ? s.DisplayName : "???";
            var detail = discovered ? MakeSynergyDetail(s) : null;
            var entry = MakeEntry(name, s.Icon, detail, !discovered);
            _contentVBox.AddChild(entry);
        }
        UpdateProgress(found, _synergies.Count);
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
