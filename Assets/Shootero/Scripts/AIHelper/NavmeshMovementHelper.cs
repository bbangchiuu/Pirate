using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshMovementHelper : MonoBehaviour
{
    NavMeshAgent mAgent;

    public NavMeshAgent GetAgent() { if (mAgent == null) mAgent = GetComponent<NavMeshAgent>(); return mAgent; }

    public Vector3 GetRandomInRange(float radius)
    {
        var vec2 = Random.insideUnitCircle * radius;
        var newPos = transform.position + new Vector3(vec2.x, 0.2f, vec2.y);
        return newPos;
    }

    public bool IsReachDestination()
    {
        var agent = GetAgent();
        if (agent != null)
        {
            if (Vector3.SqrMagnitude(agent.destination - agent.nextPosition) < agent.stoppingDistance * agent.stoppingDistance)
            {
                return true;
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    public bool IsStoppedOnPath()
    {
        var agent = GetAgent();
        if (agent != null)
        {
            if (agent.hasPath &&
                Vector3.SqrMagnitude(agent.pathEndPosition - agent.nextPosition) < agent.stoppingDistance * agent.stoppingDistance)
            {
                return true;
            }
            else
            {
                return true;
            }
        }
        return false;
    }
}
