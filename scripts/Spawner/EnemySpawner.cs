using Godot;
using Godot.Collections;

public partial class EnemySpawner : Node2D
{
    [Export] public Path2D EnemyPath;
    // Assign res://scenes/enemies/Enemy.tscn (the single generic enemy scene).
    [Export] public PackedScene GenericEnemyScene;
    [Export] public Array<WaveData> Waves;

    private int _currentWaveIndex = -1;
    private bool _waveInProgress = false;

    public bool CanStartNextWave => !_waveInProgress && _currentWaveIndex + 1 < Waves.Count;
    public string CurrentWaveDisplay => $"{_currentWaveIndex + 1} / {Waves.Count}";

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
            SpawnEnemy(wave.Enemies[i % wave.Enemies.Count]);
            await ToSignal(GetTree().CreateTimer(wave.SpawnInterval), Timer.SignalName.Timeout);
        }

        _waveInProgress = false;

        if (_currentWaveIndex + 1 >= Waves.Count)
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

        var enemy = EnemyFactory.Create(GenericEnemyScene, enemyData, EnemyPath.Curve);
        GetEnemiesContainer().CallDeferred(Node.MethodName.AddChild, enemy);
    }
}
