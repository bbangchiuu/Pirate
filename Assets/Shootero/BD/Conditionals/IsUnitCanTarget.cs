using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("IsUnitAlive")]
    [TaskCategory("Shootero")]
    public class IsUnitCanTarget : Conditional
    {
        public SharedTransform target;
        DonViChienDau unit;

        public override void OnAwake()
        {

        }

        public override void OnStart()
        {
            if (target != null)
                unit = target.Value.gameObject.GetComponent<DonViChienDau>();
        }

        public override TaskStatus OnUpdate()
        {
            if (unit != null)
            {
                return unit.IsCanTarget() ? TaskStatus.Success : TaskStatus.Failure;
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
