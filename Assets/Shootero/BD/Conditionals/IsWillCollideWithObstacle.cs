using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("IsWillCollideWithObstacle")]
    [TaskCategory("Shootero")]
    public class IsWillCollideWithObstacle : Conditional
    {
        public SharedVector3 arrivingDirection;
        public SharedVector3 outGoingDirection;
        public float distanceAhead = 0.5f;
        
        DonViChienDau unit;
        NavMeshAgent agent;

        public override void OnAwake()
        {
            unit = gameObject.GetComponent<DonViChienDau>();
            agent = gameObject.GetComponent<NavMeshAgent>();
        }

        public override TaskStatus OnUpdate()
        {
            //RaycastHit hit;
            //if (Physics.Raycast(transform.position, transform.forward, out hit, distanceAhead, LayerMask.GetMask("Obstacle")))
            //{
            //    arrivingDirection.SetValue(transform.forward);
            //    outGoingDirection.SetValue(Vector3.Reflect(transform.forward, hit.normal));

            //    return TaskStatus.Success;
            //}
            //else
            if (agent.Raycast(transform.position + transform.forward * distanceAhead, out NavMeshHit navHit))
            {
                
                arrivingDirection.SetValue(transform.forward);
                outGoingDirection.SetValue(Vector3.Reflect(transform.forward, navHit.normal));

                return TaskStatus.Success;
            }
            else 
            {

                return TaskStatus.Failure;
            }
        }

        public override void OnEnd()
        {
        }

        public override void OnReset()
        {

        }
    }
}
