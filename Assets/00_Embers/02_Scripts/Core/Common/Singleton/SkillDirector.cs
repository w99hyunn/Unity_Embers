using System.Collections.Generic;
using UnityEngine;

namespace NOLDA
{
    public class SkillDirector : MonoBehaviour
    {
        public List<SkillData> availableSkills = new List<SkillData>(); // 모든 스킬 데이터
        private Dictionary<string, float> cooldownTimers = new Dictionary<string, float>(); // 스킬 쿨타임

        private void Awake()
        {
            LoadSkillsFromResources();
        }

        private void LoadSkillsFromResources()
        {
            SkillData[] skills = Resources.LoadAll<SkillData>("SkillData");
            availableSkills.AddRange(skills);
        }

        public SkillData GetSkillData(int skillID)
        {
            return availableSkills.Find(s => s.skillID == skillID);
        }

        public int GetSkillLevel(int skillID)
        {
            return Director.Game.playerData.Skills.ContainsKey(skillID) ? Director.Game.playerData.Skills[skillID] : 0;
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
