using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ANTICHEAT
using Int64 = CodeStage.AntiCheat.ObscuredTypes.ObscuredLong;
using Int32 = CodeStage.AntiCheat.ObscuredTypes.ObscuredInt;
using Float = CodeStage.AntiCheat.ObscuredTypes.ObscuredFloat;
using Bool = CodeStage.AntiCheat.ObscuredTypes.ObscuredBool;
#else
using Int64 = System.Int64;
using Int32 = System.Int32;
using Float = System.Single;
using Bool = System.Boolean;
#endif


public class DaiTuTeQuaCauBay : SatThuongDT
{
    public int ReflectCount = 0;

    protected int ReflectCounter = 0;

    public int ReflectDMG = 100;
    public int PiercingCount = 0;
    public int PiercingDMG = 100;

    public float Speed;

    protected TweenScale m_TweenScale = null;

    protected int MaskReflect = 0;

    public void ResetReflectCounter()
    {
        ReflectCounter = ReflectCount;
    }

    protected virtual void Awake()
    {
        MaskReflect = LayerMask.GetMask("Obstacle", "Default");
        ResetReflectCounter();

        int __r = Random.Range(0, 100);

        float _goc = -150;
        if (__r > 50)
        {
            _goc = Random.Range(-170, -150);
        }
        else
        {
            _goc = Random.Range(-210, -190);
        }

        this.transform.eulerAngles = new Vector3(0, _goc,0);

    }

    protected virtual void OnDeactiveProjectile()
    {
    }
    
    GameObject mLastObstacleReflect = null;

    protected override bool OnHitObstacle(GameObject obstacle)
    {
        if (ReflectCounter > 0) // khong disable object neu object co reflect
        {
            return false;
        }
        else // projectile khong xuyen obstacle
        {
            return true;
        }
    }
    
    Vector3 lastPosRaycast;
    bool firstRaycast = false;
    float timeRaycast = 0;
    float raycastDistance = 0;
    const float timeCheckRaycast = 0;//1f / 25;
    void ResetRaycast()
    {
        lastPosRaycast = transform.position;
        raycastDistance = 0;
        timeRaycast = 0;
        firstRaycast = true;
    }
    bool CheckRayCast(float distance, float deltaTime)
    {
        if (ReflectCounter <= 0) return false;
        if (firstRaycast == false)
        {
            ResetRaycast();
        }

        raycastDistance += distance;

        if (timeRaycast < timeCheckRaycast)
        {
            timeRaycast += deltaTime;
            return false;
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(
                    lastPosRaycast,
                    transform.forward,
                    out hit,
                    raycastDistance,
                    MaskReflect,
                    QueryTriggerInteraction.Ignore))
            {
                var outDir = Vector3.Reflect(transform.forward, hit.normal);
                var reflectDis = distance - Vector3.Distance(hit.point, transform.position);
                transform.position = hit.point + outDir * reflectDis;
                transform.rotation = Quaternion.LookRotation(outDir);
                ReflectCounter--;
                Damage = Damage * ReflectDMG / 100;
                
                ResetRaycast();

                return true;
            }
            else
            {
                timeRaycast = deltaTime;
                raycastDistance = distance;
                lastPosRaycast = transform.position;

                return false;
            }
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (QuanlyManchoi.instance != null && QuanlyManchoi.instance.IsLevelClear())
        {
            GameObject.Destroy(this.gameObject);
        }

        //if (mIsActive)
        {
            if (QuanlyNguoichoi.Instance.IsLoadingMission)
            {
                return;
            }

            float distance = Speed * Time.deltaTime;
            if (ReflectCounter > 0)
            {
                //RaycastHit hit;
                //if (Physics.Raycast(
                //        transform.position,
                //        transform.forward,
                //        out hit,
                //        distance,
                //        MaskReflect,
                //        QueryTriggerInteraction.Ignore))
                //{
                //    var outDir = Vector3.Reflect(transform.forward, hit.normal);
                //    var reflectDis = distance - Vector3.Distance(hit.point, transform.position);
                //    transform.position = hit.point + outDir * reflectDis;
                //    transform.rotation = Quaternion.LookRotation(outDir);
                //    ReflectCounter--;
                //    Damage = Damage * ReflectDMG / 100;
                //}
                if (CheckRayCast(distance+0.1f, Time.deltaTime))
                {

                }
                else
                {
                    transform.Translate(transform.forward * distance, Space.World);
                }
            }
            else
            {
                transform.Translate(transform.forward * distance, Space.World);
            }
        }

        if (LifeTime > 0)
        {
            LifeTime -= Time.deltaTime;
        }
        else
        {
        }
    }
    
}
