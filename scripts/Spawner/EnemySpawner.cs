using Godot;
using Godot.Collections;

public partial class EnemySpawner : Node2D
{
    [Export] public Path2D EnemyPath;
    [Export] public PackedScene GenericEnemyScene;
    [Export] public Array<WaveData> Waves;
    [Export] public WaveData BossWaveData;

    private int _currentWaveIndex = -1;
    private bool _waveInProgress = false;
    private int _activeEnemyCount = 0;
    private bool _allWavesSpawned = false;

    private bool _isBossFight = false;
    private bool _isMiniboss = false;
    private float _minibossMultiplier = 1.5f;

    public bool CanStartNextWave => !_waveInProgress && _currentWaveIndex + 1 < Waves.Count;
    public string CurrentWaveDisplay => $"{_currentWaveIndex + 1} / {Waves.Count}";

    public void ConfigureForRun(bool isBoss, bool isMiniboss, float minibossMult, Array<WaveData> runWaves = null)
    {
        _isBossFight = isBoss;
        _isMiniboss = isMiniboss;
        _minibossMultiplier = minibossMult;
        if (_isBossFight)
        {
            var bossWave = BossWaveData ?? GD.Load<WaveData>("res://resources/run_data/BossWave.tres");
            if (bossWave != null)
                Waves = new Array<WaveData> { bossWave };
        }
        else if (runWaves != null && runWaves.Count > 0)
        {
            Waves = runWaves;
        }
    }

    public override void _Ready()
    {
        EventBus.Instance.EnemyDied += OnEnemyDied;
        EventBus.Instance.EnemyReachedEnd += OnEnemyReachedEnd;
    }

    public override void _ExitTree()
    {
        if (EventBus.Instance != null)
        {
            EventBus.Instance.EnemyDied -= OnEnemyDied;
            EventBus.Instance.EnemyReachedEnd -= OnEnemyReachedEnd;
        }
    }

    public async void StartNextWave()
    {
        if (!CanStartNextWave) return;

        _currentWaveIndex++;
        _waveInProgress = true;
        var wave = Waves[_currentWaveIndex];

        if (wave.Enemies == null || wave.Enemies.Count == 0)
        {
            GD.PrintErr($"EnemySpawner: Wave {_currentWaveIndex} has no enemies defined.");
            _waveInProgress = false;
            return;
        }

        for (int i = 0; i < wave.EnemyCount; i++)
        {
            _activeEnemyCount++;
            SpawnEnemy(wave.Enemies[i % wave.Enemies.Count]);
            await ToSignal(GetTree().CreateTimer(wave.SpawnInterval), Timer.SignalName.Timeout);
        }

        _waveInProgress = false;

        if (_currentWaveIndex + 1 >= Waves.Count)
            _allWavesSpawned = true;

        CheckAllWavesCompleted();
    }

    private void OnEnemyDied(int _)
    {
        _activeEnemyCount--;
        CheckAllWavesCompleted();
    }

    private void OnEnemyReachedEnd(int _)
    {
        _activeEnemyCount--;
        CheckAllWavesCompleted();
    }

    private void CheckAllWavesCompleted()
    {
        if (_allWavesSpawned && _activeEnemyCount <= 0)
            EventBus.Instance?.EmitSignal(EventBus.SignalName.AllWavesCompleted);
    }

    private Node GetEnemiesContainer()
    {
        if (LevelManager.Instance.CurrentLevelNode is BaseLevel level && level.EnemiesContainer != null)
            return level.EnemiesContainer;

        GD.PushWarning("EnemySpawner: EnemiesContainer not found, falling back to level root.");
        return LevelManager.Instance.CurrentLevelNode;
    }

    private void SpawnEnemy(EnemyData enemyData)
    {
        if (EnemyPath == null || GenericEnemyScene == null || enemyData == null)
        {
            GD.PrintErr("EnemySpawner: EnemyPath, GenericEnemyScene, or EnemyData not assigned.");
            return;
        }

        SaveManager.Instance?.MarkDiscovered($"enemy_{enemyData.Id}");

        float mult = _isMiniboss ? _minibossMultiplier : 1f;
        var enemy = EnemyFactory.Create(GenericEnemyScene, enemyData, EnemyPath.Curve, mult);
        GetEnemiesContainer().CallDeferred(Node.MethodName.AddChild, enemy);
    }
}
