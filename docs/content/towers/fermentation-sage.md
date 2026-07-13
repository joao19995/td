# Fermentation Sage — "Keeper of Living Cultures"

*"Fermentation is a chain reaction. So is enlightenment."*

The Fermentation Sages are the greatest scholars of the Order, dedicating their lives to studying the mysterious bond between living yeast and the Mother Dough. They believe that every fermentation is connected, just as every soul can be converted. As their knowledge grows, so too does their ability to spread the Order's influence across entire armies.

Their libraries contain starters thousands of years old.

Every culture has a name.

Every bubble is recorded.

They teach that fermentation connects every living thing — one starter becomes two, two become four, four become an entire civilization.

This philosophy inspired their signature combat technique, where divine energy leaps from one heretic to another exactly as healthy yeast spreads through dough.

To the Sage, conversion is contagious.

---

## Stats

| Stat | Value |
|------|-------|
| Cost | 180g |
| Damage | 30 |
| Fire Rate | 0.9 |
| Range | 52 |
| Base DPS | 27 (+13.5 chain) = **40.5 total** |
| Full Upgrade DPS | 178.5 (+89.3 chain) = **267.8 total** |
| Total Investment | 1027g |
| Special | Chain: 1 bounce, 40 range, 0.5x damage on bounce. Bounces to nearest enemy. |
| Mechanics | Chain lightning |

> The Sage has no native Poison or Slow (`HasPoison` = false, `HasSlow` = false). This matters for equipment compatibility.

## Levels

| Level | Name | Cost | Damage | Fire Rate | Total Damage | Total FR | Direct DPS | Total DPS (+chain) |
|-------|------|------|--------|-----------|-------------|----------|------------|-------------------|
| I | Culture Scholar | — | 30 | 0.9 | 30 | 0.9 | 27 | 40.5 |
| II | Extended Culture | 110g | +8 | +0.2 | 38 | 1.1 | 41.8 | 62.7 |
| III | Wild Fermentation | 165g | +12 | +0.3 | 50 | 1.4 | 70 | 105 |
| IV | Master of Living Cultures | 242g | +15 | +0.3 | 65 | 1.7 | 110.5 | 165.8 |
| V | Archsage of the First Starter | 330g | +20 | +0.4 | 85 | 2.1 | 178.5 | 267.8 |

### Level I — Culture Scholar

*"Every bubble has a purpose."*

Newly appointed Sages spend decades observing living starters before they are trusted to cultivate one of their own.

Their first lesson is simple: *"Never rush fermentation."*

### Level II — Extended Culture

Cost: 110g — +8 Damage, +0.2 Fire Rate

*"Healthy cultures never stop growing."*

The Sage learns to sustain ancient yeast colonies far beyond their natural lifespan. Their living cultures become stronger, allowing each burst of fermented energy to spread with greater consistency.

### Level III — Wild Fermentation

Cost: 165g — +12 Damage, +0.3 Fire Rate

*"Order begins with controlled chaos."*

After years of experimentation, the Sage dares to cultivate wild strains once considered too unpredictable for the monasteries.

The cultures spread faster, stronger and with remarkable enthusiasm—much to the concern of everyone else.

### Level IV — Master of Living Cultures

Cost: 242g — +15 Damage, +0.3 Fire Rate

*"Every starter remembers its ancestors."*

Masters preserve fermentation cultures that have survived for centuries. They understand that every living starter shares an invisible connection, allowing their knowledge to flow through entire crowds like ripples through dough.

Even fellow Sages seek their guidance.

### Level V — Archsage of the First Starter

Cost: 330g — +20 Damage, +0.4 Fire Rate

*"The first culture still lives."*

Only the greatest scholars are permitted to study the legendary First Starter, the living culture believed to have been created alongside the Mother Dough herself.

Its bubbles are said to whisper forgotten recipes.

Its aroma carries memories older than kingdoms.

Where an Archsage walks, fermentation follows.

## Visual Evolution

| Level | Appearance |
|-------|------------|
| I | Simple scholar robes carrying a small proofing bowl filled with an active starter. |
| II | Larger ceramic fermentation vessel with glowing runes, leather satchel overflowing with scrolls and cultures. |
| III | Multiple glass flasks containing bubbling wild yeasts orbit around the Sage, green fermentation energy crackles between them. |
| IV | Ornate blue-and-gold robes embroidered with wheat patterns, a floating golden proofing bowl radiates chains of living fermentation energy. |
| V | The legendary First Starter levitates before the Sage inside a crystalline vessel, connected by glowing strands of living dough while ancient runes and swirling flour constantly circle around them. |

## Equipment

| Item | Cost | Effect | Lore |
|------|------|--------|------|
| Golden Proofing Bowl | 100g | +1 chain bounce | *A revered artifact re-purposed to amplify chain fermentation.* |
| Wild Yeast Culture | 150g | Chain +1 extra bounce, -10% initial damage (trade-off) | *Unstable, aggressive, and banned in three monasteries.* |

## Upgrades

| Level | Upgrade | Cost | Damage | Fire Rate |
|-------|---------|------|--------|-----------|
| I→II | Extended Culture | 110g | +8 | +0.2 |
| II→III | Wild Fermentation | 165g | +12 | +0.3 |
| III→IV | Master of Living Cultures | 242g | +15 | +0.3 |
| IV→V | Archsage of the First Starter | 330g | +20 | +0.4 |

## Synergies

### Holy Fermentation Network

*"Fermentation spreads farther than fear."*

Scholars discovered that sacred aromas could travel through the same invisible currents used by living yeast cultures. When an Aroma Keeper and a Fermentation Sage work together, conversion spreads through crowds like a chain reaction.

**Effect**: +30% range for slow/chain spread.

### Grand Opening Rush

*"When every craft joins the feast, the streets move faster."*

The Order celebrates new bakeries with a chaotic festival involving monks, couriers, bakers, sages, and trucks all working at once. Nobody knows who is in charge, but bread appears everywhere.

**Effect**: +10% fire rate to all towers if 3+ tower types are on the map.

## Open Questions

1. No native HasPoison/HasSlow despite shared lore with Aroma Keeper / Taste Tester — equipment compatibility is limited.
2. ~~Golden Proofing Bowl: currently broken.~~ **Resolved**: Golden Proofing Bowl redesigned — now grants +1 chain bounce (100g), no longer references status effects.
