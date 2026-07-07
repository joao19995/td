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
    public int TotalEnemiesKilled { get; private set; } = 0;
    public int TotalGoldEarned { get; private set; } = 0;
    public int TotalGoldSpent { get; private set; } = 0;
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
    private readonly System.Collections.Generic.List<PassiveGoldEffect> _passiveGoldEffects = new();

    private Dictionary<string, int> _towerLevels = new();
    private Dictionary<string, string> _equippedItems = new();
    private Dictionary<string, int> _ancientStarterAttackCount = new();
    private Dictionary<string, int> _ancientStarterStacks = new();

    private sealed class PassiveGoldEffect
    {
        public int Amount;
        public float Interval;
        public float RemainingTime;
    }

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        EventBus.Instance.EnemyDied += OnEnemyDied;
        EventBus.Instance.MoneyChanged += OnMoneyChanged;
    }

    public override void _ExitTree()
    {
        if (EventBus.Instance != null)
        {
            EventBus.Instance.EnemyDied -= OnEnemyDied;
            EventBus.Instance.MoneyChanged -= OnMoneyChanged;
        }
    }

    private void OnEnemyDied(int _)
    {
        TotalEnemiesKilled++;
    }

    private int _lastMoney;
    private void OnMoneyChanged(int currentMoney)
    {
        int diff = currentMoney - _lastMoney;
        if (diff > 0)
            TotalGoldEarned += diff;
        else if (diff < 0)
            TotalGoldSpent -= diff;
        _lastMoney = currentMoney;
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
        TotalEnemiesKilled = 0;
        TotalGoldEarned = 0;
        TotalGoldSpent = 0;
        LastTokenReward = 0;
        LastFightsCompleted = 0;
        LastEnemiesKilled = 0;
        LastGoldEarned = 0;
        LastGoldSpent = 0;
        _lastMoney = gold;
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
        _passiveGoldEffects.Clear();

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
        SaveManager.Instance.DeleteRunState();
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

        LastTokenReward = totalTokens;
        LastFightsCompleted = FightsCompleted;
        LastEnemiesKilled = TotalEnemiesKilled;
        LastGoldEarned = TotalGoldEarned;
        LastGoldSpent = TotalGoldSpent;
        SaveManager.Instance.AddMetaTokens(totalTokens);
        SaveManager.Instance.DeleteRunState();
        _towerLevels.Clear();
        _equippedItems.Clear();
        _ancientStarterAttackCount.Clear();
        _ancientStarterStacks.Clear();
        SelectedTowerIds.Clear();
        FightsCompleted = 0;
        TotalEnemiesKilled = 0;
        TotalGoldEarned = 0;
        TotalGoldSpent = 0;
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
        TrinketFireRateBonusPercent = 0f;
        TrinketRangeBonusPercent = 0f;
        TrinketCritDamageBonusPercent = 0f;
        TrinketStatusDurationBonusPercent = 0f;
        TrinketStatusStrengthBonusPercent = 0f;
        TrinketRangeFlatBonus = 0f;
        TrinketBasicDamagePercentBonus = 0f;
        ShopDamageBonusPercent = 0f;
        ShopFireRateBonusPercent = 0f;
        ShopRangeBonusPercent = 0f;
        ShopHeavyDamageBonusPercent = 0f;
        FirstPurchaseDiscountPercent = 0f;
        GlobalAuraDamagePercent = 0f;
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
        {
            _passiveGoldEffects.Add(new PassiveGoldEffect
            {
                Amount = trinket.PassiveGoldPerInterval,
                Interval = trinket.PassiveGoldInterval,
                RemainingTime = trinket.PassiveGoldInterval
            });
        }
    }

    public void SaveCurrentRun()
    {
        var data = new Godot.Collections.Dictionary
        {
            { "gold", EconomyManager.Instance.CurrentMoney },
            { "lives", GameManager.Instance.CurrentLives },
            { "fights_completed", FightsCompleted },
            { "is_boss_fight", IsBossFight },
            { "is_miniboss", IsMiniboss },
            { "total_enemies_killed", TotalEnemiesKilled },
            { "total_gold_earned", TotalGoldEarned },
            { "total_gold_spent", TotalGoldSpent },
            { "global_aura_damage_percent", GlobalAuraDamagePercent },
            { "trinket_range_flat_bonus", TrinketRangeFlatBonus },
            { "trinket_basic_damage_percent_bonus", TrinketBasicDamagePercentBonus },
            { "shop_damage_bonus_percent", ShopDamageBonusPercent },
            { "shop_fire_rate_bonus_percent", ShopFireRateBonusPercent },
            { "shop_range_bonus_percent", ShopRangeBonusPercent },
            { "shop_heavy_damage_bonus_percent", ShopHeavyDamageBonusPercent },
            { "first_purchase_discount_percent", FirstPurchaseDiscountPercent },
            { "trinket_damage_bonus_percent", TrinketDamageBonusPercent },
            { "trinket_fire_rate_bonus_percent", TrinketFireRateBonusPercent },
            { "trinket_range_bonus_percent", TrinketRangeBonusPercent },
            { "trinket_crit_damage_bonus_percent", TrinketCritDamageBonusPercent },
            { "trinket_status_duration_bonus_percent", TrinketStatusDurationBonusPercent },
            { "trinket_status_strength_bonus_percent", TrinketStatusStrengthBonusPercent }
        };

        var selectedIds = new Godot.Collections.Array();
        foreach (var id in SelectedTowerIds)
            selectedIds.Add(id);
        data["selected_tower_ids"] = selectedIds;

        var towerLevelsDict = new Godot.Collections.Dictionary();
        foreach (var kv in _towerLevels)
            towerLevelsDict[kv.Key] = kv.Value;
        data["tower_levels"] = towerLevelsDict;

        var equippedItemsDict = new Godot.Collections.Dictionary();
        foreach (var kv in _equippedItems)
            equippedItemsDict[kv.Key] = kv.Value;
        data["equipped_items"] = equippedItemsDict;

        var ancientAttackDict = new Godot.Collections.Dictionary();
        foreach (var kv in _ancientStarterAttackCount)
            ancientAttackDict[kv.Key] = kv.Value;
        data["ancient_attack_count"] = ancientAttackDict;

        var ancientStacksDict = new Godot.Collections.Dictionary();
        foreach (var kv in _ancientStarterStacks)
            ancientStacksDict[kv.Key] = kv.Value;
        data["ancient_stacks"] = ancientStacksDict;

        var trinketIds = new Godot.Collections.Array();
        foreach (var id in AppliedTrinketIds)
            trinketIds.Add(id);
        data["applied_trinket_ids"] = trinketIds;

        var shopItemIds = new Godot.Collections.Array();
        foreach (var id in PurchasedShopItemIds)
            shopItemIds.Add(id);
        data["purchased_shop_item_ids"] = shopItemIds;

        var itemNames = new Godot.Collections.Array();
        foreach (var name in PurchasedItemNames)
            itemNames.Add(name);
        data["purchased_item_names"] = itemNames;

        var itemDescs = new Godot.Collections.Array();
        foreach (var desc in PurchasedItemDescriptions)
            itemDescs.Add(desc);
        data["purchased_item_descriptions"] = itemDescs;

        var passiveGoldArray = new Godot.Collections.Array();
        foreach (var effect in _passiveGoldEffects)
        {
            var effectDict = new Godot.Collections.Dictionary
            {
                { "amount", effect.Amount },
                { "interval", effect.Interval },
                { "remaining_time", effect.RemainingTime }
            };
            passiveGoldArray.Add(effectDict);
        }
        data["passive_gold_effects"] = passiveGoldArray;

        SaveManager.Instance.SaveRunState(data);
    }

    public bool TryResumeRun(Godot.Collections.Dictionary data)
    {
        if (data == null) return false;

        IsRunActive = true;
        EconomyManager.Instance.SetMoney((int)(long)data["gold"]);
        GameManager.Instance.SetLives((int)(long)data["lives"]);
        FightsCompleted = (int)(long)data["fights_completed"];
        IsBossFight = data.ContainsKey("is_boss_fight") && (bool)data["is_boss_fight"];
        IsMiniboss = data.ContainsKey("is_miniboss") && (bool)data["is_miniboss"];
        TotalEnemiesKilled = (int)(long)data["total_enemies_killed"];
        TotalGoldEarned = (int)(long)data["total_gold_earned"];
        TotalGoldSpent = (int)(long)data["total_gold_spent"];
        GlobalAuraDamagePercent = (float)(double)data["global_aura_damage_percent"];
        TrinketRangeFlatBonus = (float)(double)data["trinket_range_flat_bonus"];
        TrinketBasicDamagePercentBonus = (float)(double)data["trinket_basic_damage_percent_bonus"];
        ShopDamageBonusPercent = (float)(double)data["shop_damage_bonus_percent"];
        ShopFireRateBonusPercent = (float)(double)data["shop_fire_rate_bonus_percent"];
        ShopRangeBonusPercent = (float)(double)data["shop_range_bonus_percent"];
        ShopHeavyDamageBonusPercent = (float)(double)data["shop_heavy_damage_bonus_percent"];
        FirstPurchaseDiscountPercent = (float)(double)data["first_purchase_discount_percent"];
        TrinketDamageBonusPercent = (float)(double)data["trinket_damage_bonus_percent"];
        TrinketFireRateBonusPercent = (float)(double)data["trinket_fire_rate_bonus_percent"];
        TrinketRangeBonusPercent = (float)(double)data["trinket_range_bonus_percent"];
        TrinketCritDamageBonusPercent = (float)(double)data["trinket_crit_damage_bonus_percent"];
        TrinketStatusDurationBonusPercent = (float)(double)data["trinket_status_duration_bonus_percent"];
        TrinketStatusStrengthBonusPercent = (float)(double)data["trinket_status_strength_bonus_percent"];

        _lastMoney = EconomyManager.Instance.CurrentMoney;

        if (data.ContainsKey("selected_tower_ids"))
        {
            var ids = data["selected_tower_ids"].AsGodotArray();
            SelectedTowerIds = new Godot.Collections.Array<string>();
            foreach (var id in ids)
                SelectedTowerIds.Add(id.AsString());
        }

        if (data.ContainsKey("tower_levels"))
        {
            _towerLevels.Clear();
            var levels = data["tower_levels"].AsGodotDictionary();
            foreach (var key in levels.Keys)
                _towerLevels[key.AsString()] = (int)(long)levels[key];
        }

        if (data.ContainsKey("equipped_items"))
        {
            _equippedItems.Clear();
            var items = data["equipped_items"].AsGodotDictionary();
            foreach (var key in items.Keys)
                _equippedItems[key.AsString()] = items[key].AsString();
        }

        if (data.ContainsKey("ancient_attack_count"))
        {
            _ancientStarterAttackCount.Clear();
            var att = data["ancient_attack_count"].AsGodotDictionary();
            foreach (var key in att.Keys)
                _ancientStarterAttackCount[key.AsString()] = (int)(long)att[key];
        }

        if (data.ContainsKey("ancient_stacks"))
        {
            _ancientStarterStacks.Clear();
            var stk = data["ancient_stacks"].AsGodotDictionary();
            foreach (var key in stk.Keys)
                _ancientStarterStacks[key.AsString()] = (int)(long)stk[key];
        }

        if (data.ContainsKey("applied_trinket_ids"))
        {
            AppliedTrinketIds.Clear();
            var trinkets = data["applied_trinket_ids"].AsGodotArray();
            foreach (var id in trinkets)
                AppliedTrinketIds.Add(id.AsString());
        }

        if (data.ContainsKey("purchased_shop_item_ids"))
        {
            PurchasedShopItemIds.Clear();
            var shopIds = data["purchased_shop_item_ids"].AsGodotArray();
            foreach (var id in shopIds)
                PurchasedShopItemIds.Add(id.AsString());
        }

        PurchasedItemIcons.Clear();
        PurchasedItemNames.Clear();
        PurchasedItemDescriptions.Clear();

        if (data.ContainsKey("purchased_item_names"))
        {
            var names = data["purchased_item_names"].AsGodotArray();
            foreach (var n in names)
                PurchasedItemNames.Add(n.AsString());
        }

        if (data.ContainsKey("purchased_item_descriptions"))
        {
            var descs = data["purchased_item_descriptions"].AsGodotArray();
            foreach (var d in descs)
                PurchasedItemDescriptions.Add(d.AsString());
        }

        if (data.ContainsKey("passive_gold_effects"))
        {
            _passiveGoldEffects.Clear();
            var effects = data["passive_gold_effects"].AsGodotArray();
            foreach (var e in effects)
            {
                var ed = e.AsGodotDictionary();
                _passiveGoldEffects.Add(new PassiveGoldEffect
                {
                    Amount = (int)(long)ed["amount"],
                    Interval = (float)(double)ed["interval"],
                    RemainingTime = (float)(double)ed["remaining_time"]
                });
            }
        }

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

        return true;
    }

    public override void _Process(double delta)
    {
        if (!IsRunActive) return;

        foreach (var effect in _passiveGoldEffects)
        {
            effect.RemainingTime -= (float)delta;
            while (effect.RemainingTime <= 0f)
            {
                EconomyManager.Instance.AddMoney(effect.Amount);
                effect.RemainingTime += effect.Interval;
            }
        }
    }

    public int LastTokenReward { get; private set; }
    public int LastFightsCompleted { get; private set; }
    public int LastEnemiesKilled { get; private set; }
    public int LastGoldEarned { get; private set; }
    public int LastGoldSpent { get; private set; }

    public int PreviewTokenReward(bool isVictory = false)
    {
        int baseTokens = SaveManager.Instance.MetaTokensPerRun;
        float ratio = SlotManager.Instance != null
            ? (float)FightsCompleted / Mathf.Max(1, SlotManager.Instance.FightsPerRun)
            : 1f;
        int total = Mathf.RoundToInt(baseTokens * (GameBalance.TokenRewardBaseMultiplier + ratio));
        if (isVictory)
            total = Mathf.RoundToInt(total * GameBalance.TokenRewardVictoryMultiplier);
        return total;
    }

    public int GetEffectiveShopCost(int baseCost)
    {
        if (MetaShopDiscountPercent > 0f)
            return Mathf.Max(1, Mathf.RoundToInt(baseCost * (1f - MetaShopDiscountPercent)));
        return baseCost;
    }
}
