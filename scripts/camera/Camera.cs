using Godot;

public partial class Camera : Camera2D
{
    [Export] public float ZoomStep = 0.1f;
    [Export] public float MinZoom = 1f;
    [Export] public float MaxZoom = 4f;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            if (mouseButton.ButtonIndex == MouseButton.WheelUp)
            {
                AdjustZoom(ZoomStep);
            }
            else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
            {
                AdjustZoom(-ZoomStep);
            }
        }
    }

    private void AdjustZoom(float amount)
    {
        float newZoom = Mathf.Clamp(Zoom.X + amount, MinZoom, MaxZoom);
        Zoom = new Vector2(newZoom, newZoom);
    }
}