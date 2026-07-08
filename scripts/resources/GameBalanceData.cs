using Godot;

[GlobalClass]
public partial class GameBalanceData : Resource
{
    [Export] public float EliteHpMultiplier { get; set; } = 2f;
    [Export] public float EliteDamageMultiplier { get; set; } = 1.5f;
    [Export] public float EliteGoldMultiplier { get; set; } = 2f;

    [Export] public float MinibossStatMultiplier { get; set; } = 1.5f;

    [Export] public float HordeSpawnIntervalMultiplier { get; set; } = 0.5f;
    [Export] public int HordeEnemyCountMultiplier { get; set; } = 2;
    [Export] public float FinalStretchCountMultiplier { get; set; } = 1.5f;

    [Export] public float ArmoredHpMultiplier { get; set; } = 2f;
    [Export] public float ArmoredDamageMultiplier { get; set; } = 1f;
    [Export] public float ArmoredGoldMultiplier { get; set; } = 1f;
    [Export] public float SwiftSpeedMultiplier { get; set; } = 1.5f;
    [Export] public float GoldRushHpMultiplier { get; set; } = 1f;
    [Export] public float GoldRushDamageMultiplier { get; set; } = 1f;
    [Export] public float GoldRushGoldMultiplier { get; set; } = 2f;

    [Export] public float AntiBuffMultiplier { get; set; } = 0.5f;

    [Export] public float MetaDamagePercentPerLevel { get; set; } = 0.05f;
    [Export] public int StartingGoldPerLevel { get; set; } = 50;
    [Export] public int StartingLivesPerLevel { get; set; } = 2;
    [Export] public float MetaShopDiscountPerLevel { get; set; } = 0.05f;
    [Export] public float MetaRerollCostReductionPerLevel { get; set; } = 0.10f;
    [Export] public float MetaEnemyGoldBonusPerLevel { get; set; } = 0.10f;

    [Export] public float TokenRewardBaseMultiplier { get; set; } = 1f;
    [Export] public float TokenRewardVictoryMultiplier { get; set; } = 1.5f;

    [Export] public int PassiveGoldAmount { get; set; } = 5;
    [Export] public float PassiveGoldInterval { get; set; } = 30f;

    [Export] public float Tier3Threshold { get; set; } = 0.66f;
    [Export] public float Tier2Threshold { get; set; } = 0.33f;

    [Export] public int MinWaves { get; set; } = 5;
    [Export] public int MaxWaves { get; set; } = 10;
    [Export] public float DifficultyCurveMin { get; set; } = 0.6f;
    [Export] public float DifficultyCurveRange { get; set; } = 0.8f;
    [Export] public int FinalStretchWaveOffset { get; set; } = 3;
    [Export] public int ModifierStartWave { get; set; } = 2;
    [Export] public int HardModifierWaveOffset { get; set; } = 2;

    [Export] public float JudgmentProtocolCooldown { get; set; } = 10f;
    [Export] public float JudgmentSealCooldown { get; set; } = 5f;

    [Export] public float AuraScanInterval { get; set; } = 0.5f;

    [Export] public int StartingGold { get; set; } = 250;

    [Export] public float DifficultyScalingPerFight { get; set; } = 0.07f;
}
