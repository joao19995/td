using Godot;
using Godot.Collections; 
public partial class EnemySpawner : Node2D
{
    [Export] public Path2D EnemyPath;
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
            SpawnEnemy(wave.EnemyScenes[i % wave.EnemyScenes.Count]);
            await ToSignal(GetTree().CreateTimer(wave.SpawnInterval), Timer.SignalName.Timeout);
        }

        _waveInProgress = false;
    }

    private void SpawnEnemy(PackedScene enemyScene)
    {
        if (EnemyPath == null || enemyScene == null)
        {
            GD.PrintErr("EnemySpawner: EnemyPath ou enemyScene não estão atribuídos.");
            return;
        }

        var enemy = enemyScene.Instantiate<Enemy>();
        enemy.Initialize(EnemyPath.Curve);
        GetTree().CurrentScene.CallDeferred(Node.MethodName.AddChild, enemy);
    }
}