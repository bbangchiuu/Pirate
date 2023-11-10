using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGiatTuTru : SatThuongDT
{
    public GameObject roundIndicator;
    public GameObject Indicator;
    public SatThuongDT dmgObj;
    public GameObject dmgEff;

    public float delayTime = 0.6f;

    private void OnEnable()
    {
        dmgObj.gameObject.SetActive(false);
        dmgEff.SetActive(false);
        Indicator.SetActive(true);
        roundIndicator.SetActive(true);
    }

    public override void ActiveDan(float speed, long dmg, Vector3 target)
    {
        base.ActiveDan(speed, dmg, target);
        if (QuanlyManchoi.instance.PlayerUnit)
        {
            transform.position = QuanlyManchoi.instance.PlayerUnit.transform.position;
        }

        if (roundIndicator)
        {
            roundIndicator.transform.localScale = Vector3.one * 2f;
            var ts = TweenScale.Begin(roundIndicator, delayTime, Vector3.one * 1.15f);
            ts.ignoreTimeScale = false;
        }

        if (dmgObj)
        {
            Hiker.HikerUtils.DoAction(this,
                () => {
                    Indicator.SetActive(false);
                    roundIndicator.SetActive(false);
                    dmgEff.SetActive(true);
                    dmgObj.gameObject.SetActive(true);
                    dmgObj.ActiveDan(0, Damage, Vector3.zero);
                    dmgObj.SourceUnit = SourceUnit;
                },
                delayTime);

            Hiker.HikerUtils.DoAction(this,
                () =>
                {
                    dmgObj.gameObject.SetActive(false);
                },
                delayTime+0.1f);
        }

        //ObjectPoolManager.Unspawn(gameObject, delayTime + 0.15f);
        ObjectPoolManager.instance.AutoUnSpawn(gameObject, delayTime + 0.65f);
    }

    protected override void OnDeactiveDan()
    {
        base.OnDeactiveDan();
    }
}
