using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public class RunAnalytics : IDisposable
{
    private readonly DateTime _runStartTime;
    private DateTime _fightStartTime;
    private int _currentFightNumber;
    private bool _disposed;

    private readonly System.Collections.Generic.Dictionary<string, int> _leaksByEnemy = new();
    private readonly List<Dictionary> _fightSnapshots = new();
    private readonly List<string> _slotOutcomes = new();

    private int _goldEarnedThisFight;
    private int _goldEarnedTotal;
    private int _goldSpentThisFight;
    private int _goldSpentTowers;
    private int _goldSpentUpgrades;
    private int _goldSpentEquipment;
    private int _goldSpentRerolls;
    private int _livesLostThisFight;
    private int _livesLostTotal;

    public RunAnalytics()
    {
        _runStartTime = DateTime.UtcNow;
        _fightStartTime = _runStartTime;
        EventBus.Instance.EnemyReachedEnd += OnEnemyReachedEnd;
        GD.Print("[RunAnalytics] Initialized");
    }

    private void OnEnemyReachedEnd(int damage)
    {
        if (_disposed) return;
        _livesLostThisFight += damage;
        _livesLostTotal += damage;
    }

    public void StartFight(int fightNumber)
    {
        _currentFightNumber = fightNumber;
        _fightStartTime = DateTime.UtcNow;
        _livesLostThisFight = 0;
        _goldEarnedThisFight = 0;
        _goldSpentThisFight = 0;
    }

    public void EndFight()
    {
        double duration = (DateTime.UtcNow - _fightStartTime).TotalSeconds;
        int goldRemaining = EconomyManager.Instance?.CurrentMoney ?? 0;

        var snapshot = new Dictionary
        {
            { "fight_number", _currentFightNumber },
            { "duration_seconds", duration },
            { "lives_lost", _livesLostThisFight },
            { "gold_earned", _goldEarnedThisFight },
            { "gold_spent", _goldSpentThisFight },
            { "gold_remaining", goldRemaining },
            { "damage_by_tower", ToGodotDict(CombatLog.GetDamageByTower()) },
            { "kills_by_tower", ToGodotDict(CombatLog.GetKillsByTower()) },
            { "leaks_by_enemy", ToGodotDict(_leaksByEnemy) }
        };
        _fightSnapshots.Add(snapshot);
        CombatLog.ResetDamageKills();
    }

    public void RecordGoldEarned(int amount)
    {
        _goldEarnedThisFight += amount;
        _goldEarnedTotal += amount;
    }

    public void RecordGoldSpent(string category, int amount)
    {
        _goldSpentThisFight += amount;
        switch (category)
        {
            case "tower": _goldSpentTowers += amount; break;
            case "upgrade": _goldSpentUpgrades += amount; break;
            case "equipment": _goldSpentEquipment += amount; break;
            case "reroll": _goldSpentRerolls += amount; break;
        }
    }

    public void RecordSlotOutcome(string outcome) => _slotOutcomes.Add(outcome);

    public void RecordLeak(string enemyId)
    {
        _leaksByEnemy.TryGetValue(enemyId, out int current);
        _leaksByEnemy[enemyId] = current + 1;
    }

    public Dictionary Serialize(bool isVictory)
    {
        double totalDuration = (DateTime.UtcNow - _runStartTime).TotalSeconds;
        var runState = RunState.Instance;

        return new Dictionary
        {
            { "version", "v0" },
            { "timestamp", DateTime.UtcNow.ToString("o") },
            { "run_id", Guid.NewGuid().ToString("N")[..8] },
            { "selected_act", runState?.SelectedAct?.Id ?? "unknown" },
            { "selected_towers", ToGodotArray(runState?.SelectedTowerIds) },
            { "victory", isVictory },
            { "duration_seconds", totalDuration },
            { "total_fights", _fightSnapshots.Count },
            { "fights", ToGodotArray(_fightSnapshots) },
            { "totals", new Dictionary
                {
                    { "damage_by_tower", SumDamageByTower(_fightSnapshots) },
                    { "kills_by_tower", SumKillsByTower(_fightSnapshots) },
                    { "leaks_by_enemy", ToGodotDict(_leaksByEnemy) },
                    { "gold_earned", _goldEarnedTotal },
                    { "gold_spent", new Dictionary
                        {
                            { "towers", _goldSpentTowers },
                            { "upgrades", _goldSpentUpgrades },
                            { "equipment", _goldSpentEquipment },
                            { "rerolls", _goldSpentRerolls }
                        }
                    },
                    { "lives_lost", _livesLostTotal }
                }
            },
            { "items", new Dictionary
                {
                    { "equipment", GetEquipmentDict() },
                    { "trinkets", ToGodotArray(runState?.AppliedTrinketIds) },
                    { "shop_items", ToGodotArray(runState?.PurchasedShopItemIds) },
                    { "synergies", GetActiveSynergies() }
                }
            },
            { "slot_outcomes", new Godot.Collections.Array<string>(_slotOutcomes) }
        };
    }

    private Dictionary GetEquipmentDict()
    {
        var result = new Dictionary();
        var runState = RunState.Instance;
        if (runState?.SelectedTowerIds == null) return result;
        foreach (var towerId in runState.SelectedTowerIds)
        {
            var equip = runState.GetEquippedItem(towerId);
            if (equip != null)
                result[towerId] = equip;
        }
        return result;
    }

    private Array<string> GetActiveSynergies()
    {
        var result = new Array<string>();
        var synergies = SynergyManager.Instance?.GetActiveSynergies();
        if (synergies == null) return result;
        foreach (SynergyData s in synergies)
            result.Add(s.Id);
        return result;
    }

    private static Dictionary ToGodotDict(System.Collections.Generic.Dictionary<string, float> dict)
    {
        var result = new Dictionary();
        foreach (var kv in dict)
            result[kv.Key] = kv.Value;
        return result;
    }

    private static Dictionary ToGodotDict(System.Collections.Generic.Dictionary<string, int> dict)
    {
        var result = new Dictionary();
        foreach (var kv in dict)
            result[kv.Key] = kv.Value;
        return result;
    }

    private static Dictionary SumDamageByTower(List<Dictionary> snapshots)
    {
        var aggregated = new System.Collections.Generic.Dictionary<string, float>();
        foreach (var snapshot in snapshots)
        {
            if (!snapshot.TryGetValue("damage_by_tower", out Variant value)) continue;
            if (value.VariantType != Variant.Type.Dictionary) continue;
            var dict = value.AsGodotDictionary();
            foreach (var key in dict.Keys)
            {
                string towerId = key.AsString();
                aggregated.TryGetValue(towerId, out float current);
                aggregated[towerId] = current + (float)(double)dict[key];
            }
        }
        return ToGodotDict(aggregated);
    }

    private static Dictionary SumKillsByTower(List<Dictionary> snapshots)
    {
        var aggregated = new System.Collections.Generic.Dictionary<string, int>();
        foreach (var snapshot in snapshots)
        {
            if (!snapshot.TryGetValue("kills_by_tower", out Variant value)) continue;
            if (value.VariantType != Variant.Type.Dictionary) continue;
            var dict = value.AsGodotDictionary();
            foreach (var key in dict.Keys)
            {
                string towerId = key.AsString();
                aggregated.TryGetValue(towerId, out int current);
                aggregated[towerId] = current + (int)(long)dict[key];
            }
        }
        return ToGodotDict(aggregated);
    }

    private static Array<string> ToGodotArray(IEnumerable<string> source)
    {
        var result = new Array<string>();
        if (source == null) return result;
        foreach (var id in source)
            result.Add(id);
        return result;
    }

    private static Godot.Collections.Array ToGodotArray(System.Collections.Generic.List<Dictionary> snapshots)
    {
        var result = new Godot.Collections.Array();
        foreach (var s in snapshots)
            result.Add(s);
        return result;
    }

    public void ExportJson(bool isVictory)
    {
        var dict = Serialize(isVictory);
        var json = Json.Stringify(dict, "\t");
        DirAccess.MakeDirRecursiveAbsolute("user://analytics");
        string timestamp = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
        string id = Guid.NewGuid().ToString("N")[..8];
        string path = $"user://analytics/run_{timestamp}_{id}.json";
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        if (file != null)
            file.StoreString(json);
        else
            GD.PushError($"[RunAnalytics] Failed to write {path}");
        GD.Print($"[RunAnalytics] Exported {path}");
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (EventBus.Instance != null)
        {
            EventBus.Instance.EnemyReachedEnd -= OnEnemyReachedEnd;
        }
        GD.Print("[RunAnalytics] Disposed");
    }
}
