using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanBossWitchDoctor : DanTenLua
{
    public GameObject[] ListSpawnUnit;

    protected override void OnDeactiveProjectile()
    {
        base.OnDeactiveProjectile();

        if (ListSpawnUnit != null && ListSpawnUnit.Length>0 && QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.IsLevelClear == false)
        {
            int _r = Random.Range(0, ListSpawnUnit.Length);

            var unitObj = GameObject.Instantiate(ListSpawnUnit[_r]);
            unitObj.transform.position = this.transform.position;
            unitObj.SetActive(true);
        }
    }
}
