using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using System;
    using UnityEngine.UI;
    public class PopupLapThe : PopupBase
    {
        public static PopupLapThe instance;

        public Transform itemParent;
        public CardAvatar itemPrefab;
        public Text emptyList;
        List<CardAvatar> mListItems = new List<CardAvatar>();
        CardAvatar mSelectedItem;
        CardData mSelectedItemData;
        TrangBiData mTBData;
        int mSlotIndex;

        int SortByRarity(CardAvatar it1, CardAvatar it2)
        {
            var tb1 = GameClient.instance.UInfo.listCards.Find(e => e.ID == it1.TBId);
            var tb2 = GameClient.instance.UInfo.listCards.Find(e => e.ID == it2.TBId);
            var cfg1 = ConfigManager.CardStats[it1.TBName];
            var cfg2 = ConfigManager.CardStats[it2.TBName];
            var dif = (int)cfg2.Rarity - (int)cfg1.Rarity;
            if (dif == 0)
            {
                if (cfg1.Slot == cfg2.Slot)
                {
                    return tb1.ID.CompareTo(tb2.ID);
                }

                return cfg1.Slot - cfg2.Slot;
            }
            return dif;
        }

        private void SortItems()
        {
            mListItems.Sort(SortByRarity);

            for (int i = 0; i < mListItems.Count; ++i)
            {
                mListItems[i].transform.SetSiblingIndex(i);
            }
        }

        public static PopupLapThe Create(TrangBiData tbData, int slotIndex)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupLapThe");
            instance = go.GetComponent<PopupLapThe>();
            instance.Init(tbData, slotIndex);

            return instance;
        }

        private void Init(TrangBiData tbData, int slotIndex)
        {
            if (GameClient.instance == null ||
                GameClient.instance.UInfo == null ||
                GameClient.instance.UInfo.listCards == null)
            {
                return;
            }

            mTBData = tbData;
            mSlotIndex = slotIndex;
            ItemConfig itemCfg = ConfigManager.GetItemConfig(mTBData.Name);
            if (itemCfg == null) return;
            SlotType slotType = itemCfg.Slot;

            UserInfo uInfo = GameClient.instance.UInfo;
            int countItem = 0;
            for (int i = 0; i < uInfo.listCards.Count; ++i)
            {
                CardData itemData = uInfo.listCards[i];
                if (itemData == null || ConfigManager.CardStats.ContainsKey(itemData.Name) == false || mTBData.CardSlots.Contains(itemData.ID) == true) continue;
                CardConfig cardCfg = ConfigManager.CardStats[itemData.Name];
                if (cardCfg.Slot != slotType) continue;
                CardAvatar itemAvatar;
                if (mListItems.Count > countItem)
                {
                    itemAvatar = mListItems[countItem];
                }
                else
                {
                    itemAvatar = Instantiate(itemPrefab, itemParent);
                    mListItems.Add(itemAvatar);
                }

                countItem++;

                itemAvatar.SetItem(itemData.Name, itemData.Rarity, itemData.ID);
                var itemBtn = itemAvatar.GetComponent<Button>();
                itemBtn.onClick.AddListener(() => OnItemClick(itemAvatar, itemData));
                itemAvatar.gameObject.SetActive(true);
                
            }

            for (int i = countItem; i < mListItems.Count; ++i)
            {
                mListItems[i].gameObject.SetActive(false);
            }
            emptyList.gameObject.SetActive(countItem == 0);
            itemPrefab.gameObject.SetActive(false);

            SortItems();
        }

        void OnItemClick(CardAvatar selectedItem, CardData selectedItemData)
        {
            mSelectedItem = selectedItem;
            mSelectedItemData = selectedItemData;
            PopupCardInfo.Create(selectedItemData, false, false, true, ()=> 
            {
                mTBData.CardSlots[mSlotIndex] = selectedItemData.ID;
                GameClient.instance.RequestUpdateCardSlot(mTBData.ID, mTBData.CardSlots);
                Dismiss();
            });
        }
        
        public static void Dismiss()
        {
            if (instance != null)
            {
                instance.OnCloseBtnClick();
            }
        }
        
    }
}