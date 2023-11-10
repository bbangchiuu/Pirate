using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IceGiantDamVaCham : MonoBehaviour
{
    DonViChienDau unit;
    public event System.Action<DonViChienDau> onUnitCollision;
    private void Awake()
    {
        unit = GetComponentInParent<DonViChienDau>();
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject);
        var colDmg = unit.GetStat().COLLISION_DMG;
        if (colDmg > 0)
        {
            var otherUNit = other.gameObject.GetComponent<DonViChienDau>();
            if (otherUNit != null)
            {
                otherUNit.TakeDamage(colDmg,false, unit);
                var agent = otherUNit.GetComponent<NavMeshAgent>();
                if (agent != null && agent.enabled && agent.isOnNavMesh)
                    agent.ResetPath();

                onUnitCollision?.Invoke(otherUNit);
            }

            var ice_wall = other.gameObject.GetComponent<IceWall>();
            if (ice_wall)
            {
                ice_wall.OnCollisionWithProjectile();
            }
        }
    }
}
