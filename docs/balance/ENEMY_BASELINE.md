# Enemy Baseline — V0

Factual baseline of all 10 enemies at V0 state. **Facts** are from `.tres` resource files. **Derived metrics** are calculated from facts. **Hypotheses** are design opinions to be validated.

---

## FACTS: Enemy Stats

| Enemy | HP | Speed | Gold | Dmg | Flags |
|---|---|---|---|---|---|
| SlicedBreadTourist | 50 | 24 | 3 | 1 | — |
| GroceryJogger | 35 | 48 | 5 | 1 | — |
| LazyAlleyCat | 150 | 12 | 10 | 2 | IsHeavy |
| BaguettePigeon | 40 | 32 | 6 | 1 | — |
| MicrowaveMealPreacher | 200 | 20 | 25 | 2 | IsHeavy |
| PlasticWrappedSandwichMan | 180 | 28 | 22 | 2 | IsHeavy (added V0) |
| FrozenDoughAbomination | 400 | 10 | 35 | 3 | IsHeavy |
| SupermarketOverlord | 900 | 18 | 80 | 4 | IsHeavy |
| IndustrialBreadDragon | 1000 | 16 | 100 | 5 | IsBoss, IsHeavy |
| GlutenNullBishop | 1500 | 14 | 120 | 6 | IsBoss, IsHeavy, AntiBuff |

---

## DERIVED METRICS

### Pressure (HP × Speed)

Higher = more urgent threat (fast tank or tough speedster).

| Enemy | HP × Speed | Rank |
|---|---|---|
| GlutenNullBishop | 21,000 | 1 |
| SupermarketOverlord | 16,200 | 2 |
| IndustrialBreadDragon | 16,000 | 3 |
| PlasticWrappedSandwichMan | 5,040 | 4 |
| MicrowaveMealPreacher | 4,000 | 5 |
| FrozenDoughAbomination | 4,000 | 6 |
| LazyAlleyCat | 1,800 | 7 |
| GroceryJogger | 1,680 | 8 |
| BaguettePigeon | 1,280 | 9 |
| SlicedBreadTourist | 1,200 | 10 |

### Gold Efficiency (Gold / Pressure)

Higher = more gold per unit of threat.

| Enemy | Gold / Pressure | Rank |
|---|---|---|
| BaguettePigeon | 0.00469 | 1 |
| GroceryJogger | 0.00298 | 2 |
| SlicedBreadTourist | 0.00250 | 3 |
| LazyAlleyCat | 0.00556 | 4 |
| PlasticWrappedSandwichMan | 0.00437 | 5 |
| MicrowaveMealPreacher | 0.00625 | 6 |
| FrozenDoughAbomination | 0.00875 | 7 |
| SupermarketOverlord | 0.00494 | 8 |
| IndustrialBreadDragon | 0.00625 | 9 |
| GlutenNullBishop | 0.00571 | 10 |

### Fight 5 Scaling (2.5× HP at DifficultyScalingPerFight = 0.30)

| Enemy | Base HP | Fight 5 HP |
|---|---|---|
| GroceryJogger | 35 | 88 |
| BaguettePigeon | 40 | 100 |
| SlicedBreadTourist | 50 | 125 |
| LazyAlleyCat | 150 | 375 |
| PlasticWrappedSandwichMan | 180 | 450 |
| MicrowaveMealPreacher | 200 | 500 |
| FrozenDoughAbomination | 400 | 1,000 |
| SupermarketOverlord | 900 | 2,250 |
| IndustrialBreadDragon | 1,000 | 2,500 |
| GlutenNullBishop | 1,500 | 3,750 |

---

## Wave Tier Contents

### Tier 1 (Act 1 — all fights draw from this pool)

7 wave templates in `resources/wave_data/tier1/`. Enemy types used: SlicedBreadTourist, GroceryJogger, BaguettePigeon, LazyAlleyCat.

Average enemies per wave template: ~6.7. Average gold per enemy in tier 1: ~5.5g.

### Boss Wave (act1_boss, used after 5 fights)

| Enemy | Count |
|---|---|
| SlicedBreadTourist | 8 |
| BaguettePigeon | 4 |
| LazyAlleyCat | 3 |
| IndustrialBreadDragon | 2 |
| GlutenNullBishop | 1 |

SpawnInterval: 1.0 (down from 2.0).

### Tier 2 / 3 (future Acts — not used in Act 1 demo)

Tier 2 adds PlasticWrappedSandwichMan, MicrowaveMealPreacher, FrozenDoughAbomination.
Tier 3 includes SupermarketOverlord.

---

## HYPOTHESES

| Enemy | Hypothesis |
|---|---|
| SlicedBreadTourist | Baseline chaff. Low threat, low gold. Exists to be killed. |
| GroceryJogger | Fast scout — tests whether player has early fire rate. Highest pressure-per-HP of tier 1. |
| LazyAlleyCat | Early tank. Slow but 150 HP at fight 1 forces players to have at least 2 towers or upgrades by wave 3. |
| BaguettePigeon | Light, fast, decent gold. High gold efficiency. Meant to be a "bonus" enemy that rewards fast targeting. |
| MicrowaveMealPreacher | First proper heavy at 200 HP. Introduces the "check your DPS" mechanic. High gold (25g). |
| PlasticWrappedSandwichMan | Now IsHeavy — highest tier-2 pressure at 5,040. May need HP or speed adjustment after data. Previously was light-armor flavor without gameplay distinction. |
| FrozenDoughAbomination | Slowest enemy (10 speed). 400 HP is a DPS check. Teaches players that raw damage beats status effects on high-HP targets. |
| SupermarketOverlord | 900 HP, not a boss but elite stats. Used as slot machine miniboss. Role: mid-run spike to test whether player has invested in upgrades. |
| IndustrialBreadDragon | True boss. 1000 HP, 5 damage to player. Two appear in the boss wave. Anti-buff aura immunity matters here. |
| GlutenNullBishop | Highest pressure enemy (21,000). Anti-buff aura halves all percentage bonuses within 60px. Forces positional play. Crust Crusader crits ignore the aura — potentially hard-counter. |

### Bishop Anti-Buff Aura Notes

- Bishop's `HasAntiBuffAura` triggers a 0.5s scan of nearby towers.
- Towers within `AntiBuffAuraRadius` (60px) get a 0.5× multiplier on all percent-based bonuses (synergy, equip, aura, globalAura, trinket).
- Does NOT affect: base damage, flat damage (Ancient Starter, First Starter Relic), crit chance, crit multiplier.
- This means Crust Crusader (crit-based DPS) and flat-damage equipment partially bypass the aura.
- In the boss wave, 1 Bishop + 2 Dragons + 8 tourists means heavy pressure AND positional constraints.

### Role Distinction (Boss vs Elite)

| Enemy | IsBoss | Role |
|---|---|---|
| SupermarketOverlord | No (IsHeavy only) | Elite/miniboss during slot machine |
| IndustrialBreadDragon | Yes | True boss |
| GlutenNullBishop | Yes | True boss with anti-buff aura |

Supermarket Overlord is not IsBoss to allow it in the slot machine miniboss pool without triggering boss-level scaling or victory conditions.
