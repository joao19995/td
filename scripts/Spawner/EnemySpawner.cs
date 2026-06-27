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

        for (int i = 0; i < wave.EnemyCount; i++)
        {
            SpawnEnemy(wave.Enemies[i % wave.Enemies.Count]);
            await ToSignal(GetTree().CreateTimer(wave.SpawnInterval), Timer.SignalName.Timeout);
        }

        _waveInProgress = false;

        if (_currentWaveIndex + 1 >= Waves.Count)
            EventBus.Instance?.EmitSignal(EventBus.SignalName.AllWavesCompleted);
    }

    private void SpawnEnemy(EnemyData enemyData)
    {
        if (EnemyPath == null || GenericEnemyScene == null || enemyData == null)
        {
            GD.PrintErr("EnemySpawner: EnemyPath, GenericEnemyScene, or EnemyData not assigned.");
            return;
        }

        var enemy = EnemyFactory.Create(GenericEnemyScene, enemyData, EnemyPath.Curve);
        LevelManager.Instance.CurrentLevelNode.CallDeferred(Node.MethodName.AddChild, enemy);
    }
}
