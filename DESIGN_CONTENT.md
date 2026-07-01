# Design de Conteúdo — Tower Defense × Roguelite

> Documento de alinhamento para implementação. Tudo aqui é a fonte de verdade
> para nomes, stats, mecânicas e trade-offs. Alterações passam por este doc primeiro.

---

## 1. Towers (10)

### 1.1 Bread Baker
| Campo | Valor |
|---|---|
| Damage | 18 |
| Fire Rate | 1.0 |
| Range | 48 |
| Cost | 100 |
| Mechanic | — (base single-target) |

### 1.2 Bread Courier
| Campo | Valor |
|---|---|
| Damage | 16 |
| Fire Rate | 1.2 |
| Range | 46 |
| Cost | 120 |
| Mechanic | — (base single-target, rápido) |

### 1.3 Aroma Keeper
| Campo | Valor |
|---|---|
| Damage | 15 |
| Fire Rate | 0.9 |
| Range | 50 |
| Cost | 140 |
| Mechanic | **Slow** — aplica slow multiplier no alvo |

### 1.4 Taste Tester
| Campo | Valor |
|---|---|
| Damage | 17 |
| Fire Rate | 1.0 |
| Range | 48 |
| Cost | 160 |
| Mechanic | **Poison** — aplica poison DoT no alvo |

### 1.5 Bakery Truck
| Campo | Valor |
|---|---|
| Damage | 20 |
| Fire Rate | 0.8 |
| Range | 44 |
| Cost | 180 |
| Mechanic | **Splash** — dano em área no impacto |

### 1.6 Bread Monk
| Campo | Valor |
|---|---|
| Damage | 19 |
| Fire Rate | 1.0 |
| Range | 50 |
| Cost | 220 |
| Mechanic | **Aura Buff** — aumenta Damage + Fire Rate de torres próximas |

**Notas de implementação:**
- Range da aura: ~60px (configurável em TowerData)
- Buff é percentual, aplicado via signal/sistema similar a synergies
- Torres afetadas mostram indicador visual (glow/ícone)
- Stack: múltiplos Monks não stackam no mesmo alvo (apenas o maior)

### 1.7 Fermentation Sage
| Campo | Valor |
|---|---|
| Damage | 18 |
| Fire Rate | 0.9 |
| Range | 52 |
| Cost | 250 |
| Mechanic | **Chain** — ataques propagam efeitos para inimigo mais próximo |

**Notas de implementação:**
- Chain base: 1 bounce (configurável)
- Propaga efeitos de status (slow, poison) — se a torre tiver equip que os adiciona
- Chain priority: enemy mais próximo dentro de X range do alvo atual
- Dano inicial normal, bounce com dano reduzido (50%)

### 1.8 Crust Crusader
| Campo | Valor |
|---|---|
| Damage | 22 |
| Fire Rate | 0.9 |
| Range | 46 |
| Cost | 300 |
| Mechanic | **Critical Hits** — chance de crítico (dano aumentado + impacto forte) |

**Notas de implementação:**
- Crit chance base: 15%
- Crit multiplier base: 2.0x
- Hit-flash especial para críticos (vermelho)
- Som de impacto mais pesado

### 1.9 Dough Exorcist
| Campo | Valor |
|---|---|
| Damage | 20 |
| Fire Rate | 1.0 |
| Range | 50 |
| Cost | 340 |
| Mechanic | **Execute / Anti-Elite** — dano extra vs alvos com HP baixo e bónus vs elites/boss |

**Notas de implementação:**
- `ExecuteThreshold`: 20% HP — quando alvo abaixo, dano x2
- `EliteBonusMultiplier`: 1.5x vs inimigos marcados como elite/boss
- Dois bónus empilham (abaixo de 20% HP + elite = x3)

### 1.10 High Prophet of Sourdough
| Campo | Valor |
|---|---|
| Damage | 24 |
| Fire Rate | 0.9 |
| Range | 56 |
| Cost | 400 |
| Mechanic | **Global Aura** — buff global que escala com número de torres em campo |

**Notas de implementação:**
- `DamagePerTower`: +2% damage global por torre amiga em campo (inclui ela própria)
- Range do buff: global
- Recalculado sempre que torre é colocada/removida
- Indicador visual no HUD: "Prophet's Blessing: +X%"

---

## 2. Equipment (20 — 2 por tower)

### 2.1 Bread Baker
| Equip | Efeito | Trade-off | Cost |
|---|---|---|---|
| Stone Oven | +15% Damage | — | 80 |
| Ancient Starter | +10% Damage; cada 10 ataques +1 Damage (reset run) | — | 120 |

**Notas de implementação (Ancient Starter):**
- Contador de ataques por instância da torre
- `_attackCount` incrementa a cada tiro
- Ao atingir múltiplo de 10, `EffectiveDamage += 1` (acumulativo)
- Reset quando torre é vendida; NÃO reset entre fights na mesma run

### 2.2 Bread Courier
| Equip | Efeito | Trade-off | Cost |
|---|---|---|---|
| Electric Bike | +20% Fire Rate | — | 80 |
| Messenger Crate Upgrade | +1 pierce target | -10% Fire Rate | 120 |

**Notas de implementação (Messenger Crate):**
- `PierceCount`: quantos inimigos o projectile atravessa antes de desaparecer
- Base = 0 (morre no primeiro impacto). Com equip = 1 (atinge 2 inimigos)
- Projectile mantém velocidade/dano no inimigo seguinte

### 2.3 Aroma Keeper
| Equip | Efeito | Trade-off | Cost |
|---|---|---|---|
| Megaphone | +20% Range | — | 80 |
| Spice Wind Chimes | +30% Slow Duration | -10% Damage | 120 |

### 2.4 Taste Tester
| Equip | Efeito | Trade-off | Cost |
|---|---|---|---|
| Silver Tray | +20% Poison Damage | — | 80 |
| Double Sampling Plates | aplica poison a +1 alvo extra | -15% Fire Rate | 120 |

**Notas de implementação (Double Sampling):**
- Ao disparar, se o alvo primário está em range, aplica poison também ao inimigo mais próximo dentro de range reduzido
- `PoisonSpreadRange`: ~30px

### 2.5 Bakery Truck
| Equip | Efeito | Trade-off | Cost |
|---|---|---|---|
| Reinforced Suspension | +15% Splash Radius | — | 80 |
| Street Parade Route | +30% Area Damage | -20% Range | 120 |

### 2.6 Bread Monk
| Equip | Efeito | Trade-off | Cost |
|---|---|---|---|
| Sacred Robes | +15% Aura Range | — | 100 |
| Prayer Beads of Gluten | buffs +5% stronger | Monk não ataca | 150 |

**Notas de implementação (Prayer Beads):**
- `CanAttack = false` — torre não dispara
- `AuraBuffBonus *= 1.05`
- Visual: torre fica em pose de oração, sem projectile

### 2.7 Fermentation Sage
| Equip | Efeito | Trade-off | Cost |
|---|---|---|---|
| Golden Proofing Bowl | +15% Effect Duration | — | 100 |
| Wild Yeast Culture | chain +1 extra bounce | -10% initial Damage | 150 |

### 2.8 Crust Crusader
| Equip | Efeito | Trade-off | Cost |
|---|---|---|---|
| Tempered Crust Blade | +10% Crit Chance | — | 100 |
| Blessed Crunch Seal | crits fazem mini-splash (radius 30) | -10% Fire Rate | 150 |

**Notas de implementação (Blessed Crunch Seal):**
- Apenas projecteis que critam criam área de dano
- Splash não stacka com Bakery Truck (usa o mesmo sistema)

### 2.9 Dough Exorcist
| Equip | Efeito | Trade-off | Cost |
|---|---|---|---|
| Holy Flour Pouch | +20% Damage vs elites/boss | — | 100 |
| Judgment Seal | executa abaixo de 15% HP | -15% Damage | 150 |

**Notas de implementação (Judgment Seal):**
- Se alvo HP% < 15%, mata instantaneamente (não aplica dano normal)
- Cooldown interno: 5s para evitar chain-execute em waves pequenas
- Visual: raio de luz/partículas brancas no inimigo

### 2.10 High Prophet of Sourdough
| Equip | Efeito | Trade-off | Cost |
|---|---|---|---|
| Golden Staff of Fermentation | +10% All Stats (dmg, FR, range) | — | 120 |
| First Starter Relic | +5% Damage por torre próxima (range 40) | -15% Range | 180 |

**Notas de implementação (First Starter Relic):**
- `NearbyTowerRange`: 40px — torres dentro deste range contam para o bónus
- Bónus é multiplicativo por torre: `1 + (0.05 * nearbyCount)`
- Não inclui ela própria

---

## 3. Trinkets (10)

| # | Nome | Efeito |
|---|---|---|
| 1 | Secret Recipe Scroll | +10% Damage global |
| 2 | Starter's Blessing | +5 lives |
| 3 | Regular's Tip Jar | +100 gold start |
| 4 | Proofing Time Candle | +8% Attack Speed, -5% Range |
| 5 | Crust Fragment Relic | +12% Crit Damage (multiplicativo) |
| 6 | Fermentation Diary | +15% status effect duration (slow, poison) |
| 7 | Sacred Flour Dust | +10% slow/poison strength |
| 8 | Heretic Census List | +10% damage vs basic enemies (não elite/boss) |
| 9 | Oven Heart Ember | +1 starting tower range (small global aura, ~10px) |
| 10 | First Starter Vessel | cada 30s ganha +5 gold passivo |

**Notas de implementação:**
- Trinkets com trade-off (#4) usam valores negativos nos campos existentes
- #5 Crust Fragment Relic: multiplica o dano crítico base (2.0x → 2.24x)
- #8 Heretic Census List: `IsBasicEnemy` flag nos EnemyData
- #9 Oven Heart Ember: aura passiva global, +10px range a todas as torres
- #10 First Starter Vessel: timer no RunState, incrementa gold a cada 30s

---

## 4. Shop Items (5)

| # | Nome | Efeito | Cost |
|---|---|---|---|
| 1 | Secret Ingredient | +5% Damage global | 80 |
| 2 | Fresh Batch | +5% Fire Rate global | 60 |
| 3 | Golden Proof Flour | +5% Damage vs armored/heavy enemies | 90 |
| 4 | Rapid Oven Upgrade | +8% Fire Rate, -5% Range (trade-off) | 100 |
| 5 | Discounted Starter Yeast | primeira compra na shop custa -30% gold | 120 |

**Notas de implementação:**
- #3 Golden Proof Flour: mesma flag que execute/anti-elite, mas só para enemies com `IsHeavy` ou `IsBoss`
- #4 Rapid Oven Upgrade: shop item com trade-off (range negativo é incomum para shop items, mas adiciona profundidade)
- #5 Discounted Starter Yeast: aplica à primeira compra da run (flag detectada via RunState). Se for a primeira compra, custo *= 0.7

---

## 5. Synergies (4)

| # | Nome | Requisitos | Efeito |
|---|---|---|---|
| 1 | One Whiff, One Bite (mantido) | Aroma Keeper + Taste Tester | +15% Damage a ambos |
| 2 | Grand Opening Rush (mantido) | 3+ tower types | +10% Fire Rate a todas |
| 3 | Holy Fermentation Network | Aroma Keeper + Fermentation Sage | slow/chain effects spread +30% further |
| 4 | Crust Judgment Protocol | Crust Crusader + Dough Exorcist | críticos contra <50% HP causam execução instantânea (cooldown 10s interno) |

**Notas de implementação:**
- #3 Holy Fermentation Network: range de propagação de status aumentado em 30%
- #4 Crust Judgment Protocol: quando Crust Crusader crita um alvo abaixo de 50% HP, aplica instakill. Cooldown de 10s para não trivializar bosses

---

## 6. Enemies (10 — 2 bosses)

| # | Nome | HP | Speed | Gold | Damage | Boss? | Notas |
|---|---|---|---|---|---|---|---|
| 1 | Sliced Bread Tourist | 100 | 24 | 10 | 1 | — | básico |
| 2 | Grocery Run Jogger | 60 | 48 | 15 | 1 | — | rápido, fraco |
| 3 | Lazy Alley Cat | 300 | 12 | 30 | 2 | — | tanque early |
| 4 | Pigeon w/ Stolen Baguette | 80 | 32 | 20 | 1 | — | voador irritante |
| 5 | Industrial Bread Dragon | 1000 | 16 | 100 | 5 | **BOSS** | boss final |
| 6 | Microwave Meal Preacher | 200 | 20 | 25 | 2 | — | médio "convencido" |
| 7 | Plastic Wrapped Sandwich Man | 180 | 28 | 22 | 2 | — | resistência média |
| 8 | Frozen Dough Abomination | 400 | 10 | 35 | 3 | — | tanque lento |
| 9 | The Gluten Null Bishop | 1500 | 14 | 120 | 6 | **BOSS** | anti-buff (reduz buffs de torres em área) |
| 10 | Supermarket Overlord of White Bread | 900 | 18 | 80 | 4 | — | mini-boss/elite final |

**Notas de implementação (Gluten Null Bishop):**
- Aura de 60px que reduz buffs percentuais das torres em 50% (synergies, equip, trinkets — não afeta base stats)
- Indicador visual: aura roxa/negra à volta do boss
- Prioridade de alvo: torres devem focá-lo primeiro

**Notas de implementação (Supermarket Overlord):**
- Não é boss mas tem stats de mini-boss
- Pode ser usado como miniboss da slot machine

---

## 7. Mecânicas Especiais — Implementação

### Aura System (Bread Monk, High Prophet)
```
[Component] AuraComponent
- range: float
- statType: enum { Damage, FireRate, Range, All }
- bonusPercent: float
- isGlobal: bool
- exclusiveTag: string (prevents stacking same type)

Recalcula em: _Ready(), tower placed, tower removed
Aplica via: signal para cada torre afetada (re-aplica TowerData com bónus)
```

### Chain System (Fermentation Sage)
```
[Component] ChainComponent
- bounceCount: int
- bounceRange: float
- bounceDamageMultiplier: float
- propagateEffects: bool (slow, poison)

OnHit(target):
    if bounceCount > 0:
        nearest = findNearestEnemy(target, bounceRange)
        if nearest:
            dealDamage(nearest, damage * bounceDamageMultiplier)
            propagateStatusEffects(nearest)
```

### Crit System (Crust Crusader)
```
[Component] CritComponent
- critChance: float
- critMultiplier: float
- onCritEffects: list

OnFire():
    roll = rand()
    isCrit = roll < critChance
    damage = isCrit ? damage * critMultiplier : damage
```

### Execute System (Dough Exorcist)
```
[Component] ExecuteComponent
- thresholdHPPercent: float
- executeMultiplier: float
- eliteBonusMultiplier: float
- judgmentCooldown: float (for Judgment Seal equip)

OnHit(target):
    if HPPercent(target) < thresholdHPPercent:
        damage *= executeMultiplier
    if target.IsEliteOrBoss:
        damage *= eliteBonusMultiplier
```

### Critical Hit Formula Final
```
EffectiveCritChance = baseCritChance (0.15)
    + equipBonus
    + trinketBonus
    + synergyBonus

EffectiveCritMultiplier = baseCritMultiplier (2.0)
    * (1 + trinketBonus)   — Crust Fragment Relic: 0.12
```

### Effective Damage Formula (atualizada)
```
EffectiveDamage = baseDamage
    * (1 + synergyBonus + equipBonus)         — additive layer 1
    * (1 + shopBonus)                         — additive layer 2
    * (1 + metaBonus)                         — additive layer 3
    * (1 + trinketBonus)                      — additive layer 4
    * (1 + globalAuraBonus)                   — High Prophet
    * critMultiplier (if crit)
    * executeMultiplier (if below threshold)
```

---

## 8. Upgrade Data (2 por tower — existentes, renomear)

| Tower | Upgrade 1 | Upgrade 2 |
|---|---|---|
| Bread Baker | Reinforce (+10 dmg, 75g) | Heavy Barrel (+15 dmg, +0.2 FR, 125g) |
| Bread Courier | Overclock (+0.5 FR, 100g) | Precision Rounds (+5 dmg, +0.5 FR, 150g) |
| Aroma Keeper | Cryo Coil (+0.3 FR, 100g) | Permafrost (+10 dmg, +0.2 FR, 150g) |
| Taste Tester | Toxic Vial (+1 dmg, 125g) | Concentrated Venom (+2 dmg, +0.3 FR, 175g) |
| Bakery Truck | Explosive Charge (+15 dmg, 150g) | Nova Blast (+20 dmg, +12 range, 200g) |
| Bread Monk | Sacred Chant (+5 dmg, +0.2 FR, 120g) | Devotion Aura (+8 dmg, +0.3 FR, 180g) |
| Fermentation Sage | Extended Culture (+5 dmg, +0.2 FR, 130g) | Wild Fermentation (+8 dmg, +0.3 FR, 190g) |
| Crust Crusader | Sharpened Crust (+6 dmg, +0.2 FR, 150g) | Holy Crusher (+10 dmg, +0.3 FR, 220g) |
| Dough Exorcist | Blessed Flour (+6 dmg, +0.2 FR, 160g) | Judgment Aura (+10 dmg, +0.3 FR, 240g) |
| High Prophet | Sourdough Sermon (+6 dmg, +0.2 FR, 200g) | Great Fermentation (+10 dmg, +0.3 FR, 300g) |

---

## 9. Prioridade de Implementação

| Fase | O que | Depende de |
|---|---|---|
| 1 | Renomear towers existentes + atualizar stats nos .tres | Nada |
| 2 | Implementar Bread Monk (AuraComponent) | Fase 1 |
| 3 | Implementar Fermentation Sage (ChainComponent) | Fase 1 |
| 4 | Implementar Crust Crusader (CritComponent) | Fase 1 |
| 5 | Implementar Dough Exorcist (ExecuteComponent) | Fase 1 |
| 6 | Implementar High Prophet (GlobalAuraComponent) | Fase 1 |
| 7 | Equipment expansion — todos os 20 | Fases 1-6 (depende das towers) |
| 8 | Trinkets expansion — +7 novos | Fase 7 |
| 9 | Shop items expansion — +3 novos | Nada |
| 10 | Synergies — +2 novas | Fases 2-5 |
| 11 | Enemies expansion — +5 novos (incl. Gluten Null Bishop) | Nada |
| 12 | Balancing pass — testar tudo junto | Fases 1-11 |
| 13 | Meta-upgrade unlocks (+5 para novas towers) | Fases 2-6 |
| — | Sound system (polish) | Tudo |

---

> Este documento é a fonte de verdade. Antes de qualquer implementação,
> verificar aqui os valores e mecânicas. Alterações propostas passam por
> este doc primeiro.
