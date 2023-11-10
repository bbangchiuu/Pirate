using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;

    [TaskDescription("Rabbit Spawn")]
    [TaskCategory("Shootero")]
    public class RabbitSpawnAction : Action
    {
        public SharedGameObject prefab;

        public bool ShowSpawnEff = true;
        public bool UseDefaultRotation = false;

        public bool IsCoopRabbit = false;

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
            List<Transform> list1,list2;
            int _r1 = Random.Range(0, 4);
            int _r2 = Random.Range(0, 4);

            while (_r2==_r1)
            {
                _r2 = Random.Range(0, 4);
            }

            Vector3 start_pos = Vector3.zero;
            Vector3 offset = Vector3.zero;
            Vector3 eul = Vector3.zero;
            int count = 0;

            if (_r1 == 0)
            {
                start_pos = new Vector3(-9, 0, -2);
                offset = new Vector3(0, 0, 3);
                eul = new Vector3(0, 90, 0);
                count = 9;
            }
            else if (_r1 == 1)
            {
                start_pos = new Vector3(9, 0, -2);
                offset = new Vector3(0, 0, 3);
                eul = new Vector3(0, -90, 0);
                count = 9;
            }
            else if (_r1 == 2)
            {
                start_pos = new Vector3(-9, 0, 22);
                offset = new Vector3(3, 0, 0);
                eul = new Vector3(0, -180, 0);
                count = 7;
            }
            else if (_r1 == 3)
            {
                start_pos = new Vector3(-9, 0, -2);
                offset = new Vector3(3, 0, 0);
                eul = new Vector3(0, 0, 0);
                count = 7;
            }


            for (int i = 0; i < count; i++)
            {
                Vector3 pos = start_pos + offset * i;
                SpawnAt(pos, eul);
            }

            if (IsCoopRabbit)
                return;

            if (_r2 == 0)
            {
                start_pos = new Vector3(-9, 0, -2);
                offset = new Vector3(0, 0, 3);
                eul = new Vector3(0, 90, 0);
                count = 9;
            }
            else if (_r2 == 1)
            {
                start_pos = new Vector3(9, 0, -2);
                offset = new Vector3(0, 0, 3);
                eul = new Vector3(0, -90, 0);
                count = 9;
            }
            else if (_r2 == 2)
            {
                start_pos = new Vector3(-9, 0, 22);
                offset = new Vector3(3, 0, 0);
                eul = new Vector3(0, -180, 0);
                count = 7;
            }
            else if (_r2 == 3)
            {
                start_pos = new Vector3(-9, 0, -2);
                offset = new Vector3(3, 0, 0);
                eul = new Vector3(0, 0, 0);
                count = 7;
            }

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = start_pos + offset * i;
                SpawnAt(pos, eul);
            }

        }

        IEnumerator DelaySpawnAt(Vector3 pos, Vector3 eul)
        {
            if (ShowSpawnEff)
            {

                // effect
                GameObject eff = ObjectPoolManager.Spawn("Particle/Other/WaveSummonImpact");
                eff.transform.position = pos;
                ObjectPoolManager.instance.AutoUnSpawn(eff, 3.5f);
            }

            yield return new WaitForSeconds(2);

            var go = GameObject.Instantiate(prefab.Value);
            go.transform.position = pos;
            go.transform.eulerAngles = eul;
            go.SetActive(true);
        }

        void SpawnAt(Vector3 pos,Vector3 eul)
        {
            StartCoroutine(DelaySpawnAt(pos, eul));
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
        }
    }
}
