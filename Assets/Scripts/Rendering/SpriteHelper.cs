using UnityEngine;
using System.Collections.Generic;

namespace BuildingCrusher.Rendering
{
    public static class SpriteHelper
    {
        static readonly Dictionary<string, Sprite> _cache = new();

        // Load from Resources/Sprites/ — tries Texture2D first, then Sprite
        static Sprite LoadFromResources(string name)
        {
            string key = $"res_{name}";
            if (_cache.TryGetValue(key, out var cached)) return cached;

            // Try loading as Sprite first
            var sprite = Resources.Load<Sprite>($"Sprites/{name}");
            if (sprite != null)
            {
                Debug.Log($"[SpriteHelper] Loaded sprite: {name}");
                _cache[key] = sprite;
                return sprite;
            }

            // Try loading as Texture2D and convert to Sprite
            var tex = Resources.Load<Texture2D>($"Sprites/{name}");
            if (tex != null)
            {
                Debug.Log($"[SpriteHelper] Loaded texture: {name} ({tex.width}x{tex.height})");
                sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f), Mathf.Max(tex.width, tex.height));
                _cache[key] = sprite;
                return sprite;
            }

            Debug.LogWarning($"[SpriteHelper] Failed to load: Sprites/{name}");
            return null;
        }

        // Character — uses Kenney p1_stand or fallback
        public static Sprite Character
        {
            get
            {
                var s = LoadFromResources("character");
                return s ?? MakeSolid("char_fallback", 24, 32, new Color(1f, 0.75f, 0.2f));
            }
        }

        // Building floor — uses Kenney house tiles or procedural fallback
        public static Sprite BuildingSprite(string defId)
        {
            string resName = defId switch
            {
                "wooden_shack"       => "building_wood",
                "brick_house"        => "building_brick",
                "concrete_building"  => "building_concrete",
                "glass_office"       => "building_glass",
                "steel_structure"    => "building_steel",
                "gas_storage"        => "building_gas",
                "electric_facility"  => "building_electric",
                "reinforced_bunker"  => "building_bunker",
                _ => null,
            };

            if (resName != null)
            {
                var s = LoadFromResources(resName);
                if (s != null) return s;
            }

            // Fallback: procedural floor with windows
            Color wallColor = defId switch
            {
                "wooden_shack"       => new Color(0.76f, 0.60f, 0.42f),
                "brick_house"        => new Color(0.80f, 0.40f, 0.30f),
                "concrete_building"  => new Color(0.60f, 0.60f, 0.65f),
                "glass_office"       => new Color(0.55f, 0.80f, 0.95f),
                "steel_structure"    => new Color(0.45f, 0.50f, 0.55f),
                "gas_storage"        => new Color(0.90f, 0.60f, 0.20f),
                "electric_facility"  => new Color(0.95f, 0.85f, 0.20f),
                "reinforced_bunker"  => new Color(0.30f, 0.35f, 0.30f),
                _ => Color.gray,
            };
            return MakeFloorSprite($"floor_{defId}", wallColor);
        }

        // Hazard
        public static Sprite HazardSprite(string type)
        {
            string resName = type switch
            {
                "debris"        => "debris",
                "glass"         => "glass_shard",
                "missile"       => "missile",
                "gas_explosion" => "explosion",
                _ => null,
            };

            if (resName != null)
            {
                var s = LoadFromResources(resName);
                if (s != null) return s;
            }

            return type switch
            {
                "debris"        => MakeSolid("debris_fb", 8, 8, new Color(0.6f, 0.5f, 0.4f)),
                "glass"         => MakeSolid("glass_fb", 6, 6, new Color(0.7f, 0.9f, 1f)),
                "missile"       => MakeSolid("missile_fb", 8, 12, Color.red),
                "gas_explosion" => MakeSolid("gas_fb", 16, 16, new Color(1f, 0.6f, 0.1f)),
                _ => MakeSolid("hazard_fb", 8, 8, Color.white),
            };
        }

        // === Procedural Generators (fallback) ===

        static Sprite MakeSolid(string key, int w, int h, Color color)
        {
            if (_cache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(w, h);
            tex.filterMode = FilterMode.Point;
            var pixels = new Color[w * h];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            tex.SetPixels(pixels);
            tex.Apply();

            var sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 16f);
            _cache[key] = sprite;
            return sprite;
        }

        static Sprite MakeFloorSprite(string key, Color wallColor)
        {
            if (_cache.TryGetValue(key, out var cached)) return cached;

            int w = 64, h = 24;
            var tex = new Texture2D(w, h);
            tex.filterMode = FilterMode.Point;
            var pixels = new Color[w * h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (x == 0 || x == w - 1 || y == 0 || y == h - 1)
                        pixels[y * w + x] = wallColor * 0.5f;
                    else if (y >= 6 && y <= 18 && ((x >= 8 && x <= 16) || (x >= 24 && x <= 32) || (x >= 40 && x <= 48) || (x >= 54 && x <= 60)))
                        pixels[y * w + x] = new Color(0.2f, 0.25f, 0.4f);
                    else
                        pixels[y * w + x] = wallColor;
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 16f);
            _cache[key] = sprite;
            return sprite;
        }
    }
}
