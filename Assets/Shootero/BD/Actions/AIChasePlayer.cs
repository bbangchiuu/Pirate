using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("AIChasePlayer")]
    [TaskCategory("Shootero")]
    public class AIChasePlayer : Action
    {
        NavMeshAgent agent;
        DonViChienDau unit;
        public override void OnAwake()
        {
            base.OnAwake();
            agent = gameObject.GetComponent<NavMeshAgent>();
            unit = gameObject.GetComponent<DonViChienDau>();
        }

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            if (QuanlyManchoi.instance == null) return TaskStatus.Failure;

            var player = QuanlyManchoi.instance.PlayerUnit;
            if (player == null) return TaskStatus.Failure;

            agent.SetDestination(player.transform.position);
            return TaskStatus.Success;
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
            agent.speed = unit.GetStat().SPD;
        }
    }
}