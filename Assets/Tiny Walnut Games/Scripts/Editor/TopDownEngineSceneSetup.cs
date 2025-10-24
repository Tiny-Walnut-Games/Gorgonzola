using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using MoreMountains.TopDownEngine;
using Unity.Cinemachine;

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
            CreateNecroHAMSTOPlayer(scene);
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
            var managersGO = new GameObject("--- MANAGERS ---");
            SceneManager.MoveGameObjectToScene(managersGO, scene);

            // TurnManager (custom system)
            var turnGO = new GameObject("TurnManager");
            turnGO.transform.SetParent(managersGO.transform);
            var turnManager = turnGO.AddComponent<TurnManager>();

            // LevelManager (TopDown Engine)
            var levelMgrGO = new GameObject("LevelManager");
            levelMgrGO.transform.SetParent(managersGO.transform);
            var levelMgr = levelMgrGO.AddComponent<LevelManager>();
            levelMgr.DelayBeforeDestruction = 2f;

            // GameManager (TopDown Engine core)
            var gameMgrGO = new GameObject("GameManager");
            gameMgrGO.transform.SetParent(managersGO.transform);
            var gameMgr = gameMgrGO.AddComponent<GameManager>();
            gameMgr.GameOverScreen = null; // Will be set after UI creation

            // InputManager 
            var inputMgrGO = new GameObject("InputManager");
            inputMgrGO.transform.SetParent(managersGO.transform);
            var inputMgr = inputMgrGO.AddComponent<InputManager>();

            // SoundManager
            var soundMgrGO = new GameObject("SoundManager");
            soundMgrGO.transform.SetParent(managersGO.transform);
            var soundMgr = soundMgrGO.AddComponent<SoundManager>();

            Debug.Log("[TopDownEngineSceneSetup] Managers created: Turn, Level, Game, Input, Sound");
        }

        private static void CreateArenaEnvironment(Scene scene)
        {
            // === ARENA HIERARCHY ===
            var arenaGO = new GameObject("--- ARENA ---");
            SceneManager.MoveGameObjectToScene(arenaGO, scene);

            // Ground plane
            var groundGO = new GameObject("Ground");
            groundGO.transform.SetParent(arenaGO.transform);
            groundGO.layer = LayerMask.NameToLayer("Default");
            
            var groundMF = groundGO.AddComponent<MeshFilter>();
            groundMF.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            
            var groundMR = groundGO.AddComponent<MeshRenderer>();
            var groundMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            groundMat.color = new Color(0.2f, 0.4f, 0.2f); // Dark green
            groundMR.sharedMaterial = groundMat;
            
            var groundCol = groundGO.AddComponent<BoxCollider>();
            groundGO.transform.localScale = new Vector3(50, 0.5f, 50);
            groundGO.transform.position = new Vector3(0, -0.25f, 0);

            // Spawn points container
            var spawnsGO = new GameObject("SpawnPoints");
            spawnsGO.transform.SetParent(arenaGO.transform);

            // Player spawn
            var playerSpawnGO = new GameObject("PlayerSpawn");
            playerSpawnGO.transform.SetParent(spawnsGO.transform);
            playerSpawnGO.transform.position = new Vector3(0, 1, 0);

            // Enemy spawns (corners of arena)
            var enemySpawnsGO = new GameObject("EnemySpawns");
            enemySpawnsGO.transform.SetParent(spawnsGO.transform);
            
            Vector3[] enemyPositions = {
                new Vector3(15, 1, 15),   // Top-right
                new Vector3(-15, 1, 15),  // Top-left
                new Vector3(15, 1, -15),  // Bottom-right
                new Vector3(-15, 1, -15)  // Bottom-left
            };

            for (int i = 0; i < enemyPositions.Length; i++)
            {
                var spawn = new GameObject($"EnemySpawn_{i}");
                spawn.transform.SetParent(enemySpawnsGO.transform);
                spawn.transform.position = enemyPositions[i];
            }

            // Phylactery spawn (center-ish, offset from player)
            var phylacterySpawnGO = new GameObject("PhylacterySpawn");
            phylacterySpawnGO.transform.SetParent(spawnsGO.transform);
            phylacterySpawnGO.transform.position = new Vector3(5, 1, 5);

            Debug.Log("[TopDownEngineSceneSetup] Arena environment created: Ground, spawn points");
        }

        private static void CreateNecroHAMSTOPlayer(Scene scene)
        {
            // === NECROHAMSTO PLAYER ===
            var playerGO = new GameObject("NecroHAMSTO");
            SceneManager.MoveGameObjectToScene(playerGO, scene);
            playerGO.transform.position = new Vector3(0, 1, 0);
            playerGO.tag = "Player";
            playerGO.layer = LayerMask.NameToLayer("Player");

            // Model container
            var modelGO = new GameObject("Model");
            modelGO.transform.SetParent(playerGO.transform);
            modelGO.layer = LayerMask.NameToLayer("Player");

            // Visual representation (cube for now - replace with hamster model later)
            var visualGO = new GameObject("Visual");
            visualGO.transform.SetParent(modelGO.transform);
            
            var mf = visualGO.AddComponent<MeshFilter>();
            mf.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            
            var mr = visualGO.AddComponent<MeshRenderer>();
            var playerMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            playerMat.color = new Color(0.8f, 0.6f, 0.2f); // Golden hamster color
            mr.sharedMaterial = playerMat;
            
            visualGO.transform.localScale = new Vector3(1, 1.5f, 1);

            // === TOPDOWN ENGINE COMPONENTS ===

            // 1. Physics (CharacterController for 3D)
            var characterController = playerGO.AddComponent<CharacterController>();
            characterController.height = 1.8f;
            characterController.radius = 0.5f;
            characterController.center = new Vector3(0, 0.9f, 0);

            // 2. TopDownController3D (core movement)
            var topDownController = playerGO.AddComponent<TopDownController3D>();
            topDownController.Gravity = 20f;

            // 3. Character (TopDown Engine core)
            var character = playerGO.AddComponent<Character>();
            character.CharacterType = Character.CharacterTypes.Player;
            character.PlayerID = "Player1";
            character.CharacterModel = modelGO;

            // 4. CharacterMovement (handles input and movement)
            var characterMovement = playerGO.AddComponent<CharacterMovement>();
            characterMovement.WalkSpeed = 4f;
            characterMovement.RunSpeed = 8f;

            // 5. CharacterOrientation3D (handles facing direction)
            var characterOrientation = playerGO.AddComponent<CharacterOrientation3D>();

            // 6. CharacterRun (allows running)
            var characterRun = playerGO.AddComponent<CharacterRun>();

            // 7. Health system
            var health = playerGO.AddComponent<Health>();
            health.InitialHealth = 100f;
            health.MaximumHealth = 100f;

            // 8. CharacterHandleWeapon (for attacks)
            var weaponHandler = playerGO.AddComponent<CharacterHandleWeapon>();

            // 9. InputManager binding (if using InputSystem)
            var inputManager = playerGO.AddComponent<InputManager>();

            // === WEAPON SYSTEM ===
            CreatePlayerWeapon(playerGO);

            Debug.Log("[TopDownEngineSceneSetup] NecroHAMSTO player created with full TopDown Engine components");
        }

        private static void CreatePlayerWeapon(GameObject player)
        {
            // Create weapon holder
            var weaponHolderGO = new GameObject("WeaponAttachment");
            weaponHolderGO.transform.SetParent(player.transform);
            weaponHolderGO.transform.localPosition = new Vector3(0, 1, 0.5f);

            // Create sunflower seed projectile weapon
            var weaponGO = new GameObject("SunflowerSeedLauncher");
            weaponGO.transform.SetParent(weaponHolderGO.transform);
            weaponGO.transform.localPosition = Vector3.zero;

            // ProjectileWeapon component
            var weapon = weaponGO.AddComponent<ProjectileWeapon>();
            weapon.WeaponName = "Sunflower Seed Launcher";
            weapon.TimeBetweenUses = 0.5f;
            weapon.MagazineSize = 999; // Infinite ammo for jam
            
            // Create projectile spawn point
            var spawnPointGO = new GameObject("ProjectileSpawn");
            spawnPointGO.transform.SetParent(weaponGO.transform);
            spawnPointGO.transform.localPosition = new Vector3(0, 0, 1);
            
            weapon.WeaponAttachment = spawnPointGO.transform;

            // Weapon aim (for targeting)
            var weaponAim = weaponGO.AddComponent<WeaponAim3D>();

            Debug.Log("[TopDownEngineSceneSetup] Player weapon system created");
        }

        private static void CreateSnakeEnemies(Scene scene)
        {
            // === SNAKE ENEMIES ===
            var enemiesGO = new GameObject("--- ENEMIES ---");
            SceneManager.MoveGameObjectToScene(enemiesGO, scene);

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
                var snakeGO = CreateSnakeEnemy(snakeType, spawn.position, snakeColors[i]);
                snakeGO.transform.SetParent(enemiesGO.transform);
                SceneManager.MoveGameObjectToScene(snakeGO, scene);
            }

            Debug.Log("[TopDownEngineSceneSetup] Snake enemies created with TopDown Engine AI");
        }

        private static GameObject CreateSnakeEnemy(string snakeType, Vector3 position, Color color)
        {
            var snakeGO = new GameObject(snakeType);
            snakeGO.transform.position = position;
            snakeGO.tag = "Enemy";
            snakeGO.layer = LayerMask.NameToLayer("Enemies");

            // Visual model
            var modelGO = new GameObject("Model");
            modelGO.transform.SetParent(snakeGO.transform);
            
            var visualGO = new GameObject("Visual");
            visualGO.transform.SetParent(modelGO.transform);
            
            var mf = visualGO.AddComponent<MeshFilter>();
            mf.mesh = Resources.GetBuiltinResource<Mesh>("Capsule.fbx");
            
            var mr = visualGO.AddComponent<MeshRenderer>();
            var snakeMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            snakeMat.color = color;
            mr.sharedMaterial = snakeMat;
            
            visualGO.transform.localScale = new Vector3(0.8f, 0.5f, 2f); // Snake-like proportions

            // Physics
            var rigidbody = snakeGO.AddComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            
            var collider = snakeGO.AddComponent<CapsuleCollider>();
            collider.height = 2f;
            collider.radius = 0.4f;

            // TopDown Engine components
            var controller = snakeGO.AddComponent<TopDownController3D>();
            controller.Gravity = 20f;
            
            var character = snakeGO.AddComponent<Character>();
            character.CharacterType = Character.CharacterTypes.AI;
            character.CharacterModel = modelGO;
            
            var movement = snakeGO.AddComponent<CharacterMovement>();
            var orientation = snakeGO.AddComponent<CharacterOrientation3D>();

            // AI Brain based on snake type
            var brain = snakeGO.AddComponent<AIBrain>();
            var aiDecision = snakeGO.AddComponent<AIDecisionDetectTargetRadius>();
            var aiAction = snakeGO.AddComponent<AIActionMoveTowardsTarget>();
            
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
            var health = snakeGO.AddComponent<Health>();
            health.InitialHealth = 50f;
            health.MaximumHealth = 50f;

            // Damage on touch
            var damageOnTouch = snakeGO.AddComponent<DamageOnTouch>();
            damageOnTouch.DamageCaused = 10f;

            return snakeGO;
        }

        private static void CreatePhylactery(Scene scene)
        {
            // === PHYLACTERY (LOSE CONDITION) ===
            var phylacteryGO = new GameObject("Phylactery");
            SceneManager.MoveGameObjectToScene(phylacteryGO, scene);
            phylacteryGO.transform.position = new Vector3(5, 1, 5);
            phylacteryGO.tag = "Phylactery";

            // Visual (crystal-like)
            var mf = phylacteryGO.AddComponent<MeshFilter>();
            mf.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            
            var mr = phylacteryGO.AddComponent<MeshRenderer>();
            var phylacteryMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            phylacteryMat.color = new Color(0.5f, 0.2f, 0.8f); // Purple crystal
            mr.sharedMaterial = phylacteryMat;
            
            phylacteryGO.transform.localScale = new Vector3(1.5f, 2f, 1.5f);

            // Collider
            var collider = phylacteryGO.AddComponent<SphereCollider>();

            // Health (game over when destroyed)
            var health = phylacteryGO.AddComponent<Health>();
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
            var virtualCamGO = new GameObject("CM_PlayerFollow");
            SceneManager.MoveGameObjectToScene(virtualCamGO, scene);
            virtualCamGO.transform.position = new Vector3(0, 10, -8);
            virtualCamGO.transform.rotation = Quaternion.Euler(45, 0, 0);

            var virtualCam = virtualCamGO.AddComponent<CinemachineCamera>();
            virtualCam.Priority = 10;

            // Follow player
            var player = GameObject.Find("NecroHAMSTO");
            if (player != null)
            {
                virtualCam.Follow = player.transform;
                virtualCam.LookAt = player.transform;
            }

            // Position composer for smooth following
            var positionComposer = virtualCamGO.AddComponent<CinemachinePositionComposer>();
            positionComposer.ScreenPosition = new Vector2(0.5f, 0.4f); // Slightly below center

            // Rotation composer
            var rotationComposer = virtualCamGO.AddComponent<CinemachineRotationComposer>();

            Debug.Log("[TopDownEngineSceneSetup] Cinemachine camera system created");
        }

        private static void CreateUI(Scene scene)
        {
            // === UI SYSTEM ===
            var canvasGO = new GameObject("Canvas");
            SceneManager.MoveGameObjectToScene(canvasGO, scene);
            
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            var canvasScaler = canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasScaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);

            var graphicRaycaster = canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            // Turn indicator
            var turnIndicatorGO = new GameObject("TurnIndicator");
            turnIndicatorGO.transform.SetParent(canvasGO.transform);
            
            var turnText = turnIndicatorGO.AddComponent<UnityEngine.UI.Text>();
            turnText.text = "PLAYER TURN - Press SPACE to confirm";
            turnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            turnText.fontSize = 24;
            turnText.color = Color.white;
            turnText.alignment = TextAnchor.UpperCenter;
            
            var turnRect = turnIndicatorGO.GetComponent<RectTransform>();
            turnRect.anchorMin = new Vector2(0, 1);
            turnRect.anchorMax = new Vector2(1, 1);
            turnRect.anchoredPosition = new Vector2(0, -50);
            turnRect.sizeDelta = new Vector2(0, 50);

            // Health bar placeholder
            var healthBarGO = new GameObject("HealthBar");
            healthBarGO.transform.SetParent(canvasGO.transform);
            // TODO: Implement health bar UI

            Debug.Log("[TopDownEngineSceneSetup] Basic UI created");
        }
    }
}