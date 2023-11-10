using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KiemKhi : DanBay
{
    DonViChienDau unitTarget;

    protected override void OnEnable()
    {
        mIsActive = false;
        LifeTime = 1000;
        LifeTimeDuration = 1000;
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
            float distance = Speed * Time.deltaTime;

            if (unitTarget != null && unitTarget.IsAlive() && unitTarget.IsCanTarget())
            {
                var f = unitTarget.transform.position - transform.position;
                if (f.sqrMagnitude > 0.01f)
                {
                    transform.rotation = Quaternion.LookRotation(f.normalized);
                }
            }
            else
            {
                unitTarget = QuanlyManchoi.FindClosestEnemy(transform.position, 10000);
                if (unitTarget != null)
                {
                    var f = unitTarget.transform.position - transform.position;
                    if (f.sqrMagnitude > 0.01f)
                    {
                        transform.rotation = Quaternion.LookRotation(f.normalized);
                    }
                }
                else
                {
                    transform.rotation = Quaternion.LookRotation(Vector3.forward);
                }
            }

            transform.Translate(transform.forward * distance, Space.World);
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
        transform.SetParent(null);
        unitTarget = QuanlyManchoi.FindClosestEnemy(transform.position, 10000);
        cothongke = false;
    }

    protected override bool OnHitObstacle(GameObject obstacle)
    {
        return false; // xuyen obstacle
    }

    protected override bool OnCatchTarget(DonViChienDau unit)
    {
        var re = base.OnCatchTarget(unit);
        if (re)
        {
            var unitDMG = SourceUnit.GetCurStat().DMG;
            var curDmg = Damage;

            while (curDmg >= unitDMG)
            {
                QuanlyNguoichoi.Instance.AddTKSTNguoiChoi(unitDMG);
                curDmg -= unitDMG;
            }

            if (curDmg > 0)
            {
                QuanlyNguoichoi.Instance.AddTKSTNguoiChoi(unitDMG);
            }
        }

        return re;
    }
}
