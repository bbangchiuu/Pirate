using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("AIChaseTarget")]
    [TaskCategory("Shootero")]
    public class AIChaseTarget : Action
    {
        public SharedTransform target;
        public SharedFloat distance;
        public SharedBool overrideSpd;
        public SharedFloat speed;
        public SharedFloat accel;
        
        NavMeshAgent agent;
        DonViChienDau unit;
        float originSpd;
        float originAcel;
        public override void OnAwake()
        {
            base.OnAwake();
            agent = gameObject.GetComponent<NavMeshAgent>();
            unit = gameObject.GetComponent<DonViChienDau>();
        }

        public override void OnStart()
        {
            agent.velocity = Vector3.zero;
            originSpd = agent.speed;
            originAcel = agent.acceleration;
        }

        public override TaskStatus OnUpdate()
        {
            //if (QuanlyManchoi.instance == null) return TaskStatus.Failure;

            //var player = QuanlyManchoi.instance.PlayerUnit;
            if (target == null || target.Value == null) return TaskStatus.Failure;
            if (agent.isOnNavMesh == false) return TaskStatus.Failure;

            var pos = target.Value.position;

            if (Vector3.Distance(transform.position, pos) > distance.Value)
            {
                if (overrideSpd.Value)
                {
                    agent.speed = speed.Value;
                    agent.acceleration = accel.Value;
                }
                //agent.velocity = (target.Value.position - transform.position).normalized * speed.Value;
                if (agent.SetDestination(pos))
                {
                    return TaskStatus.Running;
                }
                else
                {
                    return TaskStatus.Failure;
                }
            }

            if (agent.isOnNavMesh && agent.hasPath)
                agent.ResetPath();

            return TaskStatus.Success;
        }

        public override void OnEnd()
        {
            base.OnEnd();
            agent.speed = originSpd;
            agent.acceleration = originAcel;
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
            //agent.speed = unit.GetStat().SPD;
        }
    }
}