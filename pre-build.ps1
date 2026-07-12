<#
.SYNOPSIS
    Static validation against the non-negotiable rules of
    CLAUDE.md / CLAUDE_ADDENDUM.md (R1-R10), followed by solution build.
    Run BEFORE any commit or before marking a task as done.

.DESCRIPTION
    This is the OFFICIAL quality gate for the project.
    .opencode/rules.json is editor feedback only (NOT supported as enforcement).

.USAGE
    ./pre-build.ps1 -Build                           # validate everything + compile
    ./pre-build.ps1 -Build -Path "scripts/towers"    # validate only that folder + compile

.EXIT CODES
    0 = no violations + build passed
    1 = violations found
    2 = build failed (violations passed but compilation failed)

.COVERAGE
    Honest coverage — partial means the regex catches common patterns but
    cannot guarantee full compliance:

    R1  — partial automation + reviewer (heuristic for obvious hardcoded values)
    R2  — partial automation + reviewer (detects subclassing, not copy-paste)
    R3  — reviewer only (semantic duplication requires human judgment)
    R4  — partial automation + reviewer (detects hardcoded arrays, not all discovery patterns)
    R5  — reviewer only (requires understanding of "high-frequency" semantics)
    R6  — partial automation + reviewer (detects external GetNode<> but not all god-object patterns)
    R7  — reviewer only (requires understanding of coordination vs gameplay logic)
    R8  — partial automation + reviewer (detects mutation of known Resource fields)
    R9  — reviewer only (requires understanding of global vs local communication intent)
    R10 — partial automation + reviewer (detects GetTree().CurrentScene for spawning)
#>

param(
    [string]$Path = "scripts",
    [switch]$Build
)

$ErrorActionPreference = "Stop"
$violations = @()

function Add-Violation {
    param($File, $Line, $Rule, $Snippet)
    $script:violations += [PSCustomObject]@{
        File   = $File
        Line   = $Line
        Rule   = $Rule
        Code   = $Snippet.Trim()
    }
}

$csFiles = Get-ChildItem -Path $Path -Recurse -Filter *.cs -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -notmatch '\.godot' }

if ($csFiles) {
    foreach ($file in $csFiles) {
        $lines = Get-Content $file.FullName
        for ($i = 0; $i -lt $lines.Count; $i++) {
            $line = $lines[$i]
            $lineNum = $i + 1

            # --- R6 — No Deep Node Paths (partial) ---
            # Detects: anyObj.GetNode<T>("path") — accessing another object's children.
            # Does NOT detect: complex chaining, indirect access, or cases
            # where a node legitimately accesses its own component children.
            # Does NOT flag: plain GetNode<T>("path") without a prefix (self-access).
            $hasObjRef = $line -match '\w+\s*\.\s*Get(NodeOrNull|Node)<\s*\w+\s*>\s*\('
            $hasGetParent = $line -match 'GetParent\s*\(\s*\)\s*\.\s*Get(NodeOrNull|Node)<\s*\w+\s*>\s*\('
            if ($hasObjRef -or $hasGetParent) {
                Add-Violation -File $file.FullName -Line $lineNum -Rule "R6-DeepNodePath" -Snippet $line
            }

            # --- R1 — Data-Driven (partial) ---
            # Detects: literal numbers assigned to gameplay-sounding variable names
            # (damage, health, hp, cost) when not reading from a known data source.
            # Does NOT detect: hardcoded values assigned to differently-named variables,
            # magic numbers in method arguments, constants used for non-gameplay purposes.
            # Known false positives: [Export] defaults (inspector-overridable), zero
            # initialization, math constants like 1f in neutral multipliers.
            if ($line -match '(damage|health|hp|cost)\s*(\*=|\+=|=)\s*[0-9]+(\.[0-9]+)?f?\b' -and
                $line -notmatch '_data\.|_equipData\.|RunState\.|GameBalance\.|Data\.' -and
                $line -notmatch '\[Export\]' -and
                $line -notmatch '=\s*0[fF]?\s*[;,)]') {
                Add-Violation -File $file.FullName -Line $lineNum -Rule "R1-HardcodedValue" -Snippet $line
            }

            # --- R4 — Zero-Code-Change Content (partial) ---
            # Detects: new[] { "....tres", ... } patterns — hardcoded file lists.
            # Does NOT detect: List<string>.Add(), arrays built procedurally,
            # single-resource loads, or other non-array patterns.
            if ($line -match 'new\s*\[\]\s*\{.*\.tres') {
                Add-Violation -File $file.FullName -Line $lineNum -Rule "R4-HardcodedFileList" -Snippet $line
            }

            # --- R8 — Never Mutate Shared Resources (partial) ---
            # Detects: _data.X *=, enemyData.X +=, _equipData.X -=, towerData.X *= etc.
            # on known Resource-carrying field names.
            # Does NOT detect: mutations via method calls, mutations on variables
            # with different names, reassignment to the field itself.
            if ($line -match '(_data|_equipData|enemyData|towerData)\.\w+\s*(\*=|\+=|-=)') {
                Add-Violation -File $file.FullName -Line $lineNum -Rule "R8-SharedResourceMutation" -Snippet $line
            }

            # --- R2 — Composition Over Inheritance (partial) ---
            # Detects: class Foo : Enemy or class Foo : Tower.
            # Does NOT detect: copy-paste (R3), abstract base violations not
            # involving Enemy/Tower subclasses.
            if ($line -match 'class\s+\w+\s*:\s*(Enemy|Tower)\b') {
                Add-Violation -File $file.FullName -Line $lineNum -Rule "R2-CopyPasteSubclass" -Snippet $line
            }

            # --- R10 — Spawn Under Active Level Containers (partial) ---
            # Detects: GetTree().CurrentScene used for AddChild or similar.
            # Does NOT detect: spawning via indirect references, scene tree
            # manipulation through helper methods, or legitimate non-spawning uses.
            if ($line -match 'GetTree\(\)\s*\.\s*CurrentScene') {
                Add-Violation -File $file.FullName -Line $lineNum -Rule "R10-SuspectSpawnTarget" -Snippet $line
            }

            # --- Async guard (warning, not a numbered rule) ---
            # Detects: await ToSignal(...) without a nearby IsInstanceValid guard.
            # Heuristic only — does not check semantic correctness.
            if ($line -match 'await\s+ToSignal\(') {
                $hasGuard = $false
                for ($j = [Math]::Max(0, $i - 5); $j -le [Math]::Min($lines.Count - 1, $i + 5); $j++) {
                    if ($lines[$j] -match 'IsInstanceValid\(') { $hasGuard = $true; break }
                }
                if (-not $hasGuard) {
                    Add-Violation -File $file.FullName -Line $lineNum -Rule "WARN-AsyncGuard" -Snippet $line
                }
            }
        }
    }

    if ($violations.Count -eq 0) {
        Write-Host "OK - no violations found in '$Path'." -ForegroundColor Green
    }
    else {
        Write-Host "`nVIOLATIONS FOUND ($($violations.Count)):`n" -ForegroundColor Red
        foreach ($v in $violations) {
            $color = if ($v.Rule -like 'WARN-*') { 'Yellow' } else { 'Yellow' }
            Write-Host "[$($v.Rule)] $($v.File):$($v.Line)" -ForegroundColor $color
            Write-Host "    $($v.Code)`n"
        }
        Write-Host "Review each line above against CLAUDE_ADDENDUM.md before continuing." -ForegroundColor Red
    }
}
else {
    Write-Host "No .cs files found in '$Path' — skipping static validation." -ForegroundColor Yellow
}

# --- Build phase (always runs when -Build is set, regardless of validation results) ---
$buildFailed = $false
if ($Build) {
    Write-Host "Compiling td.sln..." -ForegroundColor Cyan
    $buildResult = dotnet build td.sln --nologo 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Build OK." -ForegroundColor Green
    }
    else {
        Write-Host "`nBUILD FAILED:`n" -ForegroundColor Red
        Write-Host ($buildResult -join "`n")
        $buildFailed = $true
    }
}

if ($violations.Count -gt 0) { exit 1 }
if ($buildFailed) { exit 2 }
exit 0
