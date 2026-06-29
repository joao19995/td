# Roadmap

MVP feature list, in a sensible build order based on dependencies. Each item gets
detailed (tasks, design decisions) when work on it actually starts. When an item is
finished, move its outcome into GAME_STATUS.md as a behavior description.

**Global constraints:**
- 5 tower types, fixed (Base, Fast, Ice, Poison, Splash)
- Each map: max 1 tower of each type = max 5 towers per map
- Towers are placed fresh each fight (no persistence of positions between fights)
- Runs reuse one map (Map1 for MVP)

---

## MVP Build Order

### 1. RunState
Run-scoped state container: gold for the run, lives, and per-tower upgrade levels
(these persist across fights within a run). Tower positions are not stored — they
are placed fresh each fight. Each fight the player can place at most one of each
tower type.

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
