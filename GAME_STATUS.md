# Game Status

Factual description of what the game currently does, organized by system.
Target audience: managers and designers. Focus is on features and configurability,
not implementation details.

---

## Game Loop

- Game opens on a **Main Menu** with a "Start Run" button.
- **Start Run** opens the **Loadout screen**, where the player selects 1–4 tower types to bring into the run.
- The run begins on a random map (Map1 or Map2). After placing towers and defeating all waves (all enemies must be killed, not just spawned), a **Fight Complete** screen appears.
- From Fight Complete the player clicks **"Continue"** to spin the slot machine, which determines the next step (next fight, shop, heal, or miniboss). Gold/lives/tower upgrades are preserved across the run.
- After the configured number of fights (default 3), the slot machine automatically triggers a **Boss Fight** instead of rolling. Defeating the boss ends the run with Victory.
- The player can always choose **"End Run"** from Fight Complete to end early and receive meta tokens.
- Pressing **ESC** pauses the game at any time. Pressing again resumes.
- Running out of lives shows a **Game Over** screen with Menu buttons.
- Completing all levels in classic mode shows a **Victory** screen.

---

## Camera

- A single fixed camera covers the entire play area (default 320×190 pixels).
- No panning or zoom controls — the camera automatically fits the world to the screen so there is never a visible border.
- Each level can override the play area size.

---

## Run System

- **RunState** tracks the active run: gold, lives, per-tower upgrade levels, and selected tower IDs.
- **Loadout**: before each run the player chooses up to 4 tower types from the 5 available. Only chosen towers appear in the HUD during fights.
- **Gold and lives persist** across fights within a run (no reset between fights). They are the single source of truth in EconomyManager/GameManager — RunState does not duplicate them.
- **Tower upgrades persist** by tower type across fights. If a Base tower is upgraded to level 2 and sold, the next Base tower placed starts at level 2.
- **1-per-type** enforcement: only one tower of each type can be on the map at once. The HUD grays out the button and shows "(Placed)" for types already placed.
- **Random map rotation**: each "Next Fight" loads a random map (Map1 or Map2).

## Run Engine (Slot Machine)

- After each fight during a run, a **slot machine** roll determines what happens next:
  - **Fight** (35%): proceeds to a standard random map fight
  - **Shop** (20%): enters the run shop to buy run-wide stat bonuses and tower equipment
  - **Heal** (15%): restores 5 lives (configurable) without a fight
  - **Miniboss** (10%): harder fight with 1.5x enemy stats on a random map
  - **Treasure** (20%): pick 1 of 3 random trinkets (run-wide charms)
- After the configured number of fights (default 3), a **boss fight** triggers automatically using a dedicated boss wave (Boss enemy + minions).
- Defeating the boss ends the run with a **Victory** screen and awards meta tokens.
- The shop sells run-wide bonuses (damage, fire rate) **and** tower-specific equipment that can be equipped per tower type.
- All weights, heal amount, miniboss multiplier, fights-per-run, reroll cost, and skew factor are exported on SlotManager — no code changes needed to tweak.
- Outcomes that involve a fight (Fight, Miniboss, Boss) reset all tower placement — player places towers fresh each fight.
- **Reroll**: after seeing the slot outcome (except Boss/Heal), the player can pay gold to re-roll. Cost scales (50g → 100g → 150g). Each reroll reduces the weight of the outcome being rerolled by 50%, making repeats less likely.

## Tower Equipment

- Each tower type has **1 equipment slot**. Equipment is bought in the run Shop and persists across fights within the run.
- **Type-restricted**: equipment items target specific tower types (e.g. Heavy Barrel only works on Base). Only shown in the Shop if that tower is in the loadout.
- **Stat bonuses** are multiplicative-percent and stack additively with synergy bonuses:
  `EffectiveX = (base + upgradeFlat) * (1 + synergy + equipPercent) * (1 + shop) * (1 + meta) * (1 + trinket)`
- **3 items shipped**: Heavy Barrel (+15% damage, Base, 80g), Overdrive Coils (+20% fire rate, Fast, 80g), Precision Lens (+20% range, Ice, 80g).

## Trinkets (Run-Wide Charms)

- **Treasure outcome** on the slot machine (20% base weight) lets the player choose 1 of 3 random trinkets.
- Trinkets apply **run-wide** effects that last for the rest of the run:
  - *War Banner*: +10% global damage (multiplicative in EffectiveDamage formula)
  - *Guardian Angel*: restore 5 lives immediately
  - *Lucky Coin*: gain 100 gold immediately
- Trinkets are single-use per run — once chosen, the effect is applied.

## Meta-Progression (Save System)

- **Persistence**: meta-progression data (tokens, unlocked towers) is saved to `user://save_data.json` using `FileAccess` + JSON — no `.tres` resources for save data (avoids documented security vector of `ResourceLoader.Load` on untrusted files).
- **Meta tokens**: awarded at the end of every run (win or lose). Configurable via `SaveManager.MetaTokensPerRun` (default 10).
- **Unlocked towers**: only Base and Fast start unlocked. Ice, Poison, and Splash are purchased with tokens in the Meta Shop.
- **Meta Shop**: accessible from the Main Menu via a "Meta Shop" button. Lists all available upgrades with current level, cost, and Buy/MAX status. Purchases use meta-tokens exclusively.
- **Upgrade catalog** (5 items):
  - Unlock Ice Tower (20 tokens)
  - Unlock Poison Tower (30 tokens)
  - Unlock Splash Tower (50 tokens)
  - Global Damage +5% per level (15 tokens base, 3 levels max)
  - Starting Gold +50 per level (10 tokens base, 3 levels max)
- **Multi-level upgrades**: costs scale by level. Buying level 1 costs `CostTokens × 1`, level 2 costs `CostTokens × 2`, etc. Each level grants the configured bonus.
- **Stat application**: damage bonus applies multiplicatively to all towers (`EffectiveDamage × (1 + metaPercent)`). Starting gold bonus is added before the run begins.
- **Corruption handling**: save file corruption or incompatible schema triggers a clean default reset with a warning — the game never crashes on broken save data.
- **Migration**: if new default towers are added in a future update, existing save files automatically include them on first load after the update.

## Levels

- The game ships with **2 maps** (Map1, Map2), each with its own tile layout, enemy path, and wave list.
- Adding a new map requires only a new scene and a **LevelData** resource file — no code changes.
- Levels can configure:
  - Starting money and lives (or use global defaults)
  - Play area size (world size in pixels)

---

## Waves

- Waves are defined per level. Each wave specifies enemy type, count, spawn interval, and optional delay.
- The player must manually click **"Next Wave"** to start each wave.
- The HUD shows progress (`Wave X / Y`).
- **All enemies must be killed** (not just spawned) before the game considers all waves completed.
- After all waves are completed in classic mode:
  - If another level exists, a **"Next Level"** button appears.
  - If it was the final level, the **Victory** screen is shown.
- During a run, all waves completed shows the **Fight Complete** screen instead.

### Wave Decoupling (Run Mode)

- Waves are **decoupled from map layout** during runs. Each fight picks a wave from a **difficulty tier** based on `FightsCompleted`:
  - Fight 1 → `tier1` (easy: Normal, Flying)
  - Fight 2 → `tier2` (medium: Normal+Fast, Boss+Tank)
  - Fight 3+ → `tier3` (hard: Tank+Fast+Normal, Flying+Tank)
- Waves are stored in `resources/wave_data/tier1/`, `tier2/`, `tier3/`. Adding a `.tres` to a folder is zero-code-change.
- `LevelData.Waves` is used only in classic mode (non-run) — unchanged.
- **Boss fights** ignore the tier system and always use `BossWaveData` (set in `LevelManager` Inspector).
- **Miniboss** `statMultiplier` (1.5× HP, gold, damage) is cumulative with tier difficulty.

---

## Towers

- **5 tower types**: Base, Fast, Ice, Poison, Splash. Each has configurable damage, fire rate, range, cost, and projectile.
- **Placement**: towers can only be placed on designated buildable tiles. Already-occupied tiles are blocked.
- **Selection**: clicking a placed tower selects it. A semi-transparent circle shows its range. Right-click deselects.
- **Upgrades**: each tower has a fixed upgrade path (2 tiers per tower). Upgrades increase damage, fire rate, and/or range. Each upgrade tier is a separate resource file with configurable cost and stat bonuses.
- **Targeting priority**: each tower can be set to target the first enemy in range, the closest, the strongest (most health), or the most recent.

### Tower special abilities

| Tower | Behavior | Configurable in |
|---|---|---|
| **Base** | Standard single-target | TowerData resource |
| **Fast** | High fire rate, lower damage | TowerData resource |
| **Ice** | Slows enemies on hit (blue tint). Reapplying refreshes duration, does not stack intensity | TowerData + SlowEffectData resources |
| **Poison** | Damage-over-time on hit (green tint). Reapplying refreshes duration, does not stack intensity | TowerData + PoisonEffectData resources |
| **Splash** | Area damage to all enemies within a radius of the impact point. Shows a fading circle effect | TowerData resource (radius, damage) |

---

## Projectiles

- Projectiles are homing — they track and follow their target automatically.
- On hit: the projectile deals damage, applies the tower's special effect (if any), and disappears.
- If the target enemy dies or reaches the end while the projectile is in flight, the projectile disappears without hitting anything.

---

## Enemies

- **5 enemy types**: Normal, Fast, Tank, Flying, Boss. Each has configurable health, speed, gold reward, sprite, and damage to player on reaching the end.
- Enemies follow a fixed path defined per level.
- If an enemy reaches the end, it damages the player's lives and is removed.
- Each enemy displays:
  - A **health bar** above it (green/red) that updates in real time.
  - **Damage numbers** that float up when the enemy takes damage.
- Status effects are visible as sprite tints (green for poison, blue for slow).

---

## Status Effects

- **Poison**: deals damage over time at regular intervals. Duration refreshes on reapplication; intensity does not stack.
- **Slow**: reduces movement speed. Duration refreshes on reapplication; intensity does not stack.
- When the target dies or is removed, all effects are cleared.
- Both effects have configurable duration, damage per tick (poison), and speed multiplier (slow) via resource files.

---

## Economy & Lives

- Each level can configure starting money and lives individually. If not set, the game uses global defaults.
- **Money** is earned by killing enemies and spent on placing and upgrading towers.
- **Lives** are lost when enemies reach the end of the path. Zero lives triggers Game Over.
- All economy values (enemy gold rewards, tower costs, upgrade costs) are configurable in resource files.

---

## HUD (User Interface during gameplay)

- Displays current **lives**, **money**, and **wave number**.
- Shows a button for each available tower type (determined by loadout during runs). Buttons are disabled when the player cannot afford the tower, when that type is already placed, or during Game Over.
- Buttons update reactively when money changes, towers are placed, or towers are deselected.
- **"Next Wave"** button to start each wave. **"Next Level"** button after all waves are cleared.
- When a tower is selected: shows its **name**, **upgrade level**, **upgrade cost** (or "MAX" if fully upgraded), and **equipped item**.

---

## Screens & Menus

| Screen | When shown | Can pause |
|---|---|---|---|---|
| Main Menu | On startup | N/A |
| Loadout | After clicking "Start Run" on Main Menu | No |
| Pause | Pressing ESC during gameplay | Yes |
| Game Over | Lives reach zero | Yes |
| Victory | All levels completed (classic mode) | Yes |
| Fight Complete | All waves cleared during a run | Yes |
| Shop | Slot machine outcome — buy run upgrades | Yes |
| Meta Shop | Main Menu — permanent upgrades with meta-tokens | Yes |
| Trinket Choice | Slot machine Treasure outcome — pick 1 of 3 trinkets | Yes |
| Briefing | Before every fight in a run (map name + wave list) | No |
| Bestiary | Main Menu button or Pause screen — browse towers/enemies/equip/trinkets/synergies | Yes |

- All pause-capable screens work while the game is paused and can be dismissed with ESC.

---

## Synergies

- **Type-combination bonuses**: synergies activate based on which tower types are on the board. No adjacency or positioning logic — only type presence matters.
- **Active synergies** are automatically evaluated whenever a tower is placed or removed.
- **Stat bonuses** are multiplicative: `EffectiveX = (base + upgradeFlat) * (1 + synergy + equip) * (1 + shop) * (1 + meta) * (1 + trinket)`. Damage, fire rate, and range all follow the same formula.
- **Visual feedback**: towers affected by any synergy show a green tint. The HUD lists active synergy names at the top of the screen.
- **Data-driven**: synergies are defined in `SynergyData` resource files under `resources/synergy_data/`. Adding a new `.tres` file in that folder is enough to register it — no code changes needed.
- **Examples shipped**:
  - *Frost Venom*: Ice + Poison towers on the board → +15% damage to both
  - *Overclock*: 3+ different tower types → +10% fire rate to all towers

---

## Configurability (No-Code Changes)

The following can be modified by editing resource files (`.tres`) in the project's `resources/` folder — no programming required:

| What | File type | Examples |
|---|---|---|
| Tower stats | `TowerData` | Damage, fire rate, range, cost, splash radius, poison/slow properties |
| Upgrade tiers | `UpgradeData` | Cost, damage/fire rate/range bonus |
| Enemy stats | `EnemyData` | Health, speed, gold reward, damage to player |
| Level setup | `LevelData` | Scene path, preview image, starting money/lives, world size |
| Screen/overlay config | `UIScreenData` | Scene path, whether it pauses the game |
| Synergy combos | `SynergyData` | Required tower IDs, min tower count, bonus percentages (damage/fire rate/range) |
| Run engine (slot machine) | `SlotManager` Inspector | Fight count per run, outcome weights, heal amount, miniboss multiplier, reroll cost, skew factor |
| Shop items (run upgrades) | `ShopItemData` resource | Item ID, name, cost, stat bonus percent |
| Tower equipment | `EquipData` resource | Item ID, name, cost, target tower type, stat bonus percents |
| Trinkets | `TrinketData` resource | Item ID, name, damage bonus, heal amount, gold amount |
| Status effects | `PoisonEffectData`, `SlowEffectData` | Duration, damage per tick, speed multiplier |
| Wave tiers (run mode) | directory `wave_data/tier1/`, `tier2/`, `tier3/` | Add `.tres` files to a tier folder — selected by `FightsCompleted` |
| Bestiary discovery | `SaveManager` JSON | Towers/enemies/equip/trinkets/synergies auto-discovered when first encountered |

## Bestiary / Discovery

- **Bestiary screen**: accessible from Main Menu (button) or Pause screen (`PauseGame=true` overlay). Shows 5 categories: Towers, Enemies, Equipment, Trinkets, Synergies.
- **All data is scanned from directories** — zero-code-change to add entries.
- **Discovery tracking**: items are hidden (grayed out with `???`) until first encountered:
  - Enemies: discovered when they spawn in a fight
  - Equipment: discovered when seen in the Shop
  - Trinkets: discovered when shown in Treasure choice
  - Synergies: discovered when activated (required towers placed)
  - Towers: use existing unlock system (meta-progression + default unlocks)
- **Data persists** across runs in `SaveManager` JSON (`discovered` dict).
