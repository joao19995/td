using Godot;
using System; // IMPORTANTE: Adiciona isto para poder usar Action

public partial class Projectile : Area2D
{
    [Export] public float Speed = 300f;
    [Export] public float Damage = 5f;

    protected Enemy Target;
    
    // Nova propriedade para guardar o efeito injetado pela torre
    public Action<Enemy, Vector2> OnHitEffect { get; set; }

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }

    public void Initialize(Enemy target, float damage)
    {
        Target = target;
        Damage = damage;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Target == null || !IsInstanceValid(Target))
        {
            QueueFree();
            return;
        }

        Vector2 direction = (Target.GlobalPosition - GlobalPosition).Normalized();
        GlobalPosition += direction * Speed * (float)delta;
        Rotation = direction.Angle();
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area == Target)
        {
            OnHitTarget(Target);
        }
    }

    protected virtual void OnHitTarget(Enemy mainEnemy)
    {
        // 1. Dá o dano direto padrão no alvo principal
        mainEnemy.TakeDamage(Damage);

        // 2. Se a torre tiver injetado um efeito extra (como o Splash), executa-o aqui
        OnHitEffect?.Invoke(mainEnemy, GlobalPosition);

        // 3. Destrói o projétil
        QueueFree();
    }
}