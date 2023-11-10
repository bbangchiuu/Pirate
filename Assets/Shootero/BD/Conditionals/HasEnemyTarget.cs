namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Return success when found an alive enemy target")]
    [TaskCategory("Shootero")]
    public class HasEnemyTarget : Conditional
    {
        public SharedTransform enemyTarget;
        DonViChienDau unit;

        public override void OnAwake()
        {
            unit = gameObject.GetComponent<DonViChienDau>();
        }

        public override TaskStatus OnUpdate()
        {
            var enemy = QuanlyManchoi.FindClosestEnemyCanFire(transform.position);
            
            if (enemy != null)
            {
                enemyTarget.SetValue(enemy.transform);
                unit.SetTarget(enemy.transform);
                //if (enemy.IsBoss)
                //{
                //    ScreenBattle.instance.sliderExpGrp.alpha = 0;
                //    ScreenBattle.instance.sliderBossHPGrp.alpha = 1;

                //    var hpBar = ScreenBattle.instance.CreateHPBar(enemy);
                //    hpBar.UpdateHP();
                //}
                //else
                //{
                //    ScreenBattle.instance.sliderExpGrp.alpha = 1;
                //    ScreenBattle.instance.sliderBossHPGrp.alpha = 0;
                //}

                return TaskStatus.Success;
            }
            else
            {
                //ScreenBattle.instance.sliderExpGrp.alpha = 1;
                //ScreenBattle.instance.sliderBossHPGrp.alpha = 0;
                unit.SetTarget(null);

                enemyTarget.SetValue(null);
                return TaskStatus.Failure;
            }
        }

        public override void OnEnd()
        {
        }

        public override void OnReset()
        {
            enemyTarget = null;
        }
    }
}
