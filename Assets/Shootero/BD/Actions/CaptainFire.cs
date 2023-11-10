namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine;

    [TaskDescription("Captain Shot")]
    [TaskCategory("Shootero")]
    public class CaptainFire : Action
    {
        //public SharedTransform enemyTarget;
        DonViChienDau unit;

        public override void OnAwake()
        {
            base.OnAwake();
            unit = gameObject.GetComponent<DonViChienDau>();
        }
        public override void OnStart()
        {
            var b = unit.FireAt(Vector3.zero);
        }

        public override TaskStatus OnUpdate()
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