namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine;

    [TaskDescription("Unit Shot")]
    [TaskCategory("Shootero")]
    public class PlayerFire : Action
    {        
        public SharedTransform enemyTarget;
        DonViChienDau unit;

        public override void OnAwake()
        {
            base.OnAwake();
            unit = gameObject.GetComponent<DonViChienDau>();
        }
        public override void OnStart()
        {
            unit.FireAt((enemyTarget != null && enemyTarget.Value != null) ? enemyTarget.Value.position : Vector3.zero);
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