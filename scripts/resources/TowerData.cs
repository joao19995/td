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
    [Export] public string FlavorText { get; set; } = "";
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

    [Export] public bool HasAura { get; set; } = false;
    [Export] public float AuraRange { get; set; } = 60f;
    [Export] public float AuraDamageBonusPercent { get; set; } = 0f;
    [Export] public float AuraFireRateBonusPercent { get; set; } = 0f;

    [Export] public bool HasChain { get; set; } = false;
    [Export] public int ChainBounceCount { get; set; } = 1;
    [Export] public float ChainBounceRange { get; set; } = 40f;
    [Export] public float ChainBounceDamageMultiplier { get; set; } = 0.5f;

    [Export] public bool HasCrit { get; set; } = false;
    [Export] public float CritChance { get; set; } = 0.15f;
    [Export] public float CritMultiplier { get; set; } = 2.0f;

    [Export] public bool HasExecute { get; set; } = false;
    [Export] public float ExecuteThresholdHPPercent { get; set; } = 0.2f;
    [Export] public float ExecuteMultiplier { get; set; } = 2.0f;
    [Export] public float EliteBonusMultiplier { get; set; } = 1.5f;

    [Export] public bool HasGlobalAura { get; set; } = false;
    [Export] public float GlobalAuraDamagePerTower { get; set; } = 0.02f;

    public System.Collections.Generic.List<string> GetTags()
    {
        var tags = new System.Collections.Generic.List<string>();
        if (HasSplash) tags.Add("SPLASH");
        if (HasPoison) tags.Add("POISON");
        if (HasSlow) tags.Add("SLOW");
        if (HasAura) tags.Add("AURA");
        if (HasChain) tags.Add("CHAIN");
        if (HasCrit) tags.Add("CRIT");
        if (HasExecute) tags.Add("EXECUTE");
        if (HasGlobalAura) tags.Add("GLOBAL");
        return tags;
    }
}
