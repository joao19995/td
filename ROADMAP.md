# Roadmap

MVP feature list, in a sensible build order based on dependencies. Each item gets
detailed (tasks, design decisions) when work on it actually starts. When an item is
finished, move its outcome into GAME_STATUS.md as a behavior description.

**Global constraints:**
- 5 tower types, fixed (Base, Fast, Ice, Poison, Splash)
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
- **Persistent tower upgrades** — upgrade level per tower type persists across fights via RunState. Selling does not downgrade the stored level.
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
- **2 example synergies**: Frost Venom (Ice + Poison → +15% damage), Overclock (3+ types → +10% fire rate).
- **Formula**: `EffectiveX = (baseX + upgradeFlatBonus) * (1 + synergyPercentBonus)` — consistent for damage, fire rate, and range.

Key decisions:
- SynergyManager as autoload (not per-level child) — preserves zero-code-change map addition.
- Tower subscribes to signal rather than polling — reactive, no stale stats.
- Synergy `.tres` files can drop default-valued properties (editor auto-clean). C# defaults cover missing values, so functionality is unaffected.
- `CacheMode.Replace` used in ResourceLoader to avoid stale cache after .tres edits.

### 4. Fight Integration
Existing wave system runs inside a run. Each fight loads the map and lets the
player place towers (respecting 1-per-type limit). Upgrades from RunState apply
to placed towers. Lives are per-run, not per-fight. If the player loses all lives
the run ends (Game Over). If all waves are cleared the run advances to the slot
machine.

### 5. Slot Machine (run engine)
Decides the next step after each fight: shop, heal, random event, or miniboss
(harder fight with bonus reward). Weighted per outcome type. Reroll and
probability-skewing deferred — base weighted roll only for MVP.

### 6. Run Loop
Start run → loadout (all 5 towers available at base level) → repeat
[fight → slot machine → resolve step] → win/lose → meta-progression reward.

### 7. Shop (run upgrades)
Purchases that last the entire run (e.g. "+10% damage all towers", "+1 range").
Single category for MVP. Tower equips and trinkets deferred.

### 8. Meta-Progression
Token earned at end of every run (win or lose), separate currency from run gold.
Small meta-shop/tree outside the run (e.g. from Main Menu), spendable only with
this token. Small catalog for MVP (2-3 items: permanent stat bonus, tower unlock).

---

## Post-MVP (deferred)

- Slot machine reroll (paid) and probability-skewing
- Tower equip slots (per-tower inventory)
- Trinkets (global run-wide charms)
- Expanded meta-progression catalog
- Real loadout curation (vs. all towers available)
- Second map / more waves
