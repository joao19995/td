using Godot;
using Godot.Collections;

[GlobalClass]
public partial class WaveData : Resource
{
    [Export] public Array<EnemyData> Enemies;
    [Export] public int EnemyCount = 5;
    [Export] public float SpawnInterval = 1f;
}
