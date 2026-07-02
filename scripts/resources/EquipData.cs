using Godot;

[GlobalClass]
public partial class EquipData : Resource
{
    [Export] public string Id { get; set; } = "";
    [Export] public string Name { get; set; } = "";
    [Export] public string Description { get; set; } = "";
    [Export] public Texture2D Icon { get; set; }
    [Export] public int Cost { get; set; } = 0;
    [Export] public string TargetTowerId { get; set; } = "";
    [Export] public float DamagePercentBonus { get; set; } = 0f;
    [Export] public float FireRatePercentBonus { get; set; } = 0f;
    [Export] public float RangePercentBonus { get; set; } = 0f;

    // Unique effect fields — magnitudes live here, not in C# branches
    [Export] public int ExtraChainBounces { get; set; } = 0;
    [Export] public float CritChanceBonus { get; set; } = 0f;
    [Export] public int PierceBonus { get; set; } = 0;
    [Export] public float AuraRangePercentBonus { get; set; } = 0f;
    [Export] public float AuraPotencyMultiplier { get; set; } = 1f;
    [Export] public bool DisablesAttack { get; set; } = false;
    [Export] public float SplashRadiusPercentBonus { get; set; } = 0f;
    [Export] public float ExecuteThresholdPercent { get; set; } = 0f;
    [Export] public float ExecuteCooldownSeconds { get; set; } = 0f;
    [Export] public float SplashOnCritRadius { get; set; } = 0f;
    [Export] public float PoisonSpreadRadius { get; set; } = 0f;
    [Export] public float EliteDamagePercentBonus { get; set; } = 0f;
    [Export] public float SlowDurationPercentBonus { get; set; } = 0f;
    [Export] public float StatusDurationPercentBonus { get; set; } = 0f;
}
