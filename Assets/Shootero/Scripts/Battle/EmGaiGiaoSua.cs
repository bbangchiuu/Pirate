using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmGaiGiaoSua : MonoBehaviour
{
    private void Awake()
    {
        if (QuanlyNguoichoi.Instance != null &&
            QuanlyNguoichoi.Instance.IsLevelClear)
        {
            gameObject.SetActive(false);
            return;
        }
    }
}
