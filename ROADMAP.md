# Roadmap

Reste do que falta implementar. Todos os items concluídos foram removidos
— os seus outcomes estão descritos em GAME_STATUS.md.

## Camada de Fundação — Estabilidade e Loop de Jogo

Antes de qualquer polimento visual, é necessário garantir que o jogo é jogável
do início ao fim sem crashes, softlocks, ou balanceamento quebrado. Esta camada
fecha o loop de jogo para uma demo de ~5 horas.

---

## Camada de Polimento — Improvements to Existing Features

Each item below builds on existing features and adds the visual, interactive,
and depth polish that a real game needs. Only after the foundation layer is solid.

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

### 9. Tutorial / Onboarding

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

### 10. Game Assets (16x16 Sprites)

**Why**: 5 new towers and 5 new enemies use placeholder sprites. Equipment (20), trinkets (10), shop items (5), synergies (4), and projectile variants all have no unique icons. The game has no visual identity — every tower looks the same, no item has an icon.

**What to do**:
- Create all missing sprites at 16x16 resolution (pixel art style)
- Add `Icon` (16x16) and `Sprite` (16x16) fields to each `.tres` resource pointing to the new files
- Existing sprites can be kept or replaced for consistency

**Estimate**: 5-8 days with an artist

---

### 11. Sound System (deferred — requires audio assets)

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

| # | Item | Layer | Impact | Effort | Dependencies |
|---|---|---|---|---|---|
| 1 | Playtest + Fix Softlocks | Fundação | Critical | 1d | None |
| 2 | Balance Pass | Fundação | Critical | 1-2d | #1 |
| 3 | Game Over + Victory Flows | Fundação | High | 0.5d | #1 |
| 4 | Custom Sprites | Polimento | Critical | 5-8d | None |
| 5 | Enemy/Projectile VFX | Polimento | High | 3-5d | None |
| 6 | UI Transitions | Polimento | Medium | 2-3d | None |
| 7 | Slot Machine Animation | Polimento | Medium | 3-4d | #6 |
| 8 | Tutorial/Onboarding | Polimento | Low | 1-2d | None |
| 9 | Sound System | Polimento | Critical | 2-3d | None |
