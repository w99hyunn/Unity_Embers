using UnityEngine;
using Unity.Behavior;
using UnityEngine.AI;
using System.Linq;

namespace NOLDA
{
    public class EnemyFSM : MonoBehaviour
    {
        private Transform target;
        private NavMeshAgent navMeshAgent;
        private BehaviorGraphAgent behaviorGraphAgent;

        public void Setup(Transform target, GameObject[] waypoints)
        {
            this.target = target;

            TryGetComponent<NavMeshAgent>(out navMeshAgent);
            TryGetComponent<BehaviorGraphAgent>(out behaviorGraphAgent);

            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;


            behaviorGraphAgent.SetVariableValue("PatrolPoints", waypoints.ToList());
        }
    }
}