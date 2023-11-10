using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongThan : DanBay
{
    protected override bool OnHitObstacle(GameObject obstacle)
    {
        //return base.OnHitObstacle(obstacle);
        if (LayersMan.CheckMask(obstacle.layer, LayersMan.Team1HitLayerMask))
        { // deactive when hit team1 object
            return true;
        }
        return false; // mac dinh damage object xuyen obstacle
    }
    //protected override bool OnCollisionOther(GameObject other)
    //{
    //    if (ReflectCounter > 0)
    //    {
    //        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle") ||
    //            other.gameObject.layer == LayerMask.NameToLayer("Default"))
    //        {
    //            //mIsActive = false;
    //            return false;
    //        }
    //    }

    //    var proj = other.GetComponentInParent<BattleUnit>();
    //    if (proj != null)
    //    {
    //        proj.TakeDamage(Damage);
    //        mIsActive = false;
    //        gameObject.SetActive(false);
    //        Destroy(gameObject, 0.1f);
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}
}
