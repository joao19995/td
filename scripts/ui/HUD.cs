using Godot;
using System.Collections.Generic;

public partial class HUD : CanvasLayer
{
    private List<TowerData> _availableTowers;

    [Export] public float ButtonHeight = 14f;
    [Export] public float TowerBarSideMargin = 6f;
    [Export] public float TowerBarBottomMargin = 4f;
    [Export] public float WaveButtonWidth = 56f;

    private Label _livesLabel;
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

    public override void _Ready()
    {
        Visible = false;
        _livesLabel = GetNode<Label>("InfoBar/LivesLabel");
        _moneyLabel = GetNode<Label>("InfoBar/MoneyLabel");
        _waveLabel = GetNode<Label>("InfoBar/WaveLabel");
        _waveBar = GetNode<HBoxContainer>("WaveBar");
        _towerBar = GetNode<HBoxContainer>("TowerBar");

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
        _upgradeButton = GetNode<Button>("TowerActionPanel/UpgradeButton");

        TowerSelectionManager.Instance.TowerSelected += OnTowerSelected;
        TowerSelectionManager.Instance.TowerDeselected += OnTowerDeselected;
        _upgradeButton.Pressed += OnUpgradePressed;

        _synergyLabel = GetNode<Label>("SynergyLabel");
        SynergyManager.Instance.SynergiesChanged += OnSynergiesChanged;

        UpdateLivesLabel(GameManager.Instance.CurrentLives);
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

    private void OnLivesChanged(int currentLives) => UpdateLivesLabel(currentLives);

    private void OnMoneyChanged(int currentMoney)
    {
        UpdateMoneyLabel(currentMoney);
        RefreshTowerButtons();
    }

    private void UpdateLivesLabel(int lives) => _livesLabel.Text = $"Vida: {lives}";
    private void UpdateMoneyLabel(int money) => _moneyLabel.Text = $"Dinheiro: {money}";

    private void UpdateWaveLabel()
    {
        if (_activeSpawner == null) return;
        _waveLabel.Text = $"Wave: {_activeSpawner.CurrentWaveDisplay}";
    }

    private void OnSynergiesChanged()
    {
        _synergyLabel.Text = SynergyManager.Instance.GetActiveDisplayText();
    }

    private void OnTowerSelected(Tower tower)
    {
        _towerNameLabel.Text = $"{tower.Data.TowerName} (Lv.{tower.CurrentUpgradeLevel + 1})";
        _statsLabel.Text = $"DMG:{tower.EffectiveDamage:F1} SPD:{tower.EffectiveFireRate:F1} RNG:{tower.EffectiveRange:F0}";

        string equipId = RunState.Instance?.GetEquippedItem(tower.Data.Id);
        _equipLabel.Text = !string.IsNullOrEmpty(equipId)
            ? $"Equip: {equipId.Replace("_", " ").ToUpper()}"
            : "";

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
        RefreshTowerButtons();
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
