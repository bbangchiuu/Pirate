using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasAspectControl : MonoBehaviour
{
    public CanvasScaler canvasScaler;
    public float minAspect = 0;
    public float maxAspect = 0;

    private void Awake()
    {
        if (canvasScaler == null)
            canvasScaler = GetComponent<CanvasScaler>();

        float screenAspect = (float)Screen.width / Screen.height;
        if (canvasScaler)
        {
            if (minAspect > 0 && screenAspect < minAspect)
            {
                canvasScaler.matchWidthOrHeight = 0;
                //var ratio = screenAspect / minAspect;
                //cam.rect = new Rect(0, (1 - ratio) * 0.5f, 1, ratio);
                //cam.aspect = maxAspect;

                //Screen.SetResolution(Screen.width, (int)(Screen.width / minAspect), false);
            }

            if (maxAspect > 0 && screenAspect > maxAspect)
            {
                canvasScaler.matchWidthOrHeight = 1;
            }
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
