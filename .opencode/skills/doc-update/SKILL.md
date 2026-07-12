---
name: doc-update
description: Updates project documentation (docs/status/GAME_STATUS.md, ROADMAP.md) after completing a feature. Loads when the task mentions docs, documentation, game status, or roadmap.
---

# Documentation Update — Sourdough Siege

## Files you may update

### `docs/status/docs/status/GAME_STATUS.md`
When: observable game behavior changed (new mechanic, screen, tower, enemy,
equip, trinket, synergy, outcome). Keep existing structure and terminology.
Minimal changesets only — do not rewrite the entire file.

### `ROADMAP.md`
When: an item was completed or changed state. Do not invent new roadmap items
from implementation alone. If no clear match exists, do not change it.

## Files requiring explicit user instruction

`CLAUDE.md`, `CLAUDE_ADDENDUM.md`, `AGENTS.md`, `README.md`.

## Never touch

`.cs`, `.tres`, `.tscn`, config files, or any file outside docs/ and project root.
