using Godot;

public partial class EventBus : Node
{
    public static EventBus Instance { get; private set; }

    [Signal] public delegate void EnemyDiedEventHandler(int reward);
    [Signal] public delegate void EnemyReachedEndEventHandler(int damage);
    [Signal] public delegate void TowerPlacedEventHandler(int cost);
    [Signal] public delegate void GameOverEventHandler();
    [Signal] public delegate void MoneyChangedEventHandler(int currentMoney);
    [Signal] public delegate void LivesChangedEventHandler(int currentLives);
    [Signal] public delegate void AllWavesCompletedEventHandler();
    [Signal] public delegate void FightCompletedEventHandler();
    [Signal] public delegate void BossFightCompletedEventHandler();
    [Signal] public delegate void RunCompletedEventHandler();

    public override void _EnterTree()
    {
        Instance = this;
    }
}