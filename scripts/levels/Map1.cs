using Godot;

public partial class Map1 : Node2D
{
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("test_place_tower"))
        {
            var towerScene = GD.Load<PackedScene>("res://scenes/towers/Tower.tscn");
            var tileMap = GetNode<TileMapLayer>("TileMapLayer");
            TowerPlacementManager.Instance.StartPlacement(towerScene, tileMap);
        }
        if (@event.IsActionPressed("test_next_wave"))
        {
            var spawner = GetNode<EnemySpawner>("EnemySpawner");
            if (spawner.CanStartNextWave)
            {
                spawner.StartNextWave();
            }
        }
    }
}