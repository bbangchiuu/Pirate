using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMoveDemo : MonoBehaviour
{
    NavMeshAgent agent;
    Camera mainCam;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!agent.isOnNavMesh)
        {
            agent.Warp(transform.position);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
               
        }
    }
}
