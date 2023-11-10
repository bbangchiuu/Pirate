using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GameObjectStateSync : MonoBehaviour
{
    public GameObject[] targets;
    private void OnEnable()
    {
        if (targets != null)
        {
            foreach (var t in targets)
            {
                if (t != null)
                {
                    t.gameObject.SetActive(true);
                }
            }
        }
    }
    private void OnDisable()
    {
        if (targets != null)
        {
            foreach (var t in targets)
            {
                if (t != null)
                {
                    t.gameObject.SetActive(false);
                }
            }
        }
    }
}
