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

public class BayChong : SatThuongDT
{
    public Int64 DMG;

    void Init()
    {
        DMG = QuanlyNguoichoi.Instance.GetDMGEnemy();
    }

    private void Start()
    {
        Init();
        ActiveDan(0, DMG, Vector3.zero);
    }

    protected override bool OnCollisionOther(GameObject other)
    {
        if (base.OnCollisionOther(other))
        {
            gameObject.SetActive(false);
            //Destroy(gameObject, 0.5f);

            return true;
        }

        return false;
    }
}
