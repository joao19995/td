# Game Status

Factual description of what the game currently does, organized by system.
Target audience: managers and designers. Focus is on features and configurability,
not implementation details.

---

## Game Loop

- Game opens on a **Main Menu** with a "Start Run" button.
- **Start Run** opens the **Loadout screen**, where the player selects 1–4 tower types to bring into the run.
- The run begins on a random map (Map1 or Map2). After placing towers and defeating all waves (all enemies must be killed, not just spawned), a **Fight Complete** screen appears.
- From Fight Complete the player clicks **"Continue"** to spin the slot machine, which determines the next step (next fight, shop, heal, treasure, miniboss, or boss). Gold/lives/tower upgrades are preserved across the run.
- After the configured number of fights (default 3), the slot machine automatically triggers a **Boss Fight** instead of rolling. Defeating the boss ends the run with Victory.
- The player can always choose **"End Run"** from Fight Complete to end early and receive meta tokens.
- Pressing **ESC** pauses the game at any time. Pressing again resumes.
- Running out of lives shows a **Game Over** screen with Menu buttons.
- Completing all levels in classic mode shows a **Victory** screen.

---

## Camera

- A single fixed camera covers the entire play area (default 320x190 pixels).
- No panning or zoom controls — the camera automatically fits the world to the screen so there is never a visible border.
- Each level can override the play area size.

---

## Run System

- **RunState** tracks the active run: gold, lives, per-tower upgrade levels, selected tower IDs, and per-equipment state.
- **Loadout**: before each run the player chooses up to 4 tower types from all unlocked towers. Only chosen towers appear in the HUD during fights.
  - **Stat preview**: hovering a tower shows name, sprite, DMG/SPD/RNG, and special tags (SPLASH/POISON/SLOW/AURA/CHAIN/CRIT/EXECUTE/GLOBAL) in a side panel.
  - **Synergy hints**: if the selected towers activate a synergy that has already been discovered in a previous run, the synergy name is shown.
  - **Random button**: "RND" selects `MaxTowers` random unlocked towers.
  - **Save slots**: 3 numbered buttons. Left-click loads if slot has data (or saves if empty). Right-click overwrites. Persisted in SaveManager JSON (`loadout_slots`). Tooltip shows slot contents on hover.
- **Gold and lives persist** across fights within a run (no reset between fights). They are the single source of truth in EconomyManager/GameManager — RunState does not duplicate them.
- **Tower upgrades persist** by tower type across fights. If a Bread Baker tower is upgraded to level 2 and sold, the next Bread Baker placed starts at level 2.
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
- The shop sells run-wide bonuses (damage, fire rate, heavy damage, first-purchase discount) **and** tower-specific equipment that can be equipped per tower type.
- All weights, heal amount, miniboss multiplier, fights-per-run, reroll cost, and skew factor are exported on SlotManager — no code changes needed to tweak.
- Outcomes that involve a fight (Fight, Miniboss, Boss) reset all tower placement — player places towers fresh each fight.
- **Reroll**: after seeing the slot outcome (except Boss/Heal), the player can pay gold to re-roll. Cost scales (50g -> 100g -> 150g). Each reroll reduces the weight of the outcome being rerolled by 50%, making repeats less likely.

## Tower Equipment

- Each tower type has **1 equipment slot**. Equipment is bought in the run Shop and persists across fights within the run.
- **Type-restricted**: equipment items target specific tower types (e.g. Stone Oven only works on Bread Baker). Only shown in the Shop if that tower is in the loadout.
- **Shop UI**: each equipment shows its **icon** (16x16 TextureRect), **name + cost + target tower**, and **description**. The buy button shows "Buy & Equip" if the slot is empty, "Replace (cost)" if another equip is already equipped for that tower, or "✓ Equipped" if already owned.
- **Swap**: buying a new equipment for a tower that already has one replaces it (no refund). The entire equip list is rebuilt after purchase.
- **Tower panel**: when selecting a tower during gameplay, the action panel shows the equipped item's **name**, **icon**, and **stat bonuses** (e.g. `+15% DMG | +20% POISON`) derived from the `EquipData` fields. Hovering the name shows a tooltip with description.
- **Stat bonuses** are multiplicative-percent and stack additively with synergy bonuses in the EffectiveDamage formula.
- **20 equipment items shipped** (2 per tower type), each with unique mechanics:

  | Equip | Tower | Effect | Special Mechanic |
  |---|---|---|---|
  | Stone Oven | Bread Baker | +15% Damage | — |
  | Ancient Starter | Bread Baker | +10% Damage | +1 raw damage per 10 attacks (stacking, resets on sell, persists across fights) |
  | Electric Bike | Bread Courier | +20% Fire Rate | — |
  | Messenger Crate Upgrade | Bread Courier | +1 pierce, -10% Fire Rate | Projectile hits 2 enemies instead of 1 |
  | Megaphone | Aroma Keeper | +20% Range | — |
  | Spice Wind Chimes | Aroma Keeper | +30% Slow Duration, -10% Damage | — |
  | Silver Tray | Taste Tester | +20% Poison Damage | — |
  | Double Sampling Plates | Taste Tester | applies poison to +1 extra target, -15% Fire Rate | Nearest enemy in 30px gets poison directly |
  | Reinforced Suspension | Bakery Truck | +15% Splash Radius | — |
  | Street Parade Route | Bakery Truck | +30% Area Damage, -20% Range | — |
  | Sacred Robes | Bread Monk | +15% Aura Range | Affects aura scan range, not attack range |
  | Prayer Beads of Gluten | Bread Monk | buffs +5% stronger, tower does not attack | Aura bonus percent increased; tower stops firing |
  | Golden Proofing Bowl | Fermentation Sage | +15% Effect Duration | — |
  | Wild Yeast Culture | Fermentation Sage | chain +1 extra bounce, -10% initial Damage | — |
  | Tempered Crust Blade | Crust Crusader | +10% Crit Chance | — |
  | Blessed Crunch Seal | Crust Crusader | crits make mini-splash (radius 30), -10% Fire Rate | Separate splash triggered only on crit connect |
  | Holy Flour Pouch | Dough Exorcist | +20% Damage vs elites/boss | Bonus only applies to IsBoss/IsHeavy targets, not flat |
  | Judgment Seal | Dough Exorcist | executes below 15% HP, -15% Damage | Instant kill at <15% HP with 5s internal cooldown |
  | Golden Staff of Fermentation | High Prophet | +10% All Stats | — |
  | First Starter Relic | High Prophet | +5% damage per nearby tower (range 40), -15% Range | Counts towers within 40px, bonus per tower |

## Trinkets (Run-Wide Charms)

- **Treasure outcome** on the slot machine (20% base weight) lets the player choose 1 of 3 random trinkets.
- **Card UI**: each trinket is presented as a `PanelContainer` card with:
  - 32×32 icon from `TrinketData.Icon`
  - Name and description labels
  - Rarity-colored border — **Common** (gray), **Rare** (gold)
  - Hover highlight and sequential fade-in animation (0.15s stagger)
- **Skip button**: "Skip" advances without applying a trinket
- Trinkets apply **run-wide** effects that last for the rest of the run:
  - *Secret Recipe Scroll*: +10% global damage
  - *Starter's Blessing*: restore 5 lives immediately
  - *Regular's Tip Jar*: gain 100 gold immediately
  - *Proofing Time Candle*: +8% Attack Speed, -5% Range (trade-off, **Rare**)
  - *Crust Fragment Relic*: +12% Crit Damage (multiplicative)
  - *Fermentation Diary*: +15% status effect duration (slow, poison)
  - *Sacred Flour Dust*: +10% slow/poison strength
  - *Heretic Census List*: +10% damage vs basic enemies
  - *Oven Heart Ember*: +1 starting tower range (small global aura, ~10px)
  - *First Starter Vessel*: every 30s gains +5 passive gold
- **10 trinkets total** (9 Common, 1 Rare). Rarity determined by `TrinketData.Rarity` enum field.
- Passive gold uses a timer in RunState._Process. Crit damage is multiplicative to the base crit multiplier. Trade-off trinkets use negative values in existing stat fields.
- Trinkets are single-use per run — once chosen, the effect is applied.
- On Take/Skip: fade-out animation (0.2s) before transitioning to the next screen.

## Shop Items (Run-Wide Purchases)

- **5 items** available in the run Shop:

  | # | Name | Effect | Cost |
  |---|---|---|---|
  | 1 | Secret Ingredient | +5% damage global | 80 |
  | 2 | Fresh Batch | +5% fire rate global | 60 |
  | 3 | Golden Proof Flour | +5% damage vs armored/heavy enemies | 90 |
  | 4 | Rapid Oven Upgrade | +8% fire rate, -5% range (trade-off) | 100 |
  | 5 | Discounted Starter Yeast | first shop purchase costs -30% gold | 120 |

- Shop items are loaded dynamically from `resources/run_data/` — no code changes needed.
- Each item shows its **icon** (16x16 TextureRect), **name + cost**, and **description**.
- Purchase feedback: button flashes green via Tween, text changes to "✓ Owned".
- **1-per-item enforcement**: once bought, the button is disabled and shows "✓ Owned". Purchased item IDs are tracked in `RunState.PurchasedShopItemIds` — persists across re-entering the shop within the same run.
- **In-level buff icons**: purchased items appear as small 16x16 icons in the HUD (`BuffIconsContainer`) during fights. Hovering shows a tooltip with item name and description.
- `HeavyDamageBonusPercent` and `FirstPurchaseDiscountPercent` are applied at purchase time via RunState.
- Multiplicative in the EffectiveDamage formula.

## Meta-Progression (Save System)

- **Persistence**: meta-progression data (tokens, unlocked towers, discoveries) is saved to `user://save_data.json` using `FileAccess` + JSON — no `.tres` resources for save data (avoids documented security vector of `ResourceLoader.Load` on untrusted files).
- **Meta tokens**: awarded at the end of every run (win or lose). Token reward scales: `base × (1 + fightsCompleted / totalFights)`. Victory bonus: +50% tokens if the boss was defeated.
- **Unlocked towers**: Bread Baker and Bread Courier start unlocked. All other 8 towers are purchased with tokens in the Meta Shop.
- **Meta Shop**: accessible from the Main Menu via a "Meta Shop" button. Has tabbed categories: **All**, **Unlocks**, **Stats**, **Economy**. Lists all available upgrades with current level, cost, and Buy/MAX status. Purchases use meta-tokens exclusively.
- **Upgrade catalog** (16 items, 3 categories):

  **Unlocks** (9 items):
  - 8 tower unlocks (various token costs)
  - Starter Gear Voucher (40 tokens, 1 level): start each run with 1 random equipment on a loadout tower
  - Seasoned Recruits (50 tokens, 1 level): newly placed towers start at upgrade level 1

  **Stats** (3 items):
  - Secret Recipe +5% per level (15 tokens base, 3 levels max) — global damage bonus
  - Local Sponsorship +50 starting gold per level (10 tokens base, 3 levels max) — starting gold
  - Extra Loaves +2 lives per level (15 tokens base, 3 levels max) — starting lives

  **Economy** (4 items):
  - Bulk Flour Discount -5% per level (20 tokens base, 3 levels max) — shop item costs reduced
  - Second Chance Discount -10% per level (15 tokens base, 3 levels max) — reroll costs reduced
  - Tax on Industrial Bread +10% per level (20 tokens base, 3 levels max) — enemies grant more gold

- **Multi-level upgrades**: costs scale by level. Buying level 1 costs `CostTokens x 1`, level 2 costs `CostTokens x 2`, etc. Each level grants the configured bonus.
- **Stat application**: damage bonus applies multiplicatively to all towers (`EffectiveDamage x (1 + metaPercent)`). Starting gold/lives bonuses are added before the run begins. Shop discount reduces displayed costs in the shop UI. Reroll cost reduction applies at the slot machine. Enemy gold bonus multiplies all enemy gold rewards. Starter Gear Voucher picks a random equipment for a random loadout tower at run start. Seasoned Recruits sets all loadout towers to level 1 at run start.
- **Corruption handling**: save file corruption or incompatible schema triggers a clean default reset with a warning — the game never crashes on broken save data.
- **Migration**: if new default towers are added in a future update, existing save files automatically include them on first load after the update.

## Synergies

- **Type-combination bonuses**: synergies activate based on which tower types are on the board. No adjacency or positioning logic — only type presence matters.
- **Active synergies** are automatically evaluated whenever a tower is placed or removed.
- **4 synergies shipped**:

  | # | Name | Requirements | Effect |
  |---|---|---|---|
  | 1 | One Whiff, One Bite | Aroma Keeper + Taste Tester | +15% damage to both |
  | 2 | Grand Opening Rush | 3+ tower types | +10% fire rate to all |
  | 3 | Holy Fermentation Network | Aroma Keeper + Fermentation Sage | Slow/chain effects spread +30% further |
  | 4 | Crust Judgment Protocol | Crust Crusader + Dough Exorcist | Crits against <50% HP cause instant kill (10s cooldown) |

- **Stat bonuses** are additive with equip bonuses: `EffectiveX = (base + upgradeFlat) * (1 + synergy + equip) * (1 + shop) * (1 + meta) * (1 + trinket)`. Damage, fire rate, and range all follow the same formula.
- **Visual feedback**: towers affected by any synergy show a green tint. The HUD lists active synergy names at the top of the screen.
- **Data-driven**: synergies are defined in `SynergyData` resource files under `resources/synergy_data/`. Adding a new `.tres` file in that folder is enough to register it — no code changes needed.

## Levels

- The game ships with **2 maps** (Map1, Map2), each with its own tile layout, enemy path, and wave list.
- Adding a new map requires only a new scene and a **LevelData** resource file — no code changes.
- Levels can configure:
  - Starting money and lives (or use global defaults)
  - Play area size (world size in pixels)

---

## Waves

- Waves are defined per level via `WaveData` resources. Each wave specifies:
  - `Entries` (Array of `WaveEntry`): each entry has an `EnemyData` reference and a `Count` — enables per-type enemy counts (e.g. "3x Tourist + 2x Pigeon" instead of a single flat type/count).
  - `SpawnInterval` (float): delay between individual enemy spawns.
  - `Modifier` (enum): `None`, `Horde` (2× enemies, 0.5× interval), `Armored` (2× HP), `Swift` (1.5× speed), `GoldRush` (2× gold).
- Enemy spawn order is Round-Robin across entries (interleaved).
- **Elite enemies**: each non-boss, non-heavy enemy has a 20% chance to spawn as Elite (2× HP, 1.5× damage to player, 2× gold reward). `EnemySpawner.EliteChance` is configurable in the Inspector.
- The player must manually click **"Next Wave"** to start each wave.
- The HUD shows progress (`Wave X / Y`).
- **All enemies must be killed** (not just spawned) before the game considers all waves completed.
- After all waves are completed in classic mode:
  - If another level exists, a **"Next Level"** button appears.
  - If it was the final level, the **Victory** screen is shown.
- During a run, all waves completed shows the **Fight Complete** screen instead.

### Wave Decoupling (Run Mode)

- Waves are **decoupled from map layout** during runs. Each fight picks waves from a **difficulty tier** based on `FightsCompleted`:
  - Fight 1 -> `tier1` (easy)
  - Fight 2 -> `tier2` (medium)
  - Fight 3+ -> `tier3` (hard)
- Waves are stored in `resources/wave_data/tier1/`, `tier2/`, `tier3/`. Adding a `.tres` to a folder is zero-code-change. 5 waves per tier (15 total) for run mode variety.
- `LevelData.Waves` is used only in classic mode (non-run) — unchanged.
- **Boss fights** ignore the tier system and always use `BossWaveData` (set in `LevelManager` Inspector).
- **Miniboss** `statMultiplier` (1.5x HP, gold, damage) is cumulative with tier difficulty.

### Wave Generation (5-10 Waves Per Fight)

- `RunState.PickRunWaves()` generates **5-10 random waves** per fight, drawn with repetition from the current tier pool.
- **Difficulty scaling** applies to each wave within the fight:
  - `DifficultyMultiplier` ranges from **0.6x** (first wave) to **1.4x** (last wave)
  - Affects: **enemy HP** (×multiplier), **spawn interval** (÷√multiplier → faster at high difficulty)
- **Final stretch**: the last 3 waves get **1.5× enemy count**.
- **Modifier assignment**: waves 1-2 have no modifier (use wave's base). Waves 3+ get random modifiers from the pool. Last 2 waves draw from a harder pool (Horde, Armored). Consecutive same-modifier is avoided.
- **Wave data is cloned at generation** — templates `.tres` are never mutated. Runtime fields (`DifficultyMultiplier`, `IsFinalStretch`) are set on the clone.
- Briefing screen shows each wave as `W1 [0.6x]: 5 enemies`, with modifier tags for non-`None` modifiers.

---

## Towers

- **10 tower types**, each with configurable damage, fire rate, range, cost, and projectile via TowerData resource.

  | Tower | Cost | Type | Special Mechanic |
  |---|---|---|---|
  | Bread Baker | 100 | Standard single-target | — |
  | Bread Courier | 120 | High fire rate, low damage | — |
  | Aroma Keeper | 150 | Support | Slows enemies on hit (blue tint, duration refresh, no intensity stack) |
  | Taste Tester | 180 | Support | Damage-over-time poison on hit (green tint, duration refresh, no intensity stack) |
  | Bakery Truck | 250 | Area | Splash damage to all enemies within a radius. Shows a fading circle effect |
  | Bread Monk | 220 | Aura support | Proximity aura buffs nearby towers (+10% dmg, +10% FR within 60px). Does not attack with Prayer Beads equip |
  | Fermentation Sage | 250 | Chain | Projectile chains to nearest enemy on hit (1 bounce, 40px range, 50% damage). Propagates status effects |
  | Crust Crusader | 300 | Crit | 15% base crit chance, 2x crit multiplier |
  | Dough Exorcist | 340 | Execute | 2x damage vs targets below 20% HP, 1.5x vs elite/boss |
  | High Prophet of Sourdough | 400 | Global aura | +2% global damage per friendly tower on the map (includes self). Recalculated every 0.5s |

- **Placement**: towers can only be placed on designated buildable tiles. Already-occupied tiles are blocked.
- **Selection**: clicking a placed tower selects it. A semi-transparent circle shows its range. Right-click deselects.
- **Upgrades**: each tower has a fixed upgrade path (2 tiers per tower). Upgrades increase damage, fire rate, and/or range.
- **Targeting priority**: each tower can be set to target the first enemy in range, the closest, the strongest (most health), or the most recent.
- **EffectiveDamage formula** (updated):
  ```
  EffectiveDamage = (baseDamage + upgradeBonus + ancientStarterStacks + firstStarterBonus)
    * (1 + (synergyPercent + equipPercent + auraPercent + globalAuraPercent + trinketPercent) * buffMultiplier)
    * (1 + shopPercent)
    * (1 + metaPercent)
  ```
  Where `buffMultiplier = 0.5` if affected by Gluten Null Bishop's anti-buff aura.

### Special Mechanics

- **Aura (Bread Monk)**: scans all towers in group every 0.5s. Applies static-registry buff by tower reference. Removes buff when tower leaves range or is removed.
- **Global Aura (High Prophet)**: counts towers in `towers` group every 0.5s. Sets `RunState.GlobalAuraDamagePercent = count * damagePerTower`.
- **Chain (Fermentation Sage)**: on hit, finds nearest enemy within bounce range using physics shape query. Applies damage and propagates status effects. Synergy Holy Fermentation Network increases bounce range by 30%.
- **Crit (Crust Crusader)**: random roll in AttackComponent.Fire(). If crit, damage *= critMultiplier. Equipment and trinkets can modify chance and multiplier.
- **Execute (Dough Exorcist)**: if target HP < threshold, damage *= executeMultiplier. If target IsBoss/IsHeavy, damage *= eliteBonusMultiplier.
- **Synergy Crust Judgment Protocol**: on crit against target <50% HP, instant kill (9999 damage). 10s cooldown per tower.
- **Anti-Buff**: Gluten Null Bishop applies 50% debuff to all multiplicative bonuses (synergy + equip + aura + globalAura + trinket). Implemented via static `Tower._antiBuffCount` dictionary, scanned every 0.5s.
- **Ancient Starter**: `_attackCount` tracked per tower instance and persisted in RunState across fights. Every 10 attacks, +1 raw damage (stacking).
- **Messenger Crate**: Projectile.PierceCount = 1. On hit, decrements pierce and finds next target via physics shape query. Homing continues on new target.
- **Double Sampling**: after firing, scans 30px radius for nearest enemy and applies poison directly (no projectile).
- **Blessed Crunch Seal**: on crit hit, triggers TriggerSplashAt with 30px radius. Uses same splash system as Bakery Truck.
- **Judgment Seal**: if target <15% HP and cooldown <= 0, instakill with 9999 damage. 5s cooldown (reuses `_judgmentCooldown` field that also serves Crust Judgment Protocol synergy).
- **First Starter Relic**: GetFirstStarterBonus() scans towers group, counts those within 40px, returns `_data.Damage * (0.05 * count)` as raw damage.
- **Prayer Beads**: `_auraOnly = true` means Tower._Process skips attack. Aura DamageBonusPercent and FireRateBonusPercent are multiplied by 1.05 in ApplyData.

---

## Projectiles

- Projectiles are homing — they track and follow their target automatically.
- On hit: the projectile deals damage, applies the tower's special effect (if any), and disappears.
- If the target enemy dies or reaches the end while the projectile is in flight, the projectile disappears without hitting anything.
- Piercing projectiles (Messenger Crate) re-acquire a new target after the first hit using a physics shape query.
- Each projectile stores a `WasCrit` flag for equipment effects that depend on crit (Blessed Crunch Seal).

---

## Enemies

- **10 enemy types**:

  | # | Name | HP | Speed | Gold | Damage | Boss? | Special |
  |---|---|---|---|---|---|---|---|
  | 1 | Sliced Bread Tourist | 100 | 24 | 10 | 1 | — | Basic |
  | 2 | Grocery Run Jogger | 60 | 48 | 15 | 1 | — | Fast, fragile |
  | 3 | Lazy Alley Cat | 300 | 12 | 30 | 2 | — | Early tank |
  | 4 | Pigeon w/ Stolen Baguette | 80 | 32 | 20 | 1 | — | Annoying flyer |
  | 5 | Industrial Bread Dragon | 1000 | 16 | 100 | 5 | **BOSS** | Final boss |
  | 6 | Microwave Meal Preacher | 200 | 20 | 25 | 2 | — | Medium "convinced" |
  | 7 | Plastic Wrapped Sandwich Man | 180 | 28 | 22 | 2 | — | Medium resistance |
  | 8 | Frozen Dough Abomination | 400 | 10 | 35 | 3 | — | Slow tank |
  | 9 | The Gluten Null Bishop | 1500 | 14 | 120 | 6 | **BOSS** | Anti-buff aura (60px, 50% debuff to all tower percent bonuses) |
  | 10 | Supermarket Overlord of White Bread | 900 | 18 | 80 | 4 | — | Elite/miniboss |

- Each enemy has configurable health, speed, gold reward, sprite, damage to player, and IsBoss/IsHeavy flags via EnemyData resource.
- Enemies follow a fixed path defined per level.
- If an enemy reaches the end, it damages the player's lives and is removed.
- Each enemy displays:
  - A **health bar** above it (green/red) that updates in real time.
  - **Damage numbers** that float up when the enemy takes damage.
- Status effects are visible as sprite tints (green for poison, blue for slow).
- **Gluten Null Bishop**: HasAntiBuffAura flag triggers 0.5s scan of nearby towers. Towers within AntiBuffAuraRadius (60px) get a 50% multiplier on all percent-based bonuses (synergy, equip, aura, globalAura, trinket). Debuff removed when enemy leaves range or dies.
- **Supermarket Overlord**: not a boss but has mini-boss stats. Can be used as the slot machine miniboss.

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

- Displays current **lives** (hearts + count), **money**, and **wave number**.
- **Life counter**: red `ColorRect` hearts (4×4px each) in the InfoBar, rebuilt on `LivesChanged`. Followed by the numeric lives count.
- **Tooltip panel**: `Label` with `StyleBoxFlat` background (dark + border), positioned near the mouse cursor. Appears below cursor in top half, above cursor in bottom half. Used for:
  - **Tower bar buttons**: tooltip shows tower name, cost, damage, fire rate, range (on hover)
  - **Synergy label**: tooltip shows active synergy names (on hover)
  - **Buff icons**: tooltip shows purchased shop item, trinket, or synergy description
  - **Equip label**: tooltip shows equipment name + description (on hover)
- **Buff icons area**: `HBoxContainer` at top-left (y=42) displaying 16×16 icons for:
  - **Purchased shop items** (from `RunState.PurchasedItemIcons`)
  - **Active trinket** (from `TrinketData.Icon`)
  - **Active synergies** (from `SynergyData.Icon`)
  - Each icon shows a tooltip on hover with name and description.
- Shows a button for each available tower type (determined by loadout during runs). Towers are loaded dynamically from `resources/tower_data/` directory — no scene changes needed when adding new towers.
- Buttons are disabled when the player cannot afford the tower, when that type is already placed, or during Game Over.
- Buttons update reactively when money changes, towers are placed, or towers are deselected.
- **"Next Wave"** button to start each wave. **"Next Level"** button after all waves are cleared.
- When a tower is selected: shows its **name**, **upgrade level**, **upgrade cost** (or "MAX" if fully upgraded), **targeting priority** (click to cycle First → Closest → Strongest → Last), and **equipped item**.

---

## Screens & Menus

| Screen | When shown | Can pause |
|---|---|---|
| Main Menu | On startup | N/A |
| Loadout | After clicking "Start Run" on Main Menu | No |
| Pause | Pressing ESC during gameplay | Yes |
| Game Over | Lives reach zero | Yes |
| Victory | All levels completed (classic mode) or boss defeated (run mode) | Yes |
| Fight Complete | All waves cleared during a run | Yes |
| Shop | Slot machine outcome — buy run upgrades and equipment | Yes |
| Meta Shop | Main Menu — permanent upgrades with meta-tokens | Yes |
| Trinket Choice | Slot machine Treasure outcome — pick 1 of 3 trinkets | Yes |
| Briefing | Before every fight in a run — map preview, loadout reminder, tier, synergies | No |
| Bestiary | Main Menu button or Pause screen — browse towers/enemies/equip/trinkets/synergies | Yes |

- All pause-capable screens work while the game is paused and can be dismissed with ESC.

### Briefing Screen

Shown before every fight in a run. Displays:

- **Map preview**: `LevelData.PreviewTexture` thumbnail (90×54) showing the level terrain and layout.
- **Title**: Level name (or "BOSS FIGHT!" for boss encounters).
- **Gold / Lives**: current resources.
- **Difficulty tier**: "Tier 1" (green), "Tier 2" (yellow), or "Tier 3" (red) based on `RunState.GetWaveTier()` — hidden during boss fights.
- **Miniboss indicator**: red "MINIBOSS" label when `RunState.IsMiniboss` is true.
- **Wave list**: per-wave enemy composition from `PendingRunWaves` (run mode) or `LevelData.Waves` (free play). Shows total wave count, then per-wave format: `W1: 3x Sliced Bread Tourist 2x Pigeon · Horde` — modifier tag appears when `WaveModifier != None`. Scrollable if waves exceed visible area.
- **Loadout icons**: 16×16 sprite for each selected tower (`RunState.SelectedTowerIds`) — fallback to first-letter label when sprite is missing.
- **Synergy preview**: if the selected towers activate any synergies, shows synergy names with DMG bonus.
- **Start animation**: brief fade-in (0.3s), then button text changes to "START!" with a yellow pulse loop.
- Dismissible with ESC (same as other pause-capable screens).

---

## Bestiary / Discovery

- **Bestiary screen**: accessible from Main Menu (button) and Pause screen (`PauseGame=true` overlay). Shows 5 categories: Towers, Enemies, Equipment, Trinkets, Synergies.
- **Visual Polish & Detail Views**: Each category has been upgraded with immersive visual elements and a collapsible accordion details view:
  - **Category Progress Tracking**: Displays discovery progress at the top of the screen (e.g., "Found: 3 / 5") for the active category.
  - **Sprites & Icons**: Shows high-quality sprites (towers and enemies) or icons (equipment, trinkets, synergies) next to each discovered entry. Locked/undiscovered entries show dimmed silhouettes or "???" placeholding text.
  - **Interactive Accordions**: Clicking any discovered entry smoothly expands or collapses a detailed submenu with full description, specialized mechanics, and lore.
  - **Normalized Stat Bars**: Detail views for Towers and Enemies feature visual attribute bars (Damage, Fire Rate, Range, HP, Speed, Reward Gold) normalized automatically using the maximum value in that class across all loaded resource data.
  - **Lore / Flavor Text**: Features optional immersive narrative text (`FlavorText` property on all resources) giving depth to towers, enemies, equipment, trinkets, and synergies.
- **All data is scanned from directories** — zero-code-change to add entries.
- **Discovery tracking**: items are hidden (grayed out with `???`) until first encountered:
  - Enemies: discovered when they spawn in a fight
  - Equipment: discovered when seen in the Shop
  - Trinkets: discovered when shown in Treasure choice
  - Synergies: discovered when activated (required towers placed)
  - Towers: use existing unlock system (meta-progression + default unlocks)
- **Data persists** across runs in `SaveManager` JSON (`discovered` dict).

---

## Configurability (No-Code Changes)

The following can be modified by editing resource files (`.tres`) in the project's `resources/` folder — no programming required:

| What | File type | Examples |
|---|---|---|
| Tower stats | `TowerData` | Damage, fire rate, range, cost, splash radius, poison/slow properties, aura, chain, crit, execute, globalAura |
| Upgrade tiers | `UpgradeData` | Cost, damage/fire rate/range bonus |
| Enemy stats | `EnemyData` | Health, speed, gold reward, damage to player, IsBoss, IsHeavy, HasAntiBuffAura |
| Level setup | `LevelData` | Scene path, preview image, starting money/lives, world size |
| Screen/overlay config | `UIScreenData` | Scene path, whether it pauses the game |
| Synergy combos | `SynergyData` | Required tower IDs, min tower count, bonus percentages (damage/fire rate/range) |
| Run engine (slot machine) | `SlotManager` Inspector | Fight count per run, outcome weights, heal amount, miniboss multiplier, reroll cost, skew factor |
| Shop items (run upgrades) | `ShopItemData` resource | Item ID, name, cost, stat bonus percent, heavy damage bonus, first-purchase discount |
| Tower equipment | `EquipData` resource | Item ID, name, cost, target tower type, stat percent bonuses |
| Trinkets | `TrinketData` resource | Item ID, name, rarity (Common/Rare), damage/fire rate/range/crit damage/status duration/status strength percent bonuses, heal/gold amount, passive gold interval |
| Status effects | `PoisonEffectData`, `SlowEffectData` | Duration, damage per tick, speed multiplier |
| Wave tiers (run mode) | directory `wave_data/tier1/`, `tier2/`, `tier3/` | Add `.tres` files to a tier folder — selected by `FightsCompleted`. 5 waves per tier (15 total) |
| Wave entries (per-type counts) | `WaveEntry` sub-resource inside each `WaveData` | Enemy type + count per entry |
| Wave modifiers | `WaveModifier` enum on `WaveData` | `None`, `Horde` (2× enemies, 0.5× interval), `Armored` (2× HP), `Swift` (1.5× speed), `GoldRush` (2× gold) |
| Wave generation (run mode) | `RunState.PickRunWaves()` | 5-10 waves per fight, `DifficultyMultiplier` 0.6x-1.4x scaling, final stretch (last 3 waves ×1.5 count), random modifiers on waves 3+ |
| Difficulty scaling (runtime) | `WaveData.DifficultyMultiplier` / `WaveData.IsFinalStretch` | Non-exported fields set on clone per fight. HP × mult, spawn ÷√mult, count ×1.5 on final stretch |
| Bestiary discovery | `SaveManager` JSON | Towers/enemies/equip/trinkets/synergies auto-discovered when first encountered |
| Meta-progression | `MetaUpgradeData` resource | Item ID, name, cost tokens, max level, IsTowerUnlock, TowerId, StatType, BonusPerLevel, Category (Unlocks/Stats/Economy) |
