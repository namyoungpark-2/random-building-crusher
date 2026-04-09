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
        GameObject _weaponGo;
        SpriteRenderer _weaponSR;
        float _attackAnimTimer;
        float _lastHp;
        float _lastBuildingHp = -1f;
        string _currentWeapon;

        const float SWING_DURATION = 0.25f;
        const float SWING_ANGLE_START = 60f;
        const float SWING_ANGLE_END = -30f;

        public void Initialize()
        {
            if (_view != null) Destroy(_view.gameObject);

            // Character body
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

            // Weapon (child of character)
            if (_weaponGo != null) Destroy(_weaponGo);
            _weaponGo = new GameObject("Weapon");
            _weaponGo.transform.SetParent(go.transform);
            _weaponGo.transform.localPosition = new Vector3(0.4f, 0.2f, 0f); // right side, slightly up
            _weaponSR = _weaponGo.AddComponent<SpriteRenderer>();
            _weaponSR.sortingOrder = 11;
            SetWeaponSprite(null); // default fist/hammer

            _lastHp = -1f;
            _lastBuildingHp = -1f;
            _currentWeapon = null;
        }

        void SetWeaponSprite(string weaponSkill)
        {
            if (_weaponSR == null) return;
            _currentWeapon = weaponSkill;

            Color weaponColor;
            float scaleX, scaleY;

            switch (weaponSkill)
            {
                case "power_hammer":
                    weaponColor = new Color(0.5f, 0.5f, 0.55f);
                    scaleX = 0.5f; scaleY = 0.8f;
                    break;
                case "drill_arm":
                    weaponColor = new Color(0.7f, 0.7f, 0.2f);
                    scaleX = 0.3f; scaleY = 0.7f;
                    break;
                case "demolition_crane":
                    weaponColor = new Color(0.9f, 0.5f, 0.1f);
                    scaleX = 0.2f; scaleY = 1.2f;
                    break;
                case "dual_fist":
                    weaponColor = new Color(0.9f, 0.7f, 0.5f);
                    scaleX = 0.4f; scaleY = 0.4f;
                    break;
                case "wrecking_ball":
                    weaponColor = new Color(0.3f, 0.3f, 0.35f);
                    scaleX = 0.7f; scaleY = 0.7f;
                    break;
                default: // fist
                    weaponColor = new Color(0.9f, 0.7f, 0.5f);
                    scaleX = 0.35f; scaleY = 0.35f;
                    break;
            }

            _weaponSR.sprite = MakeWeaponSprite(weaponSkill ?? "fist", weaponColor);
            _weaponGo.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }

        Sprite MakeWeaponSprite(string name, Color color)
        {
            int w = 16, h = 32;
            var tex = new Texture2D(w, h);
            tex.filterMode = FilterMode.Point;
            var px = new Color[w * h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (name == "fist" || name == "dual_fist")
                    {
                        // Circle fist
                        float cx = (x - w / 2f) / (w / 2f);
                        float cy = (y - h / 2f) / (h / 2f);
                        px[y * w + x] = cx * cx + cy * cy < 1f ? color : Color.clear;
                    }
                    else if (name == "wrecking_ball")
                    {
                        // Large circle
                        float cx = (x - w / 2f) / (w / 2f);
                        float cy = (y - h / 2f) / (h / 2f);
                        if (cx * cx + cy * cy < 1f)
                            px[y * w + x] = Color.Lerp(color, Color.black, cx * cx + cy * cy);
                        else
                            px[y * w + x] = Color.clear;
                    }
                    else
                    {
                        // Hammer/drill shape: handle + head
                        bool isHandle = x >= 6 && x <= 9 && y < 20;
                        bool isHead = y >= 16 && y <= 28 && x >= 2 && x <= 13;
                        if (isHead)
                            px[y * w + x] = color;
                        else if (isHandle)
                            px[y * w + x] = new Color(0.5f, 0.35f, 0.2f); // wood handle
                        else
                            px[y * w + x] = Color.clear;
                    }
                }
            }

            tex.SetPixels(px);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.3f, 0f), 16f);
        }

        public void Render(GameSnapshot snap)
        {
            if (_view == null) return;
            var pos = GameManager.GameToWorld(snap.character.x, snap.character.y);
            _view.transform.position = pos;

            var sr = _view.SR;
            if (sr.sprite == null) sr.sprite = SpriteHelper.Character;

            // Weapon change detection
            if (snap.character.weaponSkill != _currentWeapon)
                SetWeaponSprite(snap.character.weaponSkill);

            // Detect attack: building HP decreased
            bool attacked = false;
            if (snap.building.totalHp < _lastBuildingHp && _lastBuildingHp > 0f)
                attacked = true;
            _lastBuildingHp = snap.building.totalHp;

            if (attacked && _attackAnimTimer <= 0f)
                _attackAnimTimer = SWING_DURATION;

            // Weapon swing animation
            if (_attackAnimTimer > 0f)
            {
                _attackAnimTimer -= Time.deltaTime;
                float t = 1f - (_attackAnimTimer / SWING_DURATION); // 0→1
                float angle = Mathf.Lerp(SWING_ANGLE_START, SWING_ANGLE_END, t);
                _weaponGo.transform.localRotation = Quaternion.Euler(0, 0, angle);

                // Character body slight forward lean
                float lean = Mathf.Sin(t * Mathf.PI) * 0.15f;
                _view.transform.localScale = new Vector3(1.5f, 1.5f - lean, 1f);
            }
            else
            {
                // Idle: weapon held up
                _weaponGo.transform.localRotation = Quaternion.Euler(0, 0, SWING_ANGLE_START);
                _view.transform.localScale = new Vector3(1.5f, 1.5f, 1f);

                // Gentle idle bob
                float bob = Mathf.Sin(Time.time * 3f) * 0.02f;
                var p = _view.transform.position;
                p.y += bob;
                _view.transform.position = p;
            }

            // Character color states
            if (_lastHp > 0f && snap.character.hp < _lastHp)
                sr.color = Color.red;
            else if (snap.character.invincible)
                sr.color = new Color(1f, 1f, 1f, Mathf.PingPong(Time.time * 8f, 1f));
            else if (snap.character.stunned)
                sr.color = new Color(0.5f, 0.5f, 1f);
            else
                sr.color = Color.white;

            _lastHp = snap.character.hp;
        }
    }
}
