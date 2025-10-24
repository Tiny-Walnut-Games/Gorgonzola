using UnityEngine;
using Unity.Cinemachine;

namespace GorgonzolaMM
{
    /// <summary>
    /// Configures Cinemachine 3 camera for Unity 6+ arena gameplay.
    /// Handles smooth follow, look-ahead, and shake with modern CM3 API.
    /// </summary>
    public class CinemachineArenaSetup : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float followDamping = 0.1f;
        [SerializeField] private Vector3 followOffset = new(0, 8, -8);
        [SerializeField] private Vector3 lookAheadOffset = Vector3.zero;
        [SerializeField] private float orthoSize = 10f;
        [SerializeField] private bool useOrthographic = true;

        private CinemachineCamera cinemachineCamera;
        private CinemachinePositionComposer positionComposer;
        private CinemachineRotationComposer rotationComposer;
        private CinemachineBasicMultiChannelPerlin noiseComponent;
        private Coroutine shakeCoroutine;

        private void Start()
        {
            if (playerTransform == null)
            {
                playerTransform = FindObjectOfType<PlayerController>()?.transform;
            }

            SetupCinemachineCamera();
        }

        /// <summary>
        /// Configures Cinemachine 3 camera rig for arena follow (Unity 6+).
        /// </summary>
        public void SetupCinemachineCamera()
        {
            if (playerTransform == null)
            {
                Debug.LogError("[CinemachineArenaSetup] Player transform not found!");
                return;
            }

            // Find or create main camera
            var mainCam = Camera.main;
            if (mainCam == null)
            {
                var camGO = new GameObject("Main Camera");
                mainCam = camGO.AddComponent<Camera>();
            }

            // Find or create Cinemachine Brain (CM3 manages this differently)
            var brainGO = mainCam.gameObject;
            var brain = brainGO.GetComponent<CinemachineBrain>();
            if (brain == null)
            {
                brain = brainGO.AddComponent<CinemachineBrain>();
            }

            // Create or find virtual camera
            var virtualCamGO = GameObject.Find("VirtualCamera_Arena");
            if (virtualCamGO == null)
            {
                virtualCamGO = new GameObject("VirtualCamera_Arena");
            }

            cinemachineCamera = virtualCamGO.GetComponent<CinemachineCamera>();
            if (cinemachineCamera == null)
            {
                cinemachineCamera = virtualCamGO.AddComponent<CinemachineCamera>();
            }

            // Configure Follow (CM3 uses CinemachinePositionComposer)
            positionComposer = cinemachineCamera.GetComponent<CinemachinePositionComposer>();
            if (positionComposer == null)
            {
                positionComposer = virtualCamGO.AddComponent<CinemachinePositionComposer>();
            }

            // Set follow target and damping
            positionComposer.Target = playerTransform;
            positionComposer.FollowOffset = followOffset;
            
            // CM3 uses TargetSettings for damping control
            var followSettings = positionComposer.FollowOffset;
            positionComposer.Damping = new Vector3(followDamping, followDamping, followDamping);

            // Configure Look-Ahead (CM3 uses CinemachineRotationComposer)
            rotationComposer = cinemachineCamera.GetComponent<CinemachineRotationComposer>();
            if (rotationComposer == null)
            {
                rotationComposer = virtualCamGO.AddComponent<CinemachineRotationComposer>();
            }

            rotationComposer.Target = playerTransform;
            rotationComposer.TargetOffset = lookAheadOffset;

            // Setup noise/shake (CinemachineBasicMultiChannelPerlin in CM3)
            noiseComponent = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
            if (noiseComponent == null)
            {
                noiseComponent = virtualCamGO.AddComponent<CinemachineBasicMultiChannelPerlin>();
            }

            // Configure main camera
            mainCam.orthographic = useOrthographic;
            if (useOrthographic)
            {
                mainCam.orthographicSize = orthoSize;
            }

            Debug.Log($"[CinemachineArenaSetup] Cinemachine 3 camera configured. Following: {playerTransform.name}");
        }

        /// <summary>
        /// Triggers camera shake effect for impacts/explosions (CM3 compatible).
        /// </summary>
        public void ShakeCamera(float amplitude = 1f, float frequency = 2f, float duration = 0.1f)
        {
            if (noiseComponent == null) return;

            // Stop any existing shake coroutine
            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
            }

            // Apply noise profile (CM3 uses AmplitudeGain and FrequencyGain)
            noiseComponent.AmplitudeGain = amplitude;
            noiseComponent.FrequencyGain = frequency;

            shakeCoroutine = StartCoroutine(StopCameraShakeAfterDelay(duration));
        }

        private System.Collections.IEnumerator StopCameraShakeAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            StopCameraShake();
        }

        private void StopCameraShake()
        {
            if (noiseComponent != null)
            {
                noiseComponent.AmplitudeGain = 0;
                noiseComponent.FrequencyGain = 0;
            }
        }

        /// <summary>
        /// Changes follow target (useful for cutscenes or camera switching).
        /// </summary>
        public void SetFollowTarget(Transform target)
        {
            if (positionComposer != null)
            {
                positionComposer.Target = target;
            }

            if (rotationComposer != null)
            {
                rotationComposer.Target = target;
            }
        }
    }

    /// <summary>
    /// Placeholder interface - implement in your player controller.
    /// </summary>
    public interface IPlayerController
    {
        Transform GetTransform();
    }

    /// <summary>
    /// Minimal player controller for testing arena setup.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        private Vector3 moveInput;

        private void Update()
        {
            // Simple WASD movement
            moveInput.x = Input.GetAxis("Horizontal");
            moveInput.z = Input.GetAxis("Vertical");

            transform.position += moveInput * (moveSpeed * Time.deltaTime);
        }
    }
}