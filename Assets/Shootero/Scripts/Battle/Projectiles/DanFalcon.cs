using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanFalcon : DanBay
{
    public override void ActiveDan(float speed, long dmg, Vector3 target)
    {
        long dmgPlayer = 100;

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

        dmg = ConfigManager.GetATKRateFalcon() * dmgPlayer / 100;
        base.ActiveDan(speed, dmg, target);
    }
}
