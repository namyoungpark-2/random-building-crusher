using UnityEngine;
using System.Collections.Generic;

namespace BuildingCrusher.Rendering
{
    public static class SpriteHelper
    {
        static readonly Dictionary<string, Sprite> _cache = new();

        static Sprite MakeSprite(string key, int w, int h, Color color)
        {
            if (_cache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(w, h);
            tex.filterMode = FilterMode.Point;
            var pixels = new Color[w * h];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            tex.SetPixels(pixels);
            tex.Apply();

            // pixelsPerUnit = 16 means 16px = 1 world unit
            var sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 16f);
            _cache[key] = sprite;
            return sprite;
        }

        public static Sprite Character =>
            MakeSprite("character", 24, 32, new Color(1f, 0.75f, 0.2f));

        public static Sprite BuildingSprite(string defId)
        {
            return defId switch
            {
                "wooden_shack"       => MakeFloorSprite("wood",     new Color(0.76f, 0.60f, 0.42f)),
                "brick_house"        => MakeFloorSprite("brick",    new Color(0.80f, 0.40f, 0.30f)),
                "concrete_building"  => MakeFloorSprite("concrete", new Color(0.60f, 0.60f, 0.65f)),
                "glass_office"       => MakeFloorSprite("glass",    new Color(0.55f, 0.80f, 0.95f)),
                "steel_structure"    => MakeFloorSprite("steel",    new Color(0.45f, 0.50f, 0.55f)),
                "gas_storage"        => MakeFloorSprite("gas",      new Color(0.90f, 0.60f, 0.20f)),
                "electric_facility"  => MakeFloorSprite("electric", new Color(0.95f, 0.85f, 0.20f)),
                "reinforced_bunker"  => MakeFloorSprite("bunker",   new Color(0.30f, 0.35f, 0.30f)),
                _ => MakeSprite("default_floor", 64, 24, Color.gray),
            };
        }

        static Sprite MakeFloorSprite(string name, Color wallColor)
        {
            string key = $"floor_{name}";
            if (_cache.TryGetValue(key, out var cached)) return cached;

            int w = 64, h = 24;
            var tex = new Texture2D(w, h);
            tex.filterMode = FilterMode.Point;
            var pixels = new Color[w * h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    // Border
                    if (x == 0 || x == w - 1 || y == 0 || y == h - 1)
                        pixels[y * w + x] = wallColor * 0.5f;
                    // Windows (dark rectangles)
                    else if (y >= 6 && y <= 18 && ((x >= 8 && x <= 16) || (x >= 24 && x <= 32) || (x >= 40 && x <= 48) || (x >= 54 && x <= 60)))
                        pixels[y * w + x] = new Color(0.2f, 0.25f, 0.4f);
                    // Wall
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

        public static Sprite HazardSprite(string type)
        {
            return type switch
            {
                "debris"        => MakeSprite("debris", 8, 8, new Color(0.6f, 0.5f, 0.4f)),
                "glass"         => MakeSprite("glass", 6, 6, new Color(0.7f, 0.9f, 1f)),
                "missile"       => MakeSprite("missile", 8, 12, Color.red),
                "gas_explosion" => MakeSprite("gas_exp", 16, 16, new Color(1f, 0.6f, 0.1f)),
                _ => MakeSprite("hazard_default", 8, 8, Color.white),
            };
        }

        public static Sprite GroundSprite =>
            MakeSprite("ground", 64, 8, new Color(0.35f, 0.55f, 0.25f));
    }
}
