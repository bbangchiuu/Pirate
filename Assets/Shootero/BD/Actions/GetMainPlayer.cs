using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("GetMainPlayer")]
    [TaskCategory("Shootero")]
    public class GetMainPlayer : Action
    {
        public SharedTransform player;
        public SharedGameObject playerGO;

        public override void OnAwake()
        {
            base.OnAwake();
        }
        public override void OnStart()
        {
            
        }

        public override TaskStatus OnUpdate()
        {
            if (QuanlyManchoi.instance == null) return TaskStatus.Failure;
            if (QuanlyManchoi.instance.PlayerUnit == null) return TaskStatus.Failure;
            player.SetValue(QuanlyManchoi.instance.PlayerUnit.transform);
            playerGO.SetValue(QuanlyManchoi.instance.PlayerUnit.gameObject);

            if (player.Value != null)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
        }
    }
}