using Mirror;
using UnityEngine;

namespace NOLDA
{
    public class PlayerEnemyHandler : NetworkBehaviour
    {
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
            Singleton.Game.playerData.Hp = 0;
            playerController.State = PlayerController.PlayerState.Dead;
            animator.SetTrigger("isDie");
        }

        #endregion
    }
}