using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitShield : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var proj = other.GetComponentInParent<SatThuongDT>();
        if (proj)
        {
            //proj.gameObject.SetActive(false);
            //gameObject.SetActive(false);
        }
    }
}
