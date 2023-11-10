namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine;

    [TaskDescription("Countdown timer and return true when it reach zero")]
    [TaskCategory("Shootero")]
    public class IsTimerCountDown : Conditional
    {
        public SharedFloat timer = 0;
        public bool updateTimer = false;

        public override void OnAwake()
        {
        }

        public override TaskStatus OnUpdate()
        {
            if (updateTimer)
                timer.Value -= Time.deltaTime;

            if (timer.Value > 0)
                return TaskStatus.Failure;
            return TaskStatus.Success;
        }

        public override void OnEnd()
        {
        }

        public override void OnReset()
        {

        }
    }
}
