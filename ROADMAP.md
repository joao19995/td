# Roadmap

MVP feature list, in a sensible build order based on dependencies. Items that are
finished have been removed — their outcomes are described in GAME_STATUS.md.

**Global constraints:**
- 10 tower types, fixed (Bread Baker, Bread Courier, Aroma Keeper, Taste Tester,
  Bakery Truck, Bread Monk, Fermentation Sage, Crust Crusader, Dough Exorcist,
  High Prophet of Sourdough)
- Each map: max 1 tower of each type = max 10 towers per map (practical: 5 max per map)
- Loadout: max 4 tower types per run
- Towers are placed fresh each fight (no persistence of positions between fights)
- Runs cycle randomly between Map1 and Map2

---

## Content Completion — Missing Items

### 1. Game Assets (16x16 Sprites)

**Why**: 5 new towers and 5 new enemies use placeholder sprites. Equipment (20), trinkets (10), shop items (5), synergies (4), and projectile variants all have no unique icons. The game has no visual identity — every tower looks the same, no item has an icon.

**Folder structure** (16x16 PNG, pixel art):

```
assets/sprites/
├── towers/        (10 sprites — one per tower type)
│   ├── bread_baker.png
│   ├── bread_courier.png
│   ├── aroma_keeper.png
│   ├── taste_tester.png
│   ├── bakery_truck.png
│   ├── bread_monk.png
│   ├── fermentation_sage.png
│   ├── crust_crusader.png
│   ├── dough_exorcist.png
│   └── high_prophet.png
├── enemies/        (10 sprites — one per enemy type)
│   ├── sliced_bread_tourist.png
│   ├── grocery_jogger.png
│   ├── alley_cat.png
│   ├── baguette_pigeon.png
│   ├── bread_dragon.png
│   ├── microwave_preacher.png
│   ├── sandwich_man.png
│   ├── frozen_dough_abomination.png
│   ├── gluten_null_bishop.png
│   └── supermarket_overlord.png
├── projectiles/    (10 sprites — one per tower type)
│   ├── proj_bread_baker.png
│   ├── proj_bread_courier.png
│   ├── proj_aroma_keeper.png
│   ├── proj_taste_tester.png
│   ├── proj_bakery_truck.png
│   ├── proj_bread_monk.png
│   ├── proj_fermentation_sage.png
│   ├── proj_crust_crusader.png
│   ├── proj_dough_exorcist.png
│   └── proj_high_prophet.png
├── equipment/      (20 icons — one per equip item)
│   ├── stone_oven.png
│   ├── ancient_starter.png
│   ├── electric_bike.png
│   ├── messenger_crate.png
│   ├── megaphone.png
│   ├── spice_wind_chimes.png
│   ├── silver_tray.png
│   ├── double_sampling.png
│   ├── reinforced_suspension.png
│   ├── street_parade.png
│   ├── sacred_robes.png
│   ├── prayer_beads.png
│   ├── golden_proofing_bowl.png
│   ├── wild_yeast.png
│   ├── tempered_crust_blade.png
│   ├── blessed_crunch_seal.png
│   ├── holy_flour_pouch.png
│   ├── judgment_seal.png
│   ├── golden_staff.png
│   └── first_starter_relic.png
├── trinkets/       (10 icons — one per trinket)
│   ├── secret_recipe_scroll.png
│   ├── starters_blessing.png
│   ├── tip_jar.png
│   ├── proofing_time_candle.png
│   ├── crust_fragment_relic.png
│   ├── fermentation_diary.png
│   ├── sacred_flour_dust.png
│   ├── heretic_census.png
│   ├── oven_heart_ember.png
│   └── first_starter_vessel.png
├── shop_items/     (5 icons — one per shop item)
│   ├── secret_ingredient.png
│   ├── fresh_batch.png
│   ├── golden_proof_flour.png
│   ├── rapid_oven_upgrade.png
│   └── discounted_starter_yeast.png
├── synergies/      (4 icons — one per synergy)
│   ├── one_whiff_one_bite.png
│   ├── grand_opening_rush.png
│   ├── holy_fermentation_network.png
│   └── crust_judgment_protocol.png
├── ui/             (UI elements — hearts, coin, crosshair, etc.)
│   ├── heart_full.png
│   ├── heart_empty.png
│   ├── coin.png
│   ├── crosshair.png
│   ├── arrow.png
│   ├── star.png
│   └── sword.png
└── environment/    (tiles, ground)
    ├── grass_01.png
    ├── tile_ground.png
    └── tile_stone.png
```

**What to do**:
- Create all missing sprites at 16x16 resolution (pixel art style)
- Add `Icon` (16x16) and `Sprite` (16x16) fields to each `.tres` resource pointing to the new files
- Existing 5 tower sprites can be kept or replaced for consistency
- Existing 5 enemy sprites can be kept or replaced for consistency
- Existing 5 projectile sprites can be kept or replaced for consistency

**Estimate**: 5-8 days with an artist

---

### 2. Golden Proof Flour (Shop Item #3) — Damage Not Applied

**Why**: `ShopHeavyDamageBonusPercent` is stored on purchase but never read in the damage formula. The item costs 90g and does nothing.

**Status**: Fixed — `AttackComponent.Fire()` now applies `damage *= 1f + shopHeavyPct` when target is boss or heavy.

---

### 3. Heretic Census List (Trinket #8) — Wrong Damage Application

**Why**: The trinket applied flat +10% damage to all enemies instead of only basic (non-boss, non-heavy) enemies. `DamagePercentBonus` has been removed from the `.tres`; `IsBasicEnemy` flag replaced by `!IsBoss && !IsHeavy` check.

**Status**: Fixed — `AttackComponent.Fire()` applies 1.1x multiplier when `HasHereticCensus` is true and target is neither boss nor heavy.

---

### 4. Oven Heart Ember (Trinket #9) — Wrong Range Bonus

**Why**: The trinket applied +2% range instead of +1 flat range. `RangePercentBonus` removed from `.tres`; `TrinketRangeFlatBonus = 1f` stored in RunState.

**Status**: Fixed — `Tower.EffectiveRange` adds `flatBonus` to base range.

---

## Camada de Polimento — Improvements to Existing Features

Each item below builds on existing features and adds the visual, interactive,
and depth polish that a real game needs.

Suggested execution order (lowest dependencies, maximum impact per effort).

---

### 1. Sound System

**Why**: the game has no sound at all — the most noticeable gap. Without audio,
the game feels dead regardless of visual polish.

**What to build**:
- `SoundManager` autoload with SFX and Music buses, `PlaySFX()` and `PlayMusic()`
  categorized, persistent volume in SaveManager JSON
- Pool of `AudioStreamPlayer` for overlapping SFX
- Sound events: button click, purchase, slot machine, projectile hit, enemy death,
  tower place/upgrade, wave complete, game over, victory

**Decisions**:
- Placeholder audio assets go to `assets/audio/sfx/` and `assets/audio/music/`
- Use `.ogg` streams for short SFX
- `SoundManager.Instance.PlaySFX("res://assets/audio/sfx/click.ogg")` — direct path,
  no resource wrapper

**Estimate**: 2-3 days with placeholder assets

---

### 2. Shop — Expansion + Visual Feedback

**Why**: current shop has basic text items, no visible descriptions, no icons,
no purchase feedback.

**What to build**:
- **Icons on items**: `ShopItemData.Icon` exported and displayed in UI
- **Visible description**: `Description` field shown below the name
- **In-level buff icon**: when buying an item, a persistent icon appears in the
  HUD corner for the rest of the run (e.g. golden bread roll for damage boost)
- **Tooltip on icon**: hovering the icon shows "Secret Ingredient: +5% damage (run-wide)"
- **1-per-item enforcement**: already exists (button becomes "Owned"), but with
  clearer visual feedback (checkmark, dimming, different color)
- **Purchase feedback**: brief animation/pulse on the icon when bought

**Decisions**:
- Run-wide item icons are 2D sprites in the HUD, added by `HUD.AddRunBuffIcon()`
- Tooltips use `Control` with `mouse_entered`/`mouse_exited` + `Control.show()`/`hide()`
- New items are `.tres` files — zero-code-change to add

**Estimate**: 2-3 days

---

### 3. Tower Equipment — Visual Feedback

**Why**: 20 equipment items exist but no visible icons, no indicator on the tower.

**What to build**:
- **Icon in TowerData**: `EquipData.Icon` already exists, show it in the shop and
  on the selected tower panel
- **Visual indicator on tower**: when equipped, the tower shows a small icon/glow
  over its sprite
- **Tooltip in HUD**: hovering the equip label shows stats
- **Swap equipment**: button in the shop to swap existing equipment (costs gold,
  loses the previous one)
- **Description in shop**: `Description` field displayed

**Decisions**:
- Tower visual indicator = `Sprite2D` child added in `Tower.SetEquippedItem()`
- Swap destroys the previous equip (no refund for simplicity)

**Estimate**: 2-3 days

---



### 4. Trinkets — UI Card + Visual Polish

**Why**: 10 trinkets exist but the UI is basic with no icons or card presentation.

**What to build**:
- **UI card-based**: each trinket presented as a card with icon, name, description,
  border with rarity color
- **Choice animation**: fade-in of cards, hover highlight, brief animation on "Take"
- **Skip option**: "Skip" button if no trinket interests the player
- **Rarity**: common (1 effect) and rare (2 effects)

**Decisions**:
- Cards = `PanelContainer` with `TextureRect` + `Label` children, instantiated
  from a `TrinketCard.tscn` scene
- Rarity = `Rarity` field (enum: Common/Uncommon/Rare) on `TrinketData`
- Skip = advances without applying a trinket (RunState gets no bonus)

**Estimate**: 2-3 days

---

### 5. Meta-Progression — Expansion

**Why**: 10 upgrades (5 base + 5 tower unlocks) is still slim for long-term progression.

**What to build**:
- **Catalog expansion**: 15-20 upgrades total:
  - Tower unlocks (existing: 2 default + 8 purchasable)
  - Stat upgrades: damage, fire rate, range, starting gold (existing)
  - "Starting lives +2 per level"
  - "Shop discount 5% per level"
  - "Reroll cost -10% per level"
  - "Start with 1 random equipment" (unlock)
  - "Start towers at level 1" (unlock)
  - "Enemy gold bonus +10% per level"
- **Token reward scaling**: tokens = base × (1 + fightsCompleted / totalFights)
  — bigger reward for longer runs
- **Victory bonus**: +50% tokens if run beats the boss
- **UI categories**: tabs for "Unlocks", "Stats", "Economy"

**Decisions**:
- New upgrades = `.tres` files — zero-code-change
- Cost scaling remains `CostTokens × (level + 1)` — simple, consistent
- Token scaling is a simple formula, no external config (can be exported later)

**Estimate**: 2-3 days

---

### 6. HUD — Tooltips + Run Buff Icons

**Why**: HUD shows raw text without tooltips or visual representation of active bonuses.

**What to build**:
- **Universal tooltips**: `Control.tooltip_text` or custom popup on:
  - Tower bar buttons (shows cost, damage, fire rate, range)
  - Tower action panel (shows detailed stats + equip description)
  - Synergy label (shows which towers trigger each synergy)
  - Run buff icons (shows shop/trinket item description)
- **Run buff icons area**: HUD area showing icons of:
  - Purchased shop items (with tooltip)
  - Active trinket (with tooltip)
  - Active synergies (separate from label text)
- **Life counter**: show lives as hearts instead of "Lives: 5" text

**Decisions**:
- Tooltips = `Panel` child of HUD, show/hide on mouse enter/exit
- Buff icons = `TextureRect` with `mouse_filter = Ignore` + tooltip control
- Hearts = `TextureRect` array, updated on `LivesChanged`

**Estimate**: 2-3 days

---

### 7. Bestiary — Sprites + Stats + Lore

**Why**: bestiary currently shows only text, no sprites, no lore.

**What to build**:
- **Sprite display**: each entry shows the tower/enemy/equip/trinket sprite
- **Stat bars**: visual bars for main stats (damage, speed, range, HP)
- **Lore/flavor text**: `FlavorText` field on Resources, displayed in bestiary
- **Progress tracking**: "3/5 Towers", "4/5 Enemies" at the top
- **Detail view**: clicking an entry expands to a detailed view with all stats

**Decisions**:
- `FlavorText` added to `TowerData`, `EnemyData`, `EquipData`, `TrinketData`,
  `SynergyData` — optional field, no save-breaking changes
- Stat bars = `ColorRect` with width proportional to value / max in category

**Estimate**: 2-3 days

---

### 8. Briefing — Map Preview + Loadout Reminder

**Why**: briefing shows only text, no map preview or chosen towers.

**What to build**:
- **Map preview**: `LevelData.PreviewTexture` displayed as thumbnail
- **Loadout reminder**: shows names/icons of the 1-4 selected towers
- **Synergies preview**: if selected towers activate synergies, shows
  "Active synergies: One Whiff, One Bite (+15% dmg)"
- **Difficulty indicator**: tier label (tier1/tier2/tier3) with color (green/yellow/red)
- **Miniboss indicator**: when fighting a miniboss, shows "MINIBOSS" with danger icon
- **Start animation**: brief fade-in, "START!" pulse before closing

**Estimate**: 1-2 days

---

### 9. Enemy/Projectile VFX

**Why**: enemies spawn and die instantly with no visual feedback.

**What to build**:
- **Spawn animation**: fade-in or dust poof on enemies when appearing
- **Death animation**: particle burst, fade-out, or "splat" sprite
- **Hit-flash**: sprite turns white for 1 frame when hit
- **Damage number colors**: normal dmg = white, crit = red, poison = green
- **Projectile impact**: small particle burst at projectile impact point
- **Tower fire effect**: muzzle flash sprite on tower when firing
- **Status effect particles**: green bubbles (poison), blue crystals (slow)

**Decisions**:
- Effects go to `effects/` — `SpawnEffect`, `DeathEffect`, `HitFlash`, `MuzzleFlash`
- Use `GPUParticles2D` or `AnimatedSprite2D` depending on complexity
- PoolManager for high-frequency effects (hit-flash, muzzle flash)

**Estimate**: 3-5 days

---

### 10. Slot Machine — Animation + State-Aware Weights

**Why**: current slot is text "Next: FIGHT" — no emotion, no visual feedback.

**What to build**:
- **Slot animation**: icons spinning (like a real slot machine) for 1-2s,
  then stop on result with bounce
- **Outcome reveal**: result highlighted with color, border glow, themed icon
- **State-aware weights**: weights adjust based on state:
  - Low lives (<30%) → Heal weight +50%
  - High gold (>200) → Shop weight +30%
  - Already had Treasure this run → Treasure weight -30%
- **Pity timer**: guarantees rare outcomes (Treasure, Miniboss) appear at least
  once every N outcomes
- **Reroll cost preview**: shows reroll cost before clicking
- **Outcome history**: small indicator of the last 3 outcomes

**Decisions**:
- Slot animation = `AnimationPlayer` with sprite frames or `TextureRect` rotation
- State-aware weights = `SlotManager.GetDynamicWeights()` that modifies base weights
- Pity timer = counters per outcome on `SlotManager`, reset after appearing

**Estimate**: 3-4 days

---

### 11. UI Transitions + Loading Screen

**Why**: all screen transitions are instant — no fade, no slide.

**What to build**:
- **Fade-in/fade-out**: all screen pushes/pops do crossfade (0.2s)
- **Easing config**: `UIScreenData` gains `TransitionIn`/`TransitionOut` fields
  (Fade/SlideLeft/SlideRight/None) and `TransitionDuration`
- **Loading screen**: "Loading..." screen with spinning icon during level load
  (currently instant, but scales to larger levels)
- **Screen lifecycle callbacks**: `OnShown()` / `OnHidden()` on screens
- **Backdrop dim**: overlay screens darken the background with semi-transparent `ColorRect`
- **Pop guard**: `PushScreen` checks if screen is already on the stack (prevents duplicates)

**Decisions**:
- Transitions = `Tween` on `CanvasLayer` `Modulate` (0→1 fade-in, 1→0 fade-out)
- Loading screen = separate `CanvasLayer`, not on the normal stack

**Estimate**: 2-3 days

---

### 12. Waves — Elite Enemies + Modifiers

**Why**: waves are always the same — same enemy queues, no variation.

**What to build**:
- **Elite/champion enemies**: random enemy in wave gets 2x HP, 1.5x damage,
  2x gold reward, golden glow, star on health bar
- **Wave modifiers**: `WaveData` gets optional `Modifier` field (enum):
  - `None`: normal
  - `Horde`: 2x enemies, 0.5x spawn interval
  - `Armored`: all enemies 2x HP
  - `Swift`: all enemies 1.5x speed
  - `GoldRush`: enemies grant 2x gold
- **Elite spawn**: configurable chance on `EnemySpawner.EliteChance` (default 20%)
- **More wave variety**: +3 waves per tier (total 5 per tier = 15 waves)

**Decisions**:
- Elite = new flag on `Enemy`, stats multiplied in `Initialize()`
- Modifier = `WaveModifier` enum field on `WaveData`, read by `EnemySpawner`
- New waves = `.tres` files — zero-code-change

**Estimate**: 2-3 days

---

### 13. Tutorial / Onboarding

**Why**: new player has no guidance at all.

**What to build**:
- **Brief tutorial** on first game (check flag in SaveManager):
  - "Click a tower button to place it on a buildable tile"
  - "Click a placed tower to see its stats"
  - "Click Next Wave to start"
  - "Enemies reaching the end cost you lives"
  - "Kill all enemies to complete the wave"
- **Highlight overlay**: points arrows/glow to relevant HUD elements
- **Dismiss**: click anywhere to close each tip
- **Replayable**: "Show Tutorial" option on Main Menu

**Decisions**:
- Tutorial = `CanvasLayer` overlay with `Control` + `Label` + `TextureRect` (arrow)
- State = `_tutorialCompleted` bool in SaveManager JSON

**Estimate**: 1-2 days

---

### 14. Targeting Priority UI + Strategy Change

**Why**: targeting strategy is only configurable in the Inspector — inaccessible in-game.

**What to build**:
- "Target" button on the selected tower panel
- Click cycles between: First, Closest, Strongest, Last
- Visual indicator on the panel (e.g. "First → Closest" text changes)
- `TargetingComponent.Strategy` set at runtime, not just in `_Ready`
- Tooltip explains each strategy

**Decisions**:
- Strategy persists only for that instance (not per-type)
- Reset to default when tower is sold and re-placed

**Estimate**: 1 day

---

### 15. Loadout — Stat Preview + Synergy Hints

**Why**: loadout shows only names and costs — no stats, no synergy hints.

**What to build**:
- **Stat preview**: clicking a tower in the loadout shows damage, fire rate, range,
  special ability (slow/poison/splash/aura/chain/crit/execute/global-aura)
- **Synergy hints**: when 2+ towers that activate a synergy are selected, shows
  "Synergy: One Whiff, One Bite (+15% to both)"
- **Tower sprite**: shows the tower sprite next to the name
- **Random loadout button**: "Random" selects 4 random towers
- **Loadout save**: 3 loadout slots saved in SaveManager JSON

**Estimate**: 2-3 days

---

## Recommended Priority

| # | Item | Impact | Effort | Dependencies |
|---|---|---|---|---|---|
| 1 | Custom Sprites (towers + enemies) | Critical | 3-5d | None |
| 2 | Sound System | Critical | 2-3d | None |
| 3 | Shop + Level Icons | High | 2-3d | None |
| 4 | HUD Tooltips + Buff Icons | High | 2-3d | Shop (#3) |
| 5 | Enemy/Projectile VFX | High | 3-5d | None |
| 6 | Tower Equipment Visuals | Medium | 2-3d | Shop (#3) |
| 7 | Trinkets Cards | Medium | 2-3d | None |
| 8 | UI Transitions | Medium | 2-3d | None |
| 9 | Meta-Progression Expansion | Medium | 2-3d | None |
| 10 | Slot Machine Animation | Medium | 3-4d | UI Transitions (#8) |
| 11 | Briefing + Preview | Medium | 1-2d | None |
| 12 | Bestiary Sprites + Lore | Medium | 2-3d | None |
| 13 | Waves + Elites | Low | 2-3d | None |
| 14 | Targeted Priority UI | Low | 1d | None |
| 15 | Loadout Preview | Low | 2-3d | None |
| 16 | Tutorial/Onboarding | Low | 1-2d | None |
