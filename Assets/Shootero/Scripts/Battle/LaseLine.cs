using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaseLine : MonoBehaviour
{
    public LayerMask mask;
    LineRenderer line;
    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }
    private void OnEnable()
    {
        if (mask.value == LayersMan.Team1LegacyHitLayerMask)
        {
            mask.value = LayersMan.Team1HitLayerMask;
        }
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100, mask))
        {
            SetTarget(hit.point);
        }
        else
        {
            SetTarget(transform.position + transform.forward * 100);
        }
    }

    public void SetTarget(Vector3 p)
    {
        var localPoint = line.transform.InverseTransformPoint(p);
        line.SetPosition(1, localPoint);
    }
}
