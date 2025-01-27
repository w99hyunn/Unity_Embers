using UnityEngine;

namespace NOLDA
{
    [CreateAssetMenu(fileName = "ExperienceTable", menuName = "NOLDA/Experience Table", order = 1)]
    public class HxpTableSO : ScriptableObject
    {
        public int[] experienceRequiredPerLevel;

        public int GetExperienceForNextLevel(int level)
        {
            if (level <= 0 || level >= experienceRequiredPerLevel.Length)
                return 0;

            return experienceRequiredPerLevel[level];
        }

        public int MaxLevel => experienceRequiredPerLevel.Length - 1;
    }
}