using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenLuaDanDao : DanBay
{
    public float TimeToReachTarget = 4;
    public float RandomRadius = 8;
    public GameObject indicatorPrefab;
    public Vector3 Target { get; protected set; }

    float mTimeActive = 0;
    public float maxAltitude = 100;

    GameObject mIndicator;

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
            //transform.Translate(Speed * transform.forward * Time.deltaTime, Space.World);
            mTimeActive += Time.deltaTime;

            if (mTimeActive > TimeToReachTarget - 100 / Speed)
            {
                float time = mTimeActive - TimeToReachTarget + 100 / Speed;
                transform.position = Target + Vector3.up * Mathf.Lerp(100, 0, time / 100 * Speed);
                transform.forward = Vector3.down;
            }

            if (transform.position.y <= Target.y)
            {
                mIsActive = false;
                //gameObject.SetActive(false);
                //Destroy(gameObject);
                var selfExplode = GetComponent<SelfExplosion>();
                if (selfExplode != null)
                {
                    selfExplode.Activate(selfExplode.Radius, Damage, selfExplode.layerTarget, 0);
                }

                if (mIndicator)
                {
                    mIndicator.gameObject.SetActive(false);
                }
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
        this.Speed = speed;
        var r = Random.insideUnitCircle * RandomRadius;

        this.Target = target + new Vector3(r.x, 0, r.y);
        var dirUp = Vector3.RotateTowards(Vector3.up, transform.forward, 10 * Mathf.Deg2Rad, 0);
        transform.forward = dirUp;
        TweenPosition.Begin(gameObject, 100 / speed, transform.position + dirUp * 100, true);
        mTimeActive = 0;

        var indicator = Instantiate(indicatorPrefab);
        indicator.transform.position = this.Target;
        indicator.transform.rotation = Quaternion.identity;
        indicator.gameObject.SetActive(true);
        mIndicator = indicator;
    }

    protected override bool OnCollisionOther(GameObject other)
    {
        var re = base.OnCollisionOther(other);

        //if (re) // collide valid obj
        //{
        //    gameObject.SetActive(false);
        //    Destroy(gameObject);
        //}

        return re;
    }

    protected override void OnDeactiveProjectile()
    {
        if (mIndicator)
            Destroy(mIndicator);
    }
}
