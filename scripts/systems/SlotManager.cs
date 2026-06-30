using Godot;
using System.Collections.Generic;

public partial class SlotManager : Node
{
    public static SlotManager Instance { get; private set; }

    [Export] public int FightsPerRun = 3;
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

    private Dictionary<string, float> _weightMultipliers = new();

    public int CurrentRerollCount { get; private set; } = 0;

    private static readonly string[] AllOutcomes = { "Fight", "Shop", "Heal", "Miniboss", "Treasure" };

    public override void _EnterTree()
    {
        Instance = this;
    }

    public int GetRerollCost()
    {
        return RerollBaseCost * (CurrentRerollCount + 1);
    }

    public void ResetRerolls()
    {
        CurrentRerollCount = 0;
        _weightMultipliers = new Dictionary<string, float>();
        foreach (var o in AllOutcomes)
            _weightMultipliers[o] = 1f;
    }

    public string Spin()
    {
        float wFight = WeightFight * _weightMultipliers.GetValueOrDefault("Fight", 1f);
        float wShop = WeightShop * _weightMultipliers.GetValueOrDefault("Shop", 1f);
        float wHeal = WeightHeal * _weightMultipliers.GetValueOrDefault("Heal", 1f);
        float wMiniboss = WeightMiniboss * _weightMultipliers.GetValueOrDefault("Miniboss", 1f);
        float wTreasure = WeightTreasure * _weightMultipliers.GetValueOrDefault("Treasure", 1f);

        float total = wFight + wShop + wHeal + wMiniboss + wTreasure;
        float roll = (float)GD.RandRange(0.0, total);

        if (roll < wFight) return "Fight";
        roll -= wFight;
        if (roll < wShop) return "Shop";
        roll -= wShop;
        if (roll < wHeal) return "Heal";
        roll -= wHeal;
        if (roll < wMiniboss) return "Miniboss";
        return "Treasure";
    }

    public string Reroll()
    {
        CurrentRerollCount++;
        return Spin();
    }

    public void ApplySkew(string outcome)
    {
        if (_weightMultipliers.ContainsKey(outcome))
            _weightMultipliers[outcome] *= SkewReductionFactor;
    }

}
