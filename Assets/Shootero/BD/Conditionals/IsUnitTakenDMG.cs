using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("IsUnitTakenDMG")]
    [TaskCategory("Shootero")]
    public class IsUnitTakenDMG : Conditional
    {
        public SharedTransform target;
        DonViChienDau unit;

        public override void OnAwake()
        {
        }

        public override void OnStart()
        {
            if (target != null && target.Value != null)
            {
                unit = target.Value.gameObject.GetComponent<DonViChienDau>();
            }
            else
            {
                unit = gameObject.GetComponent<DonViChienDau>();
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (unit != null)
            {
                return unit.DMGTakenLastFrame > 0 ? TaskStatus.Success : TaskStatus.Failure;
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
