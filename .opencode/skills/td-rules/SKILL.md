---
name: td-rules
description: Project context and tooling reference for Sourdough Siege (Godot 4.7 C#). Loads when the task involves writing or modifying .cs/.tres code in this repo.
---

# Sourdough Siege — Project Context

**Rules**: read `CLAUDE.md` (R1-R10) and `CLAUDE_ADDENDUM.md` (right/wrong examples).
They are the single source of truth. This skill does NOT duplicate them.

## Stack
Godot 4.7, .NET 8, GL Compatibility, 320x190, C# only (zero GDScript).

## Mandatory validation

```powershell
# Validate + compile (everything)
./pre-build.ps1 -Build

# Validate + compile (single folder)
./pre-build.ps1 -Build -Path "scripts/towers"
```

Exit codes: 0 = OK, 1 = violations, 2 = build failure.

`pre-build.ps1` automatically checks R1, R2, R4, R6, R8.
R3, R5, R7, R9, R10 require reviewer judgment.
