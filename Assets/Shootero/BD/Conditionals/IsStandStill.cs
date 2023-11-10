using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Return success when found an alive enemy target")]
    [TaskCategory("Shootero")]
    public class IsStandStill : Conditional
    {
        public SharedGameObject go;
        PlayerMovement playerMovement;

        public override void OnAwake()
        {
            if (go != null && go.Value != null)
            {
                playerMovement = go.Value.GetComponent<PlayerMovement>();
            }
            else
            {
                playerMovement = gameObject.GetComponent<PlayerMovement>();
            }
        }

        public override void OnStart()
        {
            base.OnStart();
            if (playerMovement == null)
            {
                if (go != null && go.Value != null)
                {
                    playerMovement = go.Value.GetComponent<PlayerMovement>();
                }
                else
                {
                    playerMovement = gameObject.GetComponent<PlayerMovement>();
                }
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (playerMovement != null && playerMovement.LastDirection.sqrMagnitude <= Vector2.kEpsilonNormalSqrt)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }

        public override void OnEnd()
        {
        }

        public override void OnReset()
        {

        }
    }
}
