using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GayThietBang : SatThuongDT
{
    public GameObject root;
    protected override void OnEnable()
    {
        var rot = root.GetComponentInChildren<TweenRotation>();
        if (rot != null)
        {
            rot.ResetToBeginning();
            rot.PlayForward();
        }
    }
    protected override bool OnCatchTarget(DonViChienDau unit)
    {
        base.OnCatchTarget(unit);
        return false;
    }

    protected override bool OnHitObstacle(GameObject obstacle)
    {
        return false;
    }

    void CheckDamageEff()
    {
        var proj = this;
        var unit = SourceUnit;
        proj.listEffect.Clear();

        int eleCritBuffCount = 0;
        if (unit == QuanlyNguoichoi.Instance.PlayerUnit)
        {
            eleCritBuffCount = QuanlyNguoichoi.Instance.PlayerUnit.GetBuffCount(BuffType.ELEMENT_CRIT);
        }

        var electTricCount = unit.GetBuffCount(BuffType.ELECTRIC_EFF);
        if (electTricCount > 0)
        {
            var buffstat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.ELECTRIC_EFF);
            var randomRate = buffstat.Params[0];
            if (randomRate > 0 && Random.Range(0, 100) < randomRate)
            {
                var dmg = Mathf.CeilToInt(unit.GetCurDPS() * unit.GetElementDMGRate() * buffstat.Params[1] / 100);
                var battleEff = new EffectConfig
                {
                    Type = EffectType.Electric,
                    Damage = dmg,
                    Param1 = buffstat.Params[2], // count unit get
                    Param2 = buffstat.Params[3], // range unit lan
                    Duration = 0.6f,
                };
                
                if (eleCritBuffCount > 0)
                {
                    UnitStat stats = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat();
                    battleEff.Crit = stats.CRIT;
                    battleEff.CritDMG = stats.CRIT_DMG;
                    battleEff.isEleCrit = true;
                }

                proj.listEffect.Add(battleEff);
            }
        }

        var flameCount = unit.GetBuffCount(BuffType.FLAME_EFF);
        if (flameCount > 0)
        {
            var buffstat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.FLAME_EFF);
            var battleEff = new EffectConfig
            {
                Type = EffectType.Flame,
                Damage = Mathf.CeilToInt(unit.GetCurStat().DMG * unit.GetElementDMGRate() * buffstat.Params[0] / 100),
                Param1 = buffstat.Params[1], // tick dot
                Duration = buffstat.Params[2],
            };

            if (eleCritBuffCount > 0)
            {
                UnitStat stats = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat();
                battleEff.Crit = stats.CRIT;
                battleEff.CritDMG = stats.CRIT_DMG;
                battleEff.isEleCrit = true;
            }

            proj.listEffect.Add(battleEff);
        }

        var frozenCount = unit.GetBuffCount(BuffType.FROZEN_EFF);
        if (frozenCount > 0)
        {
            var buffstat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.FROZEN_EFF);
            var battleEff = new EffectConfig
            {
                Type = EffectType.Frozen,
                Param1 = buffstat.Params[0], // slow percent
                Duration = buffstat.Params[1],
            };

            if (eleCritBuffCount > 0)
            {
                UnitStat stats = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat();
                battleEff.Crit = stats.CRIT;
                battleEff.CritDMG = stats.CRIT_DMG;
                battleEff.isEleCrit = true;

                battleEff.Damage = Mathf.CeilToInt(unit.GetCurStat().DMG * unit.GetElementDMGRate() * buffstat.Params[2] / 100);
            }

            proj.listEffect.Add(battleEff);
        }

        if (proj.listEffect.Count == 0 && unit.GetKyNang())
        {
            var tk = unit.GetKyNang().GetTuyetKy(0);
            if (tk != null)
            {
                var tkEff = tk.GetEffectFromSkill();
                if (tkEff != null)
                {
                    proj.listEffect.Add(tkEff.Clone());
                }
            }
        }

        //if (proj is DanBay)
        //{
        //    var trackingBullet = unit.GetBuffCount(BuffType.TRACKING_BULLET);

        //    var danTamNhiet = proj.gameObject.GetComponent<DauDanTamNhiet>();
        //    if (trackingBullet > 0)
        //    {
        //        var buffStat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.TRACKING_BULLET);
        //        if (danTamNhiet == null)
        //        {
        //            danTamNhiet = proj.gameObject.AddComponent<DauDanTamNhiet>();
        //        }
        //        danTamNhiet.enabled = true;
        //        danTamNhiet.BanKinhTamNhiet = buffStat.Params[0];
        //        danTamNhiet.GocGioiHan = buffStat.Params[1];
        //        danTamNhiet.DoNhay = buffStat.Params[2];

        //        if (unit == QuanlyNguoichoi.Instance.PlayerUnit)
        //        {
        //            if (this == unit.shooter) // is main shooter
        //            {
        //                if (unit.LastTarget != null)
        //                {
        //                    var targetUnit = unit.LastTarget.gameObject.GetComponent<DonViChienDau>();
        //                    if (targetUnit)
        //                    {
        //                        danTamNhiet.SetTarget(targetUnit);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (danTamNhiet != null)
        //        {
        //            danTamNhiet.enabled = false;
        //        }
        //    }
        //}
    }

    public override void ActiveDan(float speed, long dmg, Vector3 target)
    {
        SourceUnit = QuanlyNguoichoi.Instance.PlayerUnit;
        var unit = SourceUnit;
        var crit = unit.GetStat().CRIT;
        var critDmg = unit.GetStat().CRIT_DMG;
        CRIT = crit;
        CRIT_DMG = critDmg;
        HEADSHOT = 0;
        int headShotCount = unit.GetBuffCount(BuffType.HEAD_SHOT);
        if (headShotCount > 0)
        {
            var buffStat = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.HEAD_SHOT);
            HEADSHOT = (int)buffStat.Params[headShotCount - 1];
        }
        CheckDamageEff();
        base.ActiveDan(speed, dmg, target);
    }

    protected override void Update()
    {
        base.Update();
        if (mIsActive)
        {
            LifeTime -= Time.deltaTime;
            if (LifeTime <= 0)
            {
                OnDeactiveDan();
            }
        }
        else
        {
            LifeTime -= Time.deltaTime;
            if (LifeTime <= -0.5f)
            {
                var rot = root.GetComponentInChildren<TweenRotation>();
                if(rot != null)
                {
                    rot.ResetToBeginning();
                    rot.enabled = false;
                }
                ObjectPoolManager.Unspawn(root);
            }
        }
    }

    protected override void OnDeactiveDan()
    {
        base.OnDeactiveDan();
        //var rot = root.GetComponentInChildren<TweenRotation>();
        //rot.ResetToBeginning();
        //rot.enabled = false;
        
        //ObjectPoolManager.Unspawn(root);
    }
}
