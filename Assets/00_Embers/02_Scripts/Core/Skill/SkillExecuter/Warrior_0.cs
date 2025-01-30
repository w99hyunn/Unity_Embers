using UnityEngine;

namespace NOLDA
{
    [CreateAssetMenu(fileName = SKILL_CODE, menuName = MENU_NAME)]
    public class Warrior_0 : SkillExecuter
    {
        public const string SKILL_CODE = "Warrior_0"; //스킬 제작시 이 부분 수정 필요
        public const string MENU_NAME = "Skill System/SkillExecuter/" + SKILL_CODE;
        public override void ExecuteSkill(Animator animator)
        {
            animator.SetTrigger("Warrior_0");
        }
    }
}