---
name: doc-update
description: Updates Sourdough Siege documentation after validated changes. Uses docs/README.md as the context router. Applies the change-type-to-docs decision matrix and a stale-detection step comparing doc snapshots to .tres resources. Loads when the task mentions docs, documentation, game status, roadmap, content, balance, or design.
---

# Documentation Update — Sourdough Siege

`.tres` files under `resources/` hold actual gameplay values. Documentation
describes intent, identity, and design — it does not compete with `.tres` as
the authoritative source of numeric values.

## Step 1 — Discover affected docs (router)

Read `docs/README.md` first. It maps each doc directory to its purpose and
tells you which docs *can* be affected. Do not load all documentation — load
only what the matrix below says is in scope.

## Step 2 — Map changed files to docs (decision matrix)

Minimal edits only — update only the docs the matrix lists for the change.

| Change | Docs affected |
|---|---|
| New tower `.tres`, or tower stats/levels/equipment changed | `docs/content/towers/README.md` (overview row; Synergies table **only if** a synergy changed), `docs/content/towers/{slug}.md` (Stats, Levels, Equipment tables), `docs/balance/TOWER_BASELINE.md` (FACTS, DERIVED METRICS, HYPOTHESES **only if** tower identity changed). `docs/status/GAME_STATUS.md` Towers + Special Mechanics lines **only if NEW tower**. |
| New enemy `.tres`, or enemy stats/flags changed | `docs/content/enemies/README.md` (overview row), `docs/content/enemies/{slug}.md` (Stats + Analysis), `docs/balance/ENEMY_BASELINE.md` (FACTS, DERIVED METRICS). `docs/status/GAME_STATUS.md` Enemies section **only if NEW enemy or new flag/mechanic**. |
| New/changed equipment `.tres` | `docs/content/towers/{target-tower}.md` (Equipment table), `docs/content/towers/README.md` (equipment listings + Special Mechanics if effect-type changed). `docs/balance/SHOP_BALANCING.md` **only if shop-purchasable**. `docs/status/GAME_STATUS.md` Tower Equipment count line **only if NEW equipment** (count 20 → 22 …). |
| New/changed trinket `.tres` | `docs/design/TRINKETS.md` (catalog table + Notes + Open Questions). `docs/status/GAME_STATUS.md` Trinkets section **only if NEW trinket**. |
| New/changed synergy `.tres` | `docs/content/towers/README.md` (Synergies table + per-synergy section). `docs/status/GAME_STATUS.md` Synergies section **only if NEW synergy**. |
| New/changed wave `.tres` | `docs/design/WAVES.md`. `docs/status/GAME_STATUS.md` Waves + Wave Decoupling + Wave Generation **only if structural generation changed** (counts, tiers, modifier system) — NOT for individual stat tweaks inside a wave. |
| New/changed shop item `.tres` / shop economy | `docs/balance/SHOP_BALANCING.md`. `docs/balance/ECONOMY_BASELINE.md` **if gold pacing model affected**. `docs/status/GAME_STATUS.md` Shop Items / Run Engine sections **only if weights, costs, or probabilities changed**. |
| SlotMachine (`SlotManager`) export change (weights/costs/skew/miniboss mult/heal amount/fights-per-run) | `docs/status/GAME_STATUS.md` Run Engine section. `docs/balance/ECONOMY_BASELINE.md` **if pacing affected**. |
| New/changed meta-upgrade `.tres` | `docs/design/META_PROGRESSION.md`. `docs/status/GAME_STATUS.md` Meta-Progression section **only if NEW upgrade or major change**. |
| New/changed `LevelData` / map `.tres` or `.tscn` | `docs/content/maps/README.md` (+ new map doc **if NEW map**). `docs/status/GAME_STATUS.md` Levels section + random map rotation **only if NEW map or topology change**. |
| New/changed `ActData` `.tres` (acts, fights-per-run, tier dirs) | `docs/status/GAME_STATUS.md` Run Engine + Wave Decoupling + Wave Generation sections. `docs/design/WAVES.md` if tier assignments changed. |
| `EffectiveDamage` / combat formula change in `.cs` | `docs/content/towers/README.md` (EffectiveDamage Formula section + Special Mechanics). `docs/status/GAME_STATUS.md` Towers + Special Mechanics sections. |
| Status effect behaviour change (poison/slow/anti-buff) in `.cs` or `.tres` | `docs/status/GAME_STATUS.md` Status Effects section. `docs/content/towers/README.md` Special Mechanics **if aura/chain/crit/execute/anti-buff affected**. |
| New screen / UI flow / scene `.tscn` | `docs/status/GAME_STATUS.md` relevant section (Game Loop, Screens & Menus, HUD). |
| Bestiary / discovery behaviour change | `docs/status/GAME_STATUS.md` Bestiary / Discovery section. |
| HUD / widget behaviour change | `docs/status/GAME_STATUS.md` HUD section. |
| New/changed `[Export]` config knob or `GameBalance` value | `docs/status/GAME_STATUS.md` Configurability section **only if it changes how values are tuned at runtime**. |
| ROADMAP item completed/changed/removed | `ROADMAP.md`. Do not invent new roadmap items from the implementation. |
| Lore / narrative added or changed | relevant `docs/lore/` doc **when it exists** + relevant `docs/content/{entity}.md` lore sections. |
| Pure code refactor with no behaviour or data change | NONE. |
| Bug fix changing observable described behaviour | `docs/status/GAME_STATUS.md` **only if it changes a described behaviour**. |
| Bug fix NOT changing observable behaviour | NONE. |

## Step 3 — Stale detection (numeric snapshots vs `.tres`)

For every queued doc that contains a stats / cost / levels / effect table
mirroring a `.tres`:

1. Open the corresponding `.tres` (or all matching `.tres` under the resource directory).
2. Compare each numeric value in the doc table to the `.tres` value.
3. If they differ, **update the doc to match the `.tres`**. NEVER update `.tres` to match docs.
4. If a value changed in this task, guarantee the doc table reflects the new `.tres` value.

**Source-of-truth rule for numbers:**
- `.tres` is the authority for all numeric gameplay values.
- Existing snapshot tables (e.g. `TOWER_BASELINE.md`, `ENEMY_BASELINE.md`, per-tower/enemy docs) are *snapshots*, kept in sync per this step; they do not override `.tres`.
- Do **NOT** introduce NEW numeric tables that duplicate `.tres` as a fresh source of authority — cross-reference the `.tres` path/name instead.

## Step 4 — Minimal-edit discipline

- Update only the matrix-listed docs.
- Make minimal changesets — no rewriting unrelated sections.
- Maintain existing structure, style, terminology.
- Do not "sync everything to be safe".
- If information is insufficient to document accurately, do not invent details; flag the gap in the final report.

## Step 5 — Final checklist (verify before terminating)

1. Did I read `docs/README.md` as the router first?
2. Did I map every changed file to a row of the decision matrix?
3. Did I skip every doc the matrix does NOT list for this change?
4. For each affected doc with a stat/cost/levels/effect table, did I compare it to the corresponding `.tres` and fix any drift?
5. Did I avoid introducing any NEW numeric duplicating table (preferring cross-references)?
6. Did I update `ROADMAP.md` only when an item was actually completed/changed/removed?
7. Did I respect the never-edit list (`CLAUDE.md`, `CLAUDE_ADDENDUM.md`, `AGENTS.md`, root `README.md` require explicit user instruction; `.cs`, `.tres`, `.tscn`, config files are never editable)?
8. Did I emit the mandatory response format below?

## Files requiring explicit user instruction

`CLAUDE.md`, `CLAUDE_ADDENDUM.md`, `AGENTS.md`, root `README.md`.

## Never touch

`.cs`, `.tres`, `.tscn`, configuration files, or any file outside `docs/` (and `ROADMAP.md` at project root).

## Mandatory response format

## Documentation
`UPDATED` or `NO_CHANGES_REQUIRED`

## Files changed
List of changed doc paths, or `None`.

## Summary
Short summary of the edits performed.

## Stale values detected and fixed
List of `doc path ← .tres path` corrections made, or `None`.

## Skipped
Docs considered but not changed, with reason (e.g. matrix said not-affected).
