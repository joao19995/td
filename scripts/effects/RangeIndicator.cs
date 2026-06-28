using Godot;

public partial class RangeIndicator : Node2D
{
    private float _range;

    public void SetRange(float range)
    {
        _range = range;
        QueueRedraw();
    }

    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, _range, new Color(1, 1, 1, 0.08f));
        DrawArc(Vector2.Zero, _range, 0, Mathf.Tau, 32, new Color(1, 1, 1, 0.3f), 1f);
    }
}
