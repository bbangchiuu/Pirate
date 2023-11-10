using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;

    [TaskDescription("Lon Nhao")]
    [TaskCategory("Shootero")]
    public class LonNhaoAction : Action
    {
        public SharedVector3 TocDo;
        public SharedFloat ThoiGian;

        NavMeshAgent agent;
        DonViChienDau unit;
        PlayerMovement movement;
        CharacterController characterController;
        bool turnOffNavmesh = false;

        float time = 0;

        public override void OnAwake()
        {
            base.OnAwake();
            agent = gameObject.GetComponent<NavMeshAgent>();
            unit = gameObject.GetComponent<DonViChienDau>();
            movement = gameObject.GetComponent<PlayerMovement>();
            characterController = gameObject.GetComponent<CharacterController>();
        }

        public override void OnStart()
        {
            if (agent && agent.enabled)
            {
                agent.enabled = false;
                turnOffNavmesh = true;
            }
            if (movement)
            {
                movement.enabled = false;
            }
            time = 0;
        }

        public override TaskStatus OnUpdate()
        {
            // fix loi meo lon qua cua -> loi
            if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.IsLoadingMission)
            {
                time = ThoiGian.Value + 0.1f; // finish running when the loading finish
                return TaskStatus.Running;
            }

            if (time + Time.deltaTime > ThoiGian.Value)
            {
                var deltaT = ThoiGian.Value - time;
                if (deltaT > 0)
                {
                    var motion = deltaT * TocDo.Value;
                    characterController.Move(motion);
                }

                return TaskStatus.Success;
            }
            else
            {
                time += Time.deltaTime;

                characterController.SimpleMove(TocDo.Value);
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
            base.OnEnd();

            if (agent && agent.enabled == false &&
                turnOffNavmesh)
            {
                agent.enabled = true;
            }

            if (movement)
            {
                movement.enabled = true;
            }
        }
    }
}