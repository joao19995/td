using Godot;

[GlobalClass]
public partial class TowerData : Resource
{
    [Export] public string TowerName { get; set; } = "Tower";
    [Export] public float Damage { get; set; } = 5f;
    [Export] public float FireRate { get; set; } = 1f;
    [Export] public float Range { get; set; } = 100f;
    [Export] public int Cost { get; set; } = 50;
    [Export] public PackedScene ProjectileScene { get; set; }
    [Export] public Texture2D Sprite { get; set; }
}
