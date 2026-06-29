using Godot;

public partial class Tower : Node2D
{
    [Signal] public delegate void ClickedEventHandler(Tower tower);

    private TowerData _data;
    private int _currentUpgradeLevel;
    private TargetingComponent _targeting;
    private AttackComponent _attack;

    public TowerData Data => _data;
    public int CurrentUpgradeLevel => _currentUpgradeLevel;
    public int MaxUpgradeLevel => _data?.UpgradePath?.Count ?? 0;

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

        if (_data != null)
        {
            Clicked += tower => TowerSelectionManager.Instance?.SelectTower(tower);
            ApplyData();
        }
    }

    public void Setup(TowerData data)
    {
        _data = data;
        _currentUpgradeLevel = 0;

        if (IsInsideTree())
            ApplyData();
    }

    public void Upgrade()
    {
        if (_currentUpgradeLevel >= MaxUpgradeLevel) return;
        _currentUpgradeLevel++;
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
            return _data.Damage + bonus;
        }
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
            return _data.FireRate + bonus;
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
            return _data.Range + bonus;
        }
    }

    public int SellValue => _data != null
        ? Mathf.RoundToInt(_data.Cost * _data.SellRefundRatio)
        : 0;

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
        _attack.SetEffectiveStats(EffectiveDamage, EffectiveFireRate);
    }

    public override void _Process(double delta)
    {
        if (_data == null) return;

        var target = _targeting.SelectTarget();
        _attack.TryAttack(target);
    }
}
