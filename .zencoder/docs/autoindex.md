---
description: NecroHAMSTO Turn-Based System - Autoindex of Third-Party Assets & Integration Points
lastUpdated: 2025-01-20
---

# Gorgonzola Autoindex: MoreMountains/TopDownEngine Integration Map

**Project**: NecroHAMSTO vs Cute Snakes (Turn-Based Survivors-io)  
**Framework**: TopDown Engine v4.4 + MoreMountains Ecosystem  
**Philosophy**: Leverage existing MoreMountains systems; only extend timing/turn logic.

---

## Part 1: MoreMountains Ecosystem Overview

### 1.1 MMTools (Foundation Library)
**Path**: `Assets/TopDownEngine/ThirdParty/MoreMountains/MMTools/`

| System | Purpose | Status | Integration |
|--------|---------|--------|-------------|
| **Core** | Singleton pattern, Pooling, Events | ✅ Use | No changes needed |
| **Foundation** | Utilities, Math, Collections | ✅ Use | No changes needed |
| **Accessories** | Feedback, Settings, Debugging | ✅ Use | Optional for polish |

**Key Classes**:
- `MMSingleton<T>` – Inherit for global managers (we're already using this for `TurnManager`)
- `MMObjectPooler` – Object recycling (enemies, projectiles, UI)
- `MMEventManager` – Event bus for decoupled communication

---

### 1.2 MMFeedbacks (Juice System)
**Path**: `Assets/TopDownEngine/ThirdParty/MoreMountains/MMFeedbacks/`

| System | Purpose | Status | Integration |
|--------|---------|--------|-------------|
| **MMFeedbacks** | Feedback controller (screenshake, CameraShake, sound triggers, particles, etc.) | ✅ Use | Trigger on turn events |
| **MMFeedbacksForThirdParty** | Integrations with external packages | ✅ Use | If needed |

**Key Classes**:
- `MMF_Player` – Main feedback orchestrator
- `MMF_ScreenShake`, `MMF_SoundFX` – Individual feedback types

**Turn Integration**: Create MMFeedbacks on:
- Turn phase transitions (`OnPhaseChanged`)
- Unit actions (attacks, abilities)
- Status effect application

---

### 1.3 MMInterface (UI Framework)
**Path**: `Assets/TopDownEngine/ThirdParty/MoreMountains/MMInterface/`

| System | Purpose | Status | Integration |
|--------|---------|--------|-------------|
| **Common** | UI base classes, buttons, panels, HUD | ✅ Use | UI prototyping |
| **Styles** | Theming system | ✅ Use | Optional |

**Key Classes**:
- `MMButton` – Base button with callbacks
- `MMPanelManager` – Panel transitions and lifecycle

**Turn Integration**: 
- Display turn indicator (who's active)
- Highlight valid action zones during PlayerTurn phase
- Update HUD (health, status effects) on turn transitions

---

### 1.4 InventoryEngine (Item/Equip System)
**Path**: `Assets/TopDownEngine/ThirdParty/MoreMountains/InventoryEngine/`

| System | Purpose | Status | Integration |
|--------|---------|--------|-------------|
| **InventoryEngine** | Full inventory, items, equipment, loot | ✅ Use | Equip phylactery, minion items |

**Key Classes**:
- `InventoryEngine` – Main inventory manager
- `InventoryItem` – Base item class
- `InventoryEquipment` – Equipment slots (helm, armor, etc.)

**Turn Integration**:
- Phylactery as special equipment slot
- Equipment stat bonuses affect turn-based stats (e.g., armor → damage reduction per turn)
- Minion summoning items

---

### 1.5 TopDownEngine Character System
**Path**: `Assets/TopDownEngine/Common/Scripts/Characters/`

| Subsystem | Purpose | Status | Integration |
|-----------|---------|--------|-------------|
| **Core** | Character, CharacterMovement, CharacterHealth | ✅ Use | Base for all entities |
| **Health** | Health component, damage handling | ✅ Use | Turn-based damage resolution |
| **Damage** | DamageOnTouch, DamageArea | ✅ Use | Phylactery damage on touch |
| **Weapons** | Weapon, Projectile, WeaponModules | ✅ Use | Turn-based attacks |
| **AI** | AIDecisionTree, PatrolState, ChasseState | ⚠️ Extend | Hook into TurnManager phases |
| **Animations** | CharacterAnimationManager | ✅ Use | Turn action animations |
| **CharacterAbilities** | Abilities system | ✅ Use | Special minion/player abilities |

**Key Integration Points**:
```csharp
// Characters inherit from Character (TopDownEngine)
// Add IEnemyBrain or IMinionAI interface
// Register with TurnManager in OnEnable()
```

---

## Part 2: Turn-Based Integration Architecture

### 2.1 Custom Interfaces (Gorgonzola-Specific)
**Location**: `Assets/Scripts/_Core/` or `Assets/Scripts/Systems/`

```csharp
// Entities implementing these register with TurnManager during their phase
public interface IEnemyBrain
{
    void OnEnemyTurnStart();
    void OnEnemyTurnEnd();
    void ExecuteAction();  // Called by TurnManager during EnemyTurn phase
}

public interface IMinionAI
{
    void OnMinionTurnStart();
    void OnMinionTurnEnd();
    void ExecuteAction();  // Called by TurnManager during MinionTurn phase
}

// Player character doesn't need AI—just listens to input
```

### 2.2 TurnManager Integration Points
**Location**: `Assets/Scripts/Systems/TurnManager.cs` (Already scaffolded)

| Phase | Description | Integration |
|-------|-------------|-------------|
| **PlayerTurn** | Player input (move, attack, ability) | Listen to `OnPhaseChanged` in UI/input handler |
| **EnemyTurn** | All enemies execute actions sequentially | Loop through `registeredEnemies`, call `ExecuteAction()` |
| **MinionTurn** | All minions execute actions | Loop through `registeredMinions`, call `ExecuteAction()` |
| **Resolution** | Apply damage, resolve status effects, check win/loss | Trigger damage resolution, phylactery checks |
| **Idle** | Waiting for next turn or game over | Show "Turn X" banner, prepare UI for next cycle |

---

### 2.3 Entity Prefab Structure

#### Player Prefab: `Assets/Prefabs/NecroHAMSTO.prefab`
```
NecroHAMSTO (GameObject)
├── Character (TopDownEngine)
│   ├── Health component
│   ├── Weapon system
│   └── CharacterAnimationManager
├── PlayerController (Custom)
│   ├── Listens to TurnManager.OnPhaseChanged
│   └── Enables input only during PlayerTurn phase
└── UIDisplay (Custom)
    └── Shows HP, status effects, phylactery glow
```

#### Enemy Prefab: `Assets/Prefabs/CuteSnake.prefab`
```
CuteSnake (GameObject)
├── Character (TopDownEngine)
│   ├── Health component
│   ├── Weapon system (bite/spit)
│   └── CharacterAnimationManager
├── SnakeAI (Custom, implements IEnemyBrain)
│   ├── OnEnemyTurnStart() → Plan action
│   └── ExecuteAction() → Move/Attack
└── Registerer
    └── Calls TurnManager.RegisterEnemy() on OnEnable()
```

#### Minion Prefab: `Assets/Prefabs/Minion.prefab`
```
Minion (GameObject)
├── Character (TopDownEngine)
│   ├── Health component
│   ├── Weapon system
│   └── CharacterAnimationManager
├── MinionAI (Custom, implements IMinionAI)
│   ├── OnMinionTurnStart() → Evaluate targets
│   └── ExecuteAction() → Attack/Support
└── Registerer
    └── Calls TurnManager.RegisterMinion() on OnEnable()
```

---

### 2.4 Damage Resolution Pipeline

**Turn-Based Damage Flow**:

1. **Action Phase** (PlayerTurn/EnemyTurn/MinionTurn)
   - Entity decides action (move to tile, target enemy)
   - Weapon/ability determines damage value
   - **NO damage applied yet** – queue the action

2. **Resolution Phase**
   - Apply all queued damage at once
   - Trigger MMFeedbacks (screenshake, blood particle, SFX)
   - Update Health component via `ReceiveDamage()`
   - Check if Health ≤ 0 → trigger death animation + removal

3. **Idle Phase**
   - Update UI (HP bars, status effects)
   - Prepare for next turn

**Key**: Leverage `TopDownEngine.Character.ReceiveDamage()` but call it only during Resolution phase, not during entity actions.

---

## Part 3: What We DON'T Change

### DO NOT Modify:
- ❌ TopDownEngine character movement (use as-is)
- ❌ MoreMountains MMTools/MMFeedbacks/MMInterface (use as-is)
- ❌ InventoryEngine (use as-is)
- ❌ Health/Damage resolution inside `Character.ReceiveDamage()` (call it, don't rewrite it)

### DO Extend:
- ✅ AI behavior trees → Wrap actions with `IEnemyBrain.ExecuteAction()` 
- ✅ Input handling → Gate during `PlayerTurn` phase
- ✅ Animation triggers → Hook `OnPhaseChanged` events
- ✅ UI updates → Subscribe to `TurnManager.OnPhaseChanged`

---

## Part 4: File Structure & Namespaces

### Core Namespaces (Gorgonzola):
```
Gorgonzola.Core              → GameJamBootstrap, TurnManager
Gorgonzola.Systems          → Managers, Orchestrators
Gorgonzola.Entities.Player  → NecroHAMSTO logic
Gorgonzola.Entities.Enemies → Snake, other enemy implementations
Gorgonzola.Entities.Minions → Summon/minion logic
Gorgonzola.UI               → HUD, phase indicators, feedback
Gorgonzola.Services         → Audio, visual effects, utilities
```

### MoreMountains Namespaces (Reference):
```
MoreMountains.Tools         → Singleton, Pooling, Events
MoreMountains.Feedbacks     → MMF_Player, feedback types
MoreMountains.MMInterface   → UI base classes
MoreMountains.InventoryEngine → Item/Equipment management
MoreMountains.TopDownEngine → Character, Health, Weapons, AI
```

---

## Part 5: Dependency Graph

```
TurnManager (Singleton, orchestrates phases)
    ↓
    ├─→ Registered Enemies (implement IEnemyBrain)
    │   └─→ TopDownEngine.Character (health, damage)
    │
    ├─→ Registered Minions (implement IMinionAI)
    │   └─→ TopDownEngine.Character (health, damage)
    │
    ├─→ Player (PlayerController)
    │   └─→ TopDownEngine.Character (health, damage)
    │
    ├─→ UI Listeners (subscribe to OnPhaseChanged)
    │   └─→ MMInterface (buttons, panels)
    │
    └─→ Feedback Triggers (subscribe to OnPhaseChanged)
        └─→ MMFeedbacks (screenshake, SFX, particles)

Phylactery (Special GameObject)
    ├─→ TopDownEngine.DamageArea (take damage)
    └─→ InventoryEngine (equip bonuses)

Inventory (Singleton)
    └─→ InventoryEngine (items, equipment, minion summoning)
```

---

## Part 6: Quick Reference: What Each MoreMountains System Provides

| System | Provides | Don't Reinvent |
|--------|----------|----------------|
| **MMTools** | Singleton, events, pooling | Use `MMSingleton<T>`, `MMEventManager` |
| **MMFeedbacks** | Screenshake, SFX, particles | Call `MMF_Player.Play()` on turn events |
| **MMInterface** | Buttons, panels, HUD layout | Inherit `MMButton`, `MMPanel` for UI |
| **InventoryEngine** | Items, equipment, inventory UI | Use `InventoryEngine` for phylactery stats |
| **TopDownEngine.Character** | Movement, health, weapons, AI hooks | Inherit or compose with custom `IEnemyBrain` |
| **TopDownEngine.Health** | Damage, death callbacks, buffs | Call `ReceiveDamage()`, not custom health logic |
| **TopDownEngine.Weapons** | Projectiles, melee, targeting | Use weapon system; turn-gate the execution |
| **TopDownEngine.AI** | AI trees, states, decisions | Hook into state machines with TurnManager phases |

---

## Part 7: Integration Checklist

### Phase 1: Character Setup ✅
- [x] TurnManager scaffolded
- [ ] Create NecroHAMSTO prefab (inherit `TopDownEngine.Character`)
- [ ] Create CuteSnake prefab (inherit `TopDownEngine.Character` + implement `IEnemyBrain`)
- [ ] Create Minion prefab (inherit `TopDownEngine.Character` + implement `IMinionAI`)

### Phase 2: Input & UI ⏳
- [ ] PlayerController script (gates input during `PlayerTurn`)
- [ ] Phase indicator UI (show "Enemy Turn" etc.)
- [ ] Action validation (can player move to tile? can attack target?)

### Phase 3: Damage & Resolution ⏳
- [ ] Action queueing system (collect all actions, apply in Resolution)
- [ ] Damage resolution (call `Character.ReceiveDamage()` in bulk)
- [ ] Feedback triggers (MMFeedbacks on damage)
- [ ] Death handling (remove from turn queue, trigger death anim)

### Phase 4: Feedback & Polish ⏳
- [ ] MMFeedbacks integration (screenshake, blood, SFX)
- [ ] Animation callbacks (trigger on action, damage, death)
- [ ] UI polish (health bars, status effects, turn banner)

---

## Recommended Reading

1. **TopDownEngine Documentation**: Check `Assets/TopDownEngine/IMPORTANT-HOW-TO-INSTALL.txt`
2. **MoreMountains.Tools**: Explore `MMSingleton<T>` pattern and `MMEventManager`
3. **InventoryEngine**: Study demos in `Assets/TopDownEngine/ThirdParty/MoreMountains/InventoryEngine/Demos/`
4. **Your Custom Code**: 
   - `Assets/Scripts/_Core/TurnManager.cs` – Already scaffolded, ready to extend
   - `Assets/Scripts/_Core/GameJamBootstrap.cs` – Entry point, minimal changes needed

---

## Summary

**The Philosophy**: We are not reimplementing a game engine. We are orchestrating MoreMountains' existing systems with a turn-based phase manager (`TurnManager`). 

- **MoreMountains provides**: Characters, health, damage, weapons, AI trees, inventory, UI, feedback.
- **We add**: Turn phases, action queueing, phase-gated input, turn-based damage resolution.

Every feature should answer: **"Is this already in TopDownEngine/MoreMountains?"** If yes, use it. If no, build it lightly.

---

**Last Updated**: 2025-01-20  
**Next Steps**: Create entity prefabs (Player, Enemy, Minion) and integrate with TurnManager.