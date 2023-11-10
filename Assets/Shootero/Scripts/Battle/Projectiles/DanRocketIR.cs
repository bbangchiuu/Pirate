using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanRocketIR : DanBay
{
    protected override bool OnCatchTarget(DonViChienDau unit)
    {
        var re = base.OnCatchTarget(unit);

        return re;
    }

    protected override bool OnHitObstacle(GameObject obstacle)
    {
        //return base.OnHitObstacle(obstacle);
        return false;
    }

    public override void ActiveDan(float speed, long dmg, Vector3 target)
    {
        long dmgPlayer = 100;
        PiercingCount = 0;
        PiercingDMG = 0;
        if (ricochet == null || ricochet.ListTargets == null || ricochet.ListTargets.Count <= 0)
        {
            //AddDanNhay(2);
            if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.PlayerUnit)
            {
                dmgPlayer = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat().DMG;

                var stat = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat();
                this.CRIT = stat.CRIT;
                this.CRIT_DMG = stat.CRIT_DMG;
            }
            else
            {
                CRIT = 0;
                CRIT_DMG = 0;
            }

            dmg = ConfigManager.GetATKRateRocket() * dmgPlayer / 100;
        }
        
        base.ActiveDan(speed, dmg, target);
    }
}
