# Bread Monk — "The Silent Fermenters"

*"The dough rises in silence."*

Bread Monks spend years inside mountain monasteries where speaking is forbidden.

Communication happens entirely through kneading techniques.

Masters can identify another monk's emotional state simply by watching them fold dough.

Rather than fighting themselves, they strengthen the spirit of nearby brothers.

Their chants synchronize heartbeats, breathing and fermentation cycles until everyone works in perfect harmony.

Some monks have meditated so long that birds mistake them for statues.

No monk has ever explained the Prayer Beads of Gluten.

Everyone is too afraid to ask.

---

## Stats

| Stat | Value |
|------|-------|
| Cost | 160g |
| Damage | 30 |
| Fire Rate | 1.0 |
| Range | 50 |
| Base DPS | 30 |
| T1 DPS | 45.6 |
| T2 DPS | 75 |
| Total Investment | 400g |
| Special | Aura: 60 range, +10% damage / +10% fire rate to other towers (not self) |
| Mechanics | Aura buffer |

## Equipment

| Item | Cost | Effect | Lore |
|------|------|--------|------|
| Sacred Robes | 100g | +15% aura range | *Woven from flour sacks used in the oldest monastery.* |
| Prayer Beads of Gluten | 150g | Aura potency ×1.05 (10% → 10.5%), but tower **stops attacking** (extreme trade-off — loses ~75 DPS for +0.5 p.p. aura) | *Each bead represents a year of silent fermentation. The monk becomes so focused that they stop fighting entirely.* |

## Upgrades

| Upgrade | Cost | Damage | Fire Rate |
|---------|------|--------|-----------|
| Sacred Chant | 100g | +8 | +0.2 |
| Devotion Aura | 140g | +12 | +0.3 |

> Note: Upgrades only improve the Monk's personal damage/FR, not the aura magnitude (`AuraDamageBonusPercent` / `AuraFireRateBonusPercent` are static in TowerData).

## Synergies

### Grand Opening Rush

*"When every craft joins the feast, the streets move faster."*

The Order celebrates new bakeries with a chaotic festival involving monks, couriers, bakers, sages, and trucks all working at once. Nobody knows who is in charge, but bread appears everywhere.

**Effect**: +10% fire rate to all towers if 3+ tower types are on the map.

## Open Questions

1. Upgrades only buff personal attack, never aura %. Intentional?
2. Prayer Beads of Gluten: trade-off unbalanced — loses ~75 DPS for +0.5 p.p. aura. Needs `AuraPotencyMultiplier` buff or rework.
