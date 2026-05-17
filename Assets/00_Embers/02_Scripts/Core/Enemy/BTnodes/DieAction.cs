using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Embers;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Die", story: "[Self] die for [Target] and gain [Hxp] HXP", category: "Action", id: "95db283abd2453f42f4f0ee6321dd696")]
public partial class DieAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<int> Hxp;

    private bool hasExecuted = false;
    protected override Status OnStart()
    {
        if (hasExecuted) return Status.Success;
        Self.Value.TryGetComponent<Enemy>(out Enemy enemy);
        enemy.DieAction();
        hasExecuted = true;
        return Status.Success;
    }
}

