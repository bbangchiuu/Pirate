namespace BehaviorDesigner.Runtime.Tasks
{
    public class SharedBattleUnit : SharedVariable<DonViChienDau>
    {
        public static implicit operator SharedBattleUnit(DonViChienDau value) { return new SharedBattleUnit { Value = value }; }
    }
}