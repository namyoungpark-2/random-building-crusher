using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BuildingCrusher.Core;
using BuildingCrusher.Managers;

namespace BuildingCrusher.UI
{
    public class UIManager : MonoBehaviour
    {
        GameObject _titleScreen, _gameScreen, _resultScreen;
        TextMeshProUGUI _stageText, _timerText, _levelText, _expText, _hpText;
        TextMeshProUGUI _resultScoreText, _resultDetailText, _resultStageText;
        Button _startButton, _restartButton;
        LevelUpModal _levelUpModal;

        void Awake() => BuildUI();

        void BuildUI()
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                var canvasGo = new GameObject("Canvas");
                canvas = canvasGo.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var scaler = canvasGo.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080, 1920);
                canvasGo.AddComponent<GraphicRaycaster>();
                transform.SetParent(canvasGo.transform);
            }

            // Ensure UIManager RectTransform fills the Canvas
            var myRect = GetComponent<RectTransform>();
            if (myRect == null) myRect = gameObject.AddComponent<RectTransform>();
            myRect.anchorMin = Vector2.zero;
            myRect.anchorMax = Vector2.one;
            myRect.sizeDelta = Vector2.zero;
            myRect.anchoredPosition = Vector2.zero;

            _titleScreen = CreatePanel("TitleScreen");
            var titleText = CreateText(_titleScreen.transform, "랜덤 건물 부수기", 60, TextAlignmentOptions.Center);
            titleText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 200);
            _startButton = CreateButton(_titleScreen.transform, "시작", new Vector2(0, -100), OnStartClicked);

            _gameScreen = CreatePanel("GameScreen");
            _gameScreen.SetActive(false);

            var hud = new GameObject("HUD");
            hud.transform.SetParent(_gameScreen.transform, false);
            var hudRect = hud.AddComponent<RectTransform>();
            hudRect.anchorMin = new Vector2(0, 1); hudRect.anchorMax = new Vector2(1, 1);
            hudRect.pivot = new Vector2(0.5f, 1); hudRect.sizeDelta = new Vector2(0, 120);

            _hpText = CreateText(hud.transform, "HP 100/100", 24, TextAlignmentOptions.Left);
            _hpText.GetComponent<RectTransform>().anchoredPosition = new Vector2(-350, -30);

            _stageText = CreateText(hud.transform, "스테이지 1", 28, TextAlignmentOptions.Center);
            _stageText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30);

            _timerText = CreateText(hud.transform, "30.0s", 28, TextAlignmentOptions.Right);
            _timerText.GetComponent<RectTransform>().anchoredPosition = new Vector2(350, -30);

            _levelText = CreateText(hud.transform, "Lv.1", 22, TextAlignmentOptions.Left);
            _levelText.GetComponent<RectTransform>().anchoredPosition = new Vector2(-350, -70);

            _expText = CreateText(hud.transform, "EXP 0%", 20, TextAlignmentOptions.Right);
            _expText.GetComponent<RectTransform>().anchoredPosition = new Vector2(350, -70);

            var modalGo = new GameObject("LevelUpModal");
            modalGo.transform.SetParent(_gameScreen.transform, false);
            _levelUpModal = modalGo.AddComponent<LevelUpModal>();
            _levelUpModal.Initialize();
            modalGo.SetActive(false);

            _resultScreen = CreatePanel("ResultScreen");
            _resultScreen.SetActive(false);
            _resultStageText = CreateText(_resultScreen.transform, "GAME OVER", 48, TextAlignmentOptions.Center);
            _resultStageText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 300);

            _resultDetailText = CreateText(_resultScreen.transform, "", 24, TextAlignmentOptions.Center);
            _resultDetailText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100);

            _resultScoreText = CreateText(_resultScreen.transform, "0", 72, TextAlignmentOptions.Center);
            _resultScoreText.color = new Color(1f, 0.75f, 0.2f);
            _resultScoreText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50);

            _restartButton = CreateButton(_resultScreen.transform, "다시 하기", new Vector2(0, -300), OnRestartClicked);
        }

        public void ShowTitle()
        {
            _titleScreen.SetActive(true);
            _gameScreen.SetActive(false);
            _resultScreen.SetActive(false);
        }

        public void ShowGame(GameSnapshot snap)
        {
            _titleScreen.SetActive(false);
            _gameScreen.SetActive(true);
            _resultScreen.SetActive(false);
            UpdateGameUI(snap);
        }

        public void ShowResult(GameSnapshot snap)
        {
            _gameScreen.SetActive(false);
            _resultScreen.SetActive(true);

            _resultStageText.text = "GAME OVER";
            _resultDetailText.text = $"스테이지 {snap.stage - 1} 도달\n건물 {snap.buildingsDestroyed}개 파괴\n총 데미지 {Mathf.RoundToInt(snap.totalDamage)}";
            _resultScoreText.text = $"{snap.score:N0}";
        }

        public void UpdateGameUI(GameSnapshot snap)
        {
            _hpText.text = $"HP {Mathf.RoundToInt(snap.character.hp)}/{Mathf.RoundToInt(snap.character.maxHp)}";
            _hpText.color = snap.character.hp / snap.character.maxHp < 0.3f ? Color.red : Color.white;

            _stageText.text = $"스테이지 {snap.stage}";
            _timerText.text = $"{snap.stageTimer:F1}s";
            _timerText.color = snap.stageTimer < 10f ? Color.red : Color.white;

            _levelText.text = $"Lv.{snap.level}";
            float expPct = snap.expRequired > 0 ? snap.exp / snap.expRequired : 0f;
            _expText.text = $"EXP {Mathf.RoundToInt(expPct * 100)}%";

            if (snap.levelUpPending && !_levelUpModal.gameObject.activeSelf)
                _levelUpModal.Show(snap.levelUpChoices);
            else if (!snap.levelUpPending && _levelUpModal.gameObject.activeSelf)
                _levelUpModal.Hide();

            if (snap.activeEnvHazard != null)
                _stageText.text += snap.activeEnvHazard switch
                {
                    "earthquake" => " [지진!]",
                    "storm" => " [폭풍!]",
                    "acid_rain" => " [산성비!]",
                    _ => ""
                };
        }

        void OnStartClicked() => GameManager.Instance.StartGame();
        void OnRestartClicked() => GameManager.Instance.RestartGame();

        GameObject CreatePanel(string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            return go;
        }

        TextMeshProUGUI CreateText(Transform parent, string text, float size, TextAlignmentOptions align)
        {
            var go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 80);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text; tmp.fontSize = size; tmp.alignment = align;
            tmp.color = Color.white;
            return tmp;
        }

        Button CreateButton(Transform parent, string label, Vector2 pos, UnityEngine.Events.UnityAction onClick)
        {
            var go = new GameObject("Button");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(300, 80);
            rect.anchoredPosition = pos;

            var img = go.AddComponent<Image>();
            img.color = new Color(0.2f, 0.5f, 0.9f);

            var btn = go.AddComponent<Button>();
            btn.onClick.AddListener(onClick);

            var textGo = new GameObject("Label");
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero; textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label; tmp.fontSize = 28; tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            return btn;
        }
    }
}
