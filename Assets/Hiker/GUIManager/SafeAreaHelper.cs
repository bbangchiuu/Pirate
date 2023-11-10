using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaHelper : MonoBehaviour
{
    [SerializeField]
    CanvasHelper canvasHelper;

    public RectTransform rectTrans { get; private set; }

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        Init();
        if (canvasHelper != null)
        {
            canvasHelper.AddSafeArea(this);
        }
    }

    private void OnDisable()
    {
        if (canvasHelper != null)
        {
            canvasHelper.RemoveSafeArea(this);
        }
    }

    void Init()
    {
        if (canvasHelper == null)
        {
            canvasHelper = GetComponentInParent<CanvasHelper>();
        }
        if (rectTrans == null)
            rectTrans = GetComponent<RectTransform>();
    }
}
