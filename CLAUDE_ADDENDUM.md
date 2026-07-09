# CLAUDE_ADDENDUM.md — Padrões Certo vs Errado

Motivo deste ficheiro: "No deep node paths" e regras similares no CLAUDE.md
são compreendidas em abstracto mas ignoradas na prática porque não há um exemplo
literal ao lado. Cada regra abaixo tem um exemplo tirado do próprio código do
projecto (não inventado) para eliminar ambiguidade.

---

## Regra #6 — No Deep Node Paths / No God-Node Lookups

**Errado** (procura um componente irmão pelo nome do nó, à mão, fora do dono):

```csharp
// Dentro de TargetingComponent, à procura de progresso de outro nó
var movement = enemy.GetNodeOrNull<MovementComponent>("MovementComponent");
float progress = movement != null ? movement.GetProgressRatio() : 0f;
```

**Certo** (o dono do componente expõe um método público; quem precisa, chama-o):

```csharp
// Enemy.cs expõe o que os outros sistemas precisam
public float GetCurrentHealth() => _health?.GetCurrentHealth() ?? 0f;

// TargetingComponent.cs usa a API pública, nunca sabe do path interno
float health = enemy.GetCurrentHealth();
```

Nota: `TargetingComponent.GetFurthestEnemy()` no código actual FAZ
`enemy.GetNodeOrNull<MovementComponent>("MovementComponent")` — isto é uma
violação existente da Regra #6, não um exemplo a copiar. Se tocares nesse
método, corrige-o para `enemy.GetProgressRatio()` (adicionar o método a
`Enemy.cs` se ainda não existir).

---

## Regra #1 — No Hardcoded Gameplay Values

**Errado**:

```csharp
if (target.HealthPercent <= 0.2f)
    damage *= 2.0f;
```

**Certo**:

```csharp
if (target.HealthPercent <= _data.ExecuteThresholdHPPercent)
    damage *= _data.ExecuteMultiplier;
```

Teste rápido: se apagares o `.tres` e o número ainda aparece nalgum `.cs`,
está errado.

---

## Regra #2/#3 — No Copy-Paste Systems / Composition over Inheritance

**Errado**:

```csharp
public partial class BossEnemy : Enemy { /* overrides */ }
```

**Certo**: novo `EnemyData.tres` com `IsBoss = true`, mesma `Enemy.tscn`
genérica. Comportamento diferente vem de flags no Resource + componentes,
nunca de uma subclasse nova.

---

## Regra #8 — Sub-resources partilhados (mutação perigosa)

**Errado** (muta o Resource partilhado por referência — afecta TODOS os
inimigos que usam o mesmo `EnemyData`):

```csharp
enemyData.MaxHealth *= 1.5f; // NUNCA
```

**Certo** (como o próprio `AttackComponent.RebuildEffects()` já faz — cria
uma instância nova por invocação):

```csharp
effects.Add((mainEnemy, _) => ApplyEffect(mainEnemy, new PoisonEffectData
{
    Duration = _data.PoisonDuration * _poisonDurationMultiplier,
    DamagePerTick = _data.PoisonDamagePerTick * strengthMult,
}));
```

Para `Shape`/`CircleShape2D` partilhados em `.tscn`, `.Duplicate()` antes de
mutar (ver `Tower.ApplyData`).

---

## Regra #4 — Zero-Code-Change Content Addition

**Errado**: array hardcoded de ficheiros a carregar.

```csharp
var files = new[] { "Wave1.tres", "Wave2.tres", "Wave3.tres" };
```

**Certo** (como `SynergyManager.LoadSynergyDefinitions()` e
`ResourceLoaderHelper.LoadFromDir<T>()` já fazem):

```csharp
var dir = DirAccess.Open(SynergyDir);
foreach (var file in dir.GetFiles())
{
    if (!file.EndsWith(".tres") && !file.EndsWith(".res")) continue;
    // carregar
}
```

---

## Regra #7 — Autoloads são infraestrutura, não gameplay

**Errado**: adicionar `public int TowerDamage` ou lógica de dano a um
autoload como `GameManager`.

**Certo**: gameplay state vive num componente da entidade (`Health`,
`AttackComponent`, etc.). Autoloads só coordenam (`EventBus`, `SceneManager`,
`PoolManager`).

---

## Checklist de auto-verificação antes de dar código como terminado

1. Procurei por `GetNode<` / `GetNodeOrNull<` fora do `_Ready()` do próprio
   dono do nó? Se sim, devia ser um método público no dono.
2. Escrevi algum número (`float`/`int`) directamente num `.cs` que afecta
   gameplay (dano, HP, custo, duração, multiplicador)? Se sim, deve vir de
   um `[Export]` num Resource.
3. Criei uma subclasse nova de `Enemy`/`Tower` só para variar comportamento?
   Se sim, devia ser um `.tres` novo.
4. Mutei directamente um campo de um Resource injectado (`TowerData`,
   `EnemyData`, `StatusEffectData`, `Shape`) sem `.Duplicate()` ou sem criar
   instância nova? Se sim, corrige antes de continuar.
5. Este ficheiro/lógica passaria bem com 20 towers e 30 enemies sem eu
   precisar de tocar em código C#? Se não, não é data-driven.
