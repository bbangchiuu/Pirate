using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaCaRongModifier : MonoBehaviour
{
    DonViChienDau unit;

    private void Awake()
    {
        unit = GetComponent<DonViChienDau>();
    }

    public static long GetCursedDmgByDracula(long curHP, long maxHP)
    {
        int tile = Mathf.Max(ConfigManager.LeoThapCfg.HeSoMauDracula, 0);
        long mauHut = maxHP * tile / 100;
        long mauConLai = maxHP - mauHut;
        
        if (mauConLai < 1)
        {
            mauConLai = 1;
        }

        if (curHP > mauConLai)
        {
            return curHP - mauConLai;
        }
        return 0;
    }

    public void MaCaRongCurse()
    {
        if (unit != null)
        {
            var dmg = GetCursedDmgByDracula(unit.GetCurHP(), unit.GetMaxHP());
            unit.TakeDamage(dmg,
                    false, // display crit
                    null, // source unit
                    false, // playHitSound
                    false, // show HUD
                    true, // xuyen bat tu
                    true, // xuyen linken
                    false, // thongKe
                    false, // headShot
                    false,
                    false,
                    true
                    );
        }
    }
}
