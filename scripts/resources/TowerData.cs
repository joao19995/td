using Godot;
using Godot.Collections;

[GlobalClass]
public partial class TowerData : Resource
{
    [Export] public string Id { get; set; } = "";
    [Export] public string TowerName { get; set; } = "Tower";
    [Export] public float Damage { get; set; } = 5f;
    [Export] public float FireRate { get; set; } = 1f;
    [Export] public float Range { get; set; } = 100f;
    [Export] public int Cost { get; set; } = 50;
    [Export] public PackedScene ProjectileScene { get; set; }
    [Export] public Texture2D Sprite { get; set; }
    [Export] public bool HasSplash { get; set; } = false;
    [Export] public float SplashRadius { get; set; } = 36f;
    [Export] public bool HasPoison { get; set; } = false;
    [Export] public float PoisonDamagePerTick { get; set; } = 2f;
    [Export] public float PoisonDuration { get; set; } = 4f;
    [Export] public bool HasSlow { get; set; } = false;
    [Export] public float SlowMultiplier { get; set; } = 0.5f;
    [Export] public float SlowDuration { get; set; } = 2f;
    [Export] public Array<UpgradeData> UpgradePath { get; set; } = new();
    [Export] public float SellRefundRatio { get; set; } = 0.5f;
}
