# Playtest — 2026-07-13 — Runs 1–6 — Primeiro playtest, tentativa de boss

## Build / Commit
Primeiro playtest registado. HEAD: `b0d8e27` ("feat: Update game balance and mechanics"). Commits relevantes desde o início do projecto — não há baseline anterior para comparação.

Commits que tocam `scripts/` ou `resources/`:
- `b0d8e27` — Update game balance and mechanics
- `80f9ca7` — Add Baseline V1 Report for Sourdough Siege Demo
- `c04030f` — Implement damage context and analytics tracking
- `c06004e` — Act 1 content & balance adjustments
- `d0a9eb8` — DevTools, enemy and loadout management
- `cef0d50` — Meta upgrade management, status effect handling
- `81aea56` — Economy and upgrade systems refactor
- `839c1b3` — Game balance and enemy data
- `1c06b4e` — Tower upgrade data refactor, unlock data
- `af0762c` — Tower data refactor (Crusader, Exorcist, Sage, Prophet, Monk)
- `7e23ae8` — Tower data refactor (Truck, Baker, Courier, Tester, Keeper)
- `846db81` — Act selection system
- `b031cde` — Run state saving/loading
- `9ceb9ff` — Tower attributes and upgrade costs; token display
- `9a29368` — Balance parameters, tower selection logic

## Setup
- **Act / Map:** Act 1 (todas as waves são de tier1, independentemente do `FightsCompleted`; tier2/tier3 são reservados para Act 2), mapas Map1/Map2
- **Loadout:**
  - Runs 1–2: Bread Baker, Bread Courier, Aroma Keeper (3 torres)
  - Runs 3–6: + Taste Tester (4 torres, desbloqueado na meta shop entre runs)
- **Trinkets:**
  - Fermentation Diary (+15% status effect duration) — runs 4, 5
  - Oven Heart Ember (+4 starting range global) — run 6
  - *(Trinkets só aparecem a partir do momento em que são desbloqueados na meta shop. Run 1 teve outcome Treasure mas sem trinkets desbloqueados — não havia nada para escolher.)*
- **Synergies:**
  - Grand Opening Rush (3+ tower types: +10% fire rate) — runs 1, 2, 4, 5, 6
  - Daily Proof (Baker + Courier: +10% damage) — runs 1, 2, 4, 5, 6
  - One Whiff One Bite (Keeper + Tester: +15% damage) — runs 4, 5, 6
  - *(Run 3: nenhuma sinergia activa — run terminou cedo no miniboss sem Tester colocado?)*
- **Shop items:**
  - Golden Proof Flour (+5% damage vs heavy/armoured, custo 90g) — runs 4, 6
- **Meta upgrades active:**
  - Taste Tester desbloqueado (torre)
  - Baker upgrades desbloqueados (T1 e T2)
  - Ancient Starter desbloqueado (equipamento do Baker)
  - Messenger Crate Upgrade desbloqueado (equipamento do Courier)
  - 2 trinkets desbloqueados (Fermentation Diary, Oven Heart Ember)
  - 1 shop item desbloqueado (Golden Proof Flour)
  - *(Nenhum upgrade de stats comprado — dano, starting gold, etc.)* — *(Player)*
- **Strategy / rationale:**
  Aposta no Bread Baker como hyper-carry: colocar Baker primeiro (75g), upgrade T1 ASAP (66g), comprar Ancient Starter (120g) mal apareça na Shop. Bread Courier como DPS secundário. Aroma Keeper e Taste Tester como suporte — colocados tardiamente, sem investimento de meta. Objectivo: vencer o boss. *(Player)*

## Analytics Summary

### Run Overview

| Run | Result | Duration | Fights | Lives Lost | Gold Earned | Gold Spent |
|-----|--------|----------|--------|------------|-------------|------------|
| 1 | Derrota (boss) | 3m 18s | 6 | 26 | 2,226 | 1,732 |
| 2 | Derrota (boss) | 3m 39s | 6 | 35 | 2,018 | 1,562 |
| 3 | Derrota (miniboss) | 1m 35s | 3 | 20 | 1,139 | 799 |
| 4 | Derrota (boss) | 3m 50s | 6 | 23 | 2,278 | 1,962 |
| 5 | Derrota (boss) | 3m 7s | 6 | 33 | 1,705 | 1,862 |
| 6 | **Vitória** (boss) | 3m 23s | 6 | **11** | **2,849** | 2,042 |

**Progressão:** a Run 1 chegou ao boss já sem qualquer equipamento. A vitória só chegou na Run 6 com Ancient Starter (Baker) + Messenger Crate (Courier) + Oven Heart Ember (range global) + Golden Proof Flour (+5% vs heavy). As runs 3 e 5 foram excepções — a 3 morreu no miniboss, a 5 não comprou equipamento.

### Damage Share (All Runs Aggregated)

| Tower | Total Damage | % Share | Avg Dmg/Run | Total Kills |
|-------|-------------|---------|-------------|-------------|
| Bread Baker | 158,394 | 62.1% | 26,399 | 1,155 |
| Bread Courier | 61,740 | 24.2% | 10,290 | 421 |
| Aroma Keeper | 27,544 | 10.8% | 4,591 | 88 |
| Taste Tester | 7,519 | 2.9% | 1,880* | 18 |

*\*Taste Tester só participou em 4 runs (3–6). Média calculada sobre 4, não 6.*

**Padrão principal:** Bread Baker domina consistentemente (62% do dano total). Quando equipado com Ancient Starter, o Baker faz mais que o dobro do Courier (ex: Run 2: 34,769 vs 8,029; Run 6: 35,257 vs 11,548). Sem equipamento, a diferença é menor (Run 1: 16,522 vs 14,164; Run 5: 23,167 vs 13,589).

### Damage Share Per Run

| Run | Bread Baker | Bread Courier | Aroma Keeper | Taste Tester | Total |
|-----|-------------|---------------|-------------|--------------|-------|
| 1 | 16,522 (41%) | 14,164 (35%) | 9,289 (23%) | — | 39,976 |
| 2 | 34,769 (71%) | 8,029 (16%) | 6,428 (13%) | — | 49,225 |
| 3 | 14,371 (72%) | 3,941 (20%) | — | 1,784 (9%) | 20,097 |
| 4 | 34,308 (63%) | 10,469 (19%) | 7,213 (13%) | 2,140 (4%) | 54,130 |
| 5 | 23,167 (58%) | 13,589 (34%) | 1,404 (4%) | 1,748 (4%) | 39,908 |
| 6 | 35,257 (68%) | 11,548 (22%) | 3,210 (6%) | 1,847 (4%) | 51,862 |

**Nota:** As percentagens de dano do Baker sobem para 68-71% quando tem Ancient Starter (runs 2, 6). O Courier perde quota de dano quando o Baker está equipado — o Baker "rouba" kills por fazer mais dano por tiro.

### Upgrade Distribution

| Tower | Total Upgrades (all runs) | Runs com upgrade | Avg Tier Reached |
|-------|--------------------------|------------------|------------------|
| Bread Baker | 8 (6× T1 + 2× T2) | 6/6 | T1.3 |
| Bread Courier | 6 (6× T1) | 6/6 | T1.0 |
| Aroma Keeper | 5 (5× T1) | 5/6 | T1.0 |
| Taste Tester | 0 | 0/4 | — |

**Custo total em upgrades:** 242g (runs 1-4) / 352g (runs 5-6, com Baker T2).

O Baker foi a única torre a receber upgrade T2 (runs 5, 6). Courier e Keeper nunca passaram de T1. Taste Tester nunca foi upgraded.

### Equipment Purchases

| Run | Baker | Courier | Custo total equip |
|-----|-------|---------|-------------------|
| 1 | — | — | 0g |
| 2 | Ancient Starter (120g) | — | 120g |
| 3 | Ancient Starter (120g) | — | 120g |
| 4 | Ancient Starter (120g) | — | 210g* |
| 5 | — | — | 0g |
| 6 | Ancient Starter (120g) | Messenger Crate (120g) | 330g* |

*\*Discrepância nos analytics: `gold_spent_breakdown.equipment` regista 210g (Run 4) e 330g (Run 6) mas a soma dos itens em `equipment_purchased` é 120g e 240g respectivamente. Diferença exacta de 90g em ambas — possível erro de registo no analytics (custo extra não mapeado) ou custo de aquisição diferente do preço do item. A confirmar.*

**Padrão:** Ancient Starter foi comprado sempre que disponível (4/6 runs). Messenger Crate só na run da vitória. Run 5 não teve equipamento — possivelmente não apareceu na Shop ou o jogador gastou em rerolls.

### Leak Trends

| Run | Pigeon | Cat | Jogger | Dragon (boss) | Gluten Bishop (boss) | Total vidas perdidas |
|-----|--------|-----|--------|---------------|----------------------|---------------------|
| 1 | 1 | — | — | 2 | — | 26 |
| 2 | 8 | — | — | — | 1 | 35 |
| 3 | — | 4 | — | — | — | 20 (morte) |
| 4 | 4 | — | — | — | 1 | 23 |
| 5 | 3 | — | 1 | 2 | — | 33 |
| 6 | 5 | — | — | — | — | **11** |

**Pigeon with a Stolen Baguette** é o leak mais frequente (21 pigeons no total, 6/6 runs). Velocidade 32 com 2 vidas por leak torna-os perigosos em grupo — na Run 2, 8 pigeons causaram 20 vidas perdidas.

**Boss leaks:** Dragon leakou em 2/5 boss fights (runs 1, 5 — 24 vidas cada). Bishop leakou em 2/5 (runs 2, 4 — 15 vidas cada). Nenhum boss leakou na Run 6 (vitória).

**Run 3 (morte precoce):** 4 Lazy Alley Cats (150 HP, heavy) no miniboss — a wave de miniboss parece ter uma composição com múltiplos cats que são difíceis de abater sem upgrades/equipamento.

### Gold Economy

| Run | Gold Earned | Towers | Upgrades | Equipment | Rerolls | Total Spent | Gold Remaining |
|-----|-------------|--------|----------|-----------|---------|-------------|----------------|
| 1 | 2,226 | 1,490 | 242 | 0 | 0 | 1,732 | 494 |
| 2 | 2,018 | 1,200 | 242 | 120 | 0 | 1,562 | 456 |
| 3 | 1,139 | 525 | 154 | 120 | 0 | 799 | 340 |
| 4 | 2,278 | 1,510 | 242 | 210 | 0 | 1,962 | 316 |
| 5 | 1,705 | 1,210 | 352 | 0 | 300 | 1,862 | -157* |
| 6 | 2,849 | 1,310 | 352 | 330 | 50 | 2,042 | **807** |

*\*Run 5: gold gasto (1,862) excede o ganho (1,705) porque o ouro inicial da run e carryover de fights anteriores cobre a diferença. Comportamento normal.*

**Nota sobre gold remanescente:** Na Run 6 (vitória), sobraram 807g. Não por falta de sinks, mas porque a run foi concluída — o boss foi derrotado e não havia necessidade de gastar mais. Na Run 1, não havia equipamentos nem shop items desbloqueados na meta shop, pelo que gastar mais não era possível — as opções estavam limitadas.

### Slot Machine Path

| Run | Sequência de outcomes |
|-----|----------------------|
| 1 | Treasure → Fight → Shop → Fight → **Boss** |
| 2 | Shop → Heal → Fight → Shop → **Boss** |
| 3 | Shop → **Miniboss** ☠️ |
| 4 | Treasure → Shop → Miniboss → Treasure → **Boss** |
| 5 | Heal → Treasure → Treasure → Treasure → **Boss** |
| 6 | Heal → Shop → Heal → Treasure → **Boss** |

**Run 1:** Treasure no primeiro outcome, mas sem trinkets desbloqueados na meta shop — o outcome foi recebido mas não pôde dar nada (sem bug, apenas falta de conteúdo desbloqueado).

**Run 5:** 3× Treasure consecutivos — a sequência mais generosa. Mesmo assim, sem equipamento, os trinkets (Fermentation Diary ×2?) não compensaram a falta de dano bruto.

**Run 6 (vitória):** Heal (recupera vidas) → Shop (compra equipamento) → Heal (recupera de novo) → Treasure (Oven Heart Ember) → Boss. Dois heals deram margem para sobreviver a leaks sem morrer.

## Code Changes Since Last Playtest
*Primeiro playtest — não há baseline anterior.* O projecto está numa fase inicial onde todas as mecânicas de run (slot machine, shop, equipamento, trinkets, synergies, wave decoupling) foram implementadas recentemente. Os commits listados em **Build / Commit** representam o estado actual do jogo.

## Result
Vitória na 6ª run. O objectivo "vencer o boss" foi cumprido. A estratégia de hyper-carry no Baker com Ancient Starter funcionou, especialmente combinada com Messenger Crate no Courier e Oven Heart Ember. O jogador nota que chegou ao boss na primeira run sem qualquer compra na meta shop, o que sugere que a progressão é demasiado rápida — esperava perder mais vezes antes de ver o boss. *(Player)*

## What Felt Good
- **Ancient Starter** no Baker — o dano cresce visivelmente ao longo da wave, dá uma sensação de scaling.
- **Sinergias** (Daily Proof, Grand Opening Rush) — fáceis de activar com 3-4 torres, dão bónus perceptíveis.
- **Loop da slot machine** — a sequência de outcomes cria antecipação (Treasure → Shop → Boss).
- **Conseguir a vitória após derrotas** — deu sensação de progressão. *(Player)*

## What Felt Bad
- **Chegar ao boss na primeira run** — sem compras na meta shop, sente-se que não há barreira de progressão suficiente. Seria mais satisfatório precisar de 3-4 runs para chegar ao boss e 5-6 para o vencer.
- **Equipamento demasiado impactante** — a diferença entre ter Ancient Starter e não ter é notória (ex: 16k vs 35k de dano no Baker).
- **Torres de suporte** (Aroma Keeper, Taste Tester) — colocadas tardiamente, sem upgrades, sem equipamento. Contribuem <10% do dano combinado.
- **Baguette Pigeons** — velocidade 32 + 2 vidas por leak = leaks consistentes em todas as runs.
- **Trinkets** — efeitos subtis (+15% status duration, +4 range) que não se sentem durante o combate, ao contrário do equipamento que é imediatamente visível. *(Player)*

## Bugs
Nenhum bug observado durante a sessão. A discrepância de 90g no custo de equipamento (analytics) pode ser um bug de tracking ou um custo extra não documentado — por confirmar. *(Player)*

## Balance Observations

### Progressão meta → run demasiado plana
Chegar ao boss na Run 1 sem qualquer investimento na meta shop (excepto desbloqueios) sugere que o scaling por fight (0.30/fight) não é suficiente para travar runs sem equipamento. O boss wave tem 2× Dragon (1000 HP) + 1× Bishop (1500 HP) como únicas ameaças reais — os adds (Tourist 50 HP, Pigeon 40 HP) são irrelevantes mesmo com 2.5× multiplicador.

### Equipamento vs Upgrades
Ancient Starter (120g): +10% dano + stacking de +1 dano a cada 10 ataques. Numa wave de 30-50s, acumula +3 a +5 de dano plano. Comparação entre runs:
- Run 2 (Ancient Starter, sem T2): 34,769 dano no Baker
- Run 5 (T2 Baker, sem equipamento): 23,167 dano

A diferença é grande (~11k), mas **a comparação não é directa** — são runs diferentes com caminhos de slot machine diferentes, trinkets diferentes, e posicionamento de torres variável (torres mais à frente fazem mais dano). O padrão geral sugere que o equipamento é muito forte, mas o valor exacto precisa de mais dados.

### Pigeons como ameaça #1
Pigeons (32 speed, 40 HP, 2 vidas/leak) são a causa mais frequente de perda de vidas. Em 21 leaks, 18 foram vidas perdidas (2 vida por leak = 42 vidas, mas alguns leaks contam como 2 vidas cada). A velocidade alta + baixo HP significa que ou são abatidos no início do percurso ou fogem — não há meio-termo. Torres com fire rate lento (Baker 1.0, Tester 1.0) sofrem.

### Gold remanescente
Na Run 6, sobraram 807g porque a run foi concluída — não porque faltam opções de gasto. Na Run 1, não havia equipamentos nem shop items desbloqueados, pelo que gastar mais não era possível. O gold remanescente não indica falta de sinks, mas sim que o jogador venceu antes de precisar de gastar.

## Player Choices & Rationale
Estratégia consistente: maximizar o Bread Baker como carry principal. Colocar Baker primeiro (75g), upgrade T1 ASAP (66g), comprar Ancient Starter (120g) mal aparecesse na Shop. Só depois expandir para Courier e outras torres. Nas runs 1-2 (3 torres), Keeper era a 3ª torre. Nas runs 3-6, Taste Tester juntou-se como 4ª mas foi colocada quase sempre no último fight.

A lógica foi: "se o Baker já está a dar carry, mais vale investir nele do que dispersar". Isto revelou-se correcto — o Baker consistentemente fez 55-71% do dano.

Rerolls foram usados minimamente (Run 5: 300g, Run 6: 50g) — o jogador preferiu guardar ouro para equipamento e upgrades.

**Meta shop escolhas:** A prioridade foi desbloquear equipamento (Ancient Starter, Messenger Crate) em vez de upgrades de stats. O Taste Tester foi desbloqueado por curiosidade mas não recebeu investimento. *(Player)*

## Ideas
- **Gating do boss:** Impedir que o boss apareça antes de N runs completadas (ex: 3 runs de "aquecimento"). Isto força acumulação de recursos antes do encontro decisivo.
- **Nerf ao equipamento ou buff aos upgrades:** Se Ancient Starter (120g) parece dar mais valor que T1+T2 do Baker (176g), os preços ou efeitos podem estar descalibrados.
- **Pigeons revistos:** Ou velocidade reduzida (32 → 28), ou dano ao jogador reduzido (2 → 1), ou agrupamento reduzido.
- **Trinkets com efeitos mais visíveis:** Efeitos como "torres ganham projéctil extra" ou "+1 dano base a todas as torres" seriam mais perceptíveis que +15% status duration. *(Player)*

## Suggestions for Next Playtest

### Auto-sugestões (baseadas em padrões dos dados)
- **Testar outras torres como carry:** Crust Crusader (crit-based), Dough Exorcist (execute), ou Fermentation Sage (chain/Bounce) — para ver se o Baker é um outlier ou se qualquer torre com equipamento na posição certa domina.
- **Variar o loadout:** 2 torres em vez de 3-4 para testar se ouro concentrado é mais eficiente.
- **Testar sem equipamento nenhum:** Confirmar se é possível vencer o boss só com upgrades (testar balanço base do boss).
- **Comparar equipamento vs upgrades em ambiente controlado:** Duas runs com o mesmo loadout e caminho de slot machine semelhante, uma com equipamento e outra com upgrades equivalentes.
- **Observar o comportamento do Taste Tester com investimento:** Colocar Taste Tester cedo, com upgrades e equipamento — ver se o poison scaling compensa.

### Preferências do jogador
- *(Player pode adicionar, remover, ou modificar sugestões)*

## Decisions
- **Scaling mais agressivo nas fights antes do boss** — o boss em si está ok, mas as fights anteriores ao boss precisam de scaling mais forte para evitar que o jogador chegue ao boss sem ter gasto recursos significativos. O gating explícito (N runs mínimas) fica como alternativa caso o scaling não resolva.
- **Equipamento vs upgrades:** manter como está para já. O equilíbrio actual será reavaliado com mais dados de playtests futuros.
- **Pigeons:** sem alteração para já — o comportamento actual é aceitável.
- **Trinkets:** sem alteração para já — efeitos subtis são intencionais por agora.
- **Discrepância de 90g no custo de equipamento:** precisa de ser investigada — confirmar se é bug de tracking no analytics ou custo real não documentado (ex: taxa de aquisição, custo de replace, ou equipamento com dois componentes). *(Player)*
