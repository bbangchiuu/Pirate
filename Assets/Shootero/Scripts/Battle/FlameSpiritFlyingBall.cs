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

public class FlameSpiritFlyingBall : SatThuongDT
{
    public Int64 DMG;

    public AudioSource BomSound;

    void Init()
    {
        //var chapterCfg = PlayerManager.Instance.currentChap;
        //var segmentIdx = PlayerManager.Instance.SegmentIndex;
        //DMG = chapterCfg.GetDMG(segmentIdx);
        if (QuanlyNguoichoi.Instance != null)
        {
            DMG = QuanlyNguoichoi.Instance.GetDMGEnemy();
        }
        else
        {
            DMG = 100;
        }
    }

    private void Start()
    {
        Init();
        ActiveDan(0, DMG, Vector3.zero);
    }

    protected override bool OnHitObstacle(GameObject obstacle)
    {
        return false;
        //// Dont destroy when player fly
        //if (obstacle.gameObject.layer == LayerMask.NameToLayer("Team1") /*||
        //    obstacle.gameObject.layer == LayerMask.NameToLayer("Team1_Flying")*/)
        //{ // deactive when hit team1 object
        //    return true;
        //}
        //return false; // mac dinh damage object xuyen obstacle
    }

    protected override void Update()
    {
        if (QuanlyNguoichoi.Instance.IsEndGame)
        {
            GameObject.Destroy(this.gameObject);
            return;
        }

        base.Update();
        mIsActive = true;
    }

    protected override bool OnCollisionOther(GameObject other)
    {
        base.OnCollisionOther(other);
        
        //if (base.OnCollisionOther(other))
        //{
        //    gameObject.SetActive(false);
        //    Destroy(gameObject, 0.5f);

        //    GameObject bom_eff = ObjectPoolManager.SpawnAutoDestroy("Particle/Other/MineImpact", 2);
        //    bom_eff.transform.position = this.transform.position;

        //    return true;
        //}

        return false;
    }
}
