using Godot;

[GlobalClass]
public partial class StatusEffectData : Resource
{
    [Export] public float Duration { get; set; } = 1f;
    [Export] public string SourceTowerId { get; set; } = "";
    [Export] public int DamageType { get; set; } = 0;
}
