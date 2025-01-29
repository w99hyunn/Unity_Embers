using UnityEngine;
using System.Collections.Generic;
namespace NOLDA
{
    [CreateAssetMenu(fileName = "NewSkill", menuName = "Skill System/SkillData", order = 1)]
    public class SkillData : ScriptableObject
    {
        public string skillName;
        public string description;
        public Sprite icon;
        public Class classType;
        public SkillType skillType;
        public string animationTriggerName; // 애니메이션 트리거 이름 추가
        public GameObject skillEffectPrefab; // 이펙트 프리팹
        public float cooldownTime; // 쿨타임
        public KeyCode defaultKey;

        // 스킬 레벨별 정보
        public List<SkillLevelData> levelData;

        // 현재 레벨의 정보를 가져오는 메서드
        public SkillLevelData GetSkillLevelData(int level)
        {
            return levelData.Find(l => l.level == level);
        }
    }

    [System.Serializable]
    public class SkillLevelData
    {
        public int level;        // 해당 레벨
        public int spCost;       // 해당 레벨에서의 SP 소모량
        public int requiredLevel; // 스킬을 배우기 위한 요구 레벨
        public float effectMultiplier; // 효과 배율 (예: 데미지 증가율)
    }

}