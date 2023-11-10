using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearWhenLoading : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.IsLoadingMission)
        {
            gameObject.SetActive(false);
        }
    }
}
