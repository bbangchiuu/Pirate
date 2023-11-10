using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlazeLine : MonoBehaviour
{
    public BlazeLine syncTarget;
    public LayerMask mask;
    public bool InitLineWhenEnable;
    [SerializeField][HideInInspector]
    Vector3 localTarget;
    LineRenderer line;

    public Vector3 LocalTarget { get { return localTarget; } }
    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        mask = LayersMan.Team1HitLayerMask;
    }

    private void OnEnable()
    {
        if (InitLineWhenEnable)
        {
            var playerPos = QuanlyNguoichoi.Instance.PlayerUnit.transform.position;
            playerPos.y = transform.position.y;
            SetTarget(playerPos);
            //RaycastHit hit;
            //if (Physics.Raycast(transform.position, transform.forward, out hit, 100, mask))
            //{
            //    SetTarget(hit.point);
            //}
            //else
            //{
            //    SetTarget(transform.position + transform.forward * 100);
            //}
        }
    }

    private void Update()
    {
        //RaycastHit hit;
        //if (Physics.Raycast(transform.position, transform.forward, out hit, 100, mask))
        //{
        //    SetTarget(hit.point);
        //}
        //else
        //{
        //    SetTarget(transform.position + transform.forward * 100);
        //}
    }

    public void SetLocalTarget(Vector3 p)
    {
        if (line == null)
        {
            line = GetComponent<LineRenderer>();
        }
        line.SetPosition(1, p);
        localTarget = p;
    }

    public void SetTarget(Vector3 p)
    {
        if (line == null)
        {
            line = GetComponent<LineRenderer>();
        }
        
        var localPoint = line.transform.InverseTransformPoint(p);
        line.SetPosition(1, localPoint);
        localTarget = localPoint;
        if (syncTarget != null)
        {
            syncTarget.SetTarget(p);
        }
    }
}
