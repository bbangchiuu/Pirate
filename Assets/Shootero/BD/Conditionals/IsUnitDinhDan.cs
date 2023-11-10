namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine;

    [TaskDescription("Unit bi dinh dan")]
    [TaskCategory("Shootero")]
    public class IsUnitDinhDan : Conditional
    {
        DonViChienDau unit;
        bool isUnitDinhDan = false;

        public override void OnAwake()
        {
            unit = gameObject.GetComponent<DonViChienDau>();
            unit.OnUnitDinhDan += (takendmg) =>
            {
                isUnitDinhDan = true;
            };
        }

        public override TaskStatus OnUpdate()
        {
            if (isUnitDinhDan) return TaskStatus.Success;
            return TaskStatus.Failure;
        }

        public override void OnEnd()
        {
            isUnitDinhDan = false;
        }

        public override void OnReset()
        {
            isUnitDinhDan = false;
        }
    }
}
