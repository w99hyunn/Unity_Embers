using System;
using Mirror;
using UnityEngine;

namespace NOLDA
{
    public class PlayerEnemyHandler : NetworkBehaviour
    {
        public event Action OnPlayerDied;
        public event Action OnPlayerRespawned;

        private PlayerController playerController;
        private Animator animator;

        private void Awake()
        {
            TryGetComponent<PlayerController>(out playerController);
            TryGetComponent<Animator>(out animator);
        }

        #region # HP / Damage

        /// <summary>
        /// 몬스터 공격으로 플레이어가 피해를 입을 때 서버에서 호출.
        /// </summary>
        [Server]
        public void TakeDamage(float damage)
        {
            int intDamage = Mathf.Max(1, Mathf.RoundToInt(damage));
            TargetApplyDamage(connectionToClient, intDamage);
        }

        [Server]
        public void GainHxp(int hxp)
        {
            TargetApplyGainHxp(connectionToClient, hxp);
        }

        [TargetRpc]
        public void TargetApplyGainHxp(NetworkConnection target, int hxp)
        {
            Singleton.Game.playerData.Hxp += hxp;
        }

        /// <summary>
        /// targetPRC로 해당 클라이언트에서만 실행 / HP 감소 > 로컬 PlayerDataSO
        /// </summary>
        [TargetRpc]
        private void TargetApplyDamage(NetworkConnection target, int damage)
        {
            if (Singleton.Game.playerData.Hp <= 0)
            {
                return;
            }

            Singleton.Game.playerData.Hp -= damage;
            DebugUtils.Log($"{gameObject.name} 피해 {damage}, 현재 HP: {Singleton.Game.playerData.Hp}/{Singleton.Game.playerData.TotalMaxHp}");

            if (Singleton.Game.playerData.Hp <= 0)
            {
                DebugUtils.Log("사망");
                //사망 UI 및 서버에 전송 필요
                DiePlayer();
            }
        }

        private void DiePlayer()
        {
            animator.SetTrigger("isDie");
            Singleton.Game.playerData.Hp = 0;
            playerController.State = PlayerController.PlayerState.Dead;

            OnPlayerDied?.Invoke();
        }

        /// <summary>
        /// 플레이어 부활 처리
        /// </summary>
        public void RespawnPlayer()
        {
            if (playerController.State != PlayerController.PlayerState.Dead)
            {
                return;
            }

            Singleton.Game.playerData.Hp = Singleton.Game.playerData.TotalMaxHp;
            Singleton.Game.playerData.Hxp -= 5000;
            playerController.State = PlayerController.PlayerState.Normal;

            animator.ResetTrigger("isDie");
            animator.Rebind();
            animator.Update(0);

            playerController.enabled = false;
            this.transform.position = Vector3.zero;
            playerController.enabled = true;

            OnPlayerRespawned?.Invoke();
        }

        #endregion
    }
}