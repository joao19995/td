<#
.SYNOPSIS
    Validação estática rápida contra as regras não-negociáveis do
    CLAUDE.md / CLAUDE_ADDENDUM.md, seguida de build da solução.
    Corre ANTES de qualquer commit ou de dar uma tarefa como terminada.

.USAGE
    ./pre-build.ps1 -Build                           # valida tudo + compila
    ./pre-build.ps1 -Build -Path "scripts/towers"    # valida só essa pasta + compila

.EXIT CODES
    0 = sem violações + build passou
    1 = violações encontradas
    2 = build falhou (violações passaram mas compilação falhou)
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

if (-not $csFiles) {
    Write-Host "Nenhum ficheiro .cs encontrado em '$Path'." -ForegroundColor Yellow
    exit 0
}

foreach ($file in $csFiles) {
    $lines = Get-Content $file.FullName
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $line = $lines[$i]
        $lineNum = $i + 1

        # Regra #6 — GetNode<T> chamado noutro objecto, não implicit this.
        # Ignora GetNode<> sem prefixo (nó a aceder aos próprios filhos).
        $hasObjRef = $line -match '\w+\s*\.\s*Get(NodeOrNull|Node)<\s*\w+\s*>\s*\('
        $hasGetParent = $line -match 'GetParent\s*\(\s*\)\s*\.\s*Get(NodeOrNull|Node)<\s*\w+\s*>\s*\('
        if ($hasObjRef -or $hasGetParent) {
            Add-Violation -File $file.FullName -Line $lineNum -Rule "R6-DeepNodePath" -Snippet $line
        }

        # Regra #1 — números de gameplay hardcoded em condições/atribuições de dano/hp/custo.
        # Heurística: literais float/int a multiplicar 'damage', 'health', 'hp', 'cost'
        # que NÃO leem de _data./ _equipData./ RunState./ GameBalance.
        if ($line -match '(damage|health|hp|cost)\s*(\*=|\+=|=)\s*[0-9]+(\.[0-9]+)?f?\b' -and
            $line -notmatch '_data\.|_equipData\.|RunState\.|GameBalance\.|Data\.') {
            Add-Violation -File $file.FullName -Line $lineNum -Rule "R1-HardcodedValue" -Snippet $line
        }

        # Regra #4 — arrays hardcoded de nomes de ficheiros .tres em vez de directory scan.
        if ($line -match 'new\s*\[\]\s*\{.*\.tres') {
            Add-Violation -File $file.FullName -Line $lineNum -Rule "R4-HardcodedFileList" -Snippet $line
        }

        # Regra #8 — mutação directa de campo em Resource injectado sem Duplicate().
        if ($line -match '(_data|_equipData|enemyData|towerData)\.\w+\s*(\*=|\+=|-=)') {
            Add-Violation -File $file.FullName -Line $lineNum -Rule "R8-SharedResourceMutation" -Snippet $line
        }

        # Regra #2/#3 — subclasse de Enemy/Tower detectada.
        if ($line -match 'class\s+\w+\s*:\s*(Enemy|Tower)\b') {
            Add-Violation -File $file.FullName -Line $lineNum -Rule "R2-CopyPasteSubclass" -Snippet $line
        }
    }
}

if ($violations.Count -eq 0) {
    Write-Host "OK - nenhuma violaçao encontrada em '$Path'." -ForegroundColor Green
}
else {
    Write-Host "`nVIOLAÇÕES ENCONTRADAS ($($violations.Count)):`n" -ForegroundColor Red
    foreach ($v in $violations) {
        Write-Host "[$($v.Rule)] $($v.File):$($v.Line)" -ForegroundColor Yellow
        Write-Host "    $($v.Code)`n"
    }
    Write-Host "Revê cada linha acima contra CLAUDE_ADDENDUM.md antes de continuar." -ForegroundColor Red
}

$buildFailed = $false
if ($Build) {
    Write-Host "A compilar td.sln..." -ForegroundColor Cyan
    $buildResult = dotnet build td.sln --nologo 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Build OK." -ForegroundColor Green
    }
    else {
        Write-Host "`nBUILD FALHOU:`n" -ForegroundColor Red
        Write-Host ($buildResult -join "`n")
        $buildFailed = $true
    }
}

if ($violations.Count -gt 0) { exit 1 }
if ($buildFailed) { exit 2 }
exit 0
