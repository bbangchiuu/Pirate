using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;

    [TaskDescription("AIRotateAroundSelf")]
    [TaskCategory("Shootero")]
    public class AIRotateAroundSelf : Action
    {
        public float rotationSpeed = 360f;
        public bool CounterClock;

        public SharedGameObject VisualTarget;
        public float VisualTarget_rotationSpeed = 360f;
        private Vector3 VisualTargetStartEuler = Vector3.zero;
        private Vector3 VisualTargetEuler = Vector3.zero;

        NavMeshAgent agent;
        bool updateRotation = true;

        public override void OnAwake()
        {
            base.OnAwake();
            agent = gameObject.GetComponent<NavMeshAgent>();
        }

        public override void OnStart()
        {
            updateRotation = agent.updateRotation;
            agent.updateRotation = false;
        }

        public override TaskStatus OnUpdate()
        {
            float dir = CounterClock ? -1f : 1f;
            var origin = transform.localEulerAngles;
            var yAngle = origin.y + dir * rotationSpeed * Time.deltaTime;
            if (yAngle > 180f)
            {
                yAngle -= 360f;
            }
            origin.y = yAngle;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(origin), rotationSpeed * Time.deltaTime);

            if (VisualTarget.Value != null)
            {
                var visual_origin = VisualTarget.Value.transform.localEulerAngles;
                var visual_yAngle = visual_origin.y + dir * VisualTarget_rotationSpeed * Time.deltaTime;
                if (visual_yAngle > 180f)
                {
                    visual_yAngle -= 360f;
                }
                VisualTargetEuler.y = visual_yAngle;

                VisualTarget.Value.transform.localEulerAngles = VisualTargetEuler;
            }

            return TaskStatus.Success;
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
        }

        public override void OnEnd()
        {
            agent.updateRotation = updateRotation;
        }
    }

}
