using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Jump to random pos around")]
    [TaskCategory("Shootero")]
    public class AIRandomJump : Action
    {
        public float minRange = 2;
        public float maxRange = 4;

        NavMeshAgent agent;
        NavMeshHit hit;
        bool haveTarget;
        bool enableAgentStarted = false;
        DonViChienDau unit;

        public bool updateRotation = false;
        public override void OnAwake()
        {
            base.OnAwake();
            agent = gameObject.GetComponent<NavMeshAgent>();
            unit = gameObject.GetComponent<DonViChienDau>();
        }

        Vector3 target;

        float jumpTime = 0f;

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

            var randomRange = Random.Range(minRange, maxRange);
            var dirRandom = Quaternion.Euler(0, Random.Range(-180, 180), 0) * Vector3.forward;
            haveTarget = false;
            float checkRange = Mathf.Max(0.5f, Mathf.Min(randomRange - minRange, maxRange - randomRange));
            NavMeshQueryFilter filter = new NavMeshQueryFilter();
            filter.agentTypeID = agent != null ? agent.agentTypeID : 0;
            filter.areaMask = NavMesh.AllAreas;

            if (NavMesh.SamplePosition(transform.position + dirRandom * randomRange, out hit, checkRange, filter))
            {
                haveTarget = true;
                StartJump(hit.position);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (haveTarget)
            {
                jumpTime -= Time.deltaTime;

                if (Vector3.SqrMagnitude(transform.position - target) < 0.1f || jumpTime < 0f)
                {
                    return TaskStatus.Success;
                }
                else
                {
                    UpdateJumping(target);
                    return TaskStatus.Running;
                }
            }

            return TaskStatus.Failure;
        }

        float gravity = -9.8f;
        Vector3 veloc;
        void StartJump(Vector3 target)
        {
            float jumpHorizonSpeed = 4f;
            if (unit != null)
            {
                jumpHorizonSpeed = unit.GetStat().SPD*unit.TimeScale;
            }
            var curPos = transform.position;

            var hPos = new Vector3(curPos.x, target.y, curPos.z);
            var aPos = curPos.y;

            var dis = Vector3.Distance(hPos, target);
            var t = dis / jumpHorizonSpeed;
            jumpTime = t;

            gravity = -9.8f * unit.TimeScale;

            var v0y = (target.y - aPos - 0.5f * gravity * t * t) / t;
            veloc = (target - hPos).normalized * jumpHorizonSpeed;
            veloc.y = v0y;
            this.target = target;

            if (unit)
            {
                t = Mathf.Max(0, t - 0.3f);

                unit.SetOutOfTarget(t);
            }
        }

        void UpdateJumping(Vector3 target)
        {
            float vy = veloc.y;

            var curPos = transform.position;

            float t = Time.deltaTime;
            var newY = curPos.y + vy * t;

            if (newY < target.y)
            {
                t = (target.y - curPos.y) / vy;
                newY = target.y;
            }

            vy += gravity * t;

            veloc.y = 0;
            curPos.y = target.y;
            curPos += veloc * t;
            curPos.y = newY;

            if (updateRotation)
            {
                transform.forward = veloc.normalized;
            }

            veloc.y = vy;
            transform.position = curPos;
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
            if (agent)
            {
                if (enableAgentStarted)
                {
                    agent.enabled = true;
                    agent.isStopped = false;
                }
            }
        }
    }
}
