using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;

    [TaskDescription("AIRotateSomething")]
    [TaskCategory("Shootero")]
    public class AIRotateSomething : Action
    {
        public float rotationSpeed = 360f;
        public bool CounterClock;
        public SharedTransform ObjToRotate;

        public bool ShouldLateUpdate = false;

        private Vector3 targetEul = Vector3.zero;

        public override void OnAwake()
        {
            base.OnAwake();
            //agent = gameObject.GetComponent<NavMeshAgent>();
        }

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            if (ShouldLateUpdate)
            {
                float dir = CounterClock ? -1f : 1f;

                var yAngle = dir * rotationSpeed * Time.deltaTime;
                //if (yAngle > 180f)
                //{
                //    yAngle -= 360f;
                //}

                targetEul.y += yAngle;
                return TaskStatus.Running;
            }
            else
            {
                float dir = CounterClock ? -1f : 1f;
                var origin = ObjToRotate.Value.localEulerAngles;
                var yAngle = origin.y + dir * rotationSpeed * Time.deltaTime;
                if (yAngle > 180f)
                {
                    yAngle -= 360f;
                }
                origin.y = yAngle;

                ObjToRotate.Value.rotation = Quaternion.RotateTowards(ObjToRotate.Value.rotation, Quaternion.Euler(origin), rotationSpeed * Time.deltaTime);
                return TaskStatus.Success;
            }

            
        }

        public override void OnLateUpdate()
        {
            if (ShouldLateUpdate)
                ObjToRotate.Value.Rotate(targetEul, Space.World);
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
            targetEul = Vector3.zero;
        }

        public override void OnEnd()
        {
            //agent.updateRotation = true;
        }
    }

}
