using Godot;
using System.Collections.Generic;

public partial class StatusEffectComponent : Node
{
    private Health _health;
    private Sprite2D _sprite;
    private readonly List<ActiveEffect> _activeEffects = new();

    private class ActiveEffect
    {
        public StatusEffectData Data;
        public float RemainingTime;
        public float TickCooldown;
    }

    public override void _Ready()
    {
        _health = GetParent().GetNode<Health>("Health");
        _sprite = GetParent().GetNode<Sprite2D>("Sprite2D");
    }

    public void ApplyEffect(StatusEffectData data)
    {
        if (data is PoisonEffectData poison)
        {
            ApplyPoison(poison);
            return;
        }
    }

    private void ApplyPoison(PoisonEffectData poison)
    {
        foreach (var effect in _activeEffects)
        {
            if (effect.Data is PoisonEffectData)
            {
                effect.RemainingTime = poison.Duration;
                effect.Data = poison;
                UpdateVisuals();
                return;
            }
        }

        _activeEffects.Add(new ActiveEffect
        {
            Data = poison,
            RemainingTime = poison.Duration,
            TickCooldown = poison.TickInterval
        });
        UpdateVisuals();
    }

    public override void _Process(double delta)
    {
        if (_activeEffects.Count == 0) return;

        for (int i = _activeEffects.Count - 1; i >= 0; i--)
        {
            var effect = _activeEffects[i];
            effect.RemainingTime -= (float)delta;

            if (effect.Data is PoisonEffectData poison)
            {
                effect.TickCooldown -= (float)delta;
                if (effect.TickCooldown <= 0f)
                {
                    _health?.TakeDamage(poison.DamagePerTick);
                    if (_activeEffects.Count == 0) break;
                    effect.TickCooldown = poison.TickInterval;
                }
            }

            if (effect.RemainingTime <= 0f && i < _activeEffects.Count)
                _activeEffects.RemoveAt(i);
        }

        UpdateVisuals();
    }

    public void ClearEffects()
    {
        _activeEffects.Clear();
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (_sprite == null) return;

        bool hasActiveEffect = false;
        foreach (var e in _activeEffects)
        {
            if (e.Data is PoisonEffectData) { hasActiveEffect = true; break; }
        }

        _sprite.Modulate = hasActiveEffect ? Colors.Green : Colors.White;
    }
}
