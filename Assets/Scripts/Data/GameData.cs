using UnityEngine;

namespace BuildingCrusher.Data
{
    public static class GameData
    {
        public const float WORLD_WIDTH = 1080f;
        public const float WORLD_HEIGHT = 1920f;
        public const float BASE_STAGE_TIME = 30f;
        public const float BASE_HP = 100f;
        public const float BASE_ATTACK_DAMAGE = 10f;
        public const float BASE_ATTACK_INTERVAL = 0.5f;
        public const float BASE_MOVE_SPEED = 300f;
        public const float BASE_ATTACK_RANGE = 80f;
        public const float BASE_CRIT_CHANCE = 0.05f;
        public const float BASE_CRIT_MULTIPLIER = 1.5f;
        public const float INVINCIBILITY_DURATION = 0.5f;
        public const float BASE_EXP_REQUIRED = 100f;
        public const float EXP_SCALING = 1.2f;
        public const float BUILDING_HP_SCALE_PER_STAGE = 0.15f;
        public const int SCORE_PER_BUILDING = 100;
        public const int SCORE_PER_STAGE = 500;
        public const int SCORE_HP_BONUS_MAX = 1000;

        public static int FloorsForStage(int stage) => Mathf.Min(3 + stage / 3, 8);
        public static float ExpForLevel(int level) => BASE_EXP_REQUIRED * Mathf.Pow(EXP_SCALING, level - 1);
        public static float BuildingHpAtStage(float baseHp, int stage) => baseHp * (1f + stage * BUILDING_HP_SCALE_PER_STAGE);
    }
}
