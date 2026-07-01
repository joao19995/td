using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class SynergyManager : Node
{
    public static SynergyManager Instance { get; private set; }

    [Signal] public delegate void SynergiesChangedEventHandler();

    private const string SynergyDir = "res://resources/synergy_data/";

    private readonly List<SynergyData> _activeSynergies = new();
    private readonly List<SynergyData> _synergies = new();

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        LoadSynergyDefinitions();
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

    private void LoadSynergyDefinitions()
    {
        _synergies.Clear();
        var dir = DirAccess.Open(SynergyDir);
        if (dir == null)
        {
            GD.PrintErr($"SynergyManager: failed to open {SynergyDir}");
            return;
        }

        using (dir)
        {
            foreach (var file in dir.GetFiles())
            {
                if (!file.EndsWith(".tres") && !file.EndsWith(".res"))
                    continue;

                var path = SynergyDir + file;
                var synergy = ResourceLoader.Load<SynergyData>(path, "", ResourceLoader.CacheMode.Replace);
                if (synergy != null)
                    _synergies.Add(synergy);
            }
        }
    }

    private void OnLevelLoaded(Node _)
    {
        _activeSynergies.Clear();
        EmitSignal(SignalName.SynergiesChanged);
    }

    private void OnTowerPlaced(int _)
    {
        ReEvaluate();
    }

    public void OnTowerRemoved()
    {
        ReEvaluate();
    }

    private void ReEvaluate()
    {
        _activeSynergies.Clear();
        var placedIds = TowerPlacementManager.Instance.PlacedTowerIds;
        var idList = new List<string>(placedIds);

        foreach (var synergy in _synergies)
        {
            if (synergy == null) continue;

            bool allRequiredPresent = true;
            foreach (var reqId in synergy.RequiredTowerIds)
            {
                if (!idList.Contains(reqId))
                {
                    allRequiredPresent = false;
                    break;
                }
            }

            if (allRequiredPresent && idList.Count >= synergy.MinTowerCount)
                _activeSynergies.Add(synergy);
        }

        foreach (var synergy in _activeSynergies)
            SaveManager.Instance?.MarkDiscovered($"synergy_{synergy.Id}");

        EmitSignal(SignalName.SynergiesChanged);
    }

    public bool IsTowerAffected(string towerId)
    {
        foreach (var synergy in _activeSynergies)
        {
            if (synergy.AppliesToTowerIds == null || synergy.AppliesToTowerIds.Count == 0)
                return true;
            if (synergy.AppliesToTowerIds.Contains(towerId))
                return true;
        }
        return false;
    }

    public float GetDamageBonus(string towerId)
    {
        float total = 0f;
        foreach (var synergy in _activeSynergies)
        {
            if (synergy.AppliesToTowerIds == null || synergy.AppliesToTowerIds.Count == 0 ||
                synergy.AppliesToTowerIds.Contains(towerId))
                total += synergy.DamageBonusPercent;
        }
        return total;
    }

    public float GetFireRateBonus(string towerId)
    {
        float total = 0f;
        foreach (var synergy in _activeSynergies)
        {
            if (synergy.AppliesToTowerIds == null || synergy.AppliesToTowerIds.Count == 0 ||
                synergy.AppliesToTowerIds.Contains(towerId))
                total += synergy.FireRateBonusPercent;
        }
        return total;
    }

    public float GetRangeBonus(string towerId)
    {
        float total = 0f;
        foreach (var synergy in _activeSynergies)
        {
            if (synergy.AppliesToTowerIds == null || synergy.AppliesToTowerIds.Count == 0 ||
                synergy.AppliesToTowerIds.Contains(towerId))
                total += synergy.RangeBonusPercent;
        }
        return total;
    }

    public bool IsSynergyActive(string synergyId)
    {
        foreach (var s in _activeSynergies)
            if (s.Id == synergyId) return true;
        return false;
    }

    public string GetActiveDisplayText()
    {
        if (_activeSynergies.Count == 0) return "";
        var names = new List<string>();
        foreach (var s in _activeSynergies)
            names.Add(s.DisplayName);
        return string.Join(", ", names);
    }
}
