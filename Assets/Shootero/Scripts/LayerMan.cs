using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMan
{
    public static readonly int itemLayer = LayerMask.NameToLayer("Item");
    public static readonly int uiLayer = LayerMask.NameToLayer("UI");
    public static readonly int defaultLayer = LayerMask.NameToLayer("Default");

    //public static readonly int sortingLayerBoard = SortingLayer.NameToID("Board");
    //public static readonly int sortingLayerUI = SortingLayer.NameToID("UI");

}