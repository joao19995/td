using Godot;

[GlobalClass]
public partial class UpgradeData : Resource
{
    [Export] public string UpgradeName { get; set; } = "";
    [Export] public int Cost { get; set; } = 0;
    [Export] public float DamageBonus { get; set; } = 0f;
    [Export] public float FireRateBonus { get; set; } = 0f;
    [Export] public float RangeBonus { get; set; } = 0f;
}
