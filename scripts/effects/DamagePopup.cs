using Godot;

public partial class DamagePopup : Node2D
{
    private Label _label;
    private Tween _tween;
    private bool _returningToPool;

    public override void _Ready()
    {
        _label = GetNodeOrNull<Label>("Label");
        if (_label != null)
            _label.LabelSettings = new LabelSettings { FontSize = 4 };
    }

    public void Initialize(Vector2 globalPosition)
    {
        _returningToPool = false;
        GlobalPosition = globalPosition;
        Visible = true;
        if (_label != null)
            _label.Modulate = Colors.White;
    }

    public void ShowDamage(float amount, Color color)
    {
        if (_returningToPool || _label == null) return;
        _label.Text = Mathf.RoundToInt(amount).ToString();
        _label.Modulate = color;

        if (_tween != null && _tween.IsValid()) _tween.Kill();
        _tween = CreateTween();
        _tween.TweenProperty(this, "position", Vector2.Up * 14f, 1.2f)
              .SetTrans(Tween.TransitionType.Quad);
        _tween.Parallel().TweenProperty(_label, "modulate",
            new Color(color.R, color.G, color.B, 0f), 1.2f);
        _tween.TweenCallback(Callable.From(ReturnToPool));
    }

    private void ReturnToPool()
    {
        if (_returningToPool) return;
        _returningToPool = true;
        if (_tween != null && _tween.IsValid()) _tween.Kill();
        CallDeferred(nameof(DeferredReturnToPool));
    }

    private void DeferredReturnToPool()
    {
        if (PoolManager.Instance != null && HasMeta("_pool_key"))
        {
            PoolManager.Instance.Return(this);
        }
        else
        {
            QueueFree();
        }
    }
}
