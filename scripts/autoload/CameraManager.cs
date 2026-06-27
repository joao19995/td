using Godot;

/// <summary>
/// Autoload singleton — the single, persistent camera for the entire game.
/// Fixed and centered on the play area; no player pan or zoom.
/// Survives level transitions (lives outside the level scene tree), so it
/// never needs to be reconfigured or recreated when SceneManager swaps levels.
/// WorldSize is exported here because, by design, every map shares the same
/// logical play-area size — see my-agent.agent.md Rule #4 (data-driven) and
/// Rule #7 (autoloads are infrastructure, not per-level config).
/// </summary>
public partial class CameraManager : Camera2D
{
    public static CameraManager Instance { get; private set; }

    /// <summary>Logical play-area size in pixels. Same for every map.</summary>
    [Export] public Vector2 WorldSize = new Vector2(320, 190);

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        MakeCurrent();
        Configure();
    }

    /// <summary>
    /// Centers the camera on the world and sets the zoom so the world exactly
    /// fills the viewport with no visible gray border, regardless of viewport size.
    /// Call again if the viewport is resized at runtime.
    /// </summary>
    public void Configure()
    {
        var viewportSize = GetViewport().GetVisibleRect().Size;

        float zoomX = viewportSize.X / WorldSize.X;
        float zoomY = viewportSize.Y / WorldSize.Y;
        float coverZoom = Mathf.Max(zoomX, zoomY);

        Zoom = new Vector2(coverZoom, coverZoom);
        Position = WorldSize / 2f;

        LimitLeft = 0;
        LimitTop = 0;
        LimitRight = (int)WorldSize.X;
        LimitBottom = (int)WorldSize.Y;
    }
}