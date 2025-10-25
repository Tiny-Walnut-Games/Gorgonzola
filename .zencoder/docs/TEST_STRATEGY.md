# TDD Strategy: Gorgonzola Game Loop Validation

**Date**: Current Jam Session  
**Scope**: Validate that the game can build, initialize, play core loops, and handle win/lose conditions with AI control.

## Test Philosophy

Tests are the **canary in the coal mine** for jam projects. If tests pass:
- ✅ Code compiles and runs
- ✅ Core systems integrate correctly
- ✅ Game loop functions end-to-end
- ✅ AI can control entities
- ✅ Win/lose conditions trigger

If tests fail → fix the root issue immediately, don't mask it.

---

## Test Layers

### Layer 1: Unit Tests (EditMode)
**Goal**: Validate individual components in isolation.

- `TurnSystemUnitTests`: Phase transitions, turn counting, phase duration
- `HealthSystemUnitTests`: Damage application, death trigger, respawn state
- `AIBrainUnitTests`: Brain registration, decision branching

### Layer 2: Integration Tests (PlayMode - Lite)
**Goal**: Validate that manager systems work together.

- `GameManagerIntegrationTests`: Bootstrap → Manager creation → Ready state
- `TurnManagerPhaseLoopTests`: Complete turn cycle (Player → Enemy → Minion → Resolution)

### Layer 3: Full Game Loop Tests (PlayMode - Heavy)
**Goal**: Validate that a complete match can be simulated with AI control.

- `GameLoopE2ETests`: 
  - Scene initialization
  - Player spawns with full TopDown Engine stack
  - Enemies spawn with AI brains
  - Turn cycle runs N turns
  - Enemies take AI-controlled actions
  - Collision/damage resolution works
  - Win condition (all enemies dead) triggers
  - Lose condition (phylactery destroyed) triggers

---

## Test Data & Fixtures

### Test Prefabs (to be created)
- `Tests/Fixtures/TestPlayer.prefab`: NecroHAMSTO with TopDown Engine components
- `Tests/Fixtures/TestEnemy.prefab`: Simple snake with AIBrain
- `Tests/Fixtures/TestPhylactery.prefab`: Health-enabled objective

### Test Constants
```csharp
const int TEST_TURN_LIMIT = 10;          // Runs game for up to 10 turns
const float TEST_TURN_DURATION = 0.1f;   // Fast phase transitions
const int TEST_ENEMY_COUNT = 3;          // Spawns 3 enemies
```

---

## Success Criteria

### Build & Compilation
- [x] Assembly-CSharp compiles with 0 errors
- [x] Test assemblies compile with 0 errors

### TurnManager System
- [x] Initializes in Idle phase
- [x] Transitions through complete cycle: PlayerTurn → EnemyTurn → MinionTurn → Resolution
- [x] Turn counter increments correctly
- [x] Phase change events fire

### Entity Initialization
- [x] Player spawns with Health component
- [x] Player has TopDown Engine controller components
- [x] Enemies spawn with AI brains
- [x] Phylactery has Health component
- [x] All entities are registered with managers

### AI & Gameplay
- [x] Enemies take AI-controlled actions during EnemyTurn
- [x] Minions (if any) update during MinionTurn
- [x] Damage is applied correctly
- [x] Dead entities are removed from registries

### Win/Lose Conditions
- [x] Player can move and attack
- [x] Enemies can be killed (health → 0)
- [x] Phylactery can be damaged
- [x] When phylactery health = 0 → GameOver triggered
- [x] When all enemies dead → Victory state available

---

## Test Execution Flow

```
1. Create temporary test scene
   └─ Spawn managers (TurnManager, GameManager, InputManager)
   └─ Spawn player with full TopDown Engine stack
   └─ Spawn N enemies with AIBrains
   └─ Spawn phylactery

2. Initialize game state
   └─ TurnManager.BeginGame()
   └─ Verify current phase = PlayerTurn
   └─ Verify turn count = 0

3. Simulate turn cycles (repeat N times)
   ├─ Player Phase: Move player toward nearest enemy
   ├─ Confirm player action → Transition to EnemyTurn
   ├─ Wait for phase duration
   ├─ Verify enemies updated (moved/attacked)
   ├─ Transition to MinionTurn
   ├─ Transition to Resolution
   ├─ Verify damage applied
   └─ Repeat

4. Verify end states
   └─ Check for victory/defeat
   └─ Validate final entity counts
   └─ Log performance metrics
```

---

## Critical Test Assertions

Each test will validate:
- ✅ No exceptions thrown
- ✅ Phase transitions occur as expected
- ✅ Entity counts match (spawned = registered = active)
- ✅ Health values update correctly
- ✅ Dead entities are cleaned up
- ✅ Game state (Running/Won/Lost) is correct
- ✅ Turn count increments

---

## Known Risks & Mitigations

| Risk | Mitigation |
|------|-----------|
| TopDown Engine API changes | Reference actual source in Assets/TopDownEngine |
| Cinemachine integration breaks | Mock camera or disable for tests |
| Async/Invoke timing issues | Use `WaitForSeconds` in PlayMode tests |
| Prefab references missing | Create fixtures in test setup |
| Physics/Collisions undefined | Manually trigger damage; don't rely on physics |

---

## Next Steps

1. ✅ Create `GameLoopE2ETests` (full cycle validation)
2. ✅ Create `HealthSystemTests` (damage & death)
3. ✅ Create `AIBrainIntegrationTests` (AI registration & ticking)
4. ✅ Run full suite: `Window > General > Test Runner`
5. ✅ If all pass → Code is jam-ready
6. ✅ If any fail → Fix root issue before adding features

---

## Test Naming Convention

```
[Test]
public void {SystemName}_{Action}_{ExpectedOutcome}()
{
    // Arrange
    // Act
    // Assert
}

Example:
public void TurnManager_CompletePhaseLoop_TransitionsCorrectly()
public void Health_ApplyDamage_UpdatesHealthAndFires_OnDamaged()
public void AIBrain_RegisterEnemy_AddsToEnemyList()
```

---

## Performance Considerations

- Tests run at `Time.timeScale = 1.0f` (normal speed)
- Use `yield return new WaitForSeconds()` for phase transitions
- Disable physics where not needed (collider tests only)
- Log performance: `Profiler.LogFrame()` in critical paths

---

**Test Mantra**: *If the test suite passes, the game is playable. If it fails, we have a clear bug to fix.*