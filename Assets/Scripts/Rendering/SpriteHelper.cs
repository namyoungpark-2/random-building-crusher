using UnityEngine;
using System.Collections.Generic;

namespace BuildingCrusher.Rendering
{
    public static class SpriteHelper
    {
        static readonly Dictionary<string, Sprite> _cache = new();

        public static Sprite Load(string name)
        {
            if (_cache.TryGetValue(name, out var cached)) return cached;
            var sprite = Resources.Load<Sprite>($"Sprites/{name}");
            if (sprite != null)
            {
                _cache[name] = sprite;
                return sprite;
            }
            // Fallback: create white square
            return WhiteSquare;
        }

        static Sprite _whiteSquare;
        public static Sprite WhiteSquare
        {
            get
            {
                if (_whiteSquare == null)
                {
                    var tex = new Texture2D(4, 4);
                    var pixels = new Color[16];
                    for (int i = 0; i < 16; i++) pixels[i] = Color.white;
                    tex.SetPixels(pixels);
                    tex.Apply();
                    _whiteSquare = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
                }
                return _whiteSquare;
            }
        }

        public static Sprite Character => Load("character");

        public static Sprite BuildingSprite(string defId)
        {
            return defId switch
            {
                "wooden_shack" => Load("building_wood"),
                "brick_house" => Load("building_brick"),
                "concrete_building" => Load("building_concrete"),
                "glass_office" => Load("building_glass"),
                "steel_structure" => Load("building_steel"),
                "gas_storage" => Load("building_gas"),
                "electric_facility" => Load("building_electric"),
                "reinforced_bunker" => Load("building_bunker"),
                _ => WhiteSquare,
            };
        }

        public static Sprite HazardSprite(string type)
        {
            return type switch
            {
                "debris" => Load("debris"),
                "glass" => Load("glass_shard"),
                "missile" => Load("missile"),
                "gas_explosion" => Load("explosion"),
                _ => WhiteSquare,
            };
        }
    }
}
