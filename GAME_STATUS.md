# Game Status

Factual description of what the game currently does, organized by system.
Target audience: managers and designers. Focus is on features and configurability,
not implementation details.

---

## Game Loop

- Game opens on a **Main Menu** with a Play button.
- Playing starts the first level. After placing towers and defeating waves, the player advances through levels until the final victory.
- Pressing **ESC** pauses the game at any time. Pressing again resumes.
- Running out of lives shows a **Game Over** screen with a Retry button.
- Completing all waves on a level shows **"Next Level"**. The final level shows a **Victory** screen.

---

## Camera

- A single fixed camera covers the entire play area (default 320×190 pixels).
- No panning or zoom controls — the camera automatically fits the world to the screen so there is never a visible border.
- Each level can override the play area size.

---

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
- After all waves are completed:
  - If another level exists, a **"Next Level"** button appears.
  - If it was the final level, the **Victory** screen is shown.

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
- Shows a button for each available tower type (name + cost). Buttons are disabled when the player cannot afford the tower or during Game Over.
- **"Next Wave"** button to start each wave. **"Next Level"** button after all waves are cleared.
- When a tower is selected: shows its **name**, **upgrade level**, **upgrade cost** (or "MAX" if fully upgraded), and **sell value** with an action button.

---

## Screens & Menus

| Screen | When shown | Can pause |
|---|---|---|
| Main Menu | On startup | N/A |
| Pause | Pressing ESC during gameplay | Yes |
| Game Over | Lives reach zero | Yes |
| Victory | All levels completed | Yes |

- All pause-capable screens work while the game is paused and can be dismissed with ESC.

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
| Status effects | `PoisonEffectData`, `SlowEffectData` | Duration, damage per tick, speed multiplier |
