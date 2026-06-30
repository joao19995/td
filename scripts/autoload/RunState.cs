using Godot;

public partial class RunState : Node
{
    public static RunState Instance { get; private set; }

    public bool IsRunActive { get; private set; }

    public Godot.Collections.Array<string> SelectedTowerIds { get; private set; } = new();

    public bool[] _selectedTowerFlags; // set by LoadoutScreen before StartRun

    public int FightsCompleted { get; private set; } = 0;
    public bool IsBossFight { get; private set; } = false;
    public bool IsMiniboss { get; private set; } = false;

    public float ShopDamageBonusPercent { get; set; } = 0f;
    public float ShopFireRateBonusPercent { get; set; } = 0f;
    public float ShopRangeBonusPercent { get; set; } = 0f;

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
        FightsCompleted = 0;
        IsBossFight = false;
        IsMiniboss = false;
        ShopDamageBonusPercent = 0f;
        ShopFireRateBonusPercent = 0f;
        ShopRangeBonusPercent = 0f;
        EconomyManager.Instance.SetMoney(gold);
        GameManager.Instance.SetLives(lives);
    }

    public void IncrementFights()
    {
        FightsCompleted++;
    }

    public void SetBossFight(bool value)
    {
        IsBossFight = value;
    }

    public void SetMiniboss(bool value)
    {
        IsMiniboss = value;
    }

    public void EndRun()
    {
        if (!IsRunActive) return;
        IsRunActive = false;
        SaveManager.Instance.AddMetaTokens(SaveManager.Instance.MetaTokensPerRun);
        _towerLevels.Clear();
        SelectedTowerIds.Clear();
        FightsCompleted = 0;
        IsBossFight = false;
        IsMiniboss = false;
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
