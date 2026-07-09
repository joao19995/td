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
    }
}
