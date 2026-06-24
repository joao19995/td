# Godot Editor Checklist — Tower Defense Project

Everything below must be verified or configured inside the **Godot 4 Editor**
after opening the project.

---

## 1. Autoloads — Confirm All Singletons Are Registered

Open **Project → Project Settings → Autoloads** and confirm every singleton is present in this order:

| Name | Path |
|---|---|
| `EconomyManager` | `res://scripts/autoload/EconomyManager.cs` |
| `EventBus` | `res://scripts/autoload/EventBus.cs` |
| `GameManager` | `res://scripts/autoload/GameManager.cs` |
| `SceneManager` | `res://scripts/autoload/SceneManager.cs` |
| `TowerPlacementManager` | `res://scripts/autoload/TowerPlacementManager.cs` |

**How to add a missing autoload:**
1. Click the folder icon and navigate to the script.
2. Set the node name exactly as shown above.
3. Click **Add**.

---

## 2. Build the C# Solution

After any `.cs` file change, rebuild from the editor:

- **Project → Tools → C# → Build Project** (or press `Alt+B`)
- Confirm **0 errors** in the Build output panel.

---

## 3. HUD — Confirm All 5 Tower Buttons Are Wired

Open `scenes/ui/HUD.tscn`. Select the **HUD** root node and look at the **Inspector**.

The `AvailableTowers` array must contain **all 5** tower resources in order:

| Index | Resource | Path |
|---|---|---|
| 0 | Base Tower | `res://resources/tower_data/Base.tres` |
| 1 | Fast Tower | `res://resources/tower_data/Fast.tres` |
| 2 | Ice Tower | `res://resources/tower_data/Ice.tres` |
| 3 | Poison Tower | `res://resources/tower_data/Poison.tres` |
| 4 | Splash Tower | `res://resources/tower_data/Splash.tres` |

> The HUD script reads `AvailableTowers` on `_Ready()` and creates one button per
> entry. The first entry reuses the static `TowerButton` node; the rest are
> created dynamically and appended to the same `TopBar` container.

If any resource is missing, drag it from the **FileSystem** panel into the
`AvailableTowers` array slot.

---

## 4. Tower Data Resources — Configure Stats

Each `.tres` file in `res://resources/tower_data/` exposes the following fields
in the Inspector. Open each file and verify (or set) the values:

| Field | Type | Description |
|---|---|---|
| `TowerName` | String | Displayed on the HUD button |
| `Damage` | Float | Damage dealt per hit |
| `FireRate` | Float | Attacks per second |
| `Range` | Float | Detection radius in pixels |
| `Cost` | Int | Gold cost — shown on the HUD button |
| `ProjectileScene` | PackedScene | Scene fired by this tower |
| `Sprite` | Texture2D | Tower sprite shown in the game |

Suggested starting values:

| Tower | TowerName | Damage | FireRate | Range | Cost |
|---|---|---|---|---|---|
| Base | Base | 10 | 1.0 | 120 | 50 |
| Fast | Fast | 5 | 3.0 | 100 | 75 |
| Ice | Ice | 8 | 1.2 | 110 | 80 |
| Poison | Poison | 3 | 2.0 | 130 | 90 |
| Splash | Splash | 15 | 0.8 | 90 | 120 |

---

## 5. Verify Main.tscn Wiring

Open `scenes/main/Main.tscn`. Confirm the tree looks like this:

```
Main  (Node2D)  ← Main.cs
└── HUD  (CanvasLayer)  ← HUD.cs
```

- `Main` must have `scripts/main/Main.cs` attached.
- `HUD` must have `scripts/ui/HUD.cs` attached.
- There must be **no** hard-coded Map scene in the tree — the level is loaded at
  runtime by `SceneManager`.

---

## 6. Verify Map1.tscn Is Not Pre-Instantiated in Main.tscn

`SceneManager` loads the level at runtime, so `Map1.tscn` must **not** be a
pre-instantiated child of `Main.tscn`. If it is:

1. Select the `Map1` node inside `Main.tscn`.
2. Press **Delete**.
3. Save the scene.

---

## 7. Confirm Existing Scenes Are Intact

### Enemy.tscn (`scenes/enemies/Enemy.tscn`)

```
Enemy  (Area2D)  ← Enemy.cs
├── Sprite2D
├── CollisionShape2D
├── Health               (Node)  ← Health.cs
└── MovementComponent    (Node)  ← MovementComponent.cs
```

### Tower.tscn (`scenes/towers/Tower.tscn`)

```
Tower  (Node2D)  ← Tower.cs
├── Sprite2D
├── DetectionArea  (Area2D)
│   └── CollisionShape2D
├── TargetingComponent   (Node)  ← TargetingComponent.cs
└── AttackComponent      (Node)  ← AttackComponent.cs
```

---

## 8. Smoke-Test Checklist

Press **F5** and verify:

- [ ] Level loads automatically (no manual scene drag needed).
- [ ] **All 5 tower buttons** appear in the HUD top bar with correct names and costs.
- [ ] Each tower button places a tower with the correct sprite and stats.
- [ ] Enemies spawn and follow the path.
- [ ] Projectiles fire, hit enemies, and deal damage.
- [ ] Enemy rewards gold on death.
- [ ] Lives decrease when an enemy reaches the end.
- [ ] Pressing **N** or the **Next Wave** button starts the next wave.
- [ ] All tower buttons are disabled when Game Over occurs.

---

## 9. Factory Call Chain (Reference)

```
EnemySpawner.SpawnEnemy()
  └─ EnemyFactory.Create(GenericEnemyScene, enemyData, path)

TowerPlacementManager.ConfirmPlacement()
  └─ TowerFactory.Create(GenericTowerScene, towerData, position)

AttackComponent.Fire()
  └─ ProjectileFactory.Create(projectileScene, damage, target, origin)
```

---

## 10. Test Scenes (Recommended)

Create a scene per system under `scenes/tests/` to isolate and test each part
independently:

| Scene | What to test |
|---|---|
| `enemy_factory_test.tscn` | Enemy spawns with correct stats and follows path |
| `tower_factory_test.tscn` | Each of the 5 towers is placed with correct data |
| `projectile_factory_test.tscn` | Projectile fires with correct damage and target |
| `scene_manager_test.tscn` | Level loads and unloads cleanly |
| `hud_test.tscn` | All 5 tower buttons render correctly and respond to clicks |
