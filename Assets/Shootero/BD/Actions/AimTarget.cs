using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;
    [TaskDescription("AimShooterAtTarget")]
    [TaskCategory("Shootero")]
    public class AimTarget : Action
    {
        public SharedTransform enemyTarget;
        public int aimByShooter = 1;
        public bool runOnLateUpdate = false;
        DonViChienDau unit;
        NavMeshAgent shooterAgent;
        bool shooterAgentUpdateRotation = false;
        public override void OnAwake()
        {
            base.OnAwake();
            unit = gameObject.GetComponent<DonViChienDau>();
        }
        public override void OnStart()
        {
            if (unit == null || unit.shooter == null) return;

            if (shooterAgent == null)
                shooterAgent = unit.shooter.GetComponent<NavMeshAgent>();
            if (shooterAgent)
            {
                shooterAgentUpdateRotation = shooterAgent.updateRotation;
                shooterAgent.updateRotation = false;
            }
        }

        TaskStatus Update()
        {
            if (enemyTarget.Value != null)
            {
                //if (QuanlyNguoichoi.Instance.PlayerUnit == unit)
                unit.SetTarget(enemyTarget.Value);

                var shooter = unit.shooter;
                if (aimByShooter == 2)
                {
                    shooter = unit.shooter2;
                }
                else if (aimByShooter == 3)
                {
                    shooter = unit.shooter3;
                }

                shooter.RotateToTarget(enemyTarget.Value);

                // The task is done waiting if the time waitDuration has elapsed since the task was started.
                if (shooter.CanFireDirection(enemyTarget.Value.position))
                {
                    return TaskStatus.Success;
                }
                // Otherwise we are still waiting.
                return TaskStatus.Running;
            }
            else
            {
                return TaskStatus.Failure;
            }
        }

        TaskStatus lastResult = TaskStatus.Running;

        public override TaskStatus OnUpdate()
        {
            if (runOnLateUpdate == false)
            {
                return Update();
            }
            return lastResult;
        }

        public override void OnLateUpdate()
        {
            if (runOnLateUpdate)
            {
                lastResult = Update();
            }
        }

        public override void OnEnd()
        {
            if (shooterAgent)
            {
                shooterAgent.updateRotation = shooterAgentUpdateRotation;
            }
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
        }
    }
}