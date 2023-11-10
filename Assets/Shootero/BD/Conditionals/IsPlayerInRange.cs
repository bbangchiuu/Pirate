namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine;

    [TaskDescription("Return success when player in range")]
    [TaskCategory("Shootero")]
    public class IsPlayerInRange : Conditional
    {
        public SharedFloat curDistance = 0;
        public SharedFloat rangeCheck = 0;

        DonViChienDau unit;

        public override void OnAwake()
        {
            unit = gameObject.GetComponent<DonViChienDau>();
        }

        public override TaskStatus OnUpdate()
        {
            if (QuanlyManchoi.instance == null) return TaskStatus.Failure;

            var player = QuanlyManchoi.instance.PlayerUnit;
            if (player == null) return TaskStatus.Failure;

            var playerBodyRange = player.GetStat().BODY_RADIUS;
            curDistance.SetValue(Vector3.Distance(transform.position, player.transform.position));
            if (curDistance.Value < rangeCheck.Value)
            {
                return TaskStatus.Success;
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
