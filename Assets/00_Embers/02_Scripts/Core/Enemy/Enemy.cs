using Mirror;
using UnityEngine;
using Unity.Behavior;
using UnityEngine.AI;
using System.Linq;

namespace NOLDA
{
    public class Enemy : NetworkBehaviour
    {
        [Header("Enemy Settings")]
        [SerializeField][SyncVar] private float maxHp = 100;
        [SyncVar] private float currentHp;

        private Transform target;
        private GameObject[] waypoints;
        private NavMeshAgent navMeshAgent;
        private BehaviorGraphAgent behaviorGraphAgent;
        private EnemySpawner enemySpawner;

        void Awake()
        {
            TryGetComponent<NavMeshAgent>(out navMeshAgent);
            TryGetComponent<BehaviorGraphAgent>(out behaviorGraphAgent);
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
                navMeshAgent.enabled = true;
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

            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = false;
            }
        }

        private void Start()
        {
            currentHp = maxHp;
        }

        [Server]
        public void TakeDamage(float damage)
        {
            currentHp -= damage;
            Debug.Log($"{gameObject.name} 피해 {damage}, 현재 HP: {currentHp}");

            if (currentHp <= 0)
            {
                Die();
            }
        }

        [Server]
        private void Die()
        {
            Debug.Log($"{gameObject.name} 사망");

            enemySpawner.OnEnemyDied(gameObject);
            NetworkServer.Destroy(gameObject);
        }
    }
}