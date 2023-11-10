using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SlotInventory
{
    public string posId;

    public int rowId;
    public int colId;

    private Dictionary<string, BuffStat> slotBuffs;

    public string InitSlot(int row, int col)
    {
        rowId = row;
        colId = col;
        posId = string.Format("{0}:{1}", row, col);
        return posId;
    }

    public string InitHandSlot(int col)
    {
        colId = col;
        posId = string.Format("{0}:{1}", "Hand", col);
        return posId;
    }

    public string InitRandomSlot(int col)
    {
        colId = col;
        posId = string.Format("{0}:{1}", "Random", col);
        return posId;
    }

    public Dictionary<string, BuffStat> GetSlotBuffs()
    {
        if (slotBuffs == null)
        {
            slotBuffs = new Dictionary<string, BuffStat>();
        }
        return slotBuffs;
    }

    public void ApplyBuffStat(string key, BuffStat buff)
    {
        if(slotBuffs == null)
        {
            slotBuffs = new Dictionary<string, BuffStat>();
        }

        if (slotBuffs.ContainsKey(key))
        {
            slotBuffs[key] = buff;
        }
        else
        {
            slotBuffs.Add(key, buff);
        }
    }

    public void ClearExpireBuffs(string key)
    {
        if (slotBuffs != null)
        {
            slotBuffs.Remove(key);
        }
    }
}
