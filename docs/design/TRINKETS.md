# Trinkets

> Rarity is currently cosmetic only — does not affect selection probability. All trinkets are "Common" except Proofing Time Candle (Rare).

| Trinket | Rarity | Effect | Downside | Scope |
|---------|--------|--------|----------|-------|
| Regular's Tip Jar | Common | Gain 100 gold immediately | — | One-shot |
| Starter's Blessing | Common | Restore 5 lives | — | One-shot |
| Secret Recipe Scroll | Common | +10% global damage for rest of run | — | All towers |
| Sacred Flour Dust | Common | +10% slow/poison strength | — | Aroma Keeper + Taste Tester |
| Proofing Time Candle | Rare | +8% attack speed | -5% range global | All towers |
| Oven Heart Ember | Common | +1 flat range global | — | All towers |
| Heretic Census List | Common | +10% damage vs basic (non-Heavy/Boss) enemies | — | All towers, situational |
| First Starter Vessel | Common | Every 30s gains +5 gold passive | — | Passive economy |
| Fermentation Diary | Common | +15% status effect duration | — | Aroma Keeper + Taste Tester |
| Crust Fragment Relic | Common | +12% crit damage | — | Only Crust Crusader |

## Notes

- **Secret Recipe Scroll**: Strongest all-around trinket. +10% global damage, no downside, all towers.
- **First Starter Vessel**: In a 20–30min run this accumulates ~200–300 gold total. Strong long-term despite Common rarity.
- **Proofing Time Candle**: Only Rare trinket but has a downside (-5% range). Weaker than several Commons with no trade-off.
- **Crust Fragment Relic**: Only effective with Crust Crusader in loadout (`if (_data.HasCrit)` check in `Tower.ApplyData`).
- **Oven Heart Ember**: +1px range on 44–56px base is ~2% increase — very small, possibly below perception threshold.

## Open Questions
1. Rarity is purely cosmetic — no weight in selection probability, no correlation with power.
2. Proofing Time Candle (Rare) seems weaker than several Commons without trade-off (especially Secret Recipe Scroll). Either buff it, implement rarity weight in selection, or accept "Rare" as a visual label.
3. Crust Fragment Relic: only works with Crust Crusader in loadout. Acceptable for a run-wide trinket?
4. Oven Heart Ember: +1px range is very small, possibly below play-perception threshold.
