using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hiker.GUI.Shootero;

public class WaveController : MonoBehaviour
{
    //public GameObject spawnIndicator;
    public float WaveTime;
    public GameObject[] Waves;
    public int[] WavesHPRatio;

    public List<DonViChienDau[]> EnemyInWave = new List<DonViChienDau[]>();

    public int CurrentWave = 0;
    float WaveTimer = 0;

    bool IsStarted = false;

    public static WaveController instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        QuanlyManchoi.instance.SetWaveController(this);

        for(int i=0;i < Waves.Length;i++)
        {
            GameObject wi = Waves[i];
            DonViChienDau[] unit_list = wi.GetComponentsInChildren<DonViChienDau>();

            EnemyInWave.Add(unit_list);
        }

        if (QuanlyNguoichoi.Instance.IsLevelClear)
        {
            // auto disable enemy if level is cleared before
            gameObject.SetActive(false);
            CurrentWave = Waves.Length;
            return;
        }
    }

    bool spawning = false;
    IEnumerator SpawnWave(int wave_idx)
    {
        spawning = true;
        DonViChienDau[] unit_list = EnemyInWave[wave_idx];

        for(int i=0;i< unit_list.Length;i++)
        {
            DonViChienDau unit = unit_list[i];
            Vector3 _pos = new Vector3(unit.transform.position.x, 0.1f, unit.transform.position.z);

            //GameObject indicator = GameObject.Instantiate(spawnIndicator, _pos, Quaternion.identity);
            GameObject indicator = ObjectPoolManager.Spawn("Particle/Other/WaveSummonImpact");
            ObjectPoolManager.instance.AutoUnSpawn(indicator, 3.5f);
            indicator.transform.position = _pos;
            indicator.SetActive(true);
        }

        yield return new WaitForSeconds(2f);
        Waves[wave_idx].SetActive(true);
        spawning = false;
    }

    public bool CheckHasNextWave()
    {
        if (CurrentWave >= Waves.Length)
        {
            return false;
        }
        return true;
    }

    public bool SpawnCurrentWave()
    {
        if (spawning) return true;

        if( Waves.Length > CurrentWave)
        {
            StartCoroutine("SpawnWave", CurrentWave);

            CurrentWave++;
            if (CurrentWave >= Waves.Length)
            {
                WaveTimer = 100000;

                ScreenBattle.instance.lblWave.text = string.Empty;
            }
            else
                WaveTimer = WaveTime;

            return true;
        }

        return false;
    }

    bool CheckDoneWave(int wave_idx)
    {
        if (wave_idx == 0) return true;
        if (wave_idx >= Waves.Length) return false;
        DonViChienDau[] unit_list = EnemyInWave[wave_idx];
        for (int i = 0; i < unit_list.Length; i++)
        {
            if (unit_list[i].IsAlive())
            {
                return false;
            }
        }
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsStarted)
        {
            //if(QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.PlayerUnit.transform.position.z > this.transform.position.z)
            if (QuanlyNguoichoi.Instance )
            {
                IsStarted = true;
                SpawnCurrentWave();
            }
            return;
        }


        //WaveTimer -= Time.deltaTime;

        //if( WaveTimer < 0 )
        //{
        //    SpawnCurrentWave();
        //}

        if ( CurrentWave < Waves.Length)
        {
            ScreenBattle.instance.lblWave.text = string.Format(Localization.Get("BattleWaveLabel"), CurrentWave, Waves.Length, (int)(WaveTimer));
        }
    }
}
