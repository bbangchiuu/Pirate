using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuyetKy
{
    public enum LoaiTK
    {
        BiDong,
        ChuDong
    }
    public string TKName { get; protected set; }
    public LoaiTK Loai { get; set; }
    public DonViChienDau Unit { get; set; }
    public float CoolDown { get; protected set; }
    public float MaxCoolDown { get; protected set; }

    public int Durability { get; set; }
    public void ResetCoolDown()
    {
        CoolDown = 0;
    }

    public virtual void OnStart()
    {
        ResetCoolDown();
    }

    public virtual void OnUpdate()
    {
        if (CoolDown > 0)
        {
            CoolDown -= Time.deltaTime;
        }
    }

    public virtual EffectConfig GetEffectFromSkill()
    {
        return null;
    }

    public virtual void OnUnitFired(Transform target)
    {
    }

    public virtual void OnEnemyDied(DonViChienDau unit)
    {

    }

    public virtual void OnUnitDied()
    {

    }

    public virtual void OnUnitHoiSinh()
    {

    }

    public virtual void OnBeforeUnitFired(Transform target)
    {
    }

    public virtual void OnUnitActiveProj(Vector3 target)
    {

    }

    public virtual bool Activate()
    {
        return false;
    }
}
