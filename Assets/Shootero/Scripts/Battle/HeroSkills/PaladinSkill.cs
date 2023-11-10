using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinSkill : TuyetKy
{
    PaladinKiemKhi listBarels;
    List<KiemKhi> listCurKiemKhi = new List<KiemKhi>();

    public override void OnStart()
    {
        base.OnStart();
        Loai = LoaiTK.ChuDong;
        TKName = "PaladinSkill";
        Durability = ConfigManager.GetHeroSkillParams(TKName, 2);
        MaxCoolDown = ConfigManager.GetHeroSkillParams(TKName, 0);

        var listMods = QuanlyNguoichoi.Instance.GetListRuntimeStatMods();
        if (listMods != null)
        {
            var mod = listMods.Find(e => e.Stat == EStatType.HEROPLUS && e.Target == "Paladin");
            if (mod != null)
            {
                Durability = (int)Mathf.Round((float)mod.Val);
                var overrideCD = ConfigManager.GetPaladinSkillUpgradeCooldown();
                if (overrideCD > 0)
                {
                    MaxCoolDown = overrideCD;
                }
            }
        }

        if (listBarels == null)
        {
            listBarels = Unit.gameObject.GetComponentInChildren<PaladinKiemKhi>();
        }

        Durability -= QuanlyNguoichoi.Instance.SoLuotDungKyNang;
        if (Durability < 0) Durability = 0;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override bool Activate()
    {
        base.Activate();
        if (CoolDown <= 0 && Durability > 0)
        {
            CoolDown = MaxCoolDown;

            listCurKiemKhi.Clear();
            var curUnitSat = Unit.GetCurStat();
            for (int i = 0; i < listBarels.barels.Length; ++i)
            {
                var bullet = ObjectPoolManager.Spawn(listBarels.kiemKhiPrefab.gameObject, Vector3.zero, Quaternion.identity, listBarels.barels[i]);
                bullet.gameObject.SetActive(true);
                var kiemKHi = bullet.GetComponent<KiemKhi>();
                kiemKHi.CRIT = curUnitSat.CRIT;
                kiemKHi.CRIT_DMG = curUnitSat.CRIT_DMG;
                kiemKHi.SourceUnit = Unit;
                listCurKiemKhi.Add(kiemKHi);
            }

            int dmgPercent = ConfigManager.GetHeroSkillParams(TKName, 1);

            long dmg = Unit.GetCurStat().DMG * dmgPercent / 100;

            //float duration = ConfigManager.GetHeroSkillParams("PaladinSkill", 1);
            //// effect have only purpose for visual
            //Unit.GetStatusEff().ApplyEffect(new BattleEffect(new EffectConfig
            //{
            //    Type = EffectType.BatTu,
            //    Duration = duration
            //}));
            //Unit.SetInvulnerableTime(duration);

            //float delay = ConfigManager.GetHeroSkillParams("PaladinSkill", 3) / 100f;

            Unit.StartCoroutine(CoProcessKiemKhi(dmg, 0.1f));

            //UnityEditor.EditorApplication.isPaused = true;

            Durability--;

            return true;
        }

        return false;
    }

    IEnumerator CoProcessKiemKhi(long dmg, float delay)
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < listCurKiemKhi.Count; ++i)
        {
            yield return new WaitForSeconds(delay);
            listCurKiemKhi[i].ActiveDan(Unit.GetCurStat().PROJ_SPD, dmg, Vector3.zero);
        }
    }
}
