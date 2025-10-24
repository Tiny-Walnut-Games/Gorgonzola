using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

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
            var managersGO = new GameObject("--- Managers ---");
            SceneManager.MoveGameObjectToScene(managersGO, scene);

            // TurnManager
            var turnGO = new GameObject("TurnManager");
            turnGO.transform.SetParent(managersGO.transform);
            var turnMgr = turnGO.AddComponent<TurnManager>();
            turnGO.AddComponent<AudioListener>();

            // GameManager
            var gameGO = new GameObject("GameManager");
            gameGO.transform.SetParent(managersGO.transform);
            gameGO.AddComponent<AudioListener>();
        }

        private static void CreateArenaLayout(Scene scene)
        {
            var sceneGO = new GameObject("--- Arena ---");
            SceneManager.MoveGameObjectToScene(sceneGO, scene);

            // Ground
            var groundGO = new GameObject("Ground");
            groundGO.transform.SetParent(sceneGO.transform);
            groundGO.transform.localPosition = Vector3.zero;
            groundGO.layer = LayerMask.NameToLayer("Default");

            var mf = groundGO.AddComponent<MeshFilter>();
            mf.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

            var mr = groundGO.AddComponent<MeshRenderer>();
            var groundMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            groundMat.color = new Color(0.3f, 0.5f, 0.3f);
            mr.sharedMaterial = groundMat;

            var col = groundGO.AddComponent<BoxCollider>();
            groundGO.transform.localScale = new Vector3(50, 0.5f, 50);

            // Spawn points
            var spawnsGO = new GameObject("SpawnPoints");
            spawnsGO.transform.SetParent(sceneGO.transform);

            var playerSpawn = new GameObject("PlayerSpawn");
            playerSpawn.transform.SetParent(spawnsGO.transform);
            playerSpawn.transform.localPosition = new Vector3(0, 1, 0);

            var enemySpawns = new GameObject("EnemySpawns");
            enemySpawns.transform.SetParent(spawnsGO.transform);

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
            var lightGO = GameObject.Find("Directional Light") ?? new GameObject("Directional Light");
            var light = lightGO.GetComponent<Light>();
            if (light == null)
                light = lightGO.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.5f;
            lightGO.transform.rotation = Quaternion.Euler(45, 45, 0);

            Debug.Log("[QuickPlaySceneSetup] Arena created: Ground, spawn points, lighting.");
        }

        private static void CreatePlayer(Scene scene)
        {
            // === NECROHAMSTO PLAYER WITH TOPDOWN ENGINE ===
            var playerGO = new GameObject("NecroHAMSTO");
            SceneManager.MoveGameObjectToScene(playerGO, scene);
            playerGO.transform.position = new Vector3(0, 1, 0);
            playerGO.tag = "Player";
            playerGO.layer = LayerMask.NameToLayer("Player");

            // Model hierarchy
            var modelGO = new GameObject("Model");
            modelGO.transform.SetParent(playerGO.transform);
            
            var visualGO = new GameObject("Visual");
            visualGO.transform.SetParent(modelGO.transform);

            // Visual representation
            var mf = visualGO.AddComponent<MeshFilter>();
            mf.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            var mr = visualGO.AddComponent<MeshRenderer>();
            var playerMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            playerMat.color = new Color(0.8f, 0.6f, 0.2f); // Hamster gold color
            mr.sharedMaterial = playerMat;
            visualGO.transform.localScale = new Vector3(1, 1.5f, 1); // Taller hamster

            // === TOPDOWN ENGINE COMPONENTS ===
            
            // 1. Physics: CharacterController for 3D movement
            var characterController = playerGO.AddComponent<CharacterController>();
            characterController.height = 1.8f;
            characterController.radius = 0.5f;
            characterController.center = new Vector3(0, 0.9f, 0);

            // 2. Core TopDown Engine components (try-catch to handle missing references)
            try 
            {
                // TopDown Controller for 3D
                var topDownController = playerGO.AddComponent<MoreMountains.TopDownEngine.TopDownController3D>();
                
                // Core Character component
                var character = playerGO.AddComponent<MoreMountains.TopDownEngine.Character>();
                character.CharacterModel = modelGO;
                character.PlayerID = "Player1";
                
                // Movement abilities
                var characterMovement = playerGO.AddComponent<MoreMountains.TopDownEngine.CharacterMovement>();
                characterMovement.WalkSpeed = 4f;
                characterMovement.RunSpeed = 8f;
                
                var characterOrientation = playerGO.AddComponent<MoreMountains.TopDownEngine.CharacterOrientation3D>();
                var characterRun = playerGO.AddComponent<MoreMountains.TopDownEngine.CharacterRun>();
                
                // Health system
                var health = playerGO.AddComponent<MoreMountains.TopDownEngine.Health>();
                health.InitialHealth = 100f;
                health.MaximumHealth = 100f;
                
                // Weapon handling
                var weaponHandler = playerGO.AddComponent<MoreMountains.TopDownEngine.CharacterHandleWeapon>();
                
                Debug.Log("[QuickPlaySceneSetup] ✅ NecroHAMSTO created with TopDown Engine components");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[QuickPlaySceneSetup] ⚠️ TopDown Engine components not fully available: {e.Message}");
                Debug.LogWarning("Using fallback SimplePlayerController - scene will still be playable for testing");
                
                // Fallback: Use simple controller for basic testing
                playerGO.AddComponent<SimplePlayerController>();
                
                // Basic collider for fallback
                var fallbackCol = playerGO.AddComponent<BoxCollider>();
                var fallbackRB = playerGO.AddComponent<Rigidbody>();
                fallbackRB.constraints = RigidbodyConstraints.FreezeRotation;
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
                
                var enemyGO = CreateCuteSnakeEnemy(snakeType, spawn.position, snakeColor, scene);
                enemyGO.transform.SetParent(enemiesParent.transform);
            }

            Debug.Log("[QuickPlaySceneSetup] ✅ Cute Snake enemies created with AI behavior");
        }

        private static GameObject CreateCuteSnakeEnemy(string snakeType, Vector3 position, Color color, Scene scene)
        {
            var snakeGO = new GameObject(snakeType);
            SceneManager.MoveGameObjectToScene(snakeGO, scene);
            snakeGO.transform.position = position;
            snakeGO.tag = "Enemy";
            snakeGO.layer = LayerMask.NameToLayer("Enemies");

            // === MODEL HIERARCHY ===
            var modelGO = new GameObject("Model");
            modelGO.transform.SetParent(snakeGO.transform);
            
            var visualGO = new GameObject("Visual");
            visualGO.transform.SetParent(modelGO.transform);

            // Snake-like visual (elongated capsule)
            var mf = visualGO.AddComponent<MeshFilter>();
            mf.mesh = Resources.GetBuiltinResource<Mesh>("Capsule.fbx");
            var mr = visualGO.AddComponent<MeshRenderer>();
            var snakeMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            snakeMat.color = color;
            mr.sharedMaterial = snakeMat;
            
            // Snake proportions: longer, lower
            visualGO.transform.localScale = new Vector3(0.8f, 0.5f, 2f);

            // === PHYSICS ===
            var collider = snakeGO.AddComponent<CapsuleCollider>();
            collider.height = 2f;
            collider.radius = 0.4f;
            
            var rigidbody = snakeGO.AddComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            // === TOPDOWN ENGINE COMPONENTS ===
            try 
            {
                // Controller
                var controller = snakeGO.AddComponent<MoreMountains.TopDownEngine.TopDownController3D>();
                
                // Character
                var character = snakeGO.AddComponent<MoreMountains.TopDownEngine.Character>();
                character.CharacterType = MoreMountains.TopDownEngine.Character.CharacterTypes.AI;
                character.CharacterModel = modelGO;
                
                // Movement
                var movement = snakeGO.AddComponent<MoreMountains.TopDownEngine.CharacterMovement>();
                var orientation = snakeGO.AddComponent<MoreMountains.TopDownEngine.CharacterOrientation3D>();
                
                // AI System
                var brain = snakeGO.AddComponent<MoreMountains.TopDownEngine.AIBrain>();
                var aiDecision = snakeGO.AddComponent<MoreMountains.TopDownEngine.AIDecisionDetectTargetRadius>();
                var aiAction = snakeGO.AddComponent<MoreMountains.TopDownEngine.AIActionMoveTowardsTarget>();
                
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
                var health = snakeGO.AddComponent<MoreMountains.TopDownEngine.Health>();
                health.InitialHealth = 50f;
                health.MaximumHealth = 50f;

                // Damage on contact
                var damageOnTouch = snakeGO.AddComponent<MoreMountains.TopDownEngine.DamageOnTouch>();
                damageOnTouch.DamageCaused = 10f;
                
                Debug.Log($"[QuickPlaySceneSetup] ✅ {snakeType} created with TopDown Engine AI");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[QuickPlaySceneSetup] ⚠️ TopDown Engine AI components not available for {snakeType}: {e.Message}");
                Debug.LogWarning("Snake will be static but scene is still testable");
            }

            return snakeGO;
        }

        private static void CreatePhylactery(Scene scene)
        {
            // === THE PHYLACTERY - GAME OVER CONDITION ===
            var phylacteryGO = new GameObject("Phylactery");
            SceneManager.MoveGameObjectToScene(phylacteryGO, scene);
            phylacteryGO.transform.position = new Vector3(8, 1, 8); // Corner position
            phylacteryGO.tag = "Phylactery";

            // Visual: Crystal-like appearance
            var mf = phylacteryGO.AddComponent<MeshFilter>();
            mf.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            var mr = phylacteryGO.AddComponent<MeshRenderer>();
            var phylacteryMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            phylacteryMat.color = new Color(0.5f, 0.2f, 0.8f, 0.8f); // Purple crystal
            mr.sharedMaterial = phylacteryMat;
            phylacteryGO.transform.localScale = new Vector3(1.5f, 2f, 1.5f);

            // Physics
            var collider = phylacteryGO.AddComponent<SphereCollider>();

            // === HEALTH SYSTEM ===
            try 
            {
                var health = phylacteryGO.AddComponent<MoreMountains.TopDownEngine.Health>();
                health.InitialHealth = 200f;
                health.MaximumHealth = 200f;
                // TODO: Add death event to trigger game over
                
                Debug.Log("[QuickPlaySceneSetup] ✅ Phylactery created - Protect it or face 'Another Realm Awaits...'");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[QuickPlaySceneSetup] ⚠️ TopDown Engine Health not available for Phylactery: {e.Message}");
                // Add basic destructible behavior as fallback
                var basicHealth = phylacteryGO.AddComponent<BasicDestructible>();
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
            var virtualCamGO = new GameObject("VirtualCamera_Player");
            SceneManager.MoveGameObjectToScene(virtualCamGO, scene);
            virtualCamGO.transform.position = new Vector3(0, 5, -8);

            var vCam = virtualCamGO.GetComponent<Unity.Cinemachine.CinemachineCamera>();
            if (vCam == null)
                vCam = virtualCamGO.AddComponent<Unity.Cinemachine.CinemachineCamera>();

            var posComposer = virtualCamGO.GetComponent<Unity.Cinemachine.CinemachinePositionComposer>();
            if (posComposer == null)
                posComposer = virtualCamGO.AddComponent<Unity.Cinemachine.CinemachinePositionComposer>();

            var rotComposer = virtualCamGO.GetComponent<Unity.Cinemachine.CinemachineRotationComposer>();
            if (rotComposer == null)
                rotComposer = virtualCamGO.AddComponent<Unity.Cinemachine.CinemachineRotationComposer>();

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
        private Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            var dir = new Vector3(x, 0, z).normalized;
            rb.linearVelocity = new Vector3(dir.x * moveSpeed, rb.linearVelocity.y, dir.z * moveSpeed);

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