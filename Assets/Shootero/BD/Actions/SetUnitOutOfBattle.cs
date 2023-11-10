using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;

    [TaskDescription("SetUnitOutOfBattle")]
    [TaskCategory("Shootero")]
    public class SetUnitOutOfBattle : Action
    {
        NavMeshAgent agent;
        DonViChienDau unit;
        bool enableAgentStarted = false;
        Vector3 lastPos = Vector3.zero;

        public SharedGameObject SkillEff = null;

        public override void OnAwake()
        {
            base.OnAwake();
            agent = gameObject.GetComponent<NavMeshAgent>();
            unit = gameObject.GetComponent<DonViChienDau>();
        }

        public override void OnStart()
        {
            if (agent)
            {
                enableAgentStarted = agent.enabled;
                if (enableAgentStarted)
                {
                    agent.isStopped = true;
                }

                agent.enabled = false;
            }

            if (unit)
            {
                unit.SetOutOfTarget(10000f);
            }

            lastPos = transform.position;
            transform.position = lastPos + Vector3.up * 200;

            if(SkillEff.Value!=null)
            {
                GameObject eff= ObjectPoolManager.SpawnAutoUnSpawn(SkillEff.Value, 3f);
                eff.transform.position = lastPos;
            }
        }

        public override TaskStatus OnUpdate()
        {
            //if (haveTarget)
            //{
            //    if (Vector3.SqrMagnitude(transform.position - target) < 0.1f)
            //    {
            //        return TaskStatus.Success;
            //    }
            //    else
            //    {
            //        UpdateJumping(target);
            //        return TaskStatus.Running;
            //    }
            //}

            return TaskStatus.Success;
        }

        public override void OnEnd()
        {
            base.OnEnd();
            //if (agent)
            //{
            //    if (enableAgentStarted)
            //    {
            //        agent.enabled = true;
            //        agent.isStopped = false;
            //    }
            //}
        }
    }
}
