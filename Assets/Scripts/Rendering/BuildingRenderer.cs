using UnityEngine;
using System.Collections.Generic;
using BuildingCrusher.Core;
using BuildingCrusher.Data;
using BuildingCrusher.Entities;
using BuildingCrusher.Managers;

namespace BuildingCrusher.Rendering
{
    public class BuildingRenderer : MonoBehaviour
    {
        [SerializeField] Transform buildingContainer;

        readonly List<BuildingView> _floorViews = new();
        readonly List<GameObject> _debrisParticles = new();
        string _currentBuildingId;
        int _currentFloorCount;
        float _shakeTimer;
        float _lastTotalHp = -1f;
        float[] _lastFloorHp;

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
            foreach (var d in _debrisParticles) if (d != null) Destroy(d);
            _debrisParticles.Clear();
            _currentBuildingId = null;
            _currentFloorCount = 0;
            _lastFloorHp = null;
        }

        public void Render(GameSnapshot snap)
        {
            if (snap.building.defId == null) return;

            if (_currentBuildingId != snap.building.defId || _currentFloorCount != snap.building.floors.Length)
                RebuildFloorViews(snap);

            // Shake on hit
            bool wasHit = _lastTotalHp > 0f && snap.building.totalHp < _lastTotalHp;
            if (wasHit) _shakeTimer = 0.12f;
            _lastTotalHp = snap.building.totalHp;

            if (_shakeTimer > 0f)
            {
                _shakeTimer -= Time.deltaTime;
                float shake = _shakeTimer * 4f;
                buildingContainer.localPosition = new Vector3(
                    Random.Range(-shake, shake), Random.Range(-shake * 0.5f, shake * 0.5f), 0f);
            }
            else
            {
                buildingContainer.localPosition = Vector3.zero;
            }

            // Update each floor
            for (int i = 0; i < snap.building.floors.Length; i++)
            {
                if (i >= _floorViews.Count) break;
                var fs = snap.building.floors[i];
                var view = _floorViews[i];

                if (fs.destroyed)
                {
                    if (view.gameObject.activeSelf)
                    {
                        // Spawn destruction debris particles
                        SpawnFloorDebris(view.transform.position);
                        view.gameObject.SetActive(false);
                    }
                    continue;
                }

                view.gameObject.SetActive(true);
                float hpRatio = fs.hp / fs.maxHp;

                // Hit flash on the specific floor being attacked
                bool floorHit = _lastFloorHp != null && i < _lastFloorHp.Length
                    && _lastFloorHp[i] > 0f && fs.hp < _lastFloorHp[i];

                Color c;
                if (floorHit)
                    c = Color.white; // flash white on hit
                else if (fs.frozen)
                    c = new Color(0.5f, 0.8f, 1f);
                else if (hpRatio < 0.1f)
                    c = new Color(0.4f, 0.35f, 0.3f);
                else if (hpRatio < 0.3f)
                    c = new Color(0.65f, 0.55f, 0.45f);
                else if (hpRatio < 0.6f)
                    c = new Color(0.85f, 0.75f, 0.65f);
                else
                    c = Color.white;

                view.SetColor(c);

                // Slight tilt when damaged
                if (hpRatio < 0.3f)
                {
                    float tilt = Mathf.Sin(Time.time * 2f) * (1f - hpRatio) * 2f;
                    view.transform.localRotation = Quaternion.Euler(0, 0, tilt);
                }
                else
                {
                    view.transform.localRotation = Quaternion.identity;
                }
            }

            // Save floor HPs for next frame comparison
            _lastFloorHp = new float[snap.building.floors.Length];
            for (int i = 0; i < snap.building.floors.Length; i++)
                _lastFloorHp[i] = snap.building.floors[i].hp;

            // Clean up old debris particles
            for (int i = _debrisParticles.Count - 1; i >= 0; i--)
            {
                if (_debrisParticles[i] == null)
                {
                    _debrisParticles.RemoveAt(i);
                    continue;
                }
                // Simple gravity fall + fade
                var dt = _debrisParticles[i].transform;
                dt.position += new Vector3(
                    Random.Range(-0.5f, 0.5f) * Time.deltaTime,
                    -3f * Time.deltaTime,
                    0f);
                var dsr = dt.GetComponent<SpriteRenderer>();
                if (dsr != null)
                {
                    var col = dsr.color;
                    col.a -= Time.deltaTime * 2f;
                    if (col.a <= 0f)
                    {
                        Destroy(_debrisParticles[i]);
                        _debrisParticles.RemoveAt(i);
                        continue;
                    }
                    dsr.color = col;
                }
            }
        }

        void SpawnFloorDebris(Vector3 floorPos)
        {
            for (int i = 0; i < 8; i++)
            {
                var go = new GameObject("Debris");
                go.transform.SetParent(transform);
                go.transform.position = floorPos + new Vector3(
                    Random.Range(-1.5f, 1.5f), Random.Range(-0.3f, 0.5f), 0f);
                go.transform.localScale = new Vector3(
                    Random.Range(0.1f, 0.3f), Random.Range(0.1f, 0.3f), 1f);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = SpriteHelper.HazardSprite("debris");
                sr.sortingOrder = 3;
                sr.color = new Color(
                    Random.Range(0.5f, 0.8f),
                    Random.Range(0.4f, 0.6f),
                    Random.Range(0.3f, 0.5f));
                _debrisParticles.Add(go);
            }
        }

        void RebuildFloorViews(GameSnapshot snap)
        {
            ClearFloors();
            _currentBuildingId = snap.building.defId;
            _currentFloorCount = snap.building.floors.Length;

            float floorHeight = 1.5f;
            float floorWidth = 4f;
            Vector3 baseWorld = GameManager.GameToWorld(GameData.BUILDING_X, GameData.BUILDING_BASE_Y);
            float baseY = baseWorld.y;

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

                _floorViews.Add(view);
            }

            _lastFloorHp = new float[snap.building.floors.Length];
            for (int i = 0; i < snap.building.floors.Length; i++)
                _lastFloorHp[i] = snap.building.floors[i].hp;
        }
    }
}
