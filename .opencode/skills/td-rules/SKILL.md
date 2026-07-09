---
name: td-rules
description: Regras de arquitectura e verificação do projecto Sourdough Siege (Godot 4.7 C#). Carrega quando a tarefa envolve escrever ou alterar código .cs/.tres neste repo.
---
# Sourdough Siege — Regras Compactas

## As 8 Regras (CLAUDE.md)

| # | Regra | Verificação |
|---|-------|-------------|
| 1 | **Data-driven**: zero valores de gameplay em .cs | Grep por `float`/`int` com valor literal que afecta gameplay |
| 2 | **Composição**: nunca `class X : Enemy` ou `: Tower` | Grep por `: Enemy` ou `: Tower` |
| 3 | **Zero copy-paste**: abstrai duplicação | Revisão manual |
| 4 | **Zero-code-change content**: directory scan, não array hardcoded | Grep por arrays de strings com nomes de .tres |
| 6 | **No deep node paths**: `GetNode<T>()` só no dono do nó | Grep por `GetNode<` / `GetNodeOrNull<` fora de `_Ready` |
| 7 | **Autoloads = infraestrutura**: zero gameplay state em singletons | Verificar se novo campo em autoload é coordenação ou gameplay |
| 8 | **Sub-resource copy**: `.Duplicate()` antes de mutar Resource | Grep por atribuição directa a campo de Data Resource |

## Processo de verificação

```powershell
# Validar + compilar (tudo)
./pre-build.ps1 -Build

# Validar + compilar (só uma pasta)
./pre-build.ps1 -Build -Path "scripts/towers"
```

Exit codes: 0=OK, 1=violações, 2=erro de compilação

## Checklist pós-código
1. `GetNode<T>` / `GetNodeOrNull<T>` fora do dono? → método público no dono.
2. Número mágico em .cs? → `[Export]` no Resource.
3. Nova subclasse de Enemy/Tower? → `.tres` novo.
4. Mutação directa de Resource? → `.Duplicate()` ou instância nova.
5. Funciona com 20 towers + 30 enemies sem tocar em C#? → data-driven.

## Stack
Godot 4.7, .NET 8, GL Compatibility, 320×190, C# only (zero GDScript)
