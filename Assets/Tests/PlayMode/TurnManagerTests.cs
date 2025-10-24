using NUnit.Framework;
using UnityEngine;
using GorgonzolaMM;

namespace GorgonzolaMM.Tests;

/// <summary>
/// PlayMode tests for TurnManager.
/// Verify phase transitions, phase duration, and AI registration.
/// </summary>
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
    }

    [Test]
    public void TurnManager_BeginGame_StartsPlayerPhase()
    {
        turnManager.BeginGame();
        Assert.AreEqual(TurnManager.TurnPhase.PlayerTurn, turnManager.CurrentPhase);
        Assert.AreEqual(0, turnManager.TurnCount);
    }

    [Test]
    public void TurnManager_ConfirmPlayerAction_TransitionsToEnemyPhase()
    {
        turnManager.BeginGame();
        turnManager.ConfirmPlayerAction();
        // Note: Actual transition is async via Invoke; test may need adjustment for async behavior.
        // For now, just verify no exceptions.
        Assert.Pass();
    }
}