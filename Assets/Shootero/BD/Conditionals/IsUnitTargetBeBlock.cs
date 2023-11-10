using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Is Unit Target be block by obstacle")]
    [TaskCategory("Shootero")]
    public class IsUnitTargetBeBlock : Conditional
    {
        public SharedTransform target;
        DonViChienDau unit;

        public override void OnAwake()
        {

        }

        public override void OnStart()
        {
            if (target != null &&
                target.Value != null &&
                target.Value.gameObject != null)
                unit = target.Value.gameObject.GetComponent<DonViChienDau>();
        }

        public override TaskStatus OnUpdate()
        {
            if (unit != null)
            {
                var startPos = transform.position;
                var dir = unit.transform.position - startPos;
                if (Physics.Raycast(transform.position + Vector3.up * 1f,
                    dir.normalized,
                    dir.magnitude,
                    LayerMask.GetMask("Default", "Obstacle")))
                {
                    return TaskStatus.Success;
                }

                return TaskStatus.Failure;
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
