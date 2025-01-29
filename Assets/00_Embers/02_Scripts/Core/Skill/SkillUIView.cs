using System.Collections.Generic;
using UnityEngine;

namespace NOLDA
{
    public class SkillUIView : MonoBehaviour
    {
        public SkillManager skillManager;

        public Transform skillPanel;
        public GameObject skillButtonPrefab;
        private List<GameObject> skillButtons = new List<GameObject>();

        private void Start()
        {
            UpdateSkillUI();
        }

        public void UpdateSkillUI()
        {
            foreach (var btn in skillButtons)
            {
                Destroy(btn);
            }
            skillButtons.Clear();

            foreach (var skill in skillManager.availableSkills)
            {
                int skillLevel = skillManager.GetSkillLevel(skill.skillName);
                bool canLearn = Singleton.Game.playerData.Level >= skill.levelData[0].requiredLevel && Singleton.Game.playerData.Sp >= skill.levelData[0].spCost;
                bool canUpgrade = skillLevel > 0 && skillLevel < skill.levelData.Count;

                GameObject skillButton = Instantiate(skillButtonPrefab, skillPanel);
                var skillPrefab = skillButton.GetComponent<SkillPrefab>();
                skillPrefab.skillIcon.sprite = skill.icon;
                skillPrefab.skillName.text = $"{skill.skillName}";
                skillPrefab.skillLevel.text = $"현재 {skillLevel}";

                if (skillLevel == 0 && canLearn)
                {
                    skillPrefab.skillLevelupBtn.onClick.AddListener(() => LearnSkill(skill));
                }
                else if (canUpgrade)
                {
                    skillPrefab.skillLevelupBtn.onClick.AddListener(() => UpgradeSkill(skill));
                }
                else
                {
                    skillPrefab.skillLevelupBtn.isInteractable = false;
                }

                skillButtons.Add(skillButton);
            }
        }

        private void LearnSkill(SkillData skill)
        {
            int sp = Singleton.Game.playerData.Sp;
            skillManager.LearnSkill(skill, Singleton.Game.playerData.Level, Singleton.Game.playerData.Sp, ref sp);
            Singleton.Game.playerData.Sp = sp;
            UpdateSkillUI();
        }

        private void UpgradeSkill(SkillData skill)
        {
            int sp = Singleton.Game.playerData.Sp;
            skillManager.UpgradeSkill(skill, Singleton.Game.playerData.Level, Singleton.Game.playerData.Sp, ref sp);
            Singleton.Game.playerData.Sp = sp;
            UpdateSkillUI();
        }
    }
}