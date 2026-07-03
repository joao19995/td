using Godot;

[GlobalClass]
public partial class StatusEffectData : Resource
{
    [Export] public float Duration { get; set; } = 1f;
}
