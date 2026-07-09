---
name: doc-update
description: Actualiza documentação do projecto (GAME_STATUS.md, ROADMAP.md) após completar uma feature. Carrega quando a tarefa menciona docs, documentação, game status, ou roadmap.
---
# Actualização de Documentação — Sourdough Siege

## Que ficheiros actualizar

### `docs/GAME_STATUS.md`
**Quando**: SEMPRE que o comportamento observável do jogo muda.
- Novo mecanismo (ex: slot machine, synergies, bestiary)
- Novo ecrã (ex: BriefingScreen, MetaShopScreen)
- Nova feature de torre/enemy (ex: poison stacking, aura system)
- Novo outcome ou estado de jogo

**Formato**: Segue as secções existentes. Adiciona entrada na categoria certa.
NÃO reescreves o ficheiro inteiro — edita só a secção relevante.

### `ROADMAP.md`
**Quando**: Um item da checklist foi completado.
- Marca `[x]` no item completado.
- Se precisas de adicionar item novo, usa o mesmo formato `[ ]`.
- Se um item mudou de escopo, actualiza a descrição.

### `CLAUDE.md` / `CLAUDE_ADDENDUM.md`
**Quando**: SÓ com aprovação explícita do utilizador.
- Nova regra de arquitectura.
- Novo exemplo certo/errado para regra existente.

## O que NUNCA tocar
- `AGENTS.md` sem aprovação explícita
- `README.md` sem aprovação explícita
- Qualquer ficheiro fora de `docs/` ou raiz do projecto
