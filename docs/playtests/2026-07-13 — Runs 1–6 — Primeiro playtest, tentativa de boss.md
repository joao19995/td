# Playtest — 2026-07-13 — Runs 1–6 — Primeiro playtest, tentativa de boss

## Build / Commit
Primeiro playtest registado. HEAD: `80f9ca7` ("Add Baseline V1 Report for Sourdough Siege Demo").
Commits relevantes desde o início do projecto — não há baseline anterior para comparação.

## Setup
- **Act / Map:** Act 1 (tier1 waves), mapas Map1/Map2 em rotação aleatória
- **Loadout:** Bread Baker, Bread Courier, Aroma Keeper (3 torres nas runs 1-2) + Taste Tester (4 torres nas runs 3-6)
- **Trinkets:** Fermentation Diary (+15% status duration, runs 4-5), Oven Heart Ember (+4 range, run 6)
- **Synergies:** Grand Opening Rush (+10% fire rate, 3+ tipos), Daily Proof (Baker+Courier +10% dmg), One Whiff One Bite (Keeper+Tester +15% dmg)
- **Shop items:** Golden Proof Flour (+5% dmg vs heavy, runs 4 e 6)
- **Meta upgrades active:** Taste Tester desbloqueado, upgrades do Baker desbloqueados. Nenhum upgrade de stats (dano, starting gold) comprado na meta shop.
- **Strategy / rationale:** Aposta no Bread Baker como carry principal com Ancient Starter (+1 dano a cada 10 ataques) + Bread Courier como DPS secundário. Aroma Keeper e Taste Tester como suporte — colocados tardiamente, sem investimento de meta. Objectivo: vencer o boss. *(Player)*

---

## Analytics Summary

### Visão Geral

| Run | Torres | Lutou até | Resultado | Vidas perdidas | Equipamento | Shop Item | Trinket |
|-----|--------|-----------|-----------|---------------|-------------|-----------|---------|
| 1 | 3 (B,C,K) | Boss (Fight 5) | ❌ Derrota | 26 | — | — | — |
| 2 | 3 (B,C,K) | Boss (Fight 5) | ❌ Derrota | 35 | Ancient Starter (Baker) | — | — |
| 3 | 4 (+T) | Miniboss (Fight 1) | ❌ Derrota | 20 (morte) | Ancient Starter (Baker) | — | — |
| 4 | 4 | Boss (Fight 5) | ❌ Derrota | 23 | Ancient Starter (Baker) | Golden Proof Flour | Fermentation Diary |
| 5 | 4 | Boss (Fight 5) | ❌ Derrota | 33 | — | — | Fermentation Diary |
| 6 | 4 | Boss (Fight 5) | ✅ **Vitória** | 11 | Ancient Starter (Baker) + Messenger Crate (Courier) | Golden Proof Flour | Oven Heart Ember |

*B = Bread Baker, C = Bread Courier, K = Aroma Keeper, T = Taste Tester*

**Progressão clara:** cada run adicionou mais recursos (equipamento, shop items, trinkets) e a run 6 — com o kit completo (2 equips + shop item + trinket de range) — conseguiu a vitória.

### Dano por Torre (Total da Run)

| Run | Bread Baker | Bread Courier | Aroma Keeper | Taste Tester | Total |
|-----|-------------|---------------|-------------|--------------|-------|
| 1 | 16,522 | 14,164 | 9,289 | — | 39,976 |
| 2 | 34,769 | 8,029 | 6,428 | — | 49,225 |
| 3 | 14,371 | 3,941 | — | 1,784 | 20,097 |
| 4 | 34,308 | 10,469 | 7,213 | 2,140 | 54,130 |
| 5 | 23,167 | 13,589 | 1,404 | 1,748 | 39,908 |
| 6 | **35,257** | 11,548 | 3,210 | 1,847 | 51,862 |

**Padrão:** Bread Baker domina consistentemente com 55-71% do dano total. O Courier é o segundo mas a diferença cresce quando o Baker tem Ancient Starter. As torres de suporte (Keeper, Tester) contribuem <20% combinadas — reflexo de serem colocadas tarde e sem upgrades.

### Kills por Torre (Total da Run)

| Run | Bread Baker | Bread Courier | Aroma Keeper | Taste Tester | Total |
|-----|-------------|---------------|-------------|--------------|-------|
| 1 | 175 | 103 | 29 | — | 307 |
| 2 | 221 | 56 | 13 | — | 290 |
| 3 | 110 | 33 | — | 11 | 154 |
| 4 | 215 | 72 | 44 | 2 | 333 |
| 5 | 162 | 101 | — | 3 | 266 |
| 6 | **272** | 56 | 2 | 2 | 332 |

Baker com 65-82% dos kills. Courier com 17-38% nas runs sem equipamento, mas cai para ~17% quando o Baker tem Ancient Starter — o Baker torna-se tão dominante que "rouba" kills.

### Melhorias (Upgrades)

| Run | Upgrades comprados | Custo total |
|-----|-------------------|-------------|
| 1 | Baker T1, Courier T1, Keeper T1 | 242g |
| 2 | Baker T1, Courier T1, Keeper T1 | 242g |
| 3 | Baker T1, Courier T1 | 154g |
| 4 | Baker T1, Courier T1, Keeper T1 | 242g |
| 5 | Courier T1, Baker T1, **Baker T2**, Keeper T1 | **352g** |
| 6 | Baker T1, **Baker T2**, Keeper T1, Courier T1 | **352g** |

As runs 5 e 6 investiram no T2 do Baker (+110g adicional). A run 5 perdeu por falta de equipamento; a run 6 venceu com equipamento + T2 Baker.

### Equipamento

| Run | Baker | Courier | Custo total equip |
|-----|-------|---------|-------------------|
| 1 | — | — | 0g |
| 2 | Ancient Starter (120g) | — | 120g |
| 3 | Ancient Starter (120g) | — | 120g |
| 4 | Ancient Starter (120g) | — | 210g* |
| 5 | — | — | 0g |
| 6 | Ancient Starter (120g) | Messenger Crate (120g) | **330g*** |

*\*discrepância analytics: custo registado > soma dos equips listados (possível replace mid-run)*

### Leaks por Tipo de Inimigo

| Run | Pigeon | Cat | Jogger | Dragon | Gluten Bishop | Total vidas |
|-----|--------|-----|--------|--------|---------------|-------------|
| 1 | 1 | — | — | 2 | — | 26 |
| 2 | 8 | — | — | — | 1 | 35 |
| 3 | — | 4 | — | — | — | 20 (morte) |
| 4 | 4 | — | — | — | 1 | 23 |
| 5 | 3 | — | 1 | 2 | — | 33 |
| 6 | 5 | — | — | — | — | **11** |

**Baguette Pigeon** é o leak mais consistente (21 pigeons leakados no total, 6/6 runs). Velocidade alta (32 px/s) + 2 vidas por leak torna-os perigosos em grupos. Na run 2, 8 pigeons causaram 20 vidas perdidas num só fight.

**Boss leaks:** Dragon leakou em 2/5 boss fights (runs 1, 5). Bishop leakou em 2/5 (runs 2, 4). Nenhum boss leakou na run 6 (vitória).

### Economia de Ouro

| Run | Ouro ganho | Torres | Upgrades | Equipamento | Rerolls | Total gasto | Eficiência |
|-----|-----------|--------|----------|-------------|---------|-------------|------------|
| 1 | 2,226 | 1,490 | 242 | 0 | 0 | 1,732 | 78% |
| 2 | 2,018 | 1,200 | 242 | 120 | 0 | 1,562 | 77% |
| 3 | 1,139 | 525 | 154 | 120 | 0 | 799 | 70% |
| 4 | 2,278 | 1,510 | 242 | 210 | 0 | 1,962 | 86% |
| 5 | 1,705 | 1,210 | 352 | 0 | 300 | 1,862 | 109%* |
| 6 | 2,849 | 1,310 | 352 | 330 | 50 | 2,042 | 72% |

*\*Run 5: rerolls (300g) ultrapassaram o ouro disponível — indica gold carryover de fights anteriores.*

Ouro não gasto no final: Run 1 = 306g, Run 6 = 925g. O jogador terminou as runs com ouro por gastar, especialmente na run da vitória — 925g remanescente sugere que a economia permite acumular reservas significativas.

### Caminho da Slot Machine

| Run | Sequência de outcomes |
|-----|----------------------|
| 1 | Treasure → Fight → Shop → Fight → **Boss** |
| 2 | Shop → Heal → Fight → Shop → **Boss** |
| 3 | Shop → Miniboss ☠️ |
| 4 | Treasure → Shop → Miniboss → Treasure → **Boss** |
| 5 | Heal → Treasure → Treasure → Treasure → **Boss** |
| 6 | Heal → Shop → Heal → Treasure → **Boss** |

**Run 1** conseguiu Treasure (trinket) no primeiro outcome — mas analytics mostram 0 trinkets na run (possível skip ou bug).
**Run 5** foi a mais generosa: 3× Treasure consecutivos + Heal inicial. Mas sem equipamento, os trinkets não chegaram.
**Run 6** (vitória): Heal → Shop → Heal → Treasure → Boss. Dois heals deram margem de vidas, Shop permitiu equipamento, Treasure deu Oven Heart Ember.

### Boss Fight — Detalhe

**Composição da boss wave (act1_boss.tres):**
8× Tourist (50 HP) + 4× Pigeon (40 HP) + 3× Cat (150 HP) + 2× Dragon (1000 HP, boss) + 1× Bishop (1500 HP, boss, anti-buff aura)

**Escalamento na boss fight (fight 5, DifficultyScalingPerFight=0.30):**
Multiplicador = 1 + 5×0.30 = **2.5× HP/dano**. Total HP ≈ **11,275**.

| Run | Dano total no boss fight | Kills | Leaks | Desfecho |
|-----|------------------------|-------|-------|-----------|
| 1 | ~3,660 | 12 | 2× Dragon | ❌ 24 vidas |
| 2 | ~7,933 | 15 | 1× Bishop | ❌ 15 vidas |
| 4 | ~10,348 | 17 | 1× Bishop | ❌ 15 vidas |
| 5 | ~7,775 | 14 | 2× Dragon | ❌ 24 vidas |
| 6 | **~11,402** | **17** | **0** | ✅ Vitória |

Run 6: 11,402 dano aplicado vs 11,275 HP total — margem mínima de 127 dano (1.1%). Vitória no limite.

---

## Code Changes Since Last Playtest
*Primeiro playtest — sem baseline anterior. Commits relevantes incluem:*
- `c04030f` — analytics tracking
- `c06004e` — Act 1 content & balance adjustments
- `80f9ca7` — Baseline V1 Report
- Diversos commits de equipamento, trinkets, synergies, e wave decoupling (ver `git log --oneline -20`)

---

## Result
Vitória na 6ª run após investimento pesado em equipamento e T2 do Baker. As runs 1-5 foram derrotas, maioritariamente no boss. Run 3 foi a única morte precoce (miniboss com cats). O objectivo "vencer o boss" foi cumprido, mas a sensação é que devia ter demorado mais runs. *(Player)*

---

## What Felt Good
- **Ancient Starter** no Baker cria uma sensação de scaling durante o fight — o dano cresce visivelmente ao longo da wave.
- As **sinergias** (Daily Proof, Grand Opening Rush) são fáceis de activar com 3-4 torres e dão bónus perceptíveis.
- O **loop da slot machine** é viciante — Treasure → Shop → Boss cria antecipação.
- Conseguir a vitória após 5 derrotas deu sensação de progressão e recompensa. *(Player)*

---

## What Felt Bad
- **Chegar ao boss na primeira run** sem qualquer compra na meta shop — sente-se que devia haver uma barreira de progressão mais forte. Esperado: 3-4 runs para ver o boss, 5-6 para o vencer.
- **Equipamento é demasiado impactante** — a diferença entre ter Ancient Starter e não ter é a diferença entre 16k e 35k de dano no Baker (2.2×). Sente-se que o equipamento vale mais que upgrades de torre.
- **Torres de suporte** (Aroma Keeper, Taste Tester) sentem-se inúteis quando não se investe nelas na meta shop. Foram colocadas só porque havia ouro extra, não por necessidade estratégica.
- **Baguette Pigeons** leakam consistentemente — velocidade 32 + 2 vidas por leak torna-os mais perigosos que bosses em algumas runs (run 2: 8 pigeons = 20 vidas).
- **Ouro não gasto** — terminar runs com 500-900g no banco sugere que ou as opções de gasto são limitadas ou o jogador não sente necessidade de gastar.
- **Trinkets** (Fermentation Diary, Oven Heart Ember) têm efeitos demasiado subtis para se notar o impacto durante o jogo — ao contrário do equipamento que é imediatamente visível. *(Player)*

---

## Bugs
*Nenhum bug observado pelo jogador durante esta sessão.*

---

## Balance Observations

### Progressão Meta → Run demasiado plana
Com 0 compras na meta shop (excepto desbloquear o Tester e upgrades do Baker), o jogador chegou ao boss na run 1. A `DifficultyScalingPerFight = 0.30` (30% por fight) devia criar uma parede no boss fight (2.5× HP), mas o boss tem apenas 2× Dragon (1000 HP) + 1× Bishop (1500 HP) como ameaças reais. Os adds (Tourist 50 HP, Pigeon 40 HP) são irrelevantes mesmo a 2.5× (125 HP e 100 HP).

**Possível ajuste:** aumentar o scaling do boss fight (ex: 3.0× em vez de 2.5×) ou adicionar um terceiro boss à wave para que o DPS requerido esteja fora do alcance de uma run sem equipamento.

### Equipamento vs Upgrades
Ancient Starter (120g): +10% dano multiplicativo + stacking de +1 dano plano a cada 10 ataques. Um Baker com 1.0 fire rate ganha ~1 dano a cada 10 segundos. Numa wave de 30-50s, são +3 a +5 de dano plano, mais o +10% multiplicativo. Comparado com o T1 do Baker (+? dano por 66g), o equipamento dá mais valor por gold e escala durante o fight.

**Dados:** Run 2 (com Ancient Starter, sem T2) = 34,769 dano no Baker. Run 5 (sem equipamento, com T2) = 23,167 dano. Ancient Starter sozinho (120g) supera T2 do Baker (66+110=176g).

### Pigeons como ameaça desproporcional
Pigeons (32 speed, 40 HP base, 2 vidas/leak) são a causa #1 de vidas perdidas. Em 4/6 runs, pigeons causaram leaks. Run 2: 8 pigeons = 20 vidas (metade das vidas totais da run). A velocidade alta + baixo HP significa que ou são mortos no início do path ou escapam — não há meio-termo. Torres de fogo lento (Baker 1.0 fire rate) sofrem contra grupos de pigeons.

### Gold remanescente
Run 6 (vitória) terminou com 925g não gastos. Isto representa 32% do ouro total ganho (2,849g). O jogador podia ter comprado mais upgrades ou torres extra mas não o fez — seja por não precisar ou por não haver opções atractivas. *(Agent)*

---

## Player Choices & Rationale
Estratégia: maximizar o Bread Baker como hyper-carry. Colocar Baker primeiro (custo 75g), upgrade T1 ASAP (66g), comprar Ancient Starter (120g) mal aparece na Shop. Só depois expandir para Courier e outras torres. Nas runs 1-2 (3 torres), Keeper era a 3ª torre. Nas runs 3-6, Taste Tester juntou-se como 4ª mas foi colocada quase sempre no último fight.

A lógica foi: "se o Baker já está a dar carry, mais vale investir nele do que dispersar". Isto revelou-se correcto — o Baker consistentemente fez 55-70% do dano.

Rerolls foram usados minimamente (run 4: 50g, run 5: 300g) — o jogador preferiu guardar ouro para equipamento e upgrades em vez de forçar outcomes específicos na slot machine. *(Player)*

---

## Ideas
- **Gating do boss:** 3-4 runs de "aquecimento" antes de o boss aparecer no slot. Isto obriga o jogador a acumular recursos (equipamento, trinkets, upgrades) antes do encontro decisivo.
- **Nerf ao equipamento ou buff aos upgrades:** Se o Ancient Starter (120g) dá mais valor que 176g em upgrades, os preços ou efeitos estão descalibrados.
- **Mais tipos de gasto de ouro:** Com 925g remanescente na run da vitória, há espaço para um sink de ouro (ex: comprar vidas extra, reroll de equipamento na loja, upgrade de trinket).
- **Pigeons revistos:** Ou menos speed (32 → 28), ou menos dano ao jogador (2 → 1 vida), ou agrupá-los em waves com menos unidades para evitar swarms que leakam por speed.
- **Trinkets com efeitos mais visíveis:** +15% status duration é invisível durante o combate. Efeitos como "torres ganham um projéctil extra a cada 5 ataques" ou "+1 de dano base a todas as torres" seriam mais sentidos. *(Player)*

---

## Suggestions for Next Playtest
- **Testar outras torres como carry:** Crust Crusader (crit), Dough Exorcist (execute), ou Fermentation Sage (chain) para ver se o Baker é um outlier ou se qualquer torre com equipamento domina.
- **Variar o loadout:** 2 torres em vez de 3-4 para testar se o ouro concentrado em menos torres é mais eficiente.
- **Testar sem equipamento de todo:** confirmar se 5-6 runs sem equipamento ainda permitem vencer (testa se o boss é matável só com upgrades).
- **Comprar upgrades de stats na meta shop** (dano, starting gold) e comparar com a progressão actual sem stats.
- **Observar a economia de ouro** — o jogador consistentemente termina fights com gold não gasto. Vale a pena testar se isso é um problema de balanço ou de comportamento. *(Agent)*

---

## Decisions
- O boss **não deve ser alcançável na primeira run** sem investimento na meta shop. Isto requer: ou gating explícito (ex: boss só aparece após N fights minimum), ou scaling mais agressivo, ou boss wave com mais HP total.
- **Equipamento precisa de revisão de balanço** — actualmente Ancient Starter sozinho supera o valor de 2 tiers de upgrade. Ou o equipamento desce de poder, ou os upgrades sobem.
- **Pigeons precisam de ajuste** — são a causa #1 de leaks não-boss e criam frustração desproporcional.
- **Trinkets precisam de mais "oomph"** — efeitos percentuais pequenos não se sentem durante o jogo. *(Player)*
