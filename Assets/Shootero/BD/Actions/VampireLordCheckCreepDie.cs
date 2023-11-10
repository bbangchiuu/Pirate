using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;

    [TaskDescription("VampireLordCheckCreepDie")]
    [TaskCategory("Shootero")]
    public class VampireLordCheckCreepDie : Conditional
    {
        public SharedGameObjectList listGO;

        Vector3 lastPos;
        NavMeshAgent agent;
        DonViChienDau unit;

        public override void OnAwake()
        {
            base.OnAwake();
            agent = gameObject.GetComponent<NavMeshAgent>();
            unit = gameObject.GetComponent<DonViChienDau>();
            lastPos = Vector3.zero;
        }

        public override void OnStart()
        {
            if (lastPos == Vector3.zero)
            {
                lastPos = transform.position;
                lastPos.y = 0;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (listGO == null || listGO.Value == null)
            {
                return TaskStatus.Failure;
            }

            foreach (var go in listGO.Value)
            {
                var unit = go.GetComponent<DonViChienDau>();
                if (unit && unit.IsAlive())
                {
                    lastPos = unit.transform.position;
                    lastPos.y = 0;
                    return TaskStatus.Failure;
                }
            }

            BackToBattle();

            return TaskStatus.Success;
        }

        void BackToBattle()
        {
            lastPos.y = 0;
            transform.position = lastPos;
            unit.SetOutOfTarget(0.1f);
            agent.enabled = true;
            agent.isStopped = false;
        }

        public override void OnEnd()
        {
            base.OnEnd();
        }
    }
}
