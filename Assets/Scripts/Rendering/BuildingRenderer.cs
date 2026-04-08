using UnityEngine;
using System.Collections.Generic;
using BuildingCrusher.Core;
using BuildingCrusher.Data;
using BuildingCrusher.Entities;

namespace BuildingCrusher.Rendering
{
    public class BuildingRenderer : MonoBehaviour
    {
        [SerializeField] GameObject floorPrefab;
        [SerializeField] Transform buildingContainer;

        readonly List<BuildingView> _floorViews = new();
        string _currentBuildingId;

        public void Initialize()
        {
            if (buildingContainer == null)
            {
                var go = new GameObject("BuildingContainer");
                go.transform.SetParent(transform);
                buildingContainer = go.transform;
            }
            ClearFloors();
        }

        void ClearFloors()
        {
            foreach (var v in _floorViews) if (v != null) Destroy(v.gameObject);
            _floorViews.Clear();
            _currentBuildingId = null;
        }

        public void Render(GameSnapshot snap)
        {
            if (snap.building.defId == null) return;

            if (_currentBuildingId != snap.building.defId || _floorViews.Count != snap.building.floors.Length)
                RebuildFloorViews(snap);

            for (int i = 0; i < snap.building.floors.Length; i++)
            {
                var fs = snap.building.floors[i];
                var view = _floorViews[i];

                if (fs.destroyed)
                {
                    view.gameObject.SetActive(false);
                    continue;
                }

                view.gameObject.SetActive(true);
                float hpRatio = fs.hp / fs.maxHp;

                var bdef = BuildingDefs.ALL[snap.building.defId];
                Color c = bdef.color;
                if (hpRatio < 0.1f) c = Color.Lerp(Color.black, c, 0.3f);
                else if (hpRatio < 0.5f) c = Color.Lerp(new Color(0.5f, 0.3f, 0.1f), c, hpRatio);

                if (fs.frozen) c = Color.Lerp(c, Color.cyan, 0.5f);

                view.SetColor(c);
            }
        }

        void RebuildFloorViews(GameSnapshot snap)
        {
            ClearFloors();
            _currentBuildingId = snap.building.defId;

            float floorHeight = 0.8f;
            float floorWidth = 2f;
            float baseY = -2f;

            for (int i = 0; i < snap.building.floors.Length; i++)
            {
                GameObject go;
                if (floorPrefab != null)
                    go = Instantiate(floorPrefab, buildingContainer);
                else
                {
                    go = new GameObject($"Floor_{i}");
                    go.transform.SetParent(buildingContainer);
                    go.AddComponent<SpriteRenderer>();
                    go.AddComponent<BuildingView>();
                }

                var view = go.GetComponent<BuildingView>();
                if (view == null) view = go.AddComponent<BuildingView>();

                float yPos = baseY + i * floorHeight;
                go.transform.localPosition = new Vector3(0f, yPos, 0f);
                go.transform.localScale = new Vector3(floorWidth, floorHeight * 0.9f, 1f);

                var bdef = BuildingDefs.ALL[snap.building.defId];
                view.SetColor(bdef.color);

                _floorViews.Add(view);
            }
        }
    }
}
