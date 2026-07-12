using Godot;

public partial class DevTools : Node
{
    public override void _UnhandledInput(InputEvent @event)
    {
        if (!OS.IsDebugBuild()) return;

        if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.S)
        {
            var level = LevelManager.Instance.CurrentLevelNode;
            if (level == null) return;

            // BaseLevel exposes Spawner as a public property
            if (level is BaseLevel baseLevel && baseLevel.Spawner != null)
                baseLevel.Spawner.SkipCurrentWave();
        }

        if (@event is InputEventKey keyEventF1 && keyEventF1.Pressed && keyEventF1.Keycode == Key.F1)
        {
            float[] scales = { 1f, 2f, 4f, 8f };
            float current = (float)Engine.TimeScale;
            int idx = System.Array.IndexOf(scales, current);
            int next = (idx + 1) % scales.Length;
            Engine.TimeScale = scales[next];
            GD.Print($"[DevTools] TimeScale: {Engine.TimeScale}x");
            GetViewport().SetInputAsHandled();
        }

        if (@event is InputEventKey keyEventF2 && keyEventF2.Pressed && keyEventF2.Keycode == Key.F2)
        {
            var runState = RunState.Instance;
            if (runState == null || !runState.IsRunActive) return;
            runState.SetFightsCompletedForDebug(runState.FightsCompleted + 1);
            runState.SetBossFight(false);
            LevelManager.Instance.PickRandomLevel();
            // The BriefingScreen flow handles the rest
            GetViewport().SetInputAsHandled();
        }

        if (@event is InputEventKey keyEventF3 && keyEventF3.Pressed && keyEventF3.Keycode == Key.F3)
        {
            var runState = RunState.Instance;
            if (runState == null || !runState.IsRunActive) return;
            int target = runState.EffectiveFightsPerRun - 1;
            runState.SetFightsCompletedForDebug(target);
            runState.SetBossFight(false);
            LevelManager.Instance.PickRandomLevel();
            GetViewport().SetInputAsHandled();
        }

        if (@event is InputEventKey keyEventF4 && keyEventF4.Pressed && keyEventF4.Keycode == Key.F4)
        {
            var runState = RunState.Instance;
            if (runState == null || !runState.IsRunActive) return;
            runState.SetFightsCompletedForDebug(runState.EffectiveFightsPerRun);
            runState.SetBossFight(true);
            LevelManager.Instance.PickRandomLevel();
            GetViewport().SetInputAsHandled();
        }

        if (@event is InputEventKey keyEventF5 && keyEventF5.Pressed && keyEventF5.Keycode == Key.F5)
        {
            var runState = RunState.Instance;
            if (runState == null) return;

            // Capture loadout before EndRun clears it
            var towerIds = new Godot.Collections.Array<string>();
            if (runState.SelectedTowerIds != null)
                foreach (var id in runState.SelectedTowerIds)
                    towerIds.Add(id);

            int gold = GameBalance.StartingGold;
            int lives = GameManager.Instance?.StartingLives ?? 20;

            // Mark as debug so EndRun does not award tokens/analytics for the abandoned run
            runState.IsDebugRun = true;

            // End current run (IsDebugRun prevents token/discovery save)
            runState.EndRun(false);

            // Start new run with same loadout
            runState.StartRun(gold, lives, towerIds, isDebugRun: true);
            LevelManager.Instance.PickRandomLevel();
            GetViewport().SetInputAsHandled();
        }

        if (@event is InputEventKey keyEvent1 && keyEvent1.Pressed && keyEvent1.Keycode == Key.Key1)
        {
            SlotManager.Instance?.SetForcedOutcomeForDebug(SlotOutcome.Fight);
            GD.Print("[DevTools] Forced next slot: Fight");
            GetViewport().SetInputAsHandled();
        }

        if (@event is InputEventKey keyEvent2 && keyEvent2.Pressed && keyEvent2.Keycode == Key.Key2)
        {
            SlotManager.Instance?.SetForcedOutcomeForDebug(SlotOutcome.Shop);
            GD.Print("[DevTools] Forced next slot: Shop");
            GetViewport().SetInputAsHandled();
        }

        if (@event is InputEventKey keyEvent3 && keyEvent3.Pressed && keyEvent3.Keycode == Key.Key3)
        {
            SlotManager.Instance?.SetForcedOutcomeForDebug(SlotOutcome.Heal);
            GD.Print("[DevTools] Forced next slot: Heal");
            GetViewport().SetInputAsHandled();
        }

        if (@event is InputEventKey keyEvent4 && keyEvent4.Pressed && keyEvent4.Keycode == Key.Key4)
        {
            SlotManager.Instance?.SetForcedOutcomeForDebug(SlotOutcome.Miniboss);
            GD.Print("[DevTools] Forced next slot: Miniboss");
            GetViewport().SetInputAsHandled();
        }

        if (@event is InputEventKey keyEvent5 && keyEvent5.Pressed && keyEvent5.Keycode == Key.Key5)
        {
            SlotManager.Instance?.SetForcedOutcomeForDebug(SlotOutcome.Treasure);
            GD.Print("[DevTools] Forced next slot: Treasure");
            GetViewport().SetInputAsHandled();
        }
    }
}
