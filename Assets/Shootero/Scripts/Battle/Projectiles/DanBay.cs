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


public enum SpawnOnDestroyType
{
    NONE,
    FOUR_RANDOM,
    FOUR_VUONG_GOC,
    FOUR_GOC_X,
    ONE,
    GOLIAH,
    SIX_VUONG_GOC,
    SIX_RANDOM,
    EIGHT_VUONG_GOC,
    EIGHT_RANDOM,
    TWO_CHEO
}

public class DanBay : SatThuongDT
{
    public int ReflectCount = 0;
    public float RangeNo = 0;
    protected int ReflectCounter = 0;

    public int ReflectDMG = 100;
    public int PiercingCount = 0;
    public int PiercingDMG = 100;
    // PhuongTD : add spawn something when proj is destroy
    public GameObject DestroyEff = null;
    public SatThuongDT SpawnOnDestroy = null;
    public SpawnOnDestroyType SpawnType = SpawnOnDestroyType.NONE;
    public float SpawnObjSpeed = 0;

    public GameObject SpawnUnitOnDestroy = null;
    public int SpawnUnitCount = 0;

    public float Speed { get; protected set; }

    protected TweenScale m_TweenScale = null;

    protected int MaskReflect = 0;
    public void ResetReflectCounter()
    {
        ReflectCounter = ReflectCount;
    }

    protected virtual void Awake()
    {
        m_TweenScale = GetComponent<TweenScale>();
        MaskReflect = LayerMask.GetMask("Obstacle", "Default");
    }

    protected virtual void OnDeactiveProjectile()
    {
        if(DestroyEff)
        {
            GameObject des_eff = ObjectPoolManager.SpawnAutoUnSpawn(DestroyEff, 1f);
            des_eff.transform.position = this.transform.position;
            if(RangeNo > 0)
            {
                des_eff.transform.localScale = Vector3.one * RangeNo;
            }
        }

        if(SpawnOnDestroy != null)
        {
            if (SpawnObjSpeed <= 1)
                SpawnObjSpeed = Speed;

            if(SourceSungCls != null && SourceSungCls.weaponCfg != null)
            {
                var buffSplit = SourceSungCls.weaponCfg.GetBuff(BuffType.SPLIT);
                if(buffSplit.Type == BuffType.SPLIT)
                {
                    int SpawnOnDestroyCount = (int)buffSplit.Params[0];

                    float rand_y = 0;
                    float degree_step = 360 / SpawnOnDestroyCount;
                    for (int i = 0; i < SpawnOnDestroyCount; i++)
                    {
                        var projObj = ObjectPoolManager.Spawn(SpawnOnDestroy.gameObject,
                            new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                        var proj = projObj.GetComponent<SatThuongDT>(); // Instantiate(SpawnOnDestroy);
                        proj.transform.eulerAngles = new Vector3(0, rand_y, 0);
                        proj.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                        proj.ActiveDan(SpawnObjSpeed, Damage + Damage * (int)buffSplit.Params[1] / 100, Vector3.one);
                        proj.SourceUnit = SourceUnit;
                        proj.sungCls = sungCls;
                        proj.gameObject.SetActive(true);
                        proj.lastObjCollision = lastObjCollision;
                        ApplyScaleDanTo(projObj);
                        proj.ScaleRate = ScaleRate;
                        rand_y += degree_step;
                    }
                }
            }
            else if (SpawnType == SpawnOnDestroyType.FOUR_RANDOM)
            {
                int SpawnOnDestroyCount = 4;

                float rand_y = Random.Range(0, 360);
                float degree_step = 360 / SpawnOnDestroyCount;
                for (int i = 0; i < SpawnOnDestroyCount; i++)
                {
                    var projObj = ObjectPoolManager.Spawn(SpawnOnDestroy.gameObject, 
                        new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                    var proj = projObj.GetComponent<SatThuongDT>(); // Instantiate(SpawnOnDestroy);
                    proj.transform.eulerAngles = new Vector3(0, rand_y, 0);
                    proj.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    proj.ActiveDan(SpawnObjSpeed, Damage, Vector3.one);
                    proj.SourceUnit = SourceUnit;
                    proj.sungCls = sungCls;
                    proj.gameObject.SetActive(true);
                    proj.lastObjCollision = lastObjCollision;
                    //if (ScaleRate > 0 && ScaleRate != 1f)
                    //{
                    //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale * ScaleRate;
                    //}
                    //else
                    //{
                    //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale;
                    //}
                    ApplyScaleDanTo(projObj);

                    proj.ScaleRate = ScaleRate;
                    rand_y += degree_step;
                }
            }
            else if( SpawnType == SpawnOnDestroyType.FOUR_VUONG_GOC )
            {
                int SpawnOnDestroyCount = 4;

                float rand_y = 0;
                float degree_step = 360 / SpawnOnDestroyCount;
                for (int i = 0; i < SpawnOnDestroyCount; i++)
                {
                    var projObj = ObjectPoolManager.Spawn(SpawnOnDestroy.gameObject, 
                        new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                    var proj = projObj.GetComponent<SatThuongDT>(); // Instantiate(SpawnOnDestroy);
                    proj.transform.eulerAngles = new Vector3(0, rand_y, 0);
                    proj.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    proj.ActiveDan(SpawnObjSpeed, Damage, Vector3.one);
                    proj.SourceUnit = SourceUnit;
                    proj.sungCls = sungCls;
                    proj.gameObject.SetActive(true);
                    proj.lastObjCollision = lastObjCollision;
                    //if (ScaleRate > 0 && ScaleRate != 1f)
                    //{
                    //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale * ScaleRate;
                    //}
                    //else
                    //{
                    //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale;
                    //}
                    ApplyScaleDanTo(projObj);
                    proj.ScaleRate = ScaleRate;
                    rand_y += degree_step;
                }
            }
            else if( SpawnType == SpawnOnDestroyType.FOUR_GOC_X )
            {
                int SpawnOnDestroyCount = 4;

                float rand_y = 45;
                float degree_step = 360 / SpawnOnDestroyCount;
                for (int i = 0; i < SpawnOnDestroyCount; i++)
                {
                    var projObj = ObjectPoolManager.Spawn(SpawnOnDestroy.gameObject, 
                        new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                    var proj = projObj.GetComponent<SatThuongDT>(); // Instantiate(SpawnOnDestroy);
                    proj.transform.eulerAngles = new Vector3(0, rand_y, 0);
                    proj.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    proj.ActiveDan(SpawnObjSpeed, Damage, Vector3.one);
                    proj.SourceUnit = SourceUnit;
                    proj.sungCls = sungCls;
                    proj.gameObject.SetActive(true);
                    proj.lastObjCollision = lastObjCollision;
                    //if (ScaleRate > 0 && ScaleRate != 1f)
                    //{
                    //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale * ScaleRate;
                    //}
                    //else
                    //{
                    //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale;
                    //}
                    ApplyScaleDanTo(projObj);
                    proj.ScaleRate = ScaleRate;
                    rand_y += degree_step;
                }
            }
            else if (SpawnType == SpawnOnDestroyType.ONE)
            {
                var projObj = ObjectPoolManager.Spawn(SpawnOnDestroy.gameObject,
                     new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                var proj = projObj.GetComponent<SatThuongDT>(); // Instantiate(SpawnOnDestroy);
                proj.transform.eulerAngles = new Vector3(0, 0, 0);
                proj.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                proj.ActiveDan(SpawnObjSpeed, Damage, Vector3.one);
                proj.SourceUnit = SourceUnit;
                proj.sungCls = sungCls;
                proj.gameObject.SetActive(true);
                proj.lastObjCollision = lastObjCollision;
                //if (ScaleRate > 0 && ScaleRate != 1f)
                //{
                //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale * ScaleRate;
                //}
                //else
                //{
                //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale;
                //}
                ApplyScaleDanTo(projObj);
                proj.ScaleRate = ScaleRate;
            }
            else if (SpawnType == SpawnOnDestroyType.GOLIAH)
            {
                int SpawnOnDestroyCount = 15;

                float rand_y = Random.Range(0, 360);
                float degree_step = 360 / SpawnOnDestroyCount;
                for (int i = 0; i < SpawnOnDestroyCount; i++)
                {
                    var projObj = ObjectPoolManager.Spawn(SpawnOnDestroy.gameObject,
                        new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
                    var proj = projObj.GetComponent<SatThuongDT>(); // Instantiate(SpawnOnDestroy);
                    proj.transform.eulerAngles = new Vector3(0, rand_y, 0);
                    proj.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
                    proj.ActiveDan(SpawnObjSpeed, Damage, Vector3.one);
                    proj.SourceUnit = SourceUnit;
                    proj.sungCls = sungCls;
                    proj.gameObject.SetActive(true);
                    proj.lastObjCollision = lastObjCollision;
                    //if (ScaleRate > 0 && ScaleRate != 1f)
                    //{
                    //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale * ScaleRate;
                    //}
                    //else
                    //{
                    //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale;
                    //}
                    ApplyScaleDanTo(projObj);
                    proj.ScaleRate = ScaleRate;
                    rand_y += degree_step;
                }
            }
            else if (SpawnType == SpawnOnDestroyType.SIX_VUONG_GOC)
            {
                int SpawnOnDestroyCount = 6;

                float rand_y = 0;
                float degree_step = 360 / SpawnOnDestroyCount;
                for (int i = 0; i < SpawnOnDestroyCount; i++)
                {
                    var projObj = ObjectPoolManager.Spawn(SpawnOnDestroy.gameObject,
                        new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                    var proj = projObj.GetComponent<SatThuongDT>(); // Instantiate(SpawnOnDestroy);
                    proj.transform.eulerAngles = new Vector3(0, rand_y, 0);
                    proj.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    proj.ActiveDan(SpawnObjSpeed, Damage, Vector3.one);
                    proj.SourceUnit = SourceUnit;
                    proj.sungCls = sungCls;
                    proj.gameObject.SetActive(true);
                    proj.lastObjCollision = lastObjCollision;
                    //if (ScaleRate > 0 && ScaleRate != 1f)
                    //{
                    //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale * ScaleRate;
                    //}
                    //else
                    //{
                    //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale;
                    //}
                    ApplyScaleDanTo(projObj);
                    proj.ScaleRate = ScaleRate;
                    rand_y += degree_step;
                }
            }
            else if (SpawnType == SpawnOnDestroyType.SIX_RANDOM)
            {
                int SpawnOnDestroyCount = 6;

                float rand_y = Random.Range(0, 360);
                float degree_step = 360 / SpawnOnDestroyCount;
                for (int i = 0; i < SpawnOnDestroyCount; i++)
                {
                    var projObj = ObjectPoolManager.Spawn(SpawnOnDestroy.gameObject,
                        new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                    var proj = projObj.GetComponent<SatThuongDT>(); // Instantiate(SpawnOnDestroy);
                    proj.transform.eulerAngles = new Vector3(0, rand_y, 0);
                    proj.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    proj.ActiveDan(SpawnObjSpeed, Damage, Vector3.one);
                    proj.SourceUnit = SourceUnit;
                    proj.sungCls = sungCls;
                    proj.gameObject.SetActive(true);
                    proj.lastObjCollision = lastObjCollision;
                    //if (ScaleRate > 0 && ScaleRate != 1f)
                    //{
                    //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale * ScaleRate;
                    //}
                    //else
                    //{
                    //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale;
                    //}
                    ApplyScaleDanTo(projObj);
                    proj.ScaleRate = ScaleRate;
                    rand_y += degree_step;
                }
            }
            else if (SpawnType == SpawnOnDestroyType.EIGHT_VUONG_GOC)
            {
                int SpawnOnDestroyCount = 8;

                float rand_y = 0;
                float degree_step = 360 / SpawnOnDestroyCount;
                for (int i = 0; i < SpawnOnDestroyCount; i++)
                {
                    var projObj = ObjectPoolManager.Spawn(SpawnOnDestroy.gameObject,
                        new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                    var proj = projObj.GetComponent<SatThuongDT>(); // Instantiate(SpawnOnDestroy);
                    proj.transform.eulerAngles = new Vector3(0, rand_y, 0);
                    proj.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    proj.ActiveDan(SpawnObjSpeed, Damage, Vector3.one);
                    proj.SourceUnit = SourceUnit;
                    proj.sungCls = sungCls;
                    proj.gameObject.SetActive(true);
                    proj.lastObjCollision = lastObjCollision;
                    //if (ScaleRate > 0 && ScaleRate != 1f)
                    //{
                    //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale * ScaleRate;
                    //}
                    //else
                    //{
                    //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale;
                    //}
                    ApplyScaleDanTo(projObj);
                    proj.ScaleRate = ScaleRate;
                    rand_y += degree_step;
                }
            }
            else if (SpawnType == SpawnOnDestroyType.EIGHT_RANDOM)
            {
                int SpawnOnDestroyCount = 8;

                float rand_y = Random.Range(0, 360);
                float degree_step = 360 / SpawnOnDestroyCount;
                for (int i = 0; i < SpawnOnDestroyCount; i++)
                {
                    var projObj = ObjectPoolManager.Spawn(SpawnOnDestroy.gameObject,
                        new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                    var proj = projObj.GetComponent<SatThuongDT>(); // Instantiate(SpawnOnDestroy);
                    proj.transform.eulerAngles = new Vector3(0, rand_y, 0);
                    proj.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    proj.ActiveDan(SpawnObjSpeed, Damage, Vector3.one);
                    proj.SourceUnit = SourceUnit;
                    proj.sungCls = sungCls;
                    proj.gameObject.SetActive(true);
                    proj.lastObjCollision = lastObjCollision;
                    //if (ScaleRate > 0 && ScaleRate != 1f)
                    //{
                    //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale * ScaleRate;
                    //}
                    //else
                    //{
                    //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale;
                    //}
                    ApplyScaleDanTo(projObj);
                    proj.ScaleRate = ScaleRate;
                    rand_y += degree_step;
                }
            }
            else if (SpawnType == SpawnOnDestroyType.TWO_CHEO)
            {
                var projObj = ObjectPoolManager.Spawn(SpawnOnDestroy.gameObject,
                    new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                var proj = projObj.GetComponent<SatThuongDT>(); ;
                proj.transform.eulerAngles = this.transform.eulerAngles + new Vector3(0,-30,0);
                proj.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                proj.ActiveDan(SpawnObjSpeed, Damage, Vector3.one);
                proj.SourceUnit = SourceUnit;
                proj.sungCls = sungCls;
                proj.gameObject.SetActive(true);
                ApplyScaleDanTo(projObj);
                proj.ScaleRate = ScaleRate;

                var projObj2 = ObjectPoolManager.Spawn(SpawnOnDestroy.gameObject,
                    new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                var proj2 = projObj2.GetComponent<SatThuongDT>(); ;
                proj2.transform.eulerAngles = this.transform.eulerAngles + new Vector3(0, 30, 0);
                proj2.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                proj2.ActiveDan(SpawnObjSpeed, Damage, Vector3.one);
                proj2.SourceUnit = SourceUnit;
                proj2.gameObject.SetActive(true);
                ApplyScaleDanTo(projObj2);
                proj2.ScaleRate = ScaleRate;
            }

        }

        if(SpawnUnitOnDestroy!=null && QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.IsLevelClear == false)
        {
            for(int i=0;i< SpawnUnitCount;i++)
            {
                var unitObj = GameObject.Instantiate(SpawnUnitOnDestroy);
                unitObj.transform.position = this.transform.position;
                unitObj.SetActive(true);
            }
        }

        ObjectPoolManager.Unspawn(gameObject);
    }

    public void AddDanNhay(int countRicoChet)
    {
        if (countRicoChet > 0)
        {
            if (ricochet == null)
            {
                ricochet = gameObject.AddMissingComponent<DanNhay>();
            }

            var cfg = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.RICOCHET);
            ricochet.Count = (int)cfg.Params[countRicoChet];
            ricochet.NhayRange = cfg.Params[countRicoChet+3];
            ricochet.DMGPercent = (int)cfg.Params[0];
        }
        else if (ricochet)
        {
            ricochet.Count = 0;
        }
    }

    public void AddDanNhay(int countRicoChet, float nhayRange, int dmgPercent)
    {
        if (countRicoChet > 0)
        {
            if (ricochet == null)
            {
                ricochet = gameObject.AddMissingComponent<DanNhay>();
            }

            ricochet.Count = countRicoChet;
            ricochet.NhayRange = nhayRange;
            ricochet.DMGPercent = dmgPercent;
        }
        else if (ricochet)
        {
            ricochet.Count = 0;
        }
    }

    public DauDanTamNhiet GetDauDanTamNhiet()
    {
        return dauDanTamNhiet;
    }

    public void AddDauDanTamNhiet(DonViChienDau target, BuffStat buffStat)
    {
        if (target == null) return;

        //var buffStat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.TRACKING_BULLET);
        if (dauDanTamNhiet == null)
        {
            dauDanTamNhiet = gameObject.AddMissingComponent<DauDanTamNhiet>();
        }
        dauDanTamNhiet.enabled = true;
        dauDanTamNhiet.BanKinhTamNhiet = buffStat.Params[0];
        dauDanTamNhiet.GocGioiHan = buffStat.Params[1];
        dauDanTamNhiet.DoNhay = buffStat.Params[2];

        dauDanTamNhiet.SetTarget(target);
    }

    public void DisableDanTamNhiet()
    {
        if (dauDanTamNhiet && dauDanTamNhiet.enabled)
        {
            dauDanTamNhiet.enabled = false;
        }
    }

    void NhayDanDenTargetMoi(DonViChienDau newTarget, long originDamage)
    {
        var dif = newTarget.transform.position - transform.position;
        dif.y = 0;
        transform.rotation = Quaternion.LookRotation(dif.normalized);

        ricochet.Count--;
        ActiveDan(Speed, originDamage * ricochet.DMGPercent / 100, newTarget.transform.position);
    }
    void ApplyDmgPiercing()
    {
        if (PiercingCount <= 0) return;
        PiercingCount--;
        Damage = Damage * PiercingDMG / 100;
    }

    DanBay CloneDan()
    {
        var newDanObj = Instantiate(gameObject, transform.position, transform.rotation);
        var newDan = newDanObj.GetComponent<DanBay>();
        newDan.listEffect = new List<EffectConfig>(this.listEffect.Count);
        foreach (var eff in this.listEffect)
        {
            newDan.listEffect.Add(eff.Clone());
        }
        newDan.Speed = Speed;
        newDan.Damage = Damage;
        return newDan;
    }
    protected DanNhay ricochet;
    DauDanTamNhiet dauDanTamNhiet;
    protected override bool OnCatchTarget(DonViChienDau unit)
    {
#if PROFILING
        UnityEngine.Profiling.Profiler.BeginSample("OnCatchTarget - DanBay");
#endif
        // chong vien dan sinh ra khi ricochet+ gay damage len chinh unit cu
        if (ricochet && ricochet.ListTargets != null && ricochet.ListTargets.Contains(unit)) return false;

        var re = base.OnCatchTarget(unit);

        if (re)
        {
            if (ricochet)
            {
                if (ricochet.ListTargets == null)
                {
                    ricochet.ListTargets = Hiker.Util.ListPool<DonViChienDau>.Claim();
                }
                ricochet.ListTargets.Add(unit);

                if (ricochet.Count > 0)
                {
                    var newTarget = QuanlyManchoi.FindClosestEnemy(transform.position,
                        ricochet.NhayRange,
                        ricochet.ListTargets);

                    var damTruocKhiNhayTarget = Damage;
                    int ricochetCount = ricochet.Count;
                    if (newTarget != null)
                    {
                        NhayDanDenTargetMoi(newTarget, damTruocKhiNhayTarget);
                        re = false;
                    }

                    if (QuanlyNguoichoi.Instance && this.SourceUnit == QuanlyNguoichoi.Instance.PlayerUnit)
                    {
                        var listRuntimeMods = QuanlyNguoichoi.Instance.GetListRuntimeStatMods();
                        if (listRuntimeMods != null)
                        {
                            var mod = listRuntimeMods.Find(e => e.Stat == EStatType.SKILLPLUS &&
                                e.Target == BuffType.RICOCHET.ToString());
                            if (mod != null)
                            {
                                // nay dan 2
                                var listUnits = Hiker.Util.ListPool<DonViChienDau>.Claim();
                                listUnits.AddRange(ricochet.ListTargets);
                                listUnits.Add(newTarget);

                                var newTarget2 = QuanlyManchoi.FindClosestEnemy(transform.position,
                                    ricochet.NhayRange,
                                    listUnits);
                                if (newTarget2 != null)
                                {
                                    var newDan = this.CloneDan();
                                    //var curPos = newDan.transform.position;
                                    //var unitPos = unit.transform.position;
                                    //newDan.transform.position = new Vector3(unitPos.x, curPos.y, unitPos.z);

                                    newDan.AddDanNhay(ricochetCount);
                                    listUnits.RemoveAt(listUnits.Count - 1);
                                    newDan.ricochet.ListTargets = listUnits;
                                    newDan.NhayDanDenTargetMoi(newTarget2, damTruocKhiNhayTarget);
                                    if (PiercingCount > 0)
                                    {
                                        newDan.ApplyDmgPiercing();
                                    }
                                    newDan.DisableDanTamNhiet();
                                }
                            }
                        }
                    }
                }
            }
            if (PiercingCount > 0)
            {
                ApplyDmgPiercing();
                re = false;
            }

            DisableDanTamNhiet();

            unit.StartVisualImpact(transform.position);
        }
#if PROFILING
        UnityEngine.Profiling.Profiler.EndSample();
#endif
        return re;
    }

    GameObject mLastObstacleReflect = null;

    protected override bool OnHitObstacle(GameObject obstacle)
    {
        DisableDanTamNhiet();

        if (ReflectCounter > 0) // khong disable object neu object co reflect
        {
            return false;
        }
        else // projectile khong xuyen obstacle
        {
            return true;
        }
    }

    protected void DeactiveProj()
    {
        mIsActive = false;
        gameObject.SetActive(false);
        OnDeactiveProjectile();
    }

    protected override void OnDeactiveDan()
    {
        if (RangeNo > 0)
        {
            Collider[] colliders = Hiker.Util.ArrayPool<Collider>.Claim(20);
            var numHit = Physics.OverlapSphereNonAlloc(transform.position, RangeNo, colliders);
            for (int i = 0; i < numHit; ++i)
            {
                if (colliders[i] != null)
                {
                    var target = colliders[i].GetComponent<DonViChienDau>();
                    if (target != null && target.gameObject != lastObjCollision && SourceUnit.TeamID != target.TeamID)
                    {
                        base.OnCatchTarget(target);
                    }
                }
            }
            Hiker.Util.ArrayPool<Collider>.Release(ref colliders);
        }

        DeactiveProj();
    }

    Vector3 lastPosRaycast;
    bool firstRaycast = false;
    float timeRaycast = 0;
    float raycastDistance = 0;
    const float timeCheckRaycast = 0;//1f / 25;
    protected void ResetRaycast()
    {
        lastPosRaycast = transform.position;
        raycastDistance = 0;
        timeRaycast = 0;
        firstRaycast = true;
    }
    protected bool CheckRayCast(float distance, float deltaTime)
    {
        if (ReflectCounter <= 0) return false;
        if (firstRaycast == false)
        {
            ResetRaycast();
        }

        raycastDistance += distance;

        if (timeRaycast < timeCheckRaycast)
        {
            timeRaycast += deltaTime;
            return false;
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(
                    lastPosRaycast,
                    transform.forward,
                    out hit,
                    raycastDistance,
                    MaskReflect,
                    QueryTriggerInteraction.Ignore))
            {
                var outDir = Vector3.Reflect(transform.forward, hit.normal);
                var reflectDis = distance - Vector3.Distance(hit.point, transform.position);
                transform.position = hit.point + outDir * reflectDis;
                transform.rotation = Quaternion.LookRotation(outDir);
                ReflectCounter--;
                Damage = Damage * ReflectDMG / 100;
                if (ricochet != null && ricochet.ListTargets != null) // reset history target after bouncing
                {
                    ricochet.ListTargets.Clear();
                }

                ResetRaycast();

                return true;
            }
            else
            {
                timeRaycast = deltaTime;
                raycastDistance = distance;
                lastPosRaycast = transform.position;

                return false;
            }
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

            float distance = Speed * Time.deltaTime;
            if (ReflectCounter > 0)
            {
                //RaycastHit hit;
                //if (Physics.Raycast(
                //        transform.position,
                //        transform.forward,
                //        out hit,
                //        distance,
                //        MaskReflect,
                //        QueryTriggerInteraction.Ignore))
                //{
                //    var outDir = Vector3.Reflect(transform.forward, hit.normal);
                //    var reflectDis = distance - Vector3.Distance(hit.point, transform.position);
                //    transform.position = hit.point + outDir * reflectDis;
                //    transform.rotation = Quaternion.LookRotation(outDir);
                //    ReflectCounter--;
                //    Damage = Damage * ReflectDMG / 100;
                //}
                if (CheckRayCast(distance, Time.deltaTime))
                {

                }
                else
                {
                    transform.Translate(transform.forward * distance, Space.World);
                }
            }
            else
            {
                transform.Translate(transform.forward * distance, Space.World);
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
        this.Speed = speed;

        this.ResetReflectCounter();

        if (m_TweenScale)
        {
            m_TweenScale.ResetToBeginning();
            m_TweenScale.enabled = true;
        }

        mLastObstacleReflect = null;

        firstRaycast = false;
    }

    public void OverrideSpd(float spd)
    {
        this.Speed = spd;
    }
    public void OverrideDmg(long d)
    {
        this.Damage = d;
    }
}
