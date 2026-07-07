# Roadmap

Reste do que falta implementar. Todos os items concluídos foram removidos
— os seus outcomes estão descritos em GAME_STATUS.md.

**Global constraints:**
- 10 tower types, fixed (Bread Baker, Bread Courier, Aroma Keeper, Taste Tester,
  Bakery Truck, Bread Monk, Fermentation Sage, Crust Crusader, Dough Exorcist,
  High Prophet of Sourdough)
- Each map: max 1 tower of each type = max 10 towers per map (practical: 5 max per map)
- Loadout: max 4 tower types per run
- Towers are placed fresh each fight (no persistence of positions between fights)
- Runs cycle randomly between Map1 and Map2

---

## Camada de Fundação — Estabilidade e Loop de Jogo

Antes de qualquer polimento visual, é necessário garantir que o jogo é jogável
do início ao fim sem crashes, softlocks, ou balanceamento quebrado. Esta camada
fecha o loop de jogo para uma demo de ~5 horas.

---

### 1. Playtest End-to-End + Fix de Softlocks

**Why**: nunca houve um playthrough completo. Flows críticos (Game Over, Shop sem
gold, reroll sem gold, briefing locked, tower placement sem dinheiro) podem estar
a produzir crashes ou dead-ends silenciosos.

**What to do**:
- Jogar uma run completa do início ao fim (Main Menu → Loadout → 3 fights →
  Boss → Victory) com logging nos flows críticos
- Jogar um cenário de derrota (perder todas as vidas a meio de uma fight)
- Testar edge cases:
  - Clicar "Next Wave" com 0 gold
  - Clicar "Reroll" com 0 gold
  - Clicar "Continue" sem ter feito o spin
  - Fechar menus com ESC em cada estado
  - Sair durante uma fight e voltar
  - Loadout com 1 única torre selecionada
  - Tentar colocar torre com gold insuficiente
  - Vender torre e colocar outra do mesmo tipo
- Corrigir todos os softlocks e crashes detetados

**Estimate**: 1 dia

---

### 2. Balance Pass (Damage / HP / Gold Economy)

**Why**: os números atuais de enemy HP, damage das torres, gold rewards e custos
nunca foram playtestados. Uma run com balanceamento errado ou morre na wave 3
ou acaba em 10 minutos sem decisões.

**What to do**:
- Definir target de run: ~20-30 min, 3 fights + boss, ~24 waves no total
- Ajustar valores base em `TowerData` e `EnemyData`:
  - Tower damage: quebra de HP de enemy médio em 3-5 hits (tower básica)
  - Enemy HP: escala por tier (tier1 fácil, tier3 duro)
  - Gold economy: torre mais cara comprável após 2-3 waves de farm
  - Upgrade costs: progression que justifica investir vs. colocar nova torre
- Validar com 3 playthroughs: early game (tier1), mid game (tier2), late game (tier3 + boss)
- Ajustar `GameBalanceData.tres` (elite multipliers, wave modifiers, meta-upgrade values)

**Estimate**: 1-2 dias

---

### 3. Game Over + Victory Flows

**Why**: quando o jogador perde todas as vidas ou derrota o boss, o ecrã final
existe mas o fluxo de recompensa (tokens, resumo, navegação) pode estar
incompleto ou quebrado.

**What to build**:
- **Game Over**: confirmar que mostra ecrã de derrota, calcula tokens
  (`FightsCompleted / TotalFights`), permite voltar ao Main Menu
- **Victory**: ecrã de vitória com badges (tokens, inimigos mortos, gold gasto,
  dano total), opção "New Run" e "Main Menu"
- **End Run (abandonar)**: se o jogador desiste a meio, ganha tokens parciais
  baseados no progresso
- **Resumo rápido**: quantas waves sobreviveu, quantos inimigos converteu,
  gold total ganho

**Files to change**:
- `scripts/ui/screens/GameOverScreen.cs` — token reward, stats summary
- `scripts/ui/screens/VictoryScreen.cs` — stats summary, New Run button
- `scripts/autoload/RunState.cs` — `EndRun()` já calcula tokens, confirmar que
  `isVictory` está a ser passado corretamente

**Estimate**: 0.5 dia

---

### 4. Save Run State Mid-Run (opcional, recomendado)

**Why**: se o jogador fecha o jogo durante uma fight (ou a meio de uma run),
perde todo o progresso. Numa run de 30 min é aceitável; numa demo de 5h é
frustrante.

**What to build**:
- Serializar `RunState` para JSON em `SaveManager`:
  - Tower levels, equipment, trinkets, shop items comprados
  - Gold, lives atuais
  - FightsCompleted, IsBossFight, IsMiniboss
  - AncientStarter stacks/attack counts
- Salvar em momentos seguros (entre fights, após shop/heal/treasure)
- Carregar no Main Menu: "Continue Run?" se houver save ativo
- Limpar save ao terminar a run (victory/game over/end run)

**Decisions**:
- Não salvar durante uma fight (mid-wave é demasiado complexo com projéteis
  e enemies ativos)
- Usar `SaveManager.SaveRunState(RunStateData data)` / `LoadRunState()`
- RunStateData = struct/simple JSON, não Resource (evita security vector)

**Estimate**: 1 dia

---

### 5. UI/UX Mínima para Clareza do Jogador

**Why**: o jogo não dá feedback quando o jogador não pode fazer algo. Sem
tooltips de erro, o jogador fica confuso.

**What to build**:
- **Botão "Next Wave" disabled** com tooltip: "Aguardando inimigos..." ou
  "Complete a wave atual"
- **Botão de tower disabled**: tooltip com "Sem gold suficiente" ou
  "Tipo já colocado"
- **Reroll button disabled**: tooltip com "Sem gold para reroll"
- **Mensagem de "Wave Complete"** no HUD quando todos os inimigos morrem
- **Feedback de compra**: flash verde no botão + "✓ Comprado" (já existe
  na Shop, confirmar nos outros sítios)
- **Indicador de boss fight no Briefing**: já existe ("BOSS FIGHT!") mas
  confirmar que o label é visível e não colide com outros elementos

**Estimate**: 0.5 dia

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
| 4 | Save Run State Mid-Run | Fundação | Medium | 1d | #3 |
| 5 | UI/UX Mínima | Fundação | Medium | 0.5d | #1 |
| 6 | Custom Sprites | Polimento | Critical | 5-8d | None |
| 7 | Enemy/Projectile VFX | Polimento | High | 3-5d | None |
| 8 | UI Transitions | Polimento | Medium | 2-3d | None |
| 9 | Slot Machine Animation | Polimento | Medium | 3-4d | #8 |
| 10 | Tutorial/Onboarding | Polimento | Low | 1-2d | #5 |
| 11 | Sound System | Polimento | Critical | 2-3d | None |
