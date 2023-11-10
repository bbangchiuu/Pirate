using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    public static GateController instance;

    private void Awake()
    {
        instance = this;
    }
}
