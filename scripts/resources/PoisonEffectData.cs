using Godot;

[GlobalClass]
public partial class PoisonEffectData : StatusEffectData
{
    [Export] public float DamagePerTick { get; set; } = 5f;
    [Export] public float TickInterval { get; set; } = 1f;
}
