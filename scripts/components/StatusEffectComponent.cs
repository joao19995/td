using Godot;
using System.Collections.Generic;

public partial class StatusEffectComponent : Node
{
    private Health _health;
    private Sprite2D _sprite;
    private MovementComponent _movement;
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
        _movement = GetParent().GetNode<MovementComponent>("MovementComponent");
    }

    public void ApplyEffect(StatusEffectData data)
    {
        if (data is PoisonEffectData poison)
        {
            ApplyPoison(poison);
            return;
        }

        if (data is SlowEffectData slow)
        {
            ApplySlow(slow);
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

    private void ApplySlow(SlowEffectData slow)
    {
        foreach (var effect in _activeEffects)
        {
            if (effect.Data is SlowEffectData)
            {
                effect.RemainingTime = slow.Duration;
                UpdateVisuals();
                return;
            }
        }

        _movement?.SetSpeedMultiplier(slow.SpeedMultiplier);
        _activeEffects.Add(new ActiveEffect
        {
            Data = slow,
            RemainingTime = slow.Duration,
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
            {
                if (effect.Data is SlowEffectData)
                    _movement?.SetSpeedMultiplier(1f);

                _activeEffects.RemoveAt(i);
            }
        }

        UpdateVisuals();
    }

    public void ClearEffects()
    {
        _movement?.SetSpeedMultiplier(1f);
        _activeEffects.Clear();
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (_sprite == null) return;

        foreach (var e in _activeEffects)
        {
            if (e.Data is SlowEffectData)
            {
                _sprite.Modulate = Colors.CornflowerBlue;
                return;
            }
        }

        foreach (var e in _activeEffects)
        {
            if (e.Data is PoisonEffectData)
            {
                _sprite.Modulate = Colors.Green;
                return;
            }
        }

        _sprite.Modulate = Colors.White;
    }
}
