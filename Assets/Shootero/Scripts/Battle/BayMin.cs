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

public class BayMin : SatThuongDT
{
    [Tooltip("Khong su dung")]
    public long DMG;
    public long SatThuongFake { get; set; }
    public Int64 SatThuong = 100;
    public AudioSource BomSound;

    void Init()
    {
        //var chapterCfg = PlayerManager.Instance.currentChap;
        //var segmentIdx = PlayerManager.Instance.SegmentIndex;
        //DMG = chapterCfg.GetDMG(segmentIdx);
        if (QuanlyNguoichoi.Instance != null)
        {
            SatThuong = QuanlyNguoichoi.Instance.GetDMGEnemy();
        }
        else
        {
            SatThuong = 100;
        }
    }

    private void Start()
    {
        Init();
        ActiveDan(0, SatThuong, Vector3.zero);
    }

    protected override bool OnHitObstacle(GameObject obstacle)
    {
        // Dont destroy when player fly
        var l = obstacle.gameObject.layer;
        if (l == LayersMan.Team1 || l == LayersMan.Team1_FlyingWater)
        { // deactive when hit team1 object
            return true;
        }
        return false; // mac dinh damage object xuyen obstacle
    }

    protected override bool OnCollisionOther(GameObject other)
    {
        if (other.layer == LayersMan.Team1_Flying)
            return false;

        if (base.OnCollisionOther(other))
        {
            gameObject.SetActive(false);
            Destroy(gameObject, 0.5f);

            GameObject bom_eff = ObjectPoolManager.SpawnAutoDestroy("Particle/Other/MineImpact", 2);
            bom_eff.transform.position = this.transform.position;

            return true;
        }

        return false;
    }
}
