# CLAUDE.md — Project Guide for AI Assistant

## Project Overview

### Game Premise

Sourdough Siege is a humorous tower defense where you command the Order of the
Mother Dough, a brotherhood dedicated to eradicating industrial bread and
converting all followers to true slow-fermentation sourdough. Over each run,
hordes of humans, animals, and increasingly absurd creatures march down your
street, loyal to packaged sliced bread, industrial croissants, and other baking
heresies. Your mission is not to destroy them but to convert them through the
power of artisan bread.

You build a network of towers, each representing a member of the Order — from
the humble Bread Baker to the legendary High Prophet of Sourdough. Each tower
has unique mechanics, equipment, synergies, and a distinct role in your
strategy. Between waves you can buy run-wide upgrades at the shop, equip
towers with unique artifacts, pick trinkets that modify the entire run, and
combine towers to unlock powerful synergies. Permanent upgrades carry over
between runs via meta-progression.

Enemies grow stranger and tougher as waves progress: from sliced-bread tourists
and grocery joggers to bread-dragon bosses and other heresy leaders. Each boss
is a unique challenge requiring a solid tower-and-synergy composition. The goal
is to prevent industrial-bread followers from crossing your defense, converting
them one by one and restoring the true faith of the mother dough.

### Tech Stack

Godot 4.7 (.NET 8) tower defense, expanding into a roguelite hybrid
(Darkest Dungeon / Slay the Spire inspired run structure). 320x190 pixel
resolution, GL Compatibility renderer, integer scaling. C# only — no GDScript.

**State: Camada 0 + roguelite run layer complete (run engine, loadout, shop,
slot machine, meta-progression, equipment, trinkets, briefing, synergies,
bestiary, wave decoupling).
Next: expanded content (more maps, enemies, meta items).**

---

## Architecture — The Non-Negotiables

These 10 rules are the single source of truth for all code in this project.
`CLAUDE_ADDENDUM.md` contains concrete right/wrong examples for each rule.

### R1 — Data-Driven, Not Hardcoded

All gameplay values come from `.tres` Resources. If you are typing a number
that affects gameplay, you are doing it wrong. EnemyData, TowerData,
UpgradeData, WaveData, LevelData — every stat belongs in a Resource file.
The `.tres` files under `resources/` are the single source of truth.

Quick test: if you delete the `.tres`, does the number still appear in any
`.cs`? If yes, it is a violation.

### R2 — Composition Over Inheritance

No `FastEnemy : Enemy`, `BossEnemy : Enemy`. Enemy has HealthComponent,
MovementComponent, StatusEffectComponent as children. Tower has
TargetingComponent, AttackComponent. Behavior emerges from composition,
not inheritance chains.

`BaseLevel` is the one accepted abstract base (zero gameplay logic in
`Map1`/`Map2`), required for zero-code-change map addition.

### R3 — Zero Copy-Paste

No duplicate code. Abstract, generalize, componentize. If you find yourself
copying a block of logic, extract it into a shared method, component, or
helper before committing.

### R4 — Zero-Code-Change Content Addition

Adding a new tower, enemy, equip, trinket, synergy, wave, or meta-upgrade
must never require C# code changes. Use directory scans (e.g.
`ResourceLoaderHelper.LoadFromDir<T>()`) instead of hardcoded arrays of
filenames. Content is discovered, not registered.

### R5 — Pool High-Frequency Objects

Projectiles, enemies, effects — all go through PoolManager. Factories
(ProjectileFactory, EnemyFactory) handle pool transparently with Instantiate
fallback. No direct `Instantiate`/`QueueFree` for high-frequency objects.

Pool key is scene ResourcePath. Nodes store `_pool_key` metadata for correct
return queue. Returns are always deferred via `CallDeferred` to avoid physics
callback violations. `_returningToPool` flag prevents duplicate calls.

### R6 — No Deep Node Paths

`GetNode<T>()` and `GetNodeOrNull<T>()` with a string literal path are only
allowed in the `_Ready()` of the node that owns the target child. Anywhere
else, the owner must expose a public method, and callers must use that
public API — they must never know the internal node hierarchy.

### R7 — Autoloads Are Infrastructure, Not Gameplay

Autoloads (singletons) exist for: EventBus, LevelManager, SceneManager,
UIManager, PoolManager, CameraManager, EconomyManager, GameManager,
TowerSelectionManager, TowerPlacementManager, GameBalance, SaveManager.
They coordinate systems. They do NOT contain gameplay logic.

If you are tempted to put game logic in an autoload, stop — put it in a
component on the relevant entity.

### R8 — Never Mutate Shared Resources

Resources loaded from `.tres` or shared in `.tscn` are shared by reference
across all instances. Never mutate them directly. Always call `.Duplicate()`
before per-instance mutation, or create a new instance (e.g.
`new PoisonEffectData { ... }`).

Upgrades never mutate shared TowerData. Effective stats are computed at
runtime in Tower properties (`EffectiveDamage`, `EffectiveFireRate`,
`EffectiveRange`) and applied via `SetEffectiveStats`.

### R9 — EventBus for Global Communication Only

EventBus is for global communication (enemy died → gold, reached end →
damage). Do not use it for every local interaction. Direct method calls or
signals between parent/child are fine for tight coupling within an entity.

### R10 — Spawn Under Active Level Containers

All runtime instances (towers, enemies, projectiles, effects) must be added
to the active level container (`BaseLevel.TowersContainer`,
`BaseLevel.EnemiesContainer`, `BaseLevel.ProjectilesContainer`). Never use
`GetTree().CurrentScene` for instantiation.

---

## Design Principles (Not Numbered Rules, Still Mandatory)

### Concrete-Necessity Test

Before proposing any change, identify the actual bug, duplication, or scaling
wall it fixes. "Cleaner in the abstract" is not a justification.

### Direct Singleton Access

Autoloads expose a static `Instance` property (set in `_EnterTree`). This is
the established pattern — do not propose DI wrappers without a concrete
reuse/testability problem that actually exists.

---

## Code Standards — Short Version

- **Public** methods/properties: PascalCase (`TakeDamage`, `EffectiveRange`)
- **Private** fields: `_camelCase` with underscore prefix
- **No direct struct mutation**: `Position = new Vector2(x, y)` not `Position.X += 1`
- **Resources use `[GlobalClass]`**, never `class_name`
- **Class name = file name**, public partial class always
- **Multi-hit protection** has 3 layers: `Monitoring = false` (deferred) on hit,
  `_returningToPool` flag, `Target.IsDead` check in `_PhysicsProcess`
- **Slow and Poison stack only by refreshing duration.** No intensity stacking.
  Slow uses fixed `SpeedMultiplier` value
- **LevelManager only swaps `_levelContainer` when a non-null container is
  passed** — required so Retry/MainMenu transitions do not break

---

## Project Layout

```
scripts/
├── autoload/        # Singletons (infrastructure only)
├── components/      # Reusable behaviors (Health, Movement, Attack, StatusEffectComponent, Targeting, Aura, GlobalAura)
├── effects/         # Visual one-shots (SplashEffect, RangeIndicator, HealthBar, DamagePopup)
├── enemies/         # Enemy entity
├── towers/          # Tower entity
├── projectiles/     # Projectile entity
├── factories/       # ProjectileFactory, EnemyFactory, TowerFactory
├── helpers/         # Utility helpers (ResourceLoaderHelper)
├── resources/       # Data classes (TowerData, EnemyData, WaveData, WaveEntry, UpgradeData, GameBalanceData, PoisonEffectData, SlowEffectData, StatusEffectData, etc.)
├── levels/          # BaseLevel, Map1, Map2
├── ui/              # HUD, screens
│   ├── screens/     # MainMenu, Loadout, FightComplete
│   ├── shop/        # ShopScreen
│   ├── meta_shop/   # MetaShopScreen
│   ├── briefing/    # BriefingScreen
│   ├── trinket/     # TrinketChoiceScreen
│   └── bestiary/    # BestiaryScreen
├── main/            # Main entry point
├── Spawner/         # EnemySpawner
├── systems/         # PoolManager, SynergyManager, SlotManager, SlotOutcome, WaveGenerator
├── autoload/        # Singletons (infrastructure only)
scenes/
├── levels/          # Map1.tscn, Map2.tscn
├── towers/          # Tower.tscn
├── enemies/         # Enemy.tscn
├── projectiles/     # projectile_*.tscn
├── ui/              # HUD.tscn, screens/
├── system/          # Autoload scenes
resources/
├── tower_data/      # BreadBaker, BreadCourier, AromaKeeper, TasteTester, BakeryTruck, BreadMonk, FermentationSage, CrustCrusader, DoughExorcist, HighProphet
├── enemy_data/      # SlicedBreadTourist, GroceryJogger, AlleyCat, BaguettePigeon, BreadDragon, MicrowaveMealPreacher, PlasticWrappedSandwichMan, GlutenNullBishop, FrozenDoughAbomination, SupermarketOverlord
├── wave_data/       # Wave1-4 + tier1/ tier2/ tier3/ (tiered for run mode)
├── upgrade_data/    # 10 upgrade files (2 per tower)
├── level_data/      # level1.tres, level2.tres
├── ui_screens/      # Pause, GameOver, Victory
├── synergy_data/    # Synergy .tres files
├── equip_data/      # Equipment .tres files
├── trinket_data/    # Trinket .tres files
├── meta_upgrade_data/ # Meta-upgrade .tres files
├── run_data/        # Shop items, BossWave
```

---

## Current State

See `docs/status/GAME_STATUS.md` for detailed feature descriptions.
See `ROADMAP.md` for MVP plan.

### Works
- 10 tower types with unique behaviors (Bread Baker, Bread Courier, Aroma Keeper, Taste Tester, Bakery Truck, Bread Monk, Fermentation Sage, Crust Crusader, Dough Exorcist, High Prophet of Sourdough)
- 10 enemy types with configurable stats
- 2 maps with wave-based spawning
- Tower placement, selection, upgrade (2 tiers each)
- Object pooling for projectiles and enemies
- Status effects (Poison DoT, Slow) with visual tinting
- Full game loop: MainMenu → level → next level / game over / victory
- Health bars, damage popups, splash ring effect, range indicator
- Pause, Game Over, Victory overlays via UIManager
- Roguelite run engine (RunState, SaveManager, SlotManager, SynergyManager)
- Loadout screen (1–4 towers per run, locked towers shown accordingly)
- Slot machine post-fight outcomes (Fight, Shop, Heal, Miniboss, Treasure, Boss)
- Reroll mechanic with cost scaling and outcome skew reduction
- Shop with run-wide bonuses and tower-specific equipment
- Meta-progression shop (16 items, token-based permanent unlocks/upgrades)
- Trinkets (Treasure outcome: choose 1 of 3)
- Pre-fight briefing screen showing wave composition
- Bestiary (accessible from Main Menu and Pause screen) with discovery tracking — locked towers/enemies/equip/trinkets/synergies hidden until encountered
- Wave decoupling — waves organized by difficulty tier (tier1/tier2/tier3), selected by `FightsCompleted`, independent of map layout

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

## What To Ask Before Implementing

1. Does this belong in a Resource file (data-driven)?
2. Can I compose this from existing components instead of writing new ones?
3. Does this need to be pooled?
4. Is this global enough for EventBus or local enough for a direct signal?
5. Is there an actual problem this solves, or is it cleaner only in the abstract?
6. Will this still make sense once the roguelite run layer sits on top of it?
7. Does this add coupling I will regret in 6 months?
