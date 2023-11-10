using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecromancerSkill : TuyetKy
{
    //PaladinKiemKhi listBarels;
    //List<KiemKhi> listCurKiemKhi = new List<KiemKhi>();

    public int SoulToCharge;
    public int CurrSoul;

    public int MaxCharge;

    public int CurrCharge;

    public int SkeletonLifeTime;

    public override void OnStart()
    {
        base.OnStart();
        Loai = LoaiTK.ChuDong;
        TKName = "NecromancerSkill";
        Durability = 99999;
        MaxCoolDown = ConfigManager.GetHeroSkillParams(TKName, 0);
        SoulToCharge = ConfigManager.GetHeroSkillParams(TKName, 1);
        MaxCharge = ConfigManager.GetHeroSkillParams(TKName, 2);
        SkeletonLifeTime = ConfigManager.GetHeroSkillParams(TKName, 3);

        // TODO : upgrade skill

        //var listMods = QuanlyNguoichoi.Instance.GetListRuntimeStatMods();
        //if (listMods != null)
        //{
        //    var mod = listMods.Find(e => e.Stat == EStatType.HEROPLUS && e.Target == "Paladin");
        //    if (mod != null)
        //    {
        //        Durability = (int)Mathf.Round((float)mod.Val);
        //        var overrideCD = ConfigManager.GetPaladinSkillUpgradeCooldown();
        //        if (overrideCD > 0)
        //        {
        //            MaxCoolDown = overrideCD;
        //        }
        //    }
        //}

        //if (listBarels == null)
        //{
        //    listBarels = Unit.gameObject.GetComponentInChildren<PaladinKiemKhi>();
        //}

        //Durability -= QuanlyNguoichoi.Instance.SoLuotDungKyNang;
        //if (Durability < 0) Durability = 0;

    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnEnemyDied(DonViChienDau unit)
    {
        base.OnEnemyDied(unit);
        CurrSoul++;
        if (CurrSoul >= SoulToCharge)
        {
            CurrSoul -= SoulToCharge;
            IncreaseCharge();
        }
    }
    
    public void IncreaseCharge()
    {
        CurrCharge++;
        if (CurrCharge > MaxCharge)
            CurrCharge = MaxCharge;
    }

    IEnumerator SpawnSkeleton(int count)
    {
        for (int i = 0; i < count; i++)
        {
            QuanlyNguoichoi.Instance.NecromancerSpawnSkeleton(SkeletonLifeTime);
            yield return new WaitForSeconds(0.3f);
        }
    }

    public override bool Activate()
    {
        base.Activate();
        if (CoolDown <= 0 && CurrCharge > 0)
        {
            Unit.StartCoroutine(SpawnSkeleton(CurrCharge));

            CoolDown = MaxCoolDown;
            CurrCharge = 0;

            return true;
        }

        return false;
    }

}
