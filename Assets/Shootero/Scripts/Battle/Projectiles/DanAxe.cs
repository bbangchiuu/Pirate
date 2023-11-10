using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanAxe : DanBay
{
    protected List<DonViChienDau> listTarget;
    public float Duration = 2;
    protected float timeIntervalDuration = 0;

    public float tickDot = 1;
    protected float timeIntervalDot = 0;

    protected bool mXoayTaiCho = false;
    protected override bool OnCatchTarget(DonViChienDau unit)
    {
        base.OnCatchTarget(unit);
        return false;
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

            if(timeIntervalDot > 0)
            {
                timeIntervalDot -= Time.deltaTime;
            }

            if (listTarget != null && listTarget.Count > 0)
            {
                if (mXoayTaiCho == false && Vector3.Distance(transform.position, listTarget[0].transform.position) <= 1f)
                {
                    mXoayTaiCho = true;
                }

                if(timeIntervalDot <= 0)
                {
                    for(int i = 0; i < listTarget.Count; i++)
                    {
                        base.OnCatchTarget(listTarget[i]);
                    }

                    timeIntervalDot = tickDot;
                }
            }

            if (mXoayTaiCho)
            {
                timeIntervalDuration -= Time.deltaTime;
                if(timeIntervalDuration <= 0)
                {
                    DeactiveProj();
                }
            }
            else
            {
                float distance = Speed * Time.deltaTime;
                transform.Translate(transform.forward * distance, Space.World);
            }
            //if (ReflectCounter > 0)
            //{               
            //    if (CheckRayCast(distance, Time.deltaTime))
            //    {

            //    }
            //    else
            //    {
            //        transform.Translate(transform.forward * distance, Space.World);
            //    }
            //}
            //else
            //{
            //    transform.Translate(transform.forward * distance, Space.World);
            //}
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        var unit = other.GetComponent<DonViChienDau>();
        if (listTarget != null && listTarget.Contains(unit))
        {
            listTarget.Remove(unit);
        }
    }
    protected override bool OnCollisionOther(GameObject other)
    {
        var unit = other.GetComponentInParent<DonViChienDau>();
        if (listTarget != null && listTarget.Contains(unit) == false)
        {
            listTarget.Add(unit);
        }
        return base.OnCollisionOther(other);
    }
    public override void ActiveDan(float speed, long dmg, Vector3 target)
    {
        base.ActiveDan(speed, dmg, target);
        timeIntervalDuration = Duration;
        timeIntervalDot = tickDot;

        mXoayTaiCho = false;
        if (listTarget == null)
        {
            listTarget = new List<DonViChienDau>();
        }
        else
        {
            listTarget.Clear();
        }
    }
}
