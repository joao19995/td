# Balanceamento — Sourdough Siege

Documento único de trabalho para validação de balance: as 10 torres, os 10 inimigos, as 4 sinergias, os 20 equipamentos e os 10 trinkets — stats extraídos diretamente dos `.tres`/`.cs`, DPS calculado (base e totalmente upgradado), lore, análise de "o nome faz sentido com a mecânica?", e uma secção final consolidada com **todas as questões em aberto e bugs concretos** encontrados ao longo da análise, mais os **próximos passos a validar**.

**Fórmula de DPS**: `Damage × FireRate` (FireRate = ataques/segundo, confirmado em `AttackComponent.TryAttack`: `_cooldown = 1f / _effectiveFireRate`). Estes DPS são "vazios" — sem synergy/equip/shop/meta/trinket bonuses, que multiplicam por cima disto (ver `Tower.EffectiveDamage`).

---

## 1. Torres — Resumo Numérico

| Torre | Cost | Dmg base | FR base | Range | DPS Base | DPS Full Upgrade | Custo Total c/ Upgrades | Mecânica |
|---|---|---|---|---|---|---|---|---|
| Bread Baker | 75 | 35 | 1.0 | 48 | 35 | 84 | 235 | — |
| Bread Courier | 90 | 28 | 1.5 | 44 | 42 | 91.2 | 290 | — |
| Aroma Keeper | 100 | 25 | 0.9 | 50 | 22.5 | 49 | 300 | Slow |
| Taste Tester | 120 | 30 | 1.0 | 48 | 30 (+10 poison) | 55.9 (+10 poison) | 360 | Poison |
| Bakery Truck | 140 | 35 | 0.8 | 44 | 28/alvo | 56/alvo | 420 | Splash (r=36) |
| Bread Monk | 160 | 30 | 1.0 | 50 | 30 | 75 | 400 | Aura (não a si) |
| Fermentation Sage | 180 | 30 | 0.9 | 52 | 27 (+13.5 chain) | 70 (+35 chain) | 430 | Chain (1 bounce) |
| Crust Crusader | 220 | 38 | 0.9 | 46 | 34.2 (~39.3 c/ crit) | 88.2 (~101.4 c/ crit) | 510 | Crit (15%, x2) |
| Dough Exorcist | 260 | 35 | 1.0 | 50 | 35 | 90 | 580 | Execute |
| High Prophet | 320 | 40 | 0.9 | 56 | 36 | 91 | 720 | Global Aura |

**Custo por DPS (base, quanto menor melhor)**: Baker 2.14 · Courier 2.14 · Aroma 4.44 · Taste 3.0 · Truck 5.0/alvo · Monk 5.33 · Sage 4.44 · Crusader 5.60 · Exorcist 7.43 · Prophet 8.89.

Padrão visível: as duas torres tier-1 (Baker/Courier) têm o dobro da eficiência DPS/gold das torres tier-3+. Isto é esperado num roguelite (torres caras compram utilidade/situacionalidade, não DPS puro), mas o custo/DPS puro das torres de suporte (Aroma, Monk) é o mais alto do jogo — o valor delas está 100% na utilidade/sinergia.

---

## 2. Torres — Detalhe Individual

### 2.1 Bread Baker (`bread_baker`)
- **Stats**: 35 dmg / 1.0 FR / 48 range / 75g
- **Upgrades**: Reinforce (60g, +15 dmg) → Heavy Barrel (100g, +20 dmg, +0.2 FR)
- **DPS**: 35 → 50 (T1) → 84 (T2), investimento total 235g
- **Mecânica**: nenhuma especial — dano direto puro
- **Lore**: *"The humble foundation of the Order. Steady hands, steady bread."*
- **Análise nome/mecânica**: ✅ Faz sentido. É a torre inicial, sem gimmick, "fundação" literal do exército — reflete-se em ser a mais barata e disponível desde o início (unlock default).
- **Notas de balance**: É a referência de DPS/gold (0.467 base) — todas as outras torres devem justificar o preço com utilidade, não com mais DPS puro.

### 2.2 Bread Courier (`bread_courier`)
- **Stats**: 28 dmg / 1.5 FR / 44 range / 90g
- **Upgrades**: Overclock (80g, +0.5 FR) → Precision Rounds (120g, +10 dmg, +0.4 FR)
- **DPS**: 42 → 56 (T1) → 91.2 (T2), investimento total 290g
- **Equips**: Electric Bike (+20% FR, 80g) · Messenger Crate (+1 pierce, -10% FR, 120g)
- **Lore**: *"Swift and tireless. The dough must reach its destination."*
- **Análise nome/mecânica**: ✅ Muito coerente — courier (estafeta) = cadência de tiro alta, dano baixo por hit.
- **Notas de balance**: Com Precision Rounds acaba por ter o FR mais alto do jogo (2.4) — confirma que é a torre de "muitos hits pequenos".

### 2.3 Aroma Keeper (`aroma_keeper`)
- **Stats**: 25 dmg / 0.9 FR / 50 range / 100g — **Slow**: 0.5× speed, 3.0s duração (refresh, sem stack de intensidade)
- **Upgrades**: Cryo Coil (80g, +0.3 FR) → Permafrost (120g, +10 dmg, +0.2 FR)
- **DPS**: 22.5 → 30 (T1) → 49 (T2), investimento total 300g
- **Equips**: Megaphone (+20% range, 80g) · Spice Wind Chimes (+30% slow duration, -10% dmg, 120g)
- **Lore**: *"The scent of fermentation slows even the most determined heretic."*
- **Análise nome/mecânica**: ✅ Narrativamente ótimo (aroma que atordoa/lentifica). ⚠️ **Inconsistência de assets**: usa `projectile_ice.tscn` / sprite de gelo — visualmente é uma torre de "gelo" com nome/flavor de "aroma". Confirmar se é placeholder pendente de arte.
- **Notas de balance**: DPS mais baixo do jogo (base) — correto para controlo puro, mas o valor dela é quase 100% sinergia com Taste Tester/Fermentation Sage.

### 2.4 Taste Tester (`taste_tester`)
- **Stats**: 30 dmg / 1.0 FR / 48 range / 120g — **Poison**: 10 dmg/tick (1s), 4.0s duração (refresh)
- **Upgrades**: Toxic Vial (100g, +5 dmg) → Concentrated Venom (140g, +8 dmg, +0.3 FR)
- **DPS**: 30 (+10 poison sustido) → 35 (+10) → 55.9 (+10) = **65.9 DPS sustido** no T2 num alvo continuamente atacado
- **Equips**: Silver Tray (+20% poison dmg, 80g) · Double Sampling (poison em +1 alvo extra, -15% FR, 120g)
- **Lore**: *"One bite tells them everything. The poison is merely persuasive."*
- **Análise nome/mecânica**: ✅ Perfeito — "taste tester" a envenenar por "provar" faz sentido cómico e mecânico.
- **Notas de balance**: Como o poison não faz stack de intensidade (só refresh de duração), o DPS extra do poison é sempre +10 flat, exceto quando multiplicado por `PoisonDamagePercentBonus` (Silver Tray, Sacred Flour Dust).

### 2.5 Bakery Truck (`bakery_truck`)
- **Stats**: 35 dmg / 0.8 FR / 44 range / 140g — **Splash**: radius 36 (dano completo, sem falloff, a todos os inimigos no raio)
- **Upgrades**: Explosive Charge (120g, +15 dmg) → Nova Blast (160g, +20 dmg, +12 range)
- **DPS**: 28/alvo → 40/alvo (T1) → 56/alvo (T2), investimento total 420g. Com 3 inimigos no splash, DPS efetivo full-upgrade = **168**.
- **Equips**: Reinforced Suspension (+15% splash radius, 80g) · Street Parade (+30% area dmg, -20% range, 120g)
- **Lore**: *"A mobile bakery. Also a mobile weapon."*
- **Análise nome/mecânica**: ✅ Faz sentido — truck = cobertura de área, bombardeamento.
- **Notas de balance**: Sem falloff de dano na splash, escala muito bem contra hordas (`WaveModifier.Horde`) — provavelmente a melhor resposta a esse modifier.

### 2.6 Bread Monk (`bread_monk`)
- **Stats**: 30 dmg / 1.0 FR / 50 range / 160g — **Aura**: 60 range, +10% dmg / +10% FR às outras torres (não a si própria)
- **Upgrades**: Sacred Chant (100g, +8 dmg, +0.2 FR) → Devotion Aura (140g, +12 dmg, +0.3 FR)
- **DPS pessoal**: 30 → 45.6 (T1) → 75 (T2), investimento total 400g
- **Equips**: Sacred Robes (+15% aura range, 100g) · Prayer Beads (+5% aura potency, mas **deixa de atacar**, 150g)
- **Lore**: *"Through meditation and proper hydration, they empower their brethren."*
- **Análise nome/mecânica**: ✅ Monge = suporte/buffer, arquétipo clássico bem aplicado.
- **Notas de balance**: Os upgrades (Sacred Chant/Devotion Aura) só sobem o dano/FR *pessoal* do Monk, não a magnitude da aura (`AuraDamageBonusPercent`/`AuraFireRateBonusPercent` são campos estáticos de `TowerData`, não escalam com upgrade level). *(Ver questão aberta em 10.1)*

### 2.7 Fermentation Sage (`fermentation_sage`)
- **Stats**: 30 dmg / 0.9 FR / 52 range / 180g — **Chain**: 1 bounce, 40 range, 0.5× dano no bounce
- **Upgrades**: Extended Culture (100g, +8 dmg, +0.2 FR) → Wild Fermentation (150g, +12 dmg, +0.3 FR)
- **DPS**: 27 (+13.5 chain) → 41.8 (+20.9) (T1) → 70 (+35) (T2), investimento total 430g. Total nos dois alvos no T2 = **105 DPS**.
- **Equips**: Golden Proofing Bowl (+15% status duration, 100g) · Wild Yeast (+1 bounce extra, -10% dmg inicial, 150g)
- **Lore**: *"Fermentation is a chain reaction. So is their judgment."*
- **Análise nome/mecânica**: ✅ Muito bem pensado — fermentação É literalmente uma reação em cadeia.
- **Notas de balance**: A torre **não tem Poison nem Slow próprios** (`HasPoison`/`HasSlow` = false no `.tres`) apesar de partilhar tema direto com Aroma Keeper/Taste Tester. Isto tem consequência concreta num equip específico. *(Ver questão aberta em 10.1 e 10.4)*

### 2.8 Crust Crusader (`crust_crusader`)
- **Stats**: 38 dmg / 0.9 FR / 46 range / 220g — **Crit**: 15% chance, ×2 multiplicador
- **Upgrades**: Sharpened Crust (120g, +10 dmg, +0.2 FR) → Holy Crusher (170g, +15 dmg, +0.3 FR)
- **DPS**: 34.2 raw (~39.3 c/ crit) → 52.8 (~60.7) (T1) → 88.2 (~101.4) (T2), investimento total 510g
- **Equips**: Tempered Crust Blade (+10% crit chance, 100g) · Blessed Crunch Seal (crit faz mini-splash raio 30, -10% FR, 150g)
- **Lore**: *"A blade tempered in the finest olive oil. The crust shall prevail."*
- **Análise nome/mecânica**: ✅ Crusader = guerreiro de golpes decisivos, crit bate certo.
- **Notas de balance**: Com o equip Tempered Crust Blade sobe para 25% crit chance. Ver nota importante em 10.1 sobre interação com a anti-buff aura do Gluten Null Bishop.

### 2.9 Dough Exorcist (`dough_exorcist`)
- **Stats**: 35 dmg / 1.0 FR / 50 range / 260g — **Execute**: ×2 dmg se HP≤20%, ×1.5 extra se alvo Boss/Heavy (multiplicativo → até ×3 total)
- **Upgrades**: Blessed Flour (130g, +10 dmg, +0.2 FR) → Judgment Aura (190g, +15 dmg, +0.3 FR)
- **DPS**: 35 → 54 (T1) → 90 (T2), investimento total 580g. Burst situacional até **270** vs elite/boss <20% HP.
- **Equips**: Holy Flour Pouch (+20% dmg vs elite/boss, 100g) · Judgment Seal (execute instantâneo <15% HP, -15% dmg, 150g)
- **Lore**: *"They purge the industrial gluten from unwilling souls."*
- **Análise nome/mecânica**: ✅ Exorcista = "purgar", mapeia perfeitamente para execute/instakill.
- **Notas de balance**: O stack ×2 × ×1.5 = ×3 é um número forte. *(Ver questão aberta em 10.1)*

### 2.10 High Prophet of Sourdough (`high_prophet`)
- **Stats**: 40 dmg / 0.9 FR / 56 range / 320g — **Global Aura**: +2% dmg por torre no mapa (incl. a si própria), recalculado a cada 0.5s
- **Upgrades**: Sourdough Sermon (160g, +10 dmg, +0.2 FR) → Great Fermentation (240g, +15 dmg, +0.3 FR)
- **DPS pessoal**: 36 → 55 (T1) → 91 (T2), investimento total 720g. Com o limite de 5 torres do MVP, o global aura máximo é **+10% dmg a todas as torres**.
- **Equips**: Golden Staff (+10% todos os stats, 120g) · First Starter Relic (+5% dmg por torre próxima em 40 range, -15% range, 180g)
- **Lore**: *"The living embodiment of the Mother Dough's will."*
- **Análise nome/mecânica**: ✅ Prophet = figura que une/canaliza todo o exército — o global aura reflete isso bem.
- **Notas de balance**: É a torre mais cara (720g total) mas o teto do global aura está fixo em +10% pela regra de 5 torres/mapa. *(Ver questão aberta em 10.1)*

---

## 3. Inimigos — Resumo Numérico

| Inimigo | HP | Speed | Gold | Dmg | Boss | Heavy | Especial | TTK Baker base (35 DPS) | TTK Baker T2 (84 DPS) |
|---|---|---|---|---|---|---|---|---|---|
| Sliced Bread Tourist | 100 | 24 | 10 | 1 | – | – | Básico | 2.86s | 1.19s |
| Grocery Run Jogger | 60 | 48 | 15 | 1 | – | – | Rápido, frágil | 1.71s | 0.71s |
| Lazy Alley Cat | 300 | 12 | 30 | 2 | – | Heavy | Tanque inicial | 8.57s | 3.57s |
| Pigeon w/ Stolen Baguette | 80 | 32 | 20 | 1 | – | – | "Voador" (sem mecânica própria) | 2.29s | 0.95s |
| Industrial Bread Dragon | 1000 | 16 | 100 | 5 | Boss | Heavy | Boss final | 28.57s | 11.9s |
| Microwave Meal Preacher | 200 | 20 | 25 | 2 | – | Heavy | "Convertido" médio | 5.71s | 2.38s |
| Plastic Wrapped Sandwich Man | 180 | 28 | 22 | 2 | – | – | Resistência média | 5.14s | 2.14s |
| Frozen Dough Abomination | 400 | 10 | 35 | 3 | – | Heavy | Tanque lento | 11.43s | 4.76s |
| The Gluten Null Bishop | 1500 | 14 | 120 | 6 | Boss | Heavy | Anti-buff aura (60px, -50%) | 42.86s | 17.86s |
| Supermarket Overlord | 900 | 18 | 80 | 4 | – | Heavy | Elite/miniboss | 25.71s | 10.71s |

*(TTK = time-to-kill, referência qualitativa com uma única Bread Baker; útil para sentir a curva de HP.)*

---

## 4. Inimigos — Detalhe Individual

### 4.1 Sliced Bread Tourist (`enemy_tourist`)
- **Stats**: 100 HP / 24 speed / 10 gold / 1 dmg
- **Análise**: ✅ "Tourist" = ameaça genérica baixa, stats medianos em tudo — bom enemy de tier1 default.
- **Forte contra ele**: qualquer torre básica; não tem resistência especial.

### 4.2 Grocery Run Jogger (`enemy_jogger`)
- **Stats**: 60 HP / 48 speed (mais rápido do jogo) / 15 gold / 1 dmg
- **Análise**: ✅ "Jogger" = velocidade, HP baixo compensa. Nome e stats alinhados na perfeição.
- **Forte contra ele**: Aroma Keeper (slow neutraliza a velocidade) ou Bread Courier (FR alto mata antes de ele passar).

### 4.3 Lazy Alley Cat (`enemy_cat`)
- **Stats**: 300 HP / 12 speed (mais lento do tier1) / 30 gold / 2 dmg / Heavy
- **Análise**: ⚠️ "Lazy" explica a velocidade baixíssima, mas um gato como tanque (300 HP) é um salto lore-mecânica menos óbvio. Funciona lido como "gato de rua gordo e teimoso" — mas falta `FlavorText` (ainda "TODO") para reforçar essa leitura. *(Ver 10.2)*
- **Forte contra ele**: Dough Exorcist (Heavy bonus), Crust Crusader (crits acumulam), splash do Bakery Truck se vier em grupo.

### 4.4 Pigeon with a Stolen Baguette (`enemy_pigeon`)
- **Stats**: 80 HP / 32 speed / 20 gold / 1 dmg
- **Análise**: ⚠️ "Flyer" conceptualmente, mas **não tem mecânica própria de voo** — segue o path normal. Já está no `ROADMAP.md` para Camada 2. *(Ver 10.2)*
- **Forte contra ele**: idêntico ao Jogger — rápido e frágil, slow/FR alto resolvem bem.

### 4.5 Industrial Bread Dragon (`enemy_dragon`) — BOSS
- **Stats**: 1000 HP / 16 speed / 100 gold / 5 dmg / Boss + Heavy
- **Análise**: ✅ Nome e função (boss final) perfeitamente alinhados.
- **Forte contra ele**: Dough Exorcist (execute + Heavy bonus), Crust Judgment Protocol (crit <50% HP = kill), sustain de Taste Tester ao longo do fight longo.

### 4.6 Microwave Meal Preacher (`enemy_preacher`)
- **Stats**: 200 HP / 20 speed / 25 gold / 2 dmg / Heavy
- **Análise**: ✅ Razoável — "preacher" convertido pela heresia, Heavy dá resiliência tier2.
- **Forte contra ele**: dano sustido (Baker/Courier upgradadas), Dough Exorcist por ser Heavy.

### 4.7 Plastic Wrapped Sandwich Man (`enemy_sandwich`)
- **Stats**: 180 HP / 28 speed / 22 gold / 2 dmg
- **Análise**: ⚠️ "Plastic wrapped" sugere proteção, tem HP acima da média para o seu tier, mas **não é marcado `IsHeavy`**. *(Ver 10.2)*
- **Forte contra ele**: dano sustido geral; não tem fraqueza codificada.

### 4.8 Frozen Dough Abomination (`enemy_abomination`)
- **Stats**: 400 HP / 10 speed (o mais lento do jogo) / 35 gold / 3 dmg / Heavy
- **Análise**: ✅ Excelente — "frozen" = lento, "abomination" = tanque feio. Um dos melhores matches nome/stats do roster.
- **Forte contra ele**: qualquer torre de dano sustido; Dough Exorcist pelo Heavy bonus.

### 4.9 The Gluten Null Bishop (`enemy_gluten_bishop`) — BOSS
- **Stats**: 1500 HP / 14 speed / 120 gold / 6 dmg / Boss + Heavy + **Anti-Buff Aura** (60px, -50% a todos os bónus percentuais multiplicativos)
- **Análise**: ✅ O melhor match lore/mecânica do jogo — "Gluten Null" opõe-se diretamente ao tema da Order, e mecanicamente "anula" (null) os buffs.
- **Forte contra ele**: Crust Crusader / Crust Judgment Protocol (imunes ao debuff — ver 10.1), dano base alto sem depender de % bonuses.

### 4.10 Supermarket Overlord of White Bread (`enemy_overlord`)
- **Stats**: 900 HP / 18 speed / 80 gold / 4 dmg / Heavy (não é Boss, usado como miniboss)
- **Análise**: ✅ "Overlord of White Bread" = antítese do sourdough, elite antes do boss final faz sentido narrativo.
- **Forte contra ele**: idêntico ao Dragon.

---

## 5. Sinergias

### 5.1 One Whiff, One Bite (`one_whiff_one_bite`)
- **Requisito**: Aroma Keeper + Taste Tester → **Efeito**: +15% dmg a ambas
- O slow mantém o alvo mais tempo em range, aumentando na prática os ticks de poison aplicados.
- **Forte contra**: Grocery Run Jogger e Pigeon (rápidos), e tanques (Alley Cat, Abomination) por sustain de poison.

### 5.2 Grand Opening Rush (`grand_opening_rush`)
- **Requisito**: 3+ tipos de torre no mapa → **Efeito**: +10% FR a todas
- Bónus genérico, recompensa diversidade de loadout. Mais fácil de ativar.
- **Forte contra**: hordas (`WaveModifier.Horde`) e waves tier3 com múltiplos tipos.

### 5.3 Holy Fermentation Network (`holy_fermentation_network`)
- **Requisito**: Aroma Keeper + Fermentation Sage → **Efeito**: +30% alcance de slow/chain
- Nome liga os dois temas de "fermentação", mas como a Sage não tem poison/slow próprios, o "slow" no nome refere-se só ao do Aroma Keeper.
- **Forte contra**: grupos compactos de inimigos (Horde modifier).

### 5.4 Crust Judgment Protocol (`crust_judgment_protocol`)
- **Requisito**: Crust Crusader + Dough Exorcist → **Efeito**: crit contra alvo <50% HP = execute instantâneo, cooldown 10s
- A sinergia mais "boss-killer" do jogo. Cooldown de 10s limita-a a um alvo de cada vez.
- **Forte contra**: Alley Cat, Frozen Dough Abomination, Supermarket Overlord, e especialmente os dois bosses.
- **Nota**: por depender de Crit (não de %dmg), esta sinergia **ignora a anti-buff aura do Gluten Null Bishop** — resposta mecanicamente "correta" a esse boss, mesmo não desenhada explicitamente para isso.

---

## 6. Equipamentos — Resumo Numérico

| Torre | Equip | Cost | Efeito Principal | Trade-off | Tipo |
|---|---|---|---|---|---|
| Bread Baker | Stone Oven | 80 | +15% dmg | — | Puro |
| Bread Baker | Ancient Starter | 120 | +10% dmg, +1 dmg flat/10 ataques (stack, persiste na run) | — (ramp-up) | Scaling |
| Bread Courier | Electric Bike | 80 | +20% FR | — | Puro |
| Bread Courier | Messenger Crate | 120 | +1 pierce | -10% FR | Trade-off |
| Aroma Keeper | Megaphone | 80 | +20% range | — | Puro |
| Aroma Keeper | Spice Wind Chimes | 120 | +30% slow duration | -10% dmg | Trade-off |
| Taste Tester | Silver Tray | 80 | +20% poison dmg | — | Puro |
| Taste Tester | Double Sampling | 120 | poison em +1 alvo extra (30px do centro da torre) | -15% FR | Trade-off |
| Bakery Truck | Reinforced Suspension | 80 | +15% splash radius | — | Puro |
| Bakery Truck | Street Parade | 120 | +30% dmg | -20% range | Trade-off |
| Bread Monk | Sacred Robes | 100 | +15% aura range | — | Puro |
| Bread Monk | Prayer Beads | 150 | aura ×1.05 (10%→10.5%) | **torre para de atacar** | Trade-off extremo |
| Fermentation Sage | Golden Proofing Bowl | 100 | +15% duração de status | — | ⚠️ **morto atualmente** |
| Fermentation Sage | Wild Yeast | 150 | +1 bounce extra | -10% dmg inicial | Trade-off |
| Crust Crusader | Tempered Crust Blade | 100 | +10 p.p. crit chance (15%→25%) | — | Puro |
| Crust Crusader | Blessed Crunch Seal | 150 | crit faz mini-splash (r=30) | -10% FR | Trade-off |
| Dough Exorcist | Holy Flour Pouch | 100 | +20% dmg vs Boss/Heavy | — | Puro |
| Dough Exorcist | Judgment Seal | 150 | execute instantâneo <15% HP (cd 5s) | -15% dmg | Trade-off |
| High Prophet | Golden Staff | 120 | +10% dmg **+10% FR +10% range** (3 stats) | — | Puro (melhor $/valor?) |
| High Prophet | First Starter Relic | 180 | +5% dmg raw por torre próxima (40px) | -15% range | Trade-off |

**Padrão de preço confirmado**: itens "puros" (sem downside) custam 80-100g; itens com trade-off ou mecânica extra custam 120-180g. Dois outliers a validar: **Ancient Starter** (120g, sem trade-off real, só ramp-up) e **Golden Staff** (120g, +10% em 3 stats sem trade-off — mesmo preço de itens de 1 stat).

---

## 7. Equipamentos — Detalhe por Torre

### 7.1 Bread Baker
**Stone Oven** (80g) — `+15% dano`. Puro. Lore ✅ (forno de pedra = mais calor = mais dano).
**Ancient Starter** (120g) — `+10% dano` + `+1 dano flat a cada 10 ataques`, stacks **persistem entre fights dentro da run** (via `RunState.SetAncientStarterStacks`, resetam só em `EndRun`). Já usa `AttackStackDamageFlat` corretamente (data-driven confirmado). Lore ✅. *(Ver 10.4)*

### 7.2 Bread Courier
**Electric Bike** (80g) — `+20% fire rate`. Puro. Lore ✅.
**Messenger Crate Upgrade** (120g) — `+1 pierce` (via query de física 200px, alvo dinâmico), `-10% FR`. Lore ✅. **Forte contra**: Horde ou inimigos em fila.

### 7.3 Aroma Keeper
**Megaphone** (80g) — `+20% range`. Puro. Lore aceitável por analogia.
**Spice Wind Chimes** (120g) — `+30% slow duration`, `-10% dano`. Lore forte. **Forte contra**: Grocery Jogger, Pigeon.

### 7.4 Taste Tester
**Silver Tray** (80g) — `+20% poison dmg`. Puro. Lore ✅.
**Double Sampling Plates** (120g) — poison a +1 alvo extra, `-15% FR`. ⚠️ Raio de 30px medido a partir da **posição da torre**, não do alvo atingido. *(Ver 10.4)*

### 7.5 Bakery Truck
**Reinforced Suspension** (80g) — `+15% splash radius` (36→41.4). Puro. Lore ✅.
**Street Parade Route** (120g) — `+30% dano`, `-20% range`. Lore ✅. **Forte contra**: grupos densos, Horde modifier.

### 7.6 Bread Monk
**Sacred Robes** (100g) — `+15% aura range`. Puro. Lore ✅.
**Prayer Beads of Gluten** (150g) — `AuraPotencyMultiplier=1.05` (aura sobe 10%→10.5%) e **desativa o ataque** (`DisablesAttack=true`). ⚠️ Custo (perder até 75 DPS pessoais) muito acima do benefício (+0.5 p.p. de aura). *(Ver 10.4)*

### 7.7 Fermentation Sage
**Golden Proofing Bowl** (100g) — `+15% duração de efeito`. ⚠️ **Item morto, confirmado no código**: a Sage tem `HasPoison=false`/`HasSlow=false`, nunca cria efeitos de poison/slow (nem diretamente nem via chain), logo este equip multiplica a duração de um efeito que nunca existe. *(Ver 10.1 e 10.4 — prioridade alta)*
**Wild Yeast Culture** (150g) — `+1 bounce extra`, `-10% dano inicial`. Funciona corretamente. Lore ✅. **Forte contra**: grupos de 3+ inimigos.

### 7.8 Crust Crusader
**Tempered Crust Blade** (100g) — `+10 p.p. crit chance` (15%→25%, adição direta). Puro. Lore ✅.
**Blessed Crunch Seal** (150g) — crits fazem mini-splash raio 30, `-10% FR`. Lore ✅. **Forte contra**: grupos de HP baixo/médio perto do alvo principal.

### 7.9 Dough Exorcist
**Holy Flour Pouch** (100g) — `+20% dano vs Boss/Heavy`. Puro, combina com o ×1.5 nativo do execute → total ×1.8 em elite/boss. Lore ✅. **Forte contra**: os dois bosses e 6 dos 10 inimigos (Heavy).
**Judgment Seal** (150g) — execute <15% HP, cooldown 5s **próprio** (`_judgmentSealCooldown`, campo separado de `_judgmentProtocolCooldown` da sinergia — nota antiga da memória sobre partilharem campo já não se aplica ao código atual). `-15% dano`. Lore ✅.

### 7.10 High Prophet of Sourdough
**Golden Staff of Fermentation** (120g) — `+10% dmg/+10% FR/+10% range`, sem downside. Lore ✅. *(Ver 10.4)*
**First Starter Relic** (180g) — `+5% dano raw por torre próxima em 40px` (baseado no dmg base da torre, não upgradado), `-15% range`. Com cap de 5 torres/mapa, max teórico +8 dano flat. O trade-off de range conflita com a necessidade de ficar perto de aliados. Lore ✅.

---

## 8. Trinkets — Resumo Numérico

| Trinket | Rarity | Efeito | Downside | Escopo |
|---|---|---|---|---|
| Secret Recipe Scroll | Common | +10% dano global | — | Todas as torres |
| Starter's Blessing | Common | +5 vidas imediato | — | One-shot |
| Regular's Tip Jar | Common | +100 gold imediato | — | One-shot |
| Proofing Time Candle | **Rare** | +8% FR global | -5% range global | Todas as torres |
| Crust Fragment Relic | Common | +12% crit dmg | — | ⚠️ só Crust Crusader |
| Fermentation Diary | Common | +15% duração de status | — | Aroma Keeper + Taste Tester |
| Sacred Flour Dust | Common | +10% força de slow/poison | — | Aroma Keeper + Taste Tester |
| Heretic Census List | Common | +10% dano vs inimigos "basic" (não-Heavy/Boss) | — | Todas as torres, situacional |
| Oven Heart Ember | Common | +1 range flat global | — | Todas as torres |
| First Starter Vessel | Common | +5 gold /30s | — | Economia passiva |

---

## 9. Trinkets — Detalhe Individual

### 9.1 Secret Recipe Scroll
`+10% dano global`, sem downside, todas as torres. Lore ✅. Provavelmente o trinket mais forte "pronto a usar" da lista, apesar de Common. *(Ver 10.5)*

### 9.2 Starter's Blessing
`+5 vidas`, one-shot. Lore ✅. Valor depende do estado de vidas no momento.

### 9.3 Regular's Tip Jar
`+100 gold`, one-shot. Lore ✅. Equivalente a ~1 torre tier1 grátis.

### 9.4 Proofing Time Candle
`+8% FR`, `-5% range`, todas as torres, marcado **Rare** (único do jogo). ⚠️ Único trinket com downside, e mesmo assim o único Rare — parece mais fraco que vários Commons sem trade-off. Confirmado no código que `Rarity` **não pesa na probabilidade de seleção** (`TrinketChoiceScreen.BuildChoices` usa `GD.RandRange` puramente uniforme) — serve só para a cor da borda do card. *(Ver 10.5)*

### 9.5 Crust Fragment Relic
`+12% crit dmg`, mas só lido dentro do `if (_data.HasCrit)` em `Tower.ApplyData` — **não faz nada** sem Crust Crusader no loadout. *(Ver 10.5)*

### 9.6 Fermentation Diary
`+15% duração de status`, funciona corretamente em Aroma Keeper/Taste Tester (as únicas torres com `HasPoison`/`HasSlow` nativo hoje). Lore ✅.

### 9.7 Sacred Flour Dust
`+10% força de slow/poison`, funciona corretamente nas mesmas duas torres. Lore ✅.

### 9.8 Heretic Census List
`+10% dano vs inimigos não-Heavy/Boss` (4 dos 10 inimigos). Forte em tier1/tier2, fraco em tier3/boss (maioria Heavy). Lore ✅.

### 9.9 Oven Heart Ember
`+1px range flat`, todas as torres. Com ranges de 44-56px, é ~2% de aumento — valor muito pequeno. *(Ver 10.5)*

### 9.10 First Starter Vessel
`+5 gold /30s`, sem downside. Numa run de 20-30min soma **~200-300 gold total**. Lore ✅. Um dos trinkets mais fortes a longo prazo apesar de Common.

---

## 10. Questões Abertas — Consolidado

### 10.1 Torres
1. **Bread Monk**: os upgrades só reforçam o ataque pessoal, nunca a % da aura. Intencional?
2. **Fermentation Sage**: sem `HasPoison`/`HasSlow` nativo, apesar do tema partilhar lore direto com Aroma Keeper/Taste Tester — e isto quebra um equip inteiro (ver 10.4 #1).
3. **High Prophet**: 720g de investimento total para um teto fixo de +10% dmg global (regra de 5 torres/mapa). O valor real parece estar no DPS pessoal (91), não na aura — alinhado com a intenção do "capstone tower"?
4. **Dough Exorcist**: stack ×2 (execute) × ×1.5 (elite) = ×3 contra bosses <20% HP. Confirmar se os HP pools dos bosses (1000/1500) aguentam sem trivializar o encontro.
5. **Crit vs Anti-Buff Aura**: o Gluten Null Bishop não debuffa Crit Chance/Multiplier, só bónus percentuais de dmg/FR/range — torna o Crust Crusader/Crust Judgment Protocol um "hard counter" implícito. Intencional? Vale comunicar isto ao jogador (ex: bestiary hint)?
6. **Bakery Truck splash radius**: nota antiga da memória do projeto ("suspected bug, 5.0 vs 36f") já não se verifica no `.tres` atual (está em 36.0) — confirmar que está resolvido.

### 10.2 Inimigos
7. **Pigeon with a Stolen Baguette**: lore promete "voador" mas segue o path normal — aceitável como placeholder até Camada 2 (flying-enemy targeting), já no `ROADMAP.md`.
8. **Plastic Wrapped Sandwich Man**: nome sugere proteção/resistência mas não tem `IsHeavy=true`. Considerar marcar como Heavy.
9. **Lazy Alley Cat**: falta `FlavorText` (ainda "TODO") — beneficiaria de uma frase que reforce "gato preguiçoso e gordo" para justificar o HP alto.

### 10.3 Sinergias
Sem questões adicionais além da nota de Crit vs Anti-Buff já coberta em 10.1 #5.

### 10.4 Equipamentos
10. **Golden Proofing Bowl (Fermentation Sage)**: não faz nada, confirmado por código — prioridade alta. Resolve-se em conjunto com 10.1 #2.
11. **Prayer Beads (Bread Monk)**: trade-off desequilibrado — perde-se ~75 DPS por +0.5 p.p. de aura. Recomenda-se rever `AuraPotencyMultiplier` (ex: 1.3-1.5) ou reformular o efeito.
12. **Double Sampling Plates (Taste Tester)**: raio de spread de poison medido a partir da torre, não do alvo atingido. Confirmar se é a leitura pretendida.
13. **Golden Staff (High Prophet)**: parece ter melhor rácio custo/benefício (3 stats a +10% por 120g) que itens de single-stat ao mesmo preço. Confirmar se é intencional como prémio da torre mais cara, ou se deve subir de preço.
14. **Ancient Starter (Bread Baker)**: sem downside real, acumula indefinidamente durante a run. Confirmar teto/ritmo pretendido para runs longas.

### 10.5 Trinkets
15. **Rarity é puramente cosmética** — não pesa na probabilidade de aparecer nem está correlacionada com força real.
16. **Proofing Time Candle (Rare)** parece mais fraco que vários Commons sem trade-off (em particular o Secret Recipe Scroll). Decidir se sobe de poder, se se implementa peso de raridade na seleção, ou se se aceita que "Rare" é só uma etiqueta visual.
17. **Crust Fragment Relic**: só tem efeito com Crust Crusader no loadout — confirmar se é aceitável para um trinket "run-wide".
18. **Oven Heart Ember**: +1px de range é um valor muito pequeno, possivelmente abaixo do limiar de perceção em jogo.

---

## 11. Próximos Passos a Validar

Ordem sugerida (mecânica antes de números — não adianta afinar % em cima de efeitos que não existem):

1. **Decidir o destino da Fermentation Sage** (10.1 #2 + 10.4 #10): dar-lhe `HasSlow`/`HasPoison` nativo fraco (resolve lore + reativa o Golden Proofing Bowl de uma vez), ou trocar o equip por um efeito que não dependa de status.
2. **Rever Prayer Beads** (10.4 #11): novo valor de `AuraPotencyMultiplier` ou reformular o efeito, dado o trade-off atual não compensar.
3. **Decidir sobre Rarity dos trinkets** (10.5 #15/#16): implementar peso na seleção, subir o poder do Proofing Time Candle, ou aceitar que é só cosmético.
4. **Confirmar interação Crit vs Anti-Buff Aura** (10.1 #5): manter como "counter" implícito ao Gluten Null Bishop, ou alinhar com o resto do sistema.
5. **Confirmar HP dos bosses vs burst do Dough Exorcist** (10.1 #4): jogar/simular um fight de boss para ver se o ×3 execute trivializa o encontro.
6. **Decidir sobre Golden Staff** (10.4 #13): manter preço a 120g ou subir para alinhar com o padrão dos outros itens "capstone".
7. **Pequenas flags de lore/dados**: `IsHeavy` no Sandwich Man, `FlavorText` do Alley Cat, `Bread Monk` upgrades vs aura (10.1 #1), `Ancient Starter` teto de stacks (10.4 #14).
8. **Só depois disto**: passar por todos os números (dano, custo, FR, ranges, %s) num pass de afinação geral, agora que a mecânica subjacente está confirmada e sem itens "mortos".
