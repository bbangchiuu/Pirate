using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfernalModifier : MonoBehaviour
{
    DonViChienDau unit;

    private void Awake()
    {
        unit = GetComponent<DonViChienDau>();
    }

    public void InfernalSkill()
    {
        var go = Instantiate(Resources.Load<GameObject>("LevelDesign/InfernalSkill"));
        go.SetActive(true);
    }
}
