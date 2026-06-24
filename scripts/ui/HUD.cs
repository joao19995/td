using Godot;
using Godot.Collections;

public partial class HUD : CanvasLayer
{
    [Export] public Array<TowerData> AvailableTowers;

    private Label _livesLabel;
    private Label _moneyLabel;
    private Label _waveLabel;
    private Button _nextWaveButton;
    private Button _towerButton;

    private TileMapLayer _activeTileMap;
    private EnemySpawner _activeSpawner;

    public override void _Ready()
    {
        _livesLabel = GetNode<Label>("TopBar/LivesLabel");
        _moneyLabel = GetNode<Label>("TopBar/MoneyLabel");
        _waveLabel = GetNode<Label>("TopBar/WaveLabel");
        _nextWaveButton = GetNode<Button>("TopBar/NextWaveButton");
        _towerButton = GetNode<Button>("TopBar/TowerButton");

        _nextWaveButton.Pressed += OnNextWavePressed;

        SetupTowerButtons();

        EventBus.Instance.LivesChanged += OnLivesChanged;
        EventBus.Instance.MoneyChanged += OnMoneyChanged;
        EventBus.Instance.GameOver += OnGameOver;

        UpdateLivesLabel(GameManager.Instance.CurrentLives);
        UpdateMoneyLabel(EconomyManager.Instance.CurrentMoney);
    }

    private void SetupTowerButtons()
    {
        if (AvailableTowers == null || AvailableTowers.Count == 0)
        {
            _towerButton.Visible = false;
            return;
        }

        // Configure the existing TowerButton for the first tower type
        var firstTower = AvailableTowers[0];
        _towerButton.Text = $"{firstTower.TowerName} ({firstTower.Cost}g)";
        _towerButton.Pressed += () => OnTowerButtonPressed(firstTower);

        // Dynamically add buttons for each additional tower type
        for (int i = 1; i < AvailableTowers.Count; i++)
        {
            var towerData = AvailableTowers[i];
            var button = new Button();
            button.Text = $"{towerData.TowerName} ({towerData.Cost}g)";
            button.Pressed += () => OnTowerButtonPressed(towerData);
            _towerButton.GetParent().AddChild(button);
        }
    }

    public void SetActiveMap(TileMapLayer tileMap, EnemySpawner spawner)
    {
        _activeTileMap = tileMap;
        _activeSpawner = spawner;
        UpdateWaveLabel();
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
        _towerButton.Disabled = true;
        GD.Print("GAME OVER");
    }
}
