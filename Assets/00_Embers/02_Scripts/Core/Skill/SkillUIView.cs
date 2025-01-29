using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NOLDA
{
    public class SkillUIView : MonoBehaviour
    {
        public UIControlManager uiControlManager;

        public Transform skillPanel;
        public GameObject skillButtonPrefab;
        public TMP_Text spPoint;

        private List<GameObject> skillButtons = new List<GameObject>();

        private void Start()
        {
            UpdateSkillUI();
            SetSPPoint();
        }

        private void SetSPPoint()
        {
            spPoint.text = $"{Singleton.Game.playerData.Sp}";
        }

        public void UpdateSkillUI()
        {
            foreach (var btn in skillButtons)
            {
                Destroy(btn);
            }
            skillButtons.Clear();

            foreach (var skill in Singleton.Skill.availableSkills)
            {
                if (skill.classType != Singleton.Game.playerData.Class)
                    continue;

                int skillLevel = Singleton.Game.playerData.Skills.ContainsKey(skill.skillID) ? Singleton.Game.playerData.Skills[skill.skillID] : 0;
                bool canLearn = Singleton.Game.playerData.Level >= skill.levelData[0].requiredLevel 
                                && Singleton.Game.playerData.Sp >= skill.levelData[0].spCost;
                bool canUpgrade = skillLevel > 0 && skillLevel < skill.levelData.Count;

                GameObject skillButton = Instantiate(skillButtonPrefab, skillPanel);
                var skillPrefab = skillButton.GetComponent<SkillPrefab>();
                skillPrefab.skillIcon.sprite = skill.icon;
                skillPrefab.skillName.text = $"{skill.skillName}";
                skillPrefab.skillLevel.text = $"현재 {skillLevel}";

                if (skillLevel == 0 && canLearn)
                {
                    skillPrefab.skillLevelupBtn.onClick.AddListener(() => LearnSkill(skill.skillID));
                }
                else if (canUpgrade)
                {
                    skillPrefab.skillLevelupBtn.onClick.AddListener(() => UpgradeSkill(skill.skillID));
                }
                else
                {
                    skillPrefab.skillLevelupBtn.isInteractable = false;
                }

                skillButtons.Add(skillButton);
            }

            SetSPPoint();
        }

        private void LearnSkill(int skillID)
        {
            if (true == Singleton.Game.playerData.LearnSkill(skillID))
            {
                UpdateSkillUI();
            }
            else
            {
                uiControlManager.InGameChatNotice("시스템", "SP가 부족하거나 레벨이 부족합니다.");
            }
        }

        private void UpgradeSkill(int skillID)
        {
            if (true == Singleton.Game.playerData.LevelUpSkill(skillID))
            {
                UpdateSkillUI();
            }
            else
            {
                uiControlManager.InGameChatNotice("시스템", "SP가 부족하거나 레벨이 부족합니다.");
            }
        }
    }
}