using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathrattleModifier : MonoBehaviour
{
    DonViChienDau unit;
    GameObject sungDeathrattle;

    private void Awake()
    {
        unit = GetComponent<DonViChienDau>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (unit && unit.shooter4 == null)
        {
            sungDeathrattle = ObjectPoolManager.Spawn("LevelDesign/Deathrattle",
                Vector3.up * 1.4f,
                Quaternion.identity,
                unit.transform);
            
            var sung = sungDeathrattle.GetComponent<SungCls>();
            sung.FireOnOwnerDie = true;
            unit.shooter4 = sung;
            sung.Init();
            sung.DMG = unit.GetCurStat().DMG;
        }
    }

    //private void Update()
    //{
    //    if (unit.IsAlive() == false)
    //    {
    //        if (sungDeathrattle)
    //        {
    //            ObjectPoolManager.Unspawn(sungDeathrattle);
    //        }
    //    }
    //}

    public void OnUnitDie()
    {
        if (sungDeathrattle)
        {
#if DEBUG
            Debug.Log("Unspawn sung");
#endif
            ObjectPoolManager.Unspawn(sungDeathrattle);
        }
    }
}
