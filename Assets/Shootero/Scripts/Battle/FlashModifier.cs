using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashModifier : MonoBehaviour
{
    DonViChienDau unit;

    public float GetBuffSpeed(float curSpd)
    {
        return curSpd + ConfigManager.LeoThapCfg.HeSoFlash * curSpd / 100f;
    }

    private void Awake()
    {
        unit = GetComponent<DonViChienDau>();
    }
}
