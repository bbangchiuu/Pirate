using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeMapVisual : UIBehaviour
{
    public NodeMap map;
    public GameObject nodeVisualPrefab;
    public Sprite dotNorm;
    public Color normColor;
    public Sprite dotPass;
    public Color passColor;

    public RectTransform rectSizeListener;

    public RectTransform ConnectionContainer;
    public RectTransform NodeContainer;

    public List<NodeLevel> ListPoints = new List<NodeLevel>();
    public List<UnityEngine.UI.Extensions.UILineConnector> listConnector = new List<UnityEngine.UI.Extensions.UILineConnector>();
    // Start is called before the first frame update
    public void Init(List<int> revealedNode, int curPos, List<int> curPath, bool showFinalBoss = false)
    {
        map.ClearNodes();

        for (int i = map.points.Count - 1; i >= 0; --i)
        {
            var node = map.points[i];
            for (int k = node.connections.Count - 1; k >= 0; --k)
            {
                var conn = node.connections[k];
                if (conn == null || conn.startPoint == null || conn.endPoint == null)
                {
                    node.connections.RemoveAt(k);
                    map.connections.Remove(conn);
                }
            }

            foreach (var conn in node.connections)
            {
                if (map.connections.Contains(conn) == false)
                {
                    map.connections.Add(conn);
                }
            }
        }

        listConnector.Clear();
        foreach (var conn in map.connections)
        {
            if (conn.startPoint && conn.endPoint)
            {
                GameObject go = new GameObject(
                    string.Format("Conn_{0}_{1}", conn.startPoint.name, conn.endPoint.name));
                go.transform.SetParent(ConnectionContainer, false);
                go.AddComponent<RectTransform>();
                var connector = go.AddComponent<UnityEngine.UI.Extensions.UILineConnector>();
                connector.transforms = new RectTransform[2]
                {
                    conn.startPoint.transform as RectTransform,
                    conn.endPoint.transform as RectTransform,
                };
                listConnector.Add(connector);
                var renderer = connector.GetComponent<UnityEngine.UI.Extensions.UILineRenderer>();

                if (curPath.Exists(e => map.points[e] == conn.endPoint) &&
                    curPath.Exists(e => map.points[e] == conn.startPoint))
                {
                    renderer.sprite = dotPass;
                    renderer.color = passColor;
                }
                else
                {
                    renderer.sprite = dotNorm;
                    renderer.color = normColor;
                }

                renderer.ImproveResolution = UnityEngine.UI.Extensions.ResolutionMode.PerLine;
                renderer.UseNativeSize = true;
            }
        }

        ListPoints.Clear();
        var curNode = map.points[curPos];
        for (int i = 0; i < map.points.Count; ++i)//(var node in map.points)
        {
            var node = map.points[i];
            var visualNode = Instantiate(nodeVisualPrefab, NodeContainer);
            visualNode.transform.position = node.transform.position;
            visualNode.gameObject.SetActive(true);
            var nodeLevel = visualNode.GetComponent<NodeLevel>();
            nodeLevel.Point = node;
            nodeLevel.DungeonId = i;
            ListPoints.Add(nodeLevel);
            var nodeCfg = node.GetComponent<NodeLevelConfig>();
            nodeLevel.SetLevelConfig(nodeCfg);

            if (revealedNode.Contains(i))
            {
                nodeLevel.Reaveal();
            }
            else
            {
                bool isReveal = false;
                foreach (var conn in curNode.connections)
                {
                    if (conn.endPoint == node)
                    {
                        nodeLevel.Reaveal();
                        isReveal = true;
                        break;
                    }
                }
                if (isReveal == false)
                {
                    foreach (var c in revealedNode)
                    {
                        var revNode = map.points[c];

                        foreach (var conn in revNode.connections)
                        {
                            if (conn.endPoint == node)
                            {
                                isReveal = true;
                                break;
                            }
                        }
                        if (isReveal)
                        {
                            nodeLevel.Reaveal();
                            break;
                        }
                    }
                }
                if(showFinalBoss && node.connections.Count == 0)
                {
                    nodeLevel.Reaveal();
                    isReveal = true;
                }

                if (isReveal == false)
                {
                    nodeLevel.UnRevealMask.gameObject.SetActive(true);
                }
            }
        }

        isDirty = true;
    }

    Rect lastRect;
    protected override void Awake()
    {
        lastRect = rectSizeListener.rect;
    }

    private void Update()
    {
        if (isDirty)
        {
            UpdateVisual();
            isDirty = false;
        }
        if (lastRect != rectSizeListener.rect)
        {
            lastRect = rectSizeListener.rect;
            isDirty = true;
        }
    }

    bool isDirty = false;
    protected override void OnTransformParentChanged()
    {
        base.OnTransformParentChanged();
        //Hiker.HikerUtils.DoAction(this, UpdateVisual, 0.15f, true);
        isDirty = true;
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        //Hiker.HikerUtils.DoAction(this, UpdateVisual, 0.15f, true);
        isDirty = true;
    }

    void UpdateVisual()
    {
        if (map != null)
        {
            for (int i = 0; i < map.points.Count; ++i)//(var node in map.points)
            {
                var node = map.points[i];
                if (ListPoints.Count > i)
                {
                    var visualNode = ListPoints[i].gameObject;
                    visualNode.transform.position = node.transform.position;
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogError("NodeMapVisualError: maybe happen when Init is not called");
                }
#endif
            }
            foreach (var connector in listConnector)
            {
                connector.UpdateLine();
            }
        }
    }
}
