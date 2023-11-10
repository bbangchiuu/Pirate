namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Return success when found an alive enemy target")]
    [TaskCategory("Shootero")]
    public class IsFireReady : Conditional
    {
        DonViChienDau unit;
        public int shooterId = 1;

        public override void OnAwake()
        {
            unit = gameObject.GetComponent<DonViChienDau>();
        }

        SungCls GetShooter()
        {
            if (unit == null) return null;
            switch (shooterId)
            {
                case 1:
                    return unit.shooter;
                case 2:
                    return unit.shooter2;
                case 3:
                    return unit.shooter3;
                case 4:
                    return unit.shooter4;
                default:
                    return null;
            }
        }

        public override TaskStatus OnUpdate()
        {
            var shooter = GetShooter();
            if (shooter == null)
            {
                return TaskStatus.Failure;
            }

            if (shooter.IsFireReady())
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

        public override void OnEnd()
        {
        }

        public override void OnReset()
        {
            
        }
    }
}
