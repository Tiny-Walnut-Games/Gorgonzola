# 🐰 Ability-Gating Pattern: Turn-Based Control Flow

**Date Anchored**: Today  
**Doctrine**: Respect TopDown Engine's ability-based architecture  
**Pattern Owner**: PlayerInputGate → Generalizable to EnemyInputGate, MinionInputGate, etc.

---

## **Core Insight**

TopDown Engine is **ability-stacked**, not monolithic:

```
Player GameObject
├─ CharacterMovement    [ability component]
├─ CharacterOrientation3D
├─ CharacterRun
├─ CharacterHandleWeapon
├─ Health
└─ (other systems)
```

Each ability is an **independent `enabled` property**. Gating should happen at the **ability layer**, not the input layer.

---

## **Pattern: Ability Layer Gating**

### ❌ **Anti-Pattern (Input Layer)**
```csharp
// Trying to gate at INPUT level:
inputManager.enabled = (phase == PlayerTurn);

// Problem: Abilities might trigger from other sources, momentum, or cached state
```

### ✅ **Canonical Pattern (Ability Layer)**
```csharp
// Gate at ABILITY level:
private void HandlePhaseChange(TurnManager.TurnPhase newPhase)
{
    bool isActive = (newPhase == YourPhase);
    
    characterMovement.enabled = isActive;
    characterOrientation.enabled = isActive;
    characterRun.enabled = isActive;
    characterWeapon.enabled = isActive;
}
```

**Benefit**: Abilities genuinely **cannot execute** outside their active phase, regardless of input.

---

## **Implementation Template: Any Actor (Player/Enemy/Minion)**

```csharp
using UnityEngine;
using MoreMountains.TopDownEngine;

public class ActorTurnGate : MonoBehaviour
{
    // === CACHED ABILITY COMPONENTS ===
    private CharacterMovement characterMovement;
    private CharacterOrientation3D characterOrientation;
    private CharacterRun characterRun;
    private CharacterHandleWeapon characterWeapon;
    
    // === CONFIGURATION ===
    public TurnManager.TurnPhase ActivationPhase = TurnManager.TurnPhase.PlayerTurn;
    
    private TurnManager turnManager;

    private void Start()
    {
        // Cache all ability components
        characterMovement = GetComponent<CharacterMovement>();
        characterOrientation = GetComponent<CharacterOrientation3D>();
        characterRun = GetComponent<CharacterRun>();
        characterWeapon = GetComponent<CharacterHandleWeapon>();

        turnManager = TurnManager.Instance;
        if (turnManager == null) return;

        // Subscribe to phase changes
        turnManager.OnPhaseChanged += HandlePhaseChange;
        
        // Start disabled
        DisableAllAbilities();
    }

    private void OnDestroy()
    {
        if (turnManager != null)
            turnManager.OnPhaseChanged -= HandlePhaseChange;
    }

    private void HandlePhaseChange(TurnManager.TurnPhase newPhase)
    {
        if (newPhase == ActivationPhase)
            EnableAllAbilities();
        else
            DisableAllAbilities();
    }

    private void EnableAllAbilities()
    {
        if (characterMovement != null) characterMovement.enabled = true;
        if (characterOrientation != null) characterOrientation.enabled = true;
        if (characterRun != null) characterRun.enabled = true;
        if (characterWeapon != null) characterWeapon.enabled = true;
    }

    private void DisableAllAbilities()
    {
        if (characterMovement != null) characterMovement.enabled = false;
        if (characterOrientation != null) characterOrientation.enabled = false;
        if (characterRun != null) characterRun.enabled = false;
        if (characterWeapon != null) characterWeapon.enabled = false;
    }
}
```

---

## **Application Roadmap**

- ✅ **PlayerInputGate**: Gate player to `PlayerTurn` phase
- 🔄 **EnemyTurnGate**: Gate enemies to `EnemyTurn` phase (use template above, set `ActivationPhase = EnemyTurn`)
- 🔄 **MinionTurnGate**: Gate minions/ghosts to `MinionTurn` phase
- 🔄 **GeneralActorGate**: Rename to `ActorTurnGate` and make fully generic

---

## **Signal: Why This Matters**

Attempting to gate input creates **false illusions of control**:
- Player *thinks* they can't move
- But underlying systems still respond to stale input
- Creates bugs like: delayed movement, ghost inputs, confusion

**Ability gating is honest**:
- Abilities genuinely disabled
- No surprises
- Clean, testable, maintainable

---

## **Lineage: This Pattern Evolves**

As we add more actors and systems:
1. Extract common pattern to base class → `ActorTurnGate`
2. Extend to AI brains → `AIBrainTurnGate` (disable behavior trees during inactive phases)
3. Extend to projectiles → `ProjectileBrainTurnGate` (freeze mid-flight during non-movement phases)
4. Extend to effects → `EffectBrainTurnGate` (pause animations during inactive phases)

**Single doctrine: Gate abilities, not inputs.**

---

## **Debugging Aid: Phase Transition Log**

PlayerInputGate now logs ability cache status at startup:
```
[PlayerInputGate] ✨ ABILITY GATING INITIALIZED
[PlayerInputGate] Cached abilities:
  - CharacterMovement: ✓
  - CharacterOrientation3D: ✓
  - CharacterRun: ✓
  - CharacterHandleWeapon: ✓
[PlayerInputGate] 🟢 ABILITIES ENABLED (PlayerTurn phase active)
[PlayerInputGate] 🔴 ABILITIES DISABLED (EnemyTurn phase active)
```

**If any show ✗**: Component missing on that actor. Check prefab/scene setup.

---

**SeedRabbit's Blessing**: This pattern honors TopDown Engine's design philosophy while providing the discrete turn control your game needs. 🐰✨