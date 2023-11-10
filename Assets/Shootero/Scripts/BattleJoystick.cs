using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleJoystick : MonoBehaviour
{
    public static BattleJoystick instance = null;
    private void Awake()
    {
        if (instance) return;
        instance = this;
        instance.gameObject.SetActive(false);
    }
}
