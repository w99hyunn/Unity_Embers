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

        private PlayerController playerController;
        private PlayerInput input;
        private Animator animator;
        private bool _isSkillInUse = false;

        private Dictionary<int, GameObject> effectInstances = new Dictionary<int, GameObject>();

        private void Awake()
        {
            TryGetComponent<PlayerController>(out playerController);
            TryGetComponent<PlayerInput>(out input);
            TryGetComponent<Animator>(out animator);
        }

        private void Start()
        {
            InitializeEffectInstances();
            Singleton.Game.playerData.OnDataChanged += OnPlayerDataChanged;
        }

        private void OnDestroy()
        {
            Singleton.Game.playerData.OnDataChanged -= OnPlayerDataChanged;

            // 이펙트 인스턴스 정리
            foreach (var effectInstance in effectInstances.Values)
            {
                if (effectInstance != null)
                {
                    Destroy(effectInstance);
                }
            }
            effectInstances.Clear();
        }

        private void OnPlayerDataChanged(string propertyName, object value)
        {
            if (propertyName == nameof(PlayerDataSO.Skills))
            {
                InitializeEffectInstances();
            }
        }

        private void InitializeEffectInstances()
        {
            foreach (var effectInstance in effectInstances.Values)
            {
                if (effectInstance != null)
                {
                    Destroy(effectInstance);
                }
            }
            effectInstances.Clear();

            // 플레이어가 보유한 스킬의 이펙트만 미리 인스턴스화
            var playerSkills = Singleton.Skill.GetPlayerSkills();
            foreach (var skillInfo in playerSkills)
            {
                if (skillInfo.skillData.skillEffectPrefab != null)
                {
                    GameObject effectInstance = Instantiate(skillInfo.skillData.skillEffectPrefab, transform);
                    effectInstance.SetActive(false);
                    effectInstances[skillInfo.skillData.skillID] = effectInstance;
                }
            }
        }

        private void Update()
        {
            if (!isLocalPlayer
                || Singleton.Game.playerData == null
                || input.IsPointerOverUI())
                return;

            var playerSkills = Singleton.Skill.GetPlayerSkills();
            foreach (var skillInfo in playerSkills)
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
            bool isPlayerSkill = false;

            // 플레이어가 보유한 스킬인지 확인
            if (effectInstances.TryGetValue(skillID, out effectInstance))
            {
                isPlayerSkill = true;
            }
            else
            {
                // 다른 플레이어의 스킬 이펙트를 위해 임시로 생성
                SkillData skillData = Singleton.Skill.GetSkillData(skillID);
                if (skillData == null || skillData.skillEffectPrefab == null)
                    return;

                effectInstance = Instantiate(skillData.skillEffectPrefab, transform);
                effectInstance.SetActive(false);
            }

            // 소환 위치 설정
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

            // 다른 플레이어의 스킬 이펙트는 재생 후 자동 파괴
            if (!isPlayerSkill)
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
