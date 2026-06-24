using Godot;

public partial class Projectile : Area2D
{
    [Export] public float Speed = 300f;
    [Export] public float Damage = 5f;

    private Enemy _target;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }

	public void Initialize(Enemy target, float damage)
	{
		_target = target;
		Damage = damage;
	}

    public override void _PhysicsProcess(double delta)
    {
        if (_target == null || !IsInstanceValid(_target))
        {
            QueueFree(); // alvo desapareceu (morreu/saiu) antes do impacto
            return;
        }

        var direction = (_target.GlobalPosition - GlobalPosition).Normalized();
        GlobalPosition += direction * Speed * (float)delta;

        // Rotação opcional do sprite na direção do movimento
        Rotation = direction.Angle();
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area is Enemy enemy && enemy == _target)
        {
            enemy.TakeDamage(Damage);
            QueueFree();
        }
    }
}