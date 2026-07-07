# Enemies

> TTK (time-to-kill) is a qualitative reference against a single Bread Baker (35 DPS base / 84 DPS T2).

## Overview

| Enemy | HP | Speed | Gold | Dmg | Boss | Heavy | Special | TTK Baker Base | TTK Baker T2 |
|-------|----|-------|------|-----|------|-------|---------|----------------|--------------|
| Sliced Bread Tourist | 100 | 24 | 10 | 1 | — | — | Basic | 2.86s | 1.19s |
| Grocery Run Jogger | 60 | 48 | 15 | 1 | — | — | Fast, fragile | 1.71s | 0.71s |
| Lazy Alley Cat | 300 | 12 | 30 | 2 | — | Heavy | Early tank | 8.57s | 3.57s |
| Pigeon with a Stolen Baguette | 80 | 32 | 20 | 1 | — | — | "Flyer" (no own mechanic) | 2.29s | 0.95s |
| Microwave Meal Preacher | 200 | 20 | 25 | 2 | — | Heavy | Medium convert | 5.71s | 2.38s |
| Plastic Wrapped Sandwich Man | 180 | 28 | 22 | 2 | — | — | Medium resist | 5.14s | 2.14s |
| Frozen Dough Abomination | 400 | 10 | 35 | 3 | — | Heavy | Slow tank | 11.43s | 4.76s |
| Supermarket Overlord of White Bread | 900 | 18 | 80 | 4 | — | Heavy | Elite/miniboss | 25.71s | 10.71s |
| The Industrial Bread Dragon | 1000 | 16 | 100 | 5 | Boss | Heavy | Final boss | 28.57s | 11.9s |
| The Gluten Null Bishop | 1500 | 14 | 120 | 6 | Boss | Heavy | Anti-Buff Aura (60px, -50%) | 42.86s | 17.86s |

## Details

### Sliced Bread Tourist (`enemy_tourist`)
- **Stats**: 100 HP / 24 speed / 10 gold / 1 dmg
- **Analysis**: Generic low threat. Good tier1 default enemy.
- **Countered by**: Any basic tower.

### Grocery Run Jogger (`enemy_jogger`)
- **Stats**: 60 HP / 48 speed (fastest in game) / 15 gold / 1 dmg
- **Analysis**: Fast, fragile. Name matches stats perfectly.
- **Countered by**: Aroma Keeper (slow) or Bread Courier (high FR).

### Lazy Alley Cat (`enemy_cat`)
- **Stats**: 300 HP / 12 speed / 30 gold / 2 dmg / Heavy
- **Analysis**: "Lazy" explains very low speed. High HP tank. Missing FlavorText (still "TODO").
- **Countered by**: Dough Exorcist (Heavy bonus), Crust Crusader (crits), Bakery Truck splash.

### Pigeon with a Stolen Baguette (`enemy_pigeon`)
- **Stats**: 80 HP / 32 speed / 20 gold / 1 dmg
- **Analysis**: Conceptually a flyer but has no flight mechanic — follows normal path. Placeholder until Layer 2.
- **Countered by**: Slow + high FR.

### Microwave Meal Preacher (`enemy_preacher`)
- **Stats**: 200 HP / 20 speed / 25 gold / 2 dmg / Heavy
- **Analysis**: Solid tier2 enemy. "Converted" by the heresy.
- **Countered by**: Sustained damage, Dough Exorcist (Heavy bonus).

### Plastic Wrapped Sandwich Man (`enemy_sandwich`)
- **Stats**: 180 HP / 28 speed / 22 gold / 2 dmg
- **Analysis**: Name suggests protection/resistance but has NO `IsHeavy` tag. Consider adding `IsHeavy=true`.
- **Countered by**: Sustained damage.

### Frozen Dough Abomination (`enemy_abomination`)
- **Stats**: 400 HP / 10 speed (slowest) / 35 gold / 3 dmg / Heavy
- **Analysis**: Best name/stat match in the roster. "Frozen" = slow, "Abomination" = ugly tank.
- **Countered by**: Sustained damage, Dough Exorcist (Heavy bonus).

### Supermarket Overlord of White Bread (`enemy_overlord`)
- **Stats**: 900 HP / 18 speed / 80 gold / 4 dmg / Heavy (not Boss, used as miniboss)
- **Analysis**: Antithesis of sourdough. Elite before the final boss.
- **Countered by**: Same as Dragon.

### The Industrial Bread Dragon (`enemy_dragon`) — BOSS
- **Stats**: 1000 HP / 16 speed / 100 gold / 5 dmg / Boss + Heavy
- **Analysis**: Final boss. Name and function aligned.
- **Countered by**: Dough Exorcist (execute + Heavy bonus), Crust Judgment Protocol (crit <50% HP = kill), Taste Tester (sustained).

### The Gluten Null Bishop (`enemy_gluten_bishop`) — BOSS
- **Stats**: 1500 HP / 14 speed / 120 gold / 6 dmg / Boss + Heavy + **Anti-Buff Aura** (60px, -50% to all multiplicative % bonuses)
- **Analysis**: Best lore/mechanic match — "Gluten Null" opposes the Order, mechanically "nulls" buffs.
- **Countered by**: Crust Crusader / Crust Judgment Protocol (immune to debuff — crits not %-based), high base-damage towers.

## Open Questions
1. Pigeon: lore promises flyer but follows normal path — placeholder until Layer 2 (flying enemy targeting), already in ROADMAP.md.
2. Plastic Wrapped Sandwich Man: name implies resistance but has no `IsHeavy = true`. Consider adding.
3. Lazy Alley Cat: missing FlavorText (still "TODO") — needs a line reinforcing "fat lazy cat" to justify high HP.
