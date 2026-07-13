# Tower Baseline — V0

Factual baseline of all 10 towers at V0 state. **Facts** are from `.tres` resource files. **Derived metrics** are calculated from facts. **Hypotheses** are design opinions to be validated.

---

## FACTS: Tower Stats

| Tower | Cost | Dmg | FR | Range | Raw DPS | Special |
|---|---|---|---|---|---|---|
| Bread Baker | 75 | 35 | 1.0 | 48 | 35.0 | — |
| Bread Courier | 90 | 28 | 1.5 | 44 | 42.0 | Fast fire rate |
| Aroma Keeper | 100 | 25 | 0.9 | 50 | 22.5 | Slow 50%/3s |
| Taste Tester | 120 | 30 | 1.0 | 48 | 30.0 | Poison 10/tick × 4s = 40 total |
| Bakery Truck | 140 | 35 | 0.8 | 44 | 28.0 | Splash r=36 |
| Bread Monk | 160 | 30 | 1.0 | 50 | 30.0 | Aura +10% dmg+FR (×2 w/ Prayer Beads) |
| Fermentation Sage | 180 | 30 | 0.9 | 52 | 27.0 | Chain 1 bounce 50% dmg (+1 w/ Golden Proofing Bowl) |
| Crust Crusader | 220 | 38 | 0.9 | 46 | 34.2 | Crit 15%/2× (expected ×1.15) |
| Dough Exorcist | 260 | 35 | 1.0 | 50 | 35.0 | Execute 2× <20% HP, elite 1.5× |
| High Prophet | 320 | 40 | 0.9 | 56 | 36.0 | Global +2%/tower |

### Upgrade Costs

Each tower has **4 upgrade tiers**. Total upgrade investment per tower:

| Tower | Tier 1 | Tier 2 | Tier 3 | Tier 4 | Upgrade Total | Full Investment (cost + upgrades) |
|---|---|---|---|---|---|---|
| Bread Baker | 66 | 110 | 176 | 264 | 616 | 691 |
| Bread Courier | 88 | 132 | 198 | 286 | 704 | 794 |
| Aroma Keeper | 88 | 132 | 198 | 286 | 704 | 804 |
| Taste Tester | 110 | 154 | 220 | 308 | 792 | 912 |
| Bakery Truck | 132 | 176 | 242 | 330 | 880 | 1,020 |
| Bread Monk | 110 | 154 | 220 | 308 | 792 | 952 |
| Fermentation Sage | 110 | 165 | 242 | 330 | 847 | 1,027 |
| Crust Crusader | 132 | 187 | 264 | 374 | 957 | 1,177 |
| Dough Exorcist | 143 | 209 | 286 | 396 | 1,034 | 1,294 |
| High Prophet | 176 | 264 | 352 | 462 | 1,254 | 1,574 |

---

## DERIVED METRICS

### Raw DPS Ranking (base, no upgrades/synergies/equip)

1. Bread Courier — 42.0
2. High Prophet — 36.0
3. Bread Baker — 35.0
4. Dough Exorcist — 35.0
5. Crust Crusader — 34.2 (39.3 expected with crit)
6. Taste Tester — 30.0
7. Bread Monk — 30.0
8. Bakery Truck — 28.0 (splash: higher vs groups)
9. Fermentation Sage — 27.0 (chain: +13.5 avg on second target)
10. Aroma Keeper — 22.5

### DPS/Gold Ranking (raw efficiency per gold spent)

1. Bread Baker — 0.467
2. Bread Courier — 0.467
3. Taste Tester — 0.250
4. Aroma Keeper — 0.225
5. Bakery Truck — 0.200
6. Bread Monk — 0.188
7. Crust Crusader — 0.155
8. Fermentation Sage — 0.150
9. Dough Exorcist — 0.135
10. High Prophet — 0.113

---

## HYPOTHESES

| Tower | Hypothesis |
|---|---|
| Bread Baker | Strong early-game backbone. High raw efficiency, no gimmick — pure DPS. Expected to fall off late as special mechanics scale better. |
| Bread Courier | Highest raw DPS, tied for best DPS/Gold. Fast fire rate makes it consistent vs all enemy types. "Simple and good" identity. |
| Aroma Keeper | Lowest DPS, highest DPS/Gold among mid-cost towers. Value is entirely in slow utility — pairs well with Taste Tester. Weak without a slow-dependent partner. |
| Taste Tester | Mediocre raw DPS but poison adds 40 total damage over 4s. Value increases with fight duration (enemies live longer → more poison ticks). |
| Bakery Truck | Splash is the only AoE mechanic. Strong vs horde waves, weak vs single targets. Positioning matters for maximum splash coverage. |
| Bread Monk | Aura is the most interesting mechanic but upgrades don't improve it. Only Prayer Beads equipment enhances aura. Without Prayer Beads, monk is an overpriced 30-DPS tower with a small buff. |
| Fermentation Sage | Chain bounce value depends entirely on enemy density. Strong in horde/grouped waves, weak vs single targets (bosses). Golden Proofing Bowl (+1 bounce) is transformative. |
| Crust Crusader | Crit makes expected DPS ~39.3, nearly matching Courier. Crit also bypasses anti-buff aura. Strong all-rounder with built-in anti-bishop tech. |
| Dough Exorcist | Execute + elite bonus is powerful vs bosses and heavy enemies. Without those, it's a 35-DPS tower at 260g — worst efficiency of non-Prophet towers. Must fight heavy enemies to justify cost. |
| High Prophet | Most expensive tower. Global aura adds ~2% per other tower on field. With 4 towers = +8% global, with 6 towers = +12%. Self-DPS is respectable but DPS/Gold is worst. Value increases with more towers placed. |

### Tower Identity Concerns

1. **Bread Monk**: upgrades only buff personal attack, never aura %. At 160g, the aura (+10% to 1–3 nearby towers) must compensate for poor personal DPS. Currently, without Prayer Beads, the monk underperforms.
2. **Fermentation Sage**: Golden Proofing Bowl changed from status duration (useless on Sage) to +1 chain bounce — now a meaningful equip that doubles chain effectiveness.
3. **High Prophet**: 320g is 4× the cost of Bread Baker. Global aura maxes out at ~+12–14% with a full board. Value proposition depends on having 5+ other towers.
