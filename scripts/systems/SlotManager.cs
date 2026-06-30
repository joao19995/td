using Godot;

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

    public override void _EnterTree()
    {
        Instance = this;
    }

    public string Spin()
    {
        float total = WeightFight + WeightShop + WeightHeal + WeightMiniboss;
        float roll = (float)GD.RandRange(0.0, total);

        if (roll < WeightFight) return "Fight";
        roll -= WeightFight;
        if (roll < WeightShop) return "Shop";
        roll -= WeightShop;
        if (roll < WeightHeal) return "Heal";
        return "Miniboss";
    }
}
