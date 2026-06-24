using Godot;

/// <summary>
/// Centralises projectile instantiation. When object pooling is added, only this class changes.
/// </summary>
public static class ProjectileFactory
{
    /// <summary>
    /// Instantiates a projectile from the given scene, sets its origin, and wires its target and
    /// damage. Returns it <b>without</b> adding it to the scene tree. The caller is responsible
    /// for AddChild.
    /// </summary>
    public static Projectile Create(PackedScene projectileScene, float damage, Enemy target, Vector2 origin)
    {
        var projectile = projectileScene.Instantiate<Projectile>();
        projectile.GlobalPosition = origin;
        projectile.Initialize(target, damage);
        return projectile;
    }
}
