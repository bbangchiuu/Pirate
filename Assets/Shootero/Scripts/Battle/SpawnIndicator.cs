﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {        
    }

    private void OnEnable()
    {
        GameObject.Destroy(this.gameObject, 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
