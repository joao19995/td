using Godot;

public static class DamagePopupFactory
{
    private static PackedScene _scene;

    public static DamagePopup Create(Vector2 globalPosition)
    {
        _scene ??= ResourceLoader.Load<PackedScene>("res://scenes/effects/DamagePopup.tscn");
        var popup = PoolManager.Instance != null
            ? PoolManager.Instance.Get<DamagePopup>(_scene)
            : _scene.Instantiate<DamagePopup>();
        popup.Initialize(globalPosition);
        return popup;
    }
}
