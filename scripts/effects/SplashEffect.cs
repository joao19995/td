using Godot;

public partial class SplashEffect : Node2D
{
    private float _radius;

    public void Initialize(float radius)
    {
        _radius = radius;
        QueueRedraw();
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate", new Color(1, 1, 1, 0), 0.3f);
        tween.TweenCallback(Callable.From(QueueFree));
    }

    public override void _Draw()
    {
        DrawArc(Vector2.Zero, _radius, 0, Mathf.Tau, 32, Colors.White, 2f);
    }
}
