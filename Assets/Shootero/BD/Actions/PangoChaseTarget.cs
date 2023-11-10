using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;

    [TaskDescription("Pango Chase Target")]
    [TaskCategory("Shootero")]
    public class PangoChaseTarget : Action
    {
        public bool IsFailureOnReachBorder = false;
        public bool IsFailureOnReachMain = false;
        public float chargeSpeed = 10f;
        public float timeDelay = 0.5f;

        float originSpeed;
        NavMeshAgent agent;
        DonViChienDau unit;
        GayDamKhiVaCham[] collisionDamage;
        bool collisionMain = false;
        Vector3 playerPos;

        public override void OnAwake()
        {
            base.OnAwake();
            agent = gameObject.GetComponent<NavMeshAgent>();
            unit = gameObject.GetComponent<DonViChienDau>();
            collisionDamage = gameObject.GetComponentsInChildren<GayDamKhiVaCham>(true);
        }

        public override void OnStart()
        {
            //originSpeed = unit;

            agent.ResetPath();

            collisionMain = false;
            if (collisionDamage != null)
            {
                foreach (var c in collisionDamage)
                {
                    c.onUnitCollision += CollisionDamage_onUnitCollision;
                }
            }
        }

        private void CollisionDamage_onUnitCollision(DonViChienDau obj)
        {
            if (obj == QuanlyManchoi.instance.PlayerUnit)
            {
                collisionMain = true;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.PlayerUnit)
            {
                playerPos = QuanlyNguoichoi.Instance.GetHistoryPos(timeDelay);
            }
            else
            {
                playerPos = transform.position;
            }
#if DEBUG
            //Debug.DrawLine(transform.position, playerPos, Color.blue);
#endif
            agent.speed = chargeSpeed * unit.TimeScale;
            var dir = transform.forward;

            var direction = playerPos - transform.position;
            if (direction.sqrMagnitude > Vector3.kEpsilonNormalSqrt)
            {
                dir = Vector3.RotateTowards(dir, direction.normalized, agent.angularSpeed * Time.deltaTime * Mathf.Deg2Rad, 0);
            }

            transform.forward = dir;
            agent.velocity = agent.speed * dir;
            NavMeshHit hit;
            if (IsFailureOnReachBorder && agent.Raycast(transform.position + dir * 0.5f, out hit))
            {
                agent.ResetPath();
                agent.velocity = Vector3.zero;
                return TaskStatus.Failure;
            }

            if (collisionMain)
            {
                agent.ResetPath();
                agent.velocity = Vector3.zero;
                if (IsFailureOnReachMain) return TaskStatus.Failure;
                return TaskStatus.Success;
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
            //agent.speed = originSpeed;
            agent.speed = unit.GetCurStat().SPD * unit.TimeScale;
            if (collisionDamage != null)
            {
                foreach (var c in collisionDamage)
                    c.onUnitCollision -= CollisionDamage_onUnitCollision;
            }
        }
    }
}