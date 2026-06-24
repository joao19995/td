using Godot;

public partial class Map1 : Node2D
{
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("test_next_wave"))
        {
            var spawner = GetNode<EnemySpawner>("EnemySpawner");
            if (spawner.CanStartNextWave)
                spawner.StartNextWave();
        }
    }
}
