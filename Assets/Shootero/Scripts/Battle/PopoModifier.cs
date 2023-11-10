using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopoModifier : MonoBehaviour
{
    DonViChienDau unit;
    GameObject popoVisual;

    private void Awake()
    {
        unit = GetComponent<DonViChienDau>();
    }

    private void Start()
    {
        if (unit.visualGO)
            unit.visualGO.SetActive(false);

        string prefabPath = "LevelDesign/PopoVisual";
        if (unit.IsAir)
        {
            prefabPath = "LevelDesign/PopoBayVisual";
        }

        popoVisual = ObjectPoolManager.Spawn(prefabPath, Vector3.zero, Quaternion.identity, transform);
        popoVisual.GetComponent<PopoVisual>().SetupVisual(unit);

        unit.SetupVisualGO(popoVisual);
    }

    private void OnDestroy()
    {
        if (popoVisual)
        {
            ObjectPoolManager.Unspawn(popoVisual);
        }
    }
}
