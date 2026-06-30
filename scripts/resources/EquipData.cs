using Godot;

[GlobalClass]
public partial class EquipData : Resource
{
    [Export] public string Id { get; set; } = "";
    [Export] public string Name { get; set; } = "";
    [Export] public string Description { get; set; } = "";
    [Export] public Texture2D Icon { get; set; }
    [Export] public int Cost { get; set; } = 0;
    [Export] public string TargetTowerId { get; set; } = "";
    [Export] public float DamagePercentBonus { get; set; } = 0f;
    [Export] public float FireRatePercentBonus { get; set; } = 0f;
    [Export] public float RangePercentBonus { get; set; } = 0f;
}
