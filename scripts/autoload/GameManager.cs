using Godot;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    [Export] public int StartingLives = 20;

    [Export] private bool _debugLogging = false;

    private int _currentLives;
    public int CurrentLives => _currentLives;

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        _currentLives = StartingLives;
        EventBus.Instance.EnemyReachedEnd += OnEnemyReachedEnd;
    }

    private void OnEnemyReachedEnd(int damage)
    {
        _currentLives = Mathf.Max(0, _currentLives - damage);
        EventBus.Instance.EmitSignal(EventBus.SignalName.LivesChanged, _currentLives);

        if (_currentLives <= 0)
        {
            TriggerGameOver();
        }
    }

    /// <summary>Conditional debug log — only prints when DebugLogging is enabled.</summary>
    public static void Log(string message)
    {
        if (Instance != null && Instance._debugLogging)
            GD.Print(message);
    }

    private void TriggerGameOver()
    {
        EventBus.Instance.EmitSignal(EventBus.SignalName.GameOver);
        GetTree().Paused = true; // pausa o jogo; Fase 10 vai mostrar o menu de game over
    }

	public void ResetForLevel(LevelData data)
	{
		_currentLives = (data.StartingLives >= 0) ? data.StartingLives : StartingLives;
		EventBus.Instance.EmitSignal(EventBus.SignalName.LivesChanged, _currentLives);
	}
}