using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;
    [TaskDescription("SelfKnockback")]
    [TaskCategory("Shootero")]
    public class SelfKnockback : Action
    {
        DonViChienDau unit;
        public float Distance;

        public override void OnAwake()
        {
            base.OnAwake();
            unit = gameObject.GetComponent<DonViChienDau>();
        }
        public override void OnStart()
        {
            var vec = -unit.transform.forward;
            vec.y = 0;
            unit.KnockBack(vec.normalized * Distance);
        }

        TaskStatus Update()
        {
            return TaskStatus.Success;
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
        }
    }
}