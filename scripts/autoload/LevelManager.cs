using Godot;
using Godot.Collections;

/// <summary>
/// Autoload singleton that manages level loading and progression.
/// Add all <see cref="LevelData"/> assets to the <see cref="Levels"/> array in
/// the Inspector. Call <see cref="LoadLevel(int, Node)"/> to start a specific level.
/// </summary>
public partial class LevelManager : Node
{
    public static LevelManager Instance { get; private set; }

    [Export] public Array<LevelData> Levels { get; set; } = new();
    [Export] public WaveData BossWaveData;

    private int _currentLevelIndex = -1;
    private Node _levelContainer;
    public Node CurrentLevelNode { get; private set; }

    public int CurrentLevelIndex => _currentLevelIndex;
    public Node LevelContainer => _levelContainer;

    public void SetContainer(Node container)
    {
        _levelContainer = container;
    }

    public void LoadRandomLevel(Node container = null)
    {
        if (Levels == null || Levels.Count == 0) return;
        int index = (int)(GD.Randi() % Levels.Count);
        LoadLevel(index, container);
    }
    public LevelData CurrentLevel =>
        Levels != null && _currentLevelIndex >= 0 && _currentLevelIndex < Levels.Count
            ? Levels[_currentLevelIndex]
            : null;
    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        SceneManager.Instance.LevelLoaded += OnLevelLoaded;
    }

    public override void _ExitTree()
    {
        if (SceneManager.Instance != null)
            SceneManager.Instance.LevelLoaded -= OnLevelLoaded;
    }

    /// <summary>Loads the level at <paramref name="index"/> into <paramref name="container"/>.</summary>
    public void LoadLevel(int index, Node container = null)
    {
        if (Levels == null || index < 0 || index >= Levels.Count)
        {
            GD.PrintErr($"LevelManager: Invalid level index {index}. Total levels: {Levels?.Count ?? 0}.");
            return;
        }

        _currentLevelIndex = index;
        if (container != null)
            _levelContainer = container;
        SceneManager.Instance.LoadLevel(Levels[index].ScenePath, _levelContainer);
    }

    private int _pendingLevelIndex = -1;
    public LevelData PendingLevelData { get; private set; }
    public Array<WaveData> PendingRunWaves { get; private set; }

    public LevelData PickRandomLevel()
    {
        if (Levels == null || Levels.Count == 0) return null;
        _pendingLevelIndex = (int)(GD.Randi() % Levels.Count);
        PendingLevelData = Levels[_pendingLevelIndex];

        if (RunState.Instance.IsRunActive && !RunState.Instance.IsBossFight)
            PendingRunWaves = RunState.Instance.PickRunWaves();
        else
            PendingRunWaves = null;

        return PendingLevelData;
    }

    public void LoadPendingLevel()
    {
        if (_pendingLevelIndex < 0) return;
        LoadLevel(_pendingLevelIndex, _levelContainer);
        _pendingLevelIndex = -1;
        PendingLevelData = null;
        PendingRunWaves = null;
    }

    private void OnLevelLoaded(Node levelNode)
    {   
        CurrentLevelNode = levelNode;

        GetTree().Paused = false;

        if (CurrentLevel == null) return;

        if (levelNode is BaseLevel baseLevel)
        {
            baseLevel.ConfigureForRun(PendingRunWaves, BossWaveData);
        }

    if (CurrentLevel != null && levelNode is not BaseLevel)
        GD.PushWarning("LevelManager: Loaded level does not extend BaseLevel.");

    // Pass per‑level world size to the camera, falling back to the default.
    CameraManager.Instance.Configure(CurrentLevel?.WorldSize);
}
}
