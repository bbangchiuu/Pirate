using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuanlyManChoiSanThe : MonoBehaviour
{
    public SanTheSpawnPoint[] SampleWaves1;
    public SanTheSpawnPoint[] SampleWaves2;
    public SanTheSpawnPoint[] SampleWaves3;
    public SanTheSpawnPoint[] SampleWaves4;

    public static QuanlyManChoiSanThe instance;
    public int CurRound { get; private set; }
    private void Awake()
    {
        instance = this;
    }

    public void SpawnRound(int round)
    {
        if (QuanlyNguoichoi.Instance == null ||
            QuanlyNguoichoi.Instance.battleData == null ||
            QuanlyNguoichoi.Instance.battleData.SanThe == null ||
            QuanlyNguoichoi.Instance.battleData.SanThe.SpawnInfo == null)
            return;

        switch (round)
        {
            case 1:
                CurRound = round;
                StartCoroutine(CoSpawn(SampleWaves1, QuanlyNguoichoi.Instance.battleData.SanThe.SpawnInfo.Wave1));
                break;
            case 2:
                CurRound = round;
                StartCoroutine(CoSpawn(SampleWaves2, QuanlyNguoichoi.Instance.battleData.SanThe.SpawnInfo.Wave2));
                break;
            case 3:
                CurRound = round;
                StartCoroutine(CoSpawn(SampleWaves3, QuanlyNguoichoi.Instance.battleData.SanThe.SpawnInfo.Wave3));
                break;
            case 4:
                CurRound = round;
                StartCoroutine(CoSpawn(SampleWaves4, QuanlyNguoichoi.Instance.battleData.SanThe.SpawnInfo.Wave4));
                break;
            default:
                break;
        }
    }

    bool spawning = false;
    IEnumerator CoSpawn(SanTheSpawnPoint[] waves, string[] units)
    {
        while (spawning)
        {
            yield return null;
        }

        spawning = true;
        var idx = Random.Range(0, waves.Length);
        var spawnPoint = waves[idx];

        if (units.Length > 0)
        {
            SpawnIndicator(spawnPoint.spawnPoints1);
        }
        if (units.Length > 1)
        {
            SpawnIndicator(spawnPoint.spawnPoints2);
        }
        yield return new WaitForSeconds(2f);
        
        if (units.Length > 0)
        {
            SpawnUnit(spawnPoint.spawnPoints1, units[0]);
        }

        if (units.Length > 1)
        {
            SpawnUnit(spawnPoint.spawnPoints2, units[1]);
        }

        spawning = false;
    }

    void SpawnIndicator(Transform[] spawnPoints)
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            var p = spawnPoints[i];
            Vector3 _pos = new Vector3(p.transform.position.x, 0.1f, p.transform.position.z);

            GameObject indicator = ObjectPoolManager.Spawn("Particle/Other/WaveSummonImpact");
            ObjectPoolManager.instance.AutoUnSpawn(indicator, 3.5f);
            indicator.transform.position = _pos;
            indicator.SetActive(true);
        }
    }

    //void SpawnPoint(SanTheSpawnPoint[] waves, string[] units)
    //{
    //    var idx = Random.Range(0, waves.Length);
    //    var spawnPoint = waves[idx];
    //    if (units.Length > 0)
    //    {
    //        SpawnUnit(spawnPoint.spawnPoints1, units[0]);
    //    }

    //    if (units.Length > 1)
    //    {
    //        SpawnUnit(spawnPoint.spawnPoints2, units[1]);
    //    }
    //}

    void SpawnUnit(Transform[] spawnPoints, string unit)
    {
        var res = Resources.Load<GameObject>("AIPrefabs/" + unit);
        if (res != null)
        {
            for (int i = 0; i < spawnPoints.Length; ++i)
            {
                var p = spawnPoints[i];
                var unitObj = Instantiate(res, p.position, p.rotation);
                if (unitObj.gameObject.activeSelf == false)
                {
                    unitObj.gameObject.SetActive(true);
                }
            }
        }
    }
}
