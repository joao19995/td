using Godot;

[GlobalClass]
public partial class LevelData : Resource
{
    [Export] public string LevelName { get; set; } = "Level";
    [Export] public string ScenePath { get; set; } = "";
    [Export] public Texture2D PreviewTexture { get; set; }

    // -1 means "use the global default set on the manager"
    [Export] public int StartingMoney { get; set; } = -1;
    [Export] public int StartingLives { get; set; } = -1;

    // Camera configuration
    [Export] public float CameraMinZoom { get; set; } = 1f;
    [Export] public float CameraMaxZoom { get; set; } = 4f;
    [Export] public float CameraZoomStep { get; set; } = 0.1f;
    [Export] public float CameraPanSpeed { get; set; } = 200f;

    // Symmetric boundary of ±10,000,000 units — effectively unlimited for any foreseeable map size
    [Export] public int CameraLimitLeft { get; set; } = -10_000_000;
    [Export] public int CameraLimitRight { get; set; } = 10_000_000;
    [Export] public int CameraLimitTop { get; set; } = -10_000_000;
    [Export] public int CameraLimitBottom { get; set; } = 10_000_000;

    /// <summary>Where the camera centers when the level is first loaded.</summary>
    [Export] public Vector2 CameraInitialPosition { get; set; } = Vector2.Zero;
}
