using Godot;

[GlobalClass]
public partial class ActData : Resource
{
    [Export] public string Id { get; set; } = "";
    [Export] public string ActName { get; set; } = "";
    [Export] public string Description { get; set; } = "";
    [Export] public Texture2D PreviewTexture { get; set; }

    [Export] public string Tier1WaveDir { get; set; } = "res://resources/wave_data/tier1/";
    [Export] public string Tier2WaveDir { get; set; } = "res://resources/wave_data/tier2/";
    [Export] public string Tier3WaveDir { get; set; } = "res://resources/wave_data/tier3/";

    [Export] public WaveData BossWaveData { get; set; }

    [Export] public int FightsPerRunOverride { get; set; } = -1;

    [Export] public string RequiredPreviousActId { get; set; } = "";
}
