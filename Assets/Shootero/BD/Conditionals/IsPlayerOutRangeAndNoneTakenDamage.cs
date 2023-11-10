namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine;

    [TaskDescription("Return success when player out range")]
    [TaskCategory("Shootero")]
    public class IsPlayerOutRangeAndNoneTakenDamage : Conditional
    {
        public SharedFloat curDistance = 0;
        public SharedFloat rangeCheck = 0;
        [Tooltip("Thoi gian ghi nho trang thai bi dinh dan")]
        public float timeCheckTakenDmg = 3f;
        DonViChienDau unit;

        float timeGetHurt = 0;

        public override void OnAwake()
        {
            unit = gameObject.GetComponent<DonViChienDau>();
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            if (QuanlyManchoi.instance == null) return TaskStatus.Failure;

            var player = QuanlyManchoi.instance.PlayerUnit;
            if (player == null) return TaskStatus.Failure;

            if (unit.DMGTakenLastFrame > 0)
            {
                timeGetHurt = timeCheckTakenDmg;
            }
            else if (timeGetHurt > 0)
            {
                timeGetHurt -= Time.deltaTime;
            }

            var playerBodyRange = player.GetStat().BODY_RADIUS;
            curDistance.SetValue(Vector3.Distance(transform.position, player.transform.position));
            if (curDistance.Value > rangeCheck.Value && timeGetHurt <= 0)
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
