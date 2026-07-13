# Playtests

## When to record a playtest
- After significant balance changes.
- When testing a new tower, enemy, equipment, trinket, or synergy.
- When the run structure changes (slot machine weights, wave generation, economy).
- When a player reports something feels wrong and you want to reproduce it.

## How to record a playtest

1. **Play the run(s).** The game automatically writes analytics JSON to
   `user://analytics/{date}/` at the end of every run.

2. **Write your subjective notes** as bullet points covering:
   - What felt good / bad
   - Bugs encountered (with reproduction steps)
   - Balance observations
   - Strategy rationale (why this loadout, why these upgrades)
   - Ideas that emerged
   - Decisions made

3. **Invoke `@playtest`** with:
   - Which JSON files to analyse (e.g., "latest 3 runs", or specific file paths)
   - Your subjective notes

   The agent merges analytics data with your notes and writes the structured
   report following `PLAYTEST_TEMPLATE.md`.

## Agent sections vs Player sections

The template marks sections as `<!-- AGENT -->` (auto-generated from data)
or `<!-- PLAYER -->` (requires human input). The `@playtest` agent handles
all AGENT sections automatically. You only need to provide the PLAYER sections.

## Naming
`YYYY-MM-DD — Run X — Brief Description.md`

For multiple runs: `YYYY-MM-DD — Runs X–Y — Brief Description.md`

Example: `2026-07-12 — Run 3 — Testing Crust Crusader crit build.md`
Example: `2026-07-13 — Runs 1–5 — Baseline data collection.md`

## Observation vs Decision
Observations are what happened. Decisions are what to do about it. Do not conflate them.
- "Aroma Keeper killed 3 enemies in 60s" — observation.
- "Aroma Keeper needs +10% damage" — potential decision, needs broader context.

## Templates
Use `PLAYTEST_TEMPLATE.md` for structured sessions.
The `@playtest` agent automatically follows this template.
