using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class ClearAllMinion : MonoBehaviour
{
    void OnDisable()
    {
        BehaviorTree btree = GetComponent<BehaviorTree>();
        if (btree != null)
        {
            SharedGameObjectList val = (SharedGameObjectList) btree.GetVariable("ListMinion");
            if (val != null && val.Value != null)
            {
                List<GameObject> list_minion = val.Value;
                for (int i = 0; i < list_minion.Count; i++)
                {
                    var m = list_minion[i];
                    if (m != null)
                    {
                        DonViChienDau b_unit = list_minion[i].GetComponent<DonViChienDau>();

                        if (b_unit != null && b_unit.IsAlive())
                        {
                            b_unit.TakeDamage(1000000, false, null, false, false);
                        }
                    }
                }
            }
        }
    }
}
