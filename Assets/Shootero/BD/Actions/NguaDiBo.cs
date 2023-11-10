using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks
{
    using UnityEngine.AI;

    [TaskDescription("Ngua Di Bo")]
    [TaskCategory("Shootero")]
    public class NguaDiBo : Action
    {
        public SharedTransform player;
        public SharedTransform forwardTrans;
        public float walkSpeed;
        public SharedBool aimPlayer;

        NavMeshAgent agent;
        DonViChienDau unit;
        bool rotateRight = true;
        float degree = 0;
        bool updateRotation;

        public override void OnAwake()
        {
            base.OnAwake();

            agent = gameObject.GetComponent<NavMeshAgent>();
            unit = gameObject.GetComponent<DonViChienDau>();
        }

        public override void OnStart()
        {
            base.OnStart();
            if (agent && agent.hasPath)
            {
                agent.ResetPath();
            }
            updateRotation = agent.updateRotation;
            agent.updateRotation = false;

            rotateRight = Random.Range(0, 2) < 1;
            degree = Random.Range(60f, 90f);
        }

        public override TaskStatus OnUpdate()
        {
            if (agent == null) return TaskStatus.Failure;
            if (agent.isOnNavMesh == false) return TaskStatus.Failure;

            agent.speed = walkSpeed * unit.TimeScale;
            var dir = transform.forward;
            var targetRotation = rotateRight ? transform.right : -transform.right;
            var d = agent.angularSpeed * Time.deltaTime;
            degree -= d;
            var newDir = Vector3.RotateTowards(dir, targetRotation, d * Mathf.Deg2Rad, 0f);
            NavMeshHit hit;
            
            if (agent.Raycast(transform.position + newDir * 1f, out hit))
            {
                //rotateRight = !rotateRight;
                //degree = Random.Range(30f, 90f);
                //var refleact = Vector3.Reflect(newDir, hit.normal);
                //newDir = newDir + refleact;
                newDir = Vector3.RotateTowards(dir, targetRotation, 3 * d * Mathf.Deg2Rad, 0f); // quay nhanh hon khi cham tuong
            }
            else if (degree < 0)
            {
                rotateRight = Random.Range(0, 2) < 1;
                degree = Random.Range(30f, 90f);
            }

            transform.forward = newDir;
            agent.velocity = newDir * agent.speed;

            return TaskStatus.Running;
        }

        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
            if (aimPlayer.Value && player.Value != null)
            {
                unit.shooter.RotateToTarget(player.Value);
            }
            else if (forwardTrans.Value != null)
            {
                unit.shooter.RotateToTarget(forwardTrans.Value);
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();
            if (agent)
                agent.updateRotation = updateRotation;
        }
    }
}