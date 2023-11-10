using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hiker.GUI.Shootero;

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

public class CaptainKhien : DanBay
{
    /// <summary>
    /// = 1 -> dan bay ra
    /// = 0 -> dan bay ve
    /// </summary>
    int phase = 1;

    protected float MaxRange = 15;
    protected Vector3 starPos;
    bool IsForwardPhase()
    {
        return phase == 1;
    }

    protected bool SatThuongDT_OnCatchTarget(DonViChienDau unit)
    {
        if (unit.IsAlive() == false) return false;

        bool reflect = false;
        if (unit.GetStat().EVASION > 0 && unit.IsInvulnerable == false) // chi tinh ne khi unit khong duoc bat bat tu
        {
            reflect = Random.Range(0, 100) < unit.GetStat().EVASION;
            OnReflectDMG(unit);
        }

        if (reflect == false)
        {
            bool isHeadShot = Random.Range(0, 100) < HEADSHOT;
            if (unit.IsBoss)
            {
                isHeadShot = false;
            }

            IsHS = isHeadShot;
            if (isHeadShot)
            {
                var takenDmg = unit.TakeDamage(
                    unit.GetMaxHP(), // dmg
                    true, // display crit
                    SourceUnit,
                    true, // play hit sound
                    false, // show hud
                    false, // xuyen bat tu
                    false, // xuyen linken
                    false, // thong ke
                    true  // isHeadshot
                    );

                PostDmg = takenDmg;

                if (takenDmg > 0)
                {
                    unit.OnPostDinhDan(takenDmg);
                }

                ScreenBattle.instance.DisplayTextHud(Localization.Get("HeadShotLabel"), unit);
            }
            else
            {
                bool isCrit = Random.Range(0, 100) < CRIT;
                var dmg = isCrit ? Damage * CRIT_DMG / 100 : Damage;

                bool isDanCuaNguoiChoi = false;
                if (SourceUnit != null && SourceUnit == QuanlyNguoichoi.Instance.PlayerUnit && this is DanBay)
                    isDanCuaNguoiChoi = true;
                var takenDmg = unit.TakeDamage(dmg,
                    isCrit,
                    SourceUnit,
                    true, // play hit sound
                    true, // show hud
                    false, // xuyen bat tu
                    false, // xuyen linken
                    cothongke, // thong ke
                    false,  // isHeadshot
                    false, // sourceUnitIsBoss
                    false, // sourceUnitIsAir
                    false, // skipSourceUnitType
                    isDanCuaNguoiChoi
                    );
                IsCrit = isCrit;
                PostDmg = takenDmg;
                if (takenDmg > 0)
                {
                    unit.OnPostDinhDan(takenDmg);
                }
                if (KnockBackDistance > 0)
                {
                    var vec = transform.forward;
                    vec.y = 0;
                    unit.KnockBack(vec.normalized * KnockBackDistance);
                    KnockBackDistance = 0;
                }

                foreach (var eff in listEffect)
                {
                    unit.GetStatusEff().ApplyEffect(new BattleEffect(eff));
                }
            }
        }
        else
        {
#if DEBUG
            //Debug.Log("EVASION");
#endif
            if (unit == QuanlyNguoichoi.Instance.PlayerUnit)
            {
                QuanlyNguoichoi.Instance.AddThongKeST("EVADE", 0,
                    QuanlyNguoichoi.Instance.PlayerUnit.GetCurHP(),
                    QuanlyNguoichoi.Instance.PlayerUnit.GetCurHP(),
                    QuanlyNguoichoi.Instance.PlayerUnit.GetMaxHP());
            }
            unit.OnEvadeBullet();
            return false;
        }

        return true;
    }

    protected override bool OnCatchTarget(DonViChienDau unit)
    {
        var re = SatThuongDT_OnCatchTarget(unit);

        if (re)
        {
            //if (ricochet)
            //{
            //    if (ricochet.ListTargets == null)
            //    {
            //        ricochet.ListTargets = Hiker.Util.ListPool<DonViChienDau>.Claim();
            //    }
            //    ricochet.ListTargets.Add(unit);

            //    if (ricochet.Count > 0)
            //    {
            //        var newTarget = QuanlyManchoi.FindClosestEnemy(transform.position,
            //            ricochet.NhayRange,
            //            ricochet.ListTargets);

            //        var damTruocKhiNhayTarget = Damage;
            //        int ricochetCount = ricochet.Count;
            //        if (newTarget != null)
            //        {
            //            NhayDanDenTargetMoi(newTarget, damTruocKhiNhayTarget);
            //            re = false;
            //        }

            //        if (QuanlyNguoichoi.Instance && this.SourceUnit == QuanlyNguoichoi.Instance.PlayerUnit)
            //        {
            //            var listRuntimeMods = QuanlyNguoichoi.Instance.GetListRuntimeStatMods();
            //            if (listRuntimeMods != null)
            //            {
            //                var mod = listRuntimeMods.Find(e => e.Stat == EStatType.SKILLPLUS &&
            //                    e.Target == BuffType.RICOCHET.ToString());
            //                if (mod != null)
            //                {
            //                    // nay dan 2
            //                    var listUnits = Hiker.Util.ListPool<DonViChienDau>.Claim();
            //                    listUnits.AddRange(ricochet.ListTargets);
            //                    listUnits.Add(newTarget);

            //                    var newTarget2 = QuanlyManchoi.FindClosestEnemy(transform.position,
            //                        ricochet.NhayRange,
            //                        listUnits);
            //                    if (newTarget2 != null)
            //                    {
            //                        var newDan = this.CloneDan();
            //                        //var curPos = newDan.transform.position;
            //                        //var unitPos = unit.transform.position;
            //                        //newDan.transform.position = new Vector3(unitPos.x, curPos.y, unitPos.z);

            //                        newDan.AddDanNhay(ricochetCount);
            //                        listUnits.RemoveAt(listUnits.Count - 1);
            //                        newDan.ricochet.ListTargets = listUnits;
            //                        newDan.NhayDanDenTargetMoi(newTarget2, damTruocKhiNhayTarget);
            //                        if (PiercingCount > 0)
            //                        {
            //                            newDan.ApplyDmgPiercing();
            //                        }
            //                        newDan.DisableDanTamNhiet();
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            if (PiercingCount > 0)
            {
                //ApplyDmgPiercing();
                re = false;
            }

            DisableDanTamNhiet();

            unit.StartVisualImpact(transform.position);
        }

        return false;
    }

    protected override bool OnHitObstacle(GameObject obstacle)
    {
        if (IsForwardPhase())
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

    protected override void OnDeactiveProjectile()
    {
        if (SourceUnit && SourceUnit.IsAlive() && SourceUnit.SungPhu)
        {
#if DEBUG
            Debug.Log("Enable sung phu");
#endif
            SourceUnit.SungPhu.IsActive = true;
        }
        base.OnDeactiveProjectile();
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
            }

            RaycastHit hit;
            if (Physics.Raycast(
                    transform.position,
                    transform.forward,
                    out hit,
                    distance,
                    MaskReflect) || Vector3.Distance(transform.position, starPos) > MaxRange)
            {
                //var outDir = Vector3.Reflect(transform.forward, hit.normal);
                //var reflectDis = distance - Vector3.Distance(hit.point, transform.position);
                //transform.position = hit.point + outDir * reflectDis;
                //transform.rotation = Quaternion.LookRotation(outDir);
                //ReflectCounter--;
                //Damage = Damage * ReflectDMG / 100;
                if (IsForwardPhase())
                {
                    OnChangePhase();
                }
                else
                {
                    transform.position = nextPos;
                }
            }
            else
            {
                //transform.Translate(transform.forward * distance, Space.World);
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
        phase = 1;
        if(sungCls != null && sungCls.weaponCfg != null)
        {
            MaxRange = sungCls.weaponCfg.ATK_RANGE;
        }
        else
        {
            MaxRange = 15;
        }

        if (SourceUnit && SourceUnit.SungPhu && SourceUnit.SungPhu.IsActive)
        {
            SourceUnit.SungPhu.IsActive = false;
#if DEBUG
            Debug.Log("Disable sung phu");
#endif
            var sk = SourceUnit.GetKyNang().GetTuyetKy(0) as CaptainSkill;
            if (sk != null)
            {
                sk.DeactiveKhien();
            }
        }
    }
}
