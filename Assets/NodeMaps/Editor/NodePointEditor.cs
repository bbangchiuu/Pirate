using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(NodePoint))]
public class NodePointEditor : Editor
{
    private NodePoint selectedPoint;
    private NodeMap selectedMap;

    private void OnEnable()
    {
        selectedPoint = target as NodePoint;
        selectedMap = selectedPoint.GetComponentInParent<NodeMap>();
    }

    bool drawConnections = false;
    NodeConnection mCurConnection;

    private void OnSceneGUI()
    {
        Handles.color = Color.blue;
        Handles.Label(selectedPoint.transform.position + Vector3.up * 2,
            selectedPoint.name);

        if (Handles.Button(selectedPoint.transform.position + Vector3.up * 40, selectedPoint.transform.rotation, 2, 4, Handles.RectangleHandleCap))
        {
            drawConnections = true;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Point"))
        {
            AddPoint();
        }

        if (GUILayout.Button("Remove Point"))
        {
            DeletePoint();
        }
        EditorGUILayout.EndHorizontal();

        if (GUI.changed && target != null)
        {
            EditorUtility.SetDirty(target);
        }
    }

    void AddPoint()
    {
        NodePoint point = selectedPoint;
        NodeMap map = selectedMap;

        int lastIndex = map.points.Count - 1;

        var mapRect = map.transform as RectTransform;

        GameObject pathPoint = new GameObject("Point_" + (lastIndex + 1));

        pathPoint.transform.SetParent(map.transform, false);
        pathPoint.transform.localScale = Vector3.one;
        if (mapRect)
        {
            var xe = pathPoint.AddComponent<RectTransform>();
            xe.anchorMin = xe.anchorMax = mapRect.pivot;
        }
        NodePoint p = pathPoint.AddComponent<NodePoint>();

        var levelConfig = pathPoint.AddComponent<NodeLevelConfig>();

        Undo.RecordObject(map, "Create PathPoint");
        //if (map.points.IndexOf(point) == lastIndex)
        {
            p.transform.localPosition = selectedMap.transform.InverseTransformPoint(point.transform.position)
                + (Vector3.up * 100);
            NodeConnection connection = new NodeConnection();
            connection.startPoint = selectedPoint;
            connection.endPoint = p;

            map.connections.Add(connection);
            selectedPoint.connections.Add(connection);
            map.points.Add(p);
        }
        //else
        //{
        //    PathPoint pointInFront = map.PathPoints[map.PathPoints.IndexOf(point) + 1];

        //    // Place it somewhere in the middle
        //    p.Point = map.CalculateBezierPoint(0.5f, selectedPoint.BezierCurve);
        //    p.Handle = new Vector3(0, 0, 0);
        //    p.transform.localPosition = p.Point;

        //    p.BezierCurve = new BezierCurve(p.Point, p.Tangent, pointInFront.Tangent, pointInFront.Point);

        //    map.PathPoints.Insert(map.PathPoints.IndexOf(point) + 1, p);
        //}

        selectedPoint = p;

        //// TODO: A weird way to change selection, couldn't find any better way online..?
        //GameObject[] selection = new GameObject[1];
        //selection[0] = selectedPoint.gameObject;
        //Selection.objects = selection;
        Selection.activeGameObject = p.gameObject;
    }

    void DeletePoint()
    {
        NodePoint point = selectedPoint;
        NodeMap map = selectedMap;

        Undo.RecordObject(map, "Delete PathPoint");
        if (map.points.Count > 1)
        {
            map.points.Remove(point);
            var connections = map.connections.FindAll(e => e.endPoint == point || e.startPoint == point);
            foreach (var c in connections)
            {
                if (c.endPoint == point)
                {
                    c.startPoint.connections.Remove(c);
                }
                map.connections.Remove(c);
            }
            DestroyImmediate(point.gameObject);
        }

        if (selectedPoint == point)
        {
            selectedPoint = map.points[0];
        }
    }
}
