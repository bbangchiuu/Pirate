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

public class ChuongReDat : SatThuongDT
{
    [SerializeField]
    Float splitDuration = 1f;
    public float SplitTime = 0.2f;

    public float SplitCount = 4;

    public float SplitOffset = 3;

    public GameObject SplitEff;

    private void OnEnable()
    {
        //transform.localScale = new Vector3(1, 1, 0);
        //TweenScale.Begin(gameObject, SplitTime, Vector3.one);

        for(int i=0;i< SplitCount;i++)
        {            
            StartCoroutine("SpawnSplit",i);
        }            
    }

    IEnumerator SpawnSplit(int i)
    {
        float t = SplitTime * i;
        float pos_offset = SplitOffset * (i+1);

        yield return new WaitForSeconds(t);        

        GameObject split = ObjectPoolManager.SpawnAutoUnSpawn(SplitEff, 0.3f);

        split.transform.position = this.transform.position + this.transform.forward* pos_offset;
    }

    protected override void Update()
    {
        if (LifeTime > 0)
        {
            LifeTime -= Time.deltaTime;
        }
        else
        {
            mIsActive = false;
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    protected override bool OnCollisionOther(GameObject other)
    {
        var unit = other.GetComponentInParent<DonViChienDau>();
        if (unit != null)
        {
            if (unit.IsAlive() == false) return false;

            unit.TakeDamage(Damage);
            if (KnockBackDistance > 0)
            {
                var vec = transform.forward;
                vec.y = 0;
                unit.KnockBack(vec.normalized * KnockBackDistance);
            }
            mIsActive = false;
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
}
