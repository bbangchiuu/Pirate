using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using System;
    using UnityEngine.UI;
    public class PopupDucLoTrangBi : PopupBase
    {
        public static PopupDucLoTrangBi instance;

        public Transform itemParent;
        public ItemAvatar itemPrefab;
        public Text emptyList;
        public Text lbTitle;
        List<ItemAvatar> mListItems = new List<ItemAvatar>();
        ItemAvatar mSelectedItem;
        TrangBiData mSelectedItemData;
        string mItemID;
        public Button btnDucLo;

        public static PopupDucLoTrangBi Create(string itemID, ERarity rarity)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupDucLoTrangBi");
            instance = go.GetComponent<PopupDucLoTrangBi>();
            instance.mItemID = itemID;
            instance.Init(rarity);

            return instance;
        }

        private void Init(ERarity rarity)
        {
            if (GameClient.instance == null ||
                GameClient.instance.UInfo == null ||
                GameClient.instance.UInfo.ListTrangBi == null)
            {
                return;
            }
            btnDucLo.interactable = false;
            UserInfo uInfo = GameClient.instance.UInfo;
            ERarity matRarity = ERarity.Common;
            if (rarity > ERarity.Common)
            {
                matRarity = (ERarity)(rarity - 1);
            }
            lbTitle.text = string.Format(Localization.Get("popup_duc_lo_trang_bi_desc"), Localization.Get("Rariry_" + matRarity.ToString()));
            
            var heroData = uInfo.ListHeroes.Find(e => e.Name == uInfo.Gamer.Hero);

            int countItem = 0;
            for (int i = 0; i < uInfo.ListTrangBi.Count; ++i)
            {
                TrangBiData tbData = uInfo.ListTrangBi[i];
                if (tbData.Rarity != matRarity || tbData.ID == mItemID) continue;
                ItemAvatar itemAvatar;
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

                itemAvatar.SetItem(tbData.Name, tbData.Rarity, tbData.Level);
                var itemBtn = itemAvatar.GetComponent<Button>();
                itemBtn.onClick.AddListener(() => OnItemClick(itemAvatar, tbData));
                
                bool isEquip = false;
                if(heroData != null && heroData.ListSlots != null)
                {
                    foreach (var slot in heroData.ListSlots)
                    {
                        if (string.IsNullOrEmpty(slot.TrangBi) == false && slot.TrangBi == tbData.ID)
                        {
                            isEquip = true;
                            break;
                        }
                    }
                }
                itemAvatar.IsEquiped = isEquip;
                itemAvatar.gameObject.SetActive(true);
                
            }

            for (int i = countItem; i < mListItems.Count; ++i)
            {
                mListItems[i].gameObject.SetActive(false);
            }
            emptyList.gameObject.SetActive(countItem == 0);
            itemPrefab.gameObject.SetActive(false);
        }

        void OnItemClick(ItemAvatar selectedItem, TrangBiData selectedItemData)
        {
            if (mSelectedItem != null) mSelectedItem.IsSelected = false;
            mSelectedItem = selectedItem;
            mSelectedItem.IsSelected = true;
            mSelectedItemData = selectedItemData;

            btnDucLo.interactable = true;
            if (checkDoubleTap)
            {
                PopupTrangBiInfo.CreateReadonly(selectedItemData);
            }
            else
            {
                StartCoroutine(DoubleTapTime(0.2f));
            }
        }


        bool checkDoubleTap = false;
        IEnumerator DoubleTapTime(float time)
        {
            checkDoubleTap = true;
            yield return new WaitForSeconds(time);
            checkDoubleTap = false;
        }

        public static void Dismiss()
        {
            if (instance != null)
            {
                instance.OnCloseBtnClick();
            }
        }

        [GUIDelegate]
        public void OnDucLoBtnClick()
        {
            GameClient.instance.RequestAddCardSlot(mItemID, mSelectedItemData.ID);
            Dismiss();
        }
    }
}