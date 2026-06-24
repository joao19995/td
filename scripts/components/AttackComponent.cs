using Godot;

/// <summary>
/// Manages attack cooldown, projectile creation, and firing logic.
/// Call TryAttack(target) every frame; the component handles its own rate limiting.
/// </summary>
public partial class AttackComponent : Node
{
    private PackedScene _projectileScene;
    private float _damage;
    private float _attackSpeed; // attacks per second
    private float _cooldown;

    public void Setup(PackedScene projectileScene, float damage, float attackSpeed)
    {
        _projectileScene = projectileScene;
        _damage = damage;
        _attackSpeed = attackSpeed;
        _cooldown = 0f;
    }

    public override void _Process(double delta)
    {
        if (_cooldown > 0f)
            _cooldown -= (float)delta;
    }

    /// <summary>
    /// Attempts to fire at the given target. Returns true if a shot was fired.
    /// Does nothing if still on cooldown or target is null.
    /// </summary>
    public bool TryAttack(Enemy target)
    {
        if (target == null || _cooldown > 0f) return false;

        Fire(target);
        _cooldown = _attackSpeed > 0f ? 1f / _attackSpeed : float.MaxValue;
        return true;
    }

    private void Fire(Enemy target)
    {
        if (_projectileScene == null)
        {
            GD.PrintErr("AttackComponent: ProjectileScene not set. Call Setup() first.");
            return;
        }

        var projectile = _projectileScene.Instantiate<Projectile>();
        projectile.GlobalPosition = GetParent<Node2D>().GlobalPosition;
        projectile.Initialize(target, _damage);
        GetTree().CurrentScene.CallDeferred(Node.MethodName.AddChild, projectile);
    }
}
