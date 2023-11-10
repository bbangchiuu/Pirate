using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using System;
    using UnityEngine.UI;

    public class PopupTinhLuyenNangSao : PopupBase
    {
        public ItemAvatar sourceItem, resultItem, matItem;
        public Text lbLevelMaxSourceItem, lbLevelMaxResultItem;
        public static PopupTinhLuyenNangSao instance;

        public GameObject grpMythicItems;
        public Transform itemMythicParent;
        public ItemAvatar itemMythicPrefab;

        public GameObject grpMatItems;
        public Transform itemMatParent;
        public ItemAvatar itemMatPrefab;

        public GameObject grpSelectSlot, grpMain;
        public GameObject grpResult;
        public ItemAvatar rewardItem;
        TrangBiData enhanceItemData;
        TrangBiData matItemData;
        public static PopupTinhLuyenNangSao Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupTinhLuyenNangSao");
            instance = go.GetComponent<PopupTinhLuyenNangSao>();
            instance.Init();
            return instance;
        }

        bool activeResourceBar = false;
        public void Init()
        {
            activeResourceBar = true;
            if (ResourceBar.instance)
            {
                activeResourceBar = ResourceBar.instance.gameObject.activeSelf;
                ResourceBar.instance.gameObject.SetActive(false);
            }

            grpResult.SetActive(false);
            grpSelectSlot.SetActive(true);
            grpMain.SetActive(false);
            grpMatItems.SetActive(false);
            grpMythicItems.SetActive(false);
            matItem.gameObject.SetActive(false);
            InitItems();
        }

        public void InitItems()
        {
            foreach (TrangBiData itemData in GameClient.instance.UInfo.ListTrangBi)
            {
                var itemRarity = itemData.Rarity;
                if (itemRarity == ERarity.Mysthic && itemData.Star < ConfigManager.GetMaxStarMythicItem())
                {
                    var itemAvatar = Instantiate(itemMythicPrefab, itemMythicParent);
                    itemAvatar.name = itemData.ID;
                    itemAvatar.SetItem(itemData.Name, itemRarity, itemData.Level, itemData.Star);
                    var itemBtn = itemAvatar.GetComponent<Button>();
                    itemBtn.onClick.AddListener(() => OnMythicItemClick(itemAvatar));
                    itemAvatar.gameObject.SetActive(true);
                }

                if (itemRarity == ERarity.Legend)
                {
                    var itemAvatar = Instantiate(itemMatPrefab, itemMatParent);
                    itemAvatar.name = itemData.ID;
                    itemAvatar.SetItem(itemData.Name, itemRarity, itemData.Level, itemData.Star);
                    var itemBtn = itemAvatar.GetComponent<Button>();
                    itemBtn.onClick.AddListener(() => OnLegendItemClick(itemAvatar));
                    itemAvatar.gameObject.SetActive(true);
                }
            }

            grpMythicItems.SetActive(false);
            grpMatItems.SetActive(false);
        }

        Action OnClaimReward;
        public void ReceiveItem(TrangBiData tbData)
        {
            rewardItem.SetItem(tbData.Name, tbData.Rarity, tbData.Level, tbData.Star);
            grpResult.SetActive(true);
        }

        public void SetEnhanceItem(string itemID)
        {
            TrangBiData itemData = GameClient.instance.UInfo.ListTrangBi.Find(e => e.ID == itemID);
            enhanceItemData = itemData;
            if(enhanceItemData != null)
            {
                sourceItem.SetItem(enhanceItemData.Name, enhanceItemData.Rarity, enhanceItemData.Level, enhanceItemData.Star, false);
                lbLevelMaxSourceItem.text = string.Format(Localization.Get("lbLevelMax"), ConfigManager.GetMaxLevelByRarity(enhanceItemData.Name, enhanceItemData.Rarity, enhanceItemData.Star));
                resultItem.SetItem(enhanceItemData.Name, enhanceItemData.Rarity, enhanceItemData.Level, enhanceItemData.Star + 1, false);
                lbLevelMaxResultItem.text = string.Format(Localization.Get("lbLevelMax"), ConfigManager.GetMaxLevelByRarity(enhanceItemData.Name, enhanceItemData.Rarity, enhanceItemData.Star + 1));

                grpMain.SetActive(true);
                grpSelectSlot.SetActive(false);
            }
        }

        void OnMythicItemClick(ItemAvatar selectedItem)
        {
            grpMythicItems.SetActive(false);
            SetEnhanceItem(selectedItem.name);
        }

        void OnLegendItemClick(ItemAvatar selectedItem)
        {
            TrangBiData itemData = GameClient.instance.UInfo.ListTrangBi.Find(e => e.ID == selectedItem.name);
            matItemData = itemData;
            matItem.SetItem(itemData.Name, itemData.Rarity, itemData.Level, itemData.Star, false);
            matItem.gameObject.SetActive(true);
            grpMatItems.SetActive(false);

        }

        [GUIDelegate]
        public void OnSelectItemBtnClick()
        {
            grpMythicItems.SetActive(true);
        }

        [GUIDelegate]
        public void OnEnhanceBtnClick()
        {
            if(matItemData == null)
            {
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("AddNangSaoMaterial"));
            }
            else if(enhanceItemData != null)
            {
                PopupConfirm.Create(Localization.Get("popup_tinh_luyen_nang_sao_confirm"), () =>
                {
                    GameClient.instance.RequestUpStarMysthicItem(enhanceItemData.ID, matItemData.ID);
                }, true);
                
            }
        }

        [GUIDelegate]
        public void OnMaterialItemBtnClick()
        {
            grpMatItems.SetActive(true);
        }

        [GUIDelegate]
        public override void OnBackBtnClick()
        {
            if (grpMythicItems.activeSelf)
            {
                grpMythicItems.SetActive(false);
            }
            else if (grpMatItems.activeSelf)
            {
                grpMatItems.SetActive(false);
            }
            else if (grpMain.activeSelf)
            {
                grpMain.SetActive(false);
                grpSelectSlot.SetActive(true);
            }
            else if (grpSelectSlot.activeSelf)
            {
                OnCloseBtnClick();
                PopupTinhLuyenMainMenu.Create();
            }
        }

        [GUIDelegate]
        public void OnClaimRewardBtnClick()
        {
            OnCloseBtnClick();
        }

        protected override void Hide()
        {
            base.Hide();

            if (ResourceBar.instance)
            {
                if (GUIManager.Instance.CurrentScreen == "Main")
                    activeResourceBar = true;

                ResourceBar.instance.gameObject.SetActive(activeResourceBar);
            }
        }
    }
}