using System;
using Embers;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "[Self] Attack [Target] on [Damage]", category: "Action", id: "b6b7047b7ad3aae775666cf350954980")]
public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<int> Damage;

    protected override Status OnStart()
    {
        if (Target.Value == null)
        {
            return Status.Failure;
        }

        if (Target.Value.TryGetComponent<PlayerEnemyHandler>(out var playerEnemyHandler))
        {
            playerEnemyHandler.TakeDamage(Damage.Value);
        }

        return Status.Success;
    }
}

