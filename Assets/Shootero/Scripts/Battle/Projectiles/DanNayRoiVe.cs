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

public class DanNayRoiVe : DanBay
{
    /// <summary>
    /// = 1 -> dan bay ra
    /// = 0 -> dan bay ve
    /// </summary>
    int phase = 1;

    bool IsForwardPhase()
    {
        return phase == 1;
    }

    protected override bool OnCatchTarget(DonViChienDau unit)
    {
        var re = base.OnCatchTarget(unit);
        if (IsForwardPhase())
            OnChangePhase();

        return false;
    }

    protected override bool OnHitObstacle(GameObject obstacle)
    {
        if (IsForwardPhase() && ReflectCounter == 0)
            OnChangePhase();
        return false;
    }

    void OnChangePhase()
    {
        phase = 1 - phase;

        if (IsForwardPhase() == false)
        {
            if (SourceUnit == null || SourceUnit.IsAlive() == false)
            {
                DeactiveProj();
                return;
            }

            var srcPos = SourceUnit.transform.position;

            var dif = srcPos - transform.position;
            dif.y = 0;
            if (dif.sqrMagnitude < 0.25f)
            {
                DeactiveProj();
                return;
            }

            transform.forward = dif.normalized;
        }
    }

    // Update is called once per frame
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

            if (IsForwardPhase() == false)
            {
                var srcPos = SourceUnit.transform.position;
                var dif = srcPos - transform.position;
                dif.y = 0;
                if (dif.sqrMagnitude < 0.25f)
                {
                    DeactiveProj();
                    return;
                }

                var d = srcPos - nextPos;
                d.y = 0;
                if (Vector3.Dot(dif, d) < 0)
                {
                    DeactiveProj();
                    return;
                }

                transform.forward = dif.normalized;
                transform.position = nextPos;
            }
            else
            {
                RaycastHit hit;
                if (Physics.Raycast(
                        transform.position,
                        transform.forward,
                        out hit,
                        distance,
                        MaskReflect,
                        QueryTriggerInteraction.Ignore))
                {
                    if (ReflectCounter > 0)
                    {
                        var outDir = Vector3.Reflect(transform.forward, hit.normal);
                        var reflectDis = distance - Vector3.Distance(hit.point, transform.position);
                        transform.position = hit.point + outDir * reflectDis;
                        transform.rotation = Quaternion.LookRotation(outDir);
                        ReflectCounter--;
                        Damage = Damage * ReflectDMG / 100;
                        if (ReflectCounter == 0)
                        {
                            OnChangePhase();
                        }
                    }
                    else 
                    {
                        OnChangePhase();
                    }
                }
                else
                {
                    transform.position = nextPos;
                }
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
        phase = 1;
    }
}
