# Towers

> DPS formula: `Damage × FireRate` (FireRate = attacks/second).
> DPS shown is raw — without synergy/equip/shop/meta/trinket bonuses.
> Each tower has dedicated stat/lore/equipment/upgrade/synergy info in its own file.

## Overview

| Tower | Cost | Dmg | FR | Range | DPS Base | DPS Full Upgrade | Total Invested | Special |
|-------|------|-----|----|-------|----------|------------------|----------------|---------|
| [Bread Baker](towers/bread_baker.md) | 75 | 35 | 1.0 | 48 | 35 | 84 | 235 | — |
| [Bread Courier](towers/bread_courier.md) | 90 | 28 | 1.5 | 44 | 42 | 91.2 | 290 | Fast projectile |
| [Aroma Keeper](towers/aroma_keeper.md) | 100 | 25 | 0.9 | 50 | 22.5 | 49 | 300 | Slow (0.5x, 3s) |
| [Taste Tester](towers/taste_tester.md) | 120 | 30 | 1.0 | 48 | 30 (+10 poison) | 55.9 (+10 poison) | 360 | Poison (10/s, 4s) |
| [Bakery Truck](towers/bakery_truck.md) | 140 | 35 | 0.8 | 44 | 28/target | 56/target | 420 | Splash (r=36) |
| [Bread Monk](towers/bread_monk.md) | 160 | 30 | 1.0 | 50 | 30 | 75 | 400 | Aura (+10% dmg/rate, r=60) |
| [Fermentation Sage](towers/fermentation_sage.md) | 180 | 30 | 0.9 | 52 | 27 (+13.5 chain) | 70 (+35 chain) | 430 | Chain (1 bounce, 40r, 50% dmg) |
| [Crust Crusader](towers/crust_crusader.md) | 220 | 38 | 0.9 | 46 | 34.2 (~39.3 w/ crit) | 88.2 (~101.4 w/ crit) | 510 | Crit (15%, x2) |
| [Dough Exorcist](towers/dough_exorcist.md) | 260 | 35 | 1.0 | 50 | 35 | 90 | 580 | Execute (<20% HP, 2x, 1.5x vs elite) |
| [High Prophet of Sourdough](towers/high_prophet.md) | 320 | 40 | 0.9 | 56 | 36 | 91 | 720 | Global Aura (+2% dmg per tower) |

### Quick Links

| Tower | File |
|-------|------|
| Bread Baker | [`docs/towers/bread_baker.md`](towers/bread_baker.md) |
| Bread Courier | [`docs/towers/bread_courier.md`](towers/bread_courier.md) |
| Aroma Keeper | [`docs/towers/aroma_keeper.md`](towers/aroma_keeper.md) |
| Taste Tester | [`docs/towers/taste_tester.md`](towers/taste_tester.md) |
| Bakery Truck | [`docs/towers/bakery_truck.md`](towers/bakery_truck.md) |
| Bread Monk | [`docs/towers/bread_monk.md`](towers/bread_monk.md) |
| Fermentation Sage | [`docs/towers/fermentation_sage.md`](towers/fermentation_sage.md) |
| Crust Crusader | [`docs/towers/crust_crusader.md`](towers/crust_crusader.md) |
| Dough Exorcist | [`docs/towers/dough_exorcist.md`](towers/dough_exorcist.md) |
| High Prophet of Sourdough | [`docs/towers/high_prophet.md`](towers/high_prophet.md) |

## Synergies

| Synergy | Requirement | Effect | Towers |
|---------|-------------|--------|--------|
| One Whiff, One Bite | Aroma Keeper + Taste Tester | +15% damage to both | [`aroma_keeper.md`](towers/aroma_keeper.md), [`taste_tester.md`](towers/taste_tester.md) |
| Holy Fermentation Network | Aroma Keeper + Fermentation Sage | Slow/chain spread +30% further | [`aroma_keeper.md`](towers/aroma_keeper.md), [`fermentation_sage.md`](towers/fermentation_sage.md) |
| Grand Opening Rush | 3+ tower types on map | +10% fire rate to all | All towers |
| Crust Judgment Protocol | Crust Crusader + Dough Exorcist | Crit vs <50% HP = instant execute (10s cd) | [`crust_crusader.md`](towers/crust_crusader.md), [`dough_exorcist.md`](towers/dough_exorcist.md) |

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

## Special Mechanics

- **Aura (Bread Monk)**: scans all towers in group every 0.5s. Applies static-registry buff by tower reference. Removes buff when tower leaves range or is removed.
- **Global Aura (High Prophet)**: counts towers in `towers` group every 0.5s. Sets `RunState.GlobalAuraDamagePercent = count * damagePerTower`.
- **Chain (Fermentation Sage)**: on hit, finds nearest enemy within bounce range using physics shape query. Applies damage and propagates status effects.
- **Crit (Crust Crusader)**: random roll in `AttackComponent.Fire()`. If crit, damage ×= `critMultiplier`. Equipment and trinkets can modify chance and multiplier.
- **Execute (Dough Exorcist)**: if target HP < threshold, damage ×= `executeMultiplier`. If target IsBoss/IsHeavy, damage ×= `eliteBonusMultiplier`.
- **Synergy Crust Judgment Protocol**: on crit against target <50% HP, instant kill (9999 damage). 10s cooldown per tower.
- **Anti-Buff**: Gluten Null Bishop applies 50% debuff to all multiplicative bonuses. Implemented via static `Tower._antiBuffCount` dictionary, scanned every 0.5s.

## EffectiveDamage Formula

```
EffectiveDamage = (baseDamage + upgradeBonus + ancientStarterStacks + firstStarterBonus)
  * (1 + (synergyPercent + equipPercent + auraPercent + globalAuraPercent + trinketPercent) * buffMultiplier)
  * (1 + shopPercent)
  * (1 + metaPercent)
```

Where `buffMultiplier = 0.5` if affected by Gluten Null Bishop's anti-buff aura.

## Open Questions

1. Bread Monk: upgrades only buff personal attack, never aura %. Intentional? (See [`bread_monk.md`](towers/bread_monk.md))
2. Fermentation Sage: no native HasPoison/HasSlow — breaks Golden Proofing Bowl equipment. (See [`fermentation_sage.md`](towers/fermentation_sage.md))
3. High Prophet: 720g for fixed +10% global dmg cap. Is value in personal DPS? (See [`high_prophet.md`](towers/high_prophet.md))
4. Dough Exorcist: x3 vs bosses <20% HP. Do boss HP pools survive without trivializing? (See [`dough_exorcist.md`](towers/dough_exorcist.md))
5. Crit vs Anti-Buff Aura: Gluten Null Bishop does NOT debuff Crit Chance/Multiplier. Makes Crust Crusader a hard counter. Intentional? (See [`crust_crusader.md`](towers/crust_crusader.md))
6. Prayer Beads (Bread Monk): unbalanced trade-off. (See [`bread_monk.md`](towers/bread_monk.md))
7. Golden Staff (High Prophet): +10% on 3 stats for 120g vs single-stat at same price. (See [`high_prophet.md`](towers/high_prophet.md))
8. Golden Proofing Bowl (Fermentation Sage): currently broken. (See [`fermentation_sage.md`](towers/fermentation_sage.md))
9. Double Sampling Plates (Taste Tester): spread radius from tower position, not target. (See [`taste_tester.md`](towers/taste_tester.md))
