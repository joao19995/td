# AGENTS.md — Instruções Operacionais Obrigatórias

Este ficheiro complementa `CLAUDE.md` e `CLAUDE_ADDENDUM.md`. Enquanto esses
dois descrevem a arquitectura e as regras, este ficheiro descreve o processo
passo-a-passo obrigatório que qualquer agente (Claude, Copilot, opencode, etc.)
deve seguir neste repositório. Não é opcional e não é "boa prática sugerida" —
é um checklist de execução.

---

## Antes de escrever qualquer código

1. Relê `CLAUDE.md` (secção "Architecture — The Non-Negotiables") e
   `CLAUDE_ADDENDUM.md` (exemplos certo/errado). Não assumas que já sabes
   as regras de memória — relê-as nesta sessão, mesmo que já as tenhas
   seguido antes.
2. Identifica explicitamente, antes de tocar em código, quais das 8 regras
   do `CLAUDE.md` são relevantes para a tarefa pedida. Escreve essa lista
   (mentalmente ou como resposta ao utilizador) antes de implementar.
3. Se a tarefa envolve adicionar conteúdo (tower, enemy, equip, trinket,
   synergy, wave, meta-upgrade): a resposta por defeito é "isto é um `.tres`
   novo", não "isto precisa de uma classe nova". Só te desvias disto com
   justificação concreta (Regra do "concrete necessity test").

---

## Depois de escrever código

1. Corre `./pre-build.ps1 -Build` sobre os ficheiros `.cs` que tocaste.
   Isto valida as 5 regras não-negociáveis E compila `td.sln`. Zero
   violações + build OK antes de continuares.
   - Para validar só uma pasta: `./pre-build.ps1 -Build -Path "scripts/towers"`
   - Exit 0 = tudo OK, exit 1 = violações, exit 2 = erro de compilação
2. Se o script não existir no teu ambiente actual, faz grep manual das tuas
   próprias alterações contra os 6 padrões proibidos:

   - `GetNode<T>("...")` / `GetNodeOrNull<T>("...")` fora do dono do nó
   - literais numéricos de gameplay fora de um Resource `[Export]`
   - `class X : Enemy` ou `class X : Tower`
   - mutação directa de campos de `TowerData`/`EnemyData`/`EquipData`/
     `StatusEffectData` sem `.Duplicate()` ou instância nova
   - arrays hardcoded de nomes de ficheiro `.tres` em vez de directory scan
   - `GetTree().CurrentScene` em vez de
     `LevelManager.Instance.CurrentLevelNode` / `BaseLevel.*Container`

3. Se encontrares uma violação introduzida por ti: corrige antes de
   reportar a tarefa como terminada. Não deixes para "depois" nem menciones
   como "known issue" a não ser que já fosse uma violação pré-existente que
   não fazia parte do âmbito do pedido.
4. Actualiza `docs/GAME_STATUS.md` se a tarefa mudou comportamento do jogo
   observável (novo mecanismo, novo ecrã, novo outcome do slot machine).
5. Actualiza `ROADMAP.md` se completaste um item da lista.

---

## Quando não tens a certeza

1. Aplica o "concrete necessity test" do `CLAUDE.md`: não proponhas nem
   apliques uma mudança de arquitectura sem um bug, duplicação, ou parede
   de escala real e demonstrável.
2. Se a dúvida for sobre uma regra específica (ex: "isto conta como deep
   node path?"), pergunta explicitamente ao utilizador em vez de assumir
   silenciosamente uma interpretação permissiva.

---

## Formato de resposta esperado ao terminar uma tarefa

Inclui sempre, no fim da resposta:

1. Que regras do `CLAUDE.md` eram relevantes para esta tarefa.
2. Confirmação de que correste a verificação (script ou grep manual) e o
   resultado (0 violações, ou violações encontradas e corrigidas).
3. Se alguma violação pré-existente foi tocada mas não corrigida (fora do
   âmbito do pedido), diz isso explicitamente — não o escondas.

---

## Porque este ficheiro existe

O `CLAUDE.md` descreve regras correctamente, mas de forma abstracta
("No deep node paths"). Sem exemplos concretos e sem um passo de
verificação obrigatório depois de escrever código, é fácil para um agente
concordar com a regra em teoria e ainda assim violá-la na prática (como
aconteceu com `TargetingComponent.GetFurthestEnemy()`, que faz exactamente
o `GetNodeOrNull<MovementComponent>("MovementComponent")` que a Regra #6
proíbe). Este ficheiro fecha esse gap ao tornar a verificação um passo
explícito do processo, não uma intenção implícita.
