using UnityEngine;

namespace GorgonzolaMM
{
    /// <summary>
    /// Entry point for the game jam. Initializes core systems and loads the main scene.
    /// </summary>
    public class GameJamBootstrap : MonoBehaviour
    {
        [SerializeField] private bool verbose = true;

        private void Awake()
        {
            if (verbose)
                Debug.Log("[Bootstrap] Gorgonzola initialized. The Burrow awaits...");

            // Initialize TurnManager singleton
            if (TurnManager.Instance is null)
                Debug.LogWarning("[Bootstrap] TurnManager not found. Ensure it exists in the scene.");
        }

        private void Start()
        {
            if (verbose)
                Debug.Log("[Bootstrap] Game ready. Awaiting player input.");
            
            // Start the turn system
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.BeginGame();
            }
            else if (verbose)
            {
                Debug.LogWarning("[Bootstrap] TurnManager not found. Cannot start turn system.");
            }
        }
    }
}