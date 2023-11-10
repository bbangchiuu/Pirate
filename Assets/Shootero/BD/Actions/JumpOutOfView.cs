using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;

    [TaskDescription("JumpOutOfView")]
    [TaskCategory("Shootero")]
    public class JumpOutOfView : Action
    {
        public float minRange = 2;
        public float maxRange = 4;
        public float jumpTime = 3;

        public float randomRangeAroundTarget = 0f;

        public SharedGameObject dropIndicator;

        public SharedTransform targetObject;

        NavMeshAgent agent;
        DonViChienDau unit;

        bool enableAgentStarted = false;
        bool haveTarget;
        NavMeshHit hit;
        Vector3 target;

        public override void OnAwake()
        {
            base.OnAwake();
            agent = gameObject.GetComponent<NavMeshAgent>();
            unit = gameObject.GetComponent<DonViChienDau>();
            if (dropIndicator != null && dropIndicator.Value != null)
                dropIndicator.Value.SetActive(false);
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

            if (targetObject != null)
            {
                haveTarget = true;

                if(randomRangeAroundTarget<0.1f)
                    StartJump(targetObject.Value.position);
                else
                {
                    NavMeshQueryFilter filter = new NavMeshQueryFilter();
                    filter.agentTypeID = agent != null ? agent.agentTypeID : 0;
                    filter.areaMask = NavMesh.AllAreas;

                    if (NavMesh.SamplePosition(targetObject.Value.position, out hit, randomRangeAroundTarget, filter))
                    {
                        haveTarget = true;
                        StartJump(hit.position);
                    }
                }
            }
            else
            {
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
        }

        public override TaskStatus OnUpdate()
        {
            if (haveTarget)
            {
                if (Vector3.SqrMagnitude(transform.position - target) < 0.1f)
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

        const float gravity = -9.8f;
        Vector3 veloc;
        void StartJump(Vector3 target)
        {
            //float jumpHorizonSpeed = 4f;
            //if (unit != null)
            //{
            //    jumpHorizonSpeed = unit.GetStat().SPD;
            //}
            //var curPos = transform.position;

            //var hPos = new Vector3(curPos.x, target.y, curPos.z);
            //var aPos = curPos.y;

            //var dis = Vector3.Distance(hPos, target);
            //var t = dis / jumpHorizonSpeed;
            //var v0y = (target.y - aPos - 0.5f * gravity * t * t) / t;
            //veloc = (target - hPos).normalized * jumpHorizonSpeed;
            //veloc.y = v0y;

            this.target = target;

            if (unit)
            {
                unit.SetOutOfTarget(jumpTime);
            }
            timeCount = 0;
            veloc = Vector3.up * 100; // start jump veloc
        }

        float originSpeed = 0;
        float timeCount = 0;
        void UpdateJumping(Vector3 target)
        {
            //float vy = veloc.y;

            var curPos = transform.position;

            //var newY = curPos.y + vy * Time.deltaTime;
            //vy += gravity * Time.deltaTime;

            //veloc.y = 0;
            //curPos.y = target.y;
            //curPos += veloc * Time.deltaTime;
            //curPos.y = newY;

            //veloc.y = vy;
            //transform.position = curPos;
            timeCount += Time.deltaTime;

            if (timeCount < 0.5f)
            {
                //veloc.y += gravity * Time.deltaTime;
                originSpeed = veloc.y;
            }
            else if (timeCount < jumpTime - 0.5f)
            {
                veloc.y = 0;
            }
            else
            {
                if (veloc.y == 0)
                {
                    veloc.y = -originSpeed;
                }

                curPos.x = target.x;
                curPos.z = target.z;
                veloc.y += gravity * Time.deltaTime;
            }

            curPos += veloc * Time.deltaTime;
            if (curPos.y < target.y)
            {
                curPos.y = target.y;
            }

            transform.position = curPos;

            if (timeCount >= 0.5f)
            {
                if (dropIndicator != null && dropIndicator.Value != null)
                {
                    dropIndicator.Value.SetActive(true);
                    dropIndicator.Value.transform.position = target;
                }
            }
            
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
            //if (agent)
            //{
            //    if (enableAgentStarted)
            //    {
            //        agent.enabled = true;
            //        agent.isStopped = false;
            //    }
            //}
        }

        public override void OnEnd()
        {
            base.OnEnd();
            if (dropIndicator != null && dropIndicator.Value != null)
                dropIndicator.Value.SetActive(false);
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