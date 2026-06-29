using Godot;

/// <summary>
/// Autoload singleton that manages scene/level transitions.
/// Call LoadLevel() instead of instantiating PackedScenes manually.
/// When loading screens or transition animations are needed, add them here.
/// </summary>
public partial class SceneManager : Node
{
    public static SceneManager Instance { get; private set; }

    [Signal] public delegate void LevelLoadedEventHandler(Node levelNode);

    private Node _currentLevel;

    private bool _isLoading;

    public override void _EnterTree()
    {
        Instance = this;
    }

    /// <summary>
    /// Loads a level scene from <paramref name="scenePath"/>, adds it as a child of
    /// <paramref name="container"/> (defaults to the scene tree root when null), and emits
    /// <see cref="LevelLoaded"/> with the instantiated level node.
    /// Any previously loaded level is freed first.
    /// Ignores the call if a load is already in progress.
    /// </summary>
    public void LoadLevel(string scenePath, Node container = null)
    {
        if (_isLoading)
        {
            GD.PushWarning("SceneManager: LoadLevel called while a load is already in progress.");
            return;
        }

        _isLoading = true;

        if (_currentLevel != null)
            _currentLevel.QueueFree();
        _currentLevel = null;

        var packedScene = GD.Load<PackedScene>(scenePath);
        if (packedScene == null)
        {
            GD.PrintErr($"SceneManager: Failed to load scene at '{scenePath}'.");
            _isLoading = false;
            return;
        }

        _currentLevel = packedScene.Instantiate();
        var parent = container ?? GetTree().Root;
        parent.AddChild(_currentLevel);

        _isLoading = false;
        EmitSignal(SignalName.LevelLoaded, _currentLevel);
    }
}
