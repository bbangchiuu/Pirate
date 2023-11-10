using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;

    [TaskDescription("Jack Spawn Objects")]
    [TaskCategory("Shootero")]
    public class JackSpawnObjectAction : Action
    {
        public int startCount = 3;
        public int count = 1;
        public int maxCount = 0;
        public bool randomPos = false;
        public SharedTransformList spawnPoints;
        public bool randomAround = false;
        public SharedTransform target;
        public float minRange = 2;
        public float maxRange = 4;
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

        GameObject SpawnRandomAround(int index)
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

            var go = GameObject.Instantiate(prefab.Value);
            go.transform.position = targetPos;
            go.SetActive(true);

            if (ShowSpawnEff)
            {
                // effect
                GameObject eff = ObjectPoolManager.Spawn("Particle/Other/SummonImpact");
                eff.transform.position = targetPos;
                ObjectPoolManager.instance.AutoUnSpawn(eff, 4);
            }

            return go;
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
                if (listObjects.Value.Count == 0) // first time
                {
                    for (int i = 0; i < startCount; i++)
                    {
                        GameObject obj = SpawnRandomAround(0);
                        listObjects.Value.Add(obj);
                    }
                }
                else
                {
                    if (listObjects.Value.Count < maxCount)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            GameObject obj = SpawnRandomAround(0);
                            listObjects.Value.Add(obj);
                        }
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
