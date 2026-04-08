using UnityEngine;
using BuildingCrusher.Core;
using BuildingCrusher.Data;
using BuildingCrusher.Rendering;
using BuildingCrusher.UI;

namespace BuildingCrusher.Managers
{
    public enum ScreenState { Title, Game, Result }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        GameState _state;
        GameSnapshot _snapshot;
        ScreenState _screen = ScreenState.Title;

        BuildingRenderer _buildingRenderer;
        CharacterRenderer _characterRenderer;
        HazardRenderer _hazardRenderer;
        EffectsManager _effectsManager;
        UIManager _uiManager;

        public GameSnapshot Snapshot => _snapshot;
        public GameState State => _state;
        public ScreenState CurrentScreen => _screen;

        const float SCALE = 0.01f;
        static readonly float HalfW = GameData.WORLD_WIDTH * 0.5f;
        static readonly float HalfH = GameData.WORLD_HEIGHT * 0.5f;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            _buildingRenderer = FindAnyObjectByType<BuildingRenderer>();
            _characterRenderer = FindAnyObjectByType<CharacterRenderer>();
            _hazardRenderer = FindAnyObjectByType<HazardRenderer>();
            _effectsManager = FindAnyObjectByType<EffectsManager>();
            _uiManager = FindAnyObjectByType<UIManager>();
            ShowTitle();
        }

        void Update()
        {
            if (_screen != ScreenState.Game || _state == null) return;
            if (_state.gameOver)
            {
                if (_screen != ScreenState.Result)
                {
                    _screen = ScreenState.Result;
                    _snapshot = GameSnapshot.Create(_state);
                    _uiManager.ShowResult(_snapshot);
                }
                return;
            }
            if (_state.paused || _state.levelUpPending) return;

            float dt = Mathf.Min(0.033f, Time.deltaTime);
            GameEngine.UpdateGame(_state, dt);
            _snapshot = GameSnapshot.Create(_state);

            _buildingRenderer.Render(_snapshot);
            _characterRenderer.Render(_snapshot);
            _hazardRenderer.Render(_snapshot);
            _effectsManager.UpdateEffects(_snapshot);
            _uiManager.UpdateGameUI(_snapshot);

            // Touch input
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
                {
                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(touch.position);
                    float gx = worldPos.x / SCALE + HalfW;
                    float gy = HalfH - worldPos.y / SCALE;
                    GameEngine.ProcessTouch(_state, gx, gy);
                }
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float gx = worldPos.x / SCALE + HalfW;
                float gy = HalfH - worldPos.y / SCALE;
                GameEngine.ProcessTouch(_state, gx, gy);
            }
        }

        public static Vector3 GameToWorld(float gx, float gy) =>
            new((gx - HalfW) * SCALE, (HalfH - gy) * SCALE, 0f);

        public static float GameToWorldScale(float v) => v * SCALE;

        public void ShowTitle()
        {
            _screen = ScreenState.Title;
            _uiManager.ShowTitle();
        }

        public void StartGame()
        {
            _state = GameEngine.CreateNewGame();
            _snapshot = GameSnapshot.Create(_state);
            _screen = ScreenState.Game;
            _buildingRenderer.Initialize();
            _characterRenderer.Initialize();
            _hazardRenderer.Initialize();
            _uiManager.ShowGame(_snapshot);
        }

        public void SelectLevelUp(int choiceIndex)
        {
            GameEngine.SelectLevelUp(_state, choiceIndex);
        }

        public void RestartGame() => StartGame();
    }
}
