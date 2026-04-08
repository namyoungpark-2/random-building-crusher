#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using BuildingCrusher.Managers;
using BuildingCrusher.Rendering;
using BuildingCrusher.UI;

namespace BuildingCrusher.Editor
{
    public static class SceneSetup
    {
        [MenuItem("BuildingCrusher/Setup Scene")]
        public static void Setup()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var camGo = new GameObject("Main Camera");
                cam = camGo.AddComponent<Camera>();
                camGo.AddComponent<AudioListener>();
                camGo.tag = "MainCamera";
            }
            cam.orthographic = true;
            cam.orthographicSize = 10f;
            cam.backgroundColor = new Color(0.05f, 0.05f, 0.15f);
            cam.transform.position = new Vector3(0, 0, -10);

            var gmGo = FindOrCreate("GameManager");
            if (!gmGo.GetComponent<GameManager>()) gmGo.AddComponent<GameManager>();

            var brGo = FindOrCreate("BuildingRenderer");
            if (!brGo.GetComponent<BuildingRenderer>()) brGo.AddComponent<BuildingRenderer>();

            var crGo = FindOrCreate("CharacterRenderer");
            if (!crGo.GetComponent<CharacterRenderer>()) crGo.AddComponent<CharacterRenderer>();

            var hrGo = FindOrCreate("HazardRenderer");
            if (!hrGo.GetComponent<HazardRenderer>()) hrGo.AddComponent<HazardRenderer>();

            var emGo = FindOrCreate("EffectsManager");
            if (!emGo.GetComponent<EffectsManager>()) emGo.AddComponent<EffectsManager>();

            var canvasGo = FindOrCreate("Canvas");
            if (!canvasGo.GetComponent<Canvas>())
            {
                var canvas = canvasGo.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var scaler = canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
                scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080, 1920);
                scaler.matchWidthOrHeight = 0.5f;
                canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
            var uiGo = FindOrCreate("UIManager", canvasGo.transform);
            if (!uiGo.GetComponent<UIManager>()) uiGo.AddComponent<UIManager>();

            if (!Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>())
            {
                var esGo = new GameObject("EventSystem");
                esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            Debug.Log("[BuildingCrusher] Scene setup complete!");
        }

        static GameObject FindOrCreate(string name, Transform parent = null)
        {
            var existing = GameObject.Find(name);
            if (existing != null) return existing;
            var go = new GameObject(name);
            if (parent != null) go.transform.SetParent(parent);
            return go;
        }
    }
}
#endif
