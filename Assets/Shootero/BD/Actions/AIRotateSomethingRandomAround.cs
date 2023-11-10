using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("AIRotateSomethingRandomAround")]
    [TaskCategory("Shootero")]
    public class AIRotateSomethingRandomAround : Action
    {
        public float rotationSpeed = 360f;

        public float minAngle = 10;
        public float maxAngle = 20;
        public SharedTransform ObjToRotate;

        Quaternion targetRotation;


        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override void OnStart()
        {
            Transform trans = ObjToRotate.Value;

            float angle_val = Random.Range(minAngle, maxAngle);

            int _r = Random.Range(0, 100);
            if (_r < 50)
                angle_val = -angle_val;

            targetRotation = Quaternion.Euler(trans.localEulerAngles.x, trans.localEulerAngles.y + angle_val, trans.localEulerAngles.z);
        }

        public override TaskStatus OnUpdate()
        {
            ObjToRotate.Value.localRotation = Quaternion.RotateTowards(ObjToRotate.Value.localRotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(ObjToRotate.Value.localRotation, targetRotation) < 0.3)
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