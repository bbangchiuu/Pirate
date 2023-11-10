using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Dash Forward")]
    [TaskCategory("Shootero")]
    public class DashForward : Action
    {
        public float dashSpeed = 10;
        public float dashTime = 1;
        public float bodyRadius = 0.55f;
        public SharedVector3 direction;
        NavMeshAgent agent;
        DonViChienDau unit;
        float countTime = 0;

        public override void OnAwake()
        {
            base.OnAwake();
            agent = gameObject.GetComponent<NavMeshAgent>();
            unit = gameObject.GetComponent<DonViChienDau>();
        }

        public override void OnStart()
        {
            agent.ResetPath();
            countTime = 0;
        }

        public override TaskStatus OnUpdate()
        {
            var dir = transform.forward;
            if (direction.Value.sqrMagnitude < Vector3.kEpsilonNormalSqrt)
            {
            }
            else
            {
                dir = direction.Value.normalized;
                //agent.velocity = agent.speed * dir;
            }

            var speed = dashSpeed > 0 ? dashSpeed : agent.speed;
            var veloc = speed * dir;

            NavMeshHit hit;
            var posOffset = transform.position + Vector3.up * 1f;
            if (agent.Raycast(transform.position + dir * bodyRadius, out hit) ||
                Physics.Raycast(posOffset + veloc * Time.deltaTime, dir, out RaycastHit rHit, bodyRadius, LayersMan.Team1HitLayerMask))
            {
                agent.ResetPath();
                agent.velocity = Vector3.zero;
            }
            else
            {
                agent.velocity = veloc;
            }
            
            countTime += Time.deltaTime;
            if (countTime >= dashTime)
            {
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
    }
}