using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanNecromancerSkeleton : DanBay
{
    public override void ActiveDan(float speed, long dmg, Vector3 target)
    {
        long dmgPlayer = 100;

        if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.PlayerUnit)
        {
            dmgPlayer = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat().DMG;
            this.CRIT = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat().CRIT;
            this.CRIT_DMG = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat().CRIT_DMG;
        }

        dmg = ConfigManager.GetHeroSkillParams("NecromancerSkill", 4) * dmgPlayer / 100;
        base.ActiveDan(speed, dmg, target);
    }
}
