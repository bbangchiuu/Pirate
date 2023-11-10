using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;

    [TaskDescription("Spawn Random Objects at spawnpoints")]
    [TaskCategory("Shootero")]
    public class SpawnRandomObj : Action
    {
        public int count = 1;
        public bool randomPoint = false;
        public SharedTransformList spawnPoints;
        public SharedGameObjectList prefabRandomPools;
        public SharedGameObjectList listObjects;
        public int maxCount = 0;

        public bool ShowSpawnEff = true;
        public bool UseDefaultRotation = false;

        GameObject SpawnFromSpawnPoints(int index)
        {
            if (prefabRandomPools.Value == null) return null;
            var trans = transform;

            Vector3 pos = trans.position;

            Transform spawnP = null;

            if (spawnPoints.Value.Count > 0)
            {
                if (randomPoint)
                {
                    spawnP = spawnPoints.Value[Random.Range(0, spawnPoints.Value.Count)];
                    pos = spawnP.position;
                }
                else
                {
                    spawnP = spawnPoints.Value[index % spawnPoints.Value.Count];
                    pos = spawnP.position;
                }
            }

            int r = Random.Range(0, prefabRandomPools.Value.Count);
            var prefab = prefabRandomPools.Value[r];
            var go = GameObject.Instantiate(prefab);
            go.transform.position = pos;

            if (UseDefaultRotation)
            {
            }
            else
            {
                if (spawnP != null)
                    go.transform.LookAt(pos + spawnP.forward, Vector3.up);
            }

            go.SetActive(true);

            if (ShowSpawnEff)
            {

                // effect
                GameObject eff = ObjectPoolManager.Spawn("Particle/Other/SummonImpact");
                eff.transform.position = pos;
                ObjectPoolManager.instance.AutoUnSpawn(eff, 4);
            }
            return go;
        }

        public override TaskStatus OnUpdate()
        {
            if (listObjects.Value != null)
            {
                for (int i = listObjects.Value.Count - 1; i >= 0; --i)
                {
                    var go = listObjects.Value[i];
                    if (go == null || go.activeInHierarchy == false)
                        listObjects.Value.RemoveAt(i);
                }
            }

            if (maxCount > 0 && listObjects.Value != null)
            {
                if (listObjects.Value.Count < maxCount)
                {
                    for (int i = listObjects.Value.Count; i < maxCount; ++i)
                    {
                        //if (randomAround)
                        //{
                        //    listObjects.Value.Add(SpawnRandomAround(i));
                        //}
                        //else
                        {
                            listObjects.Value.Add(SpawnFromSpawnPoints(i));
                        }
                    }
                }
                else
                {
                    return TaskStatus.Failure;
                }
            }
            else
            {
                for (int i = 0; i < count; ++i)
                {
                    GameObject spawn_obj = null;
                    //if (randomAround)
                    //{
                    //    spawn_obj = SpawnRandomAround(i);
                    //}
                    //else
                    {
                        spawn_obj = SpawnFromSpawnPoints(i);
                    }

                    if (listObjects.Value != null)
                    {
                        listObjects.Value.Add(spawn_obj);
                    }
                }
            }

            return TaskStatus.Success;
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
        }
    }
}
