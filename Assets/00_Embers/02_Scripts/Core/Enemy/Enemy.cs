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
                //navMeshAgent.updateRotation = false;
                //navMeshAgent.updateUpAxis = false;
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

        private void Update()
        {
            if (isServer)
            {
                FindClosestPlayer(10f);
            }
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

        /// <summary>
        /// Player를 찾는 메소드임. BT에서 직접 시행시 Player 태그를 찾지 못하는 문제가 발생함.
        /// 따라서 Enemy.cs에서 찾은 후 BT의 Target.Value에 할당함.
        /// 모든 몬스터 판단 로직(BT)는 Server에서 시행됨.
        /// </summary>
        /// <param name="radius">탐색 범위</param>
        [Server]
        public void FindClosestPlayer(float radius)
        {
            GameObject closest = null;
            float closestSqrDist = radius * radius;

            var serverPlayers = Singleton.Session.ServerPlayers;
            if (serverPlayers.Count == 0)
            {
                return;
            }

            foreach (var player in serverPlayers)
            {
                if (player == null || !player.activeInHierarchy)
                {
                    continue;
                }

                float sqrDist = (player.transform.position - transform.position).sqrMagnitude;
                if (sqrDist < closestSqrDist)
                {
                    closestSqrDist = sqrDist;
                    closest = player;
                }
            }

            behaviorGraphAgent.SetVariableValue("Target", closest);
            //return closest;
        }
    }
}