using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanStick : DanBay
{
    protected float MaxRange = 15;
    protected Vector3 starPos;

    protected override bool OnCatchTarget(DonViChienDau unit)
    {
        base.OnCatchTarget(unit);
        return false;
    }
    protected override void Update()
    {
        if (mIsActive)
        {
            if (QuanlyNguoichoi.Instance.IsLoadingMission)
            {
                DeactiveProj();
                return;
            }

            if (SourceUnit == null || SourceUnit.IsAlive() == false)
            {
                DeactiveProj();
                return;
            }

            float distance = Speed * Time.deltaTime;
            var nextPos = transform.position + transform.forward * distance;

            
            if (Vector3.Distance(transform.position, starPos) > MaxRange)
            {
                DeactiveProj();
            }
            else
            {
                transform.position = nextPos;
            }
        }

        if (LifeTime > 0)
        {
            LifeTime -= Time.deltaTime;
        }
        else
        {
            DeactiveProj();
        }
    }

    public override void ActiveDan(float speed, long dmg, Vector3 target)
    {
        base.ActiveDan(speed, dmg, target);
        PiercingCount = 1000;
        PiercingDMG = 100;
        ReflectCount = 1;
        ReflectDMG = 100;
        starPos = transform.position;
        if (sungCls != null && sungCls.weaponCfg != null)
        {
            MaxRange = sungCls.weaponCfg.ATK_RANGE;
        }
        else
        {
            MaxRange = 15;
        }
    }
}
