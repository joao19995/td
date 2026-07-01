using Godot;
using Godot.Collections;

public partial class RunState : Node
{
    public static RunState Instance { get; private set; }

    public bool IsRunActive { get; private set; }

    public Array<string> SelectedTowerIds { get; private set; } = new();

    public int FightsCompleted { get; private set; } = 0;
    public bool IsBossFight { get; private set; } = false;
    public bool IsMiniboss { get; private set; } = false;

    public float ShopDamageBonusPercent { get; set; } = 0f;
    public float ShopFireRateBonusPercent { get; set; } = 0f;
    public float ShopRangeBonusPercent { get; set; } = 0f;
    public float ShopHeavyDamageBonusPercent { get; set; } = 0f;
    public float FirstPurchaseDiscountPercent { get; set; } = 0f;

    public float MetaDamageBonusPercent { get; private set; } = 0f;
    public int StartingGoldBonus { get; private set; } = 0;

    public float TrinketDamageBonusPercent { get; set; } = 0f;
    public float TrinketFireRateBonusPercent { get; set; } = 0f;
    public float TrinketRangeBonusPercent { get; set; } = 0f;
    public float TrinketCritDamageBonusPercent { get; set; } = 0f;
    public float TrinketStatusDurationBonusPercent { get; set; } = 0f;
    public float TrinketStatusStrengthBonusPercent { get; set; } = 0f;
    public float GlobalAuraDamagePercent { get; set; } = 0f;
    private float _passiveGoldTimer;

    private Dictionary<string, int> _towerLevels = new();
    private Dictionary<string, string> _equippedItems = new();
    private Dictionary<string, int> _ancientStarterAttackCount = new();
    private Dictionary<string, int> _ancientStarterStacks = new();

    public override void _EnterTree()
    {
        Instance = this;
    }

    public void StartRun(int gold, int lives, Godot.Collections.Array<string> selectedTowerIds)
    {
        _towerLevels.Clear();
        _equippedItems.Clear();
        _ancientStarterAttackCount.Clear();
        _ancientStarterStacks.Clear();
        SelectedTowerIds = selectedTowerIds;
        IsRunActive = true;
        FightsCompleted = 0;
        IsBossFight = false;
        IsMiniboss = false;
        ShopDamageBonusPercent = 0f;
        ShopFireRateBonusPercent = 0f;
        ShopRangeBonusPercent = 0f;
        ShopHeavyDamageBonusPercent = 0f;
        FirstPurchaseDiscountPercent = 0f;
        TrinketDamageBonusPercent = 0f;
        TrinketFireRateBonusPercent = 0f;
        TrinketRangeBonusPercent = 0f;
        TrinketCritDamageBonusPercent = 0f;
        TrinketStatusDurationBonusPercent = 0f;
        TrinketStatusStrengthBonusPercent = 0f;
        GlobalAuraDamagePercent = 0f;
        _passiveGoldTimer = 0f;

        int damageLevel = SaveManager.Instance.GetMetaUpgradeLevel("secret_recipe");
        MetaDamageBonusPercent = damageLevel * 0.05f;
        int goldLevel = SaveManager.Instance.GetMetaUpgradeLevel("local_sponsorship");
        StartingGoldBonus = goldLevel * 50;

        EconomyManager.Instance.SetMoney(gold + StartingGoldBonus);
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
        _equippedItems.Clear();
        _ancientStarterAttackCount.Clear();
        _ancientStarterStacks.Clear();
        SelectedTowerIds.Clear();
        FightsCompleted = 0;
        IsBossFight = false;
        IsMiniboss = false;
        MetaDamageBonusPercent = 0f;
        StartingGoldBonus = 0;
        TrinketDamageBonusPercent = 0f;
    }

    public int GetTowerLevel(string towerId)
    {
        return _towerLevels.ContainsKey(towerId) ? _towerLevels[towerId] : 0;
    }

    public void SetTowerLevel(string towerId, int level)
    {
        _towerLevels[towerId] = level;
    }

    public string GetEquippedItem(string towerId)
    {
        return _equippedItems.ContainsKey(towerId) ? _equippedItems[towerId] : null;
    }

    public void SetEquippedItem(string towerId, string equipId)
    {
        _equippedItems[towerId] = equipId;
    }

    public int GetAncientStarterStacks(string towerId)
    {
        return _ancientStarterStacks.ContainsKey(towerId) ? _ancientStarterStacks[towerId] : 0;
    }

    public void SetAncientStarterStacks(string towerId, int stacks)
    {
        _ancientStarterStacks[towerId] = stacks;
    }

    public int GetAncientStarterAttackCounter(string towerId)
    {
        return _ancientStarterAttackCount.ContainsKey(towerId) ? _ancientStarterAttackCount[towerId] : 0;
    }

    public void SetAncientStarterAttackCounter(string towerId, int count)
    {
        _ancientStarterAttackCount[towerId] = count;
    }

    public void ApplyTrinket(TrinketData trinket)
    {
        TrinketDamageBonusPercent += trinket.DamagePercentBonus;
        TrinketFireRateBonusPercent += trinket.FireRatePercentBonus;
        TrinketRangeBonusPercent += trinket.RangePercentBonus;
        TrinketCritDamageBonusPercent += trinket.CritDamageBonusPercent;
        TrinketStatusDurationBonusPercent += trinket.StatusDurationBonusPercent;
        TrinketStatusStrengthBonusPercent += trinket.StatusStrengthBonusPercent;
        if (trinket.HealAmount > 0)
            GameManager.Instance.Heal(trinket.HealAmount);
        if (trinket.GoldAmount > 0)
            EconomyManager.Instance.AddMoney(trinket.GoldAmount);
        if (trinket.PassiveGoldPerInterval > 0 && trinket.PassiveGoldInterval > 0f)
            _passiveGoldTimer = trinket.PassiveGoldInterval;
    }

    public override void _Process(double delta)
    {
        if (!IsRunActive) return;
        if (_passiveGoldTimer > 0f)
        {
            _passiveGoldTimer -= (float)delta;
            if (_passiveGoldTimer <= 0f)
            {
                EconomyManager.Instance.AddMoney(5);
                _passiveGoldTimer = 30f;
            }
        }
    }

    public string GetWaveTier()
    {
        float ratio = SlotManager.Instance != null
            ? (float)FightsCompleted / SlotManager.Instance.FightsPerRun
            : 0f;
        if (ratio >= 0.66f) return "tier3";
        if (ratio >= 0.33f) return "tier2";
        return "tier1";
    }

    public Array<WaveData> PickRunWaves()
    {
        string tier = GetWaveTier();
        string dirPath = $"res://resources/wave_data/{tier}/";
        var dir = DirAccess.Open(dirPath);
        if (dir == null)
        {
            GD.PrintErr($"RunState: failed to open wave tier dir {dirPath}");
            return null;
        }

        var waves = new System.Collections.Generic.List<WaveData>();
        foreach (var file in dir.GetFiles())
        {
            if (!file.EndsWith(".tres") && !file.EndsWith(".res"))
                continue;
            var res = ResourceLoader.Load<Resource>(dirPath + file, "", ResourceLoader.CacheMode.Replace);
            if (res is WaveData w)
                waves.Add(w);
        }

        if (waves.Count == 0) return null;

        int idx = (int)(GD.Randi() % waves.Count);
        return new Array<WaveData> { waves[idx] };
    }
}
