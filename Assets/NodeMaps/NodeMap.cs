using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMap : MonoBehaviour
{
    //[HideInInspector]
    public List<NodePoint> points = new List<NodePoint>();
    [HideInInspector]
    public List<NodeConnection> connections = new List<NodeConnection>();

    public bool ClearNodes()
    {
        connections.Clear();
        bool changed = false;
        for (int i = points.Count - 1; i >= 0; --i)
        {
            var node = points[i];
            if (node == null)
            {
                points.RemoveAt(i);
                changed = true;
            }
        }

        return changed;
    }
}
