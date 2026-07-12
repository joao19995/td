using Godot;
using Godot.Collections;

public partial class SaveManager : Node
{
    public static SaveManager Instance { get; private set; }

    private const string SavePath = "user://save_data.json";
    private const string RunSavePath = "user://run_save.json";

    [Export] public int MetaTokensPerRun { get; set; } = 10;

    private int _metaTokens;
    private Array<string> _unlockedTowerIds = new();
    private Array<string> _unlockedActIds = new();
    private Dictionary<string, int> _metaUpgradeLevels = new();
    private Dictionary<string, bool> _discovered = new();
    private Array<string>[] _loadoutSlots = new Array<string>[3];

    public int MetaTokens => _metaTokens;
    public Array<string> UnlockedTowerIds => _unlockedTowerIds;
    public Array<string> UnlockedActIds => _unlockedActIds;

    private static readonly string[] DefaultTowerIds = { "bread_baker", "bread_courier", "aroma_keeper" };
    private static readonly string[] DefaultActIds = { "act1" };

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        LoadGame();
    }

    public void LoadGame()
    {
        if (!FileAccess.FileExists(SavePath))
        {
            SetDefaults();
            SaveGame();
            return;
        }

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PrintErr("SaveManager: failed to open save file, using defaults");
            SetDefaults();
            return;
        }

        string json = file.GetAsText();
        if (string.IsNullOrEmpty(json))
        {
            GD.PrintErr("SaveManager: save file is empty, using defaults");
            SetDefaults();
            return;
        }

        var parseResult = Json.ParseString(json);
        if (parseResult.VariantType != Variant.Type.Dictionary)
        {
            GD.PrintErr("SaveManager: save file is malformed, using defaults");
            SetDefaults();
            return;
        }

        var dict = parseResult.AsGodotDictionary();
        if (dict.ContainsKey("meta_tokens"))
            _metaTokens = (int)dict["meta_tokens"];

        if (dict.ContainsKey("unlocked_tower_ids") && dict["unlocked_tower_ids"].VariantType == Variant.Type.Array)
        {
            var raw = dict["unlocked_tower_ids"].AsGodotArray();
            _unlockedTowerIds = new Array<string>();
            foreach (var id in raw)
                _unlockedTowerIds.Add(id.AsString());
        }

        if (dict.ContainsKey("unlocked_act_ids") && dict["unlocked_act_ids"].VariantType == Variant.Type.Array)
        {
            var raw = dict["unlocked_act_ids"].AsGodotArray();
            _unlockedActIds = new Array<string>();
            foreach (var id in raw)
                _unlockedActIds.Add(id.AsString());
        }

        if (dict.ContainsKey("meta_upgrade_levels") && dict["meta_upgrade_levels"].VariantType == Variant.Type.Dictionary)
        {
            var raw = dict["meta_upgrade_levels"].AsGodotDictionary();
            _metaUpgradeLevels = new Dictionary<string, int>();
            foreach (var key in raw.Keys)
                _metaUpgradeLevels[key.AsString()] = (int)raw[key];
        }

        if (dict.ContainsKey("discovered") && dict["discovered"].VariantType == Variant.Type.Dictionary)
        {
            var raw = dict["discovered"].AsGodotDictionary();
            _discovered = new Dictionary<string, bool>();
            foreach (var key in raw.Keys)
                _discovered[key.AsString()] = true;
        }

        if (dict.ContainsKey("loadout_slots") && dict["loadout_slots"].VariantType == Variant.Type.Array)
        {
            var raw = dict["loadout_slots"].AsGodotArray();
            for (int s = 0; s < 3 && s < raw.Count; s++)
            {
                if (raw[s].VariantType == Variant.Type.Array)
                {
                    _loadoutSlots[s] = new Array<string>();
                    foreach (var id in raw[s].AsGodotArray())
                        _loadoutSlots[s].Add(id.AsString());
                }
            }
        }

        EnsureDefaultUnlocks();
        SanitizeLoadoutSlots();
    }

    public void SaveGame()
    {
        var upgradeLevelsDict = new Dictionary();
        foreach (var kv in _metaUpgradeLevels)
            upgradeLevelsDict[kv.Key] = kv.Value;

        var discoveredDict = new Dictionary();
        foreach (var kv in _discovered)
            discoveredDict[kv.Key] = kv.Value;

        var loadoutSlotsArray = new Array();
        for (int s = 0; s < 3; s++)
        {
            var slot = new Array();
            if (_loadoutSlots[s] != null)
                foreach (var id in _loadoutSlots[s])
                    slot.Add(id);
            loadoutSlotsArray.Add(slot);
        }

        var dict = new Dictionary
        {
            { "meta_tokens", _metaTokens },
            { "unlocked_tower_ids", _unlockedTowerIds },
            { "unlocked_act_ids", _unlockedActIds },
            { "meta_upgrade_levels", upgradeLevelsDict },
            { "discovered", discoveredDict },
            { "loadout_slots", loadoutSlotsArray }
        };

        string json = Json.Stringify(dict);
        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        if (file == null)
        {
            GD.PrintErr("SaveManager: failed to open save file for writing");
            return;
        }
        file.StoreString(json);
    }

    public void AddMetaTokens(int amount)
    {
        _metaTokens += amount;
        SaveGame();
    }

    public bool SpendMetaTokens(int amount)
    {
        if (_metaTokens < amount) return false;
        _metaTokens -= amount;
        SaveGame();
        return true;
    }

    public bool IsTowerUnlocked(string towerId)
    {
        return _unlockedTowerIds.Contains(towerId);
    }

    public void UnlockTower(string towerId)
    {
        if (!_unlockedTowerIds.Contains(towerId))
        {
            _unlockedTowerIds.Add(towerId);
            SaveGame();
        }
    }

    public bool IsActUnlocked(string actId)
    {
        return _unlockedActIds.Contains(actId);
    }

    public void UnlockAct(string actId)
    {
        if (!_unlockedActIds.Contains(actId))
        {
            _unlockedActIds.Add(actId);
            SaveGame();
        }
    }

    public int GetMetaUpgradeLevel(string upgradeId)
    {
        return _metaUpgradeLevels.ContainsKey(upgradeId) ? _metaUpgradeLevels[upgradeId] : 0;
    }

    public int GetMetaUpgradeLevelForTower(string towerId, string category)
    {
        var all = LoadAllMetaUpgrades();
        foreach (var mu in all)
        {
            if (mu.Category == category && mu.TowerId == towerId)
                return GetMetaUpgradeLevel(mu.Id);
        }
        return 0;
    }

    public int GetMetaUpgradeLevelForContent(string resourceType, string resourceId)
    {
        return GetMetaUpgradeLevel($"unlock_{resourceType}_{resourceId}");
    }

    private static System.Collections.Generic.List<MetaUpgradeData> _cachedMetaUpgrades;

    private static System.Collections.Generic.List<MetaUpgradeData> LoadAllMetaUpgrades()
    {
        if (_cachedMetaUpgrades == null)
            _cachedMetaUpgrades = ResourceLoaderHelper.LoadFromDir<MetaUpgradeData>("res://resources/meta_upgrade_data/");
        return _cachedMetaUpgrades;
    }

    public static void ClearMetaUpgradeCache()
    {
        _cachedMetaUpgrades = null;
    }

    public void SetMetaUpgradeLevel(string upgradeId, int level)
    {
        _metaUpgradeLevels[upgradeId] = level;
        SaveGame();
    }

    public bool IsDiscovered(string key)
    {
        return _discovered.ContainsKey(key) && _discovered[key];
    }

    public void MarkDiscovered(string key)
    {
        if (_discovered.ContainsKey(key)) return;
        _discovered[key] = true;
        SaveGame();
    }

    public Array<string> GetLoadoutSlot(int slot)
    {
        if (slot >= 0 && slot < 3 && _loadoutSlots[slot] != null)
            return _loadoutSlots[slot];
        return null;
    }

    public void SetLoadoutSlot(int slot, Array<string> ids)
    {
        if (slot >= 0 && slot < 3)
        {
            var filtered = new Array<string>();
            foreach (var id in ids)
                if (_unlockedTowerIds.Contains(id))
                    filtered.Add(id);
            _loadoutSlots[slot] = filtered;
            SaveGame();
        }
    }

    private void SanitizeLoadoutSlots()
    {
        bool changed = false;
        for (int s = 0; s < 3; s++)
        {
            if (_loadoutSlots[s] == null) continue;
            var filtered = new Array<string>();
            foreach (var id in _loadoutSlots[s])
                if (_unlockedTowerIds.Contains(id))
                    filtered.Add(id);
            if (filtered.Count != _loadoutSlots[s].Count)
            {
                _loadoutSlots[s] = filtered;
                changed = true;
            }
        }
        if (changed) SaveGame();
    }

    public void SaveRunState(Godot.Collections.Dictionary data)
    {
        string json = Json.Stringify(data);
        using var file = FileAccess.Open(RunSavePath, FileAccess.ModeFlags.Write);
        if (file == null)
        {
            GD.PrintErr("SaveManager: failed to open run save file for writing");
            return;
        }
        file.StoreString(json);
    }

    public Godot.Collections.Dictionary LoadRunState()
    {
        if (!FileAccess.FileExists(RunSavePath)) return null;
        using var file = FileAccess.Open(RunSavePath, FileAccess.ModeFlags.Read);
        if (file == null) return null;
        string json = file.GetAsText();
        if (string.IsNullOrEmpty(json)) return null;
        var result = Json.ParseString(json);
        if (result.VariantType != Variant.Type.Dictionary) return null;
        return result.AsGodotDictionary();
    }

    public void DeleteRunState()
    {
        if (FileAccess.FileExists(RunSavePath))
            DirAccess.RemoveAbsolute(RunSavePath);
    }

    public bool HasRunState()
    {
        return FileAccess.FileExists(RunSavePath);
    }

    private void SetDefaults()
    {
        _metaTokens = 0;
        _unlockedTowerIds = new Array<string>(DefaultTowerIds);
        _unlockedActIds = new Array<string>(DefaultActIds);
    }

    private void EnsureDefaultUnlocks()
    {
        bool changed = false;
        foreach (var id in DefaultTowerIds)
        {
            if (!_unlockedTowerIds.Contains(id))
            {
                _unlockedTowerIds.Add(id);
                changed = true;
            }
        }
        foreach (var id in DefaultActIds)
        {
            if (!_unlockedActIds.Contains(id))
            {
                _unlockedActIds.Add(id);
                changed = true;
            }
        }
        if (changed) SaveGame();
    }
}
