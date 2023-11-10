using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DanBay))]
public class DanNhay : MonoBehaviour
{
    //public const float MAX_RANGE = 12;
    public int Count { get; set; }
    public int DMGPercent { get; set; }
    public float NhayRange = 12;

    public List<DonViChienDau> ListTargets { get; set; }

    private void OnDisable()
    {
        if (ListTargets != null)
        {
            Hiker.Util.ListPool<DonViChienDau>.Release(ListTargets);
            ListTargets = null;
        }
    }
}
