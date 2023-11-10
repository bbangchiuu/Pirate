using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;
    [TaskDescription("ControlTween")]
    [TaskCategory("Shootero")]
    public class ControlTween : Action
    {
        public SharedGameObject target;

        public bool Reverse = false;

        UITweener Tween;        

        public override void OnAwake()
        {
            base.OnAwake();

            Tween = target.Value.GetComponent<UITweener>();
        }
        public override void OnStart()
        {
            //Tween.enabled = true;
            if (Reverse)
            {
                Tween.PlayReverse();
            }
            else
            {
                Tween.PlayForward();
            }            
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }

        public override void OnEnd()
        {            
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
        }
    }
}