using System.Collections.Generic;
using BuildingCrusher.Data;

namespace BuildingCrusher.Core
{
    public class FloorInstance
    {
        public float hp;
        public float maxHp;
        public bool destroyed;
        public float freezeTimer;

        public FloorInstance(float maxHp)
        {
            this.maxHp = maxHp;
            this.hp = maxHp;
            this.destroyed = false;
            this.freezeTimer = 0f;
        }
    }

    public class BuildingInstance
    {
        public string defId;
        public List<FloorInstance> floors;
        public float totalHp;
        public float totalMaxHp;
        public float fireTimer;
        public float fireDamage;

        public int CurrentFloorIndex()
        {
            for (int i = 0; i < floors.Count; i++)
            {
                if (!floors[i].destroyed)
                    return i;
            }
            return -1;
        }

        public bool AllDestroyed()
        {
            foreach (var floor in floors)
            {
                if (!floor.destroyed)
                    return false;
            }
            return true;
        }
    }

    public class CharacterState
    {
        public float x;
        public float y;
        public float hp;
        public float maxHp;
        public float targetX;
        public float targetY;
        public bool moving;
        public float attackCooldown;
        public float invincibilityTimer;
        public float stunTimer;
        public int comboCount;
        public float focusStacks;
    }

    public class HazardInstance
    {
        public int uid;
        public float x;
        public float y;
        public float vx;
        public float vy;
        public float damage;
        public float lifetime;
        public string type;
        public bool active;
    }

    public class AbilityState
    {
        public string id;
        public int stacks;
    }

    public class FloatingText
    {
        public int uid;
        public float x;
        public float y;
        public string text;
        public float ttl;
        public bool isCrit;
    }

    public class GameState
    {
        // Game flow
        public int stage = 1;
        public float stageTimer;
        public float carryOverTime;
        public bool gameOver;
        public bool paused;
        public string gameOverReason;

        // Core state
        public CharacterState character;
        public BuildingInstance building;

        // Hazards
        public List<HazardInstance> hazards = new();
        public int nextHazardUid;

        // Level-up
        public int level = 1;
        public float exp;
        public float expRequired;
        public bool levelUpPending;
        public List<string> levelUpChoices = new();
        public Dictionary<string, int> abilityStacks = new();
        public HashSet<string> acquiredSkills = new();

        // Score
        public int buildingsDestroyed;
        public float totalDamage;
        public int highestStage;

        // Floating texts
        public List<FloatingText> floatingTexts = new();
        public int nextFloatUid;

        // Environment
        public float envHazardTimer;
        public string activeEnvHazard;
        public float envHazardDuration;
    }
}
