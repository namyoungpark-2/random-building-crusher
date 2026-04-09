using System.Collections.Generic;

namespace BuildingCrusher.Data
{
    public enum AbilityCategory
    {
        Attack,
        Movement,
        Survival,
        Reward,
        Special
    }

    public enum StatType
    {
        AttackDamage,
        CritChance,
        CritDamage,
        AttackSpeed,
        Penetration,
        AttackRange,
        MoveSpeed,
        DashDistance,
        DodgeSize,
        MaxHp,
        Defense,
        Regeneration,
        InvincibilityTime,
        ExpBoost,
        ScoreBoost,
        BonusTime,
        ComboHit,
        FinishingBlow,
        DebrisResist,
        Focus
    }

    public struct AbilityDef
    {
        public string id;
        public string name;
        public string description;
        public StatType statType;
        public AbilityCategory category;
        public float valuePerStack;
        public int maxStack;

        public AbilityDef(string id, string name, string description,
            StatType statType, AbilityCategory category, float valuePerStack, int maxStack = 5)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.statType = statType;
            this.category = category;
            this.valuePerStack = valuePerStack;
            this.maxStack = maxStack;
        }
    }

    public static class AbilityDefs
    {
        public static readonly Dictionary<string, AbilityDef> ALL = new Dictionary<string, AbilityDef>
        {
            // Attack (6)
            {
                "attack_damage",
                new AbilityDef(
                    "attack_damage", "Power Up", "Attack +15%",
                    StatType.AttackDamage, AbilityCategory.Attack, 0.15f
                )
            },
            {
                "crit_chance",
                new AbilityDef(
                    "crit_chance", "Critical Eye", "Crit chance +8%",
                    StatType.CritChance, AbilityCategory.Attack, 0.08f
                )
            },
            {
                "crit_damage",
                new AbilityDef(
                    "crit_damage", "Critical Power", "Crit damage +25%",
                    StatType.CritDamage, AbilityCategory.Attack, 0.25f
                )
            },
            {
                "attack_speed",
                new AbilityDef(
                    "attack_speed", "Quick Strike", "Attack speed +10%",
                    StatType.AttackSpeed, AbilityCategory.Attack, -0.10f
                )
            },
            {
                "penetration",
                new AbilityDef(
                    "penetration", "Piercing", "Ignore defense +10%",
                    StatType.Penetration, AbilityCategory.Attack, 0.10f
                )
            },
            {
                "attack_range",
                new AbilityDef(
                    "attack_range", "Wide Swing", "Attack range +15%",
                    StatType.AttackRange, AbilityCategory.Attack, 0.15f
                )
            },
            // Movement (3)
            {
                "move_speed",
                new AbilityDef(
                    "move_speed", "Swift Feet", "Move speed +12%",
                    StatType.MoveSpeed, AbilityCategory.Movement, 0.12f
                )
            },
            {
                "dash_distance",
                new AbilityDef(
                    "dash_distance", "Long Dash", "Dash distance +20%",
                    StatType.DashDistance, AbilityCategory.Movement, 0.20f
                )
            },
            {
                "dodge_size",
                new AbilityDef(
                    "dodge_size", "Nimble", "Dodge hitbox -15%",
                    StatType.DodgeSize, AbilityCategory.Movement, -0.15f
                )
            },
            // Survival (4)
            {
                "max_hp",
                new AbilityDef(
                    "max_hp", "Tough Body", "Max HP +15%",
                    StatType.MaxHp, AbilityCategory.Survival, 0.15f
                )
            },
            {
                "defense",
                new AbilityDef(
                    "defense", "Iron Skin", "Damage taken -10%",
                    StatType.Defense, AbilityCategory.Survival, 0.10f
                )
            },
            {
                "regeneration",
                new AbilityDef(
                    "regeneration", "Recovery", "Heal 5% on clear",
                    StatType.Regeneration, AbilityCategory.Survival, 0.05f
                )
            },
            {
                "invincibility",
                new AbilityDef(
                    "invincibility", "Shield Time", "Invincibility +0.3s",
                    StatType.InvincibilityTime, AbilityCategory.Survival, 0.3f
                )
            },
            // Reward (3)
            {
                "exp_boost",
                new AbilityDef(
                    "exp_boost", "Fast Learner", "EXP gain +15%",
                    StatType.ExpBoost, AbilityCategory.Reward, 0.15f
                )
            },
            {
                "score_boost",
                new AbilityDef(
                    "score_boost", "Score Master", "Score +10%",
                    StatType.ScoreBoost, AbilityCategory.Reward, 0.10f
                )
            },
            {
                "bonus_time",
                new AbilityDef(
                    "bonus_time", "Time Bonus", "Clear time +2s",
                    StatType.BonusTime, AbilityCategory.Reward, 2f
                )
            },
            // Special (4)
            {
                "combo_hit",
                new AbilityDef(
                    "combo_hit", "Combo Strike", "Combo damage +3% per hit (max 30%)",
                    StatType.ComboHit, AbilityCategory.Special, 0.03f
                )
            },
            {
                "finishing_blow",
                new AbilityDef(
                    "finishing_blow", "Finisher", "2x damage when HP below 20%",
                    StatType.FinishingBlow, AbilityCategory.Special, 1f, 1
                )
            },
            {
                "debris_resist",
                new AbilityDef(
                    "debris_resist", "Hard Hat", "Debris damage -20%",
                    StatType.DebrisResist, AbilityCategory.Special, -0.20f
                )
            },
            {
                "focus",
                new AbilityDef(
                    "focus", "Focus", "Attack speed +5% per consecutive hit",
                    StatType.Focus, AbilityCategory.Special, 0.05f
                )
            },
        };
    }
}
