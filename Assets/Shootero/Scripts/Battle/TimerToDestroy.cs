using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PhuongTD : Only use for necromancer skeleton
public class TimerToDestroy : MonoBehaviour
{
    public float TimeToDestroy = 5;

    private DonViChienDau _bUnit;
    // Start is called before the first frame update

    private bool IsCanRevive = false;
    void Start()
    {
        _bUnit = this.GetComponent<DonViChienDau>();

        var listMods = QuanlyNguoichoi.Instance.GetListRuntimeStatMods();
        if (listMods != null)
        {
            var mod = listMods.Find(e => e.Stat == EStatType.HEROPLUS && e.Target == "Necromancer");
            if (mod != null)
            {
                IsCanRevive = true;
                _bUnit.Life = 2;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        TimeToDestroy -= Time.deltaTime;
        if (TimeToDestroy < 0 && _bUnit!=null)
        {
            IsCanRevive = false;
            _bUnit.Life = 1;
            _bUnit.TakeDamage(999999,false,null,false,false);
        }

        //if (_bUnit.GetCurHP() < 0 && IsCanRevive)
        //{
        //    _bUnit.Life = 0;
        //    IsCanRevive = false;
        //}
    }
}
