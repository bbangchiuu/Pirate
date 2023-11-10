using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChimBMST : SatThuongDT
{
    public override void ActiveDan(float speed, long dmg, Vector3 target)
    {
        base.ActiveDan(speed, dmg, target);
        if (SourceUnit && SourceUnit.LastTarget)
        {
            var unitTarget = SourceUnit.LastTarget.GetComponent<DonViChienDau>();
            if (unitTarget && unitTarget.IsAlive())
            {
                bool isCrit = Random.Range(0, 100) < CRIT;
                var satThuong = isCrit ? dmg * CRIT_DMG / 100 : dmg;
                unitTarget.TakeDamage(satThuong,
                    isCrit,
                    SourceUnit,
                    true, // play hit sound
                    true, // show hud
                    false, // xuyen bat tu
                    false, // xuyen linken
                    cothongke, // thong ke
                    false  // isHeadshot
                    );
            }
        }

        DeactiveDan();
    }
}
