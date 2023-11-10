using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FightingNPC : MonoBehaviour
{
    public GameObject NPCVisual;
    public DonViChienDau prefabUnit;
    public int Num;
    public SpawnIndicator spawnIndicator;
    public static FightingNPC instance;

    private void OnEnable()
    {
        instance = this;
        if (NPCVisual) NPCVisual.gameObject.SetActive(true);
    }

    Vector3 GetRandomPosInMap()
    {
        var dir = Random.insideUnitCircle;
        var posSample = transform.position + new Vector3(dir.x, 0, dir.y) * 30;
        NavMeshQueryFilter query = new NavMeshQueryFilter();
        query.agentTypeID = prefabUnit.GetComponent<NavMeshAgent>().agentTypeID;
        query.areaMask = NavMesh.AllAreas;
        if (NavMesh.Raycast(transform.position, posSample, out NavMeshHit hit, query))
        {
            posSample = hit.position;
        }

        float dis = Random.Range(0, Vector3.Distance(transform.position, posSample));
        return transform.position + (posSample - transform.position).normalized * dis;
    }

    public void SpawnBots()
    {
        StartCoroutine(CoSpawnBots(2f));
    }

    IEnumerator CoSpawnBots(float delay)
    {
        List<Vector3> randPos = new List<Vector3>();

        for (int i = 0; i < Num; ++i)
        {
            var pos = GetRandomPosInMap();

            //if (spawnIndicator)
            //{
            //    var indicator = Instantiate(spawnIndicator, pos, Quaternion.identity);
            //    indicator.gameObject.SetActive(true);
            //}
            GameObject indicator = ObjectPoolManager.Spawn("Particle/Other/WaveSummonImpact");
            ObjectPoolManager.instance.AutoUnSpawn(indicator, 3.5f);
            indicator.transform.position = pos;
            indicator.SetActive(true);


            randPos.Add(pos);
        }

        if (NPCVisual) NPCVisual.gameObject.SetActive(false);

        yield return new WaitForSeconds(delay);

        for (int i = 0; i < Num; ++i)
        {
            var pos = randPos[i];

            if (spawnIndicator)
            {
                Instantiate(prefabUnit, pos, Quaternion.Euler(0, Random.Range(-180, 180), 0));
            }
        }
    }
}
