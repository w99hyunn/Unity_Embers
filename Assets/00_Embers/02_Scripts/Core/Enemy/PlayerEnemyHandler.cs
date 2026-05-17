using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Embers
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

        [Server]
        public void GainEnemyReward(EnemySO enemyData)
        {
            List<int> itemIds = new List<int>();
            List<int> amounts = new List<int>();

            foreach (EnemySO.DropEntry entry in enemyData.DropEntries)
            {
                if (UnityEngine.Random.value < entry.DropChance)
                {
                    itemIds.Add(entry.ItemData.ID);
                    amounts.Add(entry.Amount);
                }
            }

            TargetApplyEnemyReward(connectionToClient, enemyData.Hxp, itemIds.ToArray(), amounts.ToArray());
        }

        [TargetRpc]
        public void TargetApplyGainHxp(NetworkConnection target, int hxp)
        {
            Singleton.Game.playerData.Hxp += hxp;
        }

        [TargetRpc]
        public void TargetApplyEnemyReward(NetworkConnection target, int hxp, int[] itemIds, int[] amounts)
        {
            Singleton.Game.playerData.Hxp += hxp;

            InventoryUIController inventory = FindFirstObjectByType<InventoryUIController>(FindObjectsInactive.Include);

            for (int i = 0; i < itemIds.Length; i++)
            {
                ItemData itemData = Singleton.DB.GetItemDataById(itemIds[i]);
                int remainingAmount = inventory.Add(itemData, amounts[i]);
                int gainedAmount = amounts[i] - remainingAmount;

                if (gainedAmount > 0)
                {
                    InGameChatNoticeHandler.Notice("획득", $"{itemData.Name} x{gainedAmount}");
                }
            }
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

            int finalDamage = Mathf.Max(1, damage - Singleton.Game.playerData.TotalArmor);
            Singleton.Game.playerData.Hp -= finalDamage;

            if (Singleton.Game.playerData.Hp <= 0)
            {
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

            playerController.enabled = false;
            this.transform.position = Singleton.Game.DefaultPosition;
            playerController.enabled = true;

            animator.ResetTrigger("isDie");
            animator.Rebind();
            animator.Update(0);

            OnPlayerRespawned?.Invoke();
        }

        #endregion
    }
}
