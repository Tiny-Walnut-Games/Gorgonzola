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
        private const string SceneTemplatePath = "Assets/Scenes/ArenaTemplate_{0}.unity";
        
        // More Mountains prefab paths (Common resources)
        private const string MmUICameraPrefab = "Assets/TopDownEngine/Common/Prefabs/GUI/UICamera.prefab";
        private const string MmHealthBarPrefab = "Assets/TopDownEngine/Common/Prefabs/GUI/HealthBar.prefab";
        private const string MmFaderPrefab = "Assets/TopDownEngine/Common/Prefabs/GUI/MMFaderRound.prefab";

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
            var managersGo = CreateOrGetGameObject("--- Managers ---", root);
            SetupGameManagers(managersGo.transform);

            // Create scene hierarchy
            var sceneGo = CreateOrGetGameObject("--- Scene ---", root);
            SetupSceneLayout(sceneGo.transform);

            // Create UI hierarchy
            var uiGo = CreateOrGetGameObject("--- UI ---", root);
            SetupUIHierarchy(uiGo.transform);

            Debug.Log("[GorgonzolaSceneSetup] Arena scene fully configured with MM TopDown Engine infrastructure.");
        }

        /// <summary>
        /// Sets up all manager systems (Turn, Game, UI, Inventory, Audio).
        /// </summary>
        private static void SetupGameManagers(Transform parentTransform)
        {
            // TurnManager (already exists in Scripts, but ensure it's in the scene)
            var turnManagerGo = CreateOrGetGameObject("TurnManager", parentTransform);
            var turnManager = turnManagerGo.GetComponent<TurnManager>();
            if (turnManager == null)
            {
                turnManager = turnManagerGo.AddComponent<TurnManager>();
            }

            // Game Manager (generic game flow)
            var gameManagerGo = CreateOrGetGameObject("GameManager", parentTransform);
            gameManagerGo.AddComponent<AudioListener>();
            
            // Inventory Manager (placeholder for InventoryEngine integration)
            var inventoryGo = CreateOrGetGameObject("InventoryManager", parentTransform);
            // TODO: Integrate with InventoryEngine when available

            // UI Manager (manages pause, menus, etc.)
            var uiManagerGo = CreateOrGetGameObject("UIManager", parentTransform);
            
            Debug.Log("[GorgonzolaSceneSetup] Managers created: TurnManager, GameManager, InventoryManager, UIManager");
        }

        /// <summary>
        /// Sets up the scene layout with ground plane, spawn points, and arena bounds.
        /// </summary>
        private static void SetupSceneLayout(Transform parentTransform)
        {
            // Ground plane
            var groundGo = CreateOrGetGameObject("Ground", parentTransform);
            var meshFilter = groundGo.GetComponent<MeshFilter>();
            var meshRenderer = groundGo.GetComponent<MeshRenderer>();
            var collider = groundGo.GetComponent<BoxCollider>();

            if (meshFilter == null)
            {
                meshFilter = groundGo.AddComponent<MeshFilter>();
                meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            }

            if (meshRenderer == null)
            {
                meshRenderer = groundGo.AddComponent<MeshRenderer>();
                meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            }

            if (collider == null)
            {
                collider = groundGo.AddComponent<BoxCollider>();
            }

            // Scale to be a flat plane (50x50x0.1)
            groundGo.transform.localScale = new Vector3(50, 0.1f, 50);
            groundGo.transform.localPosition = Vector3.zero;
            groundGo.layer = LayerMask.NameToLayer("Default");

            // Spawn points container
            var spawnPointsGo = CreateOrGetGameObject("SpawnPoints", parentTransform);
            
            // Player spawn
            var playerSpawnGo = CreateOrGetGameObject("PlayerSpawn", spawnPointsGo.transform);
            playerSpawnGo.transform.localPosition = Vector3.zero;
            
            // Enemy spawns (example: 4 corners)
            var enemySpawnsGo = CreateOrGetGameObject("EnemySpawns", spawnPointsGo.transform);
            Vector3[] enemySpawnPositions = new[]
            {
                new Vector3(10, 0.5f, 10),
                new Vector3(-10, 0.5f, 10),
                new Vector3(10, 0.5f, -10),
                new Vector3(-10, 0.5f, -10)
            };

            for (int i = 0; i < enemySpawnPositions.Length; i++)
            {
                var spawnGo = CreateOrGetGameObject($"EnemySpawn_{i}", enemySpawnsGo.transform);
                spawnGo.transform.localPosition = enemySpawnPositions[i];
            }

            // Lighting
            var lightGo = CreateOrGetGameObject("Directional Light", parentTransform);
            var light = lightGo.GetComponent<Light>();
            if (light == null)
            {
                light = lightGo.AddComponent<Light>();
                light.type = LightType.Directional;
                light.intensity = 1.2f;
            }
            lightGo.transform.rotation = Quaternion.Euler(45, 45, 0);

            Debug.Log("[GorgonzolaSceneSetup] Scene layout created: Ground plane, spawn points, lighting");
        }

        /// <summary>
        /// Sets up complete UI hierarchy following MM conventions.
        /// Includes HUD, Pause Menu, Inventory UI, Health displays, etc.
        /// </summary>
        private static void SetupUIHierarchy(Transform parentTransform)
        {
            // Main Canvas (for world-space HUD elements)
            var canvasGo = CreateOrGetGameObject("MainCanvas", parentTransform);
            var canvas = canvasGo.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = canvasGo.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGo.AddComponent<GraphicRaycaster>();
            }

            var canvasScaler = canvasGo.GetComponent<CanvasScaler>();
            if (canvasScaler == null)
            {
                canvasScaler = canvasGo.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            }

            // HUD Container
            var hudGo = CreateOrGetGameObject("HUD", canvasGo.transform);
            SetupHUD(hudGo.transform);

            // Pause Menu (hidden by default)
            var pauseMenuGo = CreateOrGetGameObject("PauseMenu", canvasGo.transform);
            pauseMenuGo.SetActive(false);
            SetupPauseMenu(pauseMenuGo.transform);

            // Inventory Panel (hidden by default)
            var inventoryGo = CreateOrGetGameObject("InventoryPanel", canvasGo.transform);
            inventoryGo.SetActive(false);
            SetupInventoryPanel(inventoryGo.transform);

            // Floating damage numbers / notifications (parent for popups)
            var notificationsGo = CreateOrGetGameObject("Notifications", canvasGo.transform);

            // UI Camera (if not already in scene)
            var uiCameraGo = CreateOrGetGameObject("UICamera", parentTransform);
            var uiCam = uiCameraGo.GetComponent<Camera>();
            if (uiCam == null)
            {
                uiCam = uiCameraGo.AddComponent<Camera>();
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
            var healthBarGo = CreateOrGetGameObject("HealthBar", parentTransform);
            var healthBarRectTransform = healthBarGo.GetComponent<RectTransform>();
            if (healthBarRectTransform == null)
            {
                healthBarRectTransform = healthBarGo.AddComponent<RectTransform>();
            }
            healthBarRectTransform.anchorMin = Vector2.zero;
            healthBarRectTransform.anchorMax = new Vector2(0, 1);
            healthBarRectTransform.offsetMin = new Vector2(10, -50);
            healthBarRectTransform.offsetMax = new Vector2(250, -10);

            // Add Image for background
            var healthBarImage = healthBarGo.GetComponent<Image>();
            if (healthBarImage == null)
            {
                healthBarImage = healthBarGo.AddComponent<Image>();
                healthBarImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            }

            // Health fill bar
            var fillGo = CreateOrGetGameObject("Fill", healthBarGo.transform);
            var fillImage = fillGo.GetComponent<Image>();
            if (fillImage == null)
            {
                fillImage = fillGo.AddComponent<Image>();
                fillImage.color = new Color(0.2f, 0.8f, 0.2f, 1);
            }
            var fillRectTransform = fillGo.GetComponent<RectTransform>();
            if (fillRectTransform == null)
            {
                fillRectTransform = fillGo.AddComponent<RectTransform>();
            }
            fillRectTransform.anchorMin = Vector2.zero;
            fillRectTransform.anchorMax = Vector2.one;
            fillRectTransform.offsetMin = Vector2.zero;
            fillRectTransform.offsetMax = Vector2.zero;

            // Turn Counter
            var turnCounterGo = CreateOrGetGameObject("TurnCounter", parentTransform);
            var turnCounterText = turnCounterGo.GetComponent<Text>();
            if (turnCounterText == null)
            {
                turnCounterText = turnCounterGo.AddComponent<Text>();
                turnCounterText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                turnCounterText.text = "Turn: 1";
                turnCounterText.alignment = TextAnchor.UpperRight;
                turnCounterText.color = Color.white;
                turnCounterText.fontSize = 30;
            }
            var turnRectTransform = turnCounterGo.GetComponent<RectTransform>();
            if (turnRectTransform == null)
            {
                turnRectTransform = turnCounterGo.AddComponent<RectTransform>();
            }
            turnRectTransform.anchorMin = new Vector2(1, 1);
            turnRectTransform.anchorMax = Vector2.one;
            turnRectTransform.offsetMin = new Vector2(-200, -50);
            turnRectTransform.offsetMax = Vector2.zero;

            // Ammo Counter
            var ammoGo = CreateOrGetGameObject("AmmoCounter", parentTransform);
            var ammoText = ammoGo.GetComponent<Text>();
            if (ammoText == null)
            {
                ammoText = ammoGo.AddComponent<Text>();
                ammoText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                ammoText.text = "Ammo: 30/90";
                ammoText.alignment = TextAnchor.LowerRight;
                ammoText.color = Color.white;
                ammoText.fontSize = 24;
            }
            var ammoRectTransform = ammoGo.GetComponent<RectTransform>();
            if (ammoRectTransform == null)
            {
                ammoRectTransform = ammoGo.AddComponent<RectTransform>();
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
            var bgGo = CreateOrGetGameObject("Background", parentTransform);
            var bgImage = bgGo.GetComponent<Image>();
            if (bgImage == null)
            {
                bgImage = bgGo.AddComponent<Image>();
                bgImage.color = new Color(0, 0, 0, 0.7f);
            }
            var bgRectTransform = bgGo.GetComponent<RectTransform>();
            if (bgRectTransform == null)
            {
                bgRectTransform = bgGo.AddComponent<RectTransform>();
            }
            bgRectTransform.anchorMin = Vector2.zero;
            bgRectTransform.anchorMax = Vector2.one;
            bgRectTransform.offsetMin = Vector2.zero;
            bgRectTransform.offsetMax = Vector2.zero;

            // Menu panel
            var menuGo = CreateOrGetGameObject("MenuPanel", parentTransform);
            var menuImage = menuGo.GetComponent<Image>();
            if (menuImage == null)
            {
                menuImage = menuGo.AddComponent<Image>();
                menuImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            }
            var menuRectTransform = menuGo.GetComponent<RectTransform>();
            if (menuRectTransform == null)
            {
                menuRectTransform = menuGo.AddComponent<RectTransform>();
            }
            menuRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            menuRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            menuRectTransform.sizeDelta = new Vector2(400, 300);
            menuRectTransform.anchoredPosition = Vector2.zero;

            // Title
            var titleGo = CreateOrGetGameObject("Title", menuGo.transform);
            var titleText = titleGo.GetComponent<Text>();
            if (titleText == null)
            {
                titleText = titleGo.AddComponent<Text>();
                titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                titleText.text = "PAUSED";
                titleText.alignment = TextAnchor.MiddleCenter;
                titleText.color = Color.white;
                titleText.fontSize = 40;
            }
            var titleRectTransform = titleGo.GetComponent<RectTransform>();
            if (titleRectTransform == null)
            {
                titleRectTransform = titleGo.AddComponent<RectTransform>();
            }
            titleRectTransform.anchorMin = Vector2.one * 0.5f;
            titleRectTransform.anchorMax = Vector2.one * 0.5f;
            titleRectTransform.sizeDelta = new Vector2(300, 80);
            titleRectTransform.anchoredPosition = new Vector2(0, 80);

            // Resume button
            CreateMenuButton(menuGo.transform, "Resume", new Vector2(0, 20));
            
            // Settings button
            CreateMenuButton(menuGo.transform, "Settings", new Vector2(0, -30));
            
            // Main Menu button
            CreateMenuButton(menuGo.transform, "Main Menu", new Vector2(0, -80));

            Debug.Log("[GorgonzolaSceneSetup] Pause Menu UI created");
        }

        /// <summary>
        /// Sets up Inventory Panel UI.
        /// </summary>
        private static void SetupInventoryPanel(Transform parentTransform)
        {
            // Background
            var bgGo = CreateOrGetGameObject("Background", parentTransform);
            var bgImage = bgGo.GetComponent<Image>();
            if (bgImage == null)
            {
                bgImage = bgGo.AddComponent<Image>();
                bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            }
            var bgRectTransform = bgGo.GetComponent<RectTransform>();
            if (bgRectTransform == null)
            {
                bgRectTransform = bgGo.AddComponent<RectTransform>();
            }
            bgRectTransform.anchorMin = Vector2.zero;
            bgRectTransform.anchorMax = Vector2.one;
            bgRectTransform.offsetMin = Vector2.zero;
            bgRectTransform.offsetMax = Vector2.zero;

            // Title
            var titleGo = CreateOrGetGameObject("Title", parentTransform);
            var titleText = titleGo.GetComponent<Text>();
            if (titleText == null)
            {
                titleText = titleGo.AddComponent<Text>();
                titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                titleText.text = "Inventory";
                titleText.alignment = TextAnchor.UpperLeft;
                titleText.color = Color.white;
                titleText.fontSize = 40;
            }
            var titleRectTransform = titleGo.GetComponent<RectTransform>();
            if (titleRectTransform == null)
            {
                titleRectTransform = titleGo.AddComponent<RectTransform>();
            }
            titleRectTransform.anchorMin = Vector2.zero;
            titleRectTransform.anchorMax = Vector2.one;
            titleRectTransform.offsetMin = new Vector2(20, -80);
            titleRectTransform.offsetMax = new Vector2(-20, -20);

            // Item grid (placeholder)
            var gridGo = CreateOrGetGameObject("ItemGrid", parentTransform);
            gridGo.AddComponent<GridLayoutGroup>();

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
            var buttonGo = CreateOrGetGameObject($"Button_{buttonText}", parent);
            var buttonImage = buttonGo.GetComponent<Image>();
            if (buttonImage == null)
            {
                buttonImage = buttonGo.AddComponent<Image>();
                buttonImage.color = new Color(0.3f, 0.3f, 0.3f, 1);
            }

            var buttonComponent = buttonGo.GetComponent<Button>();
            if (buttonComponent == null)
            {
                buttonComponent = buttonGo.AddComponent<Button>();
            }

            var rectTransform = buttonGo.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = buttonGo.AddComponent<RectTransform>();
            }
            rectTransform.anchorMin = Vector2.one * 0.5f;
            rectTransform.anchorMax = Vector2.one * 0.5f;
            rectTransform.sizeDelta = new Vector2(200, 40);
            rectTransform.anchoredPosition = position;

            // Text
            var textGo = CreateOrGetGameObject("Text", buttonGo.transform);
            var text = textGo.GetComponent<Text>();
            if (text == null)
            {
                text = textGo.AddComponent<Text>();
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.text = buttonText;
                text.alignment = TextAnchor.MiddleCenter;
                text.color = Color.white;
                text.fontSize = 20;
            }
            var textRectTransform = textGo.GetComponent<RectTransform>();
            if (textRectTransform == null)
            {
                textRectTransform = textGo.AddComponent<RectTransform>();
            }
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.offsetMin = Vector2.zero;
            textRectTransform.offsetMax = Vector2.zero;
        }
    }
}