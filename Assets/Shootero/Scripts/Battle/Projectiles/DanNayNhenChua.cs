using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanNayNhenChua : DanBay
{
    protected override bool OnHitObstacle(GameObject obstacle)
    {
        //return base.OnHitObstacle(obstacle);
        if (LayersMan.CheckMask(obstacle.layer, LayersMan.Team1HitLayerMask))
        { // deactive when hit team1 object
            return true;
        }
        else if (ReflectCounter > 0) // khong disable object neu object co reflect
        {
            return false;
        }
        else // projectile khong xuyen obstacle
        {
            return true;
        }
    }
}
