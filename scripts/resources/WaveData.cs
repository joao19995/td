using Godot;
using Godot.Collections;

public enum WaveModifier { None, Horde, Armored, Swift, GoldRush }

[GlobalClass]
public partial class WaveData : Resource
{
    [Export] public Array<WaveEntry> Entries;
    [Export] public float SpawnInterval = 1f;
    [Export] public WaveModifier Modifier = WaveModifier.None;
}
