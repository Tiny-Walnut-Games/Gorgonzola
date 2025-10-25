using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using GorgonzolaMM;

namespace GorgonzolaMM.Editor
{
    /// <summary>
    /// ONE-CLICK test scene setup. Converts SampleScene to a playable test arena.
    /// Uses TopDownEngine + Cinemachine infrastructure directly.
    /// Run once, then play.
    /// </summary>
    public class QuickPlaySceneSetup
    {
        [MenuItem("Gorgonzola/Quick Setup/Test Arena (One-Click)")]
        public static void SetupTestArena()
        {
            // Try TopDown Engine setup first, fallback to basic if components missing
            if (TryTopDownEngineSetup())
            {
                Debug.Log("[QuickPlaySceneSetup] ✅ TopDown Engine test arena ready. Press Play!");
                return;
            }

            // Fallback to basic Unity components
            SetupBasicArena();
        }

        private static bool TryTopDownEngineSetup()
        {
            try
            {
                // Check if TopDown Engine components are available
                var topDownType = System.Type.GetType("MoreMountains.TopDownEngine.TopDownController3D, MoreMountains.TopDownEngine");
                if (topDownType == null) return false;

                // Use the comprehensive TopDown Engine setup
                TopDownEngineSceneSetup.SetupCompleteArena();
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[QuickPlaySceneSetup] TopDown Engine components not available: {ex.Message}. Using basic setup.");
                return false;
            }
        }

        private static void SetupBasicArena()
        {
            // Open SampleScene
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            EditorSceneManager.OpenScene("Assets/Tiny Walnut Games/Scenes/SampleScene.unity", OpenSceneMode.Single);

            var scene = EditorSceneManager.GetActiveScene();

            // Clear existing objects (except lights)
            var roots = scene.GetRootGameObjects();
            foreach (var root in roots)
            {
                if (root.name != "Directional Light" && root.name != "Main Camera")
                    Object.DestroyImmediate(root);
            }

            // Create hierarchy
            CreateManagers(scene);
            CreateArenaLayout(scene);
            CreatePlayer(scene);
            CreateBasicEnemies(scene);
            CreatePhylactery(scene);
            CreateCamera(scene);

            EditorSceneManager.SaveScene(scene);
            EditorSceneManager.MarkSceneDirty(scene);

            Debug.Log("[QuickPlaySceneSetup] ✅ Basic test arena ready. Press Play!");
        }

        private static void CreateManagers(Scene scene)
        {
            var managersGo = new GameObject("--- Managers ---");
            SceneManager.MoveGameObjectToScene(managersGo, scene);

            // Bootstrap (initializes core systems)
            var bootstrapGo = new GameObject("GameJamBootstrap");
            bootstrapGo.transform.SetParent(managersGo.transform);
            var bootstrap = bootstrapGo.AddComponent<GameJamBootstrap>();

            // TurnManager
            var turnGo = new GameObject("TurnManager");
            turnGo.transform.SetParent(managersGo.transform);
            var turnMgr = turnGo.AddComponent<TurnManager>();
            turnGo.AddComponent<AudioListener>();

            // GameManager
            var gameGo = new GameObject("GameManager");
            gameGo.transform.SetParent(managersGo.transform);
            gameGo.AddComponent<AudioListener>();
        }

        private static void CreateArenaLayout(Scene scene)
        {
            var sceneGo = new GameObject("--- Arena ---");
            SceneManager.MoveGameObjectToScene(sceneGo, scene);

            // Ground
            var groundGo = new GameObject("Ground");
            groundGo.transform.SetParent(sceneGo.transform);
            groundGo.transform.localPosition = Vector3.zero;
            groundGo.layer = LayerMask.NameToLayer("Ground");  // ✅ Set to "Ground" layer so TopDownController3D detects it

            var mf = groundGo.AddComponent<MeshFilter>();
            mf.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

            var mr = groundGo.AddComponent<MeshRenderer>();
            var groundMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            groundMat.color = new Color(0.3f, 0.5f, 0.3f);
            mr.sharedMaterial = groundMat;

            var col = groundGo.AddComponent<BoxCollider>();
            col.enabled = true;  // Ensure collider is enabled
            groundGo.transform.localScale = new Vector3(50, 0.5f, 50);
            groundGo.transform.localPosition = new Vector3(0, -0.25f, 0);

            // Spawn points
            var spawnsGo = new GameObject("SpawnPoints");
            spawnsGo.transform.SetParent(sceneGo.transform);

            var playerSpawn = new GameObject("PlayerSpawn");
            playerSpawn.transform.SetParent(spawnsGo.transform);
            playerSpawn.transform.localPosition = new Vector3(0, 1, 0);

            var enemySpawns = new GameObject("EnemySpawns");
            enemySpawns.transform.SetParent(spawnsGo.transform);

            Vector3[] positions = {
                new Vector3(15, 1, 15),
                new Vector3(-15, 1, 15),
                new Vector3(15, 1, -15),
                new Vector3(-15, 1, -15)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                var spawn = new GameObject($"EnemySpawn_{i}");
                spawn.transform.SetParent(enemySpawns.transform);
                spawn.transform.localPosition = positions[i];
            }

            // Lighting
            var lightGo = GameObject.Find("Directional Light") ?? new GameObject("Directional Light");
            var light = lightGo.GetComponent<Light>();
            if (light == null)
                light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.5f;
            lightGo.transform.rotation = Quaternion.Euler(45, 45, 0);

            Debug.Log("[QuickPlaySceneSetup] Arena created: Ground, spawn points, lighting.");
        }

        private static void CreatePlayer(Scene scene)
        {
            // === NECROHAMSTO PLAYER WITH TOPDOWN ENGINE ===
            var playerGo = new GameObject("NecroHAMSTO");
            SceneManager.MoveGameObjectToScene(playerGo, scene);
            playerGo.transform.position = new Vector3(0, 1, 0);
            playerGo.tag = "Player";
            playerGo.layer = LayerMask.NameToLayer("Player");

            // Model hierarchy
            var modelGo = new GameObject("Model");
            modelGo.transform.SetParent(playerGo.transform);
            
            var visualGo = new GameObject("Visual");
            visualGo.transform.SetParent(modelGo.transform);

            // Visual representation
            var mf = visualGo.AddComponent<MeshFilter>();
            mf.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            var mr = visualGo.AddComponent<MeshRenderer>();
            var playerMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            playerMat.color = new Color(0.8f, 0.6f, 0.2f); // Hamster gold color
            mr.sharedMaterial = playerMat;
            visualGo.transform.localScale = new Vector3(1, 1.5f, 1); // Taller hamster

            // === TOPDOWN ENGINE COMPONENTS ===
            
            // 1. Physics: Rigidbody (kinematic, required by TopDownController3D)
            var rigidbody = playerGo.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = true;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            // 2. Physics: CapsuleCollider (required by TopDownController3D)
            var capsuleCollider = playerGo.AddComponent<CapsuleCollider>();
            capsuleCollider.height = 1.8f;
            capsuleCollider.radius = 0.5f;
            capsuleCollider.center = new Vector3(0, 0.9f, 0);

            // 3. Physics: CharacterController for 3D movement
            var characterController = playerGo.AddComponent<CharacterController>();
            characterController.height = 1.8f;
            characterController.radius = 0.5f;
            characterController.center = new Vector3(0, 0.9f, 0);

            // 4. Core TopDown Engine components (try-catch to handle missing references)
            try 
            {
                // TopDown Controller for 3D
                var topDownController = playerGo.AddComponent<MoreMountains.TopDownEngine.TopDownController3D>();
                
                // Core Character component
                var character = playerGo.AddComponent<MoreMountains.TopDownEngine.Character>();
                character.CharacterModel = modelGo;
                character.PlayerID = "Player1";
                
                // Movement abilities
                var characterMovement = playerGo.AddComponent<MoreMountains.TopDownEngine.CharacterMovement>();
                characterMovement.WalkSpeed = 4f;
                
                var characterOrientation = playerGo.AddComponent<MoreMountains.TopDownEngine.CharacterOrientation3D>();
                var characterRun = playerGo.AddComponent<MoreMountains.TopDownEngine.CharacterRun>();
                characterRun.RunSpeed = 8f;
                
                // Health system
                var health = playerGo.AddComponent<MoreMountains.TopDownEngine.Health>();
                health.InitialHealth = 100f;
                health.MaximumHealth = 100f;
                
                // Weapon handling
                var weaponHandler = playerGo.AddComponent<MoreMountains.TopDownEngine.CharacterHandleWeapon>();
                
                // Input system with gating
                var inputManager = playerGo.AddComponent<MoreMountains.TopDownEngine.InputManager>();
                var inputGate = playerGo.AddComponent<PlayerInputGate>();
                
                Debug.Log("[QuickPlaySceneSetup] ✅ NecroHAMSTO created with TopDown Engine components + InputGate");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[QuickPlaySceneSetup] ⚠️ TopDown Engine components not fully available: {e.Message}");
                Debug.LogWarning("Using fallback SimplePlayerController - scene will still be playable for testing");
                
                // Fallback: Use simple controller for basic testing
                // Note: Rigidbody, CapsuleCollider, and CharacterController are already added above
                playerGo.AddComponent<SimplePlayerController>();
                
                // Ensure rigidbody is not kinematic for fallback
                rigidbody.isKinematic = false;
            }

            Debug.Log("[QuickPlaySceneSetup] Player created: NecroHAMSTO ready for turn-based action");
        }

        private static void CreateBasicEnemies(Scene scene)
        {
            var enemySpawns = GameObject.Find("SpawnPoints/EnemySpawns");
            if (enemySpawns == null) return;

            // === CUTE SNAKES WITH AI ===
            var enemiesParent = new GameObject("--- ENEMIES ---");
            SceneManager.MoveGameObjectToScene(enemiesParent, scene);

            string[] snakeTypes = { "RibbonSnake", "BoaHugger", "GlitterCobra", "RibbonSnake" };
            Color[] snakeColors = { 
                Color.green,      // Ribbon Snake - fast, straight movement
                Color.yellow,     // Boa Hugger - slow, immobilizing  
                Color.magenta,    // Glitter Cobra - ranged charm
                Color.cyan        // Another Ribbon Snake
            };

            for (int i = 0; i < enemySpawns.transform.childCount; i++)
            {
                var spawn = enemySpawns.transform.GetChild(i);
                var snakeType = snakeTypes[i % snakeTypes.Length];
                var snakeColor = snakeColors[i % snakeColors.Length];
                
                var enemyGo = CreateCuteSnakeEnemy(snakeType, spawn.position, snakeColor, scene);
                enemyGo.transform.SetParent(enemiesParent.transform);
            }

            Debug.Log("[QuickPlaySceneSetup] ✅ Cute Snake enemies created with AI behavior");
        }

        private static GameObject CreateCuteSnakeEnemy(string snakeType, Vector3 position, Color color, Scene scene)
        {
            var snakeGo = new GameObject(snakeType);
            SceneManager.MoveGameObjectToScene(snakeGo, scene);
            snakeGo.transform.position = position;
            snakeGo.tag = "Enemy";
            snakeGo.layer = LayerMask.NameToLayer("Enemies");

            // === MODEL HIERARCHY ===
            var modelGo = new GameObject("Model");
            modelGo.transform.SetParent(snakeGo.transform);
            
            var visualGo = new GameObject("Visual");
            visualGo.transform.SetParent(modelGo.transform);

            // Snake-like visual (elongated capsule)
            var mf = visualGo.AddComponent<MeshFilter>();
            mf.mesh = Resources.GetBuiltinResource<Mesh>("Capsule.fbx");
            var mr = visualGo.AddComponent<MeshRenderer>();
            var snakeMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            snakeMat.color = color;
            mr.sharedMaterial = snakeMat;
            
            // Snake proportions: longer, lower
            visualGo.transform.localScale = new Vector3(0.8f, 0.5f, 2f);

            // === PHYSICS ===
            // Rigidbody (kinematic, required by TopDownController3D)
            var rigidbody = snakeGo.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = true;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            
            // CapsuleCollider (required by TopDownController3D)
            var collider = snakeGo.AddComponent<CapsuleCollider>();
            collider.height = 2f;
            collider.radius = 0.4f;

            // CharacterController (required by TopDownController3D)
            var characterController = snakeGo.AddComponent<CharacterController>();
            characterController.height = 2f;
            characterController.radius = 0.4f;
            characterController.center = new Vector3(0, 1f, 0);

            // === TOPDOWN ENGINE COMPONENTS ===
            try 
            {
                // Controller
                var controller = snakeGo.AddComponent<MoreMountains.TopDownEngine.TopDownController3D>();
                
                // Character
                var character = snakeGo.AddComponent<MoreMountains.TopDownEngine.Character>();
                character.CharacterType = MoreMountains.TopDownEngine.Character.CharacterTypes.AI;
                character.CharacterModel = modelGo;
                
                // Movement
                var movement = snakeGo.AddComponent<MoreMountains.TopDownEngine.CharacterMovement>();
                var orientation = snakeGo.AddComponent<MoreMountains.TopDownEngine.CharacterOrientation3D>();
                
                // AI System
                var brain = snakeGo.AddComponent<MoreMountains.Tools.AIBrain>();
                var aiDecision = snakeGo.AddComponent<MoreMountains.TopDownEngine.AIDecisionDetectTargetRadius3D>();
                var aiAction = snakeGo.AddComponent<MoreMountains.TopDownEngine.AIActionMoveTowardsTarget3D>();
                
                // Configure based on snake type
                switch (snakeType)
                {
                    case "RibbonSnake":
                        movement.WalkSpeed = 6f; // Fast
                        aiDecision.Radius = 15f;
                        break;
                    case "BoaHugger":
                        movement.WalkSpeed = 2f; // Slow but dangerous
                        aiDecision.Radius = 10f;
                        break;
                    case "GlitterCobra":
                        movement.WalkSpeed = 4f; // Medium
                        aiDecision.Radius = 20f; // Long range detection
                        break;
                }

                // Health
                var health = snakeGo.AddComponent<MoreMountains.TopDownEngine.Health>();
                health.InitialHealth = 50f;
                health.MaximumHealth = 50f;

                // Damage on contact
                var damageOnTouch = snakeGo.AddComponent<MoreMountains.TopDownEngine.DamageOnTouch>();
                damageOnTouch.MinDamageCaused = 10f;
                damageOnTouch.MaxDamageCaused = 10f;
                
                Debug.Log($"[QuickPlaySceneSetup] ✅ {snakeType} created with TopDown Engine AI");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[QuickPlaySceneSetup] ⚠️ TopDown Engine AI components not available for {snakeType}: {e.Message}");
                Debug.LogWarning("Snake will be static but scene is still testable");
            }

            return snakeGo;
        }

        private static void CreatePhylactery(Scene scene)
        {
            // === THE PHYLACTERY - GAME OVER CONDITION ===
            var phylacteryGo = new GameObject("Phylactery");
            SceneManager.MoveGameObjectToScene(phylacteryGo, scene);
            phylacteryGo.transform.position = new Vector3(8, 1, 8); // Corner position
            phylacteryGo.tag = "Phylactery";

            // Visual: Crystal-like appearance
            var mf = phylacteryGo.AddComponent<MeshFilter>();
            mf.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            var mr = phylacteryGo.AddComponent<MeshRenderer>();
            var phylacteryMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            phylacteryMat.color = new Color(0.5f, 0.2f, 0.8f, 0.8f); // Purple crystal
            mr.sharedMaterial = phylacteryMat;
            phylacteryGo.transform.localScale = new Vector3(1.5f, 2f, 1.5f);

            // Physics
            var collider = phylacteryGo.AddComponent<SphereCollider>();

            // === HEALTH SYSTEM ===
            try 
            {
                var health = phylacteryGo.AddComponent<MoreMountains.TopDownEngine.Health>();
                health.InitialHealth = 200f;
                health.MaximumHealth = 200f;
                // TODO: Add death event to trigger game over
                
                Debug.Log("[QuickPlaySceneSetup] ✅ Phylactery created - Protect it or face 'Another Realm Awaits...'");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[QuickPlaySceneSetup] ⚠️ TopDown Engine Health not available for Phylactery: {e.Message}");
                // Add basic destructible behavior as fallback
                var basicHealth = phylacteryGo.AddComponent<BasicDestructible>();
                Debug.Log("[QuickPlaySceneSetup] Phylactery created with basic health fallback");
            }
        }

        private static void CreateCamera(Scene scene)
        {
            // Main Camera
            var mainCam = GameObject.Find("Main Camera");
            if (mainCam == null)
            {
                mainCam = new GameObject("Main Camera");
                SceneManager.MoveGameObjectToScene(mainCam, scene);
            }

            var cam = mainCam.GetComponent<Camera>();
            if (cam == null)
                cam = mainCam.AddComponent<Camera>();

            cam.orthographic = false;

            // Cinemachine Brain
            var brain = mainCam.GetComponent<Unity.Cinemachine.CinemachineBrain>();
            if (brain == null)
                brain = mainCam.AddComponent<Unity.Cinemachine.CinemachineBrain>();

            // Virtual Camera
            var virtualCamGo = new GameObject("VirtualCamera_Player");
            SceneManager.MoveGameObjectToScene(virtualCamGo, scene);
            virtualCamGo.transform.position = new Vector3(0, 5, -8);

            var vCam = virtualCamGo.GetComponent<Unity.Cinemachine.CinemachineCamera>();
            if (vCam == null)
                vCam = virtualCamGo.AddComponent<Unity.Cinemachine.CinemachineCamera>();

            var posComposer = virtualCamGo.GetComponent<Unity.Cinemachine.CinemachinePositionComposer>();
            if (posComposer == null)
                posComposer = virtualCamGo.AddComponent<Unity.Cinemachine.CinemachinePositionComposer>();

            var rotComposer = virtualCamGo.GetComponent<Unity.Cinemachine.CinemachineRotationComposer>();
            if (rotComposer == null)
                rotComposer = virtualCamGo.AddComponent<Unity.Cinemachine.CinemachineRotationComposer>();

            // Wire up player follow
            var player = GameObject.Find("Player");
            if (player != null)
            {
                vCam.Follow = player.transform;
                vCam.LookAt = player.transform;
            }

            Debug.Log("[QuickPlaySceneSetup] Cinemachine camera configured.");
        }
    }

    /// <summary>
    /// Minimal player controller for testing.
    /// WASD to move. Space to turn (placeholder for TurnManager integration).
    /// </summary>
    public class SimplePlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        private Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            var dir = new Vector3(x, 0, z).normalized;
            _rb.linearVelocity = new Vector3(dir.x * moveSpeed, _rb.linearVelocity.y, dir.z * moveSpeed);

            // Space = placeholder for turn submission
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var turnMgr = TurnManager.Instance;
                if (turnMgr != null)
                {
                    turnMgr.ConfirmPlayerAction();
                    Debug.Log($"[Player] Turn submitted. Current phase: {turnMgr.CurrentPhase}");
                }
            }
        }
    }

    /// <summary>
    /// Basic health component for fallback when TopDown Engine Health is not available.
    /// </summary>
    public class BasicDestructible : MonoBehaviour
    {
        [SerializeField] private float health = 200f;

        public void TakeDamage(float damage)
        {
            health -= damage;
            if (health <= 0)
            {
                Debug.Log("[BasicDestructible] Phylactery destroyed! Game Over condition triggered!");
                // TODO: Trigger game over screen
                gameObject.SetActive(false);
            }
        }
    }
}