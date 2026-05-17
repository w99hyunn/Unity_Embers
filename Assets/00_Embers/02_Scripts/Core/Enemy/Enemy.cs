using Mirror;
using UnityEngine;
using Unity.Behavior;
using UnityEngine.AI;
using System.Linq;

namespace Embers
{
    public class Enemy : NetworkBehaviour
    {
        private const string IS_ATTACK_PARAMETER = "isAttack";
        private const string IS_DIE_TRIGGER = "isDie";

        [Header("Enemy Settings")]
        [SerializeField] private string enemyName;
        public string EnemyName => enemyName;
        [SerializeField][SyncVar] private float maxHp = 500;
        public float MaxHp => maxHp;
        [SyncVar] private float currentHp;

        private GameObject[] waypoints;
        private NavMeshAgent navMeshAgent;
        private BehaviorGraphAgent behaviorGraphAgent;
        private EnemySpawner enemySpawner;
        private Animator animator;
        private NetworkAnimator networkAnimator;
        private bool isDead;

        public event System.Action<float> OnAttacked;

        void Awake()
        {
            TryGetComponent<NavMeshAgent>(out navMeshAgent);
            TryGetComponent<BehaviorGraphAgent>(out behaviorGraphAgent);
            TryGetComponent<Animator>(out animator);
            TryGetComponent<NetworkAnimator>(out networkAnimator);
        }

        [Server]
        public void Setup(GameObject[] waypoints, EnemySpawner spawner)
        {
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
            if (isDead)
            {
                return;
            }

            currentHp -= damage;

            if (currentHp <= 0)
            {
                currentHp = 0;
                Die();
            }

            AttackMySelf(currentHp);
        }

        [ClientRpc]
        private void AttackMySelf(float newHp)
        {
            OnAttacked?.Invoke(newHp);
        }

        [Server]
        private async void Die()
        {
            isDead = true;
            behaviorGraphAgent.SetVariableValue("currentState", EnemyState.Die);
            PlayDeathAnimation();

            await AwaitDie();
        }

        private async Awaitable AwaitDie()
        {
            await Awaitable.WaitForSecondsAsync(5f);
            enemySpawner.OnEnemyDied(gameObject);
            NetworkServer.Destroy(gameObject);
        }

        public void DieAction()
        {
            PlayDeathAnimation();
        }

        private void PlayDeathAnimation()
        {
            animator.SetBool(IS_ATTACK_PARAMETER, false);
            networkAnimator.SetTrigger(IS_DIE_TRIGGER);
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

            var serverPlayers = Singleton.Network.ServerPlayers;

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
