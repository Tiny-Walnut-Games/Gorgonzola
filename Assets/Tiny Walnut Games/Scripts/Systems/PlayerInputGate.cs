using UnityEngine;
using MoreMountains.TopDownEngine;

namespace GorgonzolaMM
{
    /// <summary>
    /// ABILITY-GATING INPUT GATE
    /// 
    /// Gates player abilities (not input) based on turn phase.
    /// Respects TopDown Engine architecture by enabling/disabling ability components.
    /// 
    /// When PlayerTurn:   ALL abilities enabled → player can move/attack
    /// When Other Phase:  ALL abilities disabled → player frozen (even if input received)
    /// 
    /// Listens for spacebar/enter to confirm end of turn.
    /// </summary>
    public class PlayerInputGate : MonoBehaviour
    {
        // === CACHED ABILITY COMPONENTS ===
        private CharacterMovement characterMovement;
        private CharacterOrientation3D characterOrientation;
        private CharacterRun characterRun;
        private CharacterHandleWeapon characterWeapon;
        private InputManager inputManager;
        
        private TurnManager turnManager;
        private bool isPlayerTurnActive = false;

        private void Start()
        {
            // Cache all ability components on this player
            characterMovement = GetComponent<CharacterMovement>();
            characterOrientation = GetComponent<CharacterOrientation3D>();
            characterRun = GetComponent<CharacterRun>();
            characterWeapon = GetComponent<CharacterHandleWeapon>();
            inputManager = GetComponent<InputManager>();

            turnManager = TurnManager.Instance;
            if (turnManager == null)
            {
                Debug.LogError("[PlayerInputGate] TurnManager not found!");
                return;
            }

            // Subscribe to phase changes
            turnManager.OnPhaseChanged += HandlePhaseChange;
            
            // Start with all abilities disabled (waiting for PlayerTurn)
            DisableAllAbilities();

            Debug.Log("[PlayerInputGate] ✨ ABILITY GATING INITIALIZED - abilities gated to PlayerTurn phase only");
            Debug.Log($"[PlayerInputGate] Cached abilities:");
            Debug.Log($"  - CharacterMovement: {(characterMovement != null ? "✓" : "✗")}");
            Debug.Log($"  - CharacterOrientation3D: {(characterOrientation != null ? "✓" : "✗")}");
            Debug.Log($"  - CharacterRun: {(characterRun != null ? "✓" : "✗")}");
            Debug.Log($"  - CharacterHandleWeapon: {(characterWeapon != null ? "✓" : "✗")}");
        }

        private void OnDestroy()
        {
            if (turnManager != null)
                turnManager.OnPhaseChanged -= HandlePhaseChange;
        }

        private void HandlePhaseChange(TurnManager.TurnPhase newPhase)
        {
            isPlayerTurnActive = (newPhase == TurnManager.TurnPhase.PlayerTurn);
            
            if (isPlayerTurnActive)
            {
                EnableAllAbilities();
                Debug.Log("[PlayerInputGate] 🟢 ABILITIES ENABLED (PlayerTurn phase active)");
            }
            else
            {
                DisableAllAbilities();
                Debug.Log($"[PlayerInputGate] 🔴 ABILITIES DISABLED ({newPhase} phase active)");
            }
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

        private void Update()
        {
            // Only listen for turn confirmation during player turn
            if (!isPlayerTurnActive)
                return;

            // Check for spacebar or enter to end turn
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("[PlayerInputGate] ⏹️  Player pressed CONFIRM → Ending turn");
                turnManager.ConfirmPlayerAction();
            }
        }
    }
}