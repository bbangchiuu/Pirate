using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class CameraAspectControl : MonoBehaviour
{
    public Camera cam;
    public float minAspect = 0;
    public float maxAspect = 0;

    private void Awake()
    {
        if (cam == null)
            cam = GetComponent<Camera>();

        float screenAspect = (float)Screen.width / Screen.height;
        var refAspect = screenAspect;
        if (refAspect < minAspect)
        {
            refAspect = minAspect;
        }
        if (refAspect > maxAspect)
        {
            refAspect = maxAspect;
        }

        if (cam)
        {
            if (refAspect > 0 && screenAspect < refAspect)
            {
                var ratio = screenAspect / refAspect;
                cam.rect = new Rect(0, (1 - ratio) * 0.5f, 1, ratio);
                cam.aspect = refAspect;
                //Screen.SetResolution(Screen.width, (int)(Screen.width / minAspect), false);
            }
            else
            if (refAspect > 0 && screenAspect > refAspect)
            {
                var ratio = refAspect / screenAspect;
                cam.rect = new Rect((1 - ratio) * 0.5f, 0, ratio, 1);
                cam.aspect = refAspect;
                //Screen.SetResolution((int)(Screen.height * maxAspect), Screen.height, false);
            }
        }
    }

    private void Update()
    {
        
    }
}
