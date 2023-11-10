namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine;

    [TaskDescription("Return success when unit in atk range")]
    [TaskCategory("Shootero")]
    public class IsUnitInAtkRange : Conditional
    {
        public SharedFloat curDistance = 0;
        public SharedTransform targetUnit;

        DonViChienDau unit;

        public override void OnAwake()
        {
            unit = gameObject.GetComponent<DonViChienDau>();
        }

        public override TaskStatus OnUpdate()
        {
            if (unit == null) return TaskStatus.Failure;

            var rangeCheck = unit.GetCurStat().ATK_RANGE;

            if (curDistance != null && targetUnit != null && targetUnit.Value != null)
            {
                curDistance.SetValue(Vector3.Distance(unit.transform.position, targetUnit.Value.position));

                if (curDistance.Value <= rangeCheck)
                {
                    return TaskStatus.Success;
                }
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
