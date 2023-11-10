using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("IsUnitAlive")]
    [TaskCategory("Shootero")]
    public class IsUnitAlive : Conditional
    {
        public SharedTransform target;
        DonViChienDau unit;

        public override void OnAwake()
        {
            
        }

        public override void OnStart()
        {
            if (target != null && target.Value && target.Value.gameObject)
                unit = target.Value.gameObject.GetComponent<DonViChienDau>();
        }

        public override TaskStatus OnUpdate()
        {
            if (unit != null)
            {
                return unit.IsAlive() ? TaskStatus.Success : TaskStatus.Failure;
            }

            return TaskStatus.Failure;
        }

        public override void OnEnd()
        {
        }

        public override void OnReset()
        {

        }
    }
}

