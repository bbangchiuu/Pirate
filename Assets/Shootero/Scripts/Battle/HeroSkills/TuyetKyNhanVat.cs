using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuyetKyNhanVat : MonoBehaviour
{
    DonViChienDau unit;
    List<TuyetKy> listSkills = new List<TuyetKy>();

    private void Awake()
    {
        unit = GetComponent<DonViChienDau>();

        switch (unit.UnitName)
        {
            case "BeastMaster":
                {
                    var skill = new BeastMasterSkill();
                    skill.Loai = TuyetKy.LoaiTK.BiDong;
                    skill.Unit = unit;

                    listSkills.Add(skill);
                }
                break;
            case "Paladin":
                {
                    var skill = new PaladinSkill();
                    skill.Loai = TuyetKy.LoaiTK.ChuDong;
                    skill.Unit = unit;

                    listSkills.Add(skill);
                }
                break;
            case "Graves":
                {
                    var skill = new GravesSkill();
                    skill.Loai = TuyetKy.LoaiTK.ChuDong;
                    skill.Unit = unit;

                    listSkills.Add(skill);
                }
                break;
            case "Wukong":
                {
                    var skill = new WukongSkill();
                    skill.Loai = TuyetKy.LoaiTK.BiDong;
                    skill.Unit = unit;
                    listSkills.Add(skill);
                }
                break;
            case "Thor":
                {
                    var skill = new ThorSkill();
                    skill.Loai = TuyetKy.LoaiTK.BiDong;
                    skill.Unit = unit;
                    listSkills.Add(skill);
                }
                break;
            case "Magician":
                {
                    var skill = new MagicianSkill();
                    skill.Loai = TuyetKy.LoaiTK.BiDong;
                    skill.Unit = unit;
                    listSkills.Add(skill);
                }
                break;
            case "Necromancer":
                {
                    var skill = new NecromancerSkill();
                    skill.Loai = TuyetKy.LoaiTK.ChuDong;
                    skill.Unit = unit;
                    listSkills.Add(skill);
                }
                break;
            case "Vayne":
                {
                    var skill = new VayneSkill();
                    skill.Loai = TuyetKy.LoaiTK.ChuDong;
                    skill.Unit = unit;
                    listSkills.Add(skill);
                }
                break;
            case "Captain":
                {
                    var skill = new CaptainSkill();
                    skill.Loai = TuyetKy.LoaiTK.BiDong;
                    skill.Unit = unit;
                    listSkills.Add(skill);
                }
                break;
            case "IronMan":
                {
                    var skill = new IronManSkill();
                    skill.Loai = TuyetKy.LoaiTK.BiDong;
                    skill.Unit = unit;
                    listSkills.Add(skill);
                }
                break;
            default:
                break;
        }
    }

    private void Start()
    {
        for (int i = listSkills.Count - 1; i >= 0; --i)
        {
            if (listSkills[i] != null) listSkills[i].OnStart();
        }
    }

    private void Update()
    {
        for (int i = listSkills.Count - 1; i >= 0; --i)
        {
            if (listSkills[i] != null) listSkills[i].OnUpdate();
        }
    }

    public void OnUnitActiveProj(Vector3 target)
    {
        for (int i = listSkills.Count - 1; i >= 0; --i)
        {
            if (listSkills[i] != null) listSkills[i].OnUnitActiveProj(target);
        }
    }

    public void OnBeforeUnitFired(Transform target)
    {
        for (int i = listSkills.Count - 1; i >= 0; --i)
        {
            if (listSkills[i] != null) listSkills[i].OnBeforeUnitFired(target);
        }
    }

    public void OnEnemyDie(DonViChienDau unit)
    {
        for (int i = listSkills.Count - 1; i >= 0; --i)
        {
            if (listSkills[i] != null) listSkills[i].OnEnemyDied(unit);
        }
    }

    public void OnUnitFired(Transform target)
    {
        for (int i = listSkills.Count - 1; i >= 0; --i)
        {
            if (listSkills[i] != null) listSkills[i].OnUnitFired(target);
        }
    }

    public void OnUnitDie()
    {
        for (int i = listSkills.Count - 1; i >= 0; --i)
        {
            if (listSkills[i] != null) listSkills[i].OnUnitDied();
        }
    }

    public void OnUnitHoiSinh()
    {
        for (int i = listSkills.Count - 1; i >= 0; --i)
        {
            if (listSkills[i] != null) listSkills[i].OnUnitHoiSinh();
        }
    }

    public void Activate(int skill)
    {
        if (listSkills.Count > skill)
        {
            listSkills[skill].Activate();
        }
    }

    public float GetTuyetKyCoolDown(int skill)
    {
        if (listSkills.Count > skill)
        {
            return listSkills[skill].CoolDown;
        }

        return 0;
    }

    public float GetTuyetKyCoolDownMaxTime(int skill)
    {
        if (listSkills.Count > skill)
        {
            return listSkills[skill].MaxCoolDown;
        }

        return 0;
    }

    public int GetTuyetKyDurability(int skill)
    {
        if (listSkills.Count > skill)
        {
            return listSkills[skill].Durability;
        }

        return 0;
    }

    public TuyetKy GetTuyetKy(int skill)
    {
        if (listSkills.Count > skill)
        {
            return listSkills[skill];
        }

        return null;
    }

    public bool HaveTuyetKyChuDong()
    {
        for (int i = listSkills.Count - 1; i >= 0; --i)
        {
            if (listSkills[i] != null &&
                listSkills[i].Loai == TuyetKy.LoaiTK.ChuDong)
            {
                return true;
            }
        }

        return false;
    }
}