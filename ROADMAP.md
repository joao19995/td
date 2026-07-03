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

## Camada de Refatoração — Arquitetura, Higiene e Dívida Técnica

Antes de expandir conteúdo, é necessário sanear violações das guidelines do projeto
que comprometem escalabilidade e manutenibilidade. Cada item foi identificado numa
auditoria arquitetural extensiva.

---

### 1. [DONE] Centralizar GameBalance em Resource Data-Driven

**Why**: ~40 valores de gameplay estão hardcoded em 7+ arquivos .cs, violando o
princípio "all gameplay values come from .tres Resources". Multiplicadores de elite,
modifiers de wave, cooldowns, penalidade anti-buff, fórmulas de meta-upgrade —
nenhum destes é editável sem recompilação.

**What to build**:
- Criar `resources/game_balance.tres` com campos para:
  - Elite HP/Damage/Gold multipliers
  - Wave modifier values (Horde interval/count multiplier, Armored/Swift/GoldRush multipliers,
    FinalStretch count mult)
  - Anti-buff penalty (0.5f)
  - Synergy cooldown (10s), Judgment Seal cooldown (5s)
  - Meta-upgrade per-level values (damage %/level, gold/level, lives/level, etc.)
  - Passive gold amount/interval, tier thresholds (0.33, 0.66)
  - Token reward formula constants (base, victory multiplier)
  - Wave generation constants (min/max waves, difficulty curve params, final stretch offset)
  - Wave modifier pools (AllModifiers, HardModifiers)
- Criar `scripts/resources/GameBalanceData.cs` com `[GlobalClass]` e todos os exports
- Criar `scripts/autoload/GameBalance.cs` singleton que carrega o Resource e expõe
  os valores estaticamente
- Substituir todas as constantes literais nos .cs por `GameBalance.Instance.X`

**Files to change**:
- `scripts/resources/GameBalanceData.cs` — novo
- `resources/game_balance.tres` — novo
- `scripts/autoload/GameBalance.cs` — novo
- `scripts/enemies/Enemy.cs` — linhas 98, 203, 212
- `scripts/Spawner/EnemySpawner.cs` — linhas 18, 79, 86, 88, 165-171
- `scripts/towers/Tower.cs` — linhas 145, 183, 203
- `scripts/autoload/RunState.cs` — linhas 90-100, 163, 165, 260, 270-271, 282-287, 313, 328-329
- `scripts/components/AttackComponent.cs` — linhas 166, 174
- `scripts/components/AuraComponent.cs` — linha 56

**Decisions**:
- GameBalance é carregado uma vez no `_EnterTree` do autoload e nunca recarregado
- Valores que variam por tower/enemy permanecem nos respectivos Resources;
  este Resource contém apenas constantes globais de sistema
- O arquivo `.tres` fica em `resources/` (raiz) por ser global, não específico
  de uma categoria

**Estimate**: 3-4 days

---

### 2. [CRÍTICO] Extrair WaveGenerator de RunState

**Why**: RunState.cs acumula ~115 linhas de lógica de gameplay (geração de waves,
tier determination, passive gold timer, aplicação de trinkets) — viola a diretriz
"autoloads são infraestrutura, não gameplay". Dificulta testar e reusar a lógica
de waves independentemente do estado da run.

**What to build**:
- Criar `scripts/systems/WaveGenerator.cs` com método estático
  `PickRunWaves(int fightsCompleted, int fightsPerRun, string baseDir)`
- Mover `GetWaveTier()`, arrays `AllModifiers`/`HardModifiers`, e toda a lógica
  de difficulty curve / final stretch / modifier assignment
- RunState passa a chamar `WaveGenerator.PickRunWaves(FightsCompleted,
  SlotManager.Instance.FightsPerRun)`
- Manter em RunState apenas estado puro (tower levels, equips, trinkets, shop items)
  e delegação para sistemas especializados

**Files to change**:
- `scripts/systems/WaveGenerator.cs` — novo
- `scripts/autoload/RunState.cs` — remover linhas 265-346, substituir por delegação
- Atualizar referências em BriefingScreen, EnemySpawner se acessam diretamente

**Estimate**: 1-2 days

---

### 3. [CRÍTICO] Converter GetNode<>() para [Export] NodePath nas Screens

**Why**: 63 chamadas `GetNode<>()` com strings hardcoded em 12 arquivos de UI.
Paths de até 5 níveis (`VBox/ContentHBox/PreviewPanel/PreviewHBox/PreviewSprite`).
Qualquer renomeação de node no .tscn quebra silenciosamente em runtime. Viola
diretamente a diretriz "No deep node paths — use [Export] NodePath".

**What to build**:
- Em cada screen, substituir cada `GetNode<T>("path/string")` por:
  ```csharp
  [Export] private NodePath _nomeField;
  private Tipo _nome;
  // em _Ready: _nome = GetNode<Tipo>(_nomeField);
  ```
- Configurar os NodePaths nos .tscn correspondentes via Inspector
- Screens a modificar (por ordem de profundidade dos paths):
  1. `LoadoutScreen.cs` — ~16 paths, 5 deles com profundidade 4-5
  2. `BriefingScreen.cs` — ~11 paths, 4 com profundidade 4
  3. `HUD.cs` — ~12 paths
  4. `FightCompleteScreen.cs` — ~6 paths
  5. `ShopScreen.cs`, `MetaShopScreen.cs`, `MainMenu.cs`
  6. `PauseScreen.cs`, `GameOverScreen.cs`, `VictoryScreen.cs`, `TrinketChoiceScreen.cs`

**Decisions**:
- Nomear exports como `_nodeNameLabel`, `_previewPanel`, etc. para clareza
- Manter o campo privado tipado separado do NodePath (padrão Godot recomendado)
- Os .tscn exigem edição manual no Inspector para apontar os NodePaths

**Estimate**: 2-3 days

---

### 4. [ALTO] ResourceLoaderHelper + Eliminar Duplicação de Carga

**Why**: 6 screens implementam o mesmo padrão `DirAccess.Open("res://...")`
\+ foreach \+ `ResourceLoader.Load<T>()`. `BestiaryScreen.cs` já tem um genérico
`LoadFromDir<T>()` que nenhuma outra screen usa. O padrão "End Run" navigation
está quadruplicado. Tower tags checks duplicados entre LoadoutScreen e BestiaryScreen.

**What to build**:
- Criar `scripts/helpers/ResourceLoaderHelper.cs`:
  ```csharp
  public static Array<T> LoadAllFromDir<T>(string dirPath) where T : Resource
  ```
  com tratamento de erro e `ResourceLoader.CacheMode.Replace`
- Refatorar LoadoutScreen, HUD, BriefingScreen, ShopScreen, MetaShopScreen,
  TrinketChoiceScreen para usar o helper — elimina ~80 linhas duplicadas
- Adicionar método `NavigateToMainMenu()` em UIManager para eliminar quadruplicação
  em GameOverScreen, PauseScreen, VictoryScreen, FightCompleteScreen
- Adicionar `GetTags()` em TowerData para eliminar checks manuais duplicados
- Unificar fade-out+nav em TrinketChoiceScreen num método privado único

**Files to change**:
- `scripts/helpers/ResourceLoaderHelper.cs` — novo
- `scripts/autoload/UIManager.cs` — adicionar `NavigateToMainMenu()`
- `scripts/ui/screens/LoadoutScreen.cs` — usar helper + GetTags()
- `scripts/ui/screens/FightCompleteScreen.cs` — usar NavigateToMainMenu
- `scripts/ui/screens/GameOverScreen.cs` — usar NavigateToMainMenu
- `scripts/ui/screens/PauseScreen.cs` — usar NavigateToMainMenu
- `scripts/ui/screens/VictoryScreen.cs` — usar NavigateToMainMenu
- `scripts/ui/HUD.cs` — usar helper
- `scripts/ui/briefing/BriefingScreen.cs` — usar helper
- `scripts/ui/trinket/TrinketChoiceScreen.cs` — unificar fade-out
- `scripts/resources/TowerData.cs` — adicionar GetTags()

**Estimate**: 2-3 days

---

### 5. [ALTO] Popular FlavorText nos Recursos

**Why**: Todas as 5 classes Resource (TowerData, EnemyData, EquipData, TrinketData,
SynergyData) declaram `FlavorText`, mas **zero** dos ~55 arquivos .tres o preenchem.
O Bestiary renderiza lore condicionalmente — atualmente está sempre vazio. A feature
de profundidade narrativa está completamente invisível.

**What to build**:
- Adicionar `FlavorText = "..."` em cada arquivo .tres relevante:
  - 10× `tower_data/*.tres`
  - 10× `enemy_data/*.tres`
  - 20× `equip_data/*.tres`
  - 10× `trinket_data/*.tres`
  - 4× `synergy_data/*.tres`

**Decisions**:
- Conteúdo textual criado pelo designer/narrative designer
- Pode ser feito em duas etapas: primeiro placeholder "TODO: lore text",
  depois conteúdo real

**Estimate**: 2-4h (estrutural) + 2-3d (conteúdo)

---

### 6. [ALTO] Converter String Routing para Enum no FightCompleteScreen

**Why**: `_pendingOutcome` é string com switch/case em 3 lugares no
FightCompleteScreen. Comparações com `"Fight"`, `"Shop"`, `"Boss"` — frágeis,
sem type safety, erro só aparece em runtime se um outcome for renomeado.

**What to build**:
- Definir enum `SlotOutcome { Fight, Shop, Heal, Miniboss, Treasure, Boss }`
  em arquivo compartilhado (ex: `scripts/systems/SlotOutcome.cs`)
- Substituir `string _pendingOutcome` por `SlotOutcome _pendingOutcome`
- Atualizar FightCompleteScreen e SlotManager

**Estimate**: 0.5 day

---

### 7. [MÉDIO] Remover Dead Code + Inconsistências de Estilo

**Why**: HUD.cs:283 tem código morto (variável `trinketId` que retorna null
e nunca é lida). WaveEntry.cs e WaveData.cs usam fields (`[Export] public T X;`)
em vez de auto-properties (`{ get; set; }`), diferente dos outros 10 data classes.
Inconsistências que acumulam dívida técnica.

**What to fix**:
- Remover linha `string trinketId = ...` em `scripts/ui/HUD.cs:283`
- Converter `WaveEntry.cs` linhas 6-7 e `WaveData.cs` linhas 9-11 para
  auto-properties consistentes com o resto do código
- Verificar se existe mais dead code (grep por variáveis não utilizadas)

**Estimate**: 0.5 day

---

### 8. [MÉDIO] Mover StatusEffectData para Diretório Correto

**Why**: `PoisonEffectData.cs`, `SlowEffectData.cs`, `StatusEffectData.cs`
são `[GlobalClass] Resource` mas estão em `scripts/components/`. O diretório
documentado para data classes é `scripts/resources/`. Causa confusão.

**What to build**:
- Mover os 3 arquivos para `scripts/resources/`
- Atualizar referências em cenas .tscn se apontam por caminho absoluto
- Nenhuma mudança de using (namespace global)

**Estimate**: 0.5 day

---

### 9. [MÉDIO] Atualizar CLAUDE.md para Estado Real

**Why**: CLAUDE.md lista torres como `CornerBaker.tres` e `BikeCourier.tres`
que não existem — os nomes reais são `BreadBaker.tres` e `BreadCourier.tres`.
`Aroma` é `AromaKeeper`. Diretório `Spawner/` não documentado no layout.

**What to fix**:
- Atualizar lista `resources/tower_data/` no Project Layout
- Adicionar `Spawner/` ao layout (ou mover EnemySpawner para scripts/systems/)
- Verificar e corrigir outras discrepâncias entre doc e realidade

**Estimate**: 0.25 day

---

### 10. [BAIXO] Adicionar uids aos .tres Faltantes

**Why**: ~60% dos .tres (wave_data, synergy_data, meta_upgrade_data, trinket_data,
ui_screens) não têm `uid` no cabeçalho. Sem uid, o Godot não tem referência
estável se o arquivo for movido ou renomeado.

**What to build**:
- Abrir cada .tres sem uid no Inspector do Godot (uid é gerado pelo editor,
  não editado manualmente)
- Alternativa: script de build que abre e ressalva cada recurso

**Estimate**: 0.5-1d (ou incremental enquanto edita outros recursos)

---

### 11. [BAIXO] Extrair SynergyPreviewHelper

**Why**: `GetPreviewSynergies()` existe em LoadoutScreen e BriefingScreen com
lógica similar mas não idêntica. Extrair para helper comum evita divergência.

**What to build**:
- Criar `scripts/helpers/SynergyPreviewHelper.cs` com método único
- LoadoutScreen e BriefingScreen passam a chamá-lo

**Estimate**: 0.5 day

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
