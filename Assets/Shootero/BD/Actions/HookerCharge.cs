﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;

    [TaskDescription("HookerCharge")]
    [TaskCategory("Shootero")]
    public class HookerCharge : Action
    {
        public SharedVector3 direction;
        public bool IsFailureOnReachMain = false;
        public float chargeSpeed = 10f;

        float originSpeed;
        NavMeshAgent agent;
        DonViChienDau unit;
        GayDamKhiVaCham[] collisionDamage;
        bool collisionMain = false;

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
            agent.speed = chargeSpeed * unit.TimeScale;
            var dir = transform.forward;
            if (direction.Value.sqrMagnitude < Vector3.kEpsilonNormalSqrt)
            {
            }
            else
            {
                dir = direction.Value.normalized;
                //agent.velocity = agent.speed * dir;
            }

            agent.velocity = agent.speed * dir;

            if (collisionMain)
            {
                agent.ResetPath();
                agent.velocity = Vector3.zero;
                if (IsFailureOnReachMain) return TaskStatus.Failure;
                return TaskStatus.Success;
            }

            NavMeshHit hit;
            if (agent.Raycast(transform.position + dir * 0.5f, out hit))
            {
                var outVec = Vector3.Reflect(dir, hit.normal);
                outVec.y = 0;

                transform.forward = outVec;
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