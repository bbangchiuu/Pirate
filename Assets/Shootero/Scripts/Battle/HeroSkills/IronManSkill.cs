using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronManSkill : TuyetKy
{
    public string DeTu;

    static readonly string[] DeTuArray = new string[]
    {
        "IronManMedic",
        "IronManFalcon",
        "IronManRocket",
    };

    /// <summary>
    /// Lay ten de tu theo id
    /// 1: Medic, 2: Falcon, 3: Rocket
    /// </summary>
    /// <param name="id">1: Medic, 2: Falcon, 3: Rocket</param>
    /// <returns></returns>
    public static string GetDeTuNameByID(int id)
    {
        return DeTuArray[id - 1];
    }

    void InitUnit()
    {
        TKName = "IronManSkill";

        if (QuanlyNguoichoi.Instance.SoLuotDungKyNang > 0)
        {
            if (QuanlyNguoichoi.Instance.SoLuotDungKyNang <= DeTuArray.Length)
            {
                DeTu = DeTuArray[QuanlyNguoichoi.Instance.SoLuotDungKyNang - 1];
            }
        }
    }

    public void ThuPhucDeTu(string detu)
    {
        if (string.IsNullOrEmpty(detu))
        {
            return;
        }

        if (QuanlyNguoichoi.Instance == null || QuanlyNguoichoi.Instance.SoLuotDungKyNang > 0) return;

        int deTuID = 0;
        for (int i = 0; i < DeTuArray.Length; ++i)
        {
            if (detu == DeTuArray[i])
            {
                DeTu = detu;
                deTuID = i + 1;
                break;
            }
        }

        if (deTuID > 0 && string.IsNullOrEmpty(DeTu) == false &&
            QuanlyNguoichoi.Instance)
        {
            QuanlyNguoichoi.Instance.ChonDeTuIR(DeTu, deTuID);
        }
    }
    public override void OnStart()
    {
        base.OnStart();
        Loai = LoaiTK.BiDong;
        TKName = "IronManSkill";
        InitUnit();
    }
}