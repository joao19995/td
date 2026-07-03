using Godot;

[GlobalClass]
public partial class WaveEntry : Resource
{
    [Export] public EnemyData Enemy;
    [Export] public int Count = 1;
}
