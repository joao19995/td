using Godot;
using Godot.Collections;

public partial class SaveManager : Node
{
    public static SaveManager Instance { get; private set; }

    private const string SavePath = "user://save_data.json";

    [Export] public int MetaTokensPerRun { get; set; } = 10;

    private int _metaTokens;
    private Array<string> _unlockedTowerIds = new();

    public int MetaTokens => _metaTokens;
    public Array<string> UnlockedTowerIds => _unlockedTowerIds;

    private static readonly string[] DefaultTowerIds = { "base", "fast", "ice", "poison", "splash" };

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

        EnsureDefaultUnlocks();
    }

    public void SaveGame()
    {
        var dict = new Dictionary
        {
            { "meta_tokens", _metaTokens },
            { "unlocked_tower_ids", _unlockedTowerIds }
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

    private void SetDefaults()
    {
        _metaTokens = 0;
        _unlockedTowerIds = new Array<string>(DefaultTowerIds);
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
        if (changed) SaveGame();
    }
}
