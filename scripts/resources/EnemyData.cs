using Godot;

[GlobalClass]
public partial class EnemyData : Resource
{
    [Export] public string EnemyName { get; set; } = "Enemy";
    [Export] public float MaxHealth { get; set; } = 10f;
    [Export] public float MoveSpeed { get; set; } = 60f;
    [Export] public int RewardGold { get; set; } = 5;
    [Export] public int DamageToPlayer { get; set; } = 1;
    [Export] public Texture2D Sprite { get; set; }
}
