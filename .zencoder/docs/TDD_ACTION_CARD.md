# 🎯 TDD Action Card - Quick Reference

**Print this or pin it while developing!**

---

## Right Now (5 minutes)

### Verify Tests Work
```
1. Open Gorgonzola in Unity
2. Window > General > Test Runner
3. Click "PlayMode" tab
4. Click "Run All"
5. Wait ~10 seconds
6. ✅ See all tests pass (green)? YOU'RE GOOD!
```

**If tests don't show**: Close & reopen Test Runner window

---

## During Development

### Before You Implement a Feature

```
┌─ Think of a feature you want (e.g., "player can attack")
├─ Write a test that validates it
├─ Run test → RED (fails)
├─ Implement the feature
├─ Run test → GREEN (passes)
└─ Commit & move on
```

### Before Each Commit
```
✅ Open Test Runner
✅ Click "Run All"  
✅ All green? → Commit
❌ Any red? → Fix first, then commit
```

### When Something Breaks
```
1. Open Test Runner
2. Look for RED tests
3. Click the red test
4. Open Console (Window > General > Console)
5. Read error message
6. Fix the bug
7. Run test again
8. GREEN? ✅ Back to work
```

---

## Game Loop Validation

### "Is the game playable?"
Run these in order:

```
Step 1: Run all tests
        ✅ All tests pass? → Step 2

Step 2: Build game
        File > Build Settings > Build
        ✅ No errors? → Step 3

Step 3: Play the game
        Use Gorgonzola/Setup/ menu to spawn scene
        Play for 5+ turns
        ✅ No crashes? → PLAYABLE!
```

---

## Test Categories (Organize Your Mind)

Use Test Runner's **search/filter** box:

| Type | Command | Tests |
|------|---------|-------|
| All tests | (leave blank) | 40+ |
| Turn system | `Category: TurnSystem` | 9 |
| Health system | `Category: Health` | 10 |
| AI system | `Category: AI` | 10 |
| Full loop | `Category: GameLoop` | 9 |

---

## Most Important Tests (Run These First)

1. **GameLoopE2ETests.GameLoop_Initializes_WithValidManagers**
   - Confirms game can start

2. **GameLoopE2ETests.GameLoop_CompletePhaseCycle_TransitionsCorrectly**
   - Confirms turns work

3. **HealthSystemTests.Health_ApplyDamage_ReducesHealth**
   - Confirms damage works

4. **AIBrainIntegrationTests.AIBrain_EnemyRegistration_AddsToRegistry**
   - Confirms AI can be controlled

---

## File Locations (Bookmark These)

```
Test Files:
📁 Assets/Tests/PlayMode/
   📄 GameLoopE2ETests.cs          ← Full game tests
   📄 HealthSystemTests.cs          ← Damage/death tests
   📄 AIBrainIntegrationTests.cs    ← AI registry tests
   📄 TurnManagerTests.cs           ← Turn system tests

Test Docs:
📁 .zencoder/docs/
   📄 RUNNING_TESTS.md             ← How to run tests
   📄 TEST_STRATEGY.md             ← TDD philosophy
   📄 FINISHER_LOG_TDD_PHASE.md    ← What's done
   📄 TDD_ACTION_CARD.md            ← THIS FILE

Core Game Code:
📁 Assets/Tiny Walnut Games/Scripts/
   📁 _Core/
      📄 GameJamBootstrap.cs
   📁 Systems/
      📄 TurnManager.cs
```

---

## Common Scenarios

### "I want to add a new feature"

**Example**: "Player should be able to use a special ability"

```
1. Create Assets/Tests/PlayMode/SpecialAbilityTests.cs
2. Write test:
   [Test]
   public void SpecialAbility_UseSqueak_DealsAOEDamage()
   {
       // Test code here
   }
3. Run test → RED (fails)
4. Implement feature in game code
5. Run test → GREEN (passes)
6. Add more tests as needed
7. Commit
```

---

### "A test is failing"

**Example**: `GameLoopE2ETests.GameLoop_CompletePhaseCycle_TransitionsCorrectly` is RED

```
1. Click the test in Test Runner
2. Click "Run Selected"
3. Open Console (Window > General > Console)
4. Look for ERROR or assertion message
5. Example error: "Expected PlayerPhase but got EnemyPhase"
   → Bug is in phase transition logic
6. Open TurnManager.cs
7. Fix the bug
8. Run test again
9. ✅ Now GREEN? Fixed!
```

---

### "I want to debug WHY a test failed"

```
1. Add Debug.Log() to your test or game code:
   Debug.Log("Phase is: " + currentPhase);
   
2. Run test via Test Runner
3. Open Console
4. Look for [Test] logs
5. See what happened
6. Fix accordingly
```

---

## Performance Checks

### Expected Test Times
- ✅ 40+ tests run in **~10 seconds**
- ✅ Each test is under **1 second**
- ❌ If any test takes >5 seconds → Check for infinite loops

### Expected Game Performance
- ✅ 5+ turns without crash
- ✅ 60 FPS during turn cycles
- ✅ <100ms per turn phase

---

## Before Final Submission

### Jam Submission Checklist
```
[ ] All tests pass (Run All → all green)
[ ] Game builds cleanly (File > Build > no errors)
[ ] Playtest works (5+ turns, no crashes)
[ ] No ERROR logs in Console
[ ] README.md is up to date
[ ] Commit with message: "Game jam ready - all tests passing"
[ ] Push/submit
[ ] 🎉 Done!
```

---

## Quick Troubleshooting

| Problem | Solution |
|---------|----------|
| Tests don't show up | Close/reopen Test Runner |
| "DLL Load failed" | Wait 10s, Test Runner compiles |
| Test timeouts | Increase timeout in test setup |
| "NullReferenceException" | Check [SetUp] creates objects |
| Game won't play | Use scene setup menu (Gorgonzola/Setup/) |
| Turn system broken | Check TurnManagerTests pass first |

---

## Key Principle

> **If the test suite passes, the game is ready to jam.**

Do not:
- ❌ Ignore failing tests
- ❌ Comment out tests to "fix" them
- ❌ Commit with failing tests
- ❌ Assume something works if tests fail

Do:
- ✅ Fix failing tests immediately
- ✅ Run tests before committing
- ✅ Use tests to guide development
- ✅ Trust that green tests = working code

---

## Session Template

```
START SESSION:
├─ Run all tests
└─ All pass? ✅ Continue

WORK SESSION:
├─ Write test for new feature
├─ Run test (RED)
├─ Implement feature
├─ Run test (GREEN)
└─ Repeat

END SESSION:
├─ Run all tests
├─ Commit if all green
└─ Note what works in log
```

---

## Emergency SOS (If Everything Broken)

```
Last resort - full reset:

1. Copy Assets/Tiny Walnut Games/Scripts/ to backup
2. Revert last few commits (git revert)
3. Run all tests → Do they pass?
4. Yes → Problem was recent work
   No → Problem was earlier

5. Git bisect to find exact breaking commit
   git bisect start
   git bisect bad    (current broken state)
   git bisect good   (last known good)

6. Test at each bisect point
7. Found it? → Fix and commit
8. Resume development
```

---

## 🚀 You're Ready!

**Everything is set up. Now go make an awesome game!**

---

*Next: Run tests. Then code. Then commit. Repeat.*

**Happy jamming!** 🎮✨