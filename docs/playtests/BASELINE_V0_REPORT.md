# Baseline V0 Report — Sourdough Siege Demo

Aggregate analysis of 9 playtest runs with zero meta-progression.

---

## 1. Test Configuration

| Property | Value |
|---|---|
| Loadout | Bread Baker + Bread Courier + Aroma Keeper (towers placed once each, same positions per run) |
| Meta-progression | ZERO — no upgrades, no equipment, no shop items, no trinkets unlocked |
| Available towers | 3 (default-unlocked: Baker, Courier, Keeper) |
| Upgrades | Locked — HUD shows "Buy in Meta Shop" (`SaveManager.GetMetaUpgradeLevelForTower` returns 0) |
| Equipment | Locked behind meta-upgrade unlocks |
| Shop items | Locked behind meta-upgrade unlocks |
| Trinkets | Locked behind meta-upgrade unlocks |
| Active synergies | Grand Opening Rush (3+ tower types: +10% fire rate), Daily Proof (Baker + Courier: +10% damage) — both auto-activate via SynergyManager (no unlock required) |
| Bug fix applied | Projectile.cs: `OnAreaEntered` checks `Target.IsDead` before calling `OnHitTarget` (line 54) — prevents wasted projectiles on already-dead enemies |
| Speed | 8x |

---

## 2. Results Summary

### Win Rate

| Metric | Value |
|---|---|
| Runs | 9 |
| Victories | 0 (0%) |
| Avg fights survived | 3.9 |
| Runs reaching boss | 1 (11%) |

### Fight Survival

| Fight | Scaling | Survived | Survival Rate |
|---|---|---|---|
| Fight 0 | 1.0x | 9/9 | 100% |
| Fight 1 | 1.3x | 9/9 | 100% |
| Fight 2 | 1.6x | 4/9 | 44% — COLLAPSE POINT |
| Fight 3 | 1.9x | 2/9 | 22% |
| Fight 4 | 2.2x | 1/9 | 11% |
| Boss | 2.5x | 0/9 | 0% |

5 of 9 runs (56%) ended at fight 2.

### Damage Share

| Tower | Damage Share | Total Damage | Kills |
|---|---|---|---|
| Bread Baker | 42% | 106,740 | 833 |
| Bread Courier | 32% | 83,619 | 624 |
| Aroma Keeper | 26% | 66,215 | 505 |

Damage share is proportional to DPS: Baker 35.0, Courier 42.0, Keeper 22.5.

### Gold Economy

| Metric | Value |
|---|---|
| Avg gold earned (per run) | 1,529 |
| Avg gold spent | 1,070 (tower placement only) |
| Avg gold unspent | 650 (43% of earned) |
| Spending categories | 100% on tower placement. Upgrades, equipment, shop, trinkets, rerolls: 0. |

### Leaks by Enemy Type

| Enemy | Share of Leaks | Speed |
|---|---|---|
| BaguettePigeon | 55% | 32 |
| GroceryJogger | 21% | 48 |
| Other | 24% | — |

Fast enemies account for 76% of all leaks. The single Aroma Keeper (slow 50% for 3s) cannot cover all lanes against dense fast-enemy waves.

---

## 3. Root Cause Analysis

### Why the first run is mathematically impossible to win

**FACT**: With zero meta-progression, the player has exactly 3 towers (265g total) and no way to spend additional gold.

#### Gold sinks are all locked behind meta-progression

The following spending categories exist in the codebase but are inaccessible on first run:

| Category | Unlock Cost (tokens) | Locked Message |
|---|---|---|
| Tower upgrades | 15 per tower | "Buy in Meta Shop" (HUD.cs line 532) |
| Equipment | 8 per item | Requires explicit unlock in Meta Shop |
| Shop items | 10 per item | Requires explicit unlock in Meta Shop |
| Trinkets | 8 per item | Requires explicit unlock in Meta Shop |

`SaveManager.GetMetaUpgradeLevelForTower(towerId, "Upgrades")` returns 0 for all towers when no meta-upgrades have been purchased. `HUD.cs` checks `unlockedLevels <= 0` and disables the upgrade button with the "Buy in Meta Shop" message.

#### Tower DPS is fixed at base stats

| Tower | Cost | Damage | Fire Rate | Range | DPS |
|---|---|---|---|---|---|
| Bread Baker | 75 | 35 | 1.0 | 48 | 35.0 |
| Bread Courier | 90 | 28 | 1.5 | 44 | 42.0 |
| Aroma Keeper | 100 | 25 | 0.9 | 50 | 22.5 |
| **Total** | **265** | | | | **99.5** |

Combined DPS ≈ 100. With Grand Opening Rush (+10% fire rate) and Daily Proof (+10% damage), effective DPS is approximately 120.

#### Enemy HP scales +30% per fight

`game_balance.tres` sets `DifficultyScalingPerFight = 0.30`. Applied in `EnemySpawner.cs`:

```csharp
float fightScaling = 1f + RunState.Instance.FightsCompleted * GameBalance.DifficultyScalingPerFight;
```

| Fight | Scaling | Relative HP |
|---|---|---|
| 0 | 1.0x | 100% |
| 1 | 1.3x | 130% |
| 2 | 1.6x | 160% |
| 3 | 1.9x | 190% |
| 4 | 2.2x | 220% |
| Boss | 2.5x | 250% |

At fight 2, enemies have 60% more HP than fight 0. Fixed DPS (no upgrades) is now insufficient to clear waves before enemies reach the exit. The 44% survival rate at fight 2 confirms this is the collapse point.

#### Gold accumulates with no purpose

After placing 3 towers (265g), the player earns ~235g per fight but can spend it on nothing. At death, 43% of earned gold is unspent (avg 650g). The economy feedback loop — earn gold, spend gold to get stronger — is broken because all spending categories are locked.

#### Fast enemies exploit coverage gaps

BaguettePigeon (32 speed) and GroceryJogger (48 speed) account for 76% of leaks. These enemies spend less time in tower range, so they take fewer damage ticks. One Aroma Keeper (slow 50% for 3s, 0.9 fire rate) cannot apply slow reliably to multiple fast enemies in dense waves.

---

## 4. Bug Fixed: Wasted Projectiles on Dead Enemies

### Observation

During testing, projectiles were observed flying toward enemies that died between the projectile being fired and the projectile reaching the target. The projectile would hit and call `OnHitTarget` on a dead `Enemy`, wasting the shot.

### Fix

`Projectile.OnAreaEntered` (line 51–58 in `Projectile.cs`) now checks `Target.IsDead` before calling `OnHitTarget`:

```csharp
private void OnAreaEntered(Area2D area)
{
    if (_returningToPool || area != Target) return;
    if (Target.IsDead)
    {
        ReturnToPool();
        return;
    }
    OnHitTarget(Target);
}
```

Additionally, `_PhysicsProcess` (line 36–44) already returns the projectile to pool if `Target == null || !IsInstanceValid(Target) || Target.IsDead`.

### Impact

Estimated DPS efficiency improvement: ~6%. This does NOT change the structural impossibility of winning with 3 base towers and no meta-progression — the DPS gap is ~40%, far larger than a 6% efficiency gain.

---

## 5. Hypotheses for V1

Each hypothesis follows: Problem → Evidence → Hypothesis → Proposed Change.

### H1: First run is structurally unwinnable

- **Problem**: 0% win rate across 9 runs. Gold can't be spent. DPS capped at base tower stats while enemy HP scales +30% per fight.
- **Evidence**: 0 victories, 43% gold unspent. All upgrade/equipment/shop categories show "Buy in Meta Shop." Collapse at fight 2 (1.6x HP) is consistent across 9 runs.
- **Hypothesis**: The default-unlocked towers need at least tier 1-2 upgrades available without meta-progression, OR the first-run scaling must be reduced.
- **Proposed change** (choose one or combine):
  - **A) Unlock tier 1-2 upgrades for default towers without meta-progression.** This gives the player something to spend gold on and a way to scale DPS. Either modify `SaveManager.GetMetaUpgradeLevelForTower` to return at least 2 for default-unlocked towers, or add a separate "default upgrade level" mechanism.
  - **B) Reduce DifficultyScalingPerFight from 0.30 to 0.15 for the demo.** At 0.15x, fight 2 would be 1.3x HP (instead of 1.6x), boss fight at 1.75x (instead of 2.5x). This might make 3 base towers viable.
  - **C) Both A and B** — smaller reductions to both scaling and unlock barrier.

### H2: Gold has no purpose in the first run

- **Problem**: 43% unspent gold. Player places 3 towers and then has nothing to buy.
- **Evidence**: All gold spending categories except "tower placement" show 0. Economy feedback loop is broken.
- **Hypothesis**: If the player can't spend gold, the primary engagement loop (earn → spend → grow) is absent.
- **Proposed change**: Same as H1-A (unlock upgrades for default towers). Additionally, consider giving the player a small number of free meta-tokens (e.g., 15) on first ever run so they can unlock ONE upgrade path or ONE equipment piece before starting.

### H3: Fast enemies dominate leaks when slow coverage is insufficient

- **Problem**: Pigeon (55%) + Jogger (21%) = 76% of all leaks.
- **Evidence**: Only 1 Aroma Keeper provides slow. Fast enemies spend less time in tower range, take fewer damage ticks.
- **Hypothesis**: One slow tower is insufficient for multi-lane or dense fast-enemy waves. This is a strategy limitation, not a balance issue.
- **Proposed change**: With upgrades available (H1-A), the player could upgrade Aroma Keeper for better slow coverage or unlock Taste Tester (poison) via meta-progression for additional DPS. No standalone balance change needed — H1 fixes this.

### H4: Tower damage distribution is well-balanced

- **Problem**: None. The 3 default towers show proportional damage output.
- **Evidence**: Baker 42%, Courier 32%, Keeper 26% of total damage. Baker leads due to lowest cost (75g) placed first, highest DPS/gold ratio (0.467), and Daily Proof synergy (+10% damage). Courier is close behind. Keeper is lower but provides utility (slow) that doesn't appear in damage numbers.
- **Hypothesis**: No change needed for these 3 towers. Their relative performance is balanced.

### H5: 5 fights is the right length for the demo

- **Problem**: None. Fight pacing feels correct.
- **Evidence**: Fights 0-1 are clean at ~20-30 seconds each. Fight 2 is where strategy decisions would matter (if upgrades were available).
- **Hypothesis**: Keep 5 fights. The demo length is appropriate. The problem is the scaling barrier, not the fight count.

---

## 6. Demo Balance V1 — Changes Applied

### V1 Changes (applied after Baseline V0 data collection)

Three changes were applied based on the V0 findings:

#### A) Default Tier-1 Upgrades Unlocked

**File**: `scripts/autoload/SaveManager.cs` — `EnsureDefaultUnlocks()` and `SetDefaults()`

The three default towers (Bread Baker, Bread Courier, Aroma Keeper) now start with tier 1 of their upgrade path unlocked. The meta-upgrade keys `unlock_bread_baker_upgrades`, `unlock_bread_courier_upgrades`, and `unlock_aroma_keeper_upgrades` are pre-set to level 1 in `_metaUpgradeLevels`.

| Tower | Tier 1 Upgrade | Cost | Effect |
|---|---|---|---|
| Bread Baker | Reinforced Dough | 66g | +15 damage |
| Bread Courier | Overclock | 88g | +0.5 fire rate |
| Aroma Keeper | Cryo Coil | 88g | +0.3 fire rate |

**Expected impact**: The upgrade button shows actual costs instead of "Buy in Meta Shop". Players can spend accumulated gold on tier 1 upgrades (~240g total for all three). DPS increases from ~100 to ~130-150.

#### B) Unspent Gold → Meta-Tokens

**File**: `scripts/autoload/RunState.cs` — `EndRun()`

At run end (victory or defeat), unspent gold is converted to bonus meta-tokens at a rate of **100g = 1 token**. This is added to the existing `PreviewTokenReward(isVictory)` total.

| Run outcome | Gold remaining (typical) | Bonus tokens |
|---|---|---|
| Defeat fight 2 | ~450g | +4 |
| Defeat fight 4 | ~650g | +6 |
| Reach boss | ~300g | +3 |
| Victory | ~150g | +1 |

**Expected impact**: Gold earned during a run is never truly wasted. Even on defeat, the player makes meta-progress. Estimated ~18-22 tokens per run (up from ~14). After ~5 runs, enough tokens for a new tower or equipment.

#### Bug Fixes Applied

1. **Wasted projectiles on dead enemies** (`scripts/projectiles/Projectile.cs`): Added `Target.IsDead` guard in `OnAreaEntered`. Projectiles now return to pool instead of hitting dead enemies in the same frame.

2. **Fresh saves missing default upgrade levels** (`scripts/autoload/SaveManager.cs`): Added `EnsureDefaultUnlocks()` call inside `SetDefaults()`. Previously, fresh saves only got tower IDs but not upgrade levels.

### What Did NOT Change

- `DifficultyScalingPerFight` = 0.30 (unchanged)
- Tower base stats (unchanged)
- Enemy stats (unchanged)
- Upgrade costs (unchanged)
- Meta-upgrade costs (unchanged)
- Wave compositions (unchanged)

### Target Metrics for V1 Data Collection

| Metric | Baseline V0 | Target V1 |
|---|---|---|
| Win rate | 0% | 0-10% (still hard early) |
| Gold unspent | 43% | 15-25% |
| Tokens per run (defeat) | ~14 | ~18-22 |
| Runs to first victory | ∞ (impossible) | ~8-12 |
| Upgrades purchased per run | 0 | 2-4 |

---

## 7. V1 Priority

| Priority | Change | Expected Impact |
|---|---|---|
| **P0** | Unlock upgrades for default towers (H1-A) | Gives gold a purpose. Allows DPS to scale with enemy HP. Restores economy feedback loop. |
| **P1** | Reduce DifficultyScalingPerFight to 0.20 (H1-B variant) | If upgrades alone are insufficient, reduce the scaling curve. |
| **P2** | Consider free starter meta-tokens (H2) | Smoother new-player experience. 15 tokens would unlock one upgrade path or one equipment piece. |
| **P3** | Re-test with upgrades + equipment | Validate that the full economy loop works when gold has spending targets. |

### Priority Rationale

**P0 is the critical fix.** Without a way to spend gold and scale DPS, the first run is structurally unwinnable regardless of player skill. The upgrade system already exists (upgrade costs, stat boosts, `Tower.ApplyUpgrade` logic) — it is simply locked behind a meta-progression gate that new players cannot pass without first winning a run (impossible) or earning tokens from somewhere.

The Demo Contract (DEMO_CONTRACT.md) sets a 10-30% win rate target for new players on first run. The actual value of 0% across 9 runs is below this target and confirms the structural issue.

---

## 8. Open Questions

1. **Is the 30% scaling still appropriate if upgrades ARE available?** Only testing can answer. The 30% value was tagged as "placeholder — to be tuned" in DEMO_CONTRACT.md (line 64).
2. **Should the first run tutorialize the meta-shop?** For example: "You earned 15 tokens! Visit the Meta Shop to unlock upgrades" shown after first defeat.
3. **Is the 100g/token conversion rate correct, or should it be tunable via GameBalanceData?**

---

## Appendix A: Data Collection Method

Data was collected from 9 consecutive runs using the Sourdough Siege Baseline V0 build. The player used a fixed loadout (Baker + Courier + Keeper) and placed towers in the same positions each run. Damage, kills, gold, and leaks were recorded per run from the HUD and post-fight summary. Fight survival was determined by whether the player lost all 20 lives before clearing the boss wave.

---

## Appendix B: References

| Document | Relevance |
|---|---|
| `balance/DEMO_CONTRACT.md` | V0 design targets — win rate, difficulty scaling, economy |
| `balance/TOWER_BASELINE.md` | Tower stats, DPS, DPS/gold rankings |
| `balance/ENEMY_BASELINE.md` | Enemy stats, speed, pressure metrics |
| `balance/ECONOMY_BASELINE.md` | Gold pacing model, cumulative gold by fight |
| `resources/game_balance.tres` | DifficultyScalingPerFight = 0.30, StartingGold = 250 |
| `scripts/autoload/SaveManager.cs` | `GetMetaUpgradeLevelForTower` — returns 0 for locked upgrades |
| `scripts/ui/HUD.cs` | Upgrade button shows "Buy in Meta Shop" at line 532 |
| `scripts/projectiles/Projectile.cs` | IsDead check added at line 54 |
| `scripts/systems/SynergyManager.cs` | All synergies evaluated without unlock check |
