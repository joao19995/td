using Godot;

[GlobalClass]
public partial class WaveEntry : Resource
{
    [Export] public EnemyData Enemy { get; set; }
    [Export] public int Count { get; set; } = 1;
}
