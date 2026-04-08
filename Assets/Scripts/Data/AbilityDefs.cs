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
                    "attack_damage", "공격력 강화", "공격력이 15% 증가합니다.",
                    StatType.AttackDamage, AbilityCategory.Attack, 0.15f
                )
            },
            {
                "crit_chance",
                new AbilityDef(
                    "crit_chance", "치명타 확률", "치명타 확률이 8% 증가합니다.",
                    StatType.CritChance, AbilityCategory.Attack, 0.08f
                )
            },
            {
                "crit_damage",
                new AbilityDef(
                    "crit_damage", "치명타 피해", "치명타 피해가 25% 증가합니다.",
                    StatType.CritDamage, AbilityCategory.Attack, 0.25f
                )
            },
            {
                "attack_speed",
                new AbilityDef(
                    "attack_speed", "공격 속도", "공격 간격이 10% 감소합니다.",
                    StatType.AttackSpeed, AbilityCategory.Attack, -0.10f
                )
            },
            {
                "penetration",
                new AbilityDef(
                    "penetration", "관통력", "방어력 무시가 10% 증가합니다.",
                    StatType.Penetration, AbilityCategory.Attack, 0.10f
                )
            },
            {
                "attack_range",
                new AbilityDef(
                    "attack_range", "공격 범위", "공격 범위가 15% 증가합니다.",
                    StatType.AttackRange, AbilityCategory.Attack, 0.15f
                )
            },
            // Movement (3)
            {
                "move_speed",
                new AbilityDef(
                    "move_speed", "이동 속도", "이동 속도가 12% 증가합니다.",
                    StatType.MoveSpeed, AbilityCategory.Movement, 0.12f
                )
            },
            {
                "dash_distance",
                new AbilityDef(
                    "dash_distance", "대시 거리", "대시 거리가 20% 증가합니다.",
                    StatType.DashDistance, AbilityCategory.Movement, 0.20f
                )
            },
            {
                "dodge_size",
                new AbilityDef(
                    "dodge_size", "회피 범위", "회피 판정 크기가 15% 감소합니다.",
                    StatType.DodgeSize, AbilityCategory.Movement, -0.15f
                )
            },
            // Survival (4)
            {
                "max_hp",
                new AbilityDef(
                    "max_hp", "최대 체력", "최대 체력이 15% 증가합니다.",
                    StatType.MaxHp, AbilityCategory.Survival, 0.15f
                )
            },
            {
                "defense",
                new AbilityDef(
                    "defense", "방어력", "방어력이 10% 증가합니다.",
                    StatType.Defense, AbilityCategory.Survival, 0.10f
                )
            },
            {
                "regeneration",
                new AbilityDef(
                    "regeneration", "재생력", "체력 재생이 5% 증가합니다.",
                    StatType.Regeneration, AbilityCategory.Survival, 0.05f
                )
            },
            {
                "invincibility",
                new AbilityDef(
                    "invincibility", "무적 시간", "피격 후 무적 시간이 0.3초 증가합니다.",
                    StatType.InvincibilityTime, AbilityCategory.Survival, 0.3f
                )
            },
            // Reward (3)
            {
                "exp_boost",
                new AbilityDef(
                    "exp_boost", "경험치 획득", "경험치 획득량이 15% 증가합니다.",
                    StatType.ExpBoost, AbilityCategory.Reward, 0.15f
                )
            },
            {
                "score_boost",
                new AbilityDef(
                    "score_boost", "점수 획득", "점수 획득량이 10% 증가합니다.",
                    StatType.ScoreBoost, AbilityCategory.Reward, 0.10f
                )
            },
            {
                "bonus_time",
                new AbilityDef(
                    "bonus_time", "보너스 시간", "스테이지 제한 시간이 2초 증가합니다.",
                    StatType.BonusTime, AbilityCategory.Reward, 2f
                )
            },
            // Special (4)
            {
                "combo_hit",
                new AbilityDef(
                    "combo_hit", "콤보 타격", "콤보 시 추가 피해가 3% 증가합니다.",
                    StatType.ComboHit, AbilityCategory.Special, 0.03f
                )
            },
            {
                "finishing_blow",
                new AbilityDef(
                    "finishing_blow", "피니시 블로우", "체력이 낮은 건물에 추가 피해를 줍니다.",
                    StatType.FinishingBlow, AbilityCategory.Special, 1f, 1
                )
            },
            {
                "debris_resist",
                new AbilityDef(
                    "debris_resist", "잔해 저항", "잔해 피해가 20% 감소합니다.",
                    StatType.DebrisResist, AbilityCategory.Special, -0.20f
                )
            },
            {
                "focus",
                new AbilityDef(
                    "focus", "집중력", "공격 집중 시 피해가 5% 증가합니다.",
                    StatType.Focus, AbilityCategory.Special, 0.05f
                )
            },
        };
    }
}
