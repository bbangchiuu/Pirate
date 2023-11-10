using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuaThienThach : DanBay
{
    public float TimeToReachTarget = 4;
    public Bounds MetoerRandomField;
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

                LifeTime = 1f;
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
        target = new Vector3(
            Random.Range(MetoerRandomField.min.x, MetoerRandomField.max.x),
            0,
            Random.Range(MetoerRandomField.min.z, MetoerRandomField.max.z));

        base.ActiveDan(speed, dmg, target);
        this.Speed = 70;

        this.Target = target;
        var dirUp = Vector3.RotateTowards(Vector3.up, transform.forward, 30 * Mathf.Deg2Rad, 0);
        transform.forward = dirUp;
        transform.position = this.Target + dirUp * 100;
        //TweenPosition.Begin(gameObject, 100 / speed, transform.position + dirUp * 100, true);
        mTimeActive = 0;

        var indicator = Instantiate(indicatorPrefab);
        indicator.transform.position = this.Target;
        indicator.transform.rotation = Quaternion.identity;
        indicator.gameObject.SetActive(true);
        mIndicator = indicator;
    }

    private void OnEnable()
    {
        long dmg = 0;
        if (QuanlyNguoichoi.Instance != null)
        {
            dmg = QuanlyNguoichoi.Instance.GetDMGEnemy();
        }
        else
        {
            dmg = 100;
        }
        ActiveDan(Speed, dmg, Vector3.zero);
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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(MetoerRandomField.center, MetoerRandomField.size);
    }
#endif
}
