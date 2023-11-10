using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePoint : MonoBehaviour
{
    public List<NodeConnection> connections = new List<NodeConnection>();

    private void OnDrawGizmos()
    {
        //Gizmos.DrawIcon(transform.position, "dot.png", false);
        Gizmos.color = Color.yellow;
        foreach (var c in connections)
        {
            if (c.startPoint != null && c.endPoint != null)
            {
                Gizmos.DrawLine(c.startPoint.transform.position, c.endPoint.transform.position);
            }
        }
    }
}

[System.Serializable]
public class NodeConnection
{
    public NodePoint startPoint;
    public NodePoint endPoint;
}