using System;
using System.Collections.Generic;
using UnityEngine;

namespace NOLDA
{
    [Serializable]
    public struct CachedSkillInfo
    {
        public SkillData skillData;
        public int skillLevel;
        public KeyCode keyCode;
    }

    public class SkillSingleton : MonoBehaviour
    {
        [SerializeField] private List<CachedSkillInfo> cachedSkills = new List<CachedSkillInfo>();
        private Dictionary<string, float> cooldownTimers = new Dictionary<string, float>(); // 스킬 쿨타임

        private void Awake()
        {
            LoadSkillsFromResources();
        }

        private void LoadSkillsFromResources()
        {
            SkillData[] skills = Resources.LoadAll<SkillData>("SkillData");
            cachedSkills.Clear();

            foreach (var skill in skills)
            {
                cachedSkills.Add(new CachedSkillInfo
                {
                    skillData = skill,
                    skillLevel = 0,
                    keyCode = skill.defaultKey
                });
            }
        }

        public SkillData GetSkillData(int skillID)
        {
            var found = cachedSkills.Find(s => s.skillData != null && s.skillData.skillID == skillID);
            return found.skillData != null ? found.skillData : null;
        }

        public List<CachedSkillInfo> GetAllSkills()
        {
            return cachedSkills;
        }

        public List<CachedSkillInfo> GetPlayerSkills()
        {
            List<CachedSkillInfo> playerSkills = new List<CachedSkillInfo>();

            foreach (var cachedSkill in cachedSkills)
            {
                if (Singleton.Game.playerData.Skills.ContainsKey(cachedSkill.skillData.skillID))
                {
                    int skillLevel = Singleton.Game.playerData.Skills[cachedSkill.skillData.skillID];
                    playerSkills.Add(new CachedSkillInfo
                    {
                        skillData = cachedSkill.skillData,
                        skillLevel = skillLevel,
                        keyCode = cachedSkill.keyCode
                    });
                }
            }

            return playerSkills;
        }

        public int GetSkillLevel(int skillID)
        {
            return Singleton.Game.playerData.Skills.ContainsKey(skillID) ? Singleton.Game.playerData.Skills[skillID] : 0;
        }

        public bool IsSkillOnCooldown(int skillID)
        {
            SkillData skillData = GetSkillData(skillID);
            return skillData != null && cooldownTimers.ContainsKey(skillData.skillName) && cooldownTimers[skillData.skillName] > Time.time;
        }

        public void SetSkillCooldown(int skillID)
        {
            SkillData skillData = GetSkillData(skillID);
            if (skillData == null) return;
            cooldownTimers[skillData.skillName] = Time.time + skillData.cooldownTime;
        }
    }
}
