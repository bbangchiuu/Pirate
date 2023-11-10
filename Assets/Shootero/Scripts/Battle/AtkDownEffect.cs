using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(DonViChienDau))]
public class AtkDownEffect : MonoBehaviour
{
    List<AtkDownEffData> mListEffect = new List<AtkDownEffData>();
    //public DonViChienDau unit;

    private void Awake()
    {
        //unit = GetComponent<DonViChienDau>();
    }

    public void AddAtkDownEffect(AtkDownEffData effData)
    {
        var curEff = mListEffect.Find(e => e.Type == effData.Type);
        if (curEff != null)
        {
            curEff.Amount = effData.Amount;
        }
        else
        {
            mListEffect.Add(effData);
        }
    }

    public void RemoveAtkDownEffect(BuffType buffType)
    {
        mListEffect.RemoveAll(e => e.Type == buffType);
    }

    public int GetTotalAtkDownPercent()
    {
        int a = 100;
        for (int i = mListEffect.Count - 1; i >= 0; --i)
        {
            a = (a * mListEffect[i].Amount / 100);
        }
        return a;
    }
}

public class AtkDownEffData
{
    public int Amount;
    public BuffType Type;
}
