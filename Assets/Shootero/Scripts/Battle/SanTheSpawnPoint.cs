using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanTheSpawnPoint : MonoBehaviour
{
    public Transform[] spawnPoints1;
    public Transform[] spawnPoints2;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        if (spawnPoints1 != null && spawnPoints1.Length > 0)
        {
            foreach (var p in spawnPoints1)
            {
                Gizmos.DrawSphere(p.position, 0.5f);
            }
        }
        Gizmos.color = Color.magenta;
        if (spawnPoints2 != null && spawnPoints2.Length > 0)
        {
            foreach (var p in spawnPoints2)
            {
                Gizmos.DrawSphere(p.position, 0.5f);
            }
        }
    }
}
