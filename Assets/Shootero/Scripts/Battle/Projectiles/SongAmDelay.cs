using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongAmDelay : DanBay
{
    public float DelayTime = 1f;

    protected override bool OnHitObstacle(GameObject obstacle)
    {
        //return base.OnHitObstacle(obstacle);
        if (LayersMan.CheckMask(obstacle.layer, LayersMan.Team1HitLayerMask))
        { // deactive when hit team1 object
            return true;
        }
        return false; // mac dinh damage object xuyen obstacle
    }

    public float WaitTimer = 0;

    protected override void Update()
    {
        if (mIsActive)
        {
            if (QuanlyNguoichoi.Instance.IsLoadingMission)
            {
                DeactiveProj();
                return;
            }

            float distance = Speed * Time.deltaTime;
            WaitTimer += Time.deltaTime;
            if (ReflectCounter > 0)
            {
                RaycastHit hit;
                if (Physics.Raycast(
                        transform.position,
                        transform.forward,
                        out hit,
                        distance,
                        LayerMask.GetMask("Obstacle", "Default")))
                {
                    var outDir = Vector3.Reflect(transform.forward, hit.normal);
                    var reflectDis = distance - Vector3.Distance(hit.point, transform.position);
                    transform.position = hit.point + outDir * reflectDis;
                    transform.rotation = Quaternion.LookRotation(outDir);
                    ReflectCounter--;
                    Damage = Damage * ReflectDMG / 100;
                    

                }
                else
                {
                    transform.Translate(transform.forward * distance, Space.World);
                }
            }
            else
            {
                if(WaitTimer> DelayTime)
                    transform.Translate(transform.forward * distance, Space.World);
            }
        }

        if (LifeTime > 0)
        {
            LifeTime -= Time.deltaTime;
        }
        else
        {
            DeactiveProj();
        }
    }

    public override void ActiveDan(float speed, long dmg, Vector3 target)
    {
        base.ActiveDan(speed, dmg, target);
        WaitTimer = 0;
    }

}
