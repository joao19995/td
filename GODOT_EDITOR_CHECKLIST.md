# Godot Editor Checklist — Phase 2 Component Extraction

Everything below must be verified or configured inside the **Godot 4 Editor**
after opening the project. The C# code is ready; these are the wiring steps
the editor must confirm or set up.

---

## 1. Verify Scene Trees

### Enemy.tscn  (`scenes/enemies/Enemy.tscn`)
Open the scene and confirm the node tree looks exactly like this:

```
Enemy  (Area2D)  ← Enemy.cs
├── Sprite2D
├── CollisionShape2D
├── Health         (Node)  ← Health.cs
└── MovementComponent  (Node)  ← MovementComponent.cs  ✅ NEW
```

**If MovementComponent is missing:**
1. Select the root `Enemy` node.
2. Click **Add Child Node** → choose `Node`.
3. Rename it `MovementComponent` (exact spelling, case-sensitive).
4. In the Inspector, assign the script `res://scripts/components/MovementComponent.cs`.

---

### Tower.tscn  (`scenes/towers/Tower.tscn`)
Open the scene and confirm the node tree looks exactly like this:

```
Tower  (Node2D)  ← Tower.cs
├── Sprite2D
├── DetectionArea  (Area2D)
│   └── CollisionShape2D
├── TargetingComponent  (Node)  ← TargetingComponent.cs  ✅ NEW
└── AttackComponent     (Node)  ← AttackComponent.cs     ✅ NEW
```

**If TargetingComponent is missing:**
1. Select the root `Tower` node.
2. Click **Add Child Node** → choose `Node` → rename to `TargetingComponent`.
3. Assign script `res://scripts/components/TargetingComponent.cs`.

**If AttackComponent is missing:**
1. Select the root `Tower` node.
2. Click **Add Child Node** → choose `Node` → rename to `AttackComponent`.
3. Assign script `res://scripts/components/AttackComponent.cs`.

---

## 2. Configure TargetingComponent Properties

With `Tower.tscn` open, select the `TargetingComponent` node and check
the **Inspector**:

| Property | Required Value | Notes |
|---|---|---|
| `Detection Area Path` | `../DetectionArea` | NodePath to the Area2D that tracks enemies |
| `Strategy` | `First` (default) | Change to `Closest`, `Strongest`, or `Last` per tower type |

> **Tip:** Set different Strategy values on different Tower scenes to create
> varied gameplay without writing any new code.

---

## 3. Confirm Collision Layers on DetectionArea

The `DetectionArea` inside Tower must overlap with Enemy collision layers:

1. Select `DetectionArea` inside Tower.tscn.
2. In the **Inspector** → **Collision** section:
   - `Collision Layer`: should be `0` (the tower doesn't collide itself).
   - `Collision Mask`: must include the **layer that Enemies are on**.
3. Select the root `Enemy` node in Enemy.tscn.
   - `Collision Layer`: confirm it is on the layer that Tower's mask covers.
4. Adjust layers/masks as needed so `AreaEntered` fires correctly.

---

## 4. Build the C# Solution

After any change to `.cs` files, rebuild from the editor:

- **Project → Tools → C# → Build Project**  (or press `Alt+B`)
- Confirm **0 errors** in the Build output panel.

If the build fails, check:
- Class names match file names exactly (e.g., `AttackComponent.cs` → `public partial class AttackComponent`).
- All files are saved.

---

## 5. Run the Game and Smoke-Test

1. Press **F5** (Run Project).
2. Place a tower and start a wave.
3. Verify:
   - [ ] Enemies spawn and follow the path (MovementComponent working).
   - [ ] Tower rotates/targets an enemy (TargetingComponent working).
   - [ ] Projectiles fire and hit enemies (AttackComponent working).
   - [ ] Enemy dies when health reaches 0 (Health + reward gold still working).
   - [ ] Enemy damages the player when reaching the end (ReachedEnd event still working).

---

## 6. Optional — Per-Tower Strategy Overrides

To create different tower types without new code:

1. Duplicate `Tower.tscn` → e.g., `SniperTower.tscn`.
2. Select `TargetingComponent` → set `Strategy = Strongest`.
3. Select `TowerData` resource → lower `FireRate`, raise `Damage`, raise `Range`.
4. Done — a sniper tower exists with zero new C# files.

---

## 7. Test Scenes (Recommended)

Create one small test scene per component under `scenes/tests/`:

| Scene | What to test |
|---|---|
| `movement_test.tscn` | Enemy follows path at different speeds |
| `targeting_test.tscn` | All four strategy modes select the right enemy |
| `attack_test.tscn` | Correct fire rate, projectile hit and damage |
