using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UpdateDistance", story: "Update [Self] and [Target] [CurrentDistance]", category: "Action", id: "e0570d33f5522861901d56fb59439ffa")]
public partial class UpdateDistanceAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> CurrentDistance;

    protected override Status OnUpdate()
    {
        if (Target.Value == null)
        {
            return Status.Failure;
        }
        CurrentDistance.Value = Vector3.Distance(Self.Value.transform.position, Target.Value.transform.position);
        return Status.Success;
    }
}
