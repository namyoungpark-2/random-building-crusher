using System.Collections.Generic;

namespace BuildingCrusher.Core
{
    public struct FloorSnap
    {
        public float hp, maxHp;
        public bool destroyed;
        public bool frozen;
    }

    public struct BuildingSnap
    {
        public string defId;
        public FloorSnap[] floors;
        public float totalHp, totalMaxHp;
        public bool onFire;
        public int currentFloorIndex;
    }

    public struct CharacterSnap
    {
        public float x, y;
        public float hp, maxHp;
        public bool moving;
        public bool stunned;
        public bool invincible;
        public int level;
        public string weaponSkill;
    }

    public struct HazardSnap
    {
        public int uid;
        public float x, y;
        public string type;
        public bool active;
    }

    public struct FloatTextSnap
    {
        public int uid;
        public float x, y;
        public string text;
        public float ttl;
        public bool isCrit;
    }

    public struct AbilitySnap
    {
        public string id;
        public int stacks;
        public int maxStacks;
    }

    public class GameSnapshot
    {
        public int stage;
        public float stageTimer;
        public bool gameOver;
        public string gameOverReason;
        public bool paused;
        public bool levelUpPending;

        public BuildingSnap building;
        public CharacterSnap character;
        public HazardSnap[] hazards;
        public FloatTextSnap[] floatingTexts;

        public string[] levelUpChoices;

        public AbilitySnap[] abilities;
        public string[] skills;

        public float exp, expRequired;
        public int level;

        public int buildingsDestroyed;
        public float totalDamage;
        public int score;

        public string activeEnvHazard;

        public static GameSnapshot Create(GameState s)
        {
            var snap = new GameSnapshot
            {
                stage = s.stage,
                stageTimer = s.stageTimer,
                gameOver = s.gameOver,
                gameOverReason = s.gameOverReason,
                paused = s.paused,
                levelUpPending = s.levelUpPending,
                exp = s.exp,
                expRequired = s.expRequired,
                level = s.level,
                buildingsDestroyed = s.buildingsDestroyed,
                totalDamage = s.totalDamage,
                score = GameEngine.CalculateScore(s),
                activeEnvHazard = s.activeEnvHazard,
            };

            if (s.building != null)
            {
                var floors = new FloorSnap[s.building.floors.Count];
                for (int i = 0; i < floors.Length; i++)
                {
                    var f = s.building.floors[i];
                    floors[i] = new FloorSnap
                    {
                        hp = f.hp, maxHp = f.maxHp,
                        destroyed = f.destroyed,
                        frozen = f.freezeTimer > 0f,
                    };
                }
                snap.building = new BuildingSnap
                {
                    defId = s.building.defId,
                    floors = floors,
                    totalHp = s.building.totalHp,
                    totalMaxHp = s.building.totalMaxHp,
                    onFire = s.building.fireTimer > 0f,
                    currentFloorIndex = s.building.CurrentFloorIndex(),
                };
            }

            string weaponSkill = null;
            foreach (var sk in s.acquiredSkills)
            {
                if (Data.SkillDefs.ALL.TryGetValue(sk, out var def)
                    && def.category == Data.SkillCategory.WeaponChange)
                {
                    weaponSkill = sk;
                    break;
                }
            }

            snap.character = new CharacterSnap
            {
                x = s.character.x, y = s.character.y,
                hp = s.character.hp, maxHp = s.character.maxHp,
                moving = s.character.moving,
                stunned = s.character.stunTimer > 0f,
                invincible = s.character.invincibilityTimer > 0f,
                level = s.level,
                weaponSkill = weaponSkill,
            };

            snap.hazards = new HazardSnap[s.hazards.Count];
            for (int i = 0; i < s.hazards.Count; i++)
            {
                var h = s.hazards[i];
                snap.hazards[i] = new HazardSnap { uid = h.uid, x = h.x, y = h.y, type = h.type, active = h.active };
            }

            snap.floatingTexts = new FloatTextSnap[s.floatingTexts.Count];
            for (int i = 0; i < s.floatingTexts.Count; i++)
            {
                var ft = s.floatingTexts[i];
                snap.floatingTexts[i] = new FloatTextSnap
                {
                    uid = ft.uid, x = ft.x, y = ft.y,
                    text = ft.text, ttl = ft.ttl, isCrit = ft.isCrit,
                };
            }

            snap.levelUpChoices = s.levelUpChoices.ToArray();

            var abilityList = new List<AbilitySnap>();
            foreach (var kv in s.abilityStacks)
            {
                if (Data.AbilityDefs.ALL.TryGetValue(kv.Key, out var aDef))
                    abilityList.Add(new AbilitySnap { id = kv.Key, stacks = kv.Value, maxStacks = aDef.maxStack });
            }
            snap.abilities = abilityList.ToArray();
            snap.skills = new List<string>(s.acquiredSkills).ToArray();

            return snap;
        }
    }
}
