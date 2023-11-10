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
public class SongAm : DanBay
{
    //public Float ScaleRate = 0.5f;
    //float mScale = 0.1f;
    protected override void Awake()
    {
        base.Awake();
    }
    private void OnEnable()
    {
        //mScale = 0.1f;
        //transform.localScale = Vector3.one * mScale;
    }
    protected override void Update()
    {
        base.Update();
        //mScale += ScaleRate * Time.deltaTime;
        //transform.localScale = Vector3.one * mScale;
    }
    protected override bool OnHitObstacle(GameObject obstacle)
    {
        //return base.OnHitObstacle(obstacle);
        if (LayersMan.CheckMask(obstacle.layer, LayersMan.Team1HitLayerMask))
        { // deactive when hit team1 object
            return true;
        }
        return false; // mac dinh damage object xuyen obstacle
    }
}
