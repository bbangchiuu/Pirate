using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;

    [TaskDescription("Necroposh Spawn Objects")]
    [TaskCategory("Shootero")]
    public class NecrophosSpawnObjectAction : Action
    {
        public int startCount = 4;
        public int incrCount = 2;
        public int endCount = 10;

        private int count = 1;
        public bool randomPos = false;
        public SharedTransformList spawnPoints;
        public bool randomAround = false;
        public SharedTransform target;
        public float minRange = 2;
        public float maxRange = 4;
        public int maxCount = 0;
        public SharedGameObject prefab;
        public SharedGameObjectList listObjects;

        public bool ShowSpawnEff = true;
        public bool UseDefaultRotation = false;
             

        //NavMeshAgent agent;
        //NavMeshHit hit;
        //bool haveTarget;
        //float timeStuck = 0;

        public override void OnAwake()
        {
            base.OnAwake();
            count = startCount;
        }

        public override void OnStart()
        {
            //haveTarget = false;
            //float checkRange = Mathf.Max(0.5f, Mathf.Min(randomRange - minRange, maxRange - randomRange));
            //NavMeshQueryFilter filter = new NavMeshQueryFilter();
            //filter.agentTypeID = agent.agentTypeID;
            //filter.areaMask = NavMesh.AllAreas;

            //if (NavMesh.SamplePosition(transform.position + dirRandom * randomRange, out hit, checkRange, filter))
            //{
            //    haveTarget = true;
            //}
            //timeStuck = 0;
        }

        //IEnumerator DelaySpawnAt(Vector3 pos, Vector3 eul)
        //{
        //    if (ShowSpawnEff)
        //    {

        //        // effect
        //        GameObject eff = ObjectPoolManager.Spawn("Particle/Other/WaveSummonImpact");
        //        eff.transform.position = pos;
        //        ObjectPoolManager.instance.AutoUnSpawn(eff, 3.5f);
        //    }

        //    yield return new WaitForSeconds(2);

        //    var go = GameObject.Instantiate(prefab.Value);
        //    go.transform.position = pos;
        //    go.transform.eulerAngles = eul;
        //    go.SetActive(true);
        //}

        IEnumerator DelaySpawnRandomAround(int index)
        {
            var trans = target != null ? target.Value : null;
            if (trans == null)
            {
                trans = transform;
            }

            var randomRange = Random.Range(minRange, maxRange);
            var dirRandom = Quaternion.Euler(0, Random.Range(-180, 180), 0) * Vector3.forward;

            var targetPos = trans.position + dirRandom * randomRange;

            if (target != null)
            {
                if (UnityEngine.AI.NavMesh.SamplePosition(targetPos,
                        out UnityEngine.AI.NavMeshHit hit,
                        0.3f,
                        UnityEngine.AI.NavMesh.AllAreas) == false)
                {
                    var SourceUnit = this.gameObject;
                    var sourcePos = trans.position;
                    if (SourceUnit != null && SourceUnit.transform != null)
                        sourcePos = SourceUnit.transform.position;

                    sourcePos.y = trans.position.y;
                    if (UnityEngine.AI.NavMesh.Raycast(sourcePos, targetPos, out hit, UnityEngine.AI.NavMesh.AllAreas))
                    {
                        targetPos = hit.position;
                        targetPos.y = trans.position.y;
                    }
                }
            }

            if (ShowSpawnEff)
            {

                // effect
                GameObject eff = ObjectPoolManager.Spawn("Particle/Other/WaveSummonImpact");
                eff.transform.position = targetPos;
                ObjectPoolManager.instance.AutoUnSpawn(eff, 3.5f);
            }

            yield return new WaitForSeconds(2);

            var go = GameObject.Instantiate(prefab.Value);
            go.transform.position = targetPos;
            go.SetActive(true);

            //return go;
        }

        GameObject SpawnFromSpawnPoints(int index)
        {
            var trans = target != null ? target.Value : null;
            if (trans == null)
            {
                trans = transform;
            }

            Vector3 pos = trans.position;

            Transform spawnP = null;

            if (spawnPoints.Value.Count > 0)
            {
                if (randomPos)
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

            var go = GameObject.Instantiate(prefab.Value);
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
                    //for (int i = listObjects.Value.Count; i < maxCount; ++i)
                    //{
                    //    if (randomAround)
                    //    {
                    //        listObjects.Value.Add(SpawnRandomAround(i));
                    //    }
                    //    else
                    //    {
                    //        listObjects.Value.Add(SpawnFromSpawnPoints(i));
                    //    }
                    //}
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
                    if (randomAround)
                    {
                        //spawn_obj = SpawnRandomAround(i);
                        StartCoroutine(DelaySpawnRandomAround(i));
                    }
                    else
                    {
                        spawn_obj = SpawnFromSpawnPoints(i);
                    }

                    if (listObjects.Value != null)
                    {
                        listObjects.Value.Add(spawn_obj);
                    }
                }

                count += incrCount;
                if (count > endCount)
                    count = endCount;
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
