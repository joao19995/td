using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class HUD : CanvasLayer
{
    [Export] public Array<TowerData> AvailableTowers;

    // Single source of truth for sizing/positioning — change here, every button updates.
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
    private Button _nextLevelButton;
    private readonly List<Button> _allTowerButtons = new();

    private TileMapLayer _activeTileMap;
    private EnemySpawner _activeSpawner;

    public override void _Ready()
    {
        Visible = false;
        _livesLabel = GetNode<Label>("InfoBar/LivesLabel");
        _moneyLabel = GetNode<Label>("InfoBar/MoneyLabel");
        _waveLabel = GetNode<Label>("InfoBar/WaveLabel");
        _waveBar = GetNode<HBoxContainer>("WaveBar");
        _towerBar = GetNode<HBoxContainer>("TowerBar");

        PositionTowerBar();
        BuildWaveButtons();
        BuildTowerButtons();

        EventBus.Instance.LivesChanged += OnLivesChanged;
        EventBus.Instance.MoneyChanged += OnMoneyChanged;
        EventBus.Instance.GameOver += OnGameOver;
        EventBus.Instance.AllWavesCompleted += OnAllWavesCompleted;

        UpdateLivesLabel(GameManager.Instance.CurrentLives);
        UpdateMoneyLabel(EconomyManager.Instance.CurrentMoney);
    }

    public override void _ExitTree()
    {
        EventBus.Instance.LivesChanged -= OnLivesChanged;
        EventBus.Instance.MoneyChanged -= OnMoneyChanged;
        EventBus.Instance.GameOver -= OnGameOver;
        EventBus.Instance.AllWavesCompleted -= OnAllWavesCompleted;
    }

    /// <summary>
    /// Anchors the TowerBar near the bottom of the viewport with a side
    /// margin on both edges, computed from ButtonHeight/margins instead of
    /// hardcoded offsets — works regardless of viewport size.
    /// </summary>
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
        _nextLevelButton = CreateButton(_waveBar, "Next Level", size, OnNextLevelPressed);
        _nextLevelButton.Visible = false;
    }

    private void BuildTowerButtons()
    {
        if (AvailableTowers == null || AvailableTowers.Count == 0) return;

        float barWidth = GetViewport().GetVisibleRect().Size.X - (TowerBarSideMargin * 2f);
        var size = new Vector2(barWidth / AvailableTowers.Count, ButtonHeight);

        foreach (var towerData in AvailableTowers)
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
        _nextLevelButton.Visible = false;
        foreach (var btn in _allTowerButtons)
            btn.Disabled = false;

        UpdateWaveLabel();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("test_next_wave"))
            OnNextWavePressed();
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

        if (LevelManager.Instance != null && LevelManager.Instance.HasNextLevel)
            _nextLevelButton.Visible = true;
        else
            EventBus.Instance.EmitSignal(EventBus.SignalName.AllLevelsCompleted);
    }

    private void OnNextLevelPressed()
    {
        _nextLevelButton.Visible = false;
        LevelManager.Instance?.LoadNextLevel();
    }

    private void OnLivesChanged(int currentLives) => UpdateLivesLabel(currentLives);
    private void OnMoneyChanged(int currentMoney) => UpdateMoneyLabel(currentMoney);

    private void UpdateLivesLabel(int lives) => _livesLabel.Text = $"Vida: {lives}";
    private void UpdateMoneyLabel(int money) => _moneyLabel.Text = $"Dinheiro: {money}";

    private void UpdateWaveLabel()
    {
        if (_activeSpawner == null) return;
        _waveLabel.Text = $"Wave: {_activeSpawner.CurrentWaveDisplay}";
    }

    private void OnGameOver()
    {
        _nextWaveButton.Disabled = true;
        _nextLevelButton.Visible = false;
        foreach (var btn in _allTowerButtons)
            btn.Disabled = true;
        GD.Print("GAME OVER");
    }
}