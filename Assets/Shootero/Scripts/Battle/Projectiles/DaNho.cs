using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaNho : DanBay
{
    public float AngleAroundNormal = 90f;
    SphereCollider col;
    protected override void Awake()
    {
        base.Awake();
        col = GetComponent<SphereCollider>();
    }

    protected override void OnDeactiveProjectile()
    {
        //base.OnDestroy();

        if (DestroyEff)
        {
            GameObject des_eff = ObjectPoolManager.SpawnAutoUnSpawn(DestroyEff, 3f);
            des_eff.transform.position = this.transform.position;
        }

        if (SpawnOnDestroy!= null)
        {
            int SpawnOnDestroyCount = 2;

            RaycastHit hit;
            var p = transform.TransformVector(Vector3.forward * col.radius);
            var distance = p.magnitude;
            if (Physics.Raycast(
                    transform.position,
                    transform.forward,
                    out hit,
                    distance * 3,
                    LayerMask.GetMask("Obstacle", "Default")))
            {
                var lhs = Vector3.Cross(hit.normal, Vector3.up);
                var n = SpawnOnDestroyCount;
                var a = AngleAroundNormal / (n - 1);
                var h = SpawnOnDestroyCount / 2;

                for (int i = 0; i < SpawnOnDestroyCount; ++i)
                {
                    Vector3 outDir = Vector3.zero;
                    if ((n & 1) == 1) // odd num
                    {
                        if (h > i)
                        {
                            var angle = (h - i) * a;
                            outDir = Vector3.RotateTowards(hit.normal, lhs, angle * Mathf.Deg2Rad, 0f);
                        }
                        else if (h < i)
                        {
                            var angle = (i - h) * a;
                            outDir = Vector3.RotateTowards(hit.normal, -lhs, angle * Mathf.Deg2Rad, 0f);
                        }
                        else
                        {
                            outDir = hit.normal;
                        }
                    }
                    else // even
                    {
                        if (h > i)
                        {
                            var angle = (h - i) * a - 0.5f * a;
                            outDir = Vector3.RotateTowards(hit.normal, lhs, angle * Mathf.Deg2Rad, 0f);
                        }
                        else
                        {
                            var angle = (i - h) * a + 0.5f * a;
                            outDir = Vector3.RotateTowards(hit.normal, -lhs, angle * Mathf.Deg2Rad, 0f);
                        }
                    }

                    var projObj = ObjectPoolManager.Spawn(SpawnOnDestroy.gameObject);
                    var proj = projObj.GetComponent<SatThuongDT>(); // Instantiate(SpawnOnDestroy);
                    //proj.transform.eulerAngles = new Vector3(0, rand_y, 0);
                    proj.transform.position = hit.point;
                    proj.transform.rotation = Quaternion.LookRotation(outDir);
                    proj.ActiveDan(Speed, Damage, Vector3.one);
                    proj.SourceUnit = SourceUnit;
                    proj.sungCls = sungCls;
                    proj.gameObject.SetActive(true);

                    //if (ScaleRate > 0 && ScaleRate != 1f)
                    //{
                    //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale * ScaleRate;
                    //}
                    //else
                    //{
                    //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale;
                    //}
                    ApplyScaleDanTo(projObj);
                    proj.ScaleRate = ScaleRate;
                }
            }
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

            transform.Translate(transform.forward * distance, Space.World);
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

    protected override bool OnCollisionOther(GameObject other)
    {
        return base.OnCollisionOther(other);
    }
}
