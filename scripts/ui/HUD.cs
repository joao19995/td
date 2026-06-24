using Godot;

public partial class HUD : CanvasLayer
{
    [Export] public PackedScene TowerScene;
    [Export] public int TowerCost = 50;

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
        _towerButton.Pressed += OnTowerButtonPressed;

        EventBus.Instance.LivesChanged += OnLivesChanged;
        EventBus.Instance.MoneyChanged += OnMoneyChanged;
        EventBus.Instance.GameOver += OnGameOver;

        // Estado inicial
        UpdateLivesLabel(GameManager.Instance.CurrentLives);
        UpdateMoneyLabel(EconomyManager.Instance.CurrentMoney);
    }

    // Chamado pelo Map1 depois de instanciar o mapa, para a HUD saber a quem se ligar
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

    private void OnTowerButtonPressed()
    {
        if (_activeTileMap == null) return;
        if (!EconomyManager.Instance.CanAfford(TowerCost)) return;

        TowerPlacementManager.Instance.StartPlacement(TowerScene, _activeTileMap);
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
        // Fase 12 vai mostrar aqui um painel de "Game Over" completo
        GD.Print("GAME OVER");
    }
}