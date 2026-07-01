using Godot;
using Godot.Collections;

[GlobalClass]
public partial class SynergyData : Resource
{
    [Export] public string Id { get; set; } = "";
    [Export] public string DisplayName { get; set; } = "";
    [Export] public string Description { get; set; } = "";
    [Export] public Array<string> RequiredTowerIds { get; set; } = new();
    [Export] public int MinTowerCount { get; set; }
    [Export] public Array<string> AppliesToTowerIds { get; set; } = new();
    [Export] public float DamageBonusPercent { get; set; }
    [Export] public float FireRateBonusPercent { get; set; }
    [Export] public float RangeBonusPercent { get; set; }

    [Export] public Texture2D Icon { get; set; }
}
