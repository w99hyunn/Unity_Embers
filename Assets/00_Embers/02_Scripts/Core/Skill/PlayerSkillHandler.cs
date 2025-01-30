using Mirror;
using UnityEngine;

namespace NOLDA
{
    public class PlayerSkillHandler : NetworkBehaviour
    {
        private SkillDirector skillManager;
        private Animator animator;

        private void Start()
        {
            skillManager = FindAnyObjectByType<SkillDirector>();
            animator = GetComponent<Animator>();
        }

        // TODO: 스킬 애니메이션 자연스럽게 처리 및 playerInput쪽과 연동해서 커서 상태에 따른 스킬 사용 가능 여부 연동처리 필요

        
        private void Update()
        {
            if (!isLocalPlayer || Director.Game.playerData == null) return;

            foreach (var skillEntry in Director.Game.playerData.Skills) // PlayerDataSO에서 직접 스킬 데이터 가져오기
            {
                SkillData skillData = Director.Skill.GetSkillData(skillEntry.Key);
                if (skillData == null) continue;

                if (Input.GetKeyDown(skillData.defaultKey))
                {
                    ExecuteSkill(skillData, skillEntry.Value);
                }
            }
        }

        /// <summary>
        /// 스킬 실행시 호출되는 함수(플레이어단)
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="skillLevel"></param>
        public void ExecuteSkill(SkillData skill, int skillLevel)
        {
            if (skillManager.IsSkillOnCooldown(skill.skillID))
            {
                return;
            }

            // 애니메이션 실행
            if (skill.skillExecuter is ISkill skillScript)
            {
                skillScript.ExecuteSkill(animator);
            }

            // 주변 적을 찾아 공격
            //TryUseSkillOnEnemy(skill);

            // 쿨타임 설정
            skillManager.SetSkillCooldown(skill.skillID);
        }

        // private void TryUseSkillOnEnemy(SkillData skill)
        // {
        //     Collider[] hitColliders = Physics.OverlapSphere(transform.position, skill.hitRadius);
        //     foreach (Collider hitCollider in hitColliders)
        //     {
        //         if (hitCollider.TryGetComponent(out NetworkIdentity targetIdentity) &&
        //             hitCollider.TryGetComponent(out Enemy enemy))
        //         {
        //             CmdUseSkillOnTarget(skill.skillID, targetIdentity);
        //             break;
        //         }
        //     }
        // }

        // [Command]
        // private void CmdUseSkillOnTarget(int skillID, NetworkIdentity target)
        // {
        //     SkillData skill = skillManager.GetSkillData(skillID);
        //     if (skill == null || target == null) return;

        //     if (!Director.Game.playerData.Skills.ContainsKey(skillID)) return; // PlayerDataSO에서 직접 확인
        //     int skillLevel = Director.Game.playerData.Skills[skillID];

        //     SkillLevelData levelData = skill.GetSkillLevelData(skillLevel);
        //     if (levelData == null) return;

        //     if (target.TryGetComponent(out Enemy enemy))
        //     {
        //         enemy.TakeDamage(skill.baseDamage);
        //     }

        //     RpcPlaySkillEffects(skill.skillEffectPrefab, target.transform.position);
        // }

        // [ClientRpc]
        // private void RpcPlaySkillEffects(GameObject effectPrefab, Vector3 targetPosition)
        // {
        //     if (effectPrefab != null)
        //     {
        //         Instantiate(effectPrefab, targetPosition, Quaternion.identity);
        //     }
        // }
    }
}
