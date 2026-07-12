# Trinkets

> Rarity is currently cosmetic only — does not affect selection probability. All trinkets are "Common" except Proofing Time Candle (Rare).

| Trinket | Rarity | Effect | Downside | Scope |
|---------|--------|--------|----------|-------|
| Regular's Tip Jar | Common | Gain 100 gold immediately | — | One-shot |
| Starter's Blessing | Common | Restore 5 lives | — | One-shot |
| Secret Recipe Scroll | Common | +10% global damage for rest of run | — | All towers |
| Sacred Flour Dust | Common | +10% slow/poison strength | — | Aroma Keeper + Taste Tester |
| Proofing Time Candle | Rare | +15% attack speed | -5% range global | All towers |
| Oven Heart Ember | Common | +4 flat range global | — | All towers |
| Heretic Census List | Common | +10% damage vs basic (non-Heavy/Boss) enemies | — | All towers, situational |
| First Starter Vessel | Common | Every 30s gains +5 gold passive | — | Passive economy |
| Fermentation Diary | Common | +15% status effect duration | — | Aroma Keeper + Taste Tester |
| Crust Fragment Relic | Common | +12% crit damage | — | Only Crust Crusader |

## Notes

- **Secret Recipe Scroll**: Strongest all-around trinket. +10% global damage, no downside, all towers.
- **First Starter Vessel**: In a 20–30min run this accumulates ~200–300 gold total. Strong long-term despite Common rarity.
- **Proofing Time Candle**: Only Rare trinket but has a downside (-5% range). Buffed from +8% to +15% in V0 — now +15% attack speed for -5% range is a strong trade-off.
- **Crust Fragment Relic**: Only effective with Crust Crusader in loadout (`if (_data.HasCrit)` check in `Tower.ApplyData`).
- **Oven Heart Ember**: Buffed from +1 to +4 flat range in V0 — on 44–56px base that's ~7–9% increase, now noticeable.

## Open Questions
1. Rarity is purely cosmetic — no weight in selection probability, no correlation with power.
2. Crust Fragment Relic: only works with Crust Crusader in loadout. Acceptable for a run-wide trinket?
3. Proofing Time Candle: +15% attack speed at -5% range — is the range penalty meaningful enough to offset the attack speed gain?
