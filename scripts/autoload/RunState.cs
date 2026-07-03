using Godot;
using Godot.Collections;

public partial class RunState : Node
{
    public static RunState Instance { get; private set; }

    public bool IsRunActive { get; private set; }
    public System.Collections.Generic.HashSet<string> AppliedTrinketIds { get; } = new();
    public System.Collections.Generic.HashSet<string> PurchasedShopItemIds { get; } = new();
    public Godot.Collections.Array<Texture2D> PurchasedItemIcons { get; } = new();
    public Godot.Collections.Array<string> PurchasedItemNames { get; } = new();
    public Godot.Collections.Array<string> PurchasedItemDescriptions { get; } = new();

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
    public int MetaStartingLivesBonus { get; private set; } = 0;
    public float MetaShopDiscountPercent { get; private set; } = 0f;
    public float MetaRerollCostReductionPercent { get; private set; } = 0f;
    public float MetaEnemyGoldBonusPercent { get; private set; } = 0f;
    public bool MetaStartWithEquipment { get; private set; } = false;
    public bool MetaStartTowerLevel { get; private set; } = false;

    public float TrinketDamageBonusPercent { get; set; } = 0f;
    public float TrinketFireRateBonusPercent { get; set; } = 0f;
    public float TrinketRangeBonusPercent { get; set; } = 0f;
    public float TrinketCritDamageBonusPercent { get; set; } = 0f;
    public float TrinketStatusDurationBonusPercent { get; set; } = 0f;
    public float TrinketStatusStrengthBonusPercent { get; set; } = 0f;
    public float GlobalAuraDamagePercent { get; set; } = 0f;
    public float TrinketRangeFlatBonus { get; set; } = 0f;
    public float TrinketBasicDamagePercentBonus { get; set; } = 0f;
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
        AppliedTrinketIds.Clear();
        PurchasedShopItemIds.Clear();
        PurchasedItemIcons.Clear();
        PurchasedItemNames.Clear();
        PurchasedItemDescriptions.Clear();
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
        TrinketRangeFlatBonus = 0f;
        TrinketBasicDamagePercentBonus = 0f;
        _passiveGoldTimer = 0f;

        int damageLevel = SaveManager.Instance.GetMetaUpgradeLevel("secret_recipe");
        MetaDamageBonusPercent = damageLevel * GameBalance.MetaDamagePercentPerLevel;
        int goldLevel = SaveManager.Instance.GetMetaUpgradeLevel("local_sponsorship");
        StartingGoldBonus = goldLevel * GameBalance.StartingGoldPerLevel;
        int livesLevel = SaveManager.Instance.GetMetaUpgradeLevel("starting_lives");
        MetaStartingLivesBonus = livesLevel * GameBalance.StartingLivesPerLevel;
        int shopLevel = SaveManager.Instance.GetMetaUpgradeLevel("shop_discount");
        MetaShopDiscountPercent = shopLevel * GameBalance.MetaShopDiscountPerLevel;
        int rerollLevel = SaveManager.Instance.GetMetaUpgradeLevel("reroll_cost_reduction");
        MetaRerollCostReductionPercent = rerollLevel * GameBalance.MetaRerollCostReductionPerLevel;
        int goldBonusLevel = SaveManager.Instance.GetMetaUpgradeLevel("enemy_gold_bonus");
        MetaEnemyGoldBonusPercent = goldBonusLevel * GameBalance.MetaEnemyGoldBonusPerLevel;
        MetaStartWithEquipment = SaveManager.Instance.GetMetaUpgradeLevel("start_with_equipment") > 0;
        MetaStartTowerLevel = SaveManager.Instance.GetMetaUpgradeLevel("start_tower_level") > 0;

        if (MetaStartTowerLevel)
        {
            foreach (var id in selectedTowerIds)
            {
                if (GetTowerLevel(id) == 0)
                    SetTowerLevel(id, 1);
            }
        }

        if (MetaStartWithEquipment)
        {
            System.Collections.Generic.List<EquipData> allEquips = new();
            var dir = DirAccess.Open("res://resources/equip_data/");
            if (dir != null)
            {
                foreach (var file in dir.GetFiles())
                {
                    if (!file.EndsWith(".tres") && !file.EndsWith(".res")) continue;
                    var eq = ResourceLoader.Load<EquipData>("res://resources/equip_data/" + file, "", ResourceLoader.CacheMode.Replace);
                    if (eq != null) allEquips.Add(eq);
                }
            }

            var loadoutEquips = allEquips.FindAll(e => selectedTowerIds.Contains(e.TargetTowerId));
            if (loadoutEquips.Count > 0)
            {
                var chosen = loadoutEquips[GD.RandRange(0, loadoutEquips.Count - 1)];
                SetEquippedItem(chosen.TargetTowerId, chosen.Id);
            }
        }

        EconomyManager.Instance.SetMoney(gold + StartingGoldBonus);
        GameManager.Instance.SetLives(lives + MetaStartingLivesBonus);
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

    public void EndRun(bool isVictory = false)
    {
        if (!IsRunActive) return;
        IsRunActive = false;

        int baseTokens = SaveManager.Instance.MetaTokensPerRun;
        float ratio = SlotManager.Instance != null
            ? (float)FightsCompleted / Mathf.Max(1, SlotManager.Instance.FightsPerRun)
            : 1f;
        int totalTokens = Mathf.RoundToInt(baseTokens * (GameBalance.TokenRewardBaseMultiplier + ratio));
        if (isVictory)
            totalTokens = Mathf.RoundToInt(totalTokens * GameBalance.TokenRewardVictoryMultiplier);

        SaveManager.Instance.AddMetaTokens(totalTokens);
        _towerLevels.Clear();
        _equippedItems.Clear();
        _ancientStarterAttackCount.Clear();
        _ancientStarterStacks.Clear();
        SelectedTowerIds.Clear();
        FightsCompleted = 0;
        AppliedTrinketIds.Clear();
        PurchasedShopItemIds.Clear();
        PurchasedItemIcons.Clear();
        PurchasedItemNames.Clear();
        PurchasedItemDescriptions.Clear();
        IsBossFight = false;
        IsMiniboss = false;
        MetaDamageBonusPercent = 0f;
        StartingGoldBonus = 0;
        MetaStartingLivesBonus = 0;
        MetaShopDiscountPercent = 0f;
        MetaRerollCostReductionPercent = 0f;
        MetaEnemyGoldBonusPercent = 0f;
        MetaStartWithEquipment = false;
        MetaStartTowerLevel = false;
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
        AppliedTrinketIds.Add(trinket.Id);
        TrinketDamageBonusPercent += trinket.DamagePercentBonus;
        TrinketFireRateBonusPercent += trinket.FireRateBonusPercent;
        TrinketRangeBonusPercent += trinket.RangePercentBonus;
        TrinketCritDamageBonusPercent += trinket.CritDamageBonusPercent;
        TrinketStatusDurationBonusPercent += trinket.StatusDurationBonusPercent;
        TrinketStatusStrengthBonusPercent += trinket.StatusStrengthBonusPercent;
        TrinketRangeFlatBonus += trinket.RangeFlatBonus;
        TrinketBasicDamagePercentBonus += trinket.BasicDamagePercentBonus;
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
                EconomyManager.Instance.AddMoney(GameBalance.PassiveGoldAmount);
                _passiveGoldTimer = GameBalance.PassiveGoldInterval;
            }
        }
    }

    public string GetWaveTier()
    {
        float ratio = SlotManager.Instance != null
            ? (float)FightsCompleted / SlotManager.Instance.FightsPerRun
            : 0f;
        if (ratio >= GameBalance.Tier3Threshold) return "tier3";
        if (ratio >= GameBalance.Tier2Threshold) return "tier2";
        return "tier1";
    }

    public int GetEffectiveShopCost(int baseCost)
    {
        if (MetaShopDiscountPercent > 0f)
            return Mathf.Max(1, Mathf.RoundToInt(baseCost * (1f - MetaShopDiscountPercent)));
        return baseCost;
    }

    private static readonly WaveModifier[] AllModifiers = new[] {
        WaveModifier.None, WaveModifier.Horde, WaveModifier.Armored,
        WaveModifier.Swift, WaveModifier.GoldRush
    };
    private static readonly WaveModifier[] HardModifiers = new[] {
        WaveModifier.Horde, WaveModifier.Armored
    };

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

        var pool = new System.Collections.Generic.List<WaveData>();
        foreach (var file in dir.GetFiles())
        {
            if (!file.EndsWith(".tres") && !file.EndsWith(".res"))
                continue;
            var res = ResourceLoader.Load<Resource>(dirPath + file, "", ResourceLoader.CacheMode.Replace);
            if (res is WaveData w)
                pool.Add(w);
        }

        if (pool.Count == 0) return null;

        int totalWaves = (int)(GD.Randi() % (GameBalance.MaxWaves - GameBalance.MinWaves + 1)) + GameBalance.MinWaves;
        var result = new Array<WaveData>();

        WaveModifier? prevModifier = null;

        for (int i = 0; i < totalWaves; i++)
        {
            var wave = pool[(int)(GD.Randi() % pool.Count)];
            var clone = new WaveData
            {
                Entries = wave.Entries,
                SpawnInterval = wave.SpawnInterval,
                Modifier = wave.Modifier,
            };

            clone.IsFinalStretch = i >= totalWaves - GameBalance.FinalStretchWaveOffset;
            clone.DifficultyMultiplier = GameBalance.DifficultyCurveMin + GameBalance.DifficultyCurveRange * (float)i / Mathf.Max(1, totalWaves - 1);

            if (i >= GameBalance.ModifierStartWave)
            {
                var mods = i >= totalWaves - GameBalance.HardModifierWaveOffset ? HardModifiers : AllModifiers;
                int attempts = 0;
                do {
                    clone.Modifier = mods[(int)(GD.Randi() % mods.Length)];
                    attempts++;
                } while (prevModifier.HasValue && clone.Modifier == prevModifier.Value && attempts < 10);
            }
            prevModifier = clone.Modifier;

            result.Add(clone);
        }

        return result;
    }
}
