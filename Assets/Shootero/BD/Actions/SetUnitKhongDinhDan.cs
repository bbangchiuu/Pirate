using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;

    [TaskDescription("SetUnitKhongDinhDan")]
    [TaskCategory("Shootero")]
    public class SetUnitKhongDinhDan : Action
    {
        DonViChienDau unit;
        public float duration;

        public SharedGameObject SkillEff = null;

        public override void OnAwake()
        {
            base.OnAwake();
            unit = gameObject.GetComponent<DonViChienDau>();
        }

        public override void OnStart()
        {
            if (unit)
            {
                unit.SetKhongDinhDan(duration);
            }

            if(SkillEff != null && SkillEff.Value != null)
            {
                GameObject eff= ObjectPoolManager.SpawnAutoUnSpawn(SkillEff.Value, 3f);
                eff.transform.position = transform.position;
            }
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }

        public override void OnEnd()
        {
            base.OnEnd();
        }
    }
}
