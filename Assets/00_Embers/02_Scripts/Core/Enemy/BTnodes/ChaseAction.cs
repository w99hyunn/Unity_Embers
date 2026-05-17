using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase", story: "[Self] Navigate To [Target]", category: "Action", id: "7de2b7fa3b6015f9ef06cf01fbebc9bc")]
public partial class ChaseAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private NavMeshAgent navMeshAgent;

    protected override Status OnStart()
    {
        Self.Value.TryGetComponent<NavMeshAgent>(out navMeshAgent);
        navMeshAgent.speed = 5f;
        navMeshAgent.SetDestination(Target.Value.transform.position);

        return Status.Running;
    }

}

