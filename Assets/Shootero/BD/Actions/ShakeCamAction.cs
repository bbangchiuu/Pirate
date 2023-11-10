namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine;

    [TaskDescription("Shake Camera")]
    [TaskCategory("Shootero")]
    public class ShakeCamAction : Action
    {
        public float magnitude = 1f;
        public float roughness = 1f;
        public float fadeInTime = 0.1f;
        public float fadeOutTime = 2f;

        public override void OnAwake()
        {
            base.OnAwake();
        }
        public override void OnStart()
        {
            EZCameraShake.CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
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