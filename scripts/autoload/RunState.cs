using Godot;

public partial class RunState : Node
{
    public static RunState Instance { get; private set; }

    public bool IsRunActive { get; private set; }

    public Godot.Collections.Array<string> SelectedTowerIds { get; private set; } = new();

    public bool[] _selectedTowerFlags; // set by LoadoutScreen before StartRun

    private Godot.Collections.Dictionary<string, int> _towerLevels = new();

    public override void _EnterTree()
    {
        Instance = this;
    }

    public void StartRun(int gold, int lives, Godot.Collections.Array<string> selectedTowerIds)
    {
        _towerLevels.Clear();
        SelectedTowerIds = selectedTowerIds;
        IsRunActive = true;
        EconomyManager.Instance.SetMoney(gold);
        GameManager.Instance.SetLives(lives);
    }

    public void EndRun()
    {
        IsRunActive = false;
        _towerLevels.Clear();
        SelectedTowerIds.Clear();
    }

    public int GetTowerLevel(string towerId)
    {
        return _towerLevels.ContainsKey(towerId) ? _towerLevels[towerId] : 0;
    }

    public void SetTowerLevel(string towerId, int level)
    {
        _towerLevels[towerId] = level;
    }
}
