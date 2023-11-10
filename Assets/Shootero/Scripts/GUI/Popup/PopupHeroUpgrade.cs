using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using LitJson;
    using UnityEngine.UI;

    public class PopupHeroUpgrade : PopupBase
    {
        public Text lbItemName;
        public HeroAvatar avatar;
        public Text lbLevel;
        public Text lbGoldRes;
        public Button btnUpgrade;

        public MaterialAvatar upgradeMaterial;

        public RectTransform grpInfo;
        public RectTransform grpStat;
        public RectTransform grpUpgrade;
        public RectTransform grpBtns;

        public static PopupHeroUpgrade instance;

        public HeroUpgradeStatItem[] basicStatItems;
        public HeroUpgradeStatItem[] advancedStatItems;

        public Transform grpBasicStats, grpAdvanceStats;

        HeroData mHeroData;
        bool mIsEquiped = false;

        public bool IsReadonly { get; private set; }

        public static PopupHeroUpgrade Create(HeroData heroData)
        {
            var popup = Create();
            popup.Init(heroData);
            return popup;
        }

        static PopupHeroUpgrade Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupHeroUpgrade");
            instance = go.GetComponent<PopupHeroUpgrade>();
            return instance;
        }
        
        private void InitStats()
        {
            HeroUpgradeConfig.HeroStatsCfg heroStatsCfg = ConfigManager.HeroUpgradeCfg.Heroes[mHeroData.Name];

            //init basic stats
            for(int i = 0; i < basicStatItems.Length; i++)
            {
                bool isEnable = i < heroStatsCfg.BasicStats.Length;
                if (isEnable)
                {
                    basicStatItems[i].SetBasicItem(heroStatsCfg.BasicStats[i], mHeroData.Level);
                }
                basicStatItems[i].gameObject.SetActive(isEnable);
            }

            //init advenced stats
            for (int i = 0; i < advancedStatItems.Length; i++)
            {
                bool isEnable = i < heroStatsCfg.AdvancedStats.Length;
                if (isEnable)
                {
                    advancedStatItems[i].SetAdvancedItem(heroStatsCfg.AdvancedStats[i].Stat, mHeroData.Level, heroStatsCfg.AdvancedStats[i].unlockLevel);
                }
                advancedStatItems[i].gameObject.SetActive(isEnable);
            }

        }
        public void Init(HeroData heroData)
        {
            mHeroData = heroData;

            if (mHeroData == null) return;
            if (!ConfigManager.HeroUpgradeCfg.Heroes.ContainsKey(mHeroData.Name)) return;

            lbItemName.text = Localization.Get(mHeroData.Name + "_Name");
            avatar.SetItem(mHeroData.Name);
            lbLevel.text = string.Format(Localization.Get("LevelLabel"), mHeroData.Level, ConfigManager.HeroUpgradeCfg.Heroes[mHeroData.Name].LevelMax);

            btnUpgrade.gameObject.SetActive(mHeroData.Level < ConfigManager.HeroUpgradeCfg.Heroes[mHeroData.Name].LevelMax);

            bool isEnoughUpgradeResources = true;
            int goldCost = ConfigManager.GetHeroUpgradeGoldCost(mHeroData.Level);
            int heroStoneCost = ConfigManager.GetHeroUpgradeHeroStoneCost(mHeroData.Level);

            if (goldCost > GameClient.instance.UInfo.Gamer.Gold) isEnoughUpgradeResources = false;
            lbGoldRes.text = goldCost + "";
            LayoutRebuilder.ForceRebuildLayoutImmediate(lbGoldRes.transform.parent as RectTransform);

            MaterialData matData = null;
            if (GameClient.instance != null &&
                GameClient.instance.UInfo != null &&
                GameClient.instance.UInfo.ListMaterials != null)
            {
                matData = GameClient.instance.UInfo.ListMaterials.Find(e => e.Name == "M_HeroStone");
            }
            var haveNum = matData != null ? matData.Num : 0;
            upgradeMaterial.SetItem("M_HeroStone", heroStoneCost);
            if (haveNum >= heroStoneCost)
            {
                upgradeMaterial.lbQuantity.text = string.Format(Localization.Get("MaterialUpgradeCountFormat"), Localization.Get("M_HeroStone_Name"), heroStoneCost, haveNum);
            }
            else
            {
                isEnoughUpgradeResources = false;
                upgradeMaterial.lbQuantity.supportRichText = true;
                upgradeMaterial.lbQuantity.text = string.Format(Localization.Get("MaterialUpgradeCountFormat2"), Localization.Get("M_HeroStone_Name"), heroStoneCost, haveNum);
            }
            btnUpgrade.interactable = isEnoughUpgradeResources;

            InitStats();
        }
        
        [GUIDelegate]
        public void OnBtnUpgradeClick()
        {
            if (mHeroData != null)
            {
                GameClient.instance.RequestHeroUpgrade(mHeroData.ID);
            }
        }

        [GUIDelegate]
        public void OnBtnInfoTalentsClick()
        {
            
        }
        [GUIDelegate]
        public void OnBtnInfoStatsClick()
        {
            
        }
        [GUIDelegate]
        public override void OnCloseBtnClick()
        {
            if (Hiker.GUI.Shootero.PopupSelectHero.instance)
            {
                Hiker.GUI.Shootero.PopupSelectHero.instance.Init();
            }
            base.OnCloseBtnClick();
        }
    }
}