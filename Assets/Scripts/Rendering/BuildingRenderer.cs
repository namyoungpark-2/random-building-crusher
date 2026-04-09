using UnityEngine;
using System.Collections.Generic;
using BuildingCrusher.Core;
using BuildingCrusher.Data;
using BuildingCrusher.Entities;

namespace BuildingCrusher.Rendering
{
    public class BuildingRenderer : MonoBehaviour
    {
        [SerializeField] Transform buildingContainer;

        readonly List<BuildingView> _floorViews = new();
        string _currentBuildingId;
        int _currentFloorCount;

        public void Initialize()
        {
            if (buildingContainer == null)
            {
                var go = new GameObject("BuildingContainer");
                go.transform.SetParent(transform);
                buildingContainer = go.transform;
            }
            ClearFloors();
            Debug.Log("[BuildingRenderer] Initialized");
        }

        void ClearFloors()
        {
            foreach (var v in _floorViews) if (v != null) Destroy(v.gameObject);
            _floorViews.Clear();
            _currentBuildingId = null;
            _currentFloorCount = 0;
        }

        float _shakeTimer;
        float _lastTotalHp = -1f;

        public void Render(GameSnapshot snap)
        {
            if (snap.building.defId == null) return;

            if (_currentBuildingId != snap.building.defId || _currentFloorCount != snap.building.floors.Length)
                RebuildFloorViews(snap);

            // Shake on hit
            if (_lastTotalHp > 0f && snap.building.totalHp < _lastTotalHp)
                _shakeTimer = 0.1f;
            _lastTotalHp = snap.building.totalHp;

            if (_shakeTimer > 0f)
            {
                _shakeTimer -= Time.deltaTime;
                float shake = _shakeTimer * 3f;
                buildingContainer.localPosition = new Vector3(
                    Random.Range(-shake, shake), Random.Range(-shake, shake), 0f);
            }
            else
            {
                buildingContainer.localPosition = Vector3.zero;
            }

            for (int i = 0; i < snap.building.floors.Length; i++)
            {
                if (i >= _floorViews.Count) break;
                var fs = snap.building.floors[i];
                var view = _floorViews[i];

                if (fs.destroyed)
                {
                    view.gameObject.SetActive(false);
                    continue;
                }

                view.gameObject.SetActive(true);
                float hpRatio = fs.hp / fs.maxHp;

                // Color based on damage level
                Color c = Color.white;
                if (hpRatio < 0.1f) c = new Color(0.4f, 0.35f, 0.3f);
                else if (hpRatio < 0.3f) c = new Color(0.6f, 0.5f, 0.4f);
                else if (hpRatio < 0.5f) c = new Color(0.8f, 0.7f, 0.6f);
                if (fs.frozen) c = new Color(0.5f, 0.8f, 1f);

                view.SetColor(c);
            }
        }

        void RebuildFloorViews(GameSnapshot snap)
        {
            ClearFloors();
            _currentBuildingId = snap.building.defId;
            _currentFloorCount = snap.building.floors.Length;

            float floorHeight = 1.5f;
            float floorWidth = 4f;
            float baseY = -4f;

            Debug.Log($"[BuildingRenderer] Building {snap.building.defId} with {snap.building.floors.Length} floors");

            for (int i = 0; i < snap.building.floors.Length; i++)
            {
                var go = new GameObject($"Floor_{i}");
                go.transform.SetParent(buildingContainer);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = SpriteHelper.BuildingSprite(snap.building.defId);
                sr.sortingOrder = 1;
                var view = go.AddComponent<BuildingView>();

                float yPos = baseY + i * floorHeight;
                go.transform.localPosition = new Vector3(0f, yPos, 0f);
                go.transform.localScale = new Vector3(floorWidth, floorHeight * 0.95f, 1f);

                Debug.Log($"[BuildingRenderer] Floor {i} at y={yPos}, sprite={sr.sprite != null}");

                _floorViews.Add(view);
            }
        }
    }
}
