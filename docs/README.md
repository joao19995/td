# Sourdough Siege — Documentation

## Quick Route

| Task type | Read first |
|---|---|
| Architecture / code change | `../CLAUDE.md` + `../CLAUDE_ADDENDUM.md` |
| Feature implementation | `../AGENTS.md` + relevant design doc |
| New tower | `content/towers/README.md` + `design/TOWER_ROLES.md` (planned) |
| New enemy | `content/enemies/README.md` + `design/ENEMY_SCALING.md` (planned) |
| Economy / shop change | `balance/ECONOMY_BASELINE.md` + `balance/SHOP_BALANCING.md` |
| Lore / content design | relevant `lore/` + `content/` doc |
| Visual asset | `art/SPRITE_GUIDELINES.md` (planned) |
| Playtest / balance adjustment | `playtests/` + `balance/` (start with `DEMO_CONTRACT.md`) |

## Index

### Architecture & Process
| File | Purpose |
|---|---|
| [`../CLAUDE.md`](../CLAUDE.md) | Source of truth for architectural rules (R1–R10) |
| [`../CLAUDE_ADDENDUM.md`](../CLAUDE_ADDENDUM.md) | Concrete right/wrong examples for each rule |
| [`../AGENTS.md`](../AGENTS.md) | Mandatory operational process for AI agents |

### Status & Roadmap
| File | Purpose |
|---|---|
| [`status/GAME_STATUS.md`](status/GAME_STATUS.md) | Currently implemented state — what exists and how it works |
| [`../ROADMAP.md`](../ROADMAP.md) | Future work and planned progress |

### Design
System-level design intent and rules.

| File | Purpose |
|---|---|
| `design/GAME_LOOP.md` (planned) | Menu → run → result flow |
| `design/RUN_STRUCTURE.md` (planned) | Slot machine, shop, fight rhythm |
| `design/COMBAT.md` (planned) | Tower mechanics, enemy design, damage formula |
| `design/STATUS_EFFECTS.md` (planned) | Poison, slow, anti-buff rules |
| [`design/TRINKETS.md`](design/TRINKETS.md) | Trinket catalog |
| [`design/WAVES.md`](design/WAVES.md) | Wave compositions by tier |
| [`design/META_PROGRESSION.md`](design/META_PROGRESSION.md) | Permanent upgrades catalog |

### Balance
Numbers, economy, and tuning models.

| File | Purpose |
|---|---|
| [`balance/DEMO_CONTRACT.md`](balance/DEMO_CONTRACT.md) | Demo V0 design targets and success criteria |
| [`balance/TOWER_BASELINE.md`](balance/TOWER_BASELINE.md) | All 10 towers: stats, DPS, DPS/Gold, hypotheses |
| [`balance/ENEMY_BASELINE.md`](balance/ENEMY_BASELINE.md) | All 10 enemies: stats, pressure, gold efficiency |
| [`balance/ECONOMY_BASELINE.md`](balance/ECONOMY_BASELINE.md) | Gold pacing model for 5 fights + 1 boss |
| `balance/BALANCE_OVERVIEW.md` (planned) | Principles and methodology |
| `balance/TOWER_ROLES.md` (planned) | Tower identity, niche, role analysis |
| `balance/ENEMY_SCALING.md` (planned) | HP/gold/damage curves by tier |
| [`balance/SHOP_BALANCING.md`](balance/SHOP_BALANCING.md) | Shop item costs and effects |

### Content
Per-entity documentation (identity, role, lore, strategy).

| Directory | Files |
|---|---|
| [`content/towers/`](content/towers/) | README + 10 tower docs |
| [`content/enemies/`](content/enemies/) | README + 10 enemy docs |
| [`content/maps/`](content/maps/) | Map catalog |
| `content/equipment/` | (planned — 20 items) |
| `content/trinkets/` | (planned — 10 trinkets) |
| `content/synergies/` | (planned — 6 synergies) |

### Lore
Narrative canon and world-building.

| File | Purpose |
|---|---|
| `lore/WORLD.md` (planned) | Setting, world rules |
| `lore/ORDER_OF_THE_MOTHER_DOUGH.md` (planned) | Faction lore |
| `lore/ENEMY_FACTIONS.md` (planned) | Enemy lore and hierarchy |

### Art
Visual standards and guidelines.

| File | Purpose |
|---|---|
| `art/SPRITE_GUIDELINES.md` (planned) | 320x190 constraints, palette, silhouette |
| `art/ANIMATION_GUIDELINES.md` (planned) | Frame budgets, VFX rules |
| `art/UI_GUIDELINES.md` (planned) | HUD, screens, tooltips |

### Playtests
Test records and observations.

| File | Purpose |
|---|---|
| [`playtests/README.md`](playtests/README.md) | When and how to record playtests |
| [`playtests/PLAYTEST_TEMPLATE.md`](playtests/PLAYTEST_TEMPLATE.md) | Session template |

### Architecture
Detailed technical docs (not global rules — those are in `CLAUDE.md`).

| Directory | Purpose |
|---|---|
| `architecture/` | (planned) Component docs, data flow, system internals |

---

> **Source of truth**: `.tres` files under `resources/` hold actual gameplay values. Documentation describes intent, identity, and design — it does not compete with `.tres` as the authoritative source of numeric values.
