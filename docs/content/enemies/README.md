# Enemies

> TTK (time-to-kill) is a qualitative reference against a single Bread Baker (35 DPS base / 84 DPS T2).
> Each enemy has dedicated lore, stats, and strategy info in its own file.

## Progression

Enemies evolve across tiers as the game progresses:

- **Tier 1 — Innocents**: Tourist, Jogger, Pigeon, Alley Cat. Ordinary victims of convenience.
- **Tier 2 — Followers**: Sandwich Man, Microwave Preacher, Frozen Dough. Fully influenced by industrial baking.
- **Tier 3 — Leaders**: Supermarket Overlord. The corporate face of the heresy.
- **Tier 4 — Legends**: Industrial Bread Dragon and Gluten Null Bishop. Mythic embodiments of greed and convenience.

## Overview

| Enemy | HP | Speed | Gold | Dmg | Type | Tier | File |
|-------|----|-------|------|-----|------|------|------|
| Sliced Bread Tourist | 50 | 24 | 3 | 1 | Basic | 1 — Innocent | [📄](sliced-bread-tourist.md) |
| Grocery Run Jogger | 35 | 48 | 5 | 1 | Fast, fragile | 1 — Innocent | [📄](grocery-run-jogger.md) |
| Lazy Alley Cat | 150 | 12 | 10 | 2 | Heavy — early tank | 1 — Innocent | [📄](lazy-alley-cat.md) |
| Pigeon with a Stolen Baguette | 40 | 32 | 6 | 1 | Light, fast | 1 — Innocent | [📄](pigeon-with-a-stolen-baguette.md) |
| Microwave Meal Preacher | 200 | 20 | 25 | 2 | Heavy — medium convert | 2 — Follower | [📄](microwave-meal-preacher.md) |
| Plastic Wrapped Sandwich Man | 180 | 28 | 22 | 2 | Medium resist | 2 — Follower | [📄](plastic-wrapped-sandwich-man.md) |
| Frozen Dough Abomination | 400 | 10 | 35 | 3 | Heavy — slow tank | 2 — Follower | [📄](frozen-dough-abomination.md) |
| Supermarket Overlord of White Bread | 900 | 18 | 80 | 4 | Heavy — elite/miniboss | 3 — Leader | [📄](supermarket-overlord.md) |
| The Industrial Bread Dragon | 1000 | 16 | 100 | 5 | Boss + Heavy | 4 — Legend | [📄](industrial-bread-dragon.md) |
| The Gluten Null Bishop | 1500 | 14 | 120 | 6 | Boss + Heavy + Anti-Buff Aura | 4 — Legend | [📄](gluten-null-bishop.md) |

## Quick Links

| Enemy | File |
|-------|------|
| Sliced Bread Tourist | [`content/enemies/sliced-bread-tourist.md`](sliced-bread-tourist.md) |
| Grocery Run Jogger | [`content/enemies/grocery-run-jogger.md`](grocery-run-jogger.md) |
| Lazy Alley Cat | [`content/enemies/lazy-alley-cat.md`](lazy-alley-cat.md) |
| Pigeon with a Stolen Baguette | [`content/enemies/pigeon-with-a-stolen-baguette.md`](pigeon-with-a-stolen-baguette.md) |
| Microwave Meal Preacher | [`content/enemies/microwave-meal-preacher.md`](microwave-meal-preacher.md) |
| Plastic Wrapped Sandwich Man | [`content/enemies/plastic-wrapped-sandwich-man.md`](plastic-wrapped-sandwich-man.md) |
| Frozen Dough Abomination | [`content/enemies/frozen-dough-abomination.md`](frozen-dough-abomination.md) |
| Supermarket Overlord of White Bread | [`content/enemies/supermarket-overlord.md`](supermarket-overlord.md) |
| The Industrial Bread Dragon | [`content/enemies/industrial-bread-dragon.md`](industrial-bread-dragon.md) |
| The Gluten Null Bishop | [`content/enemies/gluten-null-bishop.md`](gluten-null-bishop.md) |

## Open Questions

1. Pigeon: lore promises flyer but follows normal path — placeholder until Layer 2 (flying enemy targeting), already in ROADMAP.md.
2. Lazy Alley Cat: missing FlavorText — needs a line reinforcing "fat lazy cat" to justify high HP.
3. Plastic Wrapped Sandwich Man: now has `IsHeavy = true` (added V0). May need HP/speed adjustment after data collection.
