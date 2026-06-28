using Godot;

public static class ProjectileFactory
{
    public static Projectile Create(PackedScene projectileScene, float damage, Enemy target, Vector2 origin)
    {
        var fromPool = PoolManager.Instance != null;
        var projectile = fromPool
            ? PoolManager.Instance.Get<Projectile>(projectileScene)
            : projectileScene.Instantiate<Projectile>();

        projectile.GlobalPosition = origin;
        projectile.Initialize(target, damage);
        return projectile;
    }
}
