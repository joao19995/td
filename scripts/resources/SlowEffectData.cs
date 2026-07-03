using Godot;

[GlobalClass]
public partial class SlowEffectData : StatusEffectData
{
    [Export] public float SpeedMultiplier { get; set; } = 0.5f;
}
