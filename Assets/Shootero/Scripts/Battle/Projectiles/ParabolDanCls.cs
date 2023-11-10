using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ParabolDanCls : DanBay
{
    // PhuongTD : add spawn something when proj is destroy
    public float maxHeight = 3;
    private Vector3 startPos;
    private Vector3 endPos;

    public GameObject indicatorPrefab;
    protected GameObject mIndicator;
    public override void ActiveDan(float speed, long dmg, Vector3 target)
    {
        base.ActiveDan(speed, dmg, target);
        startPos = transform.position;
        endPos = target;
        if(indicatorPrefab != null)
        {
            var indicator = ObjectPoolManager.Spawn(indicatorPrefab, target, Quaternion.identity);
            indicator.transform.position = target;
            indicator.transform.rotation = Quaternion.identity;
            indicator.gameObject.SetActive(true);
            mIndicator = indicator;
        }
    }

    protected override void OnDeactiveProjectile()
    {
        base.OnDeactiveProjectile();
        if (mIndicator)
        {
            ObjectPoolManager.Unspawn(mIndicator);
        }
    }

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
            if (ReflectCounter > 0)
            {                
                if (CheckRayCast(distance, Time.deltaTime))
                {

                }
                else
                {
                    transform.position = MathParabola.Parabola(startPos, endPos, maxHeight, LifeTimeDuration - LifeTime);
                    LifeTime -= Time.deltaTime;
                }
            }
            else
            {
                transform.position = MathParabola.Parabola(startPos, endPos, maxHeight, LifeTimeDuration - LifeTime);
                LifeTime -= Time.deltaTime;
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
}

public class MathParabola
{
    public static float test(float t, float height)
    {
        return -4 * height * t * t + 4 * height * t;
    }

    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;
        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector2.Lerp(start, end, t);

        return new Vector2(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t));
    }

}