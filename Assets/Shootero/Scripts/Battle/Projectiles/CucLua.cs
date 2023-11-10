using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CucLua : SatThuongDT
{
    public bool DontHaveTimelife = false;

    protected override void Update()
    {
        //if (QuanlyNguoichoi.Instance.IsLoadingMission)
        //{
        //    OnDeactiveFire();
        //    return;
        //}

        //if (QuanlyNguoichoi.Instance.IsLevelClear && DontHaveTimelife)
        //{
        //    OnDeactiveFire();
        //    return;
        //}

        if (DontHaveTimelife == false)
        {
            if (LifeTime > 0)
            {
                LifeTime -= Time.deltaTime;
            }
            else
            {
                OnDeactiveFire();
            }
        }
    }

    protected void OnDeactiveFire()
    {
        mIsActive = false;
        gameObject.SetActive(false);
        //Destroy(gameObject);
        ObjectPoolManager.Unspawn(this.gameObject);
    }

    protected override bool OnHitObstacle(GameObject obstacle)
    {
        return false;
    }

    //protected override bool OnCollisionOther(GameObject other)
    //{
    //    var unit = other.GetComponentInParent<BattleUnit>();
    //    if (unit != null)
    //    {
    //        var re = OnCatchTarget(unit);
    //        if (re)
    //        {
    //            OnDeactiveDan();
    //        }
    //        return re;
    //    }
    //    else
    //    {
    //        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle") ||
    //            other.gameObject.layer == LayerMask.NameToLayer("Default"))
    //        {
    //            if (OnHitObstacle(other))
    //            {
    //                OnDeactiveDan();
    //                return true;
    //            }
    //        }
    //    }

    //    return false;
    //}
}
