using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleItem
{
    public string posId;
    public string codeName;
    public int tier;

    public BattleItemStat stat;

    public BattleItemStat GetStat()
    {
        if (stat == null)
        {
            stat = BattleItemConfig.ItemStat(codeName);
        }
        return stat;
    }

    public void SetPosId(string pos)
    {
        posId = pos;
    }

    public void IncTier()
    {
        tier++;
    }
}
