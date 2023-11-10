using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("AIRotateRandomAround")]
    [TaskCategory("Shootero")]
    public class AIRotateRandomAround : Action
    {
        public float rotationSpeed = 360f;
        public float maxAngle = 40;

        Quaternion targetRotation;

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override void OnStart()
        {
            targetRotation = Quaternion.Euler(0, Random.Range(-maxAngle, maxAngle), 0);
        }

        public override TaskStatus OnUpdate()
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.localRotation, targetRotation) < 0.1)
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Running;
            }
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
        }

        public override void OnEnd()
        {
            //agent.updateRotation = true;
        }
    }
}