using Godot;
using Godot.Collections;

/// <summary>
/// Autoload singleton that manages level order, loading, and progression.
/// Add all <see cref="LevelData"/> assets to the <see cref="Levels"/> array in
/// the Inspector. Call <see cref="LoadLevel(int, Node)"/> to start a specific
/// level, or <see cref="LoadNextLevel"/> to advance to the next one.
/// </summary>
public partial class LevelManager : Node
{
    public static LevelManager Instance { get; private set; }

    [Export] public Array<LevelData> Levels { get; set; } = new();

    private int _currentLevelIndex = -1;
    private Node _levelContainer;
    public Node CurrentLevelNode { get; private set; }

    public int CurrentLevelIndex => _currentLevelIndex;
    public Node LevelContainer => _levelContainer;

    public void SetContainer(Node container)
    {
        _levelContainer = container;
    }
    public LevelData CurrentLevel =>
        Levels != null && _currentLevelIndex >= 0 && _currentLevelIndex < Levels.Count
            ? Levels[_currentLevelIndex]
            : null;
    public bool HasNextLevel => Levels != null && _currentLevelIndex + 1 < Levels.Count;

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
        GD.Print($"[LevelManager] LoadLevel — idx={index}, scene={Levels[index].ScenePath}, container={( _levelContainer?.Name ?? "null")}");
        SceneManager.Instance.LoadLevel(Levels[index].ScenePath, _levelContainer);
    }

    /// <summary>Loads the level that follows the current one.</summary>
    public void LoadNextLevel()
    {
        if (!HasNextLevel)
        {
            GD.PushWarning("LevelManager: LoadNextLevel called but there is no next level.");
            return;
        }

        LoadLevel(_currentLevelIndex + 1, _levelContainer);
    }

    private void OnLevelLoaded(Node levelNode)
    {   
        CurrentLevelNode = levelNode;

        GetTree().Paused = false;

        if (CurrentLevel == null) return;

        GameManager.Instance.ResetForLevel(CurrentLevel);
        EconomyManager.Instance.ResetForLevel(CurrentLevel);

  
    if (levelNode is not BaseLevel)
        GD.PushWarning("LevelManager: Loaded level does not extend BaseLevel.");

    // Pass per‑level world size to the camera, falling back to the default.
    CameraManager.Instance.Configure(CurrentLevel?.WorldSize);
}
}
