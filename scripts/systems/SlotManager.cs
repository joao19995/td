using Godot;
using System.Collections.Generic;

public partial class SlotManager : Node
{
    public static SlotManager Instance { get; private set; }

    [Export] public int FightsPerRun = 3;
    [Export] public float WeightFight = 45f;
    [Export] public float WeightShop = 25f;
    [Export] public float WeightHeal = 20f;
    [Export] public float WeightMiniboss = 10f;
    [Export] public int HealAmount = 5;
    [Export] public int MinibossGoldBonus = 50;
    [Export] public float MinibossStatMultiplier = 1.5f;
    [Export] public int RerollBaseCost = 50;
    [Export] public float SkewReductionFactor = 0.5f;

    private Dictionary<string, float> _weightMultipliers = new();

    public int CurrentRerollCount { get; private set; } = 0;

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
        _weightMultipliers = new Dictionary<string, float>
        {
            { "Fight", 1f }, { "Shop", 1f }, { "Heal", 1f }, { "Miniboss", 1f }
        };
    }

    public string Spin()
    {
        float wFight = WeightFight * _weightMultipliers.GetValueOrDefault("Fight", 1f);
        float wShop = WeightShop * _weightMultipliers.GetValueOrDefault("Shop", 1f);
        float wHeal = WeightHeal * _weightMultipliers.GetValueOrDefault("Heal", 1f);
        float wMiniboss = WeightMiniboss * _weightMultipliers.GetValueOrDefault("Miniboss", 1f);

        float total = wFight + wShop + wHeal + wMiniboss;
        float roll = (float)GD.RandRange(0.0, total);

        if (roll < wFight) return "Fight";
        roll -= wFight;
        if (roll < wShop) return "Shop";
        roll -= wShop;
        if (roll < wHeal) return "Heal";
        return "Miniboss";
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
