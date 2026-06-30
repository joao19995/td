using Godot;

[GlobalClass]
public partial class MetaUpgradeData : Resource
{
    [Export] public string Id { get; set; } = "";
    [Export] public string Name { get; set; } = "";
    [Export] public string Description { get; set; } = "";
    [Export] public int CostTokens { get; set; } = 0;
    [Export] public int MaxLevel { get; set; } = 1;

    [Export] public bool IsTowerUnlock { get; set; } = false;
    [Export] public string TowerId { get; set; } = "";

    [Export] public string StatType { get; set; } = "";
    [Export] public float BonusPerLevel { get; set; } = 0f;
}
