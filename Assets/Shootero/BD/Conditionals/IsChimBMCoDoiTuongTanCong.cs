namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Return success when found an alive enemy target")]
    [TaskCategory("Shootero")]
    public class IsChimBMCoDoiTuongTanCong : Conditional
    {
        public SharedGameObject playerGO;
        DonViChienDau unit;
        TuyetKyNhanVat tk;
        public override void OnAwake()
        {
            base.OnAwake();
            unit = GetComponent<DonViChienDau>();
        }

        public override void OnStart()
        {
            base.OnStart();
            if (playerGO != null && playerGO.Value)
            {
                tk = playerGO.Value.GetComponent<TuyetKyNhanVat>();
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (tk == null) return TaskStatus.Failure;

            var sk = tk.GetTuyetKy(0) as BeastMasterSkill;
            if (sk != null)
            {
                var atkCmd = sk.GetAtkCommand();
                if (atkCmd != null)
                {
                    sk.Dequeue();
                    Owner.SetVariableValue("playerTarget", atkCmd.T);
                    unit.OverrideChiSoDam(atkCmd.D);

                    return TaskStatus.Success;
                }
            }

            Owner.SetVariableValue("playerTarget", null);
            return TaskStatus.Failure;
        }
    }
}