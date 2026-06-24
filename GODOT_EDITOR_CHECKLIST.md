# Godot Editor Checklist — Phase 3 & 4

Everything below must be verified or configured inside the **Godot 4 Editor**
after opening the project. The C# code is ready; these are the wiring steps
the editor must confirm or set up.

---

## 1. Register the SceneManager Autoload

The `SceneManager` singleton must appear in **Project → Project Settings →
Autoloads** (in addition to the existing ones).

| Name | Path |
|---|---|
| `SceneManager` | `res://scripts/autoload/SceneManager.cs` |

**How to add it (if missing):**
1. Open **Project → Project Settings → Autoloads**.
2. Click the folder icon, navigate to `scripts/autoload/SceneManager.cs`.
3. Set the node name to `SceneManager`.
4. Click **Add**.
5. Confirm the order is: `EconomyManager`, `EventBus`, `GameManager`,
   `SceneManager`, `TowerPlacementManager`.

---

## 2. Build the C# Solution

After any change to `.cs` files, rebuild from the editor:

- **Project → Tools → C# → Build Project** (or press `Alt+B`)
- Confirm **0 errors** in the Build output panel.

New files introduced in this phase:

| File | Class |
|---|---|
| `scripts/factories/EnemyFactory.cs` | `EnemyFactory` (static) |
| `scripts/factories/TowerFactory.cs` | `TowerFactory` (static) |
| `scripts/factories/ProjectileFactory.cs` | `ProjectileFactory` (static) |
| `scripts/autoload/SceneManager.cs` | `SceneManager` |

---

## 3. Verify the Factory Call Chain

No scene wiring is required for the factories (they are static classes).
Open the scripts and confirm the call flow:

```
EnemySpawner.SpawnEnemy()
  └─ EnemyFactory.Create(GenericEnemyScene, enemyData, path)
       └─ returns Enemy → AddChild (deferred)

TowerPlacementManager.ConfirmPlacement()
  └─ TowerFactory.Create(GenericTowerScene, towerData, position)
       └─ returns Tower → AddChild

AttackComponent.Fire()
  └─ ProjectileFactory.Create(projectileScene, damage, target, origin)
       └─ returns Projectile → AddChild (deferred)
```

---

## 4. Verify Main.tscn Wiring

Open `scenes/main/Main.tscn`. Confirm:

- `Main` root node has the script `res://scripts/main/Main.cs` attached.
- `HUD` (CanvasLayer) is a direct child of `Main`.
- There is **no** hard-coded Map scene in the tree — `Main.cs` now loads it
  via `SceneManager.LoadLevel()` at runtime.

```
Main  (Node2D)  ← Main.cs
└── HUD  (CanvasLayer)  ← HUD.cs
```

---

## 5. Verify Map1.tscn is NOT in Main.tscn

Because `SceneManager` loads the level at runtime, `Map1.tscn` must **not**
be a pre-instantiated child of `Main.tscn`. If it is:

1. Select the `Map1` node inside `Main.tscn`.
2. Press **Delete** to remove it.
3. Save the scene.

---

## 6. Verify Map1.tscn Has No Input Handler

`Map1.cs` no longer contains `_UnhandledInput`. The `test_next_wave` keyboard
shortcut (default: **N**) is now handled by `HUD.cs`.

1. Open `scenes/levels/Map1.tscn`.
2. Confirm the root script is `res://scripts/levels/Map1.cs`.
3. No action required — the script is now an empty shell.

---

## 7. Confirm Existing Scenes Are Intact

### Enemy.tscn (`scenes/enemies/Enemy.tscn`)

```
Enemy  (Area2D)  ← Enemy.cs
├── Sprite2D
├── CollisionShape2D
├── Health         (Node)  ← Health.cs
└── MovementComponent  (Node)  ← MovementComponent.cs
```

### Tower.tscn (`scenes/towers/Tower.tscn`)

```
Tower  (Node2D)  ← Tower.cs
├── Sprite2D
├── DetectionArea  (Area2D)
│   └── CollisionShape2D
├── TargetingComponent  (Node)  ← TargetingComponent.cs
└── AttackComponent     (Node)  ← AttackComponent.cs
```

---

## 8. Run the Game and Smoke-Test

1. Press **F5** (Run Project).
2. Place a tower (click the tower button in the HUD) and start a wave.
3. Verify:
   - [ ] Level loads automatically via `SceneManager` (no manual scene in Main).
   - [ ] Enemies spawn and follow the path (`EnemyFactory` working).
   - [ ] Tower is placed at the correct grid position (`TowerFactory` working).
   - [ ] Projectiles fire and hit enemies (`ProjectileFactory` working).
   - [ ] Enemy dies when health reaches 0 (Health + reward gold still working).
   - [ ] Enemy damages the player when reaching the end.
   - [ ] Pressing **N** starts the next wave (keyboard shortcut now in HUD).
   - [ ] Pressing the **Next Wave** button still works.

---

## 9. Future: Adding Object Pooling

When pooling is introduced, **only the three factory classes change** —
`EnemyFactory`, `TowerFactory`, and `ProjectileFactory`. No other file needs
to know about pooling. This is the main architectural benefit of the factory
layer.

---

## 10. Test Scenes (Recommended)

Create one small test scene per factory/system under `scenes/tests/`:

| Scene | What to test |
|---|---|
| `enemy_factory_test.tscn` | Enemy spawns with correct stats and follows path |
| `tower_factory_test.tscn` | Tower placed with correct data injected |
| `projectile_factory_test.tscn` | Projectile fires with correct damage and target |
| `scene_manager_test.tscn` | Level loads/unloads cleanly on demand |
