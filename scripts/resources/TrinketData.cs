using Godot;

[GlobalClass]
public partial class TrinketData : Resource
{
    [Export] public string Id { get; set; } = "";
    [Export] public string Name { get; set; } = "";
    [Export] public string Description { get; set; } = "";
    [Export] public Texture2D Icon { get; set; }
    [Export] public float DamagePercentBonus { get; set; } = 0f;
    [Export] public float FireRateBonusPercent { get; set; } = 0f;
    [Export] public float RangePercentBonus { get; set; } = 0f;
    [Export] public int HealAmount { get; set; } = 0;
    [Export] public int GoldAmount { get; set; } = 0;
    [Export] public float CritDamageBonusPercent { get; set; } = 0f;
    [Export] public float StatusDurationBonusPercent { get; set; } = 0f;
    [Export] public float StatusStrengthBonusPercent { get; set; } = 0f;
    [Export] public int PassiveGoldPerInterval { get; set; } = 0;
    [Export] public float PassiveGoldInterval { get; set; } = 0f;
    [Export] public float RangeFlatBonus { get; set; } = 0f;
    [Export] public float BasicDamagePercentBonus { get; set; } = 0f;
}
