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
                    "power_hammer", "Power Hammer", "Slow but powerful AOE. Speed -30%, Damage +80%",
                    SkillType.PowerHammer, SkillCategory.WeaponChange,
                    attackSpeedMult: 1.3f, damageMult: 1.8f, isAoe: true
                )
            },
            {
                "drill_arm",
                new SkillDef(
                    "drill_arm", "Drill Arm", "Ultra-fast drill. DPS x3 while stationary",
                    SkillType.DrillArm, SkillCategory.WeaponChange,
                    attackSpeedMult: 0.33f, rangeMult: 0.5f
                )
            },
            {
                "demolition_crane",
                new SkillDef(
                    "demolition_crane", "Demo Crane", "Ranged attack. Range +200%, Damage -20%",
                    SkillType.DemolitionCrane, SkillCategory.WeaponChange,
                    damageMult: 0.8f, rangeMult: 3f
                )
            },
            {
                "dual_fist",
                new SkillDef(
                    "dual_fist", "Dual Fist", "Double punch. 2 hits at 60% each",
                    SkillType.DualFist, SkillCategory.WeaponChange,
                    damageMult: 0.6f
                )
            },
            {
                "wrecking_ball",
                new SkillDef(
                    "wrecking_ball", "Wrecking Ball", "Giant ball. Speed -50%, 180 AOE, Damage +120%",
                    SkillType.WreckingBall, SkillCategory.WeaponChange,
                    attackSpeedMult: 1.5f, damageMult: 2.2f, rangeMult: 1.2f, isAoe: true
                )
            },
            // Special Effect (5)
            {
                "explosion_impact",
                new SkillDef(
                    "explosion_impact", "Explosion", "Blast on hit. 30% splash to adjacent floors",
                    SkillType.ExplosionImpact, SkillCategory.SpecialEffect,
                    effectDamage: 0.3f
                )
            },
            {
                "electric_chain",
                new SkillDef(
                    "electric_chain", "Chain Lightning", "Lightning chains up to 3 floors",
                    SkillType.ElectricChain, SkillCategory.SpecialEffect,
                    effectDamage: 0.25f
                )
            },
            {
                "frost_strike",
                new SkillDef(
                    "frost_strike", "Frost Strike", "Freeze 3s. Defense=0, stops debris",
                    SkillType.FrostStrike, SkillCategory.SpecialEffect,
                    effectDuration: 3f
                )
            },
            {
                "flame_strike",
                new SkillDef(
                    "flame_strike", "Flame Strike", "Fire DOT. 20% damage/tick for 5s",
                    SkillType.FlameStrike, SkillCategory.SpecialEffect,
                    effectDamage: 0.2f, effectDuration: 5f
                )
            },
            {
                "earthquake",
                new SkillDef(
                    "earthquake", "Earthquake", "20% chance: damage all floors",
                    SkillType.Earthquake, SkillCategory.SpecialEffect,
                    effectChance: 0.2f, effectDamage: 0.1f
                )
            },
        };
    }
}
