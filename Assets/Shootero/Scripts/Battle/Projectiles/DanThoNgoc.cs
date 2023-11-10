using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanThoNgoc : DanBay
{
    public override void ActiveDan(float speed, long dmg, Vector3 target)
    {
        long dmgPlayer = 100;

        if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.PlayerUnit)
        {
            dmgPlayer = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat().DMG;
        }

        dmg = ConfigManager.GetATKRateThoNgoc() * dmgPlayer / 100;
        base.ActiveDan(speed, dmg, target);
    }
}
