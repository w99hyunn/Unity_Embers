using Mirror;
using UnityEngine;
using System.Collections.Generic;

namespace NOLDA
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Animator))]
    public class PlayerSkillHandler : NetworkBehaviour, ISkillEndCallback
    {
        public bool IsSkillInUse
        {
            get => _isSkillInUse;
            set
            {
                if (value != _isSkillInUse)
                {
                    _isSkillInUse = value;
                    playerController.isUseSkill = value;
                }
            }
        }

        // 캐시된 스킬 데이터를 저장할 구조체
        private struct CachedSkillInfo
        {
            public SkillData skillData;
            public int skillLevel;
            public KeyCode keyCode;
            public GameObject effectInstance; // 미리 인스턴스화된 이펙트
        }

        private PlayerController playerController;
        private PlayerInput input;
        private Animator animator;
        private bool _isSkillInUse = false;

        // 스킬 정보 캐시
        private List<CachedSkillInfo> cachedSkills = new List<CachedSkillInfo>();

        private void Awake()
        {
            TryGetComponent<PlayerController>(out playerController);
            TryGetComponent<PlayerInput>(out input);
            TryGetComponent<Animator>(out animator);
        }

        private void Start()
        {
            UpdateSkillCache(); // 플레이어가 스킬정보 캐시를 갖고있음(메모리 효율성)
            Singleton.Game.playerData.OnDataChanged += OnPlayerDataChanged;
        }

        private void OnDestroy()
        {
            Singleton.Game.playerData.OnDataChanged -= OnPlayerDataChanged;

            // 이펙트 인스턴스 정리
            foreach (var cachedSkill in cachedSkills)
            {
                if (cachedSkill.effectInstance != null)
                {
                    Destroy(cachedSkill.effectInstance);
                }
            }
        }

        private void OnPlayerDataChanged(string propertyName, object value)
        {
            if (propertyName == nameof(PlayerDataSO.Skills))
            {
                UpdateSkillCache();
            }
        }

        private void UpdateSkillCache()
        {
            foreach (var cachedSkill in cachedSkills)
            {
                if (cachedSkill.effectInstance != null)
                {
                    Destroy(cachedSkill.effectInstance);
                }
            }

            cachedSkills.Clear();
            foreach (var skillEntry in Singleton.Game.playerData.Skills)
            {
                SkillData skillData = Singleton.Skill.GetSkillData(skillEntry.Key);
                if (skillData != null)
                {
                    GameObject effectInstance = null;

                    // 이펙트 프리팹 미리 인스턴스화
                    if (skillData.skillEffectPrefab != null)
                    {
                        effectInstance = Instantiate(skillData.skillEffectPrefab, transform);
                        effectInstance.SetActive(false);
                    }

                    cachedSkills.Add(new CachedSkillInfo
                    {
                        skillData = skillData,
                        skillLevel = skillEntry.Value,
                        keyCode = skillData.defaultKey,
                        effectInstance = effectInstance
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
            if (Singleton.Skill.IsSkillOnCooldown(skill.skillID) || IsSkillInUse)
            {
                return;
            }

            // 애니메이션 실행
            IsSkillInUse = true;
            //animator.applyRootMotion = true;
            if (skill.skillExecuter is ISkill skillScript)
            {
                skillScript.ExecuteSkill(animator, this);
            }

            //이펙트 재생
            PlaySkillEffectsLocal(skill.skillID, GetSkillEffectPosition());

            // 주변 적을 찾아 공격
            //TryUseSkillOnEnemy(skill);

            // 쿨타임 설정
            Singleton.Skill.SetSkillCooldown(skill.skillID);
        }

        private Vector3 GetSkillEffectPosition()
        {
            Vector3 position;

            position = transform.position + transform.forward * 1.5f;
            position.y += 1f;

            return position;
        }

        public void OnSkillEnd()
        {
            IsSkillInUse = false;
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
            SkillData skill = Singleton.Skill.GetSkillData(skillID);
            if (skill == null || target == null) return;

            if (!Singleton.Game.playerData.Skills.ContainsKey(skillID)) return;
            int skillLevel = Singleton.Game.playerData.Skills[skillID];

            SkillLevelData levelData = skill.GetSkillLevelData(skillLevel);
            if (levelData == null) return;

            if (target.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(skill.baseDamage * levelData.effectMultiplier);
            }

            RpcPlaySkillEffects(skillID, target.transform.position);
        }

        [Command]
        private void CmdRequestPlaySkillEffects(int skillID, Vector3 targetPosition)
        {
            RpcPlaySkillEffects(skillID, targetPosition);
        }

        [ClientRpc]
        private void RpcPlaySkillEffects(int skillID, Vector3 targetPosition)
        {
            if (!isLocalPlayer)
            {
                PlaySkillEffectByID(skillID, targetPosition);
            }
        }

        private void PlaySkillEffectsLocal(int skillID, Vector3 targetPosition)
        {
            PlaySkillEffectByID(skillID, targetPosition);
            CmdRequestPlaySkillEffects(skillID, targetPosition);
        }

        private void PlaySkillEffectByID(int skillID, Vector3 targetPosition)
        {
            GameObject effectInstance = null;

            CachedSkillInfo? foundSkill = null;
            foreach (var cachedSkill in cachedSkills)
            {
                if (cachedSkill.skillData.skillID == skillID)
                {
                    foundSkill = cachedSkill;
                    break;
                }
            }

            //다른 플레이어 스킬 이펙트 임시 인스턴스
            if (foundSkill.HasValue && foundSkill.Value.effectInstance != null)
            {
                effectInstance = foundSkill.Value.effectInstance;
            }
            else
            {
                SkillData skillData = Singleton.Skill.GetSkillData(skillID);
                if (skillData == null || skillData.skillEffectPrefab == null)
                    return;

                effectInstance = Instantiate(skillData.skillEffectPrefab, transform);
                effectInstance.SetActive(false);
            }

            // 위치 설정
            effectInstance.transform.position = targetPosition;
            effectInstance.transform.rotation = Quaternion.identity;

            ParticleSystem[] particleSystems = effectInstance.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in particleSystems)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Play();
            }

            if (!effectInstance.activeSelf)
            {
                effectInstance.SetActive(true);
            }

            if (!foundSkill.HasValue)
            {
                float maxDuration = 0f;
                foreach (var ps in particleSystems)
                {
                    float duration = ps.main.duration + ps.main.startLifetime.constantMax;
                    if (duration > maxDuration)
                        maxDuration = duration;
                }

                if (maxDuration > 0f)
                {
                    Destroy(effectInstance, maxDuration);
                }
            }
        }
    }
}
