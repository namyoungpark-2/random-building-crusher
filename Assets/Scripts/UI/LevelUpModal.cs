using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BuildingCrusher.Data;
using BuildingCrusher.Managers;

namespace BuildingCrusher.UI
{
    public class LevelUpModal : MonoBehaviour
    {
        RectTransform _rect;
        GameObject[] _cards = new GameObject[3];
        TextMeshProUGUI[] _cardNames = new TextMeshProUGUI[3];
        TextMeshProUGUI[] _cardDescs = new TextMeshProUGUI[3];
        Image[] _cardBgs = new Image[3];

        public void Initialize()
        {
            _rect = gameObject.AddComponent<RectTransform>();
            _rect.anchorMin = Vector2.zero; _rect.anchorMax = Vector2.one;
            _rect.sizeDelta = Vector2.zero;

            var overlay = gameObject.AddComponent<Image>();
            overlay.color = new Color(0, 0, 0, 0.7f);

            var titleGo = new GameObject("Title");
            titleGo.transform.SetParent(transform, false);
            var titleRect = titleGo.AddComponent<RectTransform>();
            titleRect.anchoredPosition = new Vector2(0, 300);
            titleRect.sizeDelta = new Vector2(400, 80);
            var titleTmp = titleGo.AddComponent<TextMeshProUGUI>();
            titleTmp.text = "LEVEL UP!"; titleTmp.fontSize = 48;
            titleTmp.alignment = TextAlignmentOptions.Center; titleTmp.color = new Color(1f, 0.75f, 0.2f);

            for (int i = 0; i < 3; i++)
            {
                float xPos = (i - 1) * 320f;
                var card = new GameObject($"Card_{i}");
                card.transform.SetParent(transform, false);
                var cardRect = card.AddComponent<RectTransform>();
                cardRect.anchoredPosition = new Vector2(xPos, -50);
                cardRect.sizeDelta = new Vector2(280, 350);

                _cardBgs[i] = card.AddComponent<Image>();
                _cardBgs[i].color = new Color(0.12f, 0.14f, 0.18f);

                var btn = card.AddComponent<Button>();
                int idx = i;
                btn.onClick.AddListener(() => OnCardClicked(idx));

                var nameGo = new GameObject("Name");
                nameGo.transform.SetParent(card.transform, false);
                var nameRect = nameGo.AddComponent<RectTransform>();
                nameRect.anchoredPosition = new Vector2(0, 80);
                nameRect.sizeDelta = new Vector2(250, 50);
                _cardNames[i] = nameGo.AddComponent<TextMeshProUGUI>();
                _cardNames[i].fontSize = 26; _cardNames[i].alignment = TextAlignmentOptions.Center;
                _cardNames[i].color = Color.white;

                var descGo = new GameObject("Desc");
                descGo.transform.SetParent(card.transform, false);
                var descRect = descGo.AddComponent<RectTransform>();
                descRect.anchoredPosition = new Vector2(0, -30);
                descRect.sizeDelta = new Vector2(250, 120);
                _cardDescs[i] = descGo.AddComponent<TextMeshProUGUI>();
                _cardDescs[i].fontSize = 18; _cardDescs[i].alignment = TextAlignmentOptions.Center;
                _cardDescs[i].color = new Color(0.7f, 0.7f, 0.7f);

                _cards[i] = card;
            }
        }

        public void Show(string[] choices)
        {
            gameObject.SetActive(true);
            for (int i = 0; i < 3; i++)
            {
                if (i < choices.Length)
                {
                    _cards[i].SetActive(true);
                    string id = choices[i];

                    if (AbilityDefs.ALL.TryGetValue(id, out var ability))
                    {
                        _cardNames[i].text = ability.name;
                        _cardDescs[i].text = ability.description;
                        _cardBgs[i].color = new Color(0.12f, 0.14f, 0.18f);
                        _cardNames[i].color = ability.category switch
                        {
                            AbilityCategory.Attack => new Color(0.94f, 0.27f, 0.27f),
                            AbilityCategory.Movement => new Color(0.23f, 0.51f, 1f),
                            AbilityCategory.Survival => new Color(0.06f, 0.73f, 0.51f),
                            AbilityCategory.Reward => new Color(0.96f, 0.62f, 0.04f),
                            AbilityCategory.Special => new Color(0.66f, 0.33f, 0.97f),
                            _ => Color.white,
                        };
                    }
                    else if (SkillDefs.ALL.TryGetValue(id, out var skill))
                    {
                        _cardNames[i].text = skill.name;
                        _cardDescs[i].text = skill.description;
                        _cardBgs[i].color = new Color(0.15f, 0.05f, 0.10f);
                        _cardNames[i].color = new Color(1f, 0.75f, 0.2f);
                    }
                }
                else
                {
                    _cards[i].SetActive(false);
                }
            }
        }

        public void Hide() => gameObject.SetActive(false);

        void OnCardClicked(int index) => GameManager.Instance.SelectLevelUp(index);
    }
}
