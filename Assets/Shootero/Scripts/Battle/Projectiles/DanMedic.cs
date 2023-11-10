using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanMedic : DanBay
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
            
            if (QuanlyNguoichoi.Instance.PlayerUnit.IsAlive())
            {
                long regen = QuanlyNguoichoi.Instance.PlayerUnit.GetMaxHP() * ConfigManager.GetHeroSkillParams("IronManSkill", 0) / 100;
                QuanlyNguoichoi.Instance.PlayerUnit.RegenHP(regen, true, "+Medic");
            }
        }
        else
        {
            CRIT = 0;
            CRIT_DMG = 0;
        }

        dmg = ConfigManager.GetATKRateMedic() * dmgPlayer / 100;
        base.ActiveDan(speed, dmg, target);
    }
}
