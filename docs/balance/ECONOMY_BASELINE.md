# Economy Baseline — V0

Gold pacing model for Act 1 (5 fights + 1 boss) at V0 state.

---

## FACTS

| Economy property | Value |
|---|---|
| Starting gold | 250 |
| Starting lives | 20 |
| DifficultyScalingPerFight | 0.30 (not affecting gold) |
| Average gold per enemy (tier 1) | ~5.5g |
| Average enemies per wave | ~6.7 |
| Average waves per fight | ~5.5 (range 4–7) |
| Average gold per fight | ~204g (37 enemies × 5.5g) |
| Tower cost range | 75–320g |
| Upgrade cost per tower (total) | 616–1,254g |
| Equipment cost range | 80–180g |
| Reroll base cost | 50g (scales: 50 → 100 → 150 per run) |
| Miniboss gold | 1.5× normal enemy rewards |
| Shop item costs | 60–120g |

### Enemy Gold Values (tier 1 pool)

| Enemy | Gold |
|---|---|
| SlicedBreadTourist | 3 |
| GroceryJogger | 5 |
| BaguettePigeon | 6 |
| LazyAlleyCat | 10 |

Average: (3 + 5 + 6 + 10) / 4 = 6.0. Weighted average (by typical wave composition): ~5.5g.

---

## DERIVED: Gold Pacing

### Cumulative Gold by Fight

| Fight | Cumulative Gold (avg) | Typical Spending | Remaining |
|---|---|---|---|
| Start | 250 | — | 250 |
| Fight 1 | ~454 (250 + 204) | 2–3 tower placements (~200g) | ~254 |
| Fight 2 | ~658 | +1 tower + upgrades (~200g) | ~458 |
| Fight 3 | ~862 | Upgrades (~200g) | ~662 |
| Fight 4 | ~1,066 | Equipment + upgrades (~200g) | ~866 |
| Fight 5 | ~1,270 | Final upgrades (~200g) | ~1,070 |
| Boss | ~1,270 (no new gold during boss) | ~100–300g unspent | ~1,070 |

### Spending Breakdown (typical run)

| Category | Typical spend |
|---|---|
| Tower placement (3–5 towers) | 225–800g |
| Upgrades (2–4 total tiers) | 250–550g |
| Equipment (1–2 items) | 80–360g |
| Shop items (1–3) | 60–300g |
| Rerolls (0–2) | 0–150g |
| **Total typical** | **615–2,160g** |

---

## HYPOTHESES

| Statement | Hypothesis |
|---|---|
| Gold should be tight but sufficient | Leftover 100–300g at boss is healthy — player can afford last-minute upgrades if needed |
| 5 fights fixes old 8-fight oversupply | With 8 fights, gold overshot by fight 6 — players had nothing meaningful to spend on. 5 fights keeps scarcity through fight 4–5. |
| Rerolls are expensive | At 50g base (~25% of a fight's income), rerolls should be used sparingly — meaningful trade-off between gambling and guaranteed upgrades. |
| First tower choice matters most | Player must place 2–3 towers immediately. With 250g starting gold, they can afford: Baker (75) + Courier (90) + Keeper (100) = 265g (short 15g). Most common start: Baker + Courier (165g, 85g spare for upgrades mid-fight). |
| Equipment competes with upgrades | At fight 4 (~1,066g), spending 100–150g on equipment delays upgrade by 1 tier. Trade-off between immediate power and scaling. |
| Expensive towers are lategame purchases | High Prophet (320g) is 1.6× a full fight's income. Player must save for 1–2 fights to afford it, making it a commitment pick. |

### Slot Machine Economy Impact

| Outcome | Gold effect |
|---|---|
| Fight | +~204g avg, no cost |
| Shop | Spending opportunity (no fight gold). Net gold change: negative |
| Heal | No gold change (restores 5 lives, no fight) |
| Miniboss | +~306g avg (1.5× rewards). Higher risk, higher reward |
| Treasure | No gold change (pick trinket) |
| Boss | No new gold (last fight) |

The slot machine introduces variance: a Shop outcome followed by Treasure means 2 fights without income, forcing the player to stretch existing gold.
