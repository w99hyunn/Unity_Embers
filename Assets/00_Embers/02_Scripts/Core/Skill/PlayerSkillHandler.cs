using Mirror;
using UnityEngine;

namespace NOLDA
{
    public class PlayerSkillHandler : NetworkBehaviour
    {
        private SkillManager skillManager;
        private Animator animator;

        private void Start()
        {
            skillManager = FindAnyObjectByType<SkillManager>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (!isLocalPlayer || Singleton.Game.playerData == null) return;

            foreach (var skillEntry in Singleton.Game.playerData.Skills) // PlayerDataSO에서 직접 스킬 데이터 가져오기
            {
                SkillData skillData = skillManager.GetSkillData(skillEntry.Key);
                if (skillData == null) continue;

                if (Input.GetKeyDown(skillData.defaultKey))
                {
                    ExecuteSkill(skillData, skillEntry.Value);
                }
            }
        }

        public void ExecuteSkill(SkillData skill, int skillLevel)
        {
            if (skillManager.IsSkillOnCooldown(skill.skillID))
            {
                return;
            }

            // 애니메이션 실행
            if (animator != null)
            {
                animator.SetTrigger(skill.animationTriggerName);
            }

            // 주변 적을 찾아 공격
            TryUseSkillOnEnemy(skill);

            // 쿨타임 설정
            skillManager.SetSkillCooldown(skill.skillID);
        }

        private void TryUseSkillOnEnemy(SkillData skill)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, skill.hitRadius);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent(out NetworkIdentity targetIdentity) &&
                    hitCollider.TryGetComponent(out Enemy enemy))
                {
                    CmdUseSkillOnTarget(skill.skillID, targetIdentity);
                    break;
                }
            }
        }

        [Command]
        private void CmdUseSkillOnTarget(int skillID, NetworkIdentity target)
        {
            SkillData skill = skillManager.GetSkillData(skillID);
            if (skill == null || target == null) return;

            if (!Singleton.Game.playerData.Skills.ContainsKey(skillID)) return; // PlayerDataSO에서 직접 확인
            int skillLevel = Singleton.Game.playerData.Skills[skillID];

            SkillLevelData levelData = skill.GetSkillLevelData(skillLevel);
            if (levelData == null) return;

            if (target.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(skill.baseDamage);
            }

            RpcPlaySkillEffects(skill.skillEffectPrefab, target.transform.position);
        }

        [ClientRpc]
        private void RpcPlaySkillEffects(GameObject effectPrefab, Vector3 targetPosition)
        {
            if (effectPrefab != null)
            {
                Instantiate(effectPrefab, targetPosition, Quaternion.identity);
            }
        }
    }
}
