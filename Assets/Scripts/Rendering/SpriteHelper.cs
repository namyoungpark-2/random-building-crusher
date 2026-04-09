using UnityEngine;
using System.Collections.Generic;

namespace BuildingCrusher.Rendering
{
    public static class SpriteHelper
    {
        static readonly Dictionary<string, Sprite> _cache = new();

        static Sprite LoadFromResources(string name)
        {
            string key = $"res_{name}";
            if (_cache.TryGetValue(key, out var cached)) return cached;

            var sprite = Resources.Load<Sprite>($"Sprites/{name}");
            if (sprite != null) { _cache[key] = sprite; return sprite; }

            var tex = Resources.Load<Texture2D>($"Sprites/{name}");
            if (tex != null)
            {
                sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f), Mathf.Max(tex.width, tex.height));
                _cache[key] = sprite;
                return sprite;
            }
            return null;
        }

        // === Character (Kenney asset) ===
        public static Sprite Character => LoadFromResources("character") ?? MakeSolid("char_fb", 24, 32, new Color(1f, 0.75f, 0.2f));

        // === Buildings (procedural — Kenney tiles don't suit floor segments) ===
        public static Sprite BuildingSprite(string defId)
        {
            Color wall, trim;
            switch (defId)
            {
                case "wooden_shack":       wall = new Color(0.76f, 0.60f, 0.42f); trim = new Color(0.55f, 0.40f, 0.25f); break;
                case "brick_house":        wall = new Color(0.75f, 0.35f, 0.25f); trim = new Color(0.55f, 0.20f, 0.15f); break;
                case "concrete_building":  wall = new Color(0.65f, 0.65f, 0.70f); trim = new Color(0.45f, 0.45f, 0.50f); break;
                case "glass_office":       wall = new Color(0.50f, 0.75f, 0.90f); trim = new Color(0.30f, 0.55f, 0.70f); break;
                case "steel_structure":    wall = new Color(0.50f, 0.55f, 0.60f); trim = new Color(0.35f, 0.38f, 0.42f); break;
                case "gas_storage":        wall = new Color(0.85f, 0.55f, 0.15f); trim = new Color(0.65f, 0.35f, 0.05f); break;
                case "electric_facility":  wall = new Color(0.90f, 0.80f, 0.15f); trim = new Color(0.70f, 0.60f, 0.05f); break;
                case "reinforced_bunker":  wall = new Color(0.35f, 0.40f, 0.35f); trim = new Color(0.20f, 0.25f, 0.20f); break;
                default:                   wall = Color.gray; trim = Color.gray * 0.6f; break;
            }
            return MakeFloorSprite($"bld_{defId}", wall, trim);
        }

        static Sprite MakeFloorSprite(string key, Color wall, Color trim)
        {
            if (_cache.TryGetValue(key, out var cached)) return cached;

            int w = 128, h = 48;
            var tex = new Texture2D(w, h);
            tex.filterMode = FilterMode.Point;
            var px = new Color[w * h];

            Color windowFrame = trim * 0.8f;
            Color windowGlass = new Color(0.15f, 0.20f, 0.35f);
            Color windowLight = new Color(0.85f, 0.80f, 0.50f, 0.6f); // some lit windows

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    // Top/bottom trim bands
                    if (y <= 2 || y >= h - 3)
                    {
                        px[y * w + x] = trim;
                        continue;
                    }
                    // Left/right edges
                    if (x <= 2 || x >= w - 3)
                    {
                        px[y * w + x] = trim;
                        continue;
                    }
                    // Windows: 4 evenly spaced
                    bool isWindow = false;
                    int[] winStarts = { 10, 38, 66, 94 };
                    int winW = 20, winH = 28;
                    int winYStart = 8;

                    foreach (int wx in winStarts)
                    {
                        if (x >= wx && x < wx + winW && y >= winYStart && y < winYStart + winH)
                        {
                            // Window frame (2px border)
                            if (x == wx || x == wx + winW - 1 || y == winYStart || y == winYStart + winH - 1)
                                px[y * w + x] = windowFrame;
                            // Cross frame
                            else if (x == wx + winW / 2 || y == winYStart + winH / 2)
                                px[y * w + x] = windowFrame;
                            // Glass (some lit, some dark)
                            else
                            {
                                bool lit = ((wx / 28) + (winYStart / 20)) % 3 == 0;
                                px[y * w + x] = lit ? windowLight : windowGlass;
                            }
                            isWindow = true;
                            break;
                        }
                    }

                    if (!isWindow)
                    {
                        // Brick pattern for brick/concrete
                        bool brickLine = (y % 8 == 0) || (x % 16 == (y / 8 % 2 == 0 ? 0 : 8));
                        if (brickLine)
                            px[y * w + x] = Color.Lerp(wall, trim, 0.3f);
                        else
                            px[y * w + x] = wall;
                    }
                }
            }

            tex.SetPixels(px);
            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 32f);
            _cache[key] = sprite;
            return sprite;
        }

        // === Hazards (Kenney assets with fallback) ===
        public static Sprite HazardSprite(string type)
        {
            string resName = type switch
            {
                "debris" => "debris",
                "glass" => "glass_shard",
                "missile" => "missile",
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
                "debris"        => MakeSolid("debris_fb", 12, 12, new Color(0.6f, 0.5f, 0.4f)),
                "glass"         => MakeSolid("glass_fb", 10, 10, new Color(0.7f, 0.9f, 1f)),
                "missile"       => MakeSolid("missile_fb", 8, 16, Color.red),
                "gas_explosion" => MakeSolid("gas_fb", 20, 20, new Color(1f, 0.6f, 0.1f)),
                _ => MakeSolid("hazard_fb", 10, 10, Color.white),
            };
        }

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
    }
}
