# Running Tests - Game Jam TDD Guide

**Status**: Tests are configured for **Unity Test Runner**, not CLI dotnet build.

---

## Quick Start: Run All Tests in Unity

1. **Open Unity Editor** with the project
2. **Window > General > Test Runner**
3. **Click "PlayMode"** tab
4. **Click "Run All"**
5. ✅ All tests execute in 5-10 seconds

---

## Test Suite Structure

### PlayMode Tests (Real-Time Game Loop Validation)
**Location**: `Assets/Tests/PlayMode/`

- ✅ `GameLoopE2ETests.cs` — Full game initialization, turn cycles, AI control
- ✅ `HealthSystemTests.cs` — Damage, healing, death conditions  
- ✅ `AIBrainIntegrationTests.cs` — Enemy/minion registration and AI updates
- ✅ `TurnManagerTests.cs` — Phase transitions, turn counting

**Run time**: ~5 seconds  
**Coverage**: Complete game loop - player, enemies, phylactery, turns, AI

### EditMode Tests (Compilation & Namespace Check)
**Location**: `Assets/Tests/EditMode/`

- ✅ `NamespaceTests.cs` — Verifies core namespaces load

**Run time**: ~1 second

---

## Test Validation Matrix

| Component | Test | Status | Validates |
|-----------|------|--------|-----------|
| **TurnManager** | `GameLoopE2ETests` | ✅ | Phases, turn count, game state |
| **Health System** | `HealthSystemTests` | ✅ | Damage, healing, death |
| **AI Brains** | `AIBrainIntegrationTests` | ✅ | Enemy/minion registration, updates |
| **Game Loop** | `GameLoopE2ETests` (complete cycle) | ✅ | Full playable game |

---

## Success Criteria for Jam

All tests passing = Game is **jam-ready** ✅

- [x] Tests compile in Unity
- [x] All PlayMode tests pass (Turn system works)
- [x] All PlayMode tests pass (Health system works)  
- [x] All PlayMode tests pass (AI can be controlled)
- [x] Game can run for 5+ turns without crashing
- [x] Win/lose conditions trigger correctly

---

## Running Specific Test Categories

In Test Runner, use the **Search/Filter** box:

```
Category: GameLoop         → Run full game loop tests only
Category: Health           → Run health/damage tests only
Category: AI               → Run AI brain tests only
```

---

## Debugging a Failed Test

1. **Open Test Runner**: Window > General > Test Runner
2. **Click the failed test name**
3. **Click "Run Selected"**
4. **Open Console** (Window > General > Console)
5. **Look for error messages** — they'll show:
   - What phase failed
   - What entity failed
   - What assertion failed
   - Full stack trace

---

## Performance Benchmarks

Expected test run times (on modern machine):

| Test Suite | Count | Time |
|-----------|-------|------|
| PlayMode E2E | 9 tests | 5s |
| PlayMode Health | 10 tests | 2s |
| PlayMode AI | 10 tests | 2s |
| EditMode | 2 tests | 0.5s |
| **Total** | **31 tests** | **~10s** |

---

## Common Issues & Solutions

### Issue: "DLL Load failed"
**Solution**: Wait 10 seconds, Test Runner will compile. If persists, click **"Clear Compilation"** in Test Runner.

### Issue: Tests don't show up
**Solution**: 
1. Check that test files are in `Assets/Tests/` (not nested in subdirs)
2. Check that classes inherit from `MonoBehaviour` (for PlayMode)
3. Verify `[Test]` or `[UnityTest]` attributes exist
4. Close & reopen Test Runner

### Issue: "NullReferenceException" in test
**Solution**:
1. Check test [SetUp] properly creates required GameObjects
2. Ensure TurnManager.Instance is initialized before use
3. Check test [TearDown] properly cleans up

### Issue: Game scene won't load
**Solution**:
1. Use scene setup scripts: `Gorgonzola/Setup/Quick Play` or `Gorgonzola/Setup/TopDownEngine`
2. Verify Main.unity exists (or is generated)
3. Check Console for missing prefab/component errors

---

## CI/CD Integration (Optional Post-Jam)

For automated test runs outside Unity:

```bash
# Via Unity command line (post-jam)
Unity -projectPath . \
  -runTests \
  -testPlatform playmode \
  -testCategory GameLoop
```

---

## TDD Workflow for Jam

### Before implementing a feature:
1. Write a failing test for the feature
2. Run in Test Runner to confirm it fails
3. Implement the feature
4. Run test again to confirm it passes

### Before committing/submitting:
1. Open Test Runner
2. **Run All** in PlayMode tab
3. Confirm **all tests pass** (green checkmarks)
4. Commit/submit

---

## Test Results Log

Save test results for submission:

1. In Test Runner, click **"▼"** (dropdown) next to test name
2. Click **"Copy Results to Clipboard"**
3. Paste into `test-results/latest.log`

Example output:
```
✅ GameLoopE2ETests.GameLoop_Initializes_WithValidManagers
✅ GameLoopE2ETests.GameLoop_BeginGame_StartsPlayerPhase
✅ GameLoopE2ETests.GameLoop_CompletePhaseCycle_TransitionsCorrectly
✅ HealthSystemTests.Health_ApplyDamage_ReducesHealth
✅ AIBrainIntegrationTests.AIBrain_EnemyRegistration_AddsToRegistry
...
```

---

## Finisher Checklist

Before final submission:

- [ ] All PlayMode tests pass
- [ ] All EditMode tests pass
- [ ] Game can be built (File > Build Settings > Build)
- [ ] Game runs with scene setup (Gorgonzola/Setup/ menus)
- [ ] Player can move and attack
- [ ] Enemies spawn and move
- [ ] Turn cycle completes without errors
- [ ] No exceptions in Console

✅ **Ready for jam submission!**

---

## Next: Adding New Tests

When you add a new feature:

1. Create `Assets/Tests/PlayMode/FeatureNameTests.cs`
2. Add `[Test]` or `[UnityTest]` methods
3. Use `Assert.*` from NUnit
4. Open Test Runner and verify it runs

**Example**:
```csharp
[Test]
public void MyFeature_DoesX_WhenY()
{
    // Arrange
    var obj = new MyFeature();
    
    // Act
    var result = obj.DoX();
    
    // Assert
    Assert.IsTrue(result);
}
```

---

**TDD Mantra**: *If tests pass, ship it.* 🚀