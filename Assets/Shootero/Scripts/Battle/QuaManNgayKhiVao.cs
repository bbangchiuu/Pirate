using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuaManNgayKhiVao : MonoBehaviour
{
    bool execute = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (execute == false &&
            QuanlyManchoi.instance &&
            QuanlyNguoichoi.Instance &&
            QuanlyNguoichoi.Instance.IsInitedMission &&
            Hiker.GUI.Shootero.PopupRollingBuff.instance == null)
        {
            if (QuanlyNguoichoi.Instance.IsLevelClear == false)
            {
                QuanlyManchoi.instance.OnLevelClear();
            }

            execute = true;
        }
    }
}
