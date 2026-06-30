using Godot;

[GlobalClass]
public partial class TrinketData : Resource
{
    [Export] public string Id { get; set; } = "";
    [Export] public string Name { get; set; } = "";
    [Export] public string Description { get; set; } = "";
    [Export] public Texture2D Icon { get; set; }
    [Export] public float DamagePercentBonus { get; set; } = 0f;
    [Export] public int HealAmount { get; set; } = 0;
    [Export] public int GoldAmount { get; set; } = 0;
}
