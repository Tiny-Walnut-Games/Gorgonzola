using UnityEngine;
using System;
using System.Collections.Generic;

namespace GorgonzolaMM
{
    /// <summary>
    /// Manages turn-based flow: Player Phase → Enemy Phase → Minion Phase → End Turn.
    /// Orchestrates input, AI updates, and collision resolution.
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; set; }

        public enum TurnPhase
        {
            PlayerTurn,
            EnemyTurn,
            MinionTurn,
            Resolution,
            Idle
        }

        [SerializeField] private float turnDuration = 1f;
        [SerializeField] private bool verbose = true;

        private TurnPhase currentPhase = TurnPhase.Idle;
        private int turnCount = 0;
        private float phaseTimer = 0f;

        private List<IEnemyBrain> enemies = new();
        private List<IMinionAI> minions = new();

        public TurnPhase CurrentPhase => currentPhase;
        public int TurnCount => turnCount;
        
        /// <summary>
        /// Gets or sets the turn duration (delay between phase transitions).
        /// Used mainly for testing to speed up phase transitions.
        /// </summary>
        public float TurnDuration
        {
            get => turnDuration;
            set => turnDuration = value;
        }
        
        private event Action<TurnPhase> _onPhaseChanged;
        public event Action<TurnPhase> OnPhaseChanged
        {
            add => _onPhaseChanged += value;
            remove => _onPhaseChanged -= value;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            if (verbose)
                Debug.Log("[TurnManager] Ready. Awaiting first player input...");
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            // Placeholder: Tick current phase
            phaseTimer += Time.deltaTime;

            switch (currentPhase)
            {
                case TurnPhase.PlayerTurn:
                    // TODO: Handle player input
                    break;
                case TurnPhase.EnemyTurn:
                    // TODO: Tick all enemy AI
                    break;
                case TurnPhase.MinionTurn:
                    // TODO: Tick minion AI
                    break;
            }
        }

        /// <summary>
        /// Called by player controller when action is confirmed.
        /// </summary>
        public void ConfirmPlayerAction()
        {
            if (currentPhase != TurnPhase.PlayerTurn)
            {
                Debug.LogWarning("[TurnManager] Not in player phase.");
                return;
            }

            TransitionPhase(TurnPhase.EnemyTurn);
        }

        /// <summary>
        /// Register an enemy AI brain.
        /// </summary>
        public void RegisterEnemy(IEnemyBrain brain)
        {
            enemies.Add(brain);
        }

        /// <summary>
        /// Unregister an enemy (on death).
        /// </summary>
        public void UnregisterEnemy(IEnemyBrain brain)
        {
            enemies.Remove(brain);
        }

        /// <summary>
        /// Register a minion AI.
        /// </summary>
        public void RegisterMinion(IMinionAI minion)
        {
            minions.Add(minion);
        }

        /// <summary>
        /// Unregister a minion (on death).
        /// </summary>
        public void UnregisterMinion(IMinionAI minion)
        {
            minions.Remove(minion);
        }

        private void TransitionPhase(TurnPhase nextPhase)
        {
            if (currentPhase == nextPhase)
                return;

            currentPhase = nextPhase;
            phaseTimer = 0f;

            if (verbose)
                Debug.Log($"[TurnManager] Phase → {nextPhase} (Turn {turnCount})");

            _onPhaseChanged?.Invoke(nextPhase);

            // Auto-transition logic for non-player phases
            switch (nextPhase)
            {
                case TurnPhase.EnemyTurn:
                    // TODO: Tick enemies
                    Invoke(nameof(AdvanceToMinionPhase), turnDuration);
                    break;
                case TurnPhase.MinionTurn:
                    // TODO: Tick minions
                    Invoke(nameof(AdvanceToResolution), turnDuration);
                    break;
                case TurnPhase.Resolution:
                    // TODO: Resolve collisions, damage
                    Invoke(nameof(StartNewTurn), 0.5f);
                    break;
            }
        }

        private void AdvanceToMinionPhase()
        {
            TransitionPhase(TurnPhase.MinionTurn);
        }

        private void AdvanceToResolution()
        {
            TransitionPhase(TurnPhase.Resolution);
        }

        private void StartNewTurn()
        {
            turnCount++;
            TransitionPhase(TurnPhase.PlayerTurn);
        }

        public void BeginGame()
        {
            turnCount = 0;
            TransitionPhase(TurnPhase.PlayerTurn);
        }
    }

    /// <summary>
    /// Interface for enemy AI brains.
    /// </summary>
    public interface IEnemyBrain
    {
        void UpdateAI();
    }

    /// <summary>
    /// Interface for minion AI (ghosts, zombies).
    /// </summary>
    public interface IMinionAI
    {
        void UpdateAI();
    }
}