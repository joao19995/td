# Game Status

Factual description of what the game currently does, organized by system.
Target audience: managers and designers. Focus is on features and configurability,
not implementation details.

---

## Game Loop

- Game opens on a **Main Menu** with a "Start Run" button.
- **Start Run** opens the **Loadout screen**, where the player selects 1–4 tower types to bring into the run.
- The run begins on a random map (Map1 or Map2). After placing towers and defeating all waves (all enemies must be killed, not just spawned), a **Fight Complete** screen appears.
- From Fight Complete the player can **"Next Fight"** (reloads a random map with gold/lives/tower upgrades preserved) or **"End Run"** (returns to Main Menu).
- Runs are infinite by design — the player fights until they lose all lives (Game Over) or chooses to end the run.
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

## Meta-Progression (Save System)

- **Persistence**: meta-progression data (tokens, unlocked towers) is saved to `user://save_data.json` using `FileAccess` + JSON — no `.tres` resources for save data (avoids documented security vector of `ResourceLoader.Load` on untrusted files).
- **Meta tokens**: awarded at the end of every run (win or lose). Configurable via `SaveManager.MetaTokensPerRun` (default 10).
- **Unlocked towers**: all 5 towers start unlocked. Future towers can be locked behind meta-progression.
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

---

## Towers

- **5 tower types**: Base, Fast, Ice, Poison, Splash. Each has configurable damage, fire rate, range, cost, and projectile.
- **Placement**: towers can only be placed on designated buildable tiles. Already-occupied tiles are blocked.
- **Selection**: clicking a placed tower selects it. A semi-transparent circle shows its range. Right-click deselects.
- **Upgrades**: each tower has a fixed upgrade path (2 tiers per tower). Upgrades increase damage, fire rate, and/or range. Each upgrade tier is a separate resource file with configurable cost and stat bonuses.
- **Sell**: towers can be sold for a percentage of their base cost (configurable ratio, default 50%). Upgrade costs are not refunded.
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
- When a tower is selected: shows its **name**, **upgrade level**, **upgrade cost** (or "MAX" if fully upgraded), and **sell value** with an action button.

---

## Screens & Menus

| Screen | When shown | Can pause |
|---|---|---|---|
| Main Menu | On startup | N/A |
| Loadout | After clicking "Start Run" on Main Menu | No |
| Pause | Pressing ESC during gameplay | Yes |
| Game Over | Lives reach zero | Yes |
| Victory | All levels completed (classic mode) | Yes |
| Fight Complete | All waves cleared during a run | Yes |

- All pause-capable screens work while the game is paused and can be dismissed with ESC.

---

## Synergies

- **Type-combination bonuses**: synergies activate based on which tower types are on the board. No adjacency or positioning logic — only type presence matters.
- **Active synergies** are automatically evaluated whenever a tower is placed or removed.
- **Stat bonuses** are multiplicative: `EffectiveX = (base + upgradeFlat) * (1 + synergyPercent)`. Damage, fire rate, and range all follow the same formula.
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
| Status effects | `PoisonEffectData`, `SlowEffectData` | Duration, damage per tick, speed multiplier |
