using Godot;
using Godot.Collections;

public enum WaveModifier { None, Horde, Armored, Swift, GoldRush }

[GlobalClass]
public partial class WaveData : Resource
{
    [Export] public Array<WaveEntry> Entries { get; set; }
    [Export] public float SpawnInterval { get; set; } = 1f;
    [Export] public WaveModifier Modifier { get; set; } = WaveModifier.None;

    // Runtime-only: set by RunState.PickRunWaves() before spawning
    public float DifficultyMultiplier { get; set; } = 1f;
    public bool IsFinalStretch { get; set; } = false;
}
