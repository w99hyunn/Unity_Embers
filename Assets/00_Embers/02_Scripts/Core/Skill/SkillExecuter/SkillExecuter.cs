using UnityEngine;

namespace NOLDA
{
    public abstract class SkillExecuter : ScriptableObject, ISkill
    {
        public abstract void ExecuteSkill(Animator animator, ISkillEndCallback skillEndCallback);
    }
}