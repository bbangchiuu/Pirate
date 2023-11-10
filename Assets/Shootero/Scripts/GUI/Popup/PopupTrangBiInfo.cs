using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    public class PopupTrangBiInfo : PopupBase
    {
        public Text lbItemName;
        public Text lbRarity;
        public ItemAvatar avatar;
        public Text lbLevel;
        public Text lbDesc;
        public Text lbStatDesc;
        public Text lbGoldRes;
        public Button btnUpgrade;
        public Button btnEquip;
        public Button btnUnequip;

        public MaterialAvatar[] listUpgradeMaterials;

        public RectTransform grpInfo;
        public RectTransform grpStat;
        public RectTransform grpUpgrade;
        public RectTransform grpBtns;
        public RectTransform grpCards;

        public Transform cardParent;
        public GameObject cardPref;
        public List<CardAvatar> listEquipedCards;
        public Button btnUnlockCardSlot;

        public static PopupTrangBiInfo instance;

        TrangBiData mItemData;
        bool mIsEquiped = false;
        bool mViewOnly = false;
        bool mHideAllButtons = false;

        public bool IsReadonly { get; private set; }

        public static PopupTrangBiInfo Create(TrangBiData itemData, bool isEquiped, bool hideAllButtons = false)
        {
            var popup = Create();
            popup.Init(itemData, isEquiped, hideAllButtons);
            return popup;
        }

        public static PopupTrangBiInfo CreateReadonly(TrangBiData itemData, int highlightMaxLevel = 0)
        {
            var popup = Create();
            popup.InitViewOnly(itemData, highlightMaxLevel);
            return popup;
        }

        static PopupTrangBiInfo Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupTrangBiInfo");
            instance = go.GetComponent<PopupTrangBiInfo>();
            return instance;
        }

        public void SyncNetworkData()
        {
            if (IsReadonly) return;
            if (mItemData != null)
            {
                var itemData = GameClient.instance.UInfo.ListTrangBi.Find(e => e.ID == mItemData.ID);
                if (itemData != null)
                {
                    Init(itemData, mIsEquiped);
                }
                else
                {
                    Dismiss();
                }
            }
        }

        void InitStat(TrangBiData itemData, bool showNextLevel)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine();

            var basicStats = UnitStatUtils.GetBasicStatFromEquipment(itemData);
            var advStats = UnitStatUtils.GetAdvanceStatFromEquipment(itemData);
            var itemCfg = ConfigManager.GetItemConfig(itemData.Name);

            bool isMaxUpgrade = false;
            int maxLevel = ConfigManager.GetMaxLevelByRarity(itemData.Name, itemData.Rarity, itemData.Star);
            var requirements = ConfigManager.GetItemUpgradeRequirement(itemData.Name, itemData.Level, out isMaxUpgrade);
            if (maxLevel <= itemData.Level)
            {
                isMaxUpgrade = true;
            }
            

            StatMod[] nextStats = null;
            if (isMaxUpgrade == false)
            {
                nextStats = UnitStatUtils.GetBasicStatFromEquipment(itemData.Name, itemData.Level + 1, itemData.Rarity);
            }

            for (int i = 0; i < basicStats.Length; ++i)
            {
                string basic_stats_str = ConfigManager.GetStatDesc(basicStats[i], itemData.Level);
                double inc = 0;
                if (nextStats != null && nextStats.Length > i)
                {
                    inc = nextStats[i].Val - basicStats[i].Val;
                }
                string upgrade_stats_str = "";

                if (!isMaxUpgrade && showNextLevel)
                {
                    upgrade_stats_str = string.Format(" (<color=lime>+{0}</color>)", (long)inc);
                    basic_stats_str += upgrade_stats_str;
                }

                sb.AppendLine(basic_stats_str);
            }

            //if (advStats.Length > 0)
            //{
            //    sb.AppendLine("---");
            //}

            for (int i = 0; i < advStats.Length; ++i)
            {
                var statCfg = advStats[i];

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
                    sb.AppendLine(ConfigManager.GetStatDesc(statCfg, itemData.Level));
                }
            }
            lbStatDesc.text = sb.ToString();
        }
        void InitUpgradeInfo(TrangBiData itemData)
        {
            bool isMaxUpgrade = false;
            var requirements = ConfigManager.GetItemUpgradeRequirement(itemData.Name, itemData.Level, out isMaxUpgrade);
            long goldRequirement = 0;

            StatMod[] nextStats = null;
            if (isMaxUpgrade == false)
            {
                nextStats = UnitStatUtils.GetBasicStatFromEquipment(itemData.Name, itemData.Level + 1, itemData.Rarity);
            }

            List<UpgradeRequirement> materialRequirements = new List<UpgradeRequirement>();
            for (int i = 0; i < requirements.Length; ++i)
            {
                var re = requirements[i];
                if (re.Res == ConfigManager.GoldName)
                {
                    lbGoldRes.text = re.Num.ToString();
                    goldRequirement = re.Num;
                }
                else
                {
                    materialRequirements.Add(re);
                }
            }

            for (int i = 0; i < listUpgradeMaterials.Length; ++i)
            {
                if (i < materialRequirements.Count)
                {
                    listUpgradeMaterials[i].gameObject.SetActive(true);
                    listUpgradeMaterials[i].SetItem(materialRequirements[i].Res, materialRequirements[i].Num);
                    var requirementNum = materialRequirements[i].Num;
                    MaterialData matData = null;
                    if (GameClient.instance != null &&
                        GameClient.instance.UInfo != null &&
                        GameClient.instance.UInfo.ListMaterials != null)
                    {
                        matData = GameClient.instance.UInfo.ListMaterials.Find(e => e.Name == materialRequirements[i].Res);
                    }
                    var haveNum = matData != null ? matData.Num : 0;
                    if (haveNum >= requirementNum)
                    {
                        listUpgradeMaterials[i].lbQuantity.text = string.Format(Localization.Get("MaterialUpgradeCountFormat"), Localization.Get(materialRequirements[i].Res + "_Name"), requirementNum, haveNum);
                    }
                    else
                    {
                        listUpgradeMaterials[i].lbQuantity.supportRichText = true;
                        listUpgradeMaterials[i].lbQuantity.text = string.Format(Localization.Get("MaterialUpgradeCountFormat2"), Localization.Get(materialRequirements[i].Res + "_Name"), requirementNum, haveNum);
                    }
                }
                else
                {
                    listUpgradeMaterials[i].gameObject.SetActive(false);
                }
            }

            if (listUpgradeMaterials.Length > 0)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(listUpgradeMaterials[0].transform.parent as RectTransform);
            }

            var uInfo = GameClient.instance.UInfo;
            if (uInfo.Gamer.Gold >= goldRequirement)
            {
                btnUpgrade.interactable = true;
            }
            else
            {
                btnUpgrade.interactable = false;
            }

            //lbGoldRes.Rebuild(CanvasUpdate.Prelayout);
            LayoutRebuilder.ForceRebuildLayoutImmediate(lbGoldRes.transform.parent as RectTransform);
            //Hiker.HikerUtils.DoAction(this,
            //    () => LayoutRebuilder.ForceRebuildLayoutImmediate(lbGoldRes.transform.parent as RectTransform), 0.15f, true);

        }

        private void InitCardInfo(TrangBiData itemData, bool viewOnly = false)
        {
            if (GameClient.instance == null
                || GameClient.instance.UInfo == null
                || GroupChapter.IsLockSanThe(GameClient.instance.UInfo))

            {
                grpCards.gameObject.SetActive(false);
                return;
            }
            if (itemData == null || itemData.CardSlots == null) return;
            mViewOnly = viewOnly;
            btnUnlockCardSlot.gameObject.SetActive(false);

            for (int i = 0;i < itemData.CardSlots.Count; i++)
            {
                CardData cardData = GameClient.instance.UInfo.listCards.Find(e => e.ID == itemData.CardSlots[i]);
                //if (viewOnly && cardData == null) continue;
                if(i >= listEquipedCards.Count)
                {
                    GameObject obj = Instantiate(cardPref, cardParent) as GameObject;
                    CardAvatar newCard = obj.GetComponent<CardAvatar>();
                    var itemBtn = obj.GetComponent<Button>();
                    if(!viewOnly) itemBtn.onClick.AddListener(() => OnCardSlotClick(newCard));
                    listEquipedCards.Add(newCard);
                }
                if (cardData != null)
                    listEquipedCards[i].SetItem(cardData.Name, cardData.Rarity, cardData.ID);
                else
                    listEquipedCards[i].IsEmpty = true;
                listEquipedCards[i].gameObject.SetActive(true);
            }

            if(itemData.CardSlots.Count < listEquipedCards.Count)
            {
                for (int i = itemData.CardSlots.Count; i < listEquipedCards.Count; i++)
                {
                    listEquipedCards[i].gameObject.SetActive(false);
                }
            }

            cardPref.SetActive(false);
            cardPref.transform.SetParent(cardParent.parent, false);


            if(itemData.CardSlots.Count < ConfigManager.GetMaxCardSlotByRarity(itemData.Rarity) && !viewOnly)
            {
                btnUnlockCardSlot.transform.SetAsLastSibling();
                btnUnlockCardSlot.gameObject.SetActive(true);
            }
            else
            {
                btnUnlockCardSlot.gameObject.SetActive(false);
            }
        }

        void OnCardSlotClick(CardAvatar cardAvatar)
        {
            if (mViewOnly) return;
            int slotIndex = listEquipedCards.IndexOf(cardAvatar);
            if (cardAvatar.IsEmpty)
            {
                PopupLapThe.Create(mItemData, slotIndex);
            }
            else
            {
                CardData cardData = GameClient.instance.UInfo.listCards.Find(e => e.ID == cardAvatar.TBId);
                if (cardAvatar == null) return;
                PopupCardInfo.Create(cardData, true, false, true, 
                    () =>
                    {
                    
                        mItemData.CardSlots[slotIndex] = null;
                        GameClient.instance.RequestUpdateCardSlot(mItemData.ID, mItemData.CardSlots);
                    },
                    () =>
                    {
                        PopupLapThe.Create(mItemData, slotIndex);
                    }
                );
            }
        }

        private void Init(TrangBiData itemData, int highlightMaxLevel = 0)
        {
            mItemData = itemData;
            lbItemName.text = Localization.Get(itemData.Name + "_Display");
            
            lbDesc.text = Localization.Get(itemData.Name + "_Desc");

            ItemConfig item_cfg = ConfigManager.GetItemConfig(itemData);

            lbRarity.text = Localization.Get(itemData.Rarity.ToString()+ "_" + item_cfg.Slot.ToString() );

            if (highlightMaxLevel > 0)
            {
                lbLevel.text = string.Format(Localization.Get("LevelLabelHighlight"), itemData.Level, ConfigManager.GetMaxLevelByRarity(itemData.Name, itemData.Rarity, itemData.Star));
            }
            else
            {
                lbLevel.text = string.Format(Localization.Get("LevelLabel"), itemData.Level, ConfigManager.GetMaxLevelByRarity(itemData.Name, itemData.Rarity, itemData.Star));
            }
            avatar.SetItem(itemData.Name, itemData.Rarity, itemData.Level, itemData.Star);
        }

        private void InitViewOnly(TrangBiData itemData, int highlightMaxLevel = 0)
        {
            IsReadonly = true;
            Init(itemData, highlightMaxLevel);
            InitStat(itemData, false);
            grpUpgrade.gameObject.SetActive(false);
            grpBtns.gameObject.SetActive(false);
            InitCardInfo(itemData, true);

            LayoutRebuilder.ForceRebuildLayoutImmediate(grpInfo.transform.parent as RectTransform);
        }

        private void Init(TrangBiData itemData, bool isEquiped,bool hideAllButtons = false)
        {
            IsReadonly = false;
            mIsEquiped = isEquiped;
            Init(itemData);
            InitStat(itemData, true);
            grpUpgrade.gameObject.SetActive(true);
            grpBtns.gameObject.SetActive(hideAllButtons == false);

            InitUpgradeInfo(itemData);
            InitCardInfo(itemData);

            btnEquip.gameObject.SetActive(mIsEquiped == false);
            btnUnequip.gameObject.SetActive(mIsEquiped);

            LayoutRebuilder.ForceRebuildLayoutImmediate(grpInfo.transform.parent as RectTransform);
        }

        static Dictionary<string, int> listCheckMat = new Dictionary<string, int>();
        [GUIDelegate]
        public void OnBtnUpgradeClick()
        {
            if (mItemData != null)
            {
                string msg = GameClient.instance.UInfo.CheckUpgradeItem(mItemData,
                    Localization.language,
                    listCheckMat, out long changedGold, out int changedGem);
                if (string.IsNullOrEmpty(msg) == false)
                {
                    PopupMessage.Create(MessagePopupType.TEXT, msg);
                    return;
                }
                GameClient.instance.RequestUpgradeItem(mItemData.ID);
            }
        }
        [GUIDelegate]
        public void OnBtnEquipClick()
        {
            ScreenMain.instance.grpEquipment.LapTrangBi(mItemData);
            ScreenMain.instance.grpEquipment.RequestSetEquip();
            OnCloseBtnClick();
        }
        [GUIDelegate]
        public void OnBtnUnEquipClick()
        {
            ScreenMain.instance.grpEquipment.ThaoTrangBi(mItemData);
            ScreenMain.instance.grpEquipment.RequestSetEquip();
            OnCloseBtnClick();
        }
        [GUIDelegate]
        public void OnBtnUnlockSlotClick()
        {
            if(mItemData != null)
            {
                PopupDucLoTrangBi.Create(mItemData.ID, mItemData.Rarity);
            }
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