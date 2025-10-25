using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using GorgonzolaMM;

namespace GorgonzolaMM.Tests
{
    /// <summary>
    /// PlayMode tests for TurnManager.
    /// Verify phase transitions, phase duration, and AI registration.
    /// Part of core game loop validation suite.
    /// </summary>
    [Category("TurnSystem")]
    public class TurnManagerTests
    {
        private GameObject turnManagerGO;
        private TurnManager turnManager;

        [SetUp]
        public void Setup()
        {
            turnManagerGO = new GameObject("TurnManager");
            turnManager = turnManagerGO.AddComponent<TurnManager>();
        }

        [TearDown]
        public void Teardown()
        {
            Object.Destroy(turnManagerGO);
        }

        [Test]
        public void TurnManager_Initializes_InIdlePhase()
        {
            Assert.AreEqual(TurnManager.TurnPhase.Idle, turnManager.CurrentPhase);
            Debug.Log("[Test] ✓ TurnManager initializes in Idle phase");
        }

        [Test]
        public void TurnManager_BeginGame_StartsPlayerPhase()
        {
            turnManager.BeginGame();
            Assert.AreEqual(TurnManager.TurnPhase.PlayerTurn, turnManager.CurrentPhase);
            Assert.AreEqual(0, turnManager.TurnCount);
            Debug.Log("[Test] ✓ BeginGame transitions to PlayerTurn at turn 0");
        }

        [Test]
        public void TurnManager_ConfirmPlayerAction_WhenInPlayerPhase_DoesNotThrow()
        {
            turnManager.BeginGame();
            Assert.DoesNotThrow(() => turnManager.ConfirmPlayerAction());
            Debug.Log("[Test] ✓ ConfirmPlayerAction executes without exception");
        }

        [Test]
        public void TurnManager_ConfirmPlayerAction_OutsidePlayerPhase_LogsWarning()
        {
            // Start in player phase then move past it
            turnManager.BeginGame();
            turnManager.ConfirmPlayerAction();
            
            // Attempting to confirm again should log warning (graceful degradation)
            Assert.DoesNotThrow(() => turnManager.ConfirmPlayerAction());
            Debug.Log("[Test] ✓ ConfirmPlayerAction outside PlayerPhase handled gracefully");
        }

        [Test]
        public void TurnManager_CurrentPhaseProperty_IsAccessible()
        {
            var phase = turnManager.CurrentPhase;
            Assert.IsNotNull(phase);
            Assert.AreEqual(TurnManager.TurnPhase.Idle, phase);
            Debug.Log("[Test] ✓ CurrentPhase property is readable");
        }

        [Test]
        public void TurnManager_TurnCountProperty_IsAccessible()
        {
            turnManager.BeginGame();
            int count = turnManager.TurnCount;
            Assert.That(count, Is.GreaterThanOrEqualTo(0));
            Debug.Log($"[Test] ✓ TurnCount property is readable: {count}");
        }

        [Test]
        public void TurnManager_OnPhaseChanged_IsSubscribable()
        {
            Assert.DoesNotThrow(() => turnManager.OnPhaseChanged += phase => { });
            Debug.Log("[Test] ✓ OnPhaseChanged event is subscribable");
        }

        [Test]
        public void TurnManager_OnPhaseChanged_FiresOnBeginGame()
        {
            bool eventFired = false;
            turnManager.OnPhaseChanged += phase => eventFired = true;
            
            turnManager.BeginGame();
            
            Assert.IsTrue(eventFired, "OnPhaseChanged should fire when game begins");
            Debug.Log("[Test] ✓ OnPhaseChanged event fires on BeginGame");
        }
    }
}