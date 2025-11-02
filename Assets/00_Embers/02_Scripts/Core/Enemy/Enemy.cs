using Mirror;
using UnityEngine;

namespace NOLDA
{
    public class Enemy : NetworkBehaviour
    {
        [SyncVar] public float maxHp = 100;
        [SyncVar] private float currentHp;

        private void Start()
        {
            currentHp = maxHp;
        }

        [Server]
        public void TakeDamage(float damage)
        {
            currentHp -= damage;
            Debug.Log($"몬스터 {gameObject.name}가 {damage}의 피해를 받음. 현재 HP: {currentHp}");

            if (currentHp <= 0)
            {
                Die();
            }
        }

        [Server]
        private void Die()
        {
            Debug.Log($"몬스터 {gameObject.name}가 사망했습니다!");
            NetworkServer.Destroy(gameObject);
        }
    }
}