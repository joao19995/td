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

### 2. SaveManager
Persistent storage for meta-progression token and permanent unlocks.
Reads/writes the same data structures defined in RunState.

### 3. Synergies
Bonuses based on which tower types are present on the board. With only 5 towers
and max 1 of each, synergies are type-combination based (no adjacency logic).
Examples: "Ice + Poison → enemies hit by both take +X damage", "3+ different
tower types → +X% damage for all". Visual indicator on towers when a synergy
is active.

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
