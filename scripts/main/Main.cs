using Godot;

public partial class Main : Node2D
{
    public override void _Ready()
    {
        var mapScene = GD.Load<PackedScene>("res://scenes/levels/Map1.tscn");
        var map = mapScene.Instantiate();
        AddChild(map);
        
        var hud = GetNode<HUD>("HUD");
        var tileMap = map.GetNode<TileMapLayer>("TileMapLayer");
        var spawner = map.GetNode<EnemySpawner>("EnemySpawner");
        hud.SetActiveMap(tileMap, spawner);
    }
}