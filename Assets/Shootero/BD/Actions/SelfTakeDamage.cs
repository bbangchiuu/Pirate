using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("GetMainPlayer")]
    [TaskCategory("Shootero")]
    public class SelfTakeDamage : Action
    {
        public SharedTransform player;
        public SharedGameObject playerGO;
        public long dmg;

        DonViChienDau unit;

        public override void OnAwake()
        {
            base.OnAwake();
            unit = gameObject.GetComponent<DonViChienDau>();
        }

        public override void OnStart()
        {
            unit.TakeDamage(dmg,false,null,false,false);
        }
        
        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
        }
    }
}