using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using System;
    using UnityEngine.UI;

    public class PopupTinhLuyenDoiTB : PopupBase
    {
        public ItemAvatar sourceItem, resultItem, matItem;
        public Text lbLevelMaxSourceItem, lbLevelMaxResultItem;
        public static PopupTinhLuyenDoiTB instance;

        public GameObject grpMythicItems;
        public Transform itemMythicParent;
        public ItemAvatar itemMythicPrefab;

        public GameObject grpMatItems;
        public Transform itemMatParent;
        public ItemAvatar itemMatPrefab;

        public GameObject grpMatBG;
        public GameObject grpUnlockedItems;
        public Transform itemUnlockedParent;
        public ItemAvatar itemUnlockedPrefab;
        public ItemAvatar itemUnlockedRandom;
        public GameObject grpUnlockRandomFunction;
        public Text lbListReforgedItems;
        public Text lbUnlockRandom;
        public Text lbFree, lbPlus;
        List<ItemAvatar> mListUnlockedItems = new List<ItemAvatar>();
        string mUnlockedItemName = "";
        string mUnlockedRandomName = "";


        public GameObject grpSelectSlot, grpMain;
        public GameObject grpResult;
        public ItemAvatar rewardItem;
        TrangBiData enhanceItemData;
        TrangBiData matItemData;
        public static PopupTinhLuyenDoiTB Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupTinhLuyenDoiTB");
            instance = go.GetComponent<PopupTinhLuyenDoiTB>();
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
                if (itemRarity == ERarity.Mysthic)
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

        public void InitUnlockedItems(TrangBiData itemData)
        {
            UserInfo uInfo = GameClient.instance.UInfo;
            string itemType = itemData.Name.Substring(0, 2);
            if (uInfo.Gamer.reforgeHistory == null || !uInfo.Gamer.reforgeHistory.ContainsKey(itemType)) return;
            List<string> unlockedItems = uInfo.Gamer.reforgeHistory[itemType];
            if (unlockedItems == null) unlockedItems = new List<string>();

            for (int i = 0; i < unlockedItems.Count; i++)
            {
                string unlockedItemName = unlockedItems[i];
                //if (itemData.Name == unlockedItemName) continue;
                ItemAvatar itemAvatar;
                if (i < mListUnlockedItems.Count)
                {
                    itemAvatar = mListUnlockedItems[i];
                }
                else
                {
                    itemAvatar = Instantiate(itemUnlockedPrefab, itemUnlockedParent);
                    mListUnlockedItems.Add(itemAvatar);
                }
                itemAvatar.name = unlockedItemName;
                itemAvatar.SetItem(unlockedItemName, itemData.Rarity, itemData.Level, itemData.Star, false);
                var itemBtn = itemAvatar.GetComponent<Button>();
                itemBtn.onClick.AddListener(() => 
                {
                    mUnlockedItemName = unlockedItemName;
                    OnUnlockedItemClick();
                });
                itemAvatar.gameObject.SetActive(true);
            }

            if(unlockedItems.Count >= ConfigManager.GetNumberOfMythicItemBySlot(itemType))
            {
                grpUnlockRandomFunction.SetActive(false);
            }
            else
            {
                grpUnlockRandomFunction.SetActive(true);
            }

            if(mListUnlockedItems.Count > unlockedItems.Count)
            {
                for(int i = unlockedItems.Count;i < mListUnlockedItems.Count; i++)
                {
                    mListUnlockedItems[i].gameObject.SetActive(false);
                }
            }
            lbListReforgedItems.gameObject.SetActive(unlockedItems.Count > 0);
            grpUnlockedItems.SetActive(false);
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
                string slotType = itemData.Name.Substring(0, 2);
                //lbLevelMaxSourceItem.text = string.Format(Localization.Get("lbLevelMax"), ConfigManager.GetMaxLevelByRarity(enhanceItemData.Name, enhanceItemData.Rarity, enhanceItemData.Star));
                if (slotType == "VK")
                {
                    mUnlockedRandomName = "MythicWeapon";
                    
                }
                else if (slotType == "AG")
                {
                    mUnlockedRandomName = "MythicArmor";
                }
                else if (slotType == "MU")
                {
                    mUnlockedRandomName = "MythicHelm";
                }
                else if (slotType == "OH")
                {
                    mUnlockedRandomName = "MythicOffhand";
                }
                mUnlockedItemName = mUnlockedRandomName;
                
                if (GameClient.instance.UInfo.Gamer.reforgeHistory != null
                    && GameClient.instance.UInfo.Gamer.reforgeHistory.ContainsKey(slotType)
                    && GameClient.instance.UInfo.Gamer.reforgeHistory[slotType] != null
                    && GameClient.instance.UInfo.Gamer.reforgeHistory[slotType].Count >= ConfigManager.GetNumberOfMythicItemBySlot(slotType))
                {
                    mUnlockedItemName = enhanceItemData.Name;
                    resultItem.SetItem(mUnlockedItemName, enhanceItemData.Rarity, enhanceItemData.Level, enhanceItemData.Star, false);
                    lbFree.gameObject.SetActive(true);
                    lbPlus.gameObject.SetActive(false);
                    lbUnlockRandom.gameObject.SetActive(false);
                }
                else
                {
                    resultItem.SetItem(mUnlockedItemName, enhanceItemData.Rarity, enhanceItemData.Level, enhanceItemData.Star, false);
                    lbFree.gameObject.SetActive(false);
                    lbPlus.gameObject.SetActive(true);
                    lbUnlockRandom.gameObject.SetActive(true);
                }
                itemUnlockedRandom.SetItem(mUnlockedItemName, enhanceItemData.Rarity, enhanceItemData.Level, enhanceItemData.Star, false);
                InitUnlockedItems(enhanceItemData);

                //lbLevelMaxResultItem.text = string.Format(Localization.Get("lbLevelMax"), ConfigManager.GetMaxLevelByRarity(enhanceItemData.Name, enhanceItemData.Rarity, enhanceItemData.Star + 1));

                grpMain.SetActive(true);
                grpSelectSlot.SetActive(false);
            }
        }

        void OnMythicItemClick(ItemAvatar selectedItem)
        {
            grpMythicItems.SetActive(false);
            SetEnhanceItem(selectedItem.name);
        }

        void OnUnlockedItemClick()
        {
            lbFree.gameObject.SetActive(true);
            lbPlus.gameObject.SetActive(false);
            lbUnlockRandom.gameObject.SetActive(false);
            resultItem.SetItem(mUnlockedItemName, enhanceItemData.Rarity, enhanceItemData.Level, enhanceItemData.Star, false);
            grpUnlockedItems.SetActive(false);
        }

        [GUIDelegate]
        public void OnUnlockedRandomItemClick()
        {
            lbFree.gameObject.SetActive(false);
            lbPlus.gameObject.SetActive(true);
            lbUnlockRandom.gameObject.SetActive(true);
            mUnlockedItemName = mUnlockedRandomName;
            resultItem.SetItem(mUnlockedItemName, enhanceItemData.Rarity, enhanceItemData.Level, enhanceItemData.Star, false);
            grpUnlockedItems.SetActive(false);
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
            if (lbPlus.gameObject.activeSelf)
            {
                if (matItemData == null)
                {
                    PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("AddLegendItemMaterial"));
                }
                else if (enhanceItemData != null)
                {
                    PopupConfirm.Create(Localization.Get("popup_tinh_luyen_doi_tb_confirm"), () =>
                    {
                        GameClient.instance.RequestReforgeMysthicItem(enhanceItemData.ID, matItemData.ID);
                    }, true);

                }
            }
            else
            {
                if (!string.IsNullOrEmpty(mUnlockedItemName))
                {
                    GameClient.instance.RequestChangeStyleMysthicItem(enhanceItemData.ID, mUnlockedItemName);
                }
            }
        }

        [GUIDelegate]
        public void OnMaterialItemBtnClick()
        {
            if (lbPlus.gameObject.activeSelf) grpMatItems.SetActive(true);
        }

        [GUIDelegate]
        public void OnResultItemBtnClick()
        {
            grpUnlockedItems.SetActive(true);
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
            else if (grpUnlockedItems.activeSelf)
            {
                grpUnlockedItems.SetActive(false);
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