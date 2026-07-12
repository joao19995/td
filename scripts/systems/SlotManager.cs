using Godot;
using System.Collections.Generic;

public partial class SlotManager : Node
{
    public static SlotManager Instance { get; private set; }

    public const int DefaultFightsPerRun = 3;

    [Export] public int FightsPerRun = DefaultFightsPerRun;
    [Export] public float WeightFight = 35f;
    [Export] public float WeightShop = 20f;
    [Export] public float WeightHeal = 15f;
    [Export] public float WeightMiniboss = 10f;
    [Export] public float WeightTreasure = 20f;
    [Export] public int HealAmount = 5;
    [Export] public int MinibossGoldBonus = 50;
    [Export] public float MinibossStatMultiplier = 1.5f;
    [Export] public int RerollBaseCost = 50;
    [Export] public float SkewReductionFactor = 0.5f;

    private Dictionary<SlotOutcome, float> _weightMultipliers = new();
    private SlotOutcome? _forcedOutcome;

    public int CurrentRerollCount { get; private set; } = 0;

    private static readonly SlotOutcome[] AllOutcomes = { SlotOutcome.Fight, SlotOutcome.Shop, SlotOutcome.Heal, SlotOutcome.Miniboss, SlotOutcome.Treasure };

    public override void _EnterTree()
    {
        Instance = this;
    }

    public int GetRerollCost()
    {
        int baseCost = RerollBaseCost * (CurrentRerollCount + 1);
        float reduction = RunState.Instance?.MetaRerollCostReductionPercent ?? 0f;
        return Mathf.Max(1, Mathf.RoundToInt(baseCost * (1f - reduction)));
    }

    public void ResetRerolls()
    {
        CurrentRerollCount = 0;
        _weightMultipliers = new Dictionary<SlotOutcome, float>();
        foreach (var o in AllOutcomes)
            _weightMultipliers[o] = 1f;
    }

    public SlotOutcome Spin()
    {
        if (_forcedOutcome.HasValue)
        {
            var outcome = _forcedOutcome.Value;
            _forcedOutcome = null;  // Auto-clear after one use
            return outcome;
        }

        float wFight = WeightFight * _weightMultipliers.GetValueOrDefault(SlotOutcome.Fight, 1f);
        float wShop = WeightShop * _weightMultipliers.GetValueOrDefault(SlotOutcome.Shop, 1f);
        float wHeal = WeightHeal * _weightMultipliers.GetValueOrDefault(SlotOutcome.Heal, 1f);
        float wMiniboss = WeightMiniboss * _weightMultipliers.GetValueOrDefault(SlotOutcome.Miniboss, 1f);
        float wTreasure = WeightTreasure * _weightMultipliers.GetValueOrDefault(SlotOutcome.Treasure, 1f);

        float total = wFight + wShop + wHeal + wMiniboss + wTreasure;
        float roll = (float)GD.RandRange(0.0, total);

        if (roll < wFight) return SlotOutcome.Fight;
        roll -= wFight;
        if (roll < wShop) return SlotOutcome.Shop;
        roll -= wShop;
        if (roll < wHeal) return SlotOutcome.Heal;
        roll -= wHeal;
        if (roll < wMiniboss) return SlotOutcome.Miniboss;
        return SlotOutcome.Treasure;
    }

    public SlotOutcome Reroll()
    {
        CurrentRerollCount++;
        return Spin();
    }

    public void ApplySkew(SlotOutcome outcome)
    {
        _weightMultipliers[outcome] = _weightMultipliers.GetValueOrDefault(outcome, 1f) * SkewReductionFactor;
    }

    public void SetForcedOutcomeForDebug(SlotOutcome outcome)
    {
        if (!OS.IsDebugBuild()) return;
        _forcedOutcome = outcome;
    }

}
