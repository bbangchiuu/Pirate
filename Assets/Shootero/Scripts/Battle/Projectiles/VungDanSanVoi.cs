using Hiker.GUI.Shootero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VungDanSanVoi : SatThuongDT
{
    protected override bool OnCollisionOther(GameObject other)
    {
        var unit = other.GetComponentInParent<DonViChienDau>();

        if (unit != null)
        {
            if (unit.IsAlive() == false) return false;
            if (unit.IsCoTheDinhDan() == false) return false;

            var re = OnCatchTarget(unit);
            //if (re)
            //{
            //    OnDeactiveDan();
            //}
            return re;
        }
        else
        {
            if (OnHitObstacle(other))
            {
                OnDeactiveDan();
                return true;
            }
        }

        return false;
    }

    protected override bool OnCatchTarget(DonViChienDau unit)
    {
        var knockBack = KnockBackDistance;
        var re = base.OnCatchTarget(unit);
        KnockBackDistance = knockBack;
        return re;
    }

    protected override void OnDeactiveDan()
    {
        base.OnDeactiveDan();
        Hiker.HikerUtils.DoAction(this,() => {
            gameObject.SetActive(false);
            ObjectPoolManager.Unspawn(gameObject);
        }, 0.2f);
    }

    protected override void Update()
    {
        base.Update();

        if (LifeTime > 0)
        {
            LifeTime -= Time.deltaTime;
        }
        else
        {
            DeactiveDan();
        }
    }
}
