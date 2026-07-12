using System;
using System.Collections.Generic;

public static class CombatLog
{
    /// <summary>Fired on every instance of damage applied. Subscribers (telemetry) use this.</summary>
    public static event Action<DamageContext, DamageResult> DamageRecorded;

    private static readonly Dictionary<string, float> _damageByTower = new();
    private static readonly Dictionary<string, int> _killsByTower = new();

    public static void RecordDamage(DamageContext ctx, DamageResult result)
    {
        if (!string.IsNullOrEmpty(ctx.SourceTowerId))
        {
            _damageByTower.TryGetValue(ctx.SourceTowerId, out float current);
            _damageByTower[ctx.SourceTowerId] = current + result.ActualDamage;
        }

        DamageRecorded?.Invoke(ctx, result);
    }

    public static void RecordKill(string sourceTowerId)
    {
        if (string.IsNullOrEmpty(sourceTowerId)) return;
        _killsByTower.TryGetValue(sourceTowerId, out int current);
        _killsByTower[sourceTowerId] = current + 1;
    }

    /// <summary>Returns a copy of the current per-tower damage totals. Used for fight snapshots.</summary>
    public static Dictionary<string, float> GetDamageByTower() => new(_damageByTower);

    /// <summary>Returns a copy of the current per-tower kill totals. Used for fight snapshots.</summary>
    public static Dictionary<string, int> GetKillsByTower() => new(_killsByTower);

    /// <summary>Resets all accumulators. Called at run start (RunState.StartRun).</summary>
    public static void Reset()
    {
        _damageByTower.Clear();
        _killsByTower.Clear();
    }

    /// <summary>Resets only damage/kill accumulators (not the event subscriber list). Called between fights for delta tracking.</summary>
    public static void ResetDamageKills()
    {
        _damageByTower.Clear();
        _killsByTower.Clear();
    }
}
