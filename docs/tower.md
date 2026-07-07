# Towers

> DPS formula: `Damage × FireRate` (FireRate = attacks/second).
> DPS shown is raw — without synergy/equip/shop/meta/trinket bonuses.

## Overview

| Tower | Cost | Dmg | FR | Range | DPS Base | DPS Full Upgrade | Total Invested | Special |
|-------|------|-----|----|-------|----------|------------------|----------------|---------|
| Bread Baker | 75 | 35 | 1.0 | 48 | 35 | 84 | 235 | — |
| Bread Courier | 90 | 28 | 1.5 | 44 | 42 | 91.2 | 290 | Fast projectile |
| Aroma Keeper | 100 | 25 | 0.9 | 50 | 22.5 | 49 | 300 | Slow (0.5x, 3s) |
| Taste Tester | 120 | 30 | 1.0 | 48 | 30 (+10 poison) | 55.9 (+10 poison) | 360 | Poison (10/s, 4s) |
| Bakery Truck | 140 | 35 | 0.8 | 44 | 28/target | 56/target | 420 | Splash (r=36) |
| Bread Monk | 160 | 30 | 1.0 | 50 | 30 | 75 | 400 | Aura (+10% dmg/rate, r=60) |
| Fermentation Sage | 180 | 30 | 0.9 | 52 | 27 (+13.5 chain) | 70 (+35 chain) | 430 | Chain (1 bounce, 40r, 50% dmg) |
| Crust Crusader | 220 | 38 | 0.9 | 46 | 34.2 (~39.3 w/ crit) | 88.2 (~101.4 w/ crit) | 510 | Crit (15%, x2) |
| Dough Exorcist | 260 | 35 | 1.0 | 50 | 35 | 90 | 580 | Execute (<20% HP, 2x, 1.5x vs elite) |
| High Prophet of Sourdough | 320 | 40 | 0.9 | 56 | 36 | 91 | 720 | Global Aura (+2% dmg per tower) |

## Details

### Bread Baker (`bread_baker`)
- **Stats**: 35 dmg / 1.0 FR / 48 range / 75g
- **DPS**: 35 → 50 (T1) → 84 (T2), total investment 235g
- **Mechanics**: Pure direct damage
- **Flavor**: *"The humble foundation of the Order. Steady hands, steady bread."*
- **Notes**: Reference for DPS/gold efficiency (0.467 base). Default unlock.
- **Equipment**:
  - **Stone Oven** (80g) — +15% damage. Pure.
  - **Ancient Starter** (120g) — +10% damage, +1 flat damage every 10 attacks (stacks persist per run). Scaling / no real downside, just ramp-up.

### Bread Courier (`bread_courier`)
- **Stats**: 28 dmg / 1.5 FR / 44 range / 90g
- **DPS**: 42 → 56 (T1) → 91.2 (T2), total investment 290g
- **Mechanics**: Fast projectile, high fire rate
- **Flavor**: *"Swift and tireless. The dough must reach its destination."*
- **Notes**: Highest FR in the game at 2.4 (T2). Many small hits.
- **Equipment**:
  - **Electric Bike** (80g) — +20% fire rate. Pure.
  - **Messenger Crate Upgrade** (120g) — Attacks pierce +1 enemy, -10% fire rate. Trade-off.

### Aroma Keeper (`aroma_keeper`)
- **Stats**: 25 dmg / 0.9 FR / 50 range / 100g — **Slow**: 0.5x speed, 3s duration (refresh, no intensity stack)
- **DPS**: 22.5 → 30 (T1) → 49 (T2), total investment 300g
- **Mechanics**: Slow on hit
- **Flavor**: *"The scent of fermentation slows even the most determined heretic."*
- **Notes**: Lowest DPS in the game — correct for pure control. Value is in synergies with Taste Tester/Fermentation Sage. Uses `projectile_ice.tscn` (visual placeholder?).
- **Equipment**:
  - **Megaphone** (80g) — +20% range. Pure.
  - **Spice Wind Chimes** (120g) — +30% slow duration, -10% damage. Trade-off.

### Taste Tester (`taste_tester`)
- **Stats**: 30 dmg / 1.0 FR / 48 range / 120g — **Poison**: 10 dmg/tick (1s), 4s duration (refresh)
- **DPS**: 30 (+10 poison) → 35 (+10) → 55.9 (+10) = **65.9 sustained DPS** at T2 on a continuously attacked target
- **Mechanics**: Poison DoT
- **Flavor**: *"One bite tells them everything. The poison is merely persuasive."*
- **Notes**: Poison doesn't stack intensity (only refreshes duration). Extra +10 poison DPS flat except when multiplied by `PoisonDamagePercentBonus`.
- **Equipment**:
  - **Silver Tray** (80g) — +20% poison damage. Pure.
  - **Double Sampling Plates** (120g) — Poison spreads to +1 extra target (30px from tower center), -15% fire rate. Trade-off.

### Bakery Truck (`bakery_truck`)
- **Stats**: 35 dmg / 0.8 FR / 44 range / 140g — **Splash**: radius 36 (full damage, no falloff)
- **DPS**: 28/target → 40/target (T1) → 56/target (T2), total investment 420g. With 3 targets in splash: effective DPS = **168**.
- **Mechanics**: AoE splash
- **Flavor**: *"A mobile bakery. Also a mobile weapon."*
- **Notes**: No damage falloff on splash. Best vs horde waves.
- **Equipment**:
  - **Reinforced Suspension** (80g) — +15% splash radius. Pure.
  - **Street Parade Route** (120g) — +30% area damage, -20% range. Trade-off.

### Bread Monk (`bread_monk`)
- **Stats**: 30 dmg / 1.0 FR / 50 range / 160g — **Aura**: 60 range, +10% dmg / +10% FR to other towers (not self)
- **DPS**: 30 → 45.6 (T1) → 75 (T2), total investment 400g
- **Mechanics**: Aura buffer
- **Flavor**: *"Through meditation and proper hydration, they empower their brethren."*
- **Notes**: Upgrades (Sacred Chant/Devotion Aura) only improve *personal* damage/FR, not aura magnitude (`AuraDamageBonusPercent`/`AuraFireRateBonusPercent` are static in TowerData).
- **Equipment**:
  - **Sacred Robes** (100g) — +15% aura range. Pure.
  - **Prayer Beads of Gluten** (150g) — Aura potency ×1.05 (10% → 10.5%), but **tower stops attacking**. Extreme trade-off (loses ~75 DPS for +0.5 p.p. aura — unbalanced).

### Fermentation Sage (`fermentation_sage`)
- **Stats**: 30 dmg / 0.9 FR / 52 range / 180g — **Chain**: 1 bounce, 40 range, 0.5x damage on bounce
- **DPS**: 27 (+13.5 chain) → 41.8 (+20.9) (T1) → 70 (+35) (T2), total investment 430g. Total on both targets at T2 = **105 DPS**.
- **Mechanics**: Chain lightning
- **Flavor**: *"Fermentation is a chain reaction. So is their judgment."*
- **Notes**: Has no native Poison or Slow (`HasPoison`/`HasSlow` = false). This breaks one equipment (Golden Proofing Bowl). Chain bounces to nearest enemy.
- **Equipment**:
  - **Golden Proofing Bowl** (100g) — +15% status effect duration. ⚠️ **Broken** — Sage has no status effects, this item does nothing.
  - **Wild Yeast Culture** (150g) — Chain +1 extra bounce, -10% initial damage. Trade-off.

### Crust Crusader (`crust_crusader`)
- **Stats**: 38 dmg / 0.9 FR / 46 range / 220g — **Crit**: 15% chance, x2 multiplier
- **DPS**: 34.2 raw (~39.3 w/ crit) → 52.8 (~60.7) (T1) → 88.2 (~101.4) (T2), total investment 510g
- **Mechanics**: Critical strikes
- **Flavor**: *"A blade tempered in the finest olive oil. The crust shall prevail."*
- **Notes**: With Tempered Crust Blade equip, crit chance goes to 25%. Crits ignore Gluten Null Bishop's anti-buff aura (hard counter).
- **Equipment**:
  - **Tempered Crust Blade** (100g) — +10% crit chance (15% → 25%). Pure.
  - **Blessed Crunch Seal** (150g) — Crits do mini-splash (r=30), -10% fire rate. Trade-off.

### Dough Exorcist (`dough_exorcist`)
- **Stats**: 35 dmg / 1.0 FR / 50 range / 260g — **Execute**: x2 dmg if HP ≤ 20%, x1.5 extra if target is Boss/Heavy (multiplicative → up to x3 total)
- **DPS**: 35 → 54 (T1) → 90 (T2), total investment 580g. Situational burst up to **270** vs elite/boss <20% HP.
- **Mechanics**: Execute / burst
- **Flavor**: *"They purge the industrial gluten from unwilling souls."*
- **Notes**: Stack x2 (execute) x x1.5 (elite) = x3 vs bosses <20% HP. Strongest single-target burst in the game.
- **Equipment**:
  - **Holy Flour Pouch** (100g) — +20% damage vs Boss/Heavy. Pure.
  - **Judgment Seal** (150g) — Execute below 15% HP (5s cooldown), -15% damage. Trade-off.

### High Prophet of Sourdough (`high_prophet`)
- **Stats**: 40 dmg / 0.9 FR / 56 range / 320g — **Global Aura**: +2% dmg per tower on map (incl. self), recalculated every 0.5s
- **DPS**: 36 → 55 (T1) → 91 (T2), total investment 720g. With 5-tower cap: max global aura = **+10% dmg to all towers**.
- **Mechanics**: Global aura
- **Flavor**: *"The living embodiment of the Mother Dough's will."*
- **Notes**: Most expensive tower (720g total). Global aura capped at +10% by 5-tower rule.
- **Equipment**:
  - **Golden Staff of Fermentation** (120g) — +10% damage, +10% FR, +10% range (3 stats). Pure. Best $/value — outlier.
  - **First Starter Relic** (180g) — +5% raw damage per nearby tower in 40px, -15% range. Trade-off.

## Upgrades

### Bread Baker
| Upgrade | Cost | Damage | FR |
|---------|------|--------|----|
| Reinforce | 60 | +15 | — |
| Heavy Barrel | 100 | +20 | +0.2 |

### Bread Courier
| Upgrade | Cost | Damage | FR |
|---------|------|--------|----|
| Overclock | 80 | — | +0.5 |
| Precision Rounds | 120 | +10 | +0.4 |

### Aroma Keeper
| Upgrade | Cost | Damage | FR |
|---------|------|--------|----|
| Cryo Coil | 80 | — | +0.3 |
| Permafrost | 120 | +10 | +0.2 |

### Taste Tester
| Upgrade | Cost | Damage | FR |
|---------|------|--------|----|
| Toxic Vial | 100 | +5 | — |
| Concentrated Venom | 140 | +8 | +0.3 |

### Bakery Truck
| Upgrade | Cost | Damage | Range |
|---------|------|--------|-------|
| Explosive Charge | 120 | +15 | — |
| Nova Blast | 160 | +20 | +12 |

### Bread Monk
| Upgrade | Cost | Damage | FR |
|---------|------|--------|----|
| Sacred Chant | 100 | +8 | +0.2 |
| Devotion Aura | 140 | +12 | +0.3 |

### Fermentation Sage
| Upgrade | Cost | Damage | FR |
|---------|------|--------|----|
| Extended Culture | 100 | +8 | +0.2 |
| Wild Fermentation | 150 | +12 | +0.3 |

### Crust Crusader
| Upgrade | Cost | Damage | FR |
|---------|------|--------|----|
| Sharpened Crust | 120 | +10 | +0.2 |
| Holy Crusher | 170 | +15 | +0.3 |

### Dough Exorcist
| Upgrade | Cost | Damage | FR |
|---------|------|--------|----|
| Blessed Flour | 130 | +10 | +0.2 |
| Judgment Aura | 190 | +15 | +0.3 |

### High Prophet of Sourdough
| Upgrade | Cost | Damage | FR |
|---------|------|--------|----|
| Sourdough Sermon | 160 | +10 | +0.2 |
| Great Fermentation | 240 | +15 | +0.3 |

## Synergies

| Synergy | Requirement | Effect |
|---------|-------------|--------|
| One Whiff, One Bite | Aroma Keeper + Taste Tester | +15% damage to both |
| Holy Fermentation Network | Aroma Keeper + Fermentation Sage | Slow/chain spread +30% further |
| Grand Opening Rush | 3+ tower types | +10% fire rate to all |
| Crust Judgment Protocol | Crust Crusader + Dough Exorcist | Crit vs <50% HP enemy = instant execute (10s cooldown) |

### One Whiff, One Bite (`one_whiff_one_bite`)
- **Requires**: Aroma Keeper + Taste Tester
- **Effect**: +15% damage to both
- **Synergy**: Slow keeps targets in range longer, increasing poison ticks applied.
- **Strong vs**: Grocery Run Jogger, Pigeon (fast), and tanks (Alley Cat, Abomination) via poison sustain.

### Grand Opening Rush (`grand_opening_rush`)
- **Requires**: 3+ tower types on map
- **Effect**: +10% fire rate to all
- **Synergy**: Generic bonus rewarding loadout diversity. Easiest to activate.
- **Strong vs**: Horde waves and tier3 waves with multiple enemy types.

### Holy Fermentation Network (`holy_fermentation_network`)
- **Requires**: Aroma Keeper + Fermentation Sage
- **Effect**: +30% range for slow/chain spread
- **Synergy**: Sage has no native Poison/Slow — "slow" refers to Aroma Keeper's slow.
- **Strong vs**: Tight enemy groups (Horde modifier).

### Crust Judgment Protocol (`crust_judgment_protocol`)
- **Requires**: Crust Crusader + Dough Exorcist
- **Effect**: Crit vs target <50% HP = instant execute, 10s cooldown
- **Synergy**: Most powerful boss-killer synergy. 10s cooldown limits to one target at a time.
- **Strong vs**: Alley Cat, Frozen Dough Abomination, Supermarket Overlord, both bosses.
- **Note**: Since it depends on Crit (not % damage), this synergy **ignores the Gluten Null Bishop's anti-buff aura** — a hard counter even if not explicitly designed.

## Open Questions
1. Bread Monk: upgrades only buff personal attack, never aura %. Intentional?
2. Fermentation Sage: no native HasPoison/HasSlow despite shared lore with Aroma Keeper/Taste Tester — breaks Golden Proofing Bowl equipment.
3. High Prophet: 720g total for a fixed +10% global dmg cap (5-tower rule). Is the value in personal DPS (91) as the capstone?
4. Dough Exorcist: x2 (execute) x x1.5 (elite) = x3 vs bosses <20% HP. Do boss HP pools (1000/1500) survive this burst without trivializing?
5. Crit vs Anti-Buff Aura: Gluten Null Bishop does NOT debuff Crit Chance/Multiplier, only % dmg/FR/range. Makes Crust Crusader a hard counter. Intentional?
6. Prayer Beads (Bread Monk): trade-off unbalanced — loses ~75 DPS for +0.5 p.p. aura. Needs `AuraPotencyMultiplier` buff or rework.
7. Golden Staff (High Prophet): +10% on 3 stats for 120g vs single-stat items at same price. Intentional capstone reward?
8. Golden Proofing Bowl (Fermentation Sage): currently broken — Sage has no status effects. Fix or replace.
9. Double Sampling Plates (Taste Tester): spread radius measured from tower position, not target. Intentional?
