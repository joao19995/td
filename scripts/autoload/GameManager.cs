using Godot;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    [Export] public int StartingLives = 20;

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

    private void TriggerGameOver()
    {
        EventBus.Instance.EmitSignal(EventBus.SignalName.GameOver);
        GetTree().Paused = true; // pausa o jogo; Fase 10 vai mostrar o menu de game over
    }

    public void SetLives(int amount)
    {
        _currentLives = amount;
        EventBus.Instance.EmitSignal(EventBus.SignalName.LivesChanged, _currentLives);
    }

    public void ResetForLevel(LevelData data)
	{
		_currentLives = (data.StartingLives >= 0) ? data.StartingLives : StartingLives;
		EventBus.Instance.EmitSignal(EventBus.SignalName.LivesChanged, _currentLives);
	}
}