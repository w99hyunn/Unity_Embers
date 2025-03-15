using UnityEngine;

namespace NOLDA
{
    [CreateAssetMenu(fileName = SKILL_CODE, menuName = MENU_NAME)]
    public class Warrior_0 : SkillExecuter
    {
        #region Skill Code Info
        public const string SKILL_CODE = "Warrior_0"; //스킬 제작시 이 부분 수정 필요
        public const string MENU_NAME = "NOLDA/Skill System/SkillExecuter/" + SKILL_CODE;
        #endregion

        public override void ExecuteSkill(Animator animator, ISkillEndCallback skillEndCallback)
        {
            WaitForAnimationEnd(animator, skillEndCallback).Forget();
        }
        private async Awaitable WaitForAnimationEnd(Animator animator, ISkillEndCallback skillEndCallback)
        {
            animator.SetTrigger("Warrior_0");
            await Awaitable.WaitForSecondsAsync(1.5f);
            skillEndCallback.OnSkillEnd();
        }
    }
}