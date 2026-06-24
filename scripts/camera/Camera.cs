using Godot;

public partial class Camera : Camera2D
{
    [Export] public float ZoomStep = 0.1f;
    [Export] public float MinZoom = 1f;
    [Export] public float MaxZoom = 4f;
    [Export] public float PanSpeed = 200f;

    public override void _Ready()
    {
        MakeCurrent();
    }

    public override void _Process(double delta)
    {
        var direction = Vector2.Zero;

        if (Input.IsActionPressed("camera_pan_left"))
            direction.X -= 1f;
        if (Input.IsActionPressed("camera_pan_right"))
            direction.X += 1f;
        if (Input.IsActionPressed("camera_pan_up"))
            direction.Y -= 1f;
        if (Input.IsActionPressed("camera_pan_down"))
            direction.Y += 1f;

        if (direction != Vector2.Zero)
        {
            Position = new Vector2(
                Mathf.Clamp(Position.X + direction.Normalized().X * PanSpeed * (float)delta, LimitLeft, LimitRight),
                Mathf.Clamp(Position.Y + direction.Normalized().Y * PanSpeed * (float)delta, LimitTop, LimitBottom)
            );
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            if (mouseButton.ButtonIndex == MouseButton.WheelUp)
                AdjustZoom(ZoomStep);
            else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
                AdjustZoom(-ZoomStep);
        }
    }

    /// <summary>
    /// Applies per-level camera settings from a <see cref="LevelData"/> resource.
    /// Called by <see cref="LevelManager"/> immediately after a level is loaded.
    /// </summary>
    public void Configure(LevelData data)
    {
        MinZoom = data.CameraMinZoom;
        MaxZoom = data.CameraMaxZoom;
        ZoomStep = data.CameraZoomStep;
        PanSpeed = data.CameraPanSpeed;

        LimitLeft = data.CameraLimitLeft;
        LimitRight = data.CameraLimitRight;
        LimitTop = data.CameraLimitTop;
        LimitBottom = data.CameraLimitBottom;

        Position = data.CameraInitialPosition;
        Zoom = new Vector2(MinZoom, MinZoom);
    }

    private void AdjustZoom(float amount)
    {
        float newZoom = Mathf.Clamp(Zoom.X + amount, MinZoom, MaxZoom);
        Zoom = new Vector2(newZoom, newZoom);
    }
}