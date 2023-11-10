using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapExplorer : MonoBehaviour
{
    public NodeMap map;

    NodePoint currentNode;

    private void Start()
    {
        if (currentNode == null && map != null)
        {
            currentNode = map.points[0];
            transform.position = currentNode.transform.position;
        }
    }

    public List<NodePoint> GetListAvailableNode()
    {
        List<NodePoint> listNodes = new List<NodePoint>();
        foreach (var c in currentNode.connections)
        {
            listNodes.Add(c.endPoint);
        }
        return listNodes;
    }

    public bool GoToNode(NodePoint node)
    {
        if (CanGoNode(node))
        {
            SetCurNode(node);
            return true;
        }
        return false;
    }

    public bool CanGoNode(NodePoint node)
    {
        foreach (var c in currentNode.connections)
        {
            if (c.endPoint == node)
            {
                return true;
            }
        }
        return false;
    }

    public void WrapToNode(NodePoint node)
    {
        SetCurNode(node);
    }

    void SetCurNode(NodePoint node)
    {
        currentNode = node;
        transform.position = currentNode.transform.position;
        var nodeCfg = currentNode.GetComponent<NodeLevelConfig>();
        if (nodeCfg)
        {
            if (nodeCfg.LevelType == ENodeType.Start)
            {
                transform.localScale = Vector3.one * 1.2f;
            }
            else if (nodeCfg.LevelType == ENodeType.Boss)
            {
                transform.localScale = Vector3.one * 1.3f;
            }
            else
            {
                transform.localScale = Vector3.one;
            }
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            var curNodes = GetListAvailableNode();
            if (curNodes.Count > 0)
            {
                var r = Random.Range(0, curNodes.Count);
                GoToNode(curNodes[r]);
            }
        }
#endif
        if (currentNode != null && transform.position != currentNode.transform.position)
        {
            transform.position = currentNode.transform.position;
        }
    }
}
