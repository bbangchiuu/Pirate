using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanBangNguoiKHOngLo : DanBay
{
    protected override bool OnHitObstacle(GameObject obstacle)
    {
        IceWall ice_wall = obstacle.GetComponent<IceWall>();

        if (ice_wall)
        {
            ice_wall.OnCollisionWithProjectile();

            Vector3 hit_normal = transform.position - obstacle.transform.position;
            Vector3 hit_pos = obstacle.transform.position;

            var outDir = Vector3.Reflect(transform.forward, hit_normal);
            outDir.y = 0;
            //var reflectDis = Vector3.Distance(hit_pos, transform.position);
            //transform.position = hit_pos + outDir * reflectDis;
            transform.rotation = Quaternion.LookRotation(outDir);
            ReflectCounter--;
            Damage = Damage * ReflectDMG / 100;

            return false;
        }
        else
        if (LayersMan.CheckMask(obstacle.layer, LayersMan.Team1HitLayerMask))
        {
            return true;
        }
        else
        {
            bool re = base.OnHitObstacle(obstacle);

            //IceWall ice_wall = obstacle.GetComponent<IceWall>();
            //if(ice_wall)
            //    ice_wall.OnCollisionWithProjectile();

            return re;
        }
    }

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
            //if (ReflectCounter > 0)
            {
                RaycastHit hit;
                if (Physics.Raycast(
                    transform.position,
                    transform.forward,
                    out hit,
                    distance,
                    LayerMask.GetMask("Obstacle", "Default")))
                {
                    IceWall ice_wall = hit.transform.GetComponent<IceWall>();
                    if (ice_wall != null)
                    {
                        ice_wall.OnCollisionWithProjectile();

                        var outDir = Vector3.Reflect(transform.forward, hit.normal);
                        outDir.y = 0;
                        var reflectDis = distance - Vector3.Distance(hit.point, transform.position);
                        transform.position = hit.point + outDir * reflectDis;
                        transform.rotation = Quaternion.LookRotation(outDir);
                        ReflectCounter--;
                        Damage = Damage * ReflectDMG / 100;
                    }
                    else
                    {
                        //transform.Translate(transform.forward * distance, Space.World);
                        DeactiveProj();
                    }
                }
                else
                {
                    transform.Translate(transform.forward * distance, Space.World);
                }
            }
            //else
            //{
            //    transform.Translate(transform.forward * distance, Space.World);
            //}
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

}
