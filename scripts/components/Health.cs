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

    public DamageResult TakeDamage(in DamageContext ctx)
    {
        if (_currentHealth <= 0f)
            return new DamageResult(ctx.Amount, 0f, false);

        float hpBefore = _currentHealth;
        _currentHealth = Mathf.Max(0f, _currentHealth - ctx.Amount);
        float actualDamage = hpBefore - _currentHealth;
        bool wasKilled = _currentHealth <= 0f;

        EmitSignal(SignalName.HealthChanged, _currentHealth, MaxHealth);
        EmitSignal(SignalName.DamageTaken, ctx.Amount);

        if (wasKilled)
        {
            EmitSignal(SignalName.Died);
        }

        var result = new DamageResult(ctx.Amount, actualDamage, wasKilled);
        CombatLog.RecordDamage(ctx, result);
        return result;
    }

    /// <summary>Backward-compatible overload. Damage is anonymous (no source).</summary>
    public void TakeDamage(float amount)
    {
        TakeDamage(new DamageContext(amount, null, DamageType.Direct));
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