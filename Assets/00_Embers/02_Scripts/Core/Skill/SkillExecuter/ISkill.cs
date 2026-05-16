using UnityEngine;

namespace Embers
{
    public interface ISkill
    {
        void ExecuteSkill(Animator animator, ISkillEndCallback skillEndCallback);
    }
}