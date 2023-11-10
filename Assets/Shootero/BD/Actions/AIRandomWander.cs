using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Move random to pos arround")]
    [TaskCategory("Shootero")]
    public class AIRandomWander : Action
    {
        public float maxTimeMoving = 0;
        public float minRange = 2;
        public float maxRange = 4;
        public bool navMeshUpdateRotation = true;
        public float DistanceFromNavmeshBorder = 0;

        NavMeshAgent agent;
        NavMeshHit hit;
        bool haveTarget;
        float timeStuck = 0;
        float movingTime = 0;

        public override void OnAwake()
        {
            base.OnAwake();
            agent = gameObject.GetComponent<NavMeshAgent>();
        }

        public override void OnStart()
        {
            var randomRange = Random.Range(minRange, maxRange);
            var dirRandom = Quaternion.Euler(0, Random.Range(-180, 180), 0) * Vector3.forward;
            haveTarget = false;
            float checkRange = Mathf.Max(0.5f, Mathf.Min(randomRange - minRange, maxRange - randomRange));
            NavMeshQueryFilter filter = new NavMeshQueryFilter();
            filter.agentTypeID = agent.agentTypeID;
            filter.areaMask = NavMesh.AllAreas;
            var newPos = transform.position + dirRandom * randomRange;
            timeStuck = 0;
            movingTime = 0;
            agent.updateRotation = navMeshUpdateRotation;

            if (DistanceFromNavmeshBorder > 0)
            {
                if (NavMesh.Raycast(newPos, newPos + dirRandom * DistanceFromNavmeshBorder, out hit, filter))
                {
                    haveTarget = false;
                    return;
                }
            }

            if (NavMesh.SamplePosition(newPos, out hit, checkRange, filter))
            {
                haveTarget = true;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (haveTarget)
            {
                if (maxTimeMoving > 0)
                {
                    movingTime += Time.deltaTime * Mathf.Max(0, (Owner as BehaviorTree).ThinkingTimeScale);
                    if (movingTime >= maxTimeMoving)
                    {
                        agent.ResetPath();
                        return TaskStatus.Success;
                    }
                }

                if (timeStuck > 1f)
                {
                    if (agent.isOnNavMesh)
                        agent.ResetPath();

                    return TaskStatus.Success;
                }

                if (Vector3.SqrMagnitude(transform.position - hit.position) < 0.1f)
                {
                    return TaskStatus.Success;
                }
                else
                {
                    if (agent.velocity.sqrMagnitude < 0.01f)
                    {
                        timeStuck += Time.deltaTime;
                    }
                    else
                    {
                        timeStuck = 0;
                    }

                    if (agent.isOnNavMesh)
                    {
                        agent.SetDestination(hit.position);
                    }
                    return TaskStatus.Running;
                }
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