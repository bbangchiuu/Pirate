using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongChimShaman : DanBay
{
    public bool ForwardMoving = true;
    public float IdleTime = 2;
    float countTimeIdle;

    /// <summary>
    /// = 2 -> trang thai cho
    /// = 1 -> dan bay ra
    /// = 0 -> dan bay ve
    /// </summary>
    int phase = 1;

    public bool IsForwardPhase()
    {
        return phase == 1;
    }

    public bool IsIdlePhase()
    {
        return phase == 2;
    }

    public bool IsBackPhase()
    {
        return phase == 0;
    }

    //protected override bool OnCatchTarget(DonViChienDau unit)
    //{
    //    var re = base.OnCatchTarget(unit);
    //    if (IsForwardPhase())
    //        OnChangePhase();
    //    return false;
    //}
    static int Team1Layer = 0;

    protected override bool OnHitObstacle(GameObject obstacle)
    {
        if (Team1Layer == 0)
        {
            Team1Layer = LayersMan.Team1HitLayerMask;
        }
        if (IsForwardPhase())
        {
            OnChangePhase();
        }
        else if (IsBackPhase())
        {
            if (obstacle.layer == Team1Layer)
            {
                return true;
            }
        }

        return false;
    }

    void OnChangePhase()
    {
        if (phase == 1)
        {
            phase = 2;
            transform.SetParent(null, true);
        }
        else if (phase == 2)
        {
            phase = 0;
        }

        if (IsBackPhase())
        {
            if (SourceUnit == null || SourceUnit.IsAlive() == false)
            {
                DeactiveProj();
                return;
            }

            transform.SetParent(null, true);

            var srcPos = SourceUnit.transform.position;

            var dif = srcPos - transform.position;
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

            if (IsBackPhase())
            {
                var srcPos = SourceUnit.transform.position;
                var dif = srcPos - transform.position;
                if (dif.sqrMagnitude < 0.25f)
                {
                    DeactiveProj();
                    return;
                }

                if (Vector3.Dot(dif, srcPos - nextPos) < 0)
                {
                    DeactiveProj();
                    return;
                }

                transform.forward = dif.normalized;
                transform.position = nextPos;
            }
            else
            if (IsForwardPhase())
            {
                //RaycastHit hit;
                //if (Physics.Raycast(
                //        transform.position,
                //        transform.forward,
                //        out hit,
                //        distance,
                //        MaskReflect))
                //{
                //    OnChangePhase();
                //}
                //else
                {
                    if (ForwardMoving)
                    {
                        transform.position = nextPos;
                    }
                }
            }
            else
            if (IsIdlePhase())
            {
                countTimeIdle += Time.deltaTime;
                if (countTimeIdle >= IdleTime)
                {
                    OnChangePhase();
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
    }

    public override void ActiveDan(float speed, long dmg, Vector3 target)
    {
        base.ActiveDan(speed, dmg, target);
        phase = 1;
        countTimeIdle = 0;
    }
}
