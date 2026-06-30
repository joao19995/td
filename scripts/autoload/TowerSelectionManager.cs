using Godot;

public partial class TowerSelectionManager : Node
{
    public static TowerSelectionManager Instance { get; private set; }

    [Signal] public delegate void TowerSelectedEventHandler(Tower tower);
    [Signal] public delegate void TowerDeselectedEventHandler();

    private Tower _selectedTower;
    private RangeIndicator _rangeIndicator;
    public Tower SelectedTower => _selectedTower;

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        EventBus.Instance.TowerPlaced += OnTowerPlaced;
        SceneManager.Instance.LevelLoaded += OnLevelLoaded;
    }

    public override void _ExitTree()
    {
        if (EventBus.Instance != null)
            EventBus.Instance.TowerPlaced -= OnTowerPlaced;
        if (SceneManager.Instance != null)
            SceneManager.Instance.LevelLoaded -= OnLevelLoaded;
    }

    private void OnLevelLoaded(Node _)
    {
        _selectedTower = null;
        _rangeIndicator = null;
    }

    public override void _Process(double delta)
    {
        if (_selectedTower != null && Input.IsActionJustPressed("cancel_placement"))
            Deselect();
    }

    private void OnTowerPlaced(int _)
    {
        Deselect();
    }

    public void SelectTower(Tower tower)
    {
        if (tower == _selectedTower) return;

        TowerPlacementManager.Instance.CancelPlacement();

        if (_selectedTower != null && IsInstanceValid(_selectedTower))
            _selectedTower.Modulate = new Color(1, 1, 1, 1);

        _selectedTower = tower;
        _selectedTower.Modulate = new Color(0.8f, 0.9f, 1, 1);

        _rangeIndicator = new RangeIndicator();
        _rangeIndicator.SetRange(tower.EffectiveRange);
        _selectedTower.AddChild(_rangeIndicator);

        EmitSignal(SignalName.TowerSelected, tower);
    }

    public void Deselect()
    {
        if (_selectedTower == null) return;

        if (IsInstanceValid(_selectedTower))
            _selectedTower.Modulate = new Color(1, 1, 1, 1);
        _selectedTower = null;

        if (_rangeIndicator != null)
        {
            _rangeIndicator.QueueFree();
            _rangeIndicator = null;
        }

        EmitSignal(SignalName.TowerDeselected);
    }
}
