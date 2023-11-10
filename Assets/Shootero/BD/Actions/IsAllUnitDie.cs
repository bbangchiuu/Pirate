using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;

    [TaskDescription("IsAllUnitDie")]
    [TaskCategory("Shootero")]
    public class IsAllUnitDie : Conditional
    {
        public SharedGameObjectList listGO;

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            if (listGO == null || listGO.Value == null)
            {
                return TaskStatus.Failure;
            }

            foreach (var go in listGO.Value)
            {
                var unit = go.GetComponent<DonViChienDau>();
                if (unit && unit.IsAlive())
                {
                    return TaskStatus.Failure;
                }
            }

            return TaskStatus.Success;
        }

        public override void OnEnd()
        {
            base.OnEnd();
        }
    }
}
