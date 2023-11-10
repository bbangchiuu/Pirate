using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;
    [TaskDescription("Set Barel Mask")]
    [TaskCategory("Shootero")]
    public class SetMaskNongSung : Action
    {
        public int sungId = 1;
        public SharedInt mask;

        DonViChienDau unit;
        SungCls shooter;
        public override void OnAwake()
        {
            base.OnAwake();
            unit = gameObject.GetComponent<DonViChienDau>();
            shooter = GetShooter();
        }
        SungCls GetShooter()
        {
            if (shooter == null)
            {
                switch (sungId)
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

        public override TaskStatus OnUpdate()
        {
            if (shooter == null) return TaskStatus.Failure;
            shooter.SetBarelMask((uint)mask.Value);
            return TaskStatus.Success;
        }

        public override void OnEnd()
        {
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
        }
    }
}