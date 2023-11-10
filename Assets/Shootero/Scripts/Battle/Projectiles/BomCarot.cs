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

public class BomCarot : DanTenLua
{
    public Float TimeDelayExplode = 4;
    bool hitObstacle = false;

    public TweenScale warningTween;

    protected override void Update()
    {
        if (QuanlyNguoichoi.Instance.IsLoadingMission)
        {
            DeactiveProj();
            return;
        }

        if (hitObstacle == false)
        {
            base.Update();
            return;
        }
        else
        {
            if (LifeTime > 0)
            {
                LifeTime -= Time.deltaTime;

                if(LifeTime < 2 && warningTween.enabled==false)
                {
                    warningTween.enabled = true;
                }
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

        targetPosition = this.transform.position;
        this.Target = this.transform.position;

        LifeTimeDuration = 1000f;
        LifeTime = LifeTimeDuration;
        hitObstacle = false;

        warningTween.enabled = false;
    }

    protected override bool OnHitObstacle(GameObject obstacle)
    {
        base.OnHitObstacle(obstacle);
        LifeTime = TimeDelayExplode;
        hitObstacle = true;
        return false;
    }

    protected override void OnReachTarget()
    {
        LifeTime = TimeDelayExplode;
        hitObstacle = true;
        if (mIndicator)
        {
            mIndicator.gameObject.SetActive(false);
        }
    }
}
