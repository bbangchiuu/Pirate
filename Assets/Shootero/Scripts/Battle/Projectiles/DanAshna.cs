using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanAshna : DanBay
{
    public Vector3 targetPosition;

    public Vector3 Target { get; protected set; }
    protected GameObject mIndicator;

    public float ChuKi = 2;
    public float BanKinhDaoDong = 2;

    private float activeTime = 0;
    private Vector3 rightGoc;
    private Vector3 forwardGoc;
    private Vector3 old_dr = Vector3.zero;

    float initTimeClock = 0;

    // Update is called once per frame
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

            float dTimeFromActive = Time.timeSinceLevelLoad - activeTime;

            float __sx = Mathf.Sin(dTimeFromActive*Mathf.PI/ChuKi);

            Vector3 new_dr = __sx * rightGoc * BanKinhDaoDong;

            Vector3 dr = new_dr - old_dr;

            old_dr = new_dr;

            transform.Translate(forwardGoc * distance + dr, Space.World);
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
        activeTime = Time.timeSinceLevelLoad;
        rightGoc = this.transform.right;
        forwardGoc = this.transform.forward;
        old_dr = Vector3.zero;

        base.ActiveDan(speed, dmg, target);
    }

    protected override bool OnCollisionOther(GameObject other)
    {
        var proj = other.GetComponentInParent<DonViChienDau>();
        if (proj != null)
        {
            proj.TakeDamage(Damage,false,SourceUnit);
            DeactiveProj();
            return true;
        }
        //else
        //{
        //    if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle") ||
        //        other.gameObject.layer == LayerMask.NameToLayer("Default"))
        //    {
        //        mIsActive = false;
        //        return true;
        //    }
        //}

        return false;
    }

    protected override void OnDeactiveProjectile()
    {
        base.OnDeactiveProjectile();
    }
}
