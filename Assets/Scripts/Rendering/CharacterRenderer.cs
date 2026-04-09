using UnityEngine;
using BuildingCrusher.Core;
using BuildingCrusher.Entities;
using BuildingCrusher.Managers;

namespace BuildingCrusher.Rendering
{
    public class CharacterRenderer : MonoBehaviour
    {
        [SerializeField] GameObject characterPrefab;
        CharacterView _view;
        float _attackAnimTimer;
        float _lastHp;

        public void Initialize()
        {
            if (_view != null) Destroy(_view.gameObject);

            GameObject go;
            if (characterPrefab != null)
                go = Instantiate(characterPrefab, transform);
            else
            {
                go = new GameObject("Character");
                go.transform.SetParent(transform);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = SpriteHelper.Character;
                sr.color = Color.white;
                sr.sortingOrder = 10;
                go.AddComponent<CharacterView>();
            }
            _view = go.GetComponent<CharacterView>();
            if (_view == null) _view = go.AddComponent<CharacterView>();
            go.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
            _lastHp = -1f;
        }

        public void Render(GameSnapshot snap)
        {
            if (_view == null) return;
            var pos = GameManager.GameToWorld(snap.character.x, snap.character.y);
            _view.transform.position = pos;

            var sr = _view.SR;
            if (sr.sprite == null) sr.sprite = SpriteHelper.Character;

            // Attack animation (scale punch)
            if (_attackAnimTimer > 0f)
            {
                _attackAnimTimer -= Time.deltaTime;
                float t = _attackAnimTimer / 0.15f;
                float scale = 1.5f + Mathf.Sin(t * Mathf.PI) * 0.4f;
                _view.transform.localScale = new Vector3(scale, scale, 1f);
            }
            else
            {
                _view.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
            }

            // Detect attack (building HP decreased = attack happened)
            // We use a simple heuristic: if the building has a current floor being damaged
            if (snap.building.currentFloorIndex >= 0 && snap.floatingTexts.Length > 0)
            {
                // Check if there's a new damage text (attack just happened)
                foreach (var ft in snap.floatingTexts)
                {
                    if (ft.ttl > 0.9f) // newly created text
                    {
                        _attackAnimTimer = 0.15f;
                        break;
                    }
                }
            }

            // Hit flash (hp decreased)
            if (_lastHp > 0f && snap.character.hp < _lastHp)
            {
                sr.color = Color.red;
            }
            else if (snap.character.invincible)
            {
                sr.color = new Color(1f, 1f, 1f, Mathf.PingPong(Time.time * 8f, 1f));
            }
            else if (snap.character.stunned)
            {
                sr.color = new Color(0.5f, 0.5f, 1f); // blue tint for stun
            }
            else
            {
                sr.color = Color.white;
            }
            _lastHp = snap.character.hp;
        }
    }
}
