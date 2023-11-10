using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class CaptainSkill : TuyetKy
{
    float timeActivate;

    private PlayerVisual playerVisual;

    private Transform khienBone;

    public override void OnStart()
    {
        base.OnStart();
        Loai = LoaiTK.BiDong;
        TKName = "CaptainSkill";
        //Durability = ConfigManager.GetHeroSkillParams(TKName, 2);
        //MaxCoolDown = ConfigManager.GetHeroSkillParams(TKName, 0);

        //var listMods = QuanlyNguoichoi.Instance.GetListRuntimeStatMods();
        //if (listMods != null)
        //{
        //    var mod = listMods.Find(e => e.Stat == EStatType.HEROPLUS && e.Target == "Captain");
        //    if (mod != null)
        //    {
        //        Durability = (int)Mathf.Round((float)mod.Val);
        //    }
        //}

        playerVisual = Unit.visualGO.GetComponentInChildren<PlayerVisual>();
        khienBone = playerVisual.transform.Find("jnt_shield1");

        //Durability -= QuanlyNguoichoi.Instance.SoLuotDungKyNang;
        //if (Durability < 0) Durability = 0;
        if (Unit && Unit.SungPhu)
        {
            UpdateShooterStats();
            Unit.SungPhu.IsActive = true;
#if DEBUG
            Debug.Log("Enable sung phu");
#endif
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        //if (timeActivate > 0)
        //{
        //    timeActivate -= Time.deltaTime;
        //    if(timeActivate<0.2f)
        //    {
        //        Unit.Animators[0].SetTrigger("end_skill");
        //    }
        //}
        //else
        //{
        //    deactiveSkill();
        //}

        if (Unit && Unit.SungPhu && Unit.SungPhu.IsActive)
        {
            if (khienBone && khienBone.gameObject.activeSelf == false)
            {
                khienBone.gameObject.SetActive(true);
            }
        }
    }

    public void DeactiveKhien()
    {
        //if (Unit && Unit.SungPhu && Unit.SungPhu.IsActive)
        {
            if (khienBone && khienBone.gameObject.activeSelf)
            {
                khienBone.gameObject.SetActive(false);
            }
        }
    }


    public override void OnUnitDied()
    {
        base.OnUnitDied();
        //timeActivate = 0f;
        //Unit.Animators[0].SetTrigger("end_skill");
        //deactiveSkill();
    }

    //public override void OnUnitFired(Transform target)
    //{
    //    base.OnUnitFired(target);
    //}

    public override void OnBeforeUnitFired(Transform target)
    {
        if (Unit && Unit.SungPhu && Unit.SungPhu.IsActive)
            UpdateShooterStats();

        
    }

    public override bool Activate()
    {
        base.Activate();
        //if (CoolDown <= 0 && Durability > 0)
        //{
        //    CoolDown = MaxCoolDown;

        //    float duration = ConfigManager.GetHeroSkillParams(TKName, 1);
        //    // effect have only purpose for visual
        //    //Unit.GetStatusEff().ApplyEffect(new BattleEffect(new EffectConfig
        //    //{
        //    //    Type = EffectType.BatTu,
        //    //    Duration = duration
        //    //}));
        //    //Unit.SetInvulnerableTime(duration);
        //    if (Unit && Unit.SungPhu)
        //    {
        //        Unit.SungPhu.IsActive = true;
        //        Unit.GetComponent<PlayerMovement>().vuaDiVuaBan = true;

        //        if (Unit.Animators != null && Unit.Animators.Length > 0)
        //            Unit.Animators[0].SetTrigger("start_skill");

        //        if (playerVisual != null)
        //            playerVisual.SetVisualWeapon(false);

        //        if (Unit.Animators != null && Unit.Animators.Length > 0)
        //            Unit.Animators[0].SetLayerWeight(Unit.Animators[0].GetLayerIndex("Upperbody"), 1f);

        //        UpdateShooterStats();
        //    }

        //    timeActivate = duration;

        //    Durability--;
        //    return true;
        //}

        return false;
    }

    public override void OnUnitHoiSinh()
    {
        if (Unit && Unit.SungPhu)
        {
            UpdateShooterStats();
            Unit.SungPhu.IsActive = true;
#if DEBUG
            Debug.Log("Enable sung phu");
#endif
        }
    }

    void UpdateShooterStats()
    {
        var shooter = Unit.SungPhu.sung1;
        //var shooter2 = Unit.SungPhu.sung2;
        //var shooter3 = Unit.SungPhu.sung3;
        //var shooter4 = Unit.SungPhu.sung4;
        var mCurStat = Unit.GetCurStat();
        var OriginStat = Unit.OriginStat;
        var TimeScale = Unit.TimeScale;
        var RageBuffAtk = Unit.RageBuffAtk;
        var RageBuffAtkSpd = Unit.RageBuffAtkSpd;

        if (shooter)
        {
            long dmg = mCurStat.DMG + (OriginStat.DMG * RageBuffAtk / 100);
            float akSpd = mCurStat.ATK_SPD + (OriginStat.ATK_SPD * RageBuffAtkSpd / 100);

            //shooter.DMG = dmg;
            shooter.DMG = dmg * ConfigManager.GetHeroSkillParams(TKName, 0) / 100;
            shooter.AtkSpd = akSpd * TimeScale;
            //shooter.AtkSpd = 1000;
            shooter.ProjSpd = mCurStat.PROJ_SPD;
            shooter.KnockBackDistance = mCurStat.KNOCK_BACK;
            //shooter.KnockBackDistance = ConfigManager.GetHeroSkillParams(TKName, 4) / 100;

            shooter.SetFireClip("main_attack_gun");
            //shooter.SetFlashPos(weaponBone);
        }
        //if (shooter2)
        //{
        //    shooter2.DMG = shooter.DMG;
        //    shooter2.ProjSpd = shooter.ProjSpd;
        //    shooter2.AtkSpd = shooter.AtkSpd;
        //    shooter2.AttackAnimTime = shooter.AttackAnimTime;
        //    shooter2.DelayActiveDamage = shooter.DelayActiveDamage;
        //    shooter2.KnockBackDistance = shooter.KnockBackDistance;
        //}
        //if (shooter3)
        //{
        //    shooter3.DMG = shooter.DMG;
        //    shooter3.ProjSpd = shooter.ProjSpd;
        //    shooter3.AtkSpd = shooter.AtkSpd;
        //    shooter3.AttackAnimTime = shooter.AttackAnimTime;
        //    shooter3.DelayActiveDamage = shooter.DelayActiveDamage;
        //    shooter3.KnockBackDistance = shooter.KnockBackDistance;
        //}
        //if (shooter4)
        //{
        //    shooter4.DMG = shooter.DMG;
        //    shooter4.ProjSpd = shooter.ProjSpd;
        //    shooter4.AtkSpd = shooter.AtkSpd;
        //    shooter4.AttackAnimTime = shooter.AttackAnimTime;
        //    shooter4.DelayActiveDamage = shooter.DelayActiveDamage;
        //    shooter4.KnockBackDistance = shooter.KnockBackDistance;
        //}
    }
}
