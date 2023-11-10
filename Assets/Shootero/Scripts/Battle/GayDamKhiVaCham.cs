using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Hiker;

public class GayDamKhiVaCham : MonoBehaviour
{
    public DonViChienDau unit;
    public event System.Action<DonViChienDau> onUnitCollision;

    private LayerMask TargetMask;
    private void Awake()
    {
        unit = GetComponentInParent<DonViChienDau>();

        TargetMask = LayersMan.Team1HitLayerMask;
    }

    List<DonViChienDau> mTargets = new List<DonViChienDau>();


    void OnUnitEnter(DonViChienDau unit)
    {
        if (unit.TeamID == QuanlyManchoi.PlayerTeam)
        {
            mTargets.Add(unit);
        }
    }


    void OnDisable()
    {
        mTargets.Clear();
    }

    void OnUnitExit(DonViChienDau unit)
    {
        mTargets.Remove(unit);
    }

    protected virtual void OnTriggerEnter(Collider col)
    {
        if (HikerUtils.CheckLayerMask(TargetMask, col.gameObject.layer))
        {
            DonViChienDau unit = col.GetComponent<DonViChienDau>();

            if (unit)
            {
                if (mTargets.Contains(unit) == false)
                {
                    OnUnitEnter(unit);
                }

            }
        }

        // fire onCollision event
        var colDmg = unit.GetStat().COLLISION_DMG;
        if (colDmg > 0)
        {
            var otherUNit = col.gameObject.GetComponent<DonViChienDau>();
            if (otherUNit != null)
            {
                //otherUNit.TakeDamage(colDmg);
                var agent = otherUNit.GetComponent<NavMeshAgent>();
                if (agent != null && agent.enabled && agent.isOnNavMesh)
                    agent.ResetPath();

                onUnitCollision?.Invoke(otherUNit);
            }
        }
    }

    protected void OnTriggerExit(Collider col)
    {
        if (HikerUtils.CheckLayerMask(TargetMask, col.gameObject.layer))
        {
            DonViChienDau unit = col.GetComponent<DonViChienDau>();

            if (unit)
            {
                if (mTargets.Contains(unit))
                    OnUnitExit(unit);
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < mTargets.Count; i++)
        {
            var colDmg = unit.GetStat().COLLISION_DMG;
            if (colDmg > 0)
            {
                var otherUNit = mTargets[i];
                if (otherUNit != null)
                {
                    otherUNit.TakeDamage(colDmg,false, unit);
                    //var agent = otherUNit.GetComponent<NavMeshAgent>();
                    //if (agent != null && agent.enabled && agent.isOnNavMesh)
                    //    agent.ResetPath();

                    //onUnitCollision?.Invoke(otherUNit);
                }
            }
        }
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    //Debug.Log(other.gameObject);
    //    var colDmg = unit.GetStat().COLLISION_DMG;
    //    if (colDmg > 0)
    //    {
    //        var otherUNit = other.gameObject.GetComponent<DonViChienDau>();
    //        if (otherUNit != null)
    //        {
    //            otherUNit.TakeDamage(colDmg);
    //            var agent = otherUNit.GetComponent<NavMeshAgent>();
    //            if (agent != null && agent.enabled && agent.isOnNavMesh)
    //                agent.ResetPath();

    //            onUnitCollision?.Invoke(otherUNit);
    //        }
    //    }
    //}
}
