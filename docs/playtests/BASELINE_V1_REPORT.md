# Baseline V1 Report — Sourdough Siege Demo

Aggregate analysis of 11 playtest runs with corrected v1-format analytics (default tier-1 upgrades unlocked + unspent gold → meta-tokens). Report updated 2026-07-12 after analytics system upgrade (per-tower upgrade tracking, per-fight leak fix, per-fight gold breakdown).

---

## 1. V1 Changes (from V0 Baseline)

Three changes were applied after Baseline V0 data collection (see `BASELINE_V0_REPORT.md` section 6):

| Change | Description |
|---|---|
| **A) Default Tier-1 Upgrades Unlocked** | Bread Baker, Bread Courier, Aroma Keeper start with tier 1 of their upgrade path unlocked in `SaveManager`. Upgrade button shows actual costs instead of "Buy in Meta Shop". |
| **B) Unspent Gold → Meta-Tokens** | At run end, unspent gold converts to bonus meta-tokens at 100g = 1 token. Added to existing `PreviewTokenReward` total. |
| **C) Bug Fixes** | Projectile wasted-on-dead-enemy guard (`Projectile.cs`). Fresh saves missing default upgrade levels (`SaveManager.cs`). |

### What Did NOT Change
- `DifficultyScalingPerFight` = 0.30 (unchanged)
- Tower base stats (unchanged)
- Enemy stats (unchanged)
- Upgrade costs (unchanged from `.tres` files)
- Meta-upgrade costs (unchanged)
- Wave compositions (unchanged)

---

## 2. Test Configuration (v1 Analytics Format)

| Property | Value |
|---|---|
| Loadout | Bread Baker + Bread Courier + Aroma Keeper + Taste Tester (4 towers, Taste Tester unlocked via meta-progression) |
| Meta-progression | Incremental — player earned tokens across previous runs |
| Available upgrades | Tier 1 unlocked for Baker, Courier, Keeper by default (V1 change) |
| Equipment purchased | ancient_starter (Baker), electric_bike (Courier), double_sampling (Taste Tester) |
| Shop items | secret_ingredient, golden_proof_flour, rapid_oven_upgrade |
| Active synergies | grand_opening_rush (+10% FR for 3+ types), one_whiff_one_bite (Taste Tester + Aroma Keeper: +15% poison dmg), daily_proof (Baker + Courier: +10% damage) |
| Trinkets | first_starter_vessel, crust_fragment_relic |
| Speed | 8x |
| Analytics format | v1 (per-fight breakdown, per-tower upgrade tracking, per-fight leaks) |

---

## 3. Results Summary

### Win Rate

| Metric | Value |
|---|---|
| Runs | 11 |
| Victories | 1 (9.1%) |
| Defeats | 10 (90.9%) |
| Runs reaching boss | 6/11 (55%) |
| Boss victories / boss reached | 1/6 (17%) |

### Fight Survival

| Fight | Scaling | Survived | Survival Rate |
|---|---|---|---|
| Fight 0 | 1.0x | 11/11 | 100% |
| Fight 1 | 1.3x | 11/11 | 100% |
| Fight 2 | 1.6x | 9/11 | 82% |
| Fight 3 | 1.9x | 8/11 | 73% |
| Fight 4 | 2.2x | 6/11 | 55% |
| Boss | 2.5x | 6/11 (reached) | 55% reach, 17% victory |

Collapse points: Fight 2-3 (19-27% loss rate), Boss (83% defeat rate for those who reach it).

### Damage Share (All 11 Runs)

| Tower | Avg Damage Share | Notes |
|---|---|---|
| Bread Baker | **65-82%** | Dominant. Receives tier-1 in 100% of runs, tier-2 in 82% of runs. ancient_starter equipment amplifies flat damage. |
| Bread Courier | 8-18% | Underperforms in most runs. Received tier-1 in ~55% of runs, never tier-2. electric_bike equipment (+speed, not +damage) doesn't help DPS. |
| Aroma Keeper | 5-12% | Utility tower. Received tier-1 in ~55% of runs, never tier-2. Damage understates value (slow enables poison + buys time). |
| Taste Tester | 8-18% | Poison DoT. Received tier-1 in ~45% of runs, never tier-2. double_sampling equipment amplifies poison. |

### 🎯 Upgrade Distribution (The Smoking Gun)

The new v1 analytics tracks exactly which tower received which upgrade tier. This was the missing data from the v0 report.

| Tower | Tier 1 purchased | Tier 2 purchased | Avg cost/run |
|---|---|---|---|
| **Bread Baker** | **11/11 runs (100%)** | **9/11 runs (82%)** | **176g** |
| Bread Courier | ~6/11 runs (55%) | 0/11 (0%) | 88g |
| Aroma Keeper | ~6/11 runs (55%) | 0/11 (0%) | 88g |
| Taste Tester | ~5/11 runs (45%) | 0/11 (0%) | 110g |

**No tower other than Bread Baker ever received a tier-2 upgrade.** The Baker receives tier-1 AND tier-2 in the same fight (fight 0) in most runs — 176g invested before any other tower is even placed.

**Baker's tier-1 (66g, +15 damage) + tier-2 (110g, additional stats) + ancient_starter equipment (114g, flat damage that ignores Bishop anti-buff aura) creates a feedback loop: more damage → more kills → more gold → more upgrades for Baker.**

### Victory Run Deep Dive (run_id: 85a0bc51)

| Metric | Value |
|---|---|
| Loadout | Aroma Keeper, Bread Baker, Bread Courier, Taste Tester |
| Equipment | ancient_starter (Baker, 114g), electric_bike (Courier, 76g), double_sampling (Taste Tester, 114g) |
| Shop | secret_ingredient (+5% team damage, 80g) |
| Synergies | grand_opening_rush, one_whiff_one_bite, daily_proof |
| Duration | 6m 00s (real-time at 8x) |
| Gold earned | 2,299 |
| Gold spent | 2,182 (95% utilization) |
| Lives remaining | 1 / 20 |
| Boss lives lost | 0 (perfect boss fight!) |

| Tower | Damage | % | Kills | % | Upgrades | Equipment |
|---|---|---|---|---|---|---|
| Bread Baker | 37,637 | **82%** | 287 | **95%** | Tier 1 + Tier 2 | ancient_starter |
| Taste Tester | 3,926 | 9% | 4 | 1% | Tier 1 | double_sampling |
| Bread Courier | 2,521 | 5% | 4 | 1% | Tier 1 | electric_bike |
| Aroma Keeper | 1,927 | 4% | 8 | 3% | Tier 1 | — |

**Key observation**: The boss fight had **zero leaks** (first time in any run). The combination of tier-2 Baker + ancient_starter + team damage shop item + 3 synergies was sufficient to clear the boss wave without losing a single life. In contrast, ALL previous boss attempts (v0 and early v1) ended with 12-27 lives lost in the boss fight alone.

### Per-Fight Gold Spending (Victory Run)

The new `gold_spent_breakdown` field shows exactly where gold went each fight:

| Fight | Towers | Upgrades | Equipment | Rerolls | Total | Strategy |
|---|---|---|---|---|---|---|
| 0 | 75 | **176** | 0 | 0 | 251 | Baker tier 1+2 immediately |
| 1 | 75 | 0 | 114 | **270** | 459 | Baker ancient_starter + heavy rerolling |
| 2 | 175 | 88 | 0 | 0 | 263 | Place Aroma Keeper + tier-1 |
| 3 | 165 | 88 | 266 | 0 | 519 | Courier tier-1 + 2 equipments |
| 4 | 195 | 110 | 0 | 0 | 305 | Taste Tester + tier-1 |
| 5 | 385 | 0 | 0 | 0 | 385 | Place remaining towers for boss |

**Strategy pattern**: All-in on Baker first (tier 1+2 in fight 0). Equipment for Baker in fight 1. Expand to other towers in fights 2-4 with tier-1 upgrades. No new upgrades in boss fight — just placement.

### Leaks by Fight (Victory Run — Corrected Per-Fight Data)

The v0 analytics had a bug where `leaks_by_enemy` was cumulative across the run. The v1 format shows **true per-fight leaks**:

| Fight | Enemies Leaked | Lives Lost |
|---|---|---|
| 0 | (none) | 0 |
| 1 | 6× BaguettePigeon | 8 |
| 2 | 1× SlicedBreadTourist | 2 |
| 3 | (none) | 0 |
| 4 | 4× BaguettePigeon | 9 |
| 5 (Boss) | **(none)** | 0 |

**This contradicts the v0 report's claim that "1 cat + 3 pigeons leak every fight."** That was an artifact of the cumulative leak tracking bug. In reality:
- Fight 0 and 3 had **zero leaks**
- Fight 1 had a pigeon swarm (6 birds, fast enemies overwhelming early single-tower coverage)
- Fight 4 had another pigeon wave
- The boss fight was **perfect** — zero leaks

### Equipment Purchase Tracking (New Data)

The v1 analytics now records every equipment purchase with tower, equip ID, and cost:

| Run | Baker equip | Courier equip | Taste Tester equip | Total equip spend |
|---|---|---|---|---|
| Victory | ancient_starter (114g) | electric_bike (76g) | double_sampling (114g) | 304g |
| Defeat runs | ancient_starter (100-120g) | electric_bike (76-80g) | double_sampling (114-120g) | 190-410g |

**ancient_starter is ALWAYS the first equipment purchased** — typically in fight 1 or at the first Shop outcome. This flat-damage equipment synergizes with Baker's high fire rate and bypasses the Bishop's anti-buff aura (which only reduces percentage bonuses, not flat damage).

**electric_bike on Courier provides +speed, not +damage** — this explains part of Courier's low damage share. The player isn't investing in Courier's damage output, instead using it for utility (fast attacks to trigger on-hit effects? Actually, Courier doesn't have on-hit effects at base).

### Gold Economy (All 11 Runs)

| Metric | Value |
|---|---|
| Avg gold earned per run | ~1,600 |
| Avg gold spent per run | ~1,450 |
| Gold utilization | 91% |
| Spending distribution | Towers: ~50%, Upgrades: ~28%, Equipment: ~15%, Rerolls: ~7% |

Gold utilization improved from 82% (v0 report estimate) to 91% with accurate per-category tracking.

---

## 4. Comparison: V0 vs V1 (Corrected)

| Metric | Baseline V0 | Baseline V1 (corrected) | Δ |
|---|---|---|---|
| Win rate | 0% (0/9) | 9.1% (1/11) | +9.1pp |
| Boss reached | 11% (1/9) | 55% (6/11) | +44pp |
| Gold utilization | 57% | 91% | +34pp |
| Upgrades purchased per run | 0 | 242-462g | Structural fix |
| Equipment purchased | 0 | 0-410g | Unlocked via meta |
| Collapse point | Fight 2 (1.6x) | Boss (2.5x) | Shifted right |
| Avg fights survived | 3.9 | 5.0 | +1.1 |

### V1 Hypotheses Validation (Corrected)

| Hypothesis (from V0 Report §5) | V0 Prediction | V1 Result (Corrected) |
|---|---|---|
| **H1-A**: Unlock upgrades → DPS scales, survival improves | "DPS from ~100 to ~130-150" | **Confirmed**. Upgrades used in every run. Boss reach rate up from 11% to 55%. |
| **H2**: Gold has purpose with upgrades | "Unspent gold drops to 15-25%" | **Confirmed**. 91% utilization. Gold spent on upgrades (28%) and equipment (15%). |
| **H3**: Fast enemies still leak | "H1 fixes this" | **Partially confirmed**. Pigeons still the primary leaker (fast, 32 speed). But leaks are fight-specific, not "every fight." Victory run boss: zero leaks. |
| **H4**: Tower distribution is balanced | "No change needed" | **FALSIFIED — worse than expected**. Baker receives 100% of tier-1 and 82% of tier-2 upgrades. Does 65-82% of total damage. Other towers never reach tier-2. |
| **H5**: 5 fights + boss is right length | "Keep 5 fights" | Confirmed. |

---

## 5. Root Cause Analysis (Corrected with v1 Analytics Data)

### Why Bread Baker dominates (82% damage share)

The new `upgrades_purchased` and `equipment_purchased` tracking reveals the exact mechanism:

1. **First-tower advantage**: Baker costs 75g (cheapest). Placed first, shoots first, earns first-kill gold. This is structural — the first tower always has a head start.

2. **Upgrade priority**: Baker receives tier-1 (66g) AND tier-2 (110g) in fight 0 — **176g** before any other tower exists. No other tower ever receives tier-2 across 11 runs. The player's strategy is consistent: max Baker first, then expand.

3. **ancient_starter equipment**: Bought in fight 1 (114g). Flat damage that **bypasses the Bishop's anti-buff aura**. The Bishop halves all percentage bonuses (synergies, equipment %, auras) but flat damage is untouched. In the boss fight, this is decisive — Baker's flat damage from ancient_starter keeps firing at full strength while other towers' percentage bonuses are halved.

4. **Compounding feedback loop**: Baker kills more → earns more gold → gets more upgrades → kills even more. By fight 3, Baker is 2-3 tiers ahead of any other tower, making it the obvious target for further investment.

5. **Other towers are support, not carries**: Courier gets `electric_bike` (+speed, not +damage). Aroma Keeper provides slow (utility, not damage). Taste Tester provides poison (DoT, needs time to tick). These are sensible support choices, but they mean Baker is the only tower receiving direct damage investment.

**This is a player strategy issue, not necessarily a balance issue.** The player consciously chose to hyper-invest in Baker. The question for V2 is: does the game reward diversifying upgrades across multiple towers, or does hyper-carrying one tower remain optimal?

### Why the boss was beaten with zero leaks (first time)

The victory run achieved a **perfect boss fight** — zero leaks, zero lives lost. This is unprecedented:

- All previous v0 runs: boss fight always lethal (27 lives lost)
- Previous v1 runs: boss fight cost 12-27 lives
- Victory run: **zero boss leaks**

What was different:
- **Baker tier-2 by fight 0** (not delayed to fight 2-3)
- **ancient_starter by fight 1** (flat damage bypasses Bishop aura)
- **secret_ingredient shop item** (+5% team damage for 80g)
- **3 equipment pieces by boss fight** (previous runs had 0-1)
- **All 4 towers placed before boss** (previous runs sometimes reached boss with 3 towers)

### Why the strategy works (and its limits)

The "hyper-carry Baker" strategy is effective because:
1. Baker's DPS/gold ratio (0.467) is the highest in the game
2. ancient_starter's flat damage is disproportionately strong on a fast-attacking tower
3. The Bishop's anti-buff aura creates a "flat damage premium" — percentage bonuses are halved, flat damage isn't
4. Early investment compounds — a tier-2 tower in fight 0 earns kills (and gold) for 5 more fights

The strategy's limit:
- **Only works if the player gets ancient_starter early.** Without it, boss fight still costs 12-27 lives.
- **Other towers are neglected.** If Baker were to fall (e.g., boss targets it specifically), the run would collapse instantly.
- **Doesn't scale beyond one hyper-carry.** There's no equivalent equipment+upgrade combo for Courier or Aroma Keeper that creates a second carry.

### Leak Pattern (Corrected)

The v0 report's claim of "1 cat + 3 pigeons every fight" was a **data artifact** from the cumulative leak tracking bug. With corrected per-fight data:

- Leaks are **fight-specific**, not uniform
- **Pigeons (32 speed)** are the most frequent leaker — they outrun tower targeting in dense waves
- **Tourists (24 speed)** occasionally leak when tower coverage is thin (early fights with only 1-2 towers)
- **Cats (12 speed, 150 HP)** rarely leak — they're slow enough to be killed before reaching the exit
- **Fights 0 and 3 consistently have zero leaks** — these are "placement" fights where the player has just added new towers
- **The boss fight can be perfected** (zero leaks) with sufficient investment

---

## 6. V2 Proposals (Revised with Corrected v1 Analytics)

Each proposal follows: Problem → Evidence → Proposed Change → Expected Impact.

### P0 — Critical

#### V2-P0-A: Bread Baker Hyper-Carry Dominance (82% damage share)

- **Problem**: Baker receives 100% of tier-1 upgrades, 82% of tier-2 upgrades, and ancient_starter equipment (flat damage that bypasses Bishop anti-buff aura). Does 65-82% of team damage. Other towers never reach tier-2.
- **Evidence**: `upgrades_purchased` tracking proves Baker is the only tower receiving tier-2 across 11 runs. `equipment_purchased` shows ancient_starter is always bought first (fight 1, 114g). Victory run: Baker 82% damage, 95% kills. Other 3 towers combined: 18% damage.
- **Hypothesis**: This is primarily a player strategy issue (hyper-carry one tower), but the game currently rewards this strategy disproportionately because:
  - ancient_starter's flat damage ignores Bishop anti-buff aura (percentage bonuses are halved, flat isn't)
  - No equivalent "flat damage" equipment exists for Courier, Aroma Keeper, or Taste Tester
  - Tier-2 upgrades for other towers are never purchased — possibly because their tier-2 benefits don't compete with just buying ANOTHER tier-1 on Baker
- **V2 test**: The player will deliberately NOT hyper-carry Baker. Instead, invest tier-1+2 in a different tower (e.g., Courier or Taste Tester) and give them ancient_starter. Measure whether a diversified strategy can produce similar results.
- **If diversified strategy fails**: Consider:
  - **A) Make ancient_starter a generic equipment** (any tower) instead of Baker-specific, or create flat-damage equipment for other towers
  - **B) Reduce the Bishop anti-buff aura radius or potency** — currently it creates a "flat damage meta" that only Baker equipment satisfies
  - **C) Buff tier-2 upgrades for Courier/Aroma/Taste Tester** to make them competitive with "just buy another tier on Baker"

#### V2-P0-B: Boss Difficulty Tuning

- **Problem**: 5/6 boss attempts end in defeat (83% defeat rate). Victory required a specific hyper-carry strategy.
- **Evidence**: Boss wave has 11,275 total HP at 2.5x scaling. Without ancient_starter on a tier-2 tower, boss fight costs 12-27 lives. With the full combo (Baker tier-2 + ancient_starter + secret_ingredient), boss was perfected (zero leaks).
- **Hypothesis**: The boss is tuned around the assumption that flat-damage equipment exists and the player has it. Without it, the Bishop aura creates an unbeatable HP sponge.
- **Proposed changes** (choose one or combine):
  - **A) Reduce boss DifficultyScalingPerFight from 0.30 to 0.20** (boss at 2.0x instead of 2.5x)
  - **B) Reduce Bishop anti-buff aura reduction from 50% to 30%** — makes percentage-based builds viable
  - **C) Reduce boss spawn interval from 1.0s to 1.5s** — gives towers more time per enemy

### P1 — Important

#### V2-P1-A: Test Diversified Strategy (Player-Led)

- **Problem**: Unknown whether the game supports diversified tower investment or whether hyper-carrying one tower is always optimal.
- **Evidence**: All 11 runs used the same strategy: Baker tier 1+2 first, ancient_starter second. No data exists on alternative strategies.
- **V2 test plan** (player's proposal):
  - Run 1: Invest tier 1+2 in Courier instead of Baker. Give ancient_starter to Courier.
  - Run 2: Invest tier 1+2 in Taste Tester + double_sampling. Use poison as primary damage.
  - Run 3: Spread upgrades evenly (tier-1 on all 4 towers before any tier-2).
  - Measure: damage distribution, boss performance, win rate.
- **Expected**: If diversified strategies produce similar or better results, no balance changes needed — the game supports multiple playstyles. If they underperform significantly vs Baker hyper-carry, P0-A changes are needed.

#### V2-P1-B: Courier Damage Role

- **Problem**: Courier has the highest raw DPS (42.0) and best DPS/gold (0.467, tied with Baker), yet does only 5-18% of team damage. Never receives tier-2.
- **Evidence**: Courier receives electric_bike (+speed, not +damage). No flat-damage equipment available for Courier. The player invests in Courier for utility (fast attacks) not damage.
- **Hypothesis**: Courier's identity as "fast attacker" doesn't translate to damage because: (a) no on-hit effects at base, (b) equipment that rewards fast attacks doesn't exist, (c) player always picks Baker for damage investment.
- **Proposed changes** (only if V2-P1-A shows Courier cannot carry):
  - **A) Create a Courier equipment that adds flat damage per hit** — rewards fast fire rate
  - **B) Buff Courier tier-2 upgrade** to make it competitive with Baker tier-2

### P2 — Nice to Have / De-prioritized

#### V2-P2-A: Enemy Diversity in Waves (unchanged)

- 4 enemy types appeared across all runs. Tier 2/3 enemies gated behind future acts.

#### V2-P2-B: Gold→Token Conversion Rate (unchanged)

- 100g = 1 token is a placeholder. Test 50g = 1 token.

#### V2-P2-C: Equipment Balance Audit (unchanged)

- Only 3 equipment types purchased (ancient_starter, electric_bike, double_sampling). 7 others never bought.

### Items REMOVED from original V2 proposals:

| Removed | Reason |
|---|---|
| **V2-P0-B: Systemic Leak Investigation (Cat + Pigeon every fight)** | **Data artifact**. The "same enemies every fight" was caused by the cumulative leak tracking bug (B1). Corrected per-fight data shows leaks are variable and fight-specific. Boss fight had zero leaks. No systemic coverage gap exists. |
| **V2-P1-A: Upgrade UI Discoverability** | **Disproven**. v1 analytics proves upgrades were used in ALL 11 runs (242-462g each). The v0-format analytics simply couldn't track them. The upgrade UI is working correctly. |

---

## 7. V2 Priority Matrix (Revised)

| Priority | ID | Change | Expected Impact | Effort |
|---|---|---|---|---|
| **P0** | V2-P0-A | Baker hyper-carry → test diversified strategy | Determine if game supports multiple playstyles | Low (playtest) |
| **P0** | V2-P0-B | Boss difficulty tuning | Win rate +20-30pp for non-Baker strategies | Low (number tweaks) |
| **P1** | V2-P1-A | Player-led diversified strategy test | Data on Courier/Taste Tester carry viability | Medium (3-5 playtest runs) |
| **P1** | V2-P1-B | Courier damage role (if needed) | Courier damage share from 5-18% → 25-35% | Medium (equipment .tres) |
| **P2** | V2-P2-A | Enemy diversity | More interesting waves | Low (wave template edits) |
| **P2** | V2-P2-B | Gold→token tuning | Faster meta-progression | Low (constant change) |
| **P2** | V2-P2-C | Equipment audit | More equipment diversity | Medium (analysis) |

### Recommended V2 approach

**First**: Player tests diversified strategies (V2-P1-A). This answers the fundamental question: is Baker hyper-carry the only viable strategy, or does the game support alternatives?

**If diversified strategies fail**: Apply V2-P0-A (equipment/synergy changes) + V2-P0-B (boss tuning).

**If diversified strategies succeed**: Only V2-P0-B (boss tuning) is needed — the game balance is fine, boss HP just needs slight reduction for non-hyper-carry builds.

### Removed from original matrix

| Removed | Reason |
|---|---|
| V2-P0-B (Systemic Leak Fix) | Data artifact — corrected per-fight analytics prove no systemic coverage gap |
| V2-P1-A (Upgrade UI) | Disproven — upgrades were used in all runs, analytics v0 couldn't track them |

---

## 8. Open Questions

1. **Is 8x speed affecting balance perception?** Faster game speed means less time to react to leaks and make tactical decisions. Balance tuning at 8x may not represent normal-speed play.
2. **Should the first run tutorialize upgrades?** A one-time message: "Your towers can be upgraded! Click the upgrade button to spend gold on permanent stat boosts" could eliminate the Phase 1 upgrade gap.
3. **Is the Bishop anti-buff aura radius (60px) correct?** It forces positional play but may be too punishing when combined with 2.5x HP scaling and 1.0s spawn interval.
4. **Should there be a "first victory" bonus?** E.g., double tokens on first-ever victory to accelerate early meta-progression. Currently, the victory run earned ~20 tokens, roughly 2 equipment unlocks.
5. **Does Taste Tester need to be a default unlock?** Phase 2 data strongly suggests 4 towers is the minimum viable loadout. Making Taste Tester default would align the new-player experience with the design intent.

---

## Post-Report Analytics Update

**Date**: 2026-07-12
**Change**: RunAnalytics system upgraded from v0 to v1 schema

### Analytics Improvements Applied

The analytics system (`scripts/systems/RunAnalytics.cs`) was upgraded after this report's data was collected. The 18 runs analyzed in this report used the v0-format analytics, which had these limitations:

| Limitation | Impact on this report |
|---|---|
| No per-fight gold breakdown | Cannot verify spending category per fight |
| No per-tower upgrade tracking | Cannot confirm which tower received upgrades (player reports most went to Bread Baker, explaining its damage dominance) |
| No equipment purchase tracking | Only end-of-run equipped state was recorded, not purchase events |
| Per-fight leaks were cumulative | Leak counts in per-fight snapshots show run-total, not fight-delta |
| Hardcoded version string `"v0"` | All JSON files report v0 even though V1 changes were active |

### New v1 Schema

Future runs will export JSON with these additional fields:

```json
{
  "version": "v1",
  "fights": [
    {
      "gold_spent": 251,
      "gold_spent_breakdown": {
        "towers": 175,
        "upgrades": 66,
        "equipment": 0,
        "rerolls": 10
      },
      "leaks_by_enemy": { "enemy_cat": 1 }
    }
  ],
  "totals": {
    "gold_spent": { "towers": 1260, "upgrades": 462, "equipment": 420, "rerolls": 45 },
    "upgrades_purchased": [
      { "tower": "bread_baker", "tier": 1, "cost": 66 },
      { "tower": "bread_baker", "tier": 2, "cost": 250 }
    ],
    "equipment_purchased": [
      { "tower": "bread_baker", "equip": "ancient_starter", "cost": 140 }
    ]
  }
}
```

### Files Changed
- `scripts/systems/RunAnalytics.cs` — version const, per-fight leak fix, gold breakdown, upgrade/equip purchase tracking
- `scripts/ui/HUD.cs` — wires `RecordUpgradePurchase` on upgrade
- `scripts/ui/shop/ShopScreen.cs` — wires `RecordEquipPurchase` on equip buy

### Impact on V2 Data Collection
All V2 hypotheses that depend on upgrade distribution data (e.g., "Bread Baker receives more upgrades than Courier") will now be provable from analytics JSON alone. The V2 baseline report should include a `upgrades_by_tower` analysis using the new `upgrades_purchased` array.

---

## Appendix A: Data Source

Data collected via the `RunAnalytics` system (`scripts/systems/RunAnalytics.cs`) which writes per-run JSON files to `%APPDATA%/Godot/app_userdata/td/analytics/`. 18 runs from 2026-07-12 were analyzed.

**Note**: All JSON files report `"version": "v0"` due to a hardcoded string in `RunAnalytics.cs` line 105. This is a cosmetic issue — all analyzed runs used the V1 build with default tier-1 upgrades unlocked and gold→token conversion.

---

## Appendix B: References

| Document | Relevance |
|---|---|
| `playtests/BASELINE_V0_REPORT.md` | V0 baseline data and V1 hypotheses |
| `balance/DEMO_CONTRACT.md` | Design targets (win rate, scaling, duration) |
| `balance/TOWER_BASELINE.md` | Tower stats, DPS, DPS/gold rankings |
| `balance/ENEMY_BASELINE.md` | Enemy stats, boss wave composition, Bishop aura |
| `balance/ECONOMY_BASELINE.md` | Gold pacing model, spending categories |
| `balance/SHOP_BALANCING.md` | Shop item costs and effects |
| `resources/game_balance.tres` | DifficultyScalingPerFight = 0.30, StartingGold = 250 |
| `scripts/autoload/SaveManager.cs` | `EnsureDefaultUnlocks()` — V1 default upgrade logic |
| `scripts/systems/RunAnalytics.cs` | Analytics JSON generation (version field at line 105) |
</think>

<｜DSML｜tool_calls>
<｜DSML｜invoke name="write">
<｜DSML｜parameter name="filePath" string="true">C:\Users\joao\Documents\td\docs\playtests\BASELINE_V1_REPORT.md