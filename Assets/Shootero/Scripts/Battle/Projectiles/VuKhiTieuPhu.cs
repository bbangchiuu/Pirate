using Hiker.GUI.Shootero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VuKhiTieuPhu : DanBay
{
    public GameObject triggerDmg;
    public TweenRotation visualRotation;

    bool hitObstacle = false;
    bool callbackState = false;

    public static List<VuKhiTieuPhu> ListRiuActive = new List<VuKhiTieuPhu>();

    public static void RecallAxe(DonViChienDau orcKing)
    {
        for (int i = ListRiuActive.Count - 1; i >= 0; --i)
        {
            var proj = ListRiuActive[i];
            if (proj != null && proj.SourceUnit == orcKing)
            {
                proj.callbackState = true;
            }
        }
    }

    public override void ActiveDan(float speed, long dmg, Vector3 target)
    {
        LifeTimeDuration = 100000;
        base.ActiveDan(speed, dmg, target);
        hitObstacle = false;
        callbackState = false;
        ListRiuActive.Add(this);
    }

    protected override void OnDeactiveDan()
    {
        base.OnDeactiveDan();
        ListRiuActive.Remove(this);
    }

    protected override void Update()
    {
        if (QuanlyNguoichoi.Instance.IsLoadingMission)
        {
            OnDeactiveDan();
            return;
        }

        if (callbackState)
        {
            if (visualRotation && visualRotation.enabled == false)
            {
                visualRotation.enabled = true;
            }

            float distance = Speed * Time.deltaTime;
            if (SourceUnit != null)
            {
                var dif = SourceUnit.transform.position - transform.position;
                if (dif.sqrMagnitude < 0.1f)
                {
                    OnDeactiveDan();
                }
                else
                {
                    transform.Translate(dif.normalized * distance, Space.World);
                }
            }
            else
            {
                OnDeactiveDan();
            }
        }
        else
        {
            if (hitObstacle == false)
            {
                base.Update();
            }
            else
            {
                if (visualRotation)
                {
                    visualRotation.enabled = false;
                }
            }
        }
    }

    protected override bool OnCollisionOther(GameObject other)
    {
        var unit = other.GetComponentInParent<DonViChienDau>();
        if (unit != null)
        {
            var re = OnCatchTarget(unit);
            if (re)
            {
                OnDeactiveDan();
            }
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

    protected override bool OnHitObstacle(GameObject obstacle)
    {
        base.OnHitObstacle(obstacle);
        hitObstacle = true;
        return false;
    }

    protected override bool OnCatchTarget(DonViChienDau unit)
    {
        base.OnCatchTarget(unit);
        return false;
    }
}
