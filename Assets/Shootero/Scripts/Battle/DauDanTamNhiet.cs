using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DauDanTamNhiet : MonoBehaviour
{
    DanBay mDan;
    DonViChienDau unitTarget;
    public float BanKinhTamNhiet { get; set; }
    public float GocGioiHan { get; set; }
    public float DoNhay { get; set; }
    bool autoFindTarget = true;

    private void Awake()
    {
        mDan = GetComponent<DanBay>();
#if DEBUG
        //Time.timeScale = 0.2f;
#endif
    }

    public void SetTarget(DonViChienDau target)
    {
        unitTarget = target;
        autoFindTarget = false;
    }

    private void OnEnable()
    {
        mTickCheckTarget = 0;// Random.Range(0, 0.1f);
    }

    float mTickCheckTarget;
    private void Update()
    {
        //if (BanKinhTamNhiet > 0 && autoFindTarget && unitTarget == null)
        //{
        //    mTickCheckTarget = 0;
        //}

//        if (mTickCheckTarget <= 0 && BanKinhTamNhiet > 0 && autoFindTarget)
//        {
//            mTickCheckTarget = 0.5f;
//            var listEnemies = QuanlyManchoi.FindEnemiesInRange(transform.position, BanKinhTamNhiet);

//            float closest = 0;
//            DonViChienDau newTarget = null;
//            for (int i = 0; i < listEnemies.Count; ++i)
//            {
//                var e = listEnemies[i];
//                var dif = e.transform.position - transform.position;
//                dif.y = 0;
//                var dis = dif.magnitude;
//                var dot = Vector3.Dot(dif, transform.forward);
//                if (dot > Mathf.Cos(GocGioiHan * Mathf.Deg2Rad) * dis) // trong goc gioi han
//                {
//                    if (closest <= 0 || closest > dot)
//                    {
//                        newTarget = e;
//                    }
//                }
//            }
//            if (newTarget != unitTarget)
//            {
//                unitTarget = newTarget;
//#if DEBUG
//                //Hiker.GUI.Shootero.ScreenBattle.instance.DisplayTextHud("OnTarget", unitTarget);
//#endif
//            }

//            if (unitTarget == null)
//            {
//                mTickCheckTarget = 0f;
//            }

//            Hiker.Util.ListPool<DonViChienDau>.Release(listEnemies);
//        }
//        else
//        {
//            mTickCheckTarget -= Time.deltaTime;
//        }

        if (unitTarget != null && unitTarget.IsAlive())
        {
            var dif = unitTarget.transform.position - transform.position;
            //dif.y = 0;
            dif.y = transform.forward.y;

            if (GocGioiHan < 180)
            {
                float cosA = Vector3.Dot(dif.normalized, transform.forward);
                float cosB = Mathf.Cos(GocGioiHan * Mathf.Deg2Rad);
                if (cosA < cosB)
                {
#if DEBUG
                    //Hiker.GUI.Shootero.ScreenBattle.instance.DisplayTextHud("OffTarget", unitTarget);
                    //Debug.Log("OffTarget");
#endif
                    unitTarget = null;
                    return;
                }
            }

            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(dif.normalized, transform.up),
                DoNhay * Time.deltaTime);
        }
    }
}
