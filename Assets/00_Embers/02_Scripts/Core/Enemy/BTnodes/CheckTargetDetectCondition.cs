using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckTargetDetect", story: "Compare values of [CurrentDistance] and [ChaseDistance]", category: "Conditions", id: "c95e132ac3b23c1958cc095d80af871d")]
public partial class CheckTargetDetectCondition : Condition
{
    [SerializeReference] public BlackboardVariable<float> CurrentDistance;
    [SerializeReference] public BlackboardVariable<float> ChaseDistance;

    public override bool IsTrue()
    {
        if (CurrentDistance.Value <= ChaseDistance.Value)
        {
            return true;
        }
        return false;
    }
}
