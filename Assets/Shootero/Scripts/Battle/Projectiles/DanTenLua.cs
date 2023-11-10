using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanTenLua : DanBay
{
    public Vector3 targetPosition;

    public GameObject indicatorPrefab;
    public float RandomRadius = 0;

    public float maxHeight = 3;

    public float initTime = 0.1f;

    public Vector3 Target { get; protected set; }
    protected GameObject mIndicator;

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
            //transform.Translate(Speed * transform.forward * Time.deltaTime, Space.World);
            UpdateMissile(Time.deltaTime);
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

    protected void UpdateMissile(float deltaTime)
    {
        float crDistance = Vector3.Distance(this.transform.position, Target);
        Vector3 dirToTarget = (Target - this.transform.position).normalized;
        //dirToTarget.y = 0;
        Vector3 dirToUp = Vector3.RotateTowards(Vector3.up, dirToTarget, 35 * Mathf.Deg2Rad, 0).normalized;
        Vector3 dir = Vector3.zero;
        initTimeClock += deltaTime;
        float ratio = initTimeClock / initTime;
        ratio = Mathf.Clamp01(ratio);

        dir = Vector3.Slerp(dirToUp, dirToTarget, ratio);

        if (dir == Vector3.zero)
        {
            this.transform.LookAt(Target);
        }
        else
        {
            this.transform.LookAt(dir);
        }

        float moveDistance = Speed * deltaTime;

        if (crDistance < moveDistance) moveDistance = crDistance;
        Vector3 moveSpeed = dir.normalized * moveDistance;

        this.transform.position += moveSpeed;
        //        Debug.Log("Speed " + speed + " __________"+ moveSpeed.magnitude / BattleManager.GetDeltaTime());

        if (this.transform.position.y <= 0 || crDistance <= 0.01f)
        {
            OnReachTarget();
        }

        //transform.Translate(Speed * transform.forward * Time.deltaTime, Space.World);
    }

    protected virtual void OnReachTarget()
    {
        if (mIndicator)
        {
            mIndicator.gameObject.SetActive(false);
        }
        DeactiveProj();
    }

    public override void ActiveDan(float speed, long dmg, Vector3 target)
    {
        base.ActiveDan(speed, dmg, target);
        this.Speed = speed;
        targetPosition = target;
        LifeTime = 1000;
        initTimeClock = 0;

        if (RandomRadius > 0)
        {
            var r = Random.insideUnitCircle * RandomRadius;

            var targetPos = targetPosition + new Vector3(r.x, 0, r.y);

            if (UnityEngine.AI.NavMesh.SamplePosition(targetPos,
                    out UnityEngine.AI.NavMeshHit hit,
                    0.3f,
                    UnityEngine.AI.NavMesh.AllAreas) == false)
            {
                var sourcePos = target;
                if (SourceUnit != null && SourceUnit.transform != null)
                    sourcePos = SourceUnit.transform.position;

                sourcePos.y = target.y;
                if (UnityEngine.AI.NavMesh.Raycast(sourcePos, targetPos, out hit, UnityEngine.AI.NavMesh.AllAreas))
                {
                    targetPos = hit.position;
                    targetPos.y = targetPosition.y;
                }
            }

            this.Target = targetPos;
        }
        else
        {
            this.Target = targetPosition;
        }

        var t = Vector3.Distance(transform.position, Target) / speed;
        initTime = Mathf.Max(t * 0.5f, 0.1f);

        if (indicatorPrefab != null)
        {
            var indicator = Instantiate(indicatorPrefab);
            indicator.transform.position = this.Target;
            indicator.transform.rotation = Quaternion.identity;
            indicator.gameObject.SetActive(true);
            mIndicator = indicator;
        }
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

        if (mIndicator)
        {
            Destroy(mIndicator);
        }
    }
}
