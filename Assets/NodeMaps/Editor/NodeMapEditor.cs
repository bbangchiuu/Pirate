using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(NodeMap))]
public class NodeMapEditor : Editor
{
    [MenuItem("GameObject/Create Other/NodeMap")]
    public static void CreateNodeMap()
    {
        GameObject go = new GameObject("NodeMap");
        bool uiTrans = false;
        var pivotAnchor = new Vector2(0.5f, 0); // center bot
        if (Selection.activeTransform)
        {
            go.transform.SetParent(Selection.activeTransform, false);
            go.transform.localScale = Vector3.one;
            var rectTrans = Selection.activeTransform.GetComponent<RectTransform>();
            if (rectTrans)
            {
                uiTrans = true;
                var rect = go.AddComponent<RectTransform>();
                rect.pivot = rect.anchorMin = rect.anchorMax = pivotAnchor;

                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1080); // default FULLHD Resolution
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1920);
            }
        }

        NodeMap map = go.AddComponent<NodeMap>();
        go.AddComponent<MapDungeonConfig>();

        GameObject mapPoint = new GameObject("Start");
        mapPoint.transform.SetParent(go.transform, false);
        mapPoint.transform.localScale = Vector3.one;
        var levelConfig = mapPoint.AddComponent<NodeLevelConfig>();
        levelConfig.LevelType = ENodeType.Start;
        if (uiTrans)
        {
            var rect = mapPoint.AddComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = pivotAnchor;
        }

        NodePoint p = mapPoint.AddComponent<NodePoint>();

        map.points.Add(p);
        //p.Handle = new Vector3(1.5f, 0, 0);
        //p.BezierCurve = new BezierCurve(p.Point, p.Tangent, Vector3.zero, Vector3.zero);
        //path.PathPoints.Add(p);
        Selection.SetActiveObjectWithContext(go, Selection.activeContext);
    }

    NodeMap currentNodeMap;

    private void OnEnable()
    {
        currentNodeMap = target as NodeMap;
        Undo.RecordObject(target, "Clear Node");
        var changed = currentNodeMap.ClearNodes();
        if (changed)
        {
            EditorUtility.SetDirty(target);
        }
        else
        {
            
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
