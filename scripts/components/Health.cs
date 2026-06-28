using Godot;

public partial class Health : Node
{
    [Export] public float MaxHealth = 10f;
    [Signal] public delegate void HealthChangedEventHandler(float currentHealth, float maxHealth);
    [Signal] public delegate void DamageTakenEventHandler(float amount);
    [Signal] public delegate void DiedEventHandler();

    private float _currentHealth;

    public override void _Ready()
    {
        _currentHealth = MaxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (_currentHealth <= 0f) return; // já está morto, ignora dano extra

        _currentHealth = Mathf.Max(0f, _currentHealth - amount);
        EmitSignal(SignalName.HealthChanged, _currentHealth, MaxHealth);
        EmitSignal(SignalName.DamageTaken, amount);

        if (_currentHealth <= 0f)
        {
            EmitSignal(SignalName.Died);
        }
    }

    public void Heal(float amount)
    {
        _currentHealth = Mathf.Min(MaxHealth, _currentHealth + amount);
        EmitSignal(SignalName.HealthChanged, _currentHealth, MaxHealth);
    }

    public float GetCurrentHealth() => _currentHealth;

    public void Setup(float maxHealth)
    {
        MaxHealth = maxHealth;
        _currentHealth = maxHealth;
    }
}