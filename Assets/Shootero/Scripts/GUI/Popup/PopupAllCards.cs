using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using System;
    using UnityEngine.UI;
    public class PopupAllCards : PopupBase
    {
        public static PopupAllCards instance;

        public Transform itemParent;
        public CardAvatar itemPrefab;
        public Text emptyList;
        public Text lbTiLeMoHom;
        List<CardAvatar> mListItems = new List<CardAvatar>();
        CardAvatar mSelectedItem;
        CardData mSelectedItemData;
        int mSlotIndex;

        public static PopupAllCards Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupAllCards");
            instance = go.GetComponent<PopupAllCards>();
            instance.Init();

            return instance;
        }

        private void Init()
        {
            if (GameClient.instance == null ||
                GameClient.instance.UInfo == null ||
                GameClient.instance.UInfo.listCards == null)
            {
                return;
            }

            {
                var chestCfg = ConfigManager.ChestCfg["Chest_UnitCard"];
                List<int> drops = new List<int>();
                for (int d = chestCfg.dropRates.Length - 1; d >= 0; --d)
                {
                    if (chestCfg.dropRates[d] > 0)
                    {
                        drops.Add(chestCfg.dropRates[d]);
                    }
                }
                string localize = Localization.Get("popup_all_cards_desc");
                var args = new object[drops.Count];
                for (int i = 0; i < drops.Count; ++i)
                {
                    args[i] = drops[i];
                }

                lbTiLeMoHom.text = string.Format(localize, args);
            }

            UserInfo uInfo = GameClient.instance.UInfo;
            int countItem = 0;
            foreach (string cardName in ConfigManager.CardStats.Keys)
            {
                CardConfig cardCfg = ConfigManager.CardStats[cardName];

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

                itemAvatar.SetItem(cardName, cardCfg.Rarity);
                var itemBtn = itemAvatar.GetComponent<Button>();
                itemBtn.onClick.AddListener(() => OnItemClick(cardName));
                itemAvatar.gameObject.SetActive(true);

                bool haveThisCard = uInfo.listCards.Exists(e => e.Name == cardName);
                itemAvatar.IsLocked = !haveThisCard;
            }

            for (int i = countItem; i < mListItems.Count; ++i)
            {
                mListItems[i].gameObject.SetActive(false);
            }
            emptyList.gameObject.SetActive(countItem == 0);
            itemPrefab.gameObject.SetActive(false);
        }

        void OnItemClick(string cardName)
        {
            PopupCardInfo.CreateViewOnly(cardName);
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