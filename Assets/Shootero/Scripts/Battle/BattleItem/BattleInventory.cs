using Hiker.GUI.Shootero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInventory : MonoBehaviour
{
    public int row = 3;
    public int col = 3;
    public int maxHand = 6;

    //Dictionary chua tat ca item
    public Dictionary<string, BattleItem> itemDic;
    //Dictionary slot 9 o trong inventory
    public Dictionary<string, SlotInventory> slotDic;
    //Dictionary 6 slot tren hand
    public Dictionary<string, SlotInventory> handDic;
    //List item ngoai hand slot
    public List<BattleItem> queueItems;

    private QuanLyBattleItem quanLyBattleItem;

    public void InitBattleInventory(List<BattleItem> listBattleItem = null)
    {
        quanLyBattleItem = QuanLyBattleItem.instance;
        slotDic = new Dictionary<string, SlotInventory>();
        handDic = new Dictionary<string, SlotInventory>();
        itemDic = new Dictionary<string, BattleItem>();
        queueItems = new List<BattleItem>();

        for (int i = 0; i < row; ++i)
        {
            for(int j = 0; j < col; ++j)
            {
                SlotInventory slot = new SlotInventory();
                slotDic.Add(slot.InitSlot(i, j), slot);
            }
        }

        for(int i = 0; i < maxHand; ++i)
        {
            SlotInventory slot = new SlotInventory();
            handDic.Add(slot.InitHandSlot(i), slot);
        }

        if (listBattleItem != null)
        {
            foreach (var item in listBattleItem)
            {
                SetItemInPosition(item.posId, item);
            }
            ApplyInventory();
        }
    }

    //Chi quan tam set vi tri cho item. Khong quan tam la item o slot hay hand
    public void SetItemInPosition(string newPosId, BattleItem item, bool applyBuff = false)
    {
        if (itemDic == null) return;

        //Item moi add la hand item
        if (string.IsNullOrEmpty(newPosId))
        {
            AddItemInHand(item);
            return;
        }

        bool needCheckFillEmptyHand = false;

        //Set item in empty slot
        if (itemDic.ContainsKey(newPosId) == false)
        {
            //Item moi co vi tri xac dinh
            if (string.IsNullOrEmpty(item.posId) == false)
            {
                if (item.posId.StartsWith("Random"))
                {
                    if(PopupBattleInventory.instance != null) PopupBattleInventory.instance.DisableRandomContainer();
                }
                itemDic.Remove(item.posId);
                needCheckFillEmptyHand = true;
            }

            itemDic.Add(newPosId, item);
            itemDic[newPosId].SetPosId(newPosId);
        }
        //Set item in slot have item
        else
        {
            if (string.IsNullOrEmpty(item.posId))
            {
                itemDic[newPosId] = item;
                itemDic[newPosId].SetPosId(newPosId);
            }
            else
            {
                string lastPosId = item.posId;
                if(lastPosId != newPosId)
                {
                    if (item.posId.StartsWith("Random"))
                    {
                        //Chi merge
                        if (item.codeName == itemDic[newPosId].codeName && item.codeName.StartsWith("Charm") == false)
                        {
                            itemDic[newPosId].IncTier();

                            quanLyBattleItem.RemoveItemFromBattle(lastPosId);
                            itemDic.Remove(lastPosId);

                            if (PopupBattleInventory.instance != null) PopupBattleInventory.instance.DisableRandomContainer();
                        }
                    }
                    else
                    {
                        //Merge
                        if (item.codeName == itemDic[newPosId].codeName && item.codeName.StartsWith("Charm") == false)
                        {
                            itemDic[newPosId].IncTier();

                            quanLyBattleItem.RemoveItemFromBattle(lastPosId);
                            itemDic.Remove(lastPosId);
                            needCheckFillEmptyHand = true;
                        }
                        //Swap
                        else
                        {
                            itemDic[lastPosId] = itemDic[newPosId];
                            itemDic[lastPosId].SetPosId(lastPosId);

                            itemDic[newPosId] = item;
                            itemDic[newPosId].SetPosId(newPosId);
                        }
                    }
                }
            }
        }

        if (needCheckFillEmptyHand) 
        {
            CheckFillEmptySlotInHandFromQueue();
        }
        if (applyBuff) ApplyInventory();
    }

    public void AddItemInHand(BattleItem item)
    {
        int handCount = 0;
        foreach (var key in itemDic.Keys)
        {
            if (key.StartsWith("Hand")) { handCount++; }
        }
        string newPos = "Hand:" + handCount.ToString();
        SetItemInPosition(newPos, item);
        if(handCount >= maxHand)
        {
            queueItems.Add(item);
        }
    }

    public void CheckFillEmptySlotInHandFromQueue()
    {
        if(queueItems != null && queueItems.Count > 0)
        {
            foreach(var key in handDic.Keys)
            {
                if (itemDic.ContainsKey(key) == false)
                {
                    SetItemInPosition(key, queueItems[0]);
                    queueItems.RemoveAt(0);
                    break;
                }
            }
        }
    }

    public void ApplyInventory()
    {
        CalculateSlotBuff();
    }

    public void CalculateSlotBuff()
    {
        foreach(var key in itemDic.Keys)
        {
            if (itemDic[key].codeName.StartsWith("Charm") && itemDic[key].posId.StartsWith("Hand") == false)
            {
                SlotInventory slot  = slotDic[key];
                List<string> listBuffID = BattleItemConfig.ListSlotIDFromShape(itemDic[key].GetStat().shape,
                                                                                slot.rowId, slot.colId, row, col);
                if(listBuffID != null && listBuffID.Count > 0)
                {
                    foreach(var slotID in listBuffID)
                    {
                        slotDic[slotID].ApplyBuffStat(key, itemDic[key].GetStat().buffStat);
                    }
                }
            }
        }
        //Clear buff khong ton tai cua slot
        List<string> expireKeys = new List<string>();
        foreach (var key in slotDic.Keys)
        {
            expireKeys.Clear();
            foreach (var k in slotDic[key].GetSlotBuffs().Keys)
            {
                if (itemDic.ContainsKey(k) == false)
                {
                    expireKeys.Add(k);
                }
                else
                {
                    if(itemDic[k].stat == null)
                    {
                        expireKeys.Add(k);
                    }
                    else
                    {
                        if (itemDic[k].stat.buffStat.Equals(default(BuffStat)) ||
                            itemDic[k].stat.buffStat.Type != slotDic[key].GetSlotBuffs()[k].Type)
                        {
                            expireKeys.Add(k);
                        }
                    }
                }
            }
            if(expireKeys.Count > 0)
            {
                foreach (var k in expireKeys)
                {
                    slotDic[key].ClearExpireBuffs(k);
                }
            }
        }

        if (PopupBattleInventory.instance != null)
        {
            PopupBattleInventory.instance.LoadItemInInventory();
            PopupBattleInventory.instance.ReloadBuffInInventory();
        }
    }
}
