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
                sr.color = new Color(1f, 0.75f, 0.2f);
                go.AddComponent<CharacterView>();
            }
            _view = go.GetComponent<CharacterView>();
            if (_view == null) _view = go.AddComponent<CharacterView>();
            go.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        }

        public void Render(GameSnapshot snap)
        {
            if (_view == null) return;
            var pos = GameManager.GameToWorld(snap.character.x, snap.character.y);
            _view.transform.position = pos;

            var sr = _view.SR;
            if (snap.character.invincible)
                sr.color = new Color(1f, 0.75f, 0.2f, Mathf.PingPong(Time.time * 8f, 1f));
            else if (snap.character.stunned)
                sr.color = Color.gray;
            else
                sr.color = new Color(1f, 0.75f, 0.2f);
        }
    }
}
