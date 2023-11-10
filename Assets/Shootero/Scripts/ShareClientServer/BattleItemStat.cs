using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleItemConfig
{
    public Dictionary<string, BuffShape> shapes;
    public List<BattleItem> testItems;

    public static List<string> ListSlotIDFromShape(string shape, int posX, int posY, int maxX, int maxY)
    {
        if (ConfigManager.BattleItemCfg == null) return null;
        if (ConfigManager.BattleItemCfg.shapes == null) return null;

        List<string> list = new List<string>();

        foreach(var p in ConfigManager.BattleItemCfg.shapes[shape].charmBuffPos)
        {
            if(posX + p.x >= 0 && posX + p.x < maxX
                && posY + p.y >= 0 && posY + p.y < maxY)
            {
                list.Add(string.Format("{0}:{1}", (posX + p.x), (posY + p.y)));
            }
        }
        return list;
    }

    public static BattleItemStat ItemStat(string codeName)
    {
        if(ConfigManager.BattleItemStats == null) return null;
        if (ConfigManager.BattleItemStats.ContainsKey(codeName))
        {
            return ConfigManager.BattleItemStats[codeName];
        }
        return null;
    }

    public string RandomCodeName(Dictionary<string, BattleItemStat> BattleItemStats)
    {
        List<string> list = new List<string>();
        foreach(var key in BattleItemStats.Keys)
        {
            list.Add(key);
        }
        return list[Random.Range(0, list.Count)];
    }

    public List<BattleItem> GetTestItem()
    {
        List<BattleItem> list = new List<BattleItem>();

        foreach(var item in testItems)
        {
            BattleItem it = new BattleItem();
            it.posId = item.posId;
            it.tier = item.tier;
            it.codeName = item.codeName;
            it.GetStat();
            list.Add(it);
        }
        return list;
    }
}

public class BattleItemStat
{
    public string shape;
    public BuffStat buffStat;
}

public class BuffShape
{
    public PosXY[] charmBuffPos;
}

public class PosXY
{
    public int x;
    public int y;
}