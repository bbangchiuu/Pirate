using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkDataSync : MonoBehaviour
{
    public static List<NetworkDataSync> ListNetworkData = new List<NetworkDataSync>();
    protected virtual void OnEnable()
    {
        if (ListNetworkData.Contains(this) == false)
        {
            ListNetworkData.Add(this);
        }
    }

    protected virtual void OnDisable()
    {
        ListNetworkData.Remove(this);
    }

    public virtual void SyncNetworkData()
    {

    }
}
