using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;

    [TaskDescription("Bot Shot")]
    [TaskCategory("Shootero")]
    public class BotFire : Action
    {
        public int shooterId = 1;
        public SharedTransform target;
        public float missAngle = 5f;
        public int count = 1;

        public bool RandomCount = false;
        public int CountMin = 1;
        public int CountMax = 1;

        public bool shouldPlayAtkAnim = true;
           
        public bool aimTargetWhenFire = false;
        public bool doiSungActiveDan = false;

        int firedCount = 0;
        DonViChienDau unit;
        NavMeshAgent shooterAgent;
        bool shooterAgentUpdateRotation = false;
        float timeAimTarget = 0;
        bool changedNavmeshUpdateRotation = false;
        SungCls shooter = null;

        public override void OnAwake()
        {
            base.OnAwake();
            unit = gameObject.GetComponent<DonViChienDau>();
        }
        
        public override void OnStart()
        {
            changedNavmeshUpdateRotation = false;
            if (unit == null || unit.shooter == null) return;
            if (shooterAgent == null)
                shooterAgent = unit.shooter.GetComponent<NavMeshAgent>();
            if (shooterAgent)
            {
                if (doiSungActiveDan || aimTargetWhenFire)
                {
                    shooterAgentUpdateRotation = shooterAgent.updateRotation;
                    shooterAgent.updateRotation = false;
                    changedNavmeshUpdateRotation = true;
                }
            }

            firedCount = 0;

            if(RandomCount)
                RandomShotCount();

            if (target.Value != null)
            {
                unit.SetTarget(target.Value);
            }
        }

        void RandomShotCount()
        {
            count = Random.Range(CountMin, CountMax+1);
        }

        SungCls GetShooter()
        {
            if (shooter == null)
            {
                switch (shooterId)
                {
                    case 4:
                        shooter = unit.shooter4;
                        break;
                    case 3:
                        shooter = unit.shooter3;
                        break;
                    case 2:
                        shooter = unit.shooter2;
                        break;
                    case 1:
                    default:
                        shooter = unit.shooter;
                        break;
                }
            }
            return shooter;
        }

        void Fire()
        {
//#if UNITY_EDITOR
//            // gia lap hack unit khong ban
//            return;
//#endif
            GetShooter();

            if (missAngle > 0)
            {
                if (shooter.Barels != null && shooter.Barels.Length > 0)
                {
                    for (int i = 0; i < shooter.Barels.Length; ++i)
                    {
                        var trans = shooter.Barels[i];
                        trans.localRotation = Quaternion.Euler(0, Random.Range(-Mathf.Abs(missAngle), Mathf.Abs(missAngle)), 0);
                    }
                }
                else
                {
                    var trans = unit.shooter.transform;
                    trans.localRotation = Quaternion.Euler(0, Random.Range(-Mathf.Abs(missAngle), Mathf.Abs(missAngle)), 0);
                }
            }

            switch (shooterId)
            {
                case 4:
                    unit.FireAt4((target != null && target.Value != null) ? target.Value.position : Vector3.zero,
                        shouldPlayAtkAnim);
                    break;
                case 3:
                    unit.FireAt3((target != null && target.Value != null) ? target.Value.position : Vector3.zero,
                        shouldPlayAtkAnim);
                    break;
                case 2:
                    unit.FireAt2((target != null && target.Value != null) ? target.Value.position : Vector3.zero,
                        shouldPlayAtkAnim);
                    break;
                case 1:
                default:
                    unit.FireAt((target != null && target.Value != null) ? target.Value.position : Vector3.zero,
                        shouldPlayAtkAnim);
                    break;
            }
            firedCount++;
            timeAimTarget = 0;
            timeDelayShot = 0;
            QuanlyNguoichoi.Instance.TKSungDaBan();
        }

        float timeDelayShot = 0;

        public override TaskStatus OnUpdate()
        {
            if (unit == null) return TaskStatus.Failure;
            GetShooter();
            if (shooter == null) return TaskStatus.Failure;

            if (doiSungActiveDan &&
                shooter.DelayActiveDamage > 0 &&
                timeDelayShot > 0 &&
                timeDelayShot < shooter.DelayActiveDamage)
            {
                timeDelayShot += Time.deltaTime;
                return TaskStatus.Running;
            }

            if (firedCount < count)
            {
                if (shooter.IsFireReady())
                {
                    if (aimTargetWhenFire && target != null && target.Value != null)
                    {
                        if (shooter.CanFireDirection(target.Value.position))
                        {
                            Fire();
                        }
                        else
                        {
                            if (timeAimTarget > 0.8f * 1f / shooter.AtkSpd)
                            {
                                Fire();
                            }
                            else
                            {
                                //if (shooterAgent && shooterAgent.updateRotation)
                                //{
                                //    shooterAgent.updateRotation = false;
                                //}
                                shooter.RotateToTarget(target.Value);
                                timeAimTarget += Time.deltaTime;

                                return TaskStatus.Running;
                            }
                        }
                    }
                    else
                    {
                        Fire();
                    }
                }

                if (firedCount < count)
                {
                    return TaskStatus.Running;
                }
                else
                {
                    if (RandomCount)
                        RandomShotCount();

                    if (doiSungActiveDan)
                    {
                        timeDelayShot += Time.deltaTime;
                        if (timeDelayShot < shooter.DelayActiveDamage)
                        {
                            return TaskStatus.Running;
                        }
                    }

                    return TaskStatus.Success;
                }
            }
            else
            {
                if (RandomCount)
                    RandomShotCount();

                if (doiSungActiveDan)
                {
                    timeDelayShot += Time.deltaTime;
                    if (timeDelayShot < shooter.DelayActiveDamage)
                    {
                        return TaskStatus.Running;
                    }
                }

                return TaskStatus.Success;
            }
            
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
            firedCount = 0;
            timeAimTarget = 0;
        }

        public override void OnEnd()
        {
            base.OnEnd();

            if (shooterAgent && changedNavmeshUpdateRotation)
            {
                shooterAgent.updateRotation = shooterAgentUpdateRotation;
            }
        }
    }
}