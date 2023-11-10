using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;

    [TaskDescription("AIOrcKingCallAxe")]
    [TaskCategory("Shootero")]
    public class AIOrcKingCallAxe : Action
    {
        NavMeshAgent agent;
        DonViChienDau unit;

        public override void OnAwake()
        {
            base.OnAwake();
            unit = gameObject.GetComponent<DonViChienDau>();
        }

        public override void OnStart()
        {
            VuKhiTieuPhu.RecallAxe(unit);
        }

        public override TaskStatus OnUpdate()
        {
            bool allRecall = true;
            for (int i = VuKhiTieuPhu.ListRiuActive.Count - 1; i >= 0; --i)
            {
                var proj = VuKhiTieuPhu.ListRiuActive[i];
                if (proj != null && proj.SourceUnit == unit)
                {
                    if (proj.gameObject.activeInHierarchy)
                    {
                        allRecall = false;
                    }
                }
            }

            if (allRecall)
                return TaskStatus.Success;
            else
            {
                return TaskStatus.Running;
            }
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
            
        }

        public override void OnEnd()
        {
        }
    }
}