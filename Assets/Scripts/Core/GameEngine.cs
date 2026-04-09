using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using BuildingCrusher.Data;

namespace BuildingCrusher.Core
{
    public static class GameEngine
    {
        // ─── Helpers ────────────────────────────────────────────────────────
        static float Dist(float ax, float ay, float bx, float by) =>
            Mathf.Sqrt((ax - bx) * (ax - bx) + (ay - by) * (ay - by));

        // ─── State Factory ──────────────────────────────────────────────────
        public static GameState CreateNewGame()
        {
            var state = new GameState
            {
                stage = 1,
                stageTimer = GameData.BASE_STAGE_TIME,
                carryOverTime = 0f,
                expRequired = GameData.ExpForLevel(1),
                character = new CharacterState
                {
                    x = GameData.WORLD_WIDTH * 0.5f,
                    y = GameData.WORLD_HEIGHT * 0.55f,
                    hp = GameData.BASE_HP,
                    maxHp = GameData.BASE_HP,
                    targetX = GameData.WORLD_WIDTH * 0.5f,
                    targetY = GameData.WORLD_HEIGHT * 0.55f,
                },
            };
            SpawnBuilding(state);
            return state;
        }

        // ─── Building Spawn ─────────────────────────────────────────────────
        public static void SpawnBuilding(GameState s)
        {
            var available = BuildingDefs.AvailableAt(s.stage);
            var def = available[Random.Range(0, available.Count)];
            int floorCount = GameData.FloorsForStage(s.stage);

            float floorHp = GameData.BuildingHpAtStage(def.baseHp, s.stage) / floorCount;

            var building = new BuildingInstance
            {
                defId = def.id,
                floors = new List<FloorInstance>(),
                totalMaxHp = 0f,
            };

            for (int i = 0; i < floorCount; i++)
            {
                building.floors.Add(new FloorInstance(floorHp));
                building.totalMaxHp += floorHp;
            }
            building.totalHp = building.totalMaxHp;

            s.building = building;
            s.stageTimer = GameData.BASE_STAGE_TIME + s.carryOverTime;
            s.carryOverTime = 0f;
        }

        // ─── Main Update ────────────────────────────────────────────────────
        public static void UpdateGame(GameState s, float dt)
        {
            if (s.gameOver || s.paused || s.levelUpPending) return;

            UpdateTimer(s, dt);
            if (s.gameOver) return;

            UpdateCharacter(s, dt);
            UpdateBuilding(s, dt);
            UpdateHazards(s, dt);
            UpdateEnvironment(s, dt);
            UpdateExp(s);
            UpdateFloatingTexts(s, dt);
        }

        // ─── Timer ──────────────────────────────────────────────────────────
        static void UpdateTimer(GameState s, float dt)
        {
            s.stageTimer -= dt;
            if (s.stageTimer <= 0f)
            {
                s.stageTimer = 0f;
                s.gameOver = true;
                s.gameOverReason = "time";
            }
        }

        // ─── Character ─────────────────────────────────────────────────────
        static void UpdateCharacter(GameState s, float dt)
        {
            var c = s.character;

            // Invincibility cooldown
            if (c.invincibilityTimer > 0f) c.invincibilityTimer -= dt;
            // Stun
            if (c.stunTimer > 0f) { c.stunTimer -= dt; return; }

            // Auto-approach building if not moving manually
            float buildingX = GameData.WORLD_WIDTH * 0.5f;
            float buildingY = GameData.WORLD_HEIGHT * 0.5f + 150f; // stand near bottom of building
            float range = GetAttackRange(s);
            float distToBuilding = Dist(c.x, c.y, buildingX, buildingY);

            if (!c.moving && distToBuilding > range + 50f && s.building != null && !s.building.AllDestroyed())
            {
                // Auto-walk toward building
                c.targetX = buildingX;
                c.targetY = buildingY;
                c.moving = true;
            }

            // Movement
            if (c.moving)
            {
                float speed = GetMoveSpeed(s);
                float dx = c.targetX - c.x;
                float dy = c.targetY - c.y;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                if (dist < speed * dt)
                {
                    c.x = c.targetX;
                    c.y = c.targetY;
                    c.moving = false;
                }
                else
                {
                    c.x += dx / dist * speed * dt;
                    c.y += dy / dist * speed * dt;
                }
            }

            // Attack
            c.attackCooldown -= dt;
            if (c.attackCooldown <= 0f && s.building != null && !s.building.AllDestroyed())
            {
                float attackDist = Dist(c.x, c.y, buildingX, buildingY);

                if (attackDist <= range + 200f)
                {
                    PerformAttack(s);
                    c.attackCooldown = GetAttackInterval(s);
                }
                else
                {
                    c.comboCount = 0;
                    c.focusStacks = 0f;
                }
            }
        }

        // ─── Attack ─────────────────────────────────────────────────────────
        static void PerformAttack(GameState s)
        {
            var c = s.character;
            int floorIdx = s.building.CurrentFloorIndex();
            if (floorIdx < 0) return;

            var floor = s.building.floors[floorIdx];
            var bdef = BuildingDefs.ALL[s.building.defId];

            float damage = GetAttackDamage(s);

            // Crit check
            bool isCrit = Random.value < GetCritChance(s);
            if (isCrit) damage *= GetCritMultiplier(s);

            // Combo hit bonus
            float comboBonus = GetAbilityValue(s, "combo_hit");
            if (comboBonus > 0f)
            {
                c.comboCount++;
                float comboMult = 1f + Mathf.Min(c.comboCount * comboBonus, 0.30f);
                damage *= comboMult;
            }

            // Finishing blow
            if (HasAbility(s, "finishing_blow") && floor.hp / floor.maxHp <= 0.2f)
                damage *= 2f;

            // Defense reduction (with penetration)
            float pen = GetAbilityValue(s, "penetration");
            float effectiveDefense = Mathf.Max(0f, bdef.defense - pen);
            if (bdef.id == "steel_structure" && pen <= 0f)
                effectiveDefense = Mathf.Max(effectiveDefense, 0.5f);
            damage *= (1f - effectiveDefense);

            // Frost: defense = 0 during freeze
            if (floor.freezeTimer > 0f)
                damage = GetAttackDamage(s) * (isCrit ? GetCritMultiplier(s) : 1f);

            // Apply damage
            floor.hp -= damage;
            s.totalDamage += damage;

            // Floating text
            AddFloatingText(s, s.character.x, s.character.y - 40f,
                $"-{Mathf.RoundToInt(damage)}", isCrit);

            // Floor destroyed?
            if (floor.hp <= 0f)
            {
                floor.hp = 0f;
                floor.destroyed = true;
                s.building.totalHp -= floor.maxHp;

                // EXP for floor
                float expGain = floor.maxHp * 0.5f * GetExpMultiplier(s);
                s.exp += expGain;

                // Spawn hazards on destroy (glass, gas)
                SpawnDestroyHazards(s, bdef, floorIdx);
            }

            // Spawn debris on hit
            if (Random.value < 0.3f + (1f - floor.hp / floor.maxHp) * 0.4f)
                SpawnDebris(s, bdef);

            // Electric shock chance
            if (bdef.hazardType == BuildingHazardType.ElectricShock)
            {
                if (Random.value < bdef.hazardParam)
                    c.stunTimer = 1f;
            }

            // Skill effects
            ApplySkillEffects(s, floorIdx, damage);

            // Building fully destroyed?
            if (s.building.AllDestroyed())
                CompleteStage(s);

            // Focus stacks
            if (HasAbility(s, "focus"))
                c.focusStacks = Mathf.Min(c.focusStacks + GetAbilityValue(s, "focus"), 0.5f);
        }

        // ─── Skill Effects ──────────────────────────────────────────────────
        static void ApplySkillEffects(GameState s, int floorIdx, float damage)
        {
            if (s.acquiredSkills.Contains("explosion_impact"))
            {
                // Splash 30% to adjacent floors
                float splash = damage * 0.3f;
                if (floorIdx > 0 && !s.building.floors[floorIdx - 1].destroyed)
                    s.building.floors[floorIdx - 1].hp -= splash;
                if (floorIdx + 1 < s.building.floors.Count && !s.building.floors[floorIdx + 1].destroyed)
                    s.building.floors[floorIdx + 1].hp -= splash;
            }

            if (s.acquiredSkills.Contains("electric_chain"))
            {
                float chainDmg = damage * 0.25f;
                for (int i = 1; i <= 3; i++)
                {
                    int idx = floorIdx + i;
                    if (idx < s.building.floors.Count && !s.building.floors[idx].destroyed)
                        s.building.floors[idx].hp -= chainDmg;
                }
            }

            if (s.acquiredSkills.Contains("frost_strike"))
            {
                var floor = s.building.floors[floorIdx];
                floor.freezeTimer = 3f;
            }

            if (s.acquiredSkills.Contains("flame_strike"))
            {
                s.building.fireTimer = 5f;
                s.building.fireDamage = GetAttackDamage(s) * 0.2f;
            }

            if (s.acquiredSkills.Contains("earthquake"))
            {
                if (Random.value < 0.2f)
                {
                    float quakeDmg = damage * 0.1f;
                    for (int i = 0; i < s.building.floors.Count; i++)
                        if (!s.building.floors[i].destroyed)
                            s.building.floors[i].hp -= quakeDmg;
                }
            }
        }

        // ─── Building Update ────────────────────────────────────────────────
        static void UpdateBuilding(GameState s, float dt)
        {
            if (s.building == null) return;

            // Fire DoT
            if (s.building.fireTimer > 0f)
            {
                s.building.fireTimer -= dt;
                int idx = s.building.CurrentFloorIndex();
                if (idx >= 0)
                {
                    s.building.floors[idx].hp -= s.building.fireDamage * dt;
                    if (s.building.floors[idx].hp <= 0f)
                    {
                        s.building.floors[idx].hp = 0f;
                        s.building.floors[idx].destroyed = true;
                        if (s.building.AllDestroyed()) CompleteStage(s);
                    }
                }
            }

            // Freeze timers
            foreach (var floor in s.building.floors)
                if (floor.freezeTimer > 0f) floor.freezeTimer -= dt;

            // Bunker missile launcher
            var bdef = BuildingDefs.ALL[s.building.defId];
            if (bdef.hazardType == BuildingHazardType.MissileLauncher && !s.building.AllDestroyed())
            {
                // Fire missile every 3 seconds
                if (Time.frameCount % 180 == 0)
                    SpawnMissile(s, bdef);
            }
        }

        // ─── Stage Complete ─────────────────────────────────────────────────
        static void CompleteStage(GameState s)
        {
            s.buildingsDestroyed++;
            s.carryOverTime = s.stageTimer;

            // Bonus time from ability
            float bonusTime = GetAbilityValue(s, "bonus_time");
            s.carryOverTime += bonusTime;

            // Regeneration
            float regen = GetAbilityValue(s, "regeneration");
            if (regen > 0f)
                s.character.hp = Mathf.Min(s.character.hp + s.character.maxHp * regen, s.character.maxHp);

            s.stage++;
            s.highestStage = Mathf.Max(s.highestStage, s.stage);
            SpawnBuilding(s);
        }

        // ─── Hazards ────────────────────────────────────────────────────────
        static void SpawnDebris(GameState s, BuildingDef bdef)
        {
            float bx = GameData.WORLD_WIDTH * 0.5f + Random.Range(-100f, 100f);
            float by = GameData.WORLD_HEIGHT * 0.4f;
            s.hazards.Add(new HazardInstance
            {
                uid = s.nextHazardUid++,
                x = bx, y = by,
                vx = Random.Range(-50f, 50f),
                vy = Random.Range(200f, 400f),
                damage = bdef.hazardParam,
                lifetime = 2f,
                type = "debris",
                active = true,
            });
        }

        static void SpawnDestroyHazards(GameState s, BuildingDef bdef, int floorIdx)
        {
            if (bdef.hazardType == BuildingHazardType.GlassShatter)
            {
                for (int i = 0; i < 5; i++)
                {
                    s.hazards.Add(new HazardInstance
                    {
                        uid = s.nextHazardUid++,
                        x = GameData.WORLD_WIDTH * 0.5f + Random.Range(-150f, 150f),
                        y = GameData.WORLD_HEIGHT * 0.3f + floorIdx * 60f,
                        vx = Random.Range(-200f, 200f),
                        vy = Random.Range(100f, 300f),
                        damage = bdef.hazardParam,
                        lifetime = 1.5f,
                        type = "glass",
                        active = true,
                    });
                }
            }
            else if (bdef.hazardType == BuildingHazardType.GasExplosion)
            {
                s.hazards.Add(new HazardInstance
                {
                    uid = s.nextHazardUid++,
                    x = GameData.WORLD_WIDTH * 0.5f,
                    y = GameData.WORLD_HEIGHT * 0.3f + floorIdx * 60f,
                    vx = 0f, vy = 0f,
                    damage = bdef.hazardParam,
                    lifetime = 0.5f,
                    type = "gas_explosion",
                    active = true,
                });
            }
        }

        static void SpawnMissile(GameState s, BuildingDef bdef)
        {
            s.hazards.Add(new HazardInstance
            {
                uid = s.nextHazardUid++,
                x = GameData.WORLD_WIDTH * 0.5f,
                y = GameData.WORLD_HEIGHT * 0.3f,
                vx = (s.character.x - GameData.WORLD_WIDTH * 0.5f) * 2f,
                vy = (s.character.y - GameData.WORLD_HEIGHT * 0.3f) * 2f,
                damage = bdef.hazardParam,
                lifetime = 3f,
                type = "missile",
                active = true,
            });
        }

        static void UpdateHazards(GameState s, float dt)
        {
            var c = s.character;
            float dodgeMult = 1f - GetAbilityValue(s, "dodge_size");
            float debrisResist = GetAbilityValue(s, "debris_resist");
            float hitRadius = 30f * dodgeMult;

            for (int i = s.hazards.Count - 1; i >= 0; i--)
            {
                var h = s.hazards[i];
                if (!h.active) { s.hazards.RemoveAt(i); continue; }

                h.x += h.vx * dt;
                h.y += h.vy * dt;
                h.lifetime -= dt;

                if (h.lifetime <= 0f) { h.active = false; continue; }

                // Collision with character
                if (c.invincibilityTimer <= 0f && Dist(h.x, h.y, c.x, c.y) < hitRadius)
                {
                    float dmg = h.damage;
                    if (h.type == "debris") dmg *= (1f - debrisResist);

                    float defReduction = GetAbilityValue(s, "defense");
                    dmg *= (1f - defReduction);

                    c.hp -= dmg;
                    c.invincibilityTimer = GameData.INVINCIBILITY_DURATION + GetAbilityValue(s, "invincibility");
                    h.active = false;

                    AddFloatingText(s, c.x, c.y - 20f, $"-{Mathf.RoundToInt(dmg)}", false);

                    if (c.hp <= 0f)
                    {
                        c.hp = 0f;
                        s.gameOver = true;
                        s.gameOverReason = "hp";
                    }
                }
            }
        }

        // ─── Environment ────────────────────────────────────────────────────
        static void UpdateEnvironment(GameState s, float dt)
        {
            if (s.stage < 10) return;

            if (s.activeEnvHazard != null)
            {
                s.envHazardDuration -= dt;
                if (s.envHazardDuration <= 0f)
                {
                    s.activeEnvHazard = null;
                    return;
                }

                // Apply env effects
                switch (s.activeEnvHazard)
                {
                    case "earthquake":
                        // Random debris
                        if (Random.value < 0.1f * dt * 60f)
                            SpawnDebris(s, BuildingDefs.ALL[s.building.defId]);
                        break;
                    case "storm":
                        // Push character
                        s.character.x += Random.Range(-100f, 100f) * dt;
                        s.character.x = Mathf.Clamp(s.character.x, 50f, GameData.WORLD_WIDTH - 50f);
                        break;
                    case "acid_rain":
                        if (s.stage >= 20)
                        {
                            s.character.hp -= 2f * dt;
                            if (s.character.hp <= 0f)
                            {
                                s.character.hp = 0f;
                                s.gameOver = true;
                                s.gameOverReason = "hp";
                            }
                        }
                        break;
                }
                return;
            }

            // Chance to trigger new env hazard
            s.envHazardTimer -= dt;
            if (s.envHazardTimer <= 0f)
            {
                s.envHazardTimer = Random.Range(15f, 25f);
                var options = new List<string> { "earthquake" };
                if (s.stage >= 15) options.Add("storm");
                if (s.stage >= 20) options.Add("acid_rain");
                s.activeEnvHazard = options[Random.Range(0, options.Count)];
                s.envHazardDuration = Random.Range(3f, 6f);
            }
        }

        // ─── EXP / Level Up ─────────────────────────────────────────────────
        static void UpdateExp(GameState s)
        {
            if (s.exp >= s.expRequired)
            {
                s.exp -= s.expRequired;
                s.level++;
                s.expRequired = GameData.ExpForLevel(s.level);
                s.levelUpPending = true;
                GenerateLevelUpChoices(s);
            }
        }

        public static void GenerateLevelUpChoices(GameState s)
        {
            var pool = new List<string>();

            // Add abilities that haven't maxed
            foreach (var kv in AbilityDefs.ALL)
            {
                int stacks = s.abilityStacks.GetValueOrDefault(kv.Key, 0);
                if (stacks < kv.Value.maxStack) pool.Add(kv.Key);
            }

            // Add unacquired skills (30% chance per slot)
            var skillPool = new List<string>();
            foreach (var kv in SkillDefs.ALL)
                if (!s.acquiredSkills.Contains(kv.Key)) skillPool.Add(kv.Key);

            // Pick 3
            s.levelUpChoices.Clear();
            for (int i = 0; i < 3; i++)
            {
                if (pool.Count == 0 && skillPool.Count == 0) break;

                bool pickSkill = skillPool.Count > 0 && (pool.Count == 0 || Random.value < 0.3f);
                if (pickSkill)
                {
                    int idx = Random.Range(0, skillPool.Count);
                    s.levelUpChoices.Add(skillPool[idx]);
                    skillPool.RemoveAt(idx);
                }
                else if (pool.Count > 0)
                {
                    int idx = Random.Range(0, pool.Count);
                    s.levelUpChoices.Add(pool[idx]);
                    pool.RemoveAt(idx);
                }
            }
        }

        public static void SelectLevelUp(GameState s, int choiceIndex)
        {
            if (choiceIndex < 0 || choiceIndex >= s.levelUpChoices.Count) return;
            string id = s.levelUpChoices[choiceIndex];

            if (AbilityDefs.ALL.ContainsKey(id))
            {
                int current = s.abilityStacks.GetValueOrDefault(id, 0);
                s.abilityStacks[id] = current + 1;

                // Apply max HP increase immediately
                if (AbilityDefs.ALL[id].statType == StatType.MaxHp)
                {
                    float oldMax = s.character.maxHp;
                    s.character.maxHp = GameData.BASE_HP * (1f + s.abilityStacks[id] * AbilityDefs.ALL[id].valuePerStack);
                    s.character.hp += s.character.maxHp - oldMax;
                }
            }
            else if (SkillDefs.ALL.ContainsKey(id))
            {
                s.acquiredSkills.Add(id);
            }

            s.levelUpPending = false;
            s.levelUpChoices.Clear();
        }

        // ─── Input ──────────────────────────────────────────────────────────
        public static void ProcessTouch(GameState s, float worldX, float worldY)
        {
            if (s.gameOver || s.paused || s.levelUpPending) return;
            s.character.targetX = Mathf.Clamp(worldX, 50f, GameData.WORLD_WIDTH - 50f);
            s.character.targetY = Mathf.Clamp(worldY, GameData.WORLD_HEIGHT * 0.5f, GameData.WORLD_HEIGHT - 50f);
            s.character.moving = true;
        }

        // ─── Stat Getters ───────────────────────────────────────────────────
        static float GetAbilityValue(GameState s, string id)
        {
            if (!AbilityDefs.ALL.ContainsKey(id)) return 0f;
            int stacks = s.abilityStacks.GetValueOrDefault(id, 0);
            return stacks * AbilityDefs.ALL[id].valuePerStack;
        }

        static bool HasAbility(GameState s, string id) => s.abilityStacks.GetValueOrDefault(id, 0) > 0;

        static float GetAttackDamage(GameState s)
        {
            float base_ = GameData.BASE_ATTACK_DAMAGE * (1f + GetAbilityValue(s, "attack_damage"));
            // Weapon skill modifier
            foreach (var skillId in s.acquiredSkills)
                if (SkillDefs.ALL.TryGetValue(skillId, out var skill) && skill.category == SkillCategory.WeaponChange)
                    base_ *= skill.damageMult;
            return base_;
        }

        static float GetAttackInterval(GameState s)
        {
            float base_ = GameData.BASE_ATTACK_INTERVAL * (1f - GetAbilityValue(s, "attack_speed"));
            // Focus bonus
            if (HasAbility(s, "focus")) base_ *= (1f - s.character.focusStacks);
            // Weapon skill modifier
            foreach (var skillId in s.acquiredSkills)
                if (SkillDefs.ALL.TryGetValue(skillId, out var skill) && skill.category == SkillCategory.WeaponChange)
                    base_ *= skill.attackSpeedMult;
            // Dual fist: 2 hits per attack (handled in PerformAttack)
            return Mathf.Max(0.05f, base_);
        }

        static float GetAttackRange(GameState s)
        {
            float base_ = GameData.BASE_ATTACK_RANGE * (1f + GetAbilityValue(s, "attack_range"));
            foreach (var skillId in s.acquiredSkills)
                if (SkillDefs.ALL.TryGetValue(skillId, out var skill) && skill.category == SkillCategory.WeaponChange)
                    base_ *= skill.rangeMult;
            return base_;
        }

        static float GetMoveSpeed(GameState s) =>
            GameData.BASE_MOVE_SPEED * (1f + GetAbilityValue(s, "move_speed"));

        static float GetCritChance(GameState s) =>
            GameData.BASE_CRIT_CHANCE + GetAbilityValue(s, "crit_chance");

        static float GetCritMultiplier(GameState s) =>
            GameData.BASE_CRIT_MULTIPLIER + GetAbilityValue(s, "crit_damage");

        static float GetExpMultiplier(GameState s) =>
            1f + GetAbilityValue(s, "exp_boost");

        // ─── Score ──────────────────────────────────────────────────────────
        public static int CalculateScore(GameState s)
        {
            float hpRatio = s.character.hp / s.character.maxHp;
            float scoreMult = 1f + GetAbilityValue(s, "score_boost");

            int score = Mathf.RoundToInt(
                (s.buildingsDestroyed * GameData.SCORE_PER_BUILDING
                + (s.stage - 1) * GameData.SCORE_PER_STAGE
                + s.totalDamage
                + hpRatio * GameData.SCORE_HP_BONUS_MAX)
                * scoreMult
            );
            return score;
        }

        // ─── Floating Text ──────────────────────────────────────────────────
        static void AddFloatingText(GameState s, float x, float y, string text, bool isCrit)
        {
            s.floatingTexts.Add(new FloatingText
            {
                uid = s.nextFloatUid++,
                x = x, y = y,
                text = text, ttl = 1f, isCrit = isCrit,
            });
        }

        static void UpdateFloatingTexts(GameState s, float dt)
        {
            for (int i = s.floatingTexts.Count - 1; i >= 0; i--)
            {
                s.floatingTexts[i].ttl -= dt;
                s.floatingTexts[i].y -= 50f * dt;
                if (s.floatingTexts[i].ttl <= 0f)
                    s.floatingTexts.RemoveAt(i);
            }
        }
    }
}
