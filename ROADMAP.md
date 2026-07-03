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

## Camada de Polimento — Improvements to Existing Features

Each item below builds on existing features and adds the visual, interactive,
and depth polish that a real game needs.

Suggested execution order (lowest dependencies, maximum impact per effort).

---


### 1. [DONE] Trinkets — UI Card + Visual Polish

Cards with icons/name/description/rarity-colored border, fade-in/hover animation,
Skip button. Outcome documented in GAME_STATUS.md.

---

### 2. [DONE] Meta-Progression — Expansion

16 upgrades (10 new): Starting Lives, Shop Discount, Reroll Cost, Start Equip, Start Tower Lv1, Enemy Gold Bonus.
Token reward scaling with victory bonus. UI tabs (All/Unlocks/Stats/Economy). Outcome documented in GAME_STATUS.md.

---

### 3. [DONE] HUD — Tooltips + Run Buff Icons

Tooltip panel (segue o rato), tooltips em tower buttons e synergy label, life counter com
corações (ColorRect 4×4), buff icons expandidos (shop + trinket + synergy icons). 
Outcome documented in GAME_STATUS.md.

---

### 4. [DONE] Bestiary — Sprites + Stats + Lore

Sprites e ícones nos entries, barras de stats normalizadas por categoria, progresso das
categorias no topo, accordion detail views expansíveis/colapsáveis com lore (`FlavorText` adicionado
a todos os dados de recurso) e mecânicas detalhadas. Outcome documented in GAME_STATUS.md.

---

### 5. [DONE] Briefing — Map Preview + Loadout Reminder

Map preview thumbnail, loadout icons, synergy preview, tier color label, miniboss indicator,
fade-in animation + "START!" pulse. Outcome documented in GAME_STATUS.md.

---

### 6. Enemy/Projectile VFX

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

### 7. Slot Machine — Animation + State-Aware Weights

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

### 8. UI Transitions + Loading Screen

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

### 9. [DONE] Waves — Elite Enemies + Modifiers + Per-Type Counts

WaveData refatorado: `Enemies`/`EnemyCount` substituído por `Entries` (Array de `WaveEntry`,
cada um com Enemy + Count). `WaveModifier` enum (Horde/Armored/Swift/GoldRush). Elite enemies
com 2× HP, 1.5× dano, 2× gold, spawn chance 20%. Enemy suporta multiplicadores separados
(HP/damage/gold) para modifiers. +3 waves (5 por tier = 15 total). Briefing mostra
quantidades por tipo. Outcome documented in GAME_STATUS.md.

---

### 10. Tutorial / Onboarding

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

### 11. [DONE] Targeting Priority UI + Strategy Change

"Target" label on tower panel, click to cycle First→Closest→Strongest→Last.
`TargetingComponent.Strategy` set at runtime via `_selectedTower` field + `GuiInput` handler.
Strategy persists per-instance, resets when tower is sold/re-placed.
Outcome documented in GAME_STATUS.md.

---

### 12. [DONE] Loadout — Stat Preview + Synergy Hints

Hover shows stat preview panel (name, sprite, DMG/SPD/RNG, special tags).
Synergy hints only for discovered synergies with unlocked required towers.
Random "RND" button (Fisher-Yates shuffle + `_isBatchUpdating` guard).
3 save slots in SaveManager JSON (left-click = load/save, right-click = overwrite).
Outcome documented in GAME_STATUS.md.

---

### 13. Game Assets (16x16 Sprites)

**Why**: 5 new towers and 5 new enemies use placeholder sprites. Equipment (20), trinkets (10), shop items (5), synergies (4), and projectile variants all have no unique icons. The game has no visual identity — every tower looks the same, no item has an icon.

**What to do**:
- Create all missing sprites at 16x16 resolution (pixel art style)
- Add `Icon` (16x16) and `Sprite` (16x16) fields to each `.tres` resource pointing to the new files
- Existing sprites can be kept or replaced for consistency

**Estimate**: 5-8 days with an artist

---

### 14. Sound System (deferred — requires audio assets)

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

## Recommended Priority

| # | Item | Impact | Effort | Dependencies |
|---|---|---|---|---|---|---|---|
| 1 | Custom Sprites (towers + enemies) | Critical | 3-5d | None |
| 2 | HUD Tooltips + Buff Icons | High | 2-3d | None |
| 3 | Enemy/Projectile VFX | High | 3-5d | None |
| 4 | UI Transitions | Medium | 2-3d | None |
| 5 | Slot Machine Animation | Medium | 3-4d | UI Transitions (#4) |
| 6 | Briefing + Preview | Medium | 1-2d | None |
| 7 | Bestiary Sprites + Lore | Medium | 2-3d | None |
| 8 | Waves + Elites | Low | 2-3d | None |
| 9 | Targeted Priority UI | Low | 1d | None |
| 10 | Loadout Preview | Low | 2-3d | None |
| 11 | Tutorial/Onboarding | Low | 1-2d | None |
| 12 | Sound System | Critical | 2-3d | None |
