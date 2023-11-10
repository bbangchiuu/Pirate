using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("SetRotationFollower")]
    [TaskCategory("Shootero")]
    public class SetRotationFollower : Action
    {
        public SharedGameObject target;
        public bool followRotation = false;
        public float followRotationSpd = 2f;

        TargetFollower follower;
        public override void OnAwake()
        {
            base.OnAwake();
            if (target == null || target.Value == null)
            {
                follower = gameObject.GetComponent<TargetFollower>();
            }
            else
            {
                follower = target.Value.GetComponent<TargetFollower>();
            }
        }

        public override void OnStart()
        {
            
        }

        public override TaskStatus OnUpdate()
        {
            if (follower)
            {
                follower.followRotation = followRotation;
                follower.followRotationSpd = followRotationSpd;
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
        }
    }
}