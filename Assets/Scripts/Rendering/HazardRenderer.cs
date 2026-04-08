using UnityEngine;
using System.Collections.Generic;
using BuildingCrusher.Core;
using BuildingCrusher.Entities;
using BuildingCrusher.Managers;

namespace BuildingCrusher.Rendering
{
    public class HazardRenderer : MonoBehaviour
    {
        [SerializeField] GameObject hazardPrefab;
        [SerializeField] Transform hazardContainer;

        readonly Dictionary<int, HazardView> _views = new();

        public void Initialize()
        {
            if (hazardContainer == null)
            {
                var go = new GameObject("HazardContainer");
                go.transform.SetParent(transform);
                hazardContainer = go.transform;
            }
            foreach (var kv in _views) if (kv.Value != null) Destroy(kv.Value.gameObject);
            _views.Clear();
        }

        public void Render(GameSnapshot snap)
        {
            var activeUids = new HashSet<int>();

            foreach (var h in snap.hazards)
            {
                if (!h.active) continue;
                activeUids.Add(h.uid);

                if (!_views.ContainsKey(h.uid))
                    _views[h.uid] = CreateHazardView(h);

                var view = _views[h.uid];
                view.transform.position = GameManager.GameToWorld(h.x, h.y);
            }

            var toRemove = new List<int>();
            foreach (var kv in _views)
                if (!activeUids.Contains(kv.Key)) toRemove.Add(kv.Key);
            foreach (var uid in toRemove)
            {
                Destroy(_views[uid].gameObject);
                _views.Remove(uid);
            }
        }

        HazardView CreateHazardView(HazardSnap h)
        {
            GameObject go;
            if (hazardPrefab != null)
                go = Instantiate(hazardPrefab, hazardContainer);
            else
            {
                go = new GameObject($"Hazard_{h.uid}");
                go.transform.SetParent(hazardContainer);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = SpriteHelper.HazardSprite(h.type);
                sr.sortingOrder = 5;
                go.AddComponent<HazardView>();
            }
            go.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
            var view = go.GetComponent<HazardView>();
            if (view == null) view = go.AddComponent<HazardView>();
            view.Uid = h.uid;
            return view;
        }
    }
}
