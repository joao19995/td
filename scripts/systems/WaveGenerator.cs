using Godot;
using Godot.Collections;

public static class WaveGenerator
{
    private static readonly WaveModifier[] AllModifiers = new[] {
        WaveModifier.None, WaveModifier.Horde, WaveModifier.Armored,
        WaveModifier.Swift, WaveModifier.GoldRush
    };

    private static readonly WaveModifier[] HardModifiers = new[] {
        WaveModifier.Horde, WaveModifier.Armored
    };

    public static string GetWaveTier(int fightsCompleted, int fightsPerRun)
    {
        float ratio = fightsPerRun > 0
            ? (float)fightsCompleted / fightsPerRun
            : 0f;
        if (ratio >= GameBalance.Tier3Threshold) return "tier3";
        if (ratio >= GameBalance.Tier2Threshold) return "tier2";
        return "tier1";
    }

    public static Array<WaveData> PickRunWaves(int fightsCompleted, int fightsPerRun)
    {
        string tier = GetWaveTier(fightsCompleted, fightsPerRun);
        string dirPath = $"res://resources/wave_data/{tier}/";
        var dir = DirAccess.Open(dirPath);
        if (dir == null)
        {
            GD.PrintErr($"WaveGenerator: failed to open wave tier dir {dirPath}");
            return null;
        }

        var pool = new System.Collections.Generic.List<WaveData>();
        foreach (var file in dir.GetFiles())
        {
            if (!file.EndsWith(".tres") && !file.EndsWith(".res"))
                continue;
            var res = ResourceLoader.Load<Resource>(dirPath + file, "", ResourceLoader.CacheMode.Replace);
            if (res is WaveData w)
                pool.Add(w);
        }

        if (pool.Count == 0) return null;

        int totalWaves = (int)(GD.Randi() % (GameBalance.MaxWaves - GameBalance.MinWaves + 1)) + GameBalance.MinWaves;
        var result = new Array<WaveData>();

        WaveModifier? prevModifier = null;

        for (int i = 0; i < totalWaves; i++)
        {
            var wave = pool[(int)(GD.Randi() % pool.Count)];
            var clone = new WaveData
            {
                Entries = wave.Entries,
                SpawnInterval = wave.SpawnInterval,
                Modifier = wave.Modifier,
            };

            clone.IsFinalStretch = i >= totalWaves - GameBalance.FinalStretchWaveOffset;
            clone.DifficultyMultiplier = GameBalance.DifficultyCurveMin + GameBalance.DifficultyCurveRange * (float)i / Mathf.Max(1, totalWaves - 1);

            if (i >= GameBalance.ModifierStartWave)
            {
                var mods = i >= totalWaves - GameBalance.HardModifierWaveOffset ? HardModifiers : AllModifiers;
                int attempts = 0;
                do {
                    clone.Modifier = mods[(int)(GD.Randi() % mods.Length)];
                    attempts++;
                } while (prevModifier.HasValue && clone.Modifier == prevModifier.Value && attempts < 10);
            }
            prevModifier = clone.Modifier;

            result.Add(clone);
        }

        return result;
    }
}
