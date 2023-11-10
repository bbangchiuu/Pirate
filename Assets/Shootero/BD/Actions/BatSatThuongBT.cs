using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("BatSatThuongBT")]
    [TaskCategory("Shootero")]
    public class BatSatThuongBT : Action
    {
        public SharedGameObject sathuongDT;
        long DMG;
        SatThuongDT dmgObj;
        public override void OnAwake()
        {
            base.OnAwake();
            dmgObj = sathuongDT.Value.GetComponent<SatThuongDT>();

            if (QuanlyNguoichoi.Instance != null)
            {
                DMG = QuanlyNguoichoi.Instance.GetDMGEnemy();
            }
            else
            {
                DMG = 100;
            }
        }

        public override void OnStart()
        {
            dmgObj.gameObject.SetActive(true);
            dmgObj.ActiveDan(0, DMG, Vector3.zero);
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }

        public override void OnPause(bool paused)
        {
        }

        public override void OnReset()
        {
        }
    }
}
