using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class ThorSkill : TuyetKy
{
    VongNangLuongTho visual;
    int numOrb = 0;
    public override void OnStart()
    {
        base.OnStart();
        Loai = LoaiTK.BiDong;
        TKName = "ThorSkill";

        numOrb = 0;

        numOrb = QuanlyNguoichoi.Instance.SoLuotDungKyNang;
        if (visual == null)
        {
            visual = GameObject.Instantiate(Resources.Load<VongNangLuongTho>("VongNangLuong"), Unit.transform.position + Vector3.up * 1f, Unit.transform.rotation);
            visual.gameObject.AddComponent<TargetFollower>().target = Unit.transform;
        }
        visual.gameObject.SetActive(true);
        visual.Setup(numOrb);
    }

    void GetThunderOrb()
    {
        int max = ConfigManager.GetHeroSkillParams(TKName, 0);
        var listMods = QuanlyNguoichoi.Instance.GetListRuntimeStatMods();
        if (listMods != null)
        {
            var mod = listMods.Find(e => e.Stat == EStatType.HEROPLUS && e.Target == "Thor");
            if (mod != null)
            {
                max = (int)Mathf.Round((float)mod.Val);
            }
        }

        if (numOrb < max)
        {
            numOrb++;
            visual.Setup(numOrb);
            Unit.BuffAtkUp(ConfigManager.GetHeroSkillParams(TKName, 1));
            QuanlyNguoichoi.Instance.CountSkillPassive();
        }
    }

    public override void OnEnemyDied(DonViChienDau unit)
    {
        base.OnEnemyDied(unit);
        if (unit.IsBoss)
        {
            if (unit.UnitName == "LizardShaman" ||
                unit.UnitName == "Coop_LizardShaman")
            {
                var tachUnit = unit.GetComponent<TachUnitKhiMatMau>();
                if (tachUnit && tachUnit.IsUnitGoc)
                {
                    return; // khong duoc orb khi chua giet duoc unit cuoi cung
                }
            }

            GetThunderOrb();
            if (QuanlyNguoichoi.Instance.HaveHeroPlus("Thor"))
            {
                GetThunderOrb();
            }
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (QuanlyNguoichoi.Instance.gameObject.activeSelf == false)
        {
            if (visual)
            {
                visual.gameObject.SetActive(false);
                GameObject.Destroy(visual.gameObject);
            }
        }
    }
}
