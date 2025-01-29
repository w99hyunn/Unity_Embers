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
            spPoint.text = $"{Director.Game.playerData.Sp}";
        }

        public void UpdateSkillUI()
        {
            foreach (var btn in skillButtons)
            {
                Destroy(btn);
            }
            skillButtons.Clear();

            foreach (var skill in Director.Skill.availableSkills)
            {
                if (skill.classType != Director.Game.playerData.Class)
                    continue;

                int skillLevel = Director.Game.playerData.Skills.ContainsKey(skill.skillID) ? Director.Game.playerData.Skills[skill.skillID] : 0;
                bool canLearn = Director.Game.playerData.Level >= skill.levelData[0].requiredLevel 
                                && Director.Game.playerData.Sp >= skill.levelData[0].spCost;
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
            if (true == Director.Game.playerData.LearnSkill(skillID))
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
            if (true == Director.Game.playerData.LevelUpSkill(skillID))
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