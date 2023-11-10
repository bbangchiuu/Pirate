using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ANTICHEAT
using Int64 = CodeStage.AntiCheat.ObscuredTypes.ObscuredLong;
using Int32 = CodeStage.AntiCheat.ObscuredTypes.ObscuredInt;
using Float = CodeStage.AntiCheat.ObscuredTypes.ObscuredFloat;
using Bool = CodeStage.AntiCheat.ObscuredTypes.ObscuredBool;
#else
using Int64 = System.Int64;
using Int32 = System.Int32;
using Float = System.Single;
using Bool = System.Boolean;
#endif

public class BayLua : SatThuongDT
{
    protected override bool OnHitObstacle(GameObject obstacle)
    {
        return false;
    }

    protected override bool OnCollisionOther(GameObject other)
    {
        // PhuongTD : dont dmg player when flying
        if (other.layer == LayersMan.Team1_Flying)
            return false;

        var unit = other.GetComponentInParent<DonViChienDau>();
        if (unit != null)
        {
            if (unit.IsAlive() == false) return false;

            var re = OnCatchTarget(unit);
            if (re)
            {
                OnDeactiveDan();
            }
            return re;
        }
        else
        {
            if (OnHitObstacle(other))
            {
                OnDeactiveDan();
                return true;
            }
        }

        return false;
    }
}
