using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class MapController : MonoBehaviour
{
    public Camera mainCam;
    static readonly Plane PlaneZero = new Plane(Vector3.up, 0);

    Vector3[] CameraCornerPos;

    Bounds cameraBounds;

    public static MapController instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
        {
            var ratio = 9f / 16f / mainCam.aspect;
            //if (ratio > 1)
            if (mainCam.orthographic)
            {
                mainCam.orthographicSize = mainCam.orthographicSize * ratio;
            }
            else
            {
                var ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                if (PlaneZero.Raycast(ray, out float d))
                {
                    var centerPos = ray.GetPoint(d);
                    var dis = mainCam.transform.position - centerPos;
                    mainCam.transform.position = centerPos + dis.magnitude * dis.normalized * ratio;
                    mainCam.GetComponent<TargetFollower>().InitPos();
                }
            }

            CaculateCameraBox();
        }
    }

    private void Update()
    {
//        if (CameraCornerPos == null ||
//            CameraCornerPos.Length < 4
//#if UNITY_EDITOR
//            || Application.isPlaying == false
//#endif
//            )
//        {
//            CaculateCameraBox();
//        }
    }

    public bool CheckPointInMap(Vector3 pos)
    {
        //if (CameraCornerPos == null || CameraCornerPos.Length < 4)
        //{
        //    CaculateCameraBox();
        //}

        //pos.y = 0;

        //return cameraBounds.Contains(pos);
        return true;
    }


//#if UNITY_EDITOR

//    private void OnEnable()
//    {
//        EditorApplication.update += Update;
//    }


//    private void OnDisable()
//    {
//        EditorApplication.update -= Update;
//    }
//#endif

    Vector3 GetTouchPositionOnPlane(Vector3 touchPos)
    {
        if (mainCam != null)
        {
            var ray = mainCam.ScreenPointToRay(touchPos);
            if (PlaneZero.Raycast(ray, out float d))
            {
                return ray.GetPoint(d);
            }
        }

        return Vector3.zero;
    }

    void CaculateCameraBox()
    {
        var p1 = GetTouchPositionOnPlane(new Vector3(0, 0, 0));
        var p2 = GetTouchPositionOnPlane(new Vector3(Screen.width, 0, 0));
        var p3 = GetTouchPositionOnPlane(new Vector3(Screen.width, Screen.height, 0));
        var p4 = GetTouchPositionOnPlane(new Vector3(0, Screen.height, 0));

        var minx = Mathf.Min(Mathf.Abs(p1.x), Mathf.Abs(p2.x), Mathf.Abs(p3.x), Mathf.Abs(p4.x));
        var minz = Mathf.Min(p1.z, p2.z, p3.z, p4.z);
        var maxz = Mathf.Max(p1.z, p2.z, p3.z, p4.z);

        CameraCornerPos = new Vector3[4];
        CameraCornerPos[0] = new Vector3(-minx, 0, minz);
        CameraCornerPos[1] = new Vector3(minx, 0, minz);
        CameraCornerPos[2] = new Vector3(minx, 0, maxz);
        CameraCornerPos[3] = new Vector3(-minx, 0, maxz);

        cameraBounds = new Bounds((CameraCornerPos[2] + CameraCornerPos[0]) * 0.5f,
            CameraCornerPos[2] - CameraCornerPos[0]);
        cameraBounds.extents -= new Vector3(1, 0, 1f);
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (CameraCornerPos == null || CameraCornerPos.Length < 4)
            CaculateCameraBox();
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(CameraCornerPos[0], CameraCornerPos[1]);
        Gizmos.DrawLine(CameraCornerPos[1], CameraCornerPos[2]);
        Gizmos.DrawLine(CameraCornerPos[2], CameraCornerPos[3]);
        Gizmos.DrawLine(CameraCornerPos[3], CameraCornerPos[0]);
    }
#endif
}
