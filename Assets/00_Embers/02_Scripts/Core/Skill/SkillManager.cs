using System.Collections.Generic;
using UnityEngine;

namespace NOLDA
{
    public class SkillManager : MonoBehaviour
    {
        public UIControlManager uiControlManager;
        public List<SkillData> availableSkills = new List<SkillData>(); // 모든 스킬 데이터
        private Dictionary<string, int> learnedSkills = new Dictionary<string, int>(); // 배운 스킬 (스킬명, 현재 레벨)
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

        private void Update()
        {
            CheckSkillInput();
        }

        private void CheckSkillInput()
        {
            foreach (var skill in learnedSkills)
            {
                SkillData skillData = availableSkills.Find(s => s.skillName == skill.Key);
                if (skillData == null) continue;

                if (Input.GetKeyDown(skillData.defaultKey))
                {
                    ExecuteSkill(skillData, skill.Value);
                }
            }
        }

        public void LearnSkill(SkillData skill, int playerLevel, int playerSP, ref int currentSP)
        {
            if (playerLevel < skill.levelData[0].requiredLevel)
            {
                uiControlManager.chatUIController.AddChatMessageHandle("[시스템]", "레벨이 부족합니다.");
                return;
            }

            if (currentSP < skill.levelData[0].spCost)
            {
                uiControlManager.chatUIController.AddChatMessageHandle("[시스템]", "SP(Skill Point)가 부족합니다.");
                return;
            }

            if (learnedSkills.ContainsKey(skill.skillName))
            {
                uiControlManager.chatUIController.AddChatMessageHandle("[시스템]", "이미 배운 스킬입니다.");
                return;
            }

            learnedSkills.Add(skill.skillName, 1);
            currentSP -= skill.levelData[0].spCost;

            uiControlManager.chatUIController.AddChatMessageHandle("[시스템]", $"{skill.skillName} 스킬을 배웠습니다! (레벨 1)");
        }

        public void UpgradeSkill(SkillData skill, int playerLevel, int playerSP, ref int currentSP)
        {
            if (!learnedSkills.ContainsKey(skill.skillName))
            {
                uiControlManager.chatUIController.AddChatMessageHandle("[시스템]", "먼저 해당 스킬을 배워야 합니다.");
                return;
            }

            int currentLevel = learnedSkills[skill.skillName];
            if (currentLevel >= skill.levelData.Count)
            {
                uiControlManager.chatUIController.AddChatMessageHandle("[시스템]", "이미 최대 레벨입니다.");
                return;
            }

            SkillLevelData nextLevelData = skill.levelData[currentLevel];
            if (playerLevel < nextLevelData.requiredLevel)
            {
                uiControlManager.chatUIController.AddChatMessageHandle("[시스템]", "레벨이 부족합니다.");
                return;
            }

            if (currentSP < nextLevelData.spCost)
            {
                uiControlManager.chatUIController.AddChatMessageHandle("[시스템]", "SP가 부족합니다.");
                return;
            }

            learnedSkills[skill.skillName]++;
            currentSP -= nextLevelData.spCost;

            uiControlManager.chatUIController.AddChatMessageHandle("[시스템]", $"{skill.skillName} 스킬 레벨이 {currentLevel + 1}로 올랐습니다!");
        }

        public void ExecuteSkill(SkillData skill, int skillLevel)
        {
            if (cooldownTimers.ContainsKey(skill.skillName) && cooldownTimers[skill.skillName] > Time.time)
            {
                uiControlManager.chatUIController.AddChatMessageHandle("[시스템]", $"{skill.skillName} 스킬이 아직 쿨타임 중입니다.");
                return;
            }

            SkillLevelData levelData = skill.GetSkillLevelData(skillLevel);
            if (levelData == null) return;

            //스킬사용 OK

            if (skill.skillEffectPrefab != null)
            {
                Instantiate(skill.skillEffectPrefab, transform.position + transform.forward * 2, Quaternion.identity);
            }

            cooldownTimers[skill.skillName] = Time.time + skill.cooldownTime;
        }

        public int GetSkillLevel(string skillName)
        {
            return learnedSkills.ContainsKey(skillName) ? learnedSkills[skillName] : 0;
        }
    }
}