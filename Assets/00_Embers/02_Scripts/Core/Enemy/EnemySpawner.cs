using UnityEngine;
using UnityEngine.AI;
using System.Threading;
using System.Collections.Generic;
using Mirror;

namespace NOLDA
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private float spawnInterval = 5f;
        [SerializeField] private int maxEnemyCount = 10;
        [SerializeField] private float navMeshSampleDistance = 10f;
        [SerializeField] private int maxSpawnAttempts = 30;

        private BoxCollider boxCollider;
        private CancellationTokenSource cancellationTokenSource;
        private List<GameObject> spawnedEnemies = new List<GameObject>();

        private void Awake()
        {
            TryGetComponent<BoxCollider>(out boxCollider);
        }

        private async void Start()
        {
            if (NetworkServer.active)
            {
                cancellationTokenSource = new CancellationTokenSource();
                await SpawnEnemyRoutine(cancellationTokenSource.Token);
            }
        }

        private void OnDestroy()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
        }

        private async Awaitable SpawnEnemyRoutine(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Awaitable.WaitForSecondsAsync(spawnInterval, cancellationToken);
                SpawnEnemy();
            }
        }

        [Server]
        public void SpawnEnemy()
        {
            if (spawnedEnemies.Count >= maxEnemyCount)
            {
                return;
            }

            Vector3 spawnPosition = FindRandomPosition();

            //navMesh를 못찾은 경우 소환하지 않음.
            if (spawnPosition == Vector3.zero)
            {
                return;
            }

            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            GameObject[] waypoints = CreateWaypoints();
            Enemy enemyFSM = enemyInstance.GetComponent<Enemy>();
            enemyFSM.Setup(waypoints, this);

            NetworkServer.Spawn(enemyInstance);
            spawnedEnemies.Add(enemyInstance);
        }

        [Server]
        public void OnEnemyDied(GameObject enemy)
        {
            spawnedEnemies.Remove(enemy);
        }

        private Vector3 FindRandomPosition()
        {
            Bounds bounds = boxCollider.bounds;

            for (int i = 0; i < maxSpawnAttempts; i++)
            {
                Vector3 randomPoint = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    bounds.center.y,
                    Random.Range(bounds.min.z, bounds.max.z)
                );

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, navMeshSampleDistance, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            return Vector3.zero; //navMesh 못찾으면 zero 반환
        }

        private GameObject[] CreateWaypoints()
        {
            GameObject[] waypoints = new GameObject[3];

            for (int i = 0; i < 3; i++)
            {
                Vector3 waypointPosition = FindRandomPosition();
                GameObject waypoint = new GameObject($"Waypoint_{i}");
                waypoint.transform.position = waypointPosition;
                waypoints[i] = waypoint;
            }

            return waypoints;
        }
    }
}