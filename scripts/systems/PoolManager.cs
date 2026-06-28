using Godot;
using System.Collections.Generic;

public partial class PoolManager : Node
{
    public static PoolManager Instance { get; private set; }

    private readonly Dictionary<string, Queue<Node>> _pools = new();

    public override void _EnterTree()
    {
        Instance = this;
    }

    public T Get<T>(PackedScene scene) where T : Node
    {
        var key = scene.ResourcePath;
        if (_pools.TryGetValue(key, out var queue) && queue.Count > 0)
        {
            var node = queue.Dequeue();
            if (node.GetParent() == this)
                RemoveChild(node);
            GameManager.Log($"[PoolManager] Reused {typeof(T).Name} from pool (size: {queue.Count})");
            return (T)node;
        }

        var instance = scene.Instantiate<T>();
        instance.SetMeta("_pool_key", key);
        GameManager.Log($"[PoolManager] Instantiated new {typeof(T).Name}");
        return instance;
    }

    public void Return(Node node)
    {
        if (!IsInstanceValid(node))
        {
            GameManager.Log($"[PoolManager] Return skipped — node invalid");
            return;
        }

        if (!node.HasMeta("_pool_key"))
        {
            GameManager.Log($"[PoolManager] Return — no _pool_key, QueueFree fallback");
            node.QueueFree();
            return;
        }

        var key = node.GetMeta("_pool_key").AsString();
        var typeName = node.GetClass();

        ResetNode(node);

        if (node.GetParent() != null)
            node.GetParent().RemoveChild(node);

        if (node.GetParent() != null)
        {
            GameManager.Log($"[PoolManager] Return — RemoveChild failed for {typeName}, QueueFree");
            node.QueueFree();
            return;
        }

        AddChild(node);

        if (!_pools.ContainsKey(key))
            _pools[key] = new Queue<Node>();

        _pools[key].Enqueue(node);
        GameManager.Log($"[PoolManager] Returned {typeName} to pool (size: {_pools[key].Count})");
    }

    private static void ResetNode(Node node)
    {
        node.SetProcess(false);
        node.SetPhysicsProcess(false);

        if (node is CanvasItem canvas)
            canvas.Visible = false;

        if (node is Projectile proj)
        {
            proj.Target = null;
            proj.OnHitEffect = null;
        }
        else if (node is Enemy enemy)
        {
            var statusEffects = enemy.GetNode<StatusEffectComponent>("StatusEffectComponent");
            statusEffects?.ClearEffects();
        }
    }
}
