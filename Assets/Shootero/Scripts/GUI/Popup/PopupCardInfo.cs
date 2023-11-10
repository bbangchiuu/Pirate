using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    public class PopupCardInfo : PopupBase
    {
        public Text lbItemName;
        public Text lbRarity;
        public CardAvatar avatar;
        public Text lbDesc;
        public Text lbUnique;
        public Text lbStatDesc;
        public Text lbRecycleRes;
        public Button btnRecycle;
        public Button btnGoToEquip;
        public Button btnEquip;
        public Button btnUnequip;
        public Button btnSwap;

        public RectTransform grpInfo;
        public RectTransform grpStat;
        public RectTransform grpBtns;

        public static PopupCardInfo instance;

        CardData mItemData;
        bool mIsEquiped = false;
        bool mShowBtnRecycle = true;
        bool mShowBtnEquip = true;
        System.Action mOnEquipOrUnequip;
        System.Action mOnSwap;

        public static PopupCardInfo Create(CardData itemData, bool isEquiped, bool showBtnRecycle = true, bool showBtnEquip = true, System.Action OnEquipOrUnequip = null, System.Action OnSwap = null)
        {
            var popup = Create();
            popup.mOnEquipOrUnequip = OnEquipOrUnequip;
            popup.mOnSwap = OnSwap;
            popup.Init(itemData, isEquiped, showBtnRecycle, showBtnEquip);
            return popup;
        }
        public static PopupCardInfo CreateViewOnly(string cardName)
        {
            var popup = Create();
            CardConfig cardCfg = ConfigManager.CardStats[cardName];
            CardData itemData = new CardData();
            itemData.Name = cardName;
            itemData.Rarity = cardCfg.Rarity;
            itemData.ID = "";
            itemData.GID = 0;
            popup.Init(itemData, false, false, false);
            return popup;
        }

        static PopupCardInfo Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupCardInfo");
            instance = go.GetComponent<PopupCardInfo>();
            return instance;
        }

        public void SyncNetworkData()
        {
            if (mItemData != null)
            {
                var itemData = GameClient.instance.UInfo.listCards.Find(e => e.ID == mItemData.ID);
                if (itemData != null)
                {
                    Init(itemData, mIsEquiped, mShowBtnRecycle, mShowBtnEquip);
                }
                else
                {
                    Dismiss();
                }
            }
        }

        void InitStat(CardData itemData)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine();

            var itemCfg = ConfigManager.CardStats[itemData.Name];
            var cardStats = itemCfg.Stats;

            //for (int i = 0; i < basicStats.Length; ++i)
            //{
            //    string basic_stats_str = ConfigManager.GetStatDesc(basicStats[i], itemData.Level);
            //    double inc = 0;
            //    if (nextStats != null && nextStats.Length > i)
            //    {
            //        inc = nextStats[i].Val - basicStats[i].Val;
            //    }
            //    string upgrade_stats_str = "";

            //    if (!isMaxUpgrade && showNextLevel)
            //    {
            //        upgrade_stats_str = string.Format(" (<color=lime>+{0}</color>)", (long)inc);
            //        basic_stats_str += upgrade_stats_str;
            //    }

            //    sb.AppendLine(basic_stats_str);
            //}

            for (int i = 0; i < cardStats.Length; ++i)
            {
                var statCfg = cardStats[i];

                if (itemCfg.Slot == SlotType.VuKhi &&
                    (statCfg.Stat == EStatType.ATK_SPD || statCfg.Stat == EStatType.KNOCKBACK) &&
                    statCfg.Mod == EStatModType.ADD)
                {
                    if (statCfg.Stat == EStatType.ATK_SPD)
                    {
                        if (statCfg.Val >= 140 && statCfg.Val < 150)
                        {
                            sb.AppendLine(Localization.Get("AtkSpdSlow"));
                        }
                        else if (statCfg.Val >= 160 && statCfg.Val < 200)
                        {
                            sb.AppendLine(Localization.Get("AtkSpdMedium"));
                        }
                        else if (statCfg.Val >= 210)
                        {
                            sb.AppendLine(Localization.Get("AtkSpdFast"));
                        }
                    }
                }
                else
                {
                    sb.AppendLine(ConfigManager.GetStatDesc(statCfg));
                }
            }
            lbStatDesc.text = sb.ToString();
        }

        private void Init(CardData itemData)
        {
            mItemData = itemData;
            lbItemName.text = string.Format(Localization.Get("CardName_Display") , Localization.Get(itemData.Name + "_Display")) ;
            
            CardConfig item_cfg = ConfigManager.CardStats[itemData.Name];

            lbRarity.text = Localization.Get("Rariry_" + itemData.Rarity.ToString());
            string desc = string.Format(Localization.Get("popup_card_info_desc"), Localization.Get(item_cfg.Slot.ToString()));
            lbDesc.text = desc;
            lbUnique.gameObject.SetActive(item_cfg.MaxDrop == 1);

            avatar.SetItem(itemData.Name, itemData.Rarity, itemData.ID);
        }

        private void Init(CardData itemData, bool isEquiped, bool showBtnRecycle = true, bool showBtnEquip = true)
        {
            mIsEquiped = isEquiped;
            mShowBtnRecycle = showBtnRecycle;
            mShowBtnEquip = showBtnEquip;
            Init(itemData);
            InitStat(itemData);
            grpBtns.gameObject.SetActive(true);

            btnEquip.gameObject.SetActive(mIsEquiped == false && showBtnEquip);
            btnUnequip.gameObject.SetActive(mIsEquiped && showBtnEquip);
            btnSwap.gameObject.SetActive(mOnSwap != null);
            btnGoToEquip.gameObject.SetActive(showBtnRecycle);
            btnRecycle.gameObject.SetActive(showBtnRecycle);
            if (showBtnRecycle)
            {
                lbRecycleRes.text = "+" + ConfigManager.GetRecycleCardDustByRarity(itemData.Rarity);
                LayoutRebuilder.ForceRebuildLayoutImmediate(lbRecycleRes.transform.parent as RectTransform);
            }
            if (!showBtnRecycle && !showBtnEquip)
                grpBtns.gameObject.SetActive(false);
            else
                grpBtns.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(grpInfo.transform.parent as RectTransform);
        }

        [GUIDelegate]
        public void OnBtnRecycleClick()
        {
            if (mItemData != null)
            {
                PopupConfirm.Create(Localization.Get("confirm_recycle_card"), () =>
                {
                    GameClient.instance.RequestRecycleCard(new string[] { mItemData.ID });
                    OnBackBtnClick();
                }, true);
            }
        }
        [GUIDelegate]
        public void OnBtnEquipClick()
        {
            if(mOnEquipOrUnequip != null)
            {
                mOnEquipOrUnequip();
                mOnEquipOrUnequip = null;
            }
            Dismiss();
        }
        [GUIDelegate]
        public void OnBtnUnEquipClick()
        {
            if (mOnEquipOrUnequip != null)
            {
                mOnEquipOrUnequip();
                mOnEquipOrUnequip = null;
            }
            Dismiss();
        }

        [GUIDelegate]
        public void OnBtnSwapClick()
        {
            if (mOnSwap != null)
            {
                mOnSwap();
            }
            //Dismiss();
        }

        [GUIDelegate]
        public void OnBtnGoToEquipClick()
        {
            bool CanEquip = false;
            UserInfo uInfo = GameClient.instance.UInfo;
            var heroData = uInfo.ListHeroes.Find(e => e.Name == uInfo.Gamer.Hero);
            if (heroData != null)
            {
                CardConfig item_cfg = ConfigManager.CardStats[mItemData.Name];
                string slotPrefix = ConfigManager.GetTrangBiPrefixBySlotType(item_cfg.Slot);
                foreach (var slot in heroData.ListSlots)
                {
                    if (string.IsNullOrEmpty(slot.TrangBi) == false)
                    {
                        TrangBiData tbData = uInfo.ListTrangBi.Find(e => e.ID == slot.TrangBi);
                        if (tbData != null && tbData.CardSlots != null && tbData.Name.StartsWith(slotPrefix))
                        {
                            CanEquip = true;
                            PopupTrangBiInfo.Create(tbData, true, true);
                            break;
                        }
                    }
                }
            }
            if(CanEquip == false)
            {
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("cant_equip"));
            }
        }

        public static void Dismiss()
        {
            if (instance != null)
            {
                instance.OnCloseBtnClick();
            }
        }

        public override void OnCloseBtnClick()
        {
            if (mOnEquipOrUnequip != null)
            {
                mOnEquipOrUnequip = null;
            }
            if (mOnSwap != null)
            {
                mOnSwap = null;
            }
            base.OnCloseBtnClick();
        }
    }
}