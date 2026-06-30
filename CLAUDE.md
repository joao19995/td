# CLAUDE.md — Project Guide for AI Assistant

## Project Overview

Godot 4.7 (.NET 8) tower defense, expanding into a roguelite hybrid
(Darkest Dungeon / Slay the Spire inspired run structure). 320×190 pixel
resolution, GL Compatibility renderer, integer scaling. C# only — no GDScript.

**State: Camada 0 complete (full TD game loop + upgrades + status effects).
Next layer: roguelite run structure (RunState → SaveManager → Fight Integration →
Slot Machine → Shop → Meta-progression), per ROADMAP.md.**

---

## Architecture — The Non-Negotiables

### Data-Driven, Not Hardcoded
All gameplay values come from `.tres` Resources. If you're typing a number that affects
gameplay, you're doing it wrong. EnemyData, TowerData, UpgradeData, WaveData,
LevelData — every stat belongs in a Resource file. The `.tres` files under
`resources/` are the single source of truth.

### Composition Over Inheritance
No `FastEnemy : Enemy`, `BossEnemy : Enemy` garbage. Enemy has HealthComponent,
MovementComponent, StatusEffectComponent as children. Tower has TargetingComponent,
AttackComponent. Behavior emerges from composition, not inheritance chains.
`BaseLevel` is the one accepted abstract base (zero gameplay logic in `Map1`/
`Map2`), required for zero-code-change map addition.

### Concrete-Necessity Test (Apply Before Any Refactor)
Before proposing a change, identify the actual bug, duplication, or scaling
wall it fixes. "Cleaner in the abstract" is not a justification.

### Autoloads Are Infrastructure, Not Gameplay
Autoloads (singletons) exist for: EventBus, LevelManager, SceneManager, UIManager,
PoolManager, CameraManager, EconomyManager, GameManager, TowerSelectionManager,
TowerPlacementManager. They coordinate systems. They do NOT contain gameplay logic.
If you're tempted to put game logic in an autoload, stop — put it in a component
on the relevant entity.

### EventBus for Decoupling, Not Everything
EventBus is for global communication (enemy died → gold, reached end → damage).
Don't use it for every local interaction. Direct method calls or signals between
parent/child are fine for tight coupling within an entity.

### Pool Everything That Spawns Often
Projectiles, enemies, effects — all go through PoolManager. Factories
(ProjectileFactory, EnemyFactory) handle pool transparently with Instantiate
fallback. No direct `Instantiate`/`QueueFree` for high-frequency objects.

### Direct Singleton Access Is the Pattern
Autoloads expose a static `Instance` property (set in `_EnterTree`). This is the
established pattern — don't propose DI wrappers without a concrete
reuse/testability problem that actually exists.

---

## Code Standards — Short Version

- **Public** methods/properties: PascalCase (`TakeDamage`, `EffectiveRange`)
- **Private** fields: `_camelCase` with underscore prefix
- **No direct struct mutation**: `Position = new Vector2(x, y)` not `Position.X += 1`
- **Resources use `[GlobalClass]`**, never `class_name`
- **Class name = file name**, public partial class always
- **No hardcoded gameplay values** — read from Resources
- **No deep node paths** — use `[Export] NodePath` with conventional defaults
- **No duplicate code** — abstract, generalize, componentize
- **Sub-resources in `.tscn` are shared by reference** — call `.Duplicate()`
  before per-instance mutation (or you'll modify the source)
- **Spawn under the active level container** (`BaseLevel.TowersContainer/
  EnemiesContainer/ProjectilesContainer`), never `GetTree().CurrentScene`

---

## Project Layout

```
scripts/
├── autoload/        # Singletons (infrastructure only)
├── components/      # Reusable behaviors (Health, Movement, Attack, StatusEffect, Targeting)
├── effects/         # Visual one-shots (SplashEffect, RangeIndicator, HealthBar, DamagePopup)
├── enemies/         # Enemy entity
├── towers/          # Tower entity
├── projectiles/     # Projectile entity
├── factories/       # ProjectileFactory, EnemyFactory
├── resources/       # Data classes (TowerData, EnemyData, WaveData, UpgradeData, etc.)
├── levels/          # BaseLevel, Map1, Map2
├── ui/              # HUD, screens
├── main/            # Main entry point
├── systems/         # PoolManager (only system, others in autoload/)
scenes/
├── levels/          # Map1.tscn, Map2.tscn
├── towers/          # Tower.tscn
├── enemies/         # Enemy.tscn
├── projectiles/     # projectile_*.tscn
├── ui/              # HUD.tscn, screens/
├── system/          # Autoload scenes
resources/
├── tower_data/      # Base.tres, Fast.tres, Ice.tres, Poison.tres, Splash.tres
├── enemy_data/      # Normal, Fast, Tank, Flying, Boss
├── wave_data/       # Wave1-4
├── upgrade_data/    # 10 upgrade files (2 per tower)
├── level_data/      # level1.tres, level2.tres
├── ui_screens/      # Pause, GameOver, Victory
```

---

## Current State

See `GAME_STATUS.md` for detailed feature descriptions. See `ROADMAP.md` for MVP plan.

### Works
- 5 tower types with unique behaviors (Base, Fast, Ice, Poison, Splash)
- 5 enemy types with configurable stats
- 2 maps with wave-based spawning
- Tower placement, selection, upgrade (2 tiers each), sell
- Object pooling for projectiles and enemies
- Status effects (Poison DoT, Slow) with visual tinting
- Full game loop: MainMenu → level → next level / game over / victory
- Health bars, damage popups, splash ring effect, range indicator
- Pause, Game Over, Victory overlays via UIManager

### Known Limitations
- No sound system (directories exist, no assets)
- Flying enemies have no gameplay distinction (follow ground path)
- All C# in global namespace (no `namespace` declarations)
- UI positions partially hardcoded in HUD.tscn
- Splash collision mask hardcoded to layer 1
- `CameraManager.WorldSize` default is set on the autoload, not derived from
  `LevelData` until `Configure()` is called with an override (fine today,
  revisit if per-map sizes diverge)

---

## Key Design Decisions (Read Before Coding)

**Upgrades never mutate shared TowerData.** Effective stats are computed at runtime
in Tower properties (`EffectiveDamage`, `EffectiveFireRate`, `EffectiveRange`) and
applied via `SetEffectiveStats`.

**Pool key is scene ResourcePath.** Nodes store `_pool_key` metadata for correct
return queue. PoolManager stores pools by scene path.

**Returns are always deferred** via `CallDeferred` to avoid physics callback
violations. `_returningToPool` flag prevents duplicate calls.

**Multi-hit protection** has 3 layers: `Monitoring = false` (deferred) on hit,
`_returningToPool` flag, `Target.IsDead` check in `_PhysicsProcess`.

**Slow and Poison stack only by refreshing duration.** No intensity stacking.
Slow uses fixed `SpeedMultiplier` value.

**LevelManager only swaps `_levelContainer` when a non-null container is
passed** — required so Retry/MainMenu transitions don't break.

---

## What To Ask Before Implementing

1. Does this belong in a Resource file (data-driven)?
2. Can I compose this from existing components instead of writing new ones?
3. Does this need to be pooled?
4. Is this global enough for EventBus or local enough for a direct signal?
5. Is there an actual problem this solves, or is it cleaner only in the abstract?
6. Will this still make sense once the roguelite run layer sits on top of it?
7. Does this add coupling I'll regret in 6 months?
