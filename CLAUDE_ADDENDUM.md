# CLAUDE_ADDENDUM.md — Right vs Wrong Patterns

Purpose of this file: rules like "No deep node paths" in CLAUDE.md are
understood in the abstract but ignored in practice because there is no literal
example alongside. Each rule below has an example taken from the project's own
code (not invented) to eliminate ambiguity.

---

## R6 — No Deep Node Paths

**Wrong** (looks up a sibling component by node name, manually, outside the owner):

```csharp
// Inside TargetingComponent, looking for progress of another node
var movement = enemy.GetNodeOrNull<MovementComponent>("MovementComponent");
float progress = movement != null ? movement.GetProgressRatio() : 0f;
```

**Right** (the owner exposes a public method; callers use it):

```csharp
// Enemy.cs exposes what other systems need
public float GetCurrentHealth() => _health?.GetCurrentHealth() ?? 0f;

// TargetingComponent.cs uses the public API, never knows the internal path
float health = enemy.GetCurrentHealth();
```

Note: `TargetingComponent.GetFurthestEnemy()` in the current code DOES
`enemy.GetNodeOrNull<MovementComponent>("MovementComponent")` — this is an
existing violation of R6, not an example to copy. If you touch that method,
fix it to `enemy.GetProgressRatio()` (add the method to `Enemy.cs` if it does
not exist yet).

---

## R1 — Data-Driven, Not Hardcoded

**Wrong**:

```csharp
if (target.HealthPercent <= 0.2f)
    damage *= 2.0f;
```

**Right**:

```csharp
if (target.HealthPercent <= _data.ExecuteThresholdHPPercent)
    damage *= _data.ExecuteMultiplier;
```

Quick test: if you delete the `.tres`, does the number still appear in any
`.cs`? If yes, it is wrong.

---

## R2 — Composition Over Inheritance

**Wrong**:

```csharp
public partial class BossEnemy : Enemy { /* overrides */ }
```

**Right**: new `EnemyData.tres` with `IsBoss = true`, same generic `Enemy.tscn`.
Different behavior comes from flags in the Resource + components, never from a
new subclass.

---

## R3 — Zero Copy-Paste

**Wrong**:

```csharp
// TowerA.cs
private void ApplyUpgrade() {
    _data.Damage *= 1.5f;  // DUPLICATED in TowerB.cs
}

// TowerB.cs
private void ApplyUpgrade() {
    _data.Damage *= 1.5f;  // IDENTICAL
}
```

**Right** (extract to shared location):

```csharp
// AttackComponent.cs — shared, used by both TowerA and TowerB
public float GetEffectiveDamage(float baseDamage)
    => baseDamage * _upgradeMultiplier;
```

Before copying any block of logic, ask: can this live in a shared component,
helper, or base method?

---

## R4 — Zero-Code-Change Content Addition

**Wrong**: hardcoded array of filenames to load.

```csharp
var files = new[] { "Wave1.tres", "Wave2.tres", "Wave3.tres" };
```

**Right** (as `SynergyManager.LoadSynergyDefinitions()` and
`ResourceLoaderHelper.LoadFromDir<T>()` already do):

```csharp
var dir = DirAccess.Open(SynergyDir);
foreach (var file in dir.GetFiles())
{
    if (!file.EndsWith(".tres") && !file.EndsWith(".res")) continue;
    // load
}
```

Adding content must never require C# changes. Content is discovered, not
registered.

---

## R5 — Pool High-Frequency Objects

**Wrong**:

```csharp
var projectile = projectileScene.Instantiate<Projectile>();
// ...
projectile.QueueFree();
```

**Right**:

```csharp
var projectile = ProjectileFactory.Create(projectileScene, position, target);
// Later, after hit/miss:
ReturnToPool();
```

Factories (ProjectileFactory, EnemyFactory) handle pool transparently with
Instantiate fallback. No direct `Instantiate`/`QueueFree` for high-frequency
objects (projectiles, enemies, effects).

---

## R8 — Never Mutate Shared Resources

**Wrong** (mutates the shared Resource by reference — affects ALL enemies using
the same `EnemyData`):

```csharp
enemyData.MaxHealth *= 1.5f; // NEVER
```

**Right** (as `AttackComponent.RebuildEffects()` already does — creates a new
instance per invocation):

```csharp
effects.Add((mainEnemy, _) => ApplyEffect(mainEnemy, new PoisonEffectData
{
    Duration = _data.PoisonDuration * _poisonDurationMultiplier,
    DamagePerTick = _data.PoisonDamagePerTick * strengthMult,
}));
```

For `Shape`/`CircleShape2D` shared in `.tscn`, call `.Duplicate()` before
mutating (see `Tower.ApplyData`).

---

## R7 — Autoloads Are Infrastructure, Not Gameplay

**Wrong**: adding `public int TowerDamage` or damage logic to an autoload like
`GameManager`.

**Right**: gameplay state lives in an entity component (`Health`,
`AttackComponent`, etc.). Autoloads only coordinate (`EventBus`, `SceneManager`,
`PoolManager`).

---

## R9 — EventBus for Global Communication Only

**Wrong** (local interaction via EventBus):

```csharp
// Tower signalling its own attack component via global EventBus
EventBus.Instance.EmitSignal(nameof(EventBus.TowerAttackStarted), this, target);
```

**Right** (direct call/signal for tight coupling within an entity):

```csharp
// Inside Tower._Ready()
_attackComponent.Connect(AttackComponent.SignalName.AttackStarted,
    Callable.From(() => OnAttackStarted()));
```

EventBus is for truly global events: enemy died → gold, wave ended →
transition. Local interactions should use direct signals or method calls.

---

## R10 — Spawn Under Active Level Containers

**Wrong**:

```csharp
GetTree().CurrentScene.AddChild(spawnedEnemy);
```

**Right**:

```csharp
var level = LevelManager.Instance.CurrentLevelNode;
level.EnemiesContainer.AddChild(spawnedEnemy);
```

Always use `BaseLevel.TowersContainer`, `BaseLevel.EnemiesContainer`, or
`BaseLevel.ProjectilesContainer`. Never `GetTree().CurrentScene`.

---

## Self-Check Checklist Before Marking Code as Done

1. Did I use `GetNode<` / `GetNodeOrNull<` outside the `_Ready()` of the node
   that owns the target? If yes, it should be a public method on the owner.
2. Did I write any `float`/`int` directly in a `.cs` that affects gameplay
   (damage, HP, cost, duration, multiplier)? If yes, it must come from an
   `[Export]` in a Resource.
3. Did I create a new subclass of `Enemy`/`Tower` just to vary behavior?
   If yes, it should be a new `.tres` instead.
4. Did I directly mutate a field of an injected Resource (`TowerData`,
   `EnemyData`, `StatusEffectData`, `Shape`) without `.Duplicate()` or creating
   a new instance? If yes, fix before continuing.
5. Would this file/logic work with 20 towers and 30 enemies without touching
   C# code? If not, it is not data-driven.
