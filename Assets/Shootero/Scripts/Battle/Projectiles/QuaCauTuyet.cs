using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuaCauTuyet : SatThuongDT
{
    public GameObject Ball;
    public GameObject Indicator;
    public SatThuongDT dmgObj;
    public float delayTime = 0.6f;

    public SatThuongDT SpawnOnDestroy = null;
    public float SpawnObjSpeed = 0;

    private void OnEnable()
    {
        dmgObj.gameObject.SetActive(false);
        Indicator.SetActive(true);
        Ball.SetActive(true);
    }

    public override void ActiveDan(float speed, long dmg, Vector3 target)
    {
        base.ActiveDan(speed, dmg, target);

        Vector3 targetPos = Vector3.zero;

        if (QuanlyManchoi.instance && QuanlyManchoi.instance.PlayerUnit)
        {
            target = QuanlyManchoi.instance.PlayerUnit.transform.position;
            
            Vector2 random_range = Random.insideUnitCircle * 10;
            Vector3 _r;
            _r.x = random_range.x;
            _r.z = random_range.y;
            _r.y = 0;
            targetPos = target + _r;

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
                    targetPos.y = target.y;
                }
            }

            transform.position = targetPos;
        }

        if (Ball)
        {
            Vector3 ball_pos = Vector3.zero;
            ball_pos.y = 15;
            Ball.transform.localPosition = ball_pos;
            ball_pos.y = 0;
            var ts = TweenPosition.Begin(Ball, delayTime, ball_pos);
            ts.ignoreTimeScale = false;
        }

        if (dmgObj)
        {
            Hiker.HikerUtils.DoAction(this,
                () => {
                    if (Indicator)
                        Indicator.SetActive(false);
                    if (Ball)
                        Ball.SetActive(false);
                    if (dmgObj)
                    {
                        dmgObj.gameObject.SetActive(true);
                        dmgObj.ActiveDan(0, Damage, Vector3.zero);
                    }
                    
                    if (SpawnOnDestroy)
                    {
                        int SpawnOnDestroyCount = 4;

                        float rand_y = Random.Range(0, 360);
                        float degree_step = 360 / SpawnOnDestroyCount;
                        for (int i = 0; i < SpawnOnDestroyCount; i++)
                        {
                            var projObj = ObjectPoolManager.Spawn(SpawnOnDestroy.gameObject);
                            var proj = projObj.GetComponent<SatThuongDT>(); // Instantiate(SpawnOnDestroy);
                            proj.transform.eulerAngles = new Vector3(0, rand_y, 0);
                            proj.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
                            proj.ActiveDan(SpawnObjSpeed, Damage, Vector3.one);
                            proj.SourceUnit = SourceUnit;
                            proj.gameObject.SetActive(true);

                            ApplyScaleDanTo(projObj);
                            //if (ScaleRate > 0 && ScaleRate != 1f)
                            //{
                            //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale * ScaleRate;
                            //}
                            //else
                            //{
                            //    proj.transform.localScale = SpawnOnDestroy.transform.lossyScale;
                            //}

                            proj.ScaleRate = ScaleRate;

                            rand_y += degree_step;
                        }
                    }
                },
                delayTime);
        }

        //ObjectPoolManager.Unspawn(gameObject, delayTime + 0.15f);
        ObjectPoolManager.instance.AutoUnSpawn(gameObject, delayTime + 0.65f);
    }

    protected override void OnDeactiveDan()
    {
        base.OnDeactiveDan();
    }
}
