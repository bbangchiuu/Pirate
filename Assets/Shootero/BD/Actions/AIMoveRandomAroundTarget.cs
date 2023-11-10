using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;

    [TaskDescription("Move random to pos arround a target")]
    [TaskCategory("Shootero")]
    public class AIMoveRandomAroundTarget : Action
    {
        public SharedTransform target;
        public float maxTimeMoving = 0;
        public float minRange = 2;
        public float maxRange = 4;
        public float overrideSpeed = 0;

        NavMeshAgent agent;
        NavMeshHit hit;
        bool haveTarget = false;
        float movingTime = 0;
        float originSpd = 0;
        public override void OnAwake()
        {
            base.OnAwake();
            agent = gameObject.GetComponent<NavMeshAgent>();
        }

        public override void OnStart()
        {
            haveTarget = false;
            movingTime = 0;
            originSpd = agent.speed;
        }

        bool GetRandomTarget()
        {
            var randomRange = Random.Range(minRange, maxRange);
            var dirRandom = Quaternion.Euler(0, Random.Range(-180, 180), 0) * Vector3.forward;
            
            float checkRange = Mathf.Max(0.5f, Mathf.Min(randomRange - minRange, maxRange - randomRange));

            NavMeshQueryFilter filter = new NavMeshQueryFilter();
            filter.agentTypeID = agent.agentTypeID;
            filter.areaMask = NavMesh.AllAreas;
            if (NavMesh.SamplePosition(target.Value.position + dirRandom * randomRange, out hit, checkRange, filter))
            {
                haveTarget = true;
                return true;
            }
            haveTarget = false;
            return false;
        }

        public override TaskStatus OnUpdate()
        {
            if (target == null || target.Value == null) return TaskStatus.Failure;
            if (haveTarget == false)
            {
                GetRandomTarget();
            }

            if (haveTarget)
            {
                if (overrideSpeed > 0)
                    agent.speed = overrideSpeed;

                if (Vector3.SqrMagnitude(transform.position - hit.position) < 0.1f)
                {
                    return TaskStatus.Success;
                }
                else
                {
                    if (agent.SetDestination(hit.position))
                        return TaskStatus.Running;
                    else
                    {
                        return TaskStatus.Failure;
                    }
                }
            }

            if (maxTimeMoving > 0)
            {
                movingTime += Time.deltaTime;
                if (movingTime >= maxTimeMoving)
                {
                    if (agent.hasPath)
                        agent.ResetPath();

                    return TaskStatus.Success;
                }
            }

            return TaskStatus.Running;
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
            agent.speed = originSpd;
        }
    }
}
