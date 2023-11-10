using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;

    [TaskDescription("AIChargeByTime")]
    [TaskCategory("Shootero")]
    public class AIChargeByTime : Action
    {
        public SharedTransform targetTran;
        public SharedVector3 targetPos;
        public float MinRandomAroundTarget;
        public float MaxRandomAroundTarget;

        public float offset;
        public float maxTime = 1f;

        float originSpeed;
        NavMeshAgent agent;
        DonViChienDau unit;
        Vector3 veloc;
        Vector3 target;
        float timeCount = 0;

        public override void OnAwake()
        {
            base.OnAwake();
            agent = gameObject.GetComponent<NavMeshAgent>();
            unit = gameObject.GetComponent<DonViChienDau>();
        }

        public override void OnStart()
        {
            //originSpeed = unit;

            agent.ResetPath();
            var posTarget = targetPos.Value;
            if (targetTran.Value != null)
            {
                posTarget = targetTran.Value.position;
            }

            if (MaxRandomAroundTarget > 0)
            {
                var range = MaxRandomAroundTarget;
                if (MinRandomAroundTarget < MaxRandomAroundTarget)
                    range = Random.Range(Mathf.Max(MinRandomAroundTarget, 0), MaxRandomAroundTarget);

                var r = Random.insideUnitCircle * range;
                posTarget += new Vector3(r.x, 0, r.y);
            }

            var direction = posTarget - transform.position;
            if (direction.magnitude > offset)
            {
                target = posTarget - direction.normalized * offset;
            }
            else
            {
                target = transform.position;
            }

            veloc = (target - transform.position) / maxTime * unit.TimeScale;
            timeCount = 0;
            agent.speed = veloc.magnitude;
        }

        public override TaskStatus OnUpdate()
        {
            var time = maxTime / unit.TimeScale;
            if (timeCount > time)
            {
                agent.Warp(target);
                agent.ResetPath();
                agent.velocity = Vector3.zero;
                return TaskStatus.Success;
            }

            timeCount += Time.deltaTime;

            agent.velocity = veloc;
            //NavMeshHit hit;
            //if (IsFailureOnReachBorder && agent.Raycast(transform.position + agent.velocity.normalized * 0.5f, out hit))
            //{
            //    agent.ResetPath();
            //    agent.velocity = Vector3.zero;
            //    return TaskStatus.Failure;
            //}

            //if (collisionMain)
            //{
            //    agent.ResetPath();
            //    agent.velocity = Vector3.zero;
            //    if (IsFailureOnReachMain) return TaskStatus.Failure;
            //    return TaskStatus.Success;
            //}

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
            //agent.speed = originSpeed;
            agent.speed = unit.GetCurStat().SPD * unit.TimeScale;
        }
    }
}