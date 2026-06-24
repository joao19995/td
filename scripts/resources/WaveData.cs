using Godot;
using Godot.Collections;

[GlobalClass]
public partial class WaveData : Resource
{
    [Export] public Array<PackedScene> EnemyScenes;
    [Export] public int EnemyCount = 5;
    [Export] public float SpawnInterval = 1f; // segundos entre spawns dentro da wave
}