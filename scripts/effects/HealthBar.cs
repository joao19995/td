using Godot;

public partial class HealthBar : Node2D
{
    private float _current = 1f;
    private float _max = 1f;

    public void SetHealth(float current, float max)
    {
        _current = current;
        _max = max;
        QueueRedraw();
    }

    public override void _Draw()
    {
        float barWidth = 16f;
        float barHeight = 2f;
        float offsetX = -barWidth / 2f;
        float offsetY = -12f;

        float ratio = _max > 0 ? Mathf.Clamp(_current / _max, 0f, 1f) : 0f;

        DrawRect(new Rect2(offsetX, offsetY, barWidth, barHeight), new Color(0.3f, 0f, 0f));
        DrawRect(new Rect2(offsetX, offsetY, barWidth * ratio, barHeight), new Color(0f, 1f, 0f));
    }

    public void Reset()
    {
        _current = 0f;
        _max = 1f;
        QueueRedraw();
    }
}
