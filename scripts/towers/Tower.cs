using Godot;
using System.Collections.Generic;

public partial class Tower : Node2D
{
    [Signal] public delegate void ClickedEventHandler(Tower tower);

    private static readonly Dictionary<Tower, int> _antiBuffCount = new();

    public static bool IsAntiBuffed(Tower tower) => _antiBuffCount.ContainsKey(tower) && _antiBuffCount[tower] > 0;

    public static void AddAntiBuff(Tower tower)
    {
        if (!_antiBuffCount.ContainsKey(tower))
            _antiBuffCount[tower] = 0;
        _antiBuffCount[tower]++;
    }

    public static void RemoveAntiBuff(Tower tower)
    {
        if (!_antiBuffCount.ContainsKey(tower)) return;
        _antiBuffCount[tower]--;
        if (_antiBuffCount[tower] <= 0)
            _antiBuffCount.Remove(tower);
    }

    private TowerData _data;
    private int _currentUpgradeLevel;
    private TargetingComponent _targeting;
    private AttackComponent _attack;
    private AuraComponent _aura;
    private GlobalAuraComponent _globalAura;

    public TowerData Data => _data;
    public int CurrentUpgradeLevel => _currentUpgradeLevel;
    public int MaxUpgradeLevel => _data?.UpgradePath?.Count ?? 0;

    private string _equipId;
    private EquipData _equipData;
    private bool _auraOnly;
    private int _ancientStarterStacks;
    private int _ancientStarterAttackCounter;

    public override void _Ready()
    {
        _targeting = GetNode<TargetingComponent>("TargetingComponent");
        _attack = GetNode<AttackComponent>("AttackComponent");

        var selectionArea = GetNode<Area2D>("SelectionArea");
        selectionArea.InputEvent += (viewport, ev, shapeIdx) =>
        {
            if (ev is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
                EmitSignal(SignalName.Clicked, this);
        };

        AddToGroup("towers");

        if (_data != null)
        {
            Clicked += tower => TowerSelectionManager.Instance?.SelectTower(tower);
            SetupAura();
            SetupGlobalAura();
            ApplyData();
            SynergyManager.Instance.SynergiesChanged += OnSynergiesChanged;
        }
    }

    private void SetupAura()
    {
        if (_data == null || !_data.HasAura) return;
        _aura = new AuraComponent();
        _aura.AuraRange = _data.AuraRange;
        _aura.DamageBonusPercent = _data.AuraDamageBonusPercent;
        _aura.FireRateBonusPercent = _data.AuraFireRateBonusPercent;
        _aura.Name = "AuraComponent";
        AddChild(_aura);
    }

    private void SetupGlobalAura()
    {
        if (_data == null || !_data.HasGlobalAura) return;
        _globalAura = new GlobalAuraComponent();
        _globalAura.DamagePerTower = _data.GlobalAuraDamagePerTower;
        _globalAura.Name = "GlobalAuraComponent";
        AddChild(_globalAura);
    }

    public override void _ExitTree()
    {
        if (SynergyManager.Instance != null)
            SynergyManager.Instance.SynergiesChanged -= OnSynergiesChanged;
        _antiBuffCount.Remove(this);
    }

    private void OnSynergiesChanged()
    {
        ApplyData();
    }

    public void Setup(TowerData data)
    {
        _data = data;
        _currentUpgradeLevel = RunState.Instance?.GetTowerLevel(data.Id) ?? 0;
        _equipId = RunState.Instance?.GetEquippedItem(data.Id);
        _equipData = !string.IsNullOrEmpty(_equipId)
            ? GD.Load<EquipData>($"res://resources/equip_data/{_equipId}.tres")
            : null;
        _auraOnly = _equipId == "prayer_beads";
        _ancientStarterStacks = (_equipId == "ancient_starter") ? (RunState.Instance?.GetAncientStarterStacks(data.Id) ?? 0) : 0;
        _ancientStarterAttackCounter = (_equipId == "ancient_starter") ? (RunState.Instance?.GetAncientStarterAttackCounter(data.Id) ?? 0) : 0;

        if (IsInsideTree())
            ApplyData();
    }

    public string EquipId => _equipId;

    public void Upgrade()
    {
        if (_currentUpgradeLevel >= MaxUpgradeLevel) return;
        _currentUpgradeLevel++;
        RunState.Instance?.SetTowerLevel(_data.Id, _currentUpgradeLevel);
        ApplyData();
    }

    public float EffectiveDamage
    {
        get
        {
            if (_data == null) return 0f;
            float bonus = 0f;
            if (_data.UpgradePath != null)
                for (int i = 0; i < _currentUpgradeLevel && i < _data.UpgradePath.Count; i++)
                    bonus += _data.UpgradePath[i].DamageBonus;
            float baseWithUpgrade = _data.Damage + bonus + _ancientStarterStacks;
            float synergyPercent = SynergyManager.Instance?.GetDamageBonus(_data.Id) ?? 0f;
            float shopPercent = RunState.Instance?.ShopDamageBonusPercent ?? 0f;
            float metaPercent = RunState.Instance?.MetaDamageBonusPercent ?? 0f;
            float trinketPercent = RunState.Instance?.TrinketDamageBonusPercent ?? 0f;
            float equipPercent = GetEquipDamagePercent();
            float auraPercent = AuraComponent.GetDamageBonus(this);
            float globalAuraPercent = RunState.Instance?.GlobalAuraDamagePercent ?? 0f;
            float nearbyBonus = GetFirstStarterBonus();
            float buffMultiplier = 1f;
            if (IsAntiBuffed(this))
                buffMultiplier = 0.5f;
            return (baseWithUpgrade + nearbyBonus) * (1f + (synergyPercent + equipPercent + auraPercent + globalAuraPercent + trinketPercent) * buffMultiplier) * (1f + shopPercent) * (1f + metaPercent);
        }
    }

    private float GetFirstStarterBonus()
    {
        if (_equipId != "first_starter_relic") return 0f;
        int count = 0;
        var allTowers = GetTree().GetNodesInGroup("towers");
        foreach (var node in allTowers)
        {
            if (node == this) continue;
            if (node is Tower other)
            {
                float dist = other.GlobalPosition.DistanceTo(GlobalPosition);
                if (dist <= 40f)
                    count++;
            }
        }
        return _data.Damage * (0.05f * count);
    }

    public float EffectiveFireRate
    {
        get
        {
            if (_data == null) return 0f;
            float bonus = 0f;
            if (_data.UpgradePath != null)
                for (int i = 0; i < _currentUpgradeLevel && i < _data.UpgradePath.Count; i++)
                    bonus += _data.UpgradePath[i].FireRateBonus;
            float baseWithUpgrade = _data.FireRate + bonus;
            float synergyPercent = SynergyManager.Instance?.GetFireRateBonus(_data.Id) ?? 0f;
            float shopPercent = RunState.Instance?.ShopFireRateBonusPercent ?? 0f;
            float equipPercent = GetEquipFireRatePercent();
            float auraPercent = AuraComponent.GetFireRateBonus(this);
            float trinketPercent = RunState.Instance?.TrinketFireRateBonusPercent ?? 0f;
            return baseWithUpgrade * (1f + synergyPercent + equipPercent + auraPercent + trinketPercent) * (1f + shopPercent);
        }
    }

    public float EffectiveRange
    {
        get
        {
            if (_data == null) return 0f;
            float bonus = 0f;
            if (_data.UpgradePath != null)
                for (int i = 0; i < _currentUpgradeLevel && i < _data.UpgradePath.Count; i++)
                    bonus += _data.UpgradePath[i].RangeBonus;
            float flatBonus = RunState.Instance?.TrinketRangeFlatBonus ?? 0f;
            float baseWithUpgrade = _data.Range + bonus + flatBonus;
            float synergyPercent = SynergyManager.Instance?.GetRangeBonus(_data.Id) ?? 0f;
            float shopPercent = RunState.Instance?.ShopRangeBonusPercent ?? 0f;
            float equipPercent = GetEquipRangePercent();
            float trinketPercent = RunState.Instance?.TrinketRangeBonusPercent ?? 0f;
            return baseWithUpgrade * (1f + synergyPercent + equipPercent + trinketPercent) * (1f + shopPercent);
        }
    }

    private float GetEquipDamagePercent() => _equipData?.DamagePercentBonus ?? 0f;
    private float GetEquipFireRatePercent() => _equipData?.FireRatePercentBonus ?? 0f;
    private float GetEquipRangePercent() => _equipData?.RangePercentBonus ?? 0f;
    private float GetEquipCritChance() => _equipId == "tempered_crust_blade" ? 0.10f : 0f;


    public void RefreshStats()
    {
        ApplyData();
    }

    private void ApplyData()
    {
        var detectionArea = GetNode<Area2D>("DetectionArea");
        var shape = detectionArea.GetNode<CollisionShape2D>("CollisionShape2D");
        var currentShape = shape.Shape as CircleShape2D;
        var circle = currentShape != null ? (CircleShape2D)currentShape.Duplicate() : new CircleShape2D();
        circle.Radius = EffectiveRange;
        shape.Shape = circle;

        if (_data.Sprite != null)
            GetNode<Sprite2D>("Sprite2D").Texture = _data.Sprite;

        _attack.Setup(_data);
        _attack.SetEquipId(_equipId);
        _attack.SetEffectiveStats(EffectiveDamage, EffectiveFireRate);

        float trinketStatusDuration = RunState.Instance?.TrinketStatusDurationBonusPercent ?? 0f;
        float trinketStatusStrength = RunState.Instance?.TrinketStatusStrengthBonusPercent ?? 0f;
        float splashRadiusMult = _equipId == "reinforced_suspension" ? 1.15f : 1f;
        float slowDurationMult = (1f + trinketStatusDuration) * (_equipId == "spice_wind_chimes" ? 1.3f : (_equipId == "golden_proofing_bowl" ? 1.15f : 1f));
        float poisonDurationMult = (1f + trinketStatusDuration) * (_equipId == "golden_proofing_bowl" ? 1.15f : 1f);
        int extraChainBounces = _equipId == "wild_yeast" ? 1 : 0;
        _attack.SetEquipModifiers(splashRadiusMult, slowDurationMult, poisonDurationMult, extraChainBounces);

        if (_data.HasCrit)
        {
            float trinketCritDmg = RunState.Instance?.TrinketCritDamageBonusPercent ?? 0f;
            float equipCritChance = GetEquipCritChance();
            _attack.SetCritStats(_data.CritChance + equipCritChance, _data.CritMultiplier * (1f + trinketCritDmg));
        }

        if (_aura != null)
        {
            float auraRange = _data.AuraRange;
            float auraDamageBonus = _data.AuraDamageBonusPercent;
            float auraFireRateBonus = _data.AuraFireRateBonusPercent;
            if (_equipId == "sacred_robes")
                auraRange *= 1.15f;
            if (_equipId == "prayer_beads")
            {
                auraDamageBonus *= 1.05f;
                auraFireRateBonus *= 1.05f;
            }
            _aura.AuraRange = auraRange;
            _aura.DamageBonusPercent = auraDamageBonus;
            _aura.FireRateBonusPercent = auraFireRateBonus;
        }

        bool hasSynergy = SynergyManager.Instance?.IsTowerAffected(_data.Id) ?? false;
        Modulate = hasSynergy ? new Color(0.85f, 1, 0.85f) : Colors.White;
    }

    public override void _Process(double delta)
    {
        if (_data == null) return;
        if (_auraOnly) return;

        var target = _targeting.SelectTarget();
        if (_attack.TryAttack(target) && _equipId == "ancient_starter")
        {
            _ancientStarterAttackCounter++;
            if (_ancientStarterAttackCounter >= 10)
            {
                _ancientStarterAttackCounter = 0;
                _ancientStarterStacks++;
                RunState.Instance?.SetAncientStarterStacks(_data.Id, _ancientStarterStacks);
                RunState.Instance?.SetAncientStarterAttackCounter(_data.Id, _ancientStarterAttackCounter);
                RefreshStats();
            }
            else
            {
                RunState.Instance?.SetAncientStarterAttackCounter(_data.Id, _ancientStarterAttackCounter);
            }
        }
    }
}
