using Godot;

public static class EnemyFactory
{
    public static Enemy Create(PackedScene enemyScene, EnemyData data, Curve2D path)
    {
        var fromPool = PoolManager.Instance != null;
        var enemy = fromPool
            ? PoolManager.Instance.Get<Enemy>(enemyScene)
            : enemyScene.Instantiate<Enemy>();

        enemy.Initialize(data, path);
        return enemy;
    }
}
