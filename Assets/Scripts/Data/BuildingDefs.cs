using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BuildingCrusher.Data
{
    public enum BuildingHazardType
    {
        None,
        Debris,
        GlassShatter,
        GasExplosion,
        ElectricShock,
        MissileLauncher,
        ShieldRegen
    }

    public struct BuildingDef
    {
        public string id;
        public string name;
        public float baseHp;
        public float defense;
        public int minStage;
        public Color color;
        public BuildingHazardType hazardType;
        public float hazardParam;

        public BuildingDef(string id, string name, float baseHp, float defense, int minStage,
            Color color, BuildingHazardType hazardType, float hazardParam)
        {
            this.id = id;
            this.name = name;
            this.baseHp = baseHp;
            this.defense = defense;
            this.minStage = minStage;
            this.color = color;
            this.hazardType = hazardType;
            this.hazardParam = hazardParam;
        }
    }

    public static class BuildingDefs
    {
        public static readonly Dictionary<string, BuildingDef> ALL = new Dictionary<string, BuildingDef>
        {
            {
                "wooden_shack",
                new BuildingDef(
                    "wooden_shack", "나무 판잣집", 200f, 0f, 1,
                    new Color(0.76f, 0.60f, 0.42f),
                    BuildingHazardType.Debris, 3f
                )
            },
            {
                "brick_house",
                new BuildingDef(
                    "brick_house", "벽돌 주택", 400f, 0.1f, 1,
                    new Color(0.80f, 0.40f, 0.30f),
                    BuildingHazardType.Debris, 5f
                )
            },
            {
                "concrete_building",
                new BuildingDef(
                    "concrete_building", "콘크리트 빌딩", 700f, 0.2f, 3,
                    new Color(0.60f, 0.60f, 0.65f),
                    BuildingHazardType.Debris, 8f
                )
            },
            {
                "glass_office",
                new BuildingDef(
                    "glass_office", "유리 오피스", 450f, 0.05f, 5,
                    new Color(0.55f, 0.80f, 0.95f),
                    BuildingHazardType.GlassShatter, 15f
                )
            },
            {
                "steel_structure",
                new BuildingDef(
                    "steel_structure", "철골 구조물", 900f, 0.4f, 7,
                    new Color(0.45f, 0.50f, 0.55f),
                    BuildingHazardType.Debris, 10f
                )
            },
            {
                "gas_storage",
                new BuildingDef(
                    "gas_storage", "가스 저장소", 350f, 0.05f, 8,
                    new Color(0.90f, 0.60f, 0.20f),
                    BuildingHazardType.GasExplosion, 25f
                )
            },
            {
                "electric_facility",
                new BuildingDef(
                    "electric_facility", "전기 시설", 650f, 0.2f, 10,
                    new Color(0.95f, 0.85f, 0.20f),
                    BuildingHazardType.ElectricShock, 0.2f
                )
            },
            {
                "reinforced_bunker",
                new BuildingDef(
                    "reinforced_bunker", "강화 벙커", 1200f, 0.5f, 15,
                    new Color(0.30f, 0.35f, 0.30f),
                    BuildingHazardType.MissileLauncher, 20f
                )
            },
        };

        public static List<BuildingDef> AvailableAt(int stage)
        {
            return ALL.Values.Where(b => b.minStage <= stage).ToList();
        }
    }
}
