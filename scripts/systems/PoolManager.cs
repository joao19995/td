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
            return (T)node;
        }

        var instance = scene.Instantiate<T>();
        instance.SetMeta("_pool_key", key);
        return instance;
    }

    public void Return(Node node)
    {
        if (!IsInstanceValid(node))
        {
            return;
        }

        if (!node.HasMeta("_pool_key"))
        {
            node.QueueFree();
            return;
        }

        var key = node.GetMeta("_pool_key").AsString();

        ResetNode(node);

        if (node.GetParent() != null)
            node.GetParent().RemoveChild(node);

        if (node.GetParent() != null)
        {
            node.QueueFree();
            return;
        }

        AddChild(node);

        if (!_pools.ContainsKey(key))
            _pools[key] = new Queue<Node>();

        _pools[key].Enqueue(node);
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
            enemy.ResetForPool();
        }
    }
}
