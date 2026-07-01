# Roadmap

MVP feature list, in a sensible build order based on dependencies. Each item gets
detailed (tasks, design decisions) when work on it actually starts. When an item is
finished, move its outcome into GAME_STATUS.md as a behavior description.

**Global constraints:**
- 5 tower types, fixed (Corner Baker, Bike Courier, Aroma, Taste Tester, Bakery Truck)
- Each map: max 1 tower of each type = max 5 towers per map
- Loadout: max 4 tower types per run
- Towers are placed fresh each fight (no persistence of positions between fights)
- Runs cycle randomly between Map1 and Map2

---

## MVP Build Order

### 1. RunState ✅
**Status: Complete.**

What was built:
- **RunState autoload** — run-scoped storage for per-tower upgrade levels and selected tower IDs. Gold/lives live in EconomyManager/GameManager (single source of truth), not duplicated in RunState.
- **Loadout screen** — choose 1–4 tower types before each run. Only chosen towers appear in HUD during fights.
- **Run flow**: Main Menu → Loadout → random map fight → Fight Complete screen → "Next Fight" (random map, gold/lives/tower upgrades preserved) or "End Run" (back to Main Menu).
- **1-per-type enforcement** — TowerPlacementManager blocks duplicate tower types. HUD shows "(Placed)" for placed types, updated reactively on money/tower changes.
- **Random map rotation** — each fight picks randomly from LevelManager's level list (Map1 or Map2).
- **Persistent tower upgrades** — upgrade level per tower type persists across fights via RunState.
- **Enemy death tracking** — AllWavesCompleted only fires when all enemies are killed, not when the last enemy spawns.

Key decisions:
- Gold/lives live in EconomyManager/GameManager (not duplicated in RunState), surviving level reloads untouched during a run
- Tower upgrade level is per-type, not per-instance — persists across fights even if the tower is sold
- Loadout is 4 of 5 towers to force strategic choices without leaving towers unusable

### 2. SaveManager ✅
**Status: Complete.**

What was built:
- **SaveManager autoload** — persistent meta-progression storage (tokens + unlocked tower IDs).
- **JSON file** (`user://save_data.json`) via `FileAccess`, not Resource serialization (avoids documented security vector of loading untrusted `.tres` files).
- **Configurable token reward** (default 10 per run), adjustable via Inspector without code changes.
- **Corruption-safe** — load failures reset to defaults with a warning instead of crashing.
- **Migration** — existing saves auto-include new tower IDs when a game update adds towers.
- **Integration**: tokens awarded on run end, locked towers shown as "(LOCKED)" in loadout, token count displayed on Main Menu.

Key decisions:
- JSON over `.tres` for user-writable save data (security).
- Save-on-mutation is acceptable for expected frequency (end of run, meta-shop purchases).

### 3. Synergies ✅
**Status: Complete.**

What was built:
- **SynergyData resource** (`[GlobalClass]`) — defines required tower IDs, minimum tower count, which towers the bonus applies to, and percent bonuses for damage/fire rate/range.
- **SynergyManager autoload** — scans `res://resources/synergy_data/` at startup, re-evaluates active synergies whenever a tower is placed or removed, emits `SynergiesChanged` signal.
- **Reactive stat application** — each tower subscribes to `SynergiesChanged` in `_Ready` and calls `ApplyData()` on change, ensuring already-placed towers receive synergy bonuses immediately.
- **Visual feedback** — towers affected by any synergy get a green tint (`Modulate`); synergy names appear in the HUD.
- **2 example synergies**: One Whiff, One Bite (Aroma + Taste Tester → +15% damage), Grand Opening Rush (3+ types → +10% fire rate).
- **Formula**: `EffectiveX = (baseX + upgradeFlatBonus) * (1 + synergyPercentBonus)` — consistent for damage, fire rate, and range.

Key decisions:
- SynergyManager as autoload (not per-level child) — preserves zero-code-change map addition.
- Tower subscribes to signal rather than polling — reactive, no stale stats.
- Synergy `.tres` files can drop default-valued properties (editor auto-clean). C# defaults cover missing values, so functionality is unaffected.
- `CacheMode.Replace` used in ResourceLoader to avoid stale cache after .tres edits.

### 4+5. Run Engine (Slot Machine + Fight Integration) ✅

**Status: Complete.**

Merges original items 4 (Fight Integration, ~85% done from earlier work) and
5 (Slot Machine) into one.

What was built:
- **SlotManager autoload** — weighted roll (Fight 45%, Shop 25%, Heal 20%, Miniboss 10%)
  that runs after each fight. Configurable weights and fight count via Inspector.
- **Run length**: N regular fights (default 3, configurable) + 1 boss fight.
  Boss triggers automatically when `FightsCompleted >= FightsPerRun`.
- **FightCompleteScreen** now shows outcome text on first click ("Continue"),
  resolves on second click — player controls the pacing.
- **Shop outcome** — pushes a ShopScreen with run-wide stat bonuses (damage,
  fire rate). Items defined as `ShopItemData` resources.
- **Heal outcome** — restores configurable HP (default 5) without a fight.
- **Miniboss outcome** — loads a random map with 1.5x enemy stat multiplier.
- **Boss fight** — uses a dedicated boss wave (BossWave.tres) with mix of
  enemies including the Boss enemy type. Victory ends the run.
- **Shop bonuses** stack multiplicatively with upgrades and synergies in tower
  EffectiveDamage/EffectiveFireRate/EffectiveRange formulas.
- **Miniboss scaling** — Enemy accepts a statMultiplier parameter in
  Initialize(), applied to HP, reward gold, and damage.

Key decisions:
- Slot spin happens AFTER fight ends — FightCompleteScreen shows outcome,
  second click resolves it. Player always sees what's coming.
- IsBossFight/IsMiniboss flags on RunState, read by EnemySpawner via
  BaseLevel.ConfigureForRun() called from LevelManager.OnLevelLoaded.
- Shop screen is a placeholder (single category, 2 items) — expandable.
- BossWave is a separate .tres file; EnemySpawner falls back to loading it
  from `res://resources/run_data/BossWave.tres` if `BossWaveData` export is
  not set on the map scene.
### 6. Meta-Progression (Shop) ✅

**Status: Complete.**

What was built:
- **MetaUpgradeData resource** — defines a purchasable meta-progression item (ID, name, cost, max level, tower unlock or stat type with per-level bonus).
- **Meta Shop screen** — accessible from Main Menu; lists all available upgrades with current level, cost, and Buy button. Items are loaded from `.tres` files.
- **5 items in catalog**: Unlock Aroma (20t), Unlock Taste Tester (30t), Unlock Bakery Truck (50t), Secret Recipe +5%/level (15t base, 3 levels), Local Sponsorship +50/level (10t base, 3 levels).
- **Multi-level upgrades** — costs scale by level (`CostTokens * level`). Buy button disabled when maxed or insufficient tokens.
- **Tower unlocks** — purchasing a tower unlock calls `SaveManager.UnlockTower()`; Loadout screen disables locked towers.
- **Stat application** — `RunState.StartRun()` reads `SaveManager.GetMetaUpgradeLevel()` and applies `MetaDamageBonusPercent` (×0.05 per level) to `Tower.EffectiveDamage` and `StartingGoldBonus` (×50 per level) to starting gold.
- **Formula**: `EffectiveDamage = base * (1 + synergy) * (1 + shop) * (1 + meta)` — meta bonus is a separate multiplicative layer.
- **Defaults**: only Corner Baker and Bike Courier towers start unlocked (changed from all 5). Aroma/Taste Tester/Bakery Truck must be purchased.

Key decisions:
- Upgrade levels stored in SaveManager's JSON as a `meta_upgrade_levels` dictionary.
- Multi-level cost = `CostTokens * (currentLevel + 1)` — consistent scaling, no per-level config needed.
- Tower unlocks apply immediately on purchase (no restart needed).

## Post-MVP

### Slot Machine Reroll + Probability-Skewing ✅

**Status: Complete.**

What was built:
- **Reroll button** on FightCompleteScreen — after seeing the outcome (except Boss/Heal), player can pay gold to re-roll.
- **Scaling cost**: `RerollBaseCost × (rerollCount + 1)` — 50g → 100g → 150g.
- **Probability-skewing**: rerolling an outcome reduces its weight by `SkewReductionFactor` (default 50%), making repeated same outcomes less likely.
- **Configurable** via SlotManager Inspector: `RerollBaseCost`, `SkewReductionFactor`.
- Weights reset per fight session.

### Tower Equipment (Per-Tower Inventory) ✅

**Status: Complete.**

What was built:
- **EquipData resource** — id, name, description, icon, cost, target tower type, stat percent bonuses (damage/fire rate/range).
- **1 equip slot per tower** — equipment bought in the run Shop, stored per tower type in RunState.
- **Tower restriction** — each equipment item targets a specific tower type (e.g. Stone Oven → Corner Baker). Only shows in Shop if that tower is in the loadout.
- **Stat formula**: `EffectiveX = base * (1 + synergy + equipPercent) * (1 + shop) * (1 + meta) * (1 + trinket)` — equip bonus is additive with synergy.
- **3 items shipped**: Stone Oven (+15% damage, Corner Baker), Electric Bike (+20% fire rate, Bike Courier), Megaphone (+20% range, Aroma). All cost 80g.
- Equipment persists across fights within a run (like upgrades).

### Trinkets (Run-Wide Charms) ✅

**Status: Complete.**

What was built:
- **TrinketData resource** — id, name, description, damage percent bonus, heal amount, gold amount.
- **Treasure outcome** — new slot machine outcome (default 20% weight). Weights adjusted: Fight 35%, Shop 20%, Heal 15%, Miniboss 10%, Treasure 20%.
- **Choose 1 of 3** — on Treasure, a TrinketChoiceScreen shows 3 random trinkets. Player picks one.
- **Run-wide application** — trinkets affect the entire run via `RunState.TrinketDamageBonusPercent` (multiplicative in EffectiveDamage formula).
- **3 trinkets shipped**: Secret Recipe Scroll (+10% global damage), Starter's Blessing (+5 lives), Regular's Tip Jar (+100 gold).

### Deferred

- Expanded meta-progression catalog
- Real loadout curation (vs. all towers available)

---

## Post-MVP (cont.)

### Bestiary + Discovery Tracking ✅

**Status: Complete.**

What was built:
- **BestiaryScreen** — full-screen overlay accessible from Main Menu (button) and Pause screen (button). Shows 5 categories: Towers, Enemies, Equipment, Trinkets, Synergies.
- **Discovery system** — items hidden (grayed out with `???`) until first encounter:
  - Enemies: discovered when they spawn in a fight
  - Equipment: discovered when seen in the Shop
  - Trinkets: discovered when shown in Treasure choice
  - Synergies: discovered when activated (required towers placed)
  - Towers: use unlock system (meta-progression)
- **Zero-code-change** — all data scanned from directories (`tower_data/`, `enemy_data/`, etc.). Adding a new `.tres` file auto-registers it.
- **`SaveManager` persistence** — discovered state saved to JSON (`discovered` dict), persists across runs.

### Wave Decoupling (Run Mode) ✅

**Status: Complete.**

What was built:
- **Difficulty tiers** — waves organized by `resources/wave_data/tier1/tier2/tier3/`, selected by `RunState.FightsCompleted`:
  - Fight 1 → tier1 (easy: Normal, Flying)
  - Fight 2 → tier2 (medium: Tourist+Jogger, Dragon+AlleyCat)
  - Fight 3+ → tier3 (hard: AlleyCat+Jogger+Tourist, Pigeon+AlleyCat)
- **Map independence** — during runs, `LevelData.Waves` is ignored. Waves are injected into `EnemySpawner.ConfigureForRun()` at level load.
- **Random selection within tier** — `RunState.PickRunWaves()` scans the tier folder and picks one wave randomly.
- **Boss fights** unchanged — always use `BossWaveData` from `LevelManager` Inspector.
- **Zero-code-change** — adding a `.tres` to any tier folder makes it available.
- **6 waves shipped**: 2 per tier.
- Second map / more waves
