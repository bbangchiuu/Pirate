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

public class BaiMinGoblin : DanTenLua
{
    public float TimeExplosion = 3f;
    public GameObject explosionPrefab;
    public Float TimeDelayExplode = 4;
    bool hitObstacle = false;

    public TweenScale warningTween;

    public bool TargetMainPlayer = false;


    public Bounds MetoerRandomField;

    protected override void Update()
    {
        if (QuanlyNguoichoi.Instance.IsLoadingMission)
        {
            DeactiveProj();
            return;
        }

        if (hitObstacle == false)
        {
            base.Update();
            transform.rotation = Quaternion.identity;
            return;
        }
        else
        {
            if (LifeTime > 0)
            {
                LifeTime -= Time.deltaTime;

                if(LifeTime < 2 && warningTween.enabled==false)
                {
                    warningTween.enabled = true;
                }
            }
            else
            {
                DeactiveProj();
            }
        }
    }

    protected override void OnDeactiveProjectile()
    {
        base.OnDeactiveProjectile();
        if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.IsLoadingMission == false)
        {
            var explosionEff = ObjectPoolManager.SpawnAutoUnSpawn(explosionPrefab, TimeExplosion);
            explosionEff.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            explosionEff.transform.rotation = Quaternion.identity;
        }
    }

    public override void ActiveDan(float speed, long dmg, Vector3 target)
    {
        Vector3 _cell = new Vector3(
            Random.Range(MetoerRandomField.min.x, MetoerRandomField.max.x),
            0,
            Random.Range(MetoerRandomField.min.z, MetoerRandomField.max.z));

        if (TargetMainPlayer)
        {
            _cell = QuanlyManchoi.instance.PlayerUnit.transform.position;
        }

        _cell.x = (int) (_cell.x/2f)*2+1;
        _cell.z = (int)(_cell.z / 2f) * 2;

        RandomRadius = 0;
        
        base.ActiveDan(speed, dmg, _cell);
        LifeTimeDuration = 1000f;
        LifeTime = LifeTimeDuration;
        hitObstacle = false;

        warningTween.enabled = false;
    }

    protected override bool OnHitObstacle(GameObject obstacle)
    {
        base.OnHitObstacle(obstacle);
        LifeTime = TimeDelayExplode;
        hitObstacle = true;
        return false;
    }

    protected override void OnReachTarget()
    {
        LifeTime = TimeDelayExplode;
        hitObstacle = true;
        if (mIndicator)
        {
            mIndicator.gameObject.SetActive(false);
        }
    }
}
