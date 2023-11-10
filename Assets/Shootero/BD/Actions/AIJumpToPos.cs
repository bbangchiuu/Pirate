using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Jump to a  pos")]
    [TaskCategory("Shootero")]
    public class AIJumpToPos : Action
    {
        public float minRange = 2;
        public float maxRange = 4;

        public float gravity = -20f;
        //public float jumpHorizonSpeed = 15f;

        public Vector3 position;

        public float TotalJumpTime = 1;

        public SharedGameObject dropIndicator;

        NavMeshAgent agent;
        NavMeshHit hit;
        bool haveTarget;
        bool enableAgentStarted = false;
        DonViChienDau unit;
        public override void OnAwake()
        {
            base.OnAwake();
            agent = gameObject.GetComponent<NavMeshAgent>();
            unit = gameObject.GetComponent<DonViChienDau>();

            if (dropIndicator != null && dropIndicator.Value != null)
                dropIndicator.Value.SetActive(false);
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

            if (NavMesh.SamplePosition(position + dirRandom * randomRange, out hit, checkRange, filter))
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
                    agent.enabled = true;
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
        
        Vector3 veloc;
        void StartJump(Vector3 target)
        {
            float jumpHorizonSpeed = 4f;
            //if (unit != null)
            //{
            //    jumpHorizonSpeed = unit.GetStat().SPD;
            //}

            var curPos = transform.position;

            var hPos = new Vector3(curPos.x, target.y, curPos.z);
            var aPos = curPos.y;

            var dis = Vector3.Distance(hPos, target);
            var t = TotalJumpTime;// dis / jumpHorizonSpeed;

            jumpHorizonSpeed = dis / t;

            jumpTime = t;

            var v0y = (target.y - aPos - 0.5f * gravity * t * t) / t;
            veloc = (target - hPos).normalized * jumpHorizonSpeed;
            veloc.y = v0y;
            this.target = target;
            
            if (unit)
            {
                t = Mathf.Max(0, t - 0.3f);

                unit.SetOutOfTarget(t);
            }

            if (dropIndicator != null && dropIndicator.Value != null)
            {
                dropIndicator.Value.SetActive(true);
                dropIndicator.Value.transform.position = target;
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

            veloc.y = vy;
            transform.position = curPos;

            if (dropIndicator != null && dropIndicator.Value != null)
            {
                dropIndicator.Value.SetActive(true);
                dropIndicator.Value.transform.position = target;
            }
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
