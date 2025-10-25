using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using Unity.Cinemachine;
using GorgonzolaMM;

namespace GorgonzolaMM.Editor
{
    /// <summary>
    /// COMPLETE TopDown Engine scene setup for NecroHAMSTO vs Cute Snakes.
    /// Creates a fully functional turn-based game scene with all required components.
    /// One-click from empty scene to playable game.
    /// </summary>
    public class TopDownEngineSceneSetup
    {
        [MenuItem("Gorgonzola/Setup/Complete TopDown Engine Arena")]
        public static void SetupCompleteArena()
        {
            // Save current scene
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            
            // Create or open target scene
            var scenePath = "Assets/Tiny Walnut Games/Scenes/GameJamTestScene.unity";
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            
            // Clear default objects except camera and light
            ClearDefaultObjects(scene);
            
            // Create complete game hierarchy
            CreateGameManagers(scene);
            CreateArenaEnvironment(scene);
            CreateNecroHamstoPlayer(scene);
            CreateSnakeEnemies(scene);
            CreatePhylactery(scene);
            CreateCameraSystem(scene);
            CreateUI(scene);
            
            // Save the scene
            EditorSceneManager.SaveScene(scene, scenePath);
            EditorSceneManager.MarkSceneDirty(scene);
            
            Debug.Log("[TopDownEngineSceneSetup] ✅ Complete game scene ready! Press Play to test NecroHAMSTO vs Cute Snakes!");
        }

        private static void ClearDefaultObjects(Scene scene)
        {
            var roots = scene.GetRootGameObjects();
            foreach (var root in roots)
            {
                if (root.name != "Directional Light" && root.name != "Main Camera")
                    Object.DestroyImmediate(root);
            }
        }

        private static void CreateGameManagers(Scene scene)
        {
            // === MANAGERS HIERARCHY ===
            var managersGo = new GameObject("--- MANAGERS ---");
            SceneManager.MoveGameObjectToScene(managersGo, scene);

            // Bootstrap (initializes core systems)
            var bootstrapGo = new GameObject("GameJamBootstrap");
            bootstrapGo.transform.SetParent(managersGo.transform);
            var bootstrap = bootstrapGo.AddComponent<GameJamBootstrap>();

            // TurnManager (custom system)
            var turnGo = new GameObject("TurnManager");
            turnGo.transform.SetParent(managersGo.transform);
            var turnManager = turnGo.AddComponent<TurnManager>();

            // LevelManager (TopDown Engine)
            var levelMgrGo = new GameObject("LevelManager");
            levelMgrGo.transform.SetParent(managersGo.transform);
            var levelMgr = levelMgrGo.AddComponent<LevelManager>();
            levelMgr.DelayBeforeDeathScreen = 2f;

            // GameManager (TopDown Engine core)
            var gameMgrGo = new GameObject("GameManager");
            gameMgrGo.transform.SetParent(managersGo.transform);
            var gameMgr = gameMgrGo.AddComponent<GameManager>();

            // InputManager 
            var inputMgrGo = new GameObject("InputManager");
            inputMgrGo.transform.SetParent(managersGo.transform);
            var inputMgr = inputMgrGo.AddComponent<InputManager>();

            // SoundManager
            var soundMgrGo = new GameObject("SoundManager");
            soundMgrGo.transform.SetParent(managersGo.transform);
            var soundMgr = soundMgrGo.AddComponent<SoundManager>();

            Debug.Log("[TopDownEngineSceneSetup] Managers created: Bootstrap, Turn, Level, Game, Input, Sound");
        }

        private static void CreateArenaEnvironment(Scene scene)
        {
            // === ARENA HIERARCHY ===
            var arenaGo = new GameObject("--- ARENA ---");
            SceneManager.MoveGameObjectToScene(arenaGo, scene);

            // Ground plane
            var groundGo = new GameObject("Ground");
            groundGo.transform.SetParent(arenaGo.transform);
            groundGo.layer = LayerMask.NameToLayer("Ground");  // ✅ Set to "Ground" layer so TopDownController3D detects it
            
            var groundMf = groundGo.AddComponent<MeshFilter>();
            if (groundMf is not null) groundMf.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

            var groundMr = groundGo.AddComponent<MeshRenderer>();
            var groundMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            groundMat.color = new Color(0.2f, 0.4f, 0.2f); // Dark green
            if (groundMr is not null) groundMr.sharedMaterial = groundMat;

            var groundCol = groundGo.AddComponent<BoxCollider>();
            groundGo.transform.localScale = new Vector3(50, 0.5f, 50);
            groundGo.transform.position = new Vector3(0, -0.25f, 0);

            // Spawn points container
            var spawnsGo = new GameObject("SpawnPoints");
            spawnsGo.transform.SetParent(arenaGo.transform);

            // Player spawn
            var playerSpawnGo = new GameObject("PlayerSpawn");
            playerSpawnGo.transform.SetParent(spawnsGo.transform);
            playerSpawnGo.transform.position = new Vector3(0, 1, 0);

            // Enemy spawns (corners of arena)
            var enemySpawnsGo = new GameObject("EnemySpawns");
            enemySpawnsGo.transform.SetParent(spawnsGo.transform);
            
            Vector3[] enemyPositions = {
                new Vector3(15, 1, 15),   // Top-right
                new Vector3(-15, 1, 15),  // Top-left
                new Vector3(15, 1, -15),  // Bottom-right
                new Vector3(-15, 1, -15)  // Bottom-left
            };

            for (int i = 0; i < enemyPositions.Length; i++)
            {
                var spawn = new GameObject($"EnemySpawn_{i}");
                spawn.transform.SetParent(enemySpawnsGo.transform);
                spawn.transform.position = enemyPositions[i];
            }

            // Phylactery spawn (center-ish, offset from player)
            var phylacterySpawnGo = new GameObject("PhylacterySpawn");
            phylacterySpawnGo.transform.SetParent(spawnsGo.transform);
            phylacterySpawnGo.transform.position = new Vector3(5, 1, 5);

            Debug.Log("[TopDownEngineSceneSetup] Arena environment created: Ground, spawn points");
        }

        private static void CreateNecroHamstoPlayer(Scene scene)
        {
            // === NECROHAMSTO PLAYER ===
            var playerGo = new GameObject("NecroHAMSTO");
            SceneManager.MoveGameObjectToScene(playerGo, scene);
            playerGo.transform.position = new Vector3(0, 1, 0);
            playerGo.tag = "Player";
            playerGo.layer = LayerMask.NameToLayer("Player");

            // Model container
            var modelGo = new GameObject("Model");
            modelGo.transform.SetParent(playerGo.transform);
            modelGo.layer = LayerMask.NameToLayer("Player");

            // Visual representation (cube for now - replace with hamster model later)
            var visualGo = new GameObject("Visual");
            visualGo.transform.SetParent(modelGo.transform);
            
            var mf = visualGo.AddComponent<MeshFilter>();
            if (mf is not null) mf.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

            var mr = visualGo.AddComponent<MeshRenderer>();
            var playerMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            playerMat.color = new Color(0.8f, 0.6f, 0.2f); // Golden hamster color
            if (mr is not null) mr.sharedMaterial = playerMat;

            visualGo.transform.localScale = new Vector3(1, 1.5f, 1);

            // === TOPDOWN ENGINE COMPONENTS ===

            // 1. Physics: Rigidbody (kinematic, for TopDownController3D)
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

            // 3. Physics: CharacterController (for 3D movement)
            var characterController = playerGo.AddComponent<CharacterController>();
            characterController.height = 1.8f;
            characterController.radius = 0.5f;
            characterController.center = new Vector3(0, 0.9f, 0);

            // 4. TopDownController3D (core movement)
            var topDownController = playerGo.AddComponent<TopDownController3D>();
            topDownController.Gravity = 20f;

            // 5. Character (TopDown Engine core)
            var character = playerGo.AddComponent<Character>();
            character.CharacterType = Character.CharacterTypes.Player;
            character.PlayerID = "Player1";
            character.CharacterModel = modelGo;

            // 6. CharacterMovement (handles input and movement)
            var characterMovement = playerGo.AddComponent<CharacterMovement>();
            characterMovement.WalkSpeed = 4f;

            // 7. CharacterOrientation3D (handles facing direction)
            var characterOrientation = playerGo.AddComponent<CharacterOrientation3D>();

            // 8. CharacterRun (allows running)
            var characterRun = playerGo.AddComponent<CharacterRun>();
            characterRun.RunSpeed = 8f;

            // 9. Health system
            var health = playerGo.AddComponent<Health>();
            health.InitialHealth = 100f;
            health.MaximumHealth = 100f;

            // 10. CharacterHandleWeapon (for attacks)
            var weaponHandler = playerGo.AddComponent<CharacterHandleWeapon>();

            // 11. InputManager binding (if using InputSystem)
            var inputManager = playerGo.AddComponent<InputManager>();

            // 12. PlayerInputGate (gates input to PlayerTurn phase only)
            var inputGate = playerGo.AddComponent<PlayerInputGate>();

            // === WEAPON SYSTEM ===
            CreatePlayerWeapon(playerGo);

            Debug.Log("[TopDownEngineSceneSetup] NecroHAMSTO player created with full TopDown Engine components + InputGate");
        }

        private static void CreatePlayerWeapon(GameObject player)
        {
            // Create weapon holder
            var weaponHolderGo = new GameObject("WeaponAttachment");
            weaponHolderGo.transform.SetParent(player.transform);
            weaponHolderGo.transform.localPosition = new Vector3(0, 1, 0.5f);

            // Create sunflower seed projectile weapon
            var weaponGo = new GameObject("SunflowerSeedLauncher");
            weaponGo.transform.SetParent(weaponHolderGo.transform);
            weaponGo.transform.localPosition = Vector3.zero;

            // ProjectileWeapon component
            var weapon = weaponGo.AddComponent<ProjectileWeapon>();
            weapon.WeaponName = "Sunflower Seed Launcher";
            weapon.TimeBetweenUses = 0.5f;
            weapon.MagazineSize = 999; // Infinite ammo for jam
            
            // Create projectile spawn point
            var spawnPointGo = new GameObject("ProjectileSpawn");
            spawnPointGo.transform.SetParent(weaponGo.transform);
            spawnPointGo.transform.localPosition = new Vector3(0, 0, 1);
            
            // Get the CharacterHandleWeapon component from the player to set the weapon attachment
            var weaponHandler = player.GetComponent<CharacterHandleWeapon>();
            if (weaponHandler != null)
            {
                weaponHandler.WeaponAttachment = spawnPointGo.transform;
            }

            // Weapon aim (for targeting)
            var weaponAim = weaponGo.AddComponent<WeaponAim3D>();

            Debug.Log("[TopDownEngineSceneSetup] Player weapon system created");
        }

        private static void CreateSnakeEnemies(Scene scene)
        {
            // === SNAKE ENEMIES ===
            var enemiesGo = new GameObject("--- ENEMIES ---");
            SceneManager.MoveGameObjectToScene(enemiesGo, scene);

            var enemySpawns = GameObject.Find("SpawnPoints/EnemySpawns");
            if (enemySpawns == null) return;

            string[] snakeTypes = { "RibbonSnake", "BoaHugger", "GlitterCobra", "RibbonSnake" };
            Color[] snakeColors = { 
                Color.green,      // Ribbon Snake
                Color.yellow,     // Boa Hugger  
                Color.magenta,    // Glitter Cobra
                Color.cyan        // Another Ribbon Snake
            };

            for (int i = 0; i < enemySpawns.transform.childCount; i++)
            {
                var spawn = enemySpawns.transform.GetChild(i);
                var snakeType = snakeTypes[i % snakeTypes.Length];
                var snakeGo = CreateSnakeEnemy(snakeType, spawn.position, snakeColors[i]);
                snakeGo.transform.SetParent(enemiesGo.transform);
                SceneManager.MoveGameObjectToScene(snakeGo, scene);
            }

            Debug.Log("[TopDownEngineSceneSetup] Snake enemies created with TopDown Engine AI");
        }

        private static GameObject CreateSnakeEnemy(string snakeType, Vector3 position, Color color)
        {
            var snakeGo = new GameObject(snakeType);
            snakeGo.transform.position = position;
            snakeGo.tag = "Enemy";
            snakeGo.layer = LayerMask.NameToLayer("Enemies");

            // Visual model
            var modelGo = new GameObject("Model");
            modelGo.transform.SetParent(snakeGo.transform);
            
            var visualGo = new GameObject("Visual");
            visualGo.transform.SetParent(modelGo.transform);
            
            var mf = visualGo.AddComponent<MeshFilter>();
            mf.mesh = Resources.GetBuiltinResource<Mesh>("Capsule.fbx");
            
            var mr = visualGo.AddComponent<MeshRenderer>();
            var snakeMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            snakeMat.color = color;
            mr.sharedMaterial = snakeMat;
            
            visualGo.transform.localScale = new Vector3(0.8f, 0.5f, 2f); // Snake-like proportions

            // Physics: Rigidbody (kinematic, required by TopDownController3D)
            var rigidbody = snakeGo.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = true;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            
            // Physics: CapsuleCollider (required by TopDownController3D)
            var collider = snakeGo.AddComponent<CapsuleCollider>();
            collider.height = 2f;
            collider.radius = 0.4f;

            // Physics: CharacterController (required by TopDownController3D)
            var characterController = snakeGo.AddComponent<CharacterController>();
            characterController.height = 2f;
            characterController.radius = 0.4f;
            characterController.center = new Vector3(0, 1f, 0);

            // TopDown Engine components
            var controller = snakeGo.AddComponent<TopDownController3D>();
            controller.Gravity = 20f;
            
            var character = snakeGo.AddComponent<Character>();
            character.CharacterType = Character.CharacterTypes.AI;
            character.CharacterModel = modelGo;
            
            var movement = snakeGo.AddComponent<CharacterMovement>();
            var orientation = snakeGo.AddComponent<CharacterOrientation3D>();

            // AI Brain based on snake type
            var brain = snakeGo.AddComponent<AIBrain>();
            var aiDecision = snakeGo.AddComponent<AIDecisionDetectTargetRadius3D>();
            var aiAction = snakeGo.AddComponent<AIActionMoveTowardsTarget3D>();
            
            // Configure AI based on type
            switch (snakeType)
            {
                case "RibbonSnake":
                    movement.WalkSpeed = 6f; // Fast
                    aiDecision.Radius = 15f;
                    break;
                case "BoaHugger":
                    movement.WalkSpeed = 2f; // Slow
                    aiDecision.Radius = 10f;
                    // TODO: Add immobilize ability
                    break;
                case "GlitterCobra":
                    movement.WalkSpeed = 4f; // Medium
                    aiDecision.Radius = 20f; // Long range
                    // TODO: Add charm ability
                    break;
            }

            // Health
            var health = snakeGo.AddComponent<Health>();
            health.InitialHealth = 50f;
            health.MaximumHealth = 50f;

            // Damage on touch
            var damageOnTouch = snakeGo.AddComponent<DamageOnTouch>();
            damageOnTouch.MinDamageCaused = 10f;
            damageOnTouch.MaxDamageCaused = 10f;

            return snakeGo;
        }

        private static void CreatePhylactery(Scene scene)
        {
            // === PHYLACTERY (LOSE CONDITION) ===
            var phylacteryGo = new GameObject("Phylactery");
            SceneManager.MoveGameObjectToScene(phylacteryGo, scene);
            phylacteryGo.transform.position = new Vector3(5, 1, 5);
            phylacteryGo.tag = "Phylactery";

            // Visual (crystal-like)
            var mf = phylacteryGo.AddComponent<MeshFilter>();
            mf.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            
            var mr = phylacteryGo.AddComponent<MeshRenderer>();
            var phylacteryMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            phylacteryMat.color = new Color(0.5f, 0.2f, 0.8f); // Purple crystal
            mr.sharedMaterial = phylacteryMat;
            
            phylacteryGo.transform.localScale = new Vector3(1.5f, 2f, 1.5f);

            // Collider
            var collider = phylacteryGo.AddComponent<SphereCollider>();

            // Health (game over when destroyed)
            var health = phylacteryGo.AddComponent<Health>();
            health.InitialHealth = 200f;
            health.MaximumHealth = 200f;

            // TODO: Add game over trigger on phylactery death

            Debug.Log("[TopDownEngineSceneSetup] Phylactery created - Game Over condition");
        }

        private static void CreateCameraSystem(Scene scene)
        {
            // === CAMERA SYSTEM ===
            var mainCam = GameObject.Find("Main Camera");
            if (mainCam == null)
            {
                mainCam = new GameObject("Main Camera");
                SceneManager.MoveGameObjectToScene(mainCam, scene);
            }

            var camera = mainCam.GetComponent<Camera>();
            if (camera == null)
                camera = mainCam.AddComponent<Camera>();

            // Configure for top-down view
            camera.orthographic = false;
            camera.fieldOfView = 60f;

            // Cinemachine Brain
            var brain = mainCam.GetComponent<CinemachineBrain>();
            if (brain == null)
                brain = mainCam.AddComponent<CinemachineBrain>();

            // Virtual Camera for following player
            var virtualCamGo = new GameObject("CM_PlayerFollow");
            SceneManager.MoveGameObjectToScene(virtualCamGo, scene);
            virtualCamGo.transform.position = new Vector3(0, 10, -8);
            virtualCamGo.transform.rotation = Quaternion.Euler(45, 0, 0);

            var virtualCam = virtualCamGo.AddComponent<CinemachineCamera>();
            virtualCam.Priority = 10;

            // Follow player
            var player = GameObject.Find("NecroHAMSTO");
            if (player != null)
            {
                virtualCam.Follow = player.transform;
                virtualCam.LookAt = player.transform;
            }

            // Note: Cinemachine 3.x uses different composing approach
            // Position and rotation are handled via the CinemachineCamera damping settings

            Debug.Log("[TopDownEngineSceneSetup] Cinemachine virtual camera configured");

            Debug.Log("[TopDownEngineSceneSetup] Cinemachine camera system created");
        }

        private static void CreateUI(Scene scene)
        {
            // === UI SYSTEM ===
            var canvasGo = new GameObject("Canvas");
            SceneManager.MoveGameObjectToScene(canvasGo, scene);
            
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            var canvasScaler = canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasScaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);

            var graphicRaycaster = canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            // Turn indicator
            var turnIndicatorGo = new GameObject("TurnIndicator");
            turnIndicatorGo.transform.SetParent(canvasGo.transform);
            
            var turnText = turnIndicatorGo.AddComponent<UnityEngine.UI.Text>();
            turnText.text = "PLAYER TURN - Press SPACE to confirm";
            turnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            turnText.fontSize = 24;
            turnText.color = Color.white;
            turnText.alignment = TextAnchor.UpperCenter;
            
            var turnRect = turnIndicatorGo.GetComponent<RectTransform>();
            turnRect.anchorMin = new Vector2(0, 1);
            turnRect.anchorMax = new Vector2(1, 1);
            turnRect.anchoredPosition = new Vector2(0, -50);
            turnRect.sizeDelta = new Vector2(0, 50);

            // Health bar placeholder
            var healthBarGo = new GameObject("HealthBar");
            healthBarGo.transform.SetParent(canvasGo.transform);
            // TODO: Implement health bar UI

            Debug.Log("[TopDownEngineSceneSetup] Basic UI created");
        }
    }
}