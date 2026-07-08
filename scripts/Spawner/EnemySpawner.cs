using Godot;
using Godot.Collections;

public partial class EnemySpawner : Node2D
{
    [Export] public Path2D EnemyPath;
    [Export] public PackedScene GenericEnemyScene;
    [Export] public Array<WaveData> Waves;
    [Export] public float EliteChance = 0.2f;

    private int _currentWaveIndex = -1;
    private bool _waveInProgress = false;
    private int _activeEnemyCount = 0;
    private bool _allWavesSpawned = false;

    private bool _isBossFight = false;
    private bool _isMiniboss = false;
    private float _minibossMultiplier = GameBalance.MinibossStatMultiplier;
    private bool _isActive = false;
    private WaveModifier _currentModifier = WaveModifier.None;
    private float _currentDifficultyMult = 1f;

    public bool CanStartNextWave => !_waveInProgress && _currentWaveIndex + 1 < Waves.Count;
    public string CurrentWaveDisplay => $"{_currentWaveIndex + 1} / {Waves.Count}";

    public void ConfigureForRun(bool isBoss, bool isMiniboss, float minibossMult, Array<WaveData> runWaves = null, WaveData bossWave = null)
    {
        _isBossFight = isBoss;
        _isMiniboss = isMiniboss;
        _minibossMultiplier = minibossMult;
        if (_isBossFight)
        {
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
        _isActive = true;
        EventBus.Instance.EnemyDied += OnEnemyDied;
        EventBus.Instance.EnemyReachedEnd += OnEnemyReachedEnd;
    }

    public override void _ExitTree()
    {
        _isActive = false;
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
        GD.Print($"[Spawner] Starting wave {_currentWaveIndex + 1}/{Waves.Count} (mod={wave.Modifier}, diff={wave.DifficultyMultiplier:F2}, entries={wave.Entries?.Count ?? 0}).");

        if (wave.Entries == null || wave.Entries.Count == 0)
        {
            GD.PrintErr($"EnemySpawner: Wave {_currentWaveIndex} has no entries.");
            _waveInProgress = false;
            return;
        }

        _currentModifier = wave.Modifier;
        _currentDifficultyMult = wave.DifficultyMultiplier;

        float spawnInterval = wave.SpawnInterval / Mathf.Sqrt(_currentDifficultyMult);
        if (_currentModifier == WaveModifier.Horde)
            spawnInterval *= GameBalance.HordeSpawnIntervalMultiplier;

        var spawnList = new System.Collections.Generic.List<(EnemyData data, int remaining)>();
        int totalCount = 0;
        foreach (var entry in wave.Entries)
        {
            if (entry?.Enemy == null || entry.Count <= 0) continue;
            int count = _currentModifier == WaveModifier.Horde ? entry.Count * GameBalance.HordeEnemyCountMultiplier : entry.Count;
            if (wave.IsFinalStretch)
                count = Mathf.RoundToInt(count * GameBalance.FinalStretchCountMultiplier);
            spawnList.Add((entry.Enemy, count));
            totalCount += count;
        }

        int totalSpawned = 0;
        while (totalSpawned < totalCount)
        {
            for (int i = 0; i < spawnList.Count; i++)
            {
                var item = spawnList[i];
                if (item.remaining <= 0) continue;

                if (!_isActive) return;
                _activeEnemyCount++;
                SpawnEnemy(item.data);
                spawnList[i] = (item.data, item.remaining - 1);
                totalSpawned++;

                await ToSignal(GetTree().CreateTimer(spawnInterval, false), Timer.SignalName.Timeout);
                if (!_isActive || !IsInstanceValid(this)) return;
            }
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
        {
            if (GameManager.Instance.CurrentLives <= 0)
            {
                GD.Print($"[EnemySpawner] All waves completed but player is dead — suppressing AllWavesCompleted.");
                return;
            }
            EventBus.Instance?.EmitSignal(EventBus.SignalName.AllWavesCompleted);
        }
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
        bool isElite = !_isMiniboss && !enemyData.IsBoss && !enemyData.IsHeavy && GD.Randf() < EliteChance;

        var enemy = EnemyFactory.Create(GenericEnemyScene, enemyData, EnemyPath.Curve, mult, isElite);

        float fightScaling = 1f + RunState.Instance.FightsCompleted * GameBalance.DifficultyScalingPerFight;
        if (fightScaling != 1f)
            enemy.SetModifierMultipliers(fightScaling, fightScaling, 1f);

        switch (_currentModifier)
        {
            case WaveModifier.Armored:
                enemy.SetModifierMultipliers(GameBalance.ArmoredHpMultiplier, GameBalance.ArmoredDamageMultiplier, GameBalance.ArmoredGoldMultiplier);
                break;
            case WaveModifier.Swift:
                enemy.SetSpeedMultiplier(GameBalance.SwiftSpeedMultiplier);
                break;
            case WaveModifier.GoldRush:
                enemy.SetModifierMultipliers(GameBalance.GoldRushHpMultiplier, GameBalance.GoldRushDamageMultiplier, GameBalance.GoldRushGoldMultiplier);
                break;
        }

        if (_currentDifficultyMult > 1f || _currentDifficultyMult < 1f)
            enemy.SetModifierMultipliers(_currentDifficultyMult, 1f, 1f);

        GetEnemiesContainer().CallDeferred(Node.MethodName.AddChild, enemy);
    }
}
