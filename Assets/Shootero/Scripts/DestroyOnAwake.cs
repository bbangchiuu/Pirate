using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnAwake : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
