using Mirror;
using UnityEngine;
using Unity.Behavior;
using UnityEngine.AI;
using System.Linq;

namespace NOLDA
{
    public class Enemy : NetworkBehaviour
    {
        private Transform target;
        private GameObject[] waypoints;
        private NavMeshAgent navMeshAgent;
        private BehaviorGraphAgent behaviorGraphAgent;
        private EnemySpawner enemySpawner;

        void Awake()
        {
            TryGetComponent<NavMeshAgent>(out navMeshAgent);
            TryGetComponent<BehaviorGraphAgent>(out behaviorGraphAgent);

            if (behaviorGraphAgent != null && !NetworkServer.active)
            {
                behaviorGraphAgent.enabled = false;
            }
        }

        [Server]
        public void Setup(Transform target, GameObject[] waypoints, EnemySpawner spawner)
        {
            this.target = target;
            this.waypoints = waypoints;
            this.enemySpawner = spawner;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            if (behaviorGraphAgent != null)
            {
                behaviorGraphAgent.enabled = true;
            }

            if (navMeshAgent != null)
            {
                navMeshAgent.updateRotation = false;
                navMeshAgent.updateUpAxis = false;
            }

            if (behaviorGraphAgent != null && waypoints != null && waypoints.Length > 0)
            {
                behaviorGraphAgent.SetVariableValue("PatrolPoints", waypoints.ToList());
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (behaviorGraphAgent != null)
            {
                behaviorGraphAgent.enabled = false;
            }
        }



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

            enemySpawner.OnEnemyDied(gameObject);
            NetworkServer.Destroy(gameObject);
        }
    }
}