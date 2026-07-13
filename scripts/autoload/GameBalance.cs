using Godot;

public partial class GameBalance : Node
{
    public static GameBalance Instance { get; private set; }
    public static GameBalanceData Data { get; private set; }

    public override void _EnterTree()
    {
        Instance = this;
        Data = ResourceLoader.Load<GameBalanceData>("res://resources/game_balance.tres",
            "", ResourceLoader.CacheMode.Replace);
        if (Data == null)
        {
            GD.PushWarning("GameBalance: failed to load game_balance.tres, using defaults");
            Data = new GameBalanceData();
        }
    }

    public static float EliteHpMultiplier => Data.EliteHpMultiplier;
    public static float EliteDamageMultiplier => Data.EliteDamageMultiplier;
    public static float EliteGoldMultiplier => Data.EliteGoldMultiplier;
    public static float MinibossStatMultiplier => Data.MinibossStatMultiplier;
    public static float HordeSpawnIntervalMultiplier => Data.HordeSpawnIntervalMultiplier;
    public static int HordeEnemyCountMultiplier => Data.HordeEnemyCountMultiplier;
    public static float FinalStretchCountMultiplier => Data.FinalStretchCountMultiplier;
    public static float ArmoredHpMultiplier => Data.ArmoredHpMultiplier;
    public static float ArmoredDamageMultiplier => Data.ArmoredDamageMultiplier;
    public static float ArmoredGoldMultiplier => Data.ArmoredGoldMultiplier;
    public static float SwiftSpeedMultiplier => Data.SwiftSpeedMultiplier;
    public static float GoldRushHpMultiplier => Data.GoldRushHpMultiplier;
    public static float GoldRushDamageMultiplier => Data.GoldRushDamageMultiplier;
    public static float GoldRushGoldMultiplier => Data.GoldRushGoldMultiplier;
    public static float AntiBuffMultiplier => Data.AntiBuffMultiplier;
    public static float MetaDamagePercentPerLevel => Data.MetaDamagePercentPerLevel;
    public static int StartingGoldPerLevel => Data.StartingGoldPerLevel;
    public static int StartingLivesPerLevel => Data.StartingLivesPerLevel;
    public static float MetaShopDiscountPerLevel => Data.MetaShopDiscountPerLevel;
    public static float MetaRerollCostReductionPerLevel => Data.MetaRerollCostReductionPerLevel;
    public static float MetaEnemyGoldBonusPerLevel => Data.MetaEnemyGoldBonusPerLevel;
    public static float TokenRewardBaseMultiplier => Data.TokenRewardBaseMultiplier;
    public static float TokenRewardVictoryMultiplier => Data.TokenRewardVictoryMultiplier;
    public static int PassiveGoldAmount => Data.PassiveGoldAmount;
    public static float PassiveGoldInterval => Data.PassiveGoldInterval;
    public static float Tier3Threshold => Data.Tier3Threshold;
    public static float Tier2Threshold => Data.Tier2Threshold;
    public static int MinWaves => Data.MinWaves;
    public static int MaxWaves => Data.MaxWaves;
    public static float DifficultyCurveMin => Data.DifficultyCurveMin;
    public static float DifficultyCurveRange => Data.DifficultyCurveRange;
    public static int FinalStretchWaveOffset => Data.FinalStretchWaveOffset;
    public static int ModifierStartWave => Data.ModifierStartWave;
    public static int HardModifierWaveOffset => Data.HardModifierWaveOffset;
    public static float JudgmentProtocolCooldown => Data.JudgmentProtocolCooldown;
    public static float JudgmentSealCooldown => Data.JudgmentSealCooldown;
    public static float AuraScanInterval => Data.AuraScanInterval;
    public static int StartingGold => Data.StartingGold;
    public static uint EnemyCollisionMask => Data.EnemyCollisionMask;
    public static float DifficultyScalingPerFight => Data.DifficultyScalingPerFight;
}
