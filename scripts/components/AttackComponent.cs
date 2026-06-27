using Godot;
using System;

/// <summary>
/// Manages attack cooldown, projectile creation, and firing logic.
/// Call TryAttack(target) every frame; the component handles its own rate limiting.
/// Parent must be a Node2D.
/// </summary>
public partial class AttackComponent : Node
{
    private PackedScene _projectileScene;
    private float _damage;
    private float _attackSpeed; // attacks per second
    private float _cooldown;
    
    // Novas variáveis para o sistema de Splash
    private bool _hasSplash;
    private float _splashRadius;
        
    // Novas variáveis para o sistema de poison

    private bool _hasPoison;
    private float _poisonDamagePerTick;
    private float _poisonDuration;
    public override void _Ready()
    {
        if (GetParent() is not Node2D)
            GD.PushError($"AttackComponent: parent must be a Node2D (got {GetParent()?.GetClass()}).");
    }

    /// <param name="attackSpeed">Attacks per second. Must be greater than zero.</param>
    public void Setup(PackedScene projectileScene, float damage, float attackSpeed, bool hasSplash, float splashRadius, bool hasPoison, float poisonDamagePerTick, float poisonDuration)
    {
        if (attackSpeed <= 0f)
        {
            GD.PushError($"AttackComponent: attackSpeed must be > 0 (got {attackSpeed}). Defaulting to 1.");
            attackSpeed = 1f;
        }

        _projectileScene = projectileScene;
        _damage = damage;
        _attackSpeed = attackSpeed;
        _hasSplash = hasSplash;
        _splashRadius = splashRadius;
        _hasPoison = hasPoison;
        _poisonDamagePerTick = poisonDamagePerTick;
        _poisonDuration = poisonDuration;
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
        _cooldown = 1f / _attackSpeed;
        return true;
    }

private void Fire(Enemy target)
{
    if (_projectileScene == null)
    {
        GD.PrintErr("AttackComponent: ProjectileScene not set. Call Setup() first.");
        return;
    }

    // 1. Cria o projétil usando a tua Factory exatamente como já tinhas feito
    var projectile = ProjectileFactory.Create(_projectileScene, _damage, target, GetParent<Node2D>().GlobalPosition);

    // 2. Se esta torre tiver Splash, injetamos a Action diretamente no projétil criado
    if (_hasSplash)
    {
        projectile.OnHitEffect = (mainEnemy, hitPosition) => TriggerSplashDamage(mainEnemy, hitPosition);
    }
    else if (_hasPoison)
    {
        // Quando o projétil bater, ele diz ao inimigo para começar a envenenar
        projectile.OnHitEffect = (mainEnemy, hitPosition) => 
        {
            mainEnemy.ApplyPoison(_poisonDamagePerTick, _poisonDuration);
        };
    }

    // 3. Adiciona o projétil à cena através do teu LevelManager
    LevelManager.Instance.CurrentLevelNode.CallDeferred(Node.MethodName.AddChild, projectile);
}

    private void TriggerSplashDamage(Enemy mainEnemy, Vector2 hitPosition)
    {
        var spaceState = GetParent<Node2D>().GetWorld2D().DirectSpaceState;        
        var query = new PhysicsShapeQueryParameters2D();
        var circle = new CircleShape2D { Radius = _splashRadius };
        
        query.Shape = circle;
        query.Transform = new Transform2D(0, hitPosition);
        query.CollideWithAreas = true;
        query.CollideWithBodies = false;
        query.CollisionMask = 1; // Ajusta para a Collision Layer dos teus inimigos

        var results = spaceState.IntersectShape(query);

        foreach (var result in results)
        {
            if (result["collider"].AsGodotObject() is Enemy surroundingEnemy)
            {
                if (surroundingEnemy == mainEnemy) continue; // Evita dar dano duplo ao alvo principal
                surroundingEnemy.TakeDamage(_damage);
            }
        }
    }
}