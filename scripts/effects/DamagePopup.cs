using Godot;

public partial class DamagePopup : Node2D
{
    public void ShowDamage(float amount, Color color)
    {
        var label = new Label();
        label.Text = Mathf.RoundToInt(amount).ToString();
        label.Modulate = color;
        label.LabelSettings = new LabelSettings { FontSize = 4 };
        AddChild(label);

        var tween = CreateTween();
        tween.TweenProperty(this, "position", Vector2.Up * 14f, 1.2f).SetTrans(Tween.TransitionType.Quad);
        tween.Parallel().TweenProperty(label, "modulate", new Color(color.R, color.G, color.B, 0f), 1.2f);
        tween.TweenCallback(Callable.From(QueueFree));
    }
}
