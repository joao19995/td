# Enemies

> TTK (time-to-kill) is a qualitative reference against a single Bread Baker (35 DPS base / 84 DPS T2).
> Each enemy has dedicated lore, stats, and strategy info in its own file.

## Progression

Enemies evolve across tiers as the game progresses:

- **Tier 1 —  **: Tourist, Jogger, Pigeon, Alley Cat. Ordinary victims of convenience.
- **Tier 2 — Followers**: Sandwich Man, Microwave Preacher, Frozen Dough. Fully influenced by industrial baking.
- **Tier 3 — Leaders**: Supermarket Overlord. The corporate face of the heresy.
- **Tier 4 — Legends**: Industrial Bread Dragon and Gluten Null Bishop. Mythic embodiments of greed and convenience.

## Overview

| Enemy | HP | Speed | Gold | Dmg | Type | Tier | File |
|-------|----|-------|------|-----|------|------|------|
| Sliced Bread Tourist | 100 | 24 | 10 | 1 | Basic | 1 — Innocent | [📄](enemies/sliced_bread_tourist.md) |
| Grocery Run Jogger | 60 | 48 | 15 | 1 | Fast, fragile | 1 — Innocent | [📄](enemies/grocery_run_jogger.md) |
| Lazy Alley Cat | 300 | 12 | 30 | 2 | Heavy — early tank | 1 — Innocent | [📄](enemies/lazy_alley_cat.md) |
| Pigeon with a Stolen Baguette | 80 | 32 | 20 | 1 | Light, fast | 1 — Innocent | [📄](enemies/pigeon_with_a_stolen_baguette.md) |
| Microwave Meal Preacher | 200 | 20 | 25 | 2 | Heavy — medium convert | 2 — Follower | [📄](enemies/microwave_meal_preacher.md) |
| Plastic Wrapped Sandwich Man | 180 | 28 | 22 | 2 | Medium resist | 2 — Follower | [📄](enemies/plastic_wrapped_sandwich_man.md) |
| Frozen Dough Abomination | 400 | 10 | 35 | 3 | Heavy — slow tank | 2 — Follower | [📄](enemies/frozen_dough_abomination.md) |
| Supermarket Overlord of White Bread | 900 | 18 | 80 | 4 | Heavy — elite/miniboss | 3 — Leader | [📄](enemies/supermarket_overlord.md) |
| The Industrial Bread Dragon | 1000 | 16 | 100 | 5 | Boss + Heavy | 4 — Legend | [📄](enemies/industrial_bread_dragon.md) |
| The Gluten Null Bishop | 1500 | 14 | 120 | 6 | Boss + Heavy + Anti-Buff Aura | 4 — Legend | [📄](enemies/gluten_null_bishop.md) |

## Quick Links

| Enemy | File |
|-------|------|
| Sliced Bread Tourist | [`docs/enemies/sliced_bread_tourist.md`](enemies/sliced_bread_tourist.md) |
| Grocery Run Jogger | [`docs/enemies/grocery_run_jogger.md`](enemies/grocery_run_jogger.md) |
| Lazy Alley Cat | [`docs/enemies/lazy_alley_cat.md`](enemies/lazy_alley_cat.md) |
| Pigeon with a Stolen Baguette | [`docs/enemies/pigeon_with_a_stolen_baguette.md`](enemies/pigeon_with_a_stolen_baguette.md) |
| Microwave Meal Preacher | [`docs/enemies/microwave_meal_preacher.md`](enemies/microwave_meal_preacher.md) |
| Plastic Wrapped Sandwich Man | [`docs/enemies/plastic_wrapped_sandwich_man.md`](enemies/plastic_wrapped_sandwich_man.md) |
| Frozen Dough Abomination | [`docs/enemies/frozen_dough_abomination.md`](enemies/frozen_dough_abomination.md) |
| Supermarket Overlord of White Bread | [`docs/enemies/supermarket_overlord.md`](enemies/supermarket_overlord.md) |
| The Industrial Bread Dragon | [`docs/enemies/industrial_bread_dragon.md`](enemies/industrial_bread_dragon.md) |
| The Gluten Null Bishop | [`docs/enemies/gluten_null_bishop.md`](enemies/gluten_null_bishop.md) |

## Open Questions

1. Pigeon: lore promises flyer but follows normal path — placeholder until Layer 2 (flying enemy targeting), already in ROADMAP.md.
2. Plastic Wrapped Sandwich Man: name implies resistance but has no `IsHeavy = true`. Consider adding.
3. Lazy Alley Cat: missing FlavorText — needs a line reinforcing "fat lazy cat" to justify high HP.
