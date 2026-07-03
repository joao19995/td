using Godot;
using System.Collections.Generic;

public partial class AuraComponent : Node2D
{
    private static readonly Dictionary<Tower, (float damage, float fireRate)> _activeBuffs = new();

    private readonly HashSet<Tower> _affectedTowers = new();
    private float _scanTimer;
    private bool _active = true;

    [Export] public float AuraRange { get; set; } = 60f;
    [Export] public float DamageBonusPercent { get; set; } = 0.1f;
    [Export] public float FireRateBonusPercent { get; set; } = 0.1f;

    public static float GetDamageBonus(Tower tower)
    {
        return _activeBuffs.TryGetValue(tower, out var b) ? b.damage : 0f;
    }

    public static float GetFireRateBonus(Tower tower)
    {
        return _activeBuffs.TryGetValue(tower, out var b) ? b.fireRate : 0f;
    }

    public static void ClearAll()
    {
        _activeBuffs.Clear();
    }

    public void SetActive(bool value)
    {
        _active = value;
        if (!_active)
        {
            foreach (var t in _affectedTowers)
                RemoveBuff(t);
            _affectedTowers.Clear();
        }
    }

    public override void _ExitTree()
    {
        foreach (var t in _affectedTowers)
            if (GodotObject.IsInstanceValid(t))
                RemoveBuff(t);
        _affectedTowers.Clear();
    }

    public override void _Process(double delta)
    {
        if (!_active) return;

        _scanTimer -= (float)delta;
        if (_scanTimer > 0f) return;
        _scanTimer = 0.5f;

        var myPos = GlobalPosition;
        var allTowers = GetTree().GetNodesInGroup("towers");
        var currentInRange = new HashSet<Tower>();

        foreach (var node in allTowers)
        {
            if (node == GetParent()) continue;
            if (node is Tower tower && GodotObject.IsInstanceValid(tower))
            {
                float dist = tower.GlobalPosition.DistanceTo(myPos);
                if (dist <= AuraRange)
                    currentInRange.Add(tower);
            }
        }

        foreach (var tower in currentInRange)
        {
            if (!_affectedTowers.Contains(tower))
                ApplyBuff(tower);
        }

        var left = new List<Tower>();
        foreach (var tower in _affectedTowers)
        {
            if (!currentInRange.Contains(tower))
                left.Add(tower);
        }
        foreach (var tower in left)
        {
            _affectedTowers.Remove(tower);
            RemoveBuff(tower);
        }
    }

    private void ApplyBuff(Tower tower)
    {
        _affectedTowers.Add(tower);
        float curDmg = 0f, curFr = 0f;
        if (_activeBuffs.TryGetValue(tower, out var b)) { curDmg = b.damage; curFr = b.fireRate; }
        _activeBuffs[tower] = (curDmg + DamageBonusPercent, curFr + FireRateBonusPercent);
        if (GodotObject.IsInstanceValid(tower))
            tower.RefreshStats();
    }

    private void RemoveBuff(Tower tower)
    {
        float curDmg = 0f, curFr = 0f;
        if (_activeBuffs.TryGetValue(tower, out var b)) { curDmg = b.damage; curFr = b.fireRate; }
        float newDamage = curDmg - DamageBonusPercent;
        float newFireRate = curFr - FireRateBonusPercent;
        if (newDamage <= 0f && newFireRate <= 0f)
            _activeBuffs.Remove(tower);
        else
            _activeBuffs[tower] = (newDamage, newFireRate);
        if (GodotObject.IsInstanceValid(tower))
            tower.RefreshStats();
    }
}
