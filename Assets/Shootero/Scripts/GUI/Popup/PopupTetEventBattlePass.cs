using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    public class PopupTetEventBattlePass : PopupBase
    {
        public static PopupTetEventBattlePass instance;
        public List<PopupTetEventBattlePassItem> listItems;
        public GameObject itemPref;
        public Transform grpItems;
        public ScrollRect scrollView;
        public Button btnPurchase;
        public Text lbPrice;
        public Text lbPremium;
        public Text lbExp;
        public Slider sliderExp;
        public GameObject grpBuyLevel;
        public Button btnBuyLevel;
        public Text lbBuyLevelCost;
        public Text lbBuyLevelDesc;

        string mPackageName = "TetEventBattlePass";

        public static PopupTetEventBattlePass Create()
        {
            UserInfo uInfo = GameClient.instance.UInfo;
            if (uInfo == null || uInfo.tetEventGamerData == null) return null;

            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupTetEventBattlePass");
            instance = go.GetComponent<PopupTetEventBattlePass>();
            instance.Init();
            return instance;
        }

        public void Init()
        {
            UserInfo uInfo = GameClient.instance.UInfo;
            
            float scrollPosition = 1;
            int currentLevel = uInfo.tetEventGamerData.GetCurrentLevel();
            long currentExp = uInfo.tetEventGamerData.currentExp - (long)currentLevel * ConfigManager.TetEventCfg.ExpPerLevel;
            lbPrice.gameObject.SetActive(uInfo.tetEventGamerData.purchased == false);
            lbPremium.gameObject.SetActive(uInfo.tetEventGamerData.purchased == true);
            
            if (lbPrice.gameObject.activeSelf)
            {
                Dictionary<string, OfferStoreConfig> offerCfgs = CashShopUtils.GetTetEventOfferConfigs();
                OfferStoreConfig offerCfg = offerCfgs[mPackageName];

                int price = 0;

                if (price <= 0)
                {
                    string priceStr = offerCfg.Price.Substring(1);
                    price = (int)Mathf.Round(float.Parse(priceStr));
                }
                lbPrice.text = offerCfg.Price;


#if IAP_BUILD
                var local_price = UnityIAPManager.instance.GetLocalPrice(mPackageName);
                if (!string.IsNullOrEmpty(local_price))
                {
                    this.lbPrice.text = local_price;
                }
                this.btnPurchase.interactable = !UnityIAPManager.instance.CheckPurchaseIsMissing(mPackageName);
#endif
                if (btnPurchase.interactable && uInfo.tetEventGamerData.purchased) btnPurchase.interactable = false;
            }

            lbExp.text = string.Format(Localization.Get("lbLevelAndExp"), currentLevel, currentExp, ConfigManager.TetEventCfg.ExpPerLevel);
            sliderExp.value = 1f * currentExp / ConfigManager.TetEventCfg.ExpPerLevel;

            for (int i = 0; i < ConfigManager.TetEventCfg.Rewards.Count; i++)
            {
                bool claimed = uInfo.tetEventGamerData.receivedRewards.Contains(i);
                bool claimedPremium = uInfo.tetEventGamerData.receivedRewards.Contains(i);
                if(scrollPosition == 1)
                {
                    if (claimed == false || claimedPremium == false)
                    {
                        scrollPosition = 0.999f - (1f * i / ConfigManager.TetEventCfg.Rewards.Count);
                        Debug.Log("scrollPosition=" + scrollPosition);
                    }
                }
                if (i < listItems.Count)
                {
                    listItems[i].SetItem(i, currentLevel);
                    listItems[i].gameObject.SetActive(true);
                }
                else
                {
                    GameObject obj = Instantiate(itemPref, grpItems) as GameObject;
                    obj.transform.localScale = Vector3.one;
                    PopupTetEventBattlePassItem item = obj.GetComponent<PopupTetEventBattlePassItem>();
                    item.SetItem(i, currentLevel);
                    item.gameObject.SetActive(true);
                    listItems.Add(item);
                }
            }
            itemPref.SetActive(false);
            for (int i = ConfigManager.TetEventCfg.Rewards.Count; i < listItems.Count; i++)
            {
                listItems[i].gameObject.SetActive(false);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(grpItems as RectTransform);
            
            scrollView.verticalNormalizedPosition = scrollPosition;
            Debug.Log("Set scrollPosition=" + scrollPosition);

            lbBuyLevelDesc.text = string.Format(Localization.Get("LbBuyLevelBattlePassDesc"), ConfigManager.TetEventCfg.ExpPerLevel);
            lbBuyLevelCost.text = ConfigManager.TetEventCfg.BuyLevelGemCost + "";
            LayoutRebuilder.ForceRebuildLayoutImmediate(lbBuyLevelCost.transform.parent as RectTransform);
            btnBuyLevel.interactable = uInfo.Gamer.Gem >= ConfigManager.TetEventCfg.BuyLevelGemCost;
            btnBuyLevel.gameObject.SetActive(currentLevel < ConfigManager.TetEventCfg.Rewards.Count);
        }

        public void RequestClaimRewards()
        {
            GameClient.instance.RequestClaimTetEventRewards();
        }

        [GUIDelegate]
        public void OnBuyBtnClick()
        {
            if (GameClient.instance.UInfo.tetEventGamerData.purchased) return;

#if IAP_BUILD
            if (UnityIAPManager.instance.CheckLoginToPurchase() == false)
            {
                return;
            }
#endif

            Dictionary<string, OfferStoreConfig> offerCfgs = CashShopUtils.GetTetEventOfferConfigs();
#if UNITY_STANDALONE || GM_BUILD
            //  string testReceip = "{\"Store\":\"GooglePlay\",\"TransactionID\":\"GPA.3338-2557-9554-32252\",\"Payload\":\"{\\\"json\\\":\\\"{\\\\\\\"orderId\\\\\\\":\\\\\\\"GPA.3338-2557-9554-32252\\\\\\\",\\\\\\\"packageName\\\\\\\":\\\\\\\"com.hikergames.caravanwar\\\\\\\",\\\\\\\"productId\\\\\\\":\\\\\\\"com.hikergames.caravanwar.personal_deal_2\\\\\\\",\\\\\\\"purchaseTime\\\\\\\":1506941883100,\\\\\\\"purchaseState\\\\\\\":0,\\\\\\\"purchaseToken\\\\\\\":\\\\\\\"dodinjlppgaikbpakneimfij.AO-J1Ozuie_qSeca-Tml9P51Gl2VSX1w_uygJ268khknuD8MDMR72ou81jBPA6bjIK_SIGjhl1w-ve8M5zKxmSAoLhSUEwqhWBemjWtFwufwhd-NsH6lyCsLicwarUS-GgGOUbe6d471ZHZnkRBvY7bkwtZwOpWcs9cuBEkIhhA5fSFrOGZxL5A\\\\\\\"}\\\",\\\"signature\\\":\\\"UMS1HqoIGhGV7e5fi8gzkruxZgaobuhQSca7AROB8QjP7u1r3qthJC+Qry1uqtTmuKj0dp220D9e7VJVQ5MzsdmIOj9TnMOY61MrkT6jF5j0vg5w93a0ddk\\\\/xs\\\\/oKhLnM5W5juus6eP2kvFwXtQ5SuNDYcP7kbHr8gJ6AAeJLYTeIWCZC6xHYfCb7bC4wy6rCz2tDzk8P1TWIckUP6er4QfknPDhewfjSGiw\\\\/jlKYsFfoM6X669JR6FgutWT2fSslNVRF+JQTfKOPuUrHvBvndJnUUbExiVC57lRoOfDYDfzK\\\\/WxtpzkTddrnd8UnOER+mgajHYbcBe8nb3ZvD0oDA==\\\"}\"}";
            //PopupConfirmInputText.Create("Buy Gem Pack", "Password require for sercurity!", (pass) =>
            //{
            GameClient.instance.RequestBuyTetEventOffer(mPackageName, string.Empty, "asvrtr4654hfghfgh");
            //});
            return;
#endif

#if IAP_BUILD
            if (UnityIAPManager.instance != null)
                UnityIAPManager.instance.BuyStorePack(mPackageName);

            AnalyticsManager.LogEvent("IAP_CLICK",
                new AnalyticsParameter("Name", mPackageName),
                new AnalyticsParameter("Value", offerCfgs[mPackageName].Price));
#endif
        }

        [GUIDelegate]
        public void OnBtnShowGrpBuyLevel()
        {
            grpBuyLevel.SetActive(true);
        }

        [GUIDelegate]
        public void OnBtnHideGrpBuyLevel()
        {
            grpBuyLevel.SetActive(false);
        }

        [GUIDelegate]
        public void OnBtnBuyLevel()
        {
            grpBuyLevel.SetActive(false);
            GameClient.instance.RequestBuyTetEventLevel();
        }

        [GUIDelegate]
        public override void OnCloseBtnClick()
        {
            base.OnCloseBtnClick();
        }
    }
}