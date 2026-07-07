using Godot;

public partial class GlobalAuraComponent : Node2D
{
    private float _scanTimer;

    [Export] public float DamagePerTower { get; set; } = 0.02f;

    public override void _Process(double delta)
    {
        _scanTimer -= (float)delta;
        if (_scanTimer > 0f) return;
        _scanTimer = GameBalance.AuraScanInterval;

        var towers = GetTree().GetNodesInGroup("towers");
        int count = towers.Count;
        if (RunState.Instance != null)
            RunState.Instance.GlobalAuraDamagePercent = count * DamagePerTower;
    }

    public override void _ExitTree()
    {
        if (RunState.Instance != null)
            RunState.Instance.GlobalAuraDamagePercent = 0f;
    }
}
