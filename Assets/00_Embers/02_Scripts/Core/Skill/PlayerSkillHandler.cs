using Mirror;
using UnityEngine;
using System.Collections.Generic;

namespace NOLDA
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Animator))]
    public class PlayerSkillHandler : NetworkBehaviour, ISkillEndCallback
    {
        private PlayerInput input;
        private SkillSingleton skillManager;
        private Animator animator;
        private bool _isSkillInUse = false;
        public bool IsSkillInUse => _isSkillInUse;

        // 캐시된 스킬 데이터를 저장할 구조체
        private struct CachedSkillInfo
        {
            public SkillData skillData;
            public int skillLevel;
            public KeyCode keyCode;
        }
        
        // 스킬 정보 캐시
        private List<CachedSkillInfo> cachedSkills = new List<CachedSkillInfo>();
        
        private void Start()
        {
            skillManager = FindAnyObjectByType<SkillSingleton>();
            TryGetComponent<PlayerInput>(out input);
            TryGetComponent<Animator>(out animator);

            // 플레이어가 스킬정보 캐시를 갖고있음(메모리 효율성)
            UpdateSkillCache();
            Singleton.Game.playerData.OnDataChanged += OnPlayerDataChanged;
        }

        private void OnDestroy()
        {
            Singleton.Game.playerData.OnDataChanged -= OnPlayerDataChanged;
        }

        private void OnPlayerDataChanged(string propertyName, object value)
        {
            if (propertyName == nameof(PlayerDataSO.Skills))
            {
                UpdateSkillCache(); 
            }  //TODO: 스킬 업데이트 이벤트 발생이 필요한가?? 확인필요 너무오랜만이라 기억안나//
        }

        private void UpdateSkillCache()
        {
            cachedSkills.Clear();
            foreach (var skillEntry in Singleton.Game.playerData.Skills)
            {
                SkillData skillData = Singleton.Skill.GetSkillData(skillEntry.Key);
                if (skillData != null)
                {
                    cachedSkills.Add(new CachedSkillInfo
                    {
                        skillData = skillData,
                        skillLevel = skillEntry.Value,
                        keyCode = skillData.defaultKey
                    });
                }
            }
        }

        private void Update()
        {
            if (!isLocalPlayer 
                || Singleton.Game.playerData == null 
                || input.IsPointerOverUI())
                return;

            foreach (var skillInfo in cachedSkills)
            {
                if (Input.GetKeyDown(skillInfo.keyCode))
                {
                    ExecuteSkill(skillInfo.skillData, skillInfo.skillLevel);
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
            if (skillManager.IsSkillOnCooldown(skill.skillID) || _isSkillInUse)
            {
                return;
            }

            // 애니메이션 실행
            _isSkillInUse = true;
            //animator.applyRootMotion = true;
            if (skill.skillExecuter is ISkill skillScript)
            {
                skillScript.ExecuteSkill(animator, this);
            }

            // 주변 적을 찾아 공격
            TryUseSkillOnEnemy(skill);

            // 쿨타임 설정
            skillManager.SetSkillCooldown(skill.skillID);
        }

        public void OnSkillEnd()
        {
            _isSkillInUse = false;
            //animator.applyRootMotion = false;
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

            if (!Singleton.Game.playerData.Skills.ContainsKey(skillID)) return;
            int skillLevel = Singleton.Game.playerData.Skills[skillID];

            SkillLevelData levelData = skill.GetSkillLevelData(skillLevel);
            if (levelData == null) return;

            if (target.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(skill.baseDamage * levelData.effectMultiplier);
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
