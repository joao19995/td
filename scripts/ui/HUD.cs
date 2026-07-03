using Godot;
using System.Collections.Generic;

public partial class HUD : CanvasLayer
{
    private List<TowerData> _availableTowers;

    [Export] public float ButtonHeight = 14f;
    [Export] public float TowerBarSideMargin = 6f;
    [Export] public float TowerBarBottomMargin = 4f;
    [Export] public float WaveButtonWidth = 56f;

    private HBoxContainer _heartsContainer;
    private Label _moneyLabel;
    private Label _waveLabel;
    private HBoxContainer _waveBar;
    private HBoxContainer _towerBar;

    private Button _nextWaveButton;
    private readonly List<Button> _allTowerButtons = new();

    private TileMapLayer _activeTileMap;
    private EnemySpawner _activeSpawner;

    private VBoxContainer _towerActionPanel;
    private Label _towerNameLabel;
    private Label _statsLabel;
    private Label _equipLabel;
    private Button _upgradeButton;

    private Label _synergyLabel;
    private HBoxContainer _buffIconsContainer;
    private Label _tooltipLabel;
    private EquipData _hoveredEquipData;
    private bool _equipSignalConnected;
    private TextureRect _equipIcon;
    private Label _equipDesc;

    public override void _Ready()
    {
        Visible = false;
        _moneyLabel = GetNode<Label>("InfoBar/MoneyLabel");
        _waveLabel = GetNode<Label>("InfoBar/WaveLabel");
        _waveBar = GetNode<HBoxContainer>("WaveBar");
        _towerBar = GetNode<HBoxContainer>("TowerBar");

        _heartsContainer = new HBoxContainer();
        _heartsContainer.Name = "HeartsContainer";
        _heartsContainer.OffsetLeft = 0;
        _heartsContainer.OffsetTop = 0;
        var infoBar = GetNode<HBoxContainer>("InfoBar");
        infoBar.AddChild(_heartsContainer);
        infoBar.MoveChild(_heartsContainer, 1);

        _tooltipLabel = new Label();
        _tooltipLabel.Name = "TooltipLabel";
        _tooltipLabel.Visible = false;
        _tooltipLabel.MouseFilter = Control.MouseFilterEnum.Ignore;
        _tooltipLabel.Modulate = new Color(0.95f, 0.95f, 0.85f);
        _tooltipLabel.AutowrapMode = TextServer.AutowrapMode.Word;
        _tooltipLabel.MaxLinesVisible = 6;
        _tooltipLabel.CustomMinimumSize = new Vector2(60, 14);
        var tooltipBg = new StyleBoxFlat();
        tooltipBg.BgColor = new Color(0.05f, 0.05f, 0.08f, 0.92f);
        tooltipBg.BorderColor = new Color(0.6f, 0.6f, 0.6f);
        tooltipBg.BorderWidthLeft = 1;
        tooltipBg.BorderWidthRight = 1;
        tooltipBg.BorderWidthTop = 1;
        tooltipBg.BorderWidthBottom = 1;
        tooltipBg.ContentMarginLeft = 4;
        tooltipBg.ContentMarginRight = 4;
        tooltipBg.ContentMarginTop = 2;
        tooltipBg.ContentMarginBottom = 2;
        _tooltipLabel.AddThemeStyleboxOverride("normal", tooltipBg);
        AddChild(_tooltipLabel);

        _buffIconsContainer = new HBoxContainer();
        _buffIconsContainer.Name = "BuffIconsContainer";
        _buffIconsContainer.OffsetLeft = 2;
        _buffIconsContainer.OffsetTop = 42;
        AddChild(_buffIconsContainer);

        PositionTowerBar();
        _availableTowers = LoadAllTowers();
        BuildWaveButtons();
        BuildTowerButtons();

        EventBus.Instance.LivesChanged += OnLivesChanged;
        EventBus.Instance.MoneyChanged += OnMoneyChanged;
        EventBus.Instance.GameOver += OnGameOver;
        EventBus.Instance.AllWavesCompleted += OnAllWavesCompleted;
        EventBus.Instance.TowerPlaced += OnTowerPlaced;

        _towerActionPanel = GetNode<VBoxContainer>("TowerActionPanel");
        _towerNameLabel = GetNode<Label>("TowerActionPanel/TowerNameLabel");
        _statsLabel = GetNode<Label>("TowerActionPanel/StatsLabel");
        _equipLabel = GetNode<Label>("TowerActionPanel/EquipLabel");

        _equipIcon = new TextureRect();
        _equipIcon.Name = "EquipIcon";
        _equipIcon.CustomMinimumSize = new Vector2(16, 16);
        _equipIcon.StretchMode = TextureRect.StretchModeEnum.Keep;
        _towerActionPanel.AddChild(_equipIcon);

        _equipDesc = new Label();
        _equipDesc.Name = "EquipDesc";
        _towerActionPanel.AddChild(_equipDesc);

        _towerActionPanel.MoveChild(_equipIcon, 3);
        _towerActionPanel.MoveChild(_equipDesc, 4);

        _upgradeButton = GetNode<Button>("TowerActionPanel/UpgradeButton");

        TowerSelectionManager.Instance.TowerSelected += OnTowerSelected;
        TowerSelectionManager.Instance.TowerDeselected += OnTowerDeselected;
        _upgradeButton.Pressed += OnUpgradePressed;

        _synergyLabel = GetNode<Label>("SynergyLabel");
        _synergyLabel.MouseEntered += OnSynergyLabelMouseEntered;
        _synergyLabel.MouseExited += HideTooltip;
        SynergyManager.Instance.SynergiesChanged += OnSynergiesChanged;

        UpdateHearts(GameManager.Instance.CurrentLives);
        UpdateMoneyLabel(EconomyManager.Instance.CurrentMoney);
    }

    public override void _ExitTree()
    {
        EventBus.Instance.LivesChanged -= OnLivesChanged;
        EventBus.Instance.MoneyChanged -= OnMoneyChanged;
        EventBus.Instance.GameOver -= OnGameOver;
        EventBus.Instance.AllWavesCompleted -= OnAllWavesCompleted;
        EventBus.Instance.TowerPlaced -= OnTowerPlaced;

        if (TowerSelectionManager.Instance != null)
        {
            TowerSelectionManager.Instance.TowerSelected -= OnTowerSelected;
            TowerSelectionManager.Instance.TowerDeselected -= OnTowerDeselected;
        }
    }

    private void ShowTooltip(string text)
    {
        _tooltipLabel.Text = text;
        float mouseX = GetViewport().GetMousePosition().X;
        float mouseY = GetViewport().GetMousePosition().Y;
        _tooltipLabel.OffsetLeft = Mathf.Clamp(mouseX + 8, 2, 300);
        if (mouseY < 80)
            _tooltipLabel.OffsetTop = Mathf.Clamp(mouseY + 16, 16, 180);
        else
            _tooltipLabel.OffsetTop = Mathf.Clamp(mouseY - _tooltipLabel.Size.Y - 4, 2, 170);
        _tooltipLabel.Show();
    }

    private void HideTooltip()
    {
        _tooltipLabel.Hide();
    }

    private void PositionTowerBar()
    {
        _towerBar.OffsetLeft = TowerBarSideMargin;
        _towerBar.OffsetRight = -TowerBarSideMargin;
        _towerBar.OffsetBottom = -TowerBarBottomMargin;
        _towerBar.OffsetTop = -TowerBarBottomMargin - ButtonHeight;
    }

    private void BuildWaveButtons()
    {
        var size = new Vector2(WaveButtonWidth, ButtonHeight);
        _nextWaveButton = CreateButton(_waveBar, "Next Wave", size, OnNextWavePressed);
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

    private void BuildTowerButtons()
    {
        if (_availableTowers == null || _availableTowers.Count == 0) return;

        float barWidth = GetViewport().GetVisibleRect().Size.X - (TowerBarSideMargin * 2f);
        var size = new Vector2(barWidth / _availableTowers.Count, ButtonHeight);

        foreach (var towerData in _availableTowers)
        {
            var button = CreateButton(_towerBar, $"{towerData.TowerName} ({towerData.Cost}g)", size,
                () => OnTowerButtonPressed(towerData));

            var captured = towerData;
            button.MouseEntered += () =>
            {
                string tooltipText = $"{captured.TowerName}\nCost: {captured.Cost}g\nDMG: {captured.Damage} | SPD: {captured.FireRate:F2} | RNG: {captured.Range}";
                ShowTooltip(tooltipText);
            };
            button.MouseExited += HideTooltip;

            _allTowerButtons.Add(button);
        }
    }

    private Button CreateButton(HBoxContainer parent, string text, Vector2 size, System.Action onPressed)
    {
        var button = new Button();
        button.Text = text;
        button.ClipText = true;
        button.CustomMinimumSize = size;
        button.SizeFlagsHorizontal = Control.SizeFlags.Fill;
        button.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
        button.Pressed += onPressed;
        parent.AddChild(button);
        return button;
    }

    public void SetActiveMap(TileMapLayer tileMap, EnemySpawner spawner)
    {
        Visible = true;
        _activeTileMap = tileMap;
        _activeSpawner = spawner;

        _nextWaveButton.Visible = true;
        _nextWaveButton.Disabled = false;

        OnTowerDeselected();
        ApplyTowerFilter();
        UpdateWaveLabel();
        PopulateBuffIcons();
    }

    private void PopulateBuffIcons()
    {
        foreach (var child in _buffIconsContainer.GetChildren())
        {
            _buffIconsContainer.RemoveChild(child);
            child.QueueFree();
        }

        if (RunState.Instance == null) return;

        for (int i = 0; i < RunState.Instance.PurchasedItemIcons.Count; i++)
        {
            int captured = i;
            var iconRect = new TextureRect();
            iconRect.Texture = RunState.Instance.PurchasedItemIcons[captured];
            iconRect.CustomMinimumSize = new Vector2(16, 16);
            iconRect.StretchMode = TextureRect.StretchModeEnum.Keep;
            iconRect.MouseEntered += () =>
            {
                if (captured < RunState.Instance.PurchasedItemNames.Count)
                    ShowTooltip($"{RunState.Instance.PurchasedItemNames[captured]}: {RunState.Instance.PurchasedItemDescriptions[captured]}");
            };
            iconRect.MouseExited += HideTooltip;
            _buffIconsContainer.AddChild(iconRect);
        }

        string trinketId = RunState.Instance.AppliedTrinketIds.Count > 0 ? null : null;
        foreach (var tid in RunState.Instance.AppliedTrinketIds)
        {
            var trinketPath = $"res://resources/trinket_data/{tid}.tres";
            var trinketData = GD.Load<TrinketData>(trinketPath);
            if (trinketData == null) continue;

            var trinketIcon = new TextureRect();
            trinketIcon.Texture = trinketData.Icon;
            trinketIcon.CustomMinimumSize = new Vector2(16, 16);
            trinketIcon.StretchMode = TextureRect.StretchModeEnum.Keep;
            var capturedTrinket = trinketData;
            trinketIcon.MouseEntered += () => ShowTooltip($"{capturedTrinket.Name}: {capturedTrinket.Description}");
            trinketIcon.MouseExited += HideTooltip;
            _buffIconsContainer.AddChild(trinketIcon);
        }

        var activeSynergies = SynergyManager.Instance?.GetActiveSynergies();
        if (activeSynergies != null)
        {
            foreach (var synergy in activeSynergies)
            {
                if (synergy.Icon == null) continue;

                var synergyIcon = new TextureRect();
                synergyIcon.Texture = synergy.Icon;
                synergyIcon.CustomMinimumSize = new Vector2(16, 16);
                synergyIcon.StretchMode = TextureRect.StretchModeEnum.Keep;
                var capturedSynergy = synergy;
                synergyIcon.MouseEntered += () => ShowTooltip($"{capturedSynergy.DisplayName}: {capturedSynergy.Description}");
                synergyIcon.MouseExited += HideTooltip;
                _buffIconsContainer.AddChild(synergyIcon);
            }
        }
    }

    private void ApplyTowerFilter()
    {
        for (int i = 0; i < _availableTowers.Count && i < _allTowerButtons.Count; i++)
        {
            var data = _availableTowers[i];
            var btn = _allTowerButtons[i];
            bool visible = RunState.Instance.SelectedTowerIds.Contains(data.Id);
            btn.Visible = visible;
            btn.Disabled = !visible;
        }
        RefreshTowerButtons();
    }

    private void RefreshTowerButtons()
    {
        if (_allTowerButtons.Count == 0 || !Visible) return;

        int visibleCount = 0;
        for (int i = 0; i < _availableTowers.Count && i < _allTowerButtons.Count; i++)
            if (_allTowerButtons[i].Visible) visibleCount++;

        if (visibleCount > 0)
        {
            float barWidth = GetViewport().GetVisibleRect().Size.X - (TowerBarSideMargin * 2f);
            var size = new Vector2(barWidth / visibleCount, ButtonHeight);
            for (int i = 0; i < _availableTowers.Count && i < _allTowerButtons.Count; i++)
            {
                var btn = _allTowerButtons[i];
                if (btn.Visible)
                    btn.CustomMinimumSize = size;
            }
        }

        for (int i = 0; i < _availableTowers.Count && i < _allTowerButtons.Count; i++)
        {
            var btn = _allTowerButtons[i];
            if (!btn.Visible) continue;
            var data = _availableTowers[i];
            bool placed = TowerPlacementManager.Instance.IsTowerTypePlaced(data.Id);
            btn.Disabled = placed || !EconomyManager.Instance.CanAfford(data.Cost);
            btn.Text = placed ? $"{data.TowerName} (Placed)" : $"{data.TowerName} ({data.Cost}g)";
        }
    }

    private void OnNextWavePressed()
    {
        if (_activeSpawner == null || !_activeSpawner.CanStartNextWave) return;
        _activeSpawner.StartNextWave();
        UpdateWaveLabel();
    }

    private void OnTowerButtonPressed(TowerData towerData)
    {
        if (_activeTileMap == null) return;
        if (!EconomyManager.Instance.CanAfford(towerData.Cost)) return;
        TowerPlacementManager.Instance.StartPlacement(towerData, _activeTileMap);
    }

    private void OnAllWavesCompleted()
    {
        _nextWaveButton.Visible = false;
    }

    private void OnLivesChanged(int currentLives)
    {
        UpdateHearts(currentLives);
    }

    private void OnMoneyChanged(int currentMoney)
    {
        UpdateMoneyLabel(currentMoney);
        RefreshTowerButtons();
    }

    private void UpdateHearts(int lives)
    {
        foreach (var child in _heartsContainer.GetChildren())
        {
            _heartsContainer.RemoveChild(child);
            child.QueueFree();
        }

        for (int i = 0; i < lives && i < 20; i++)
        {
            var heart = new ColorRect();
            heart.Color = new Color(1f, 0.15f, 0.15f);
            heart.CustomMinimumSize = new Vector2(4, 4);
            heart.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
            heart.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
            _heartsContainer.AddChild(heart);
        }

        var livesCount = new Label();
        livesCount.Text = $"{lives}";
        livesCount.Modulate = new Color(0.9f, 0.9f, 0.9f);
        _heartsContainer.AddChild(livesCount);
    }

    private void UpdateMoneyLabel(int money) => _moneyLabel.Text = $"$: {money}";

    private void UpdateWaveLabel()
    {
        if (_activeSpawner == null) return;
        _waveLabel.Text = $"W: {_activeSpawner.CurrentWaveDisplay}";
    }

    private void OnSynergiesChanged()
    {
        _synergyLabel.Text = SynergyManager.Instance.GetActiveDisplayText();
        PopulateBuffIcons();
    }

    private void OnSynergyLabelMouseEntered()
    {
        string text = SynergyManager.Instance.GetActiveDisplayText();
        if (string.IsNullOrEmpty(text)) return;
        ShowTooltip(text);
    }

    private void OnTowerSelected(Tower tower)
    {
        _towerNameLabel.Text = $"{tower.Data.TowerName} (Lv.{tower.CurrentUpgradeLevel + 1})";
        _statsLabel.Text = $"DMG:{tower.EffectiveDamage:F1} SPD:{tower.EffectiveFireRate:F1} RNG:{tower.EffectiveRange:F0}";

        string equipId = RunState.Instance?.GetEquippedItem(tower.Data.Id);
        if (!string.IsNullOrEmpty(equipId))
        {
            var equipData = GD.Load<EquipData>($"res://resources/equip_data/{equipId}.tres");
            _equipLabel.Text = equipData?.Name ?? equipId.Replace("_", " ").ToUpper();
            _equipIcon.Texture = equipData?.Icon;
            _equipIcon.Visible = equipData?.Icon != null;

            _equipDesc.Text = BuildEquipStatsText(equipData);
            _equipDesc.Visible = true;

            if (!_equipSignalConnected)
            {
                _equipLabel.MouseEntered += OnEquipLabelMouseEntered;
                _equipLabel.MouseExited += OnEquipLabelMouseExited;
                _equipSignalConnected = true;
            }
            _hoveredEquipData = equipData;
        }
        else
        {
            _equipLabel.Text = "";
            _equipIcon.Texture = null;
            _equipIcon.Visible = false;
            _equipDesc.Text = "";
            _equipDesc.Visible = false;
            _hoveredEquipData = null;
        }

        if (tower.CurrentUpgradeLevel >= tower.MaxUpgradeLevel)
        {
            _upgradeButton.Text = "MAX";
            _upgradeButton.Disabled = true;
        }
        else
        {
            var next = tower.Data.UpgradePath[tower.CurrentUpgradeLevel];
            _upgradeButton.Text = $"Upgrade ({next.Cost}g)";
            _upgradeButton.Disabled = !EconomyManager.Instance.CanAfford(next.Cost);
        }

        _towerActionPanel.Show();
    }

    private void OnTowerDeselected()
    {
        _towerActionPanel.Hide();
        HideTooltip();
        _equipIcon.Visible = false;
        _equipDesc.Visible = false;
        if (_equipSignalConnected)
        {
            _equipLabel.MouseEntered -= OnEquipLabelMouseEntered;
            _equipLabel.MouseExited -= OnEquipLabelMouseExited;
            _equipSignalConnected = false;
        }
        _hoveredEquipData = null;
        RefreshTowerButtons();
    }

    private static string BuildEquipStatsText(EquipData equip)
    {
        if (equip == null) return "";
        var parts = new System.Collections.Generic.List<string>();
        if (equip.DamagePercentBonus != 0f)
            parts.Add($"{(equip.DamagePercentBonus > 0f ? "+" : "")}{equip.DamagePercentBonus * 100f:F0}% DMG");
        if (equip.FireRatePercentBonus != 0f)
            parts.Add($"{(equip.FireRatePercentBonus > 0f ? "+" : "")}{equip.FireRatePercentBonus * 100f:F0}% FR");
        if (equip.RangePercentBonus != 0f)
            parts.Add($"{(equip.RangePercentBonus > 0f ? "+" : "")}{equip.RangePercentBonus * 100f:F0}% RNG");
        if (equip.SplashRadiusPercentBonus != 0f)
            parts.Add($"{(equip.SplashRadiusPercentBonus > 0f ? "+" : "")}{equip.SplashRadiusPercentBonus * 100f:F0}% SPLASH");
        if (equip.CritChanceBonus != 0f)
            parts.Add($"+{equip.CritChanceBonus * 100f:F0}% CRIT");
        if (equip.SlowDurationPercentBonus != 0f)
            parts.Add($"{(equip.SlowDurationPercentBonus > 0f ? "+" : "")}{equip.SlowDurationPercentBonus * 100f:F0}% SLOW DUR");
        if (equip.StatusDurationPercentBonus != 0f)
            parts.Add($"{(equip.StatusDurationPercentBonus > 0f ? "+" : "")}{equip.StatusDurationPercentBonus * 100f:F0}% STATUS DUR");
        if (equip.PoisonDamagePercentBonus != 0f)
            parts.Add($"+{equip.PoisonDamagePercentBonus * 100f:F0}% POISON");
        if (equip.EliteDamagePercentBonus != 0f)
            parts.Add($"+{equip.EliteDamagePercentBonus * 100f:F0}% VS ELITE");
        if (equip.PierceBonus > 0)
            parts.Add($"+{equip.PierceBonus} PIERCE");
        if (equip.ExtraChainBounces > 0)
            parts.Add($"+{equip.ExtraChainBounces} BOUNCE");
        if (equip.AuraPotencyMultiplier != 1f)
            parts.Add($"AURA x{equip.AuraPotencyMultiplier:F1}");
        string stats = parts.Count > 0 ? string.Join(" | ", parts) : equip.Description;
        return stats;
    }

    private void OnEquipLabelMouseEntered()
    {
        if (_hoveredEquipData == null) return;
        ShowTooltip($"{_hoveredEquipData.Name}: {_hoveredEquipData.Description}");
    }

    private void OnEquipLabelMouseExited()
    {
        HideTooltip();
    }

    private void OnUpgradePressed()
    {
        var tower = TowerSelectionManager.Instance.SelectedTower;
        if (tower == null || tower.CurrentUpgradeLevel >= tower.MaxUpgradeLevel) return;

        var next = tower.Data.UpgradePath[tower.CurrentUpgradeLevel];
        if (!EconomyManager.Instance.SpendMoney(next.Cost)) return;

        tower.Upgrade();
        OnTowerSelected(tower);
    }

    private void OnTowerPlaced(int _)
    {
        RefreshTowerButtons();
    }

    private void OnGameOver()
    {
        _nextWaveButton.Disabled = true;
        foreach (var btn in _allTowerButtons)
            btn.Disabled = true;
    }
}
