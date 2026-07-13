# Demo Contract — Baseline V0

Design targets for the demo ("The Innocent Herd", Act 1).

---

## Target Audience

**Demo player**: new to the game, may have 0–5 runs of experience. No prior knowledge of tower roles, synergies, or enemy tiers is assumed.

---

## Run Structure

| Property | Value |
|---|---|
| Act | 1 — "The Innocent Herd" |
| Fights | 5 normal + 1 boss |
| Waves per fight | 4–7 (random draw from tier1 pool) |
| Slot machine | Fight 35%, Shop 20%, Heal 15%, Miniboss 10%, Treasure 20% |
| Boss trigger | Automatic after 5 fights |
| Difficulty scaling | +30% per fight (DifficultyScalingPerFight = 0.30) |

---

## Target Run Duration

**20–30 minutes** for a full Act 1 run (5 fights + boss).

First fight: ~2–3 minutes (learning, placing, understanding).
Mid fights: ~3–4 minutes each (more enemies, higher scaling).
Boss fight: ~4–6 minutes (complex multi-wave enemy composition).

---

## First Significant Decision

**< 2 minutes** from clicking "Start Run".

The Loadout screen with 3 default-unlocked towers (Bread Baker, Bread Courier, Aroma Keeper) presents the player with choice:
- Which 1–4 towers to bring (default 3 available immediately).
- First tower placement after the Briefing screen.

No mandatory tutorial pop-ups. Discovery is organic.

---

## Win Rate Targets

> These are **hypotheses**, not measured values. To be validated after data collection (Baseline V0 playtests).

| Player type | Win rate target |
|---|---|
| Experienced player (familiar with all mechanics) | 60–75% |
| New player, first run | 10–30% |
| New player, after 5 runs | 30–50% |

---

## Design Constraints

1. **All 10 towers should have an identifiable strategic niche.** No tower should feel like a strict upgrade or downgrade of another.
2. **Zero intentionally useless choices.** Every loadout combination, equipment, and trinket should be viable in some context.
3. **Difficulty scaling of 30% per fight** is a placeholder — to be tuned after telemetry data shows where players lose most often.
4. **Starting resources**: 275 gold, 20 lives.
5. **Default unlocks**: Bread Baker, Bread Courier, Aroma Keeper. Remaining 7 towers unlocked via meta-progression tokens.

---

## Success Criteria for V0

To be measured during the Baseline V0 data collection (10–20 playtest runs):

| Metric | Data source |
|---|---|
| Average run duration | Manual timing or telemetry |
| Win rate across 10–20 playtests | Session records |
| Fight where most defeats occur | RunState logs / post-session report |
| Gold remaining at boss | End-of-run economy snapshot |
| Lives lost per fight | Per-fight damage tracking |
| Boss completion rate | Win/loss records on boss wave |
| Tower usage diversity | Which towers are selected in loadout + placed |
| Equipment purchase rate | Shop logs |
| Trinket pick rate | Treasure choice logs |

---

## Known Gaps

- No tutorial system — first-run players may miss tower roles or the Loadout screen's stat preview.
- No sound — cannot rely on audio cues for wave completion or enemy proximity.
- Flyer enemies have no gameplay distinction yet.
- Equipment balance (e.g. Golden Staff vs single-stat items) is placeholder.
