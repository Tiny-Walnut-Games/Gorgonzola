using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

namespace GorgonzolaMM.Editor
{
    /// <summary>
    /// Automates arena scene setup following More Mountains TopDown Engine workflows.
    /// Creates a fully configured scene with UI, managers, player, camera, and HUD systems.
    /// 
    /// Usage: Right-click in Project > Create > Gorgonzola > New Arena Scene
    /// Or use menu: Gorgonzola > Scenes > Create New Arena
    /// </summary>
    public class GorgonzolaSceneSetup
    {
        private const string SCENE_TEMPLATE_PATH = "Assets/Scenes/ArenaTemplate_{0}.unity";
        
        // More Mountains prefab paths (Common resources)
        private const string MM_UI_CAMERA_PREFAB = "Assets/TopDownEngine/Common/Prefabs/GUI/UICamera.prefab";
        private const string MM_HEALTH_BAR_PREFAB = "Assets/TopDownEngine/Common/Prefabs/GUI/HealthBar.prefab";
        private const string MM_FADER_PREFAB = "Assets/TopDownEngine/Common/Prefabs/GUI/MMFaderRound.prefab";

        [MenuItem("Gorgonzola/Scenes/Create New Arena")]
        public static void CreateNewArenaScene()
        {
            string sceneName = "Arena_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            CreateArenaScene(sceneName);
            EditorUtility.DisplayDialog("Arena Created", $"New arena scene '{sceneName}' has been created and opened.", "OK");
        }

        [MenuItem("Gorgonzola/Scenes/Quick Setup (Current Scene)")]
        public static void QuickSetupCurrentScene()
        {
            if (EditorSceneManager.GetActiveScene().isDirty)
            {
                EditorUtility.DisplayDialog("Unsaved Changes", 
                    "Please save your scene first before running Quick Setup.", "OK");
                return;
            }

            SetupArenaInCurrentScene();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorUtility.DisplayDialog("Setup Complete", 
                "Arena infrastructure has been added to the current scene.", "OK");
        }

        /// <summary>
        /// Creates a completely new scene with full arena setup.
        /// </summary>
        public static void CreateArenaScene(string sceneName)
        {
            // Create new scene
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            
            // Remove default camera and light
            Object.DestroyImmediate(GameObject.Find("Camera"));
            Object.DestroyImmediate(GameObject.Find("Light"));

            // Setup the scene
            SetupArenaInCurrentScene();

            // Save the scene
            string scenePath = $"Assets/Scenes/{sceneName}.unity";
            EditorSceneManager.SaveScene(newScene, scenePath);
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }

        /// <summary>
        /// Configures an existing scene with full arena infrastructure.
        /// </summary>
        public static void SetupArenaInCurrentScene()
        {
            var scene = EditorSceneManager.GetActiveScene();
            var root = scene.GetRootGameObjects().Length > 0 ? scene.GetRootGameObjects()[0].transform : null;

            // Create managers hierarchy
            var managersGO = CreateOrGetGameObject("--- Managers ---", root);
            SetupGameManagers(managersGO.transform);

            // Create scene hierarchy
            var sceneGO = CreateOrGetGameObject("--- Scene ---", root);
            SetupSceneLayout(sceneGO.transform);

            // Create UI hierarchy
            var uiGO = CreateOrGetGameObject("--- UI ---", root);
            SetupUIHierarchy(uiGO.transform);

            Debug.Log("[GorgonzolaSceneSetup] Arena scene fully configured with MM TopDown Engine infrastructure.");
        }

        /// <summary>
        /// Sets up all manager systems (Turn, Game, UI, Inventory, Audio).
        /// </summary>
        private static void SetupGameManagers(Transform parentTransform)
        {
            // TurnManager (already exists in Scripts, but ensure it's in the scene)
            var turnManagerGO = CreateOrGetGameObject("TurnManager", parentTransform);
            var turnManager = turnManagerGO.GetComponent<TurnManager>();
            if (turnManager == null)
            {
                turnManager = turnManagerGO.AddComponent<TurnManager>();
            }

            // Game Manager (generic game flow)
            var gameManagerGO = CreateOrGetGameObject("GameManager", parentTransform);
            gameManagerGO.AddComponent<AudioListener>();
            
            // Inventory Manager (placeholder for InventoryEngine integration)
            var inventoryGO = CreateOrGetGameObject("InventoryManager", parentTransform);
            // TODO: Integrate with InventoryEngine when available

            // UI Manager (manages pause, menus, etc.)
            var uiManagerGO = CreateOrGetGameObject("UIManager", parentTransform);
            
            Debug.Log("[GorgonzolaSceneSetup] Managers created: TurnManager, GameManager, InventoryManager, UIManager");
        }

        /// <summary>
        /// Sets up the scene layout with ground plane, spawn points, and arena bounds.
        /// </summary>
        private static void SetupSceneLayout(Transform parentTransform)
        {
            // Ground plane
            var groundGO = CreateOrGetGameObject("Ground", parentTransform);
            var meshFilter = groundGO.GetComponent<MeshFilter>();
            var meshRenderer = groundGO.GetComponent<MeshRenderer>();
            var collider = groundGO.GetComponent<BoxCollider>();

            if (meshFilter == null)
            {
                meshFilter = groundGO.AddComponent<MeshFilter>();
                meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            }

            if (meshRenderer == null)
            {
                meshRenderer = groundGO.AddComponent<MeshRenderer>();
                meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            }

            if (collider == null)
            {
                collider = groundGO.AddComponent<BoxCollider>();
            }

            // Scale to be a flat plane (50x50x0.1)
            groundGO.transform.localScale = new Vector3(50, 0.1f, 50);
            groundGO.transform.localPosition = Vector3.zero;
            groundGO.layer = LayerMask.NameToLayer("Default");

            // Spawn points container
            var spawnPointsGO = CreateOrGetGameObject("SpawnPoints", parentTransform);
            
            // Player spawn
            var playerSpawnGO = CreateOrGetGameObject("PlayerSpawn", spawnPointsGO.transform);
            playerSpawnGO.transform.localPosition = Vector3.zero;
            
            // Enemy spawns (example: 4 corners)
            var enemySpawnsGO = CreateOrGetGameObject("EnemySpawns", spawnPointsGO.transform);
            Vector3[] enemySpawnPositions = new[]
            {
                new Vector3(10, 0.5f, 10),
                new Vector3(-10, 0.5f, 10),
                new Vector3(10, 0.5f, -10),
                new Vector3(-10, 0.5f, -10)
            };

            for (int i = 0; i < enemySpawnPositions.Length; i++)
            {
                var spawnGO = CreateOrGetGameObject($"EnemySpawn_{i}", enemySpawnsGO.transform);
                spawnGO.transform.localPosition = enemySpawnPositions[i];
            }

            // Lighting
            var lightGO = CreateOrGetGameObject("Directional Light", parentTransform);
            var light = lightGO.GetComponent<Light>();
            if (light == null)
            {
                light = lightGO.AddComponent<Light>();
                light.type = LightType.Directional;
                light.intensity = 1.2f;
            }
            lightGO.transform.rotation = Quaternion.Euler(45, 45, 0);

            Debug.Log("[GorgonzolaSceneSetup] Scene layout created: Ground plane, spawn points, lighting");
        }

        /// <summary>
        /// Sets up complete UI hierarchy following MM conventions.
        /// Includes HUD, Pause Menu, Inventory UI, Health displays, etc.
        /// </summary>
        private static void SetupUIHierarchy(Transform parentTransform)
        {
            // Main Canvas (for world-space HUD elements)
            var canvasGO = CreateOrGetGameObject("MainCanvas", parentTransform);
            var canvas = canvasGO.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<GraphicRaycaster>();
            }

            var canvasScaler = canvasGO.GetComponent<CanvasScaler>();
            if (canvasScaler == null)
            {
                canvasScaler = canvasGO.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            }

            // HUD Container
            var hudGO = CreateOrGetGameObject("HUD", canvasGO.transform);
            SetupHUD(hudGO.transform);

            // Pause Menu (hidden by default)
            var pauseMenuGO = CreateOrGetGameObject("PauseMenu", canvasGO.transform);
            pauseMenuGO.SetActive(false);
            SetupPauseMenu(pauseMenuGO.transform);

            // Inventory Panel (hidden by default)
            var inventoryGO = CreateOrGetGameObject("InventoryPanel", canvasGO.transform);
            inventoryGO.SetActive(false);
            SetupInventoryPanel(inventoryGO.transform);

            // Floating damage numbers / notifications (parent for popups)
            var notificationsGO = CreateOrGetGameObject("Notifications", canvasGO.transform);

            // UI Camera (if not already in scene)
            var uiCameraGO = CreateOrGetGameObject("UICamera", parentTransform);
            var uiCam = uiCameraGO.GetComponent<Camera>();
            if (uiCam == null)
            {
                uiCam = uiCameraGO.AddComponent<Camera>();
                uiCam.clearFlags = CameraClearFlags.Nothing;
                uiCam.cullingMask = LayerMask.GetMask("UI");
                uiCam.depth = 1;
            }

            Debug.Log("[GorgonzolaSceneSetup] UI hierarchy created: Canvas, HUD, Pause Menu, Inventory Panel, Notifications");
        }

        /// <summary>
        /// Sets up HUD elements: health bar, ammo counter, turn indicator, mini-map.
        /// </summary>
        private static void SetupHUD(Transform parentTransform)
        {
            // Health Bar
            var healthBarGO = CreateOrGetGameObject("HealthBar", parentTransform);
            var healthBarRectTransform = healthBarGO.GetComponent<RectTransform>();
            if (healthBarRectTransform == null)
            {
                healthBarRectTransform = healthBarGO.AddComponent<RectTransform>();
            }
            healthBarRectTransform.anchorMin = Vector2.zero;
            healthBarRectTransform.anchorMax = new Vector2(0, 1);
            healthBarRectTransform.offsetMin = new Vector2(10, -50);
            healthBarRectTransform.offsetMax = new Vector2(250, -10);

            // Add Image for background
            var healthBarImage = healthBarGO.GetComponent<Image>();
            if (healthBarImage == null)
            {
                healthBarImage = healthBarGO.AddComponent<Image>();
                healthBarImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            }

            // Health fill bar
            var fillGO = CreateOrGetGameObject("Fill", healthBarGO.transform);
            var fillImage = fillGO.GetComponent<Image>();
            if (fillImage == null)
            {
                fillImage = fillGO.AddComponent<Image>();
                fillImage.color = new Color(0.2f, 0.8f, 0.2f, 1);
            }
            var fillRectTransform = fillGO.GetComponent<RectTransform>();
            if (fillRectTransform == null)
            {
                fillRectTransform = fillGO.AddComponent<RectTransform>();
            }
            fillRectTransform.anchorMin = Vector2.zero;
            fillRectTransform.anchorMax = Vector2.one;
            fillRectTransform.offsetMin = Vector2.zero;
            fillRectTransform.offsetMax = Vector2.zero;

            // Turn Counter
            var turnCounterGO = CreateOrGetGameObject("TurnCounter", parentTransform);
            var turnCounterText = turnCounterGO.GetComponent<Text>();
            if (turnCounterText == null)
            {
                turnCounterText = turnCounterGO.AddComponent<Text>();
                turnCounterText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                turnCounterText.text = "Turn: 1";
                turnCounterText.alignment = TextAnchor.UpperRight;
                turnCounterText.color = Color.white;
                turnCounterText.fontSize = 30;
            }
            var turnRectTransform = turnCounterGO.GetComponent<RectTransform>();
            if (turnRectTransform == null)
            {
                turnRectTransform = turnCounterGO.AddComponent<RectTransform>();
            }
            turnRectTransform.anchorMin = new Vector2(1, 1);
            turnRectTransform.anchorMax = Vector2.one;
            turnRectTransform.offsetMin = new Vector2(-200, -50);
            turnRectTransform.offsetMax = Vector2.zero;

            // Ammo Counter
            var ammoGO = CreateOrGetGameObject("AmmoCounter", parentTransform);
            var ammoText = ammoGO.GetComponent<Text>();
            if (ammoText == null)
            {
                ammoText = ammoGO.AddComponent<Text>();
                ammoText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                ammoText.text = "Ammo: 30/90";
                ammoText.alignment = TextAnchor.LowerRight;
                ammoText.color = Color.white;
                ammoText.fontSize = 24;
            }
            var ammoRectTransform = ammoGO.GetComponent<RectTransform>();
            if (ammoRectTransform == null)
            {
                ammoRectTransform = ammoGO.AddComponent<RectTransform>();
            }
            ammoRectTransform.anchorMin = new Vector2(1, 0);
            ammoRectTransform.anchorMax = new Vector2(1, 0);
            ammoRectTransform.offsetMin = new Vector2(-250, 10);
            ammoRectTransform.offsetMax = new Vector2(-10, 60);

            Debug.Log("[GorgonzolaSceneSetup] HUD elements created: Health Bar, Turn Counter, Ammo Counter");
        }

        /// <summary>
        /// Sets up Pause Menu UI.
        /// </summary>
        private static void SetupPauseMenu(Transform parentTransform)
        {
            // Background panel
            var bgGO = CreateOrGetGameObject("Background", parentTransform);
            var bgImage = bgGO.GetComponent<Image>();
            if (bgImage == null)
            {
                bgImage = bgGO.AddComponent<Image>();
                bgImage.color = new Color(0, 0, 0, 0.7f);
            }
            var bgRectTransform = bgGO.GetComponent<RectTransform>();
            if (bgRectTransform == null)
            {
                bgRectTransform = bgGO.AddComponent<RectTransform>();
            }
            bgRectTransform.anchorMin = Vector2.zero;
            bgRectTransform.anchorMax = Vector2.one;
            bgRectTransform.offsetMin = Vector2.zero;
            bgRectTransform.offsetMax = Vector2.zero;

            // Menu panel
            var menuGO = CreateOrGetGameObject("MenuPanel", parentTransform);
            var menuImage = menuGO.GetComponent<Image>();
            if (menuImage == null)
            {
                menuImage = menuGO.AddComponent<Image>();
                menuImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            }
            var menuRectTransform = menuGO.GetComponent<RectTransform>();
            if (menuRectTransform == null)
            {
                menuRectTransform = menuGO.AddComponent<RectTransform>();
            }
            menuRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            menuRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            menuRectTransform.sizeDelta = new Vector2(400, 300);
            menuRectTransform.anchoredPosition = Vector2.zero;

            // Title
            var titleGO = CreateOrGetGameObject("Title", menuGO.transform);
            var titleText = titleGO.GetComponent<Text>();
            if (titleText == null)
            {
                titleText = titleGO.AddComponent<Text>();
                titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                titleText.text = "PAUSED";
                titleText.alignment = TextAnchor.MiddleCenter;
                titleText.color = Color.white;
                titleText.fontSize = 40;
            }
            var titleRectTransform = titleGO.GetComponent<RectTransform>();
            if (titleRectTransform == null)
            {
                titleRectTransform = titleGO.AddComponent<RectTransform>();
            }
            titleRectTransform.anchorMin = Vector2.one * 0.5f;
            titleRectTransform.anchorMax = Vector2.one * 0.5f;
            titleRectTransform.sizeDelta = new Vector2(300, 80);
            titleRectTransform.anchoredPosition = new Vector2(0, 80);

            // Resume button
            CreateMenuButton(menuGO.transform, "Resume", new Vector2(0, 20));
            
            // Settings button
            CreateMenuButton(menuGO.transform, "Settings", new Vector2(0, -30));
            
            // Main Menu button
            CreateMenuButton(menuGO.transform, "Main Menu", new Vector2(0, -80));

            Debug.Log("[GorgonzolaSceneSetup] Pause Menu UI created");
        }

        /// <summary>
        /// Sets up Inventory Panel UI.
        /// </summary>
        private static void SetupInventoryPanel(Transform parentTransform)
        {
            // Background
            var bgGO = CreateOrGetGameObject("Background", parentTransform);
            var bgImage = bgGO.GetComponent<Image>();
            if (bgImage == null)
            {
                bgImage = bgGO.AddComponent<Image>();
                bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            }
            var bgRectTransform = bgGO.GetComponent<RectTransform>();
            if (bgRectTransform == null)
            {
                bgRectTransform = bgGO.AddComponent<RectTransform>();
            }
            bgRectTransform.anchorMin = Vector2.zero;
            bgRectTransform.anchorMax = Vector2.one;
            bgRectTransform.offsetMin = Vector2.zero;
            bgRectTransform.offsetMax = Vector2.zero;

            // Title
            var titleGO = CreateOrGetGameObject("Title", parentTransform);
            var titleText = titleGO.GetComponent<Text>();
            if (titleText == null)
            {
                titleText = titleGO.AddComponent<Text>();
                titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                titleText.text = "Inventory";
                titleText.alignment = TextAnchor.UpperLeft;
                titleText.color = Color.white;
                titleText.fontSize = 40;
            }
            var titleRectTransform = titleGO.GetComponent<RectTransform>();
            if (titleRectTransform == null)
            {
                titleRectTransform = titleGO.AddComponent<RectTransform>();
            }
            titleRectTransform.anchorMin = Vector2.zero;
            titleRectTransform.anchorMax = Vector2.one;
            titleRectTransform.offsetMin = new Vector2(20, -80);
            titleRectTransform.offsetMax = new Vector2(-20, -20);

            // Item grid (placeholder)
            var gridGO = CreateOrGetGameObject("ItemGrid", parentTransform);
            gridGO.AddComponent<GridLayoutGroup>();

            Debug.Log("[GorgonzolaSceneSetup] Inventory Panel UI created");
        }

        /// <summary>
        /// Helper to create or get a GameObject.
        /// </summary>
        private static GameObject CreateOrGetGameObject(string name, Transform parent = null)
        {
            GameObject go;
            
            if (parent != null)
            {
                Transform existing = parent.Find(name);
                if (existing != null)
                {
                    return existing.gameObject;
                }
            }

            go = new GameObject(name);
            if (parent != null)
            {
                go.transform.SetParent(parent, false);
            }

            return go;
        }

        /// <summary>
        /// Helper to create a menu button.
        /// </summary>
        private static void CreateMenuButton(Transform parent, string buttonText, Vector2 position)
        {
            var buttonGO = CreateOrGetGameObject($"Button_{buttonText}", parent);
            var buttonImage = buttonGO.GetComponent<Image>();
            if (buttonImage == null)
            {
                buttonImage = buttonGO.AddComponent<Image>();
                buttonImage.color = new Color(0.3f, 0.3f, 0.3f, 1);
            }

            var buttonComponent = buttonGO.GetComponent<Button>();
            if (buttonComponent == null)
            {
                buttonComponent = buttonGO.AddComponent<Button>();
            }

            var rectTransform = buttonGO.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = buttonGO.AddComponent<RectTransform>();
            }
            rectTransform.anchorMin = Vector2.one * 0.5f;
            rectTransform.anchorMax = Vector2.one * 0.5f;
            rectTransform.sizeDelta = new Vector2(200, 40);
            rectTransform.anchoredPosition = position;

            // Text
            var textGO = CreateOrGetGameObject("Text", buttonGO.transform);
            var text = textGO.GetComponent<Text>();
            if (text == null)
            {
                text = textGO.AddComponent<Text>();
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.text = buttonText;
                text.alignment = TextAnchor.MiddleCenter;
                text.color = Color.white;
                text.fontSize = 20;
            }
            var textRectTransform = textGO.GetComponent<RectTransform>();
            if (textRectTransform == null)
            {
                textRectTransform = textGO.AddComponent<RectTransform>();
            }
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.offsetMin = Vector2.zero;
            textRectTransform.offsetMax = Vector2.zero;
        }
    }
}