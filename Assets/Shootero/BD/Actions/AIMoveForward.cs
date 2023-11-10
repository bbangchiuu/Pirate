using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("AIMoveForward")]
    [TaskCategory("Shootero")]
    public class AIMoveForward : Action
    {
        public SharedVector3 direction;
        public bool IsFailureOnReachBorder = false;
        public bool IsFailureOnReachMain = false;
        NavMeshAgent agent;
        public override void OnAwake()
        {
            base.OnAwake();
            agent = gameObject.GetComponent<NavMeshAgent>();
        }

        public override void OnStart()
        {
            agent.ResetPath();
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

            agent.velocity = agent.speed * dir;
            NavMeshHit hit;
            if (IsFailureOnReachBorder && agent.Raycast(transform.position + dir * 0.5f, out hit))
            {
                agent.ResetPath();
                agent.velocity = Vector3.zero;
                return TaskStatus.Failure;
            }

            var posOffset = transform.position + Vector3.up * 1f;
            if (IsFailureOnReachMain && Physics.Raycast(posOffset, dir, out RaycastHit rHit, 0.55f, LayersMan.Team1DefaultHitLayerMask))
            {
                agent.ResetPath();
                agent.velocity = Vector3.zero;
                return TaskStatus.Failure;
            }

            return TaskStatus.Success;
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
        }
    }
}