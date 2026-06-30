using Godot;

public partial class EconomyManager : Node
{
    public static EconomyManager Instance { get; private set; }

    [Export] public int StartingMoney = 1000;

    private int _currentMoney;
    public int CurrentMoney => _currentMoney;

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        _currentMoney = StartingMoney;
        EventBus.Instance.EnemyDied += OnEnemyDied;
    }

    private void OnEnemyDied(int reward)
    {
        AddMoney(reward);
    }

    public bool CanAfford(int cost) => _currentMoney >= cost;

    public bool SpendMoney(int cost)
    {
        if (!CanAfford(cost)) return false;

        _currentMoney -= cost;
        EventBus.Instance.EmitSignal(EventBus.SignalName.MoneyChanged, _currentMoney);
        return true;
    }

    public void AddMoney(int amount)
    {
        _currentMoney += amount;
        EventBus.Instance.EmitSignal(EventBus.SignalName.MoneyChanged, _currentMoney);
    }

    public void SetMoney(int amount)
    {
        _currentMoney = amount;
        EventBus.Instance.EmitSignal(EventBus.SignalName.MoneyChanged, _currentMoney);
    }
	public void ResetForLevel(LevelData _)
	{
		_currentMoney = StartingMoney;
		EventBus.Instance.EmitSignal(EventBus.SignalName.MoneyChanged, _currentMoney);
	}
}