using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using Embers;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wander", story: "[Self] Navigate To WanderPosition [WanderState]", category: "Action", id: "061ce2cad0c95aaa77de346ae9564eaf")]
public partial class WanderAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<bool> WanderState;

    private NavMeshAgent navMeshAgent;
    private Vector3 wanderPosition;
    private float currentWanderTime = 0f;
    private float maxWanderTime = 5f;

    protected override Status OnStart()
    {
        int jitterMin = 0;
        int jitterMax = 360;
        float wanderRadius = UnityEngine.Random.Range(2.5f, 6f);
        int wanderJitter = UnityEngine.Random.Range(jitterMin, jitterMax);

        // 목표 위치 = 자신(Selft)의 위치 각도(wanderJitter)에 해당하는 반지름(wanderRadius) 크기의 원의 둘레 위치
        wanderPosition = Self.Value.transform.position + Utils.GetPositionFromAngle(wanderRadius, wanderJitter);
        wanderPosition.y = Self.Value.transform.position.y;

        Self.Value.TryGetComponent<NavMeshAgent>(out navMeshAgent);
        navMeshAgent.SetDestination(wanderPosition);
        currentWanderTime = Time.time;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if ((wanderPosition - Self.Value.transform.position).sqrMagnitude < 0.1f || Time.time - currentWanderTime > maxWanderTime)
        {
            WanderState.Value = false;
            return Status.Success;
        }
        WanderState.Value = true;
        return Status.Running;
    }
}

