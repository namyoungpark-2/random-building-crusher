using System.Collections.Generic;

namespace BuildingCrusher.Data
{
    public enum SkillType
    {
        PowerHammer,
        DrillArm,
        DemolitionCrane,
        DualFist,
        WreckingBall,
        ExplosionImpact,
        ElectricChain,
        FrostStrike,
        FlameStrike,
        Earthquake
    }

    public enum SkillCategory
    {
        WeaponChange,
        SpecialEffect
    }

    public struct SkillDef
    {
        public string id;
        public string name;
        public string description;
        public SkillType skillType;
        public SkillCategory category;
        public float attackSpeedMult;
        public float damageMult;
        public float rangeMult;
        public bool isAoe;
        public float effectChance;
        public float effectDamage;
        public float effectDuration;

        public SkillDef(string id, string name, string description,
            SkillType skillType, SkillCategory category,
            float attackSpeedMult = 1f, float damageMult = 1f, float rangeMult = 1f,
            bool isAoe = false, float effectChance = 0f, float effectDamage = 0f, float effectDuration = 0f)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.skillType = skillType;
            this.category = category;
            this.attackSpeedMult = attackSpeedMult;
            this.damageMult = damageMult;
            this.rangeMult = rangeMult;
            this.isAoe = isAoe;
            this.effectChance = effectChance;
            this.effectDamage = effectDamage;
            this.effectDuration = effectDuration;
        }
    }

    public static class SkillDefs
    {
        public static readonly Dictionary<string, SkillDef> ALL = new Dictionary<string, SkillDef>
        {
            // Weapon Change (5)
            {
                "power_hammer",
                new SkillDef(
                    "power_hammer", "파워 해머", "강력한 해머로 넓은 범위를 공격합니다.",
                    SkillType.PowerHammer, SkillCategory.WeaponChange,
                    attackSpeedMult: 1.3f, damageMult: 1.8f, isAoe: true
                )
            },
            {
                "drill_arm",
                new SkillDef(
                    "drill_arm", "드릴 암", "드릴로 좁은 범위를 빠르게 관통합니다.",
                    SkillType.DrillArm, SkillCategory.WeaponChange,
                    attackSpeedMult: 0.33f, rangeMult: 0.5f
                )
            },
            {
                "demolition_crane",
                new SkillDef(
                    "demolition_crane", "철거 크레인", "크레인으로 넓은 범위를 천천히 파괴합니다.",
                    SkillType.DemolitionCrane, SkillCategory.WeaponChange,
                    damageMult: 0.8f, rangeMult: 3f
                )
            },
            {
                "dual_fist",
                new SkillDef(
                    "dual_fist", "쌍권", "양손으로 빠르게 연타합니다.",
                    SkillType.DualFist, SkillCategory.WeaponChange,
                    damageMult: 0.6f
                )
            },
            {
                "wrecking_ball",
                new SkillDef(
                    "wrecking_ball", "레킹볼", "거대한 공으로 광역을 강타합니다.",
                    SkillType.WreckingBall, SkillCategory.WeaponChange,
                    attackSpeedMult: 1.5f, damageMult: 2.2f, rangeMult: 1.2f, isAoe: true
                )
            },
            // Special Effect (5)
            {
                "explosion_impact",
                new SkillDef(
                    "explosion_impact", "폭발 충격", "공격 시 폭발을 일으켜 추가 피해를 줍니다.",
                    SkillType.ExplosionImpact, SkillCategory.SpecialEffect,
                    effectDamage: 0.3f
                )
            },
            {
                "electric_chain",
                new SkillDef(
                    "electric_chain", "전기 연쇄", "전기가 인근 구조물로 연쇄됩니다.",
                    SkillType.ElectricChain, SkillCategory.SpecialEffect,
                    effectDamage: 0.25f
                )
            },
            {
                "frost_strike",
                new SkillDef(
                    "frost_strike", "냉기 타격", "공격 시 대상을 일시적으로 빙결시킵니다.",
                    SkillType.FrostStrike, SkillCategory.SpecialEffect,
                    effectDuration: 3f
                )
            },
            {
                "flame_strike",
                new SkillDef(
                    "flame_strike", "화염 타격", "공격 시 화염을 부착하여 지속 피해를 줍니다.",
                    SkillType.FlameStrike, SkillCategory.SpecialEffect,
                    effectDamage: 0.2f, effectDuration: 5f
                )
            },
            {
                "earthquake",
                new SkillDef(
                    "earthquake", "지진", "일정 확률로 지진을 발동해 광역 피해를 줍니다.",
                    SkillType.Earthquake, SkillCategory.SpecialEffect,
                    effectChance: 0.2f, effectDamage: 0.1f
                )
            },
        };
    }
}
