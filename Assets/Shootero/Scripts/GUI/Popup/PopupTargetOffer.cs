using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using System;
    using UnityEngine.UI;

    public class PopupTargetOffer : PopupBase
    {
        public static PopupTargetOffer instance;

        public List<PhanThuongItem> listPhanThuong;
        public Text lbExpireAfter;
        public Text lbPrice;
        public Button btnPurchase;

        TargetOfferDataItem data;

        public static PopupTargetOffer Create(TargetOfferDataItem data)
        {
            if (data == null || data.EndTime < GameClient.instance.ServerTime) return null;
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupTargetOffer");
            instance = go.GetComponent<PopupTargetOffer>();

            instance.Init(data);
            return instance;
        }

        private void Init(TargetOfferDataItem _data)
        {
            data = _data;
            Dictionary<string, OfferStoreConfig> targetOfferCfgs = CashShopUtils.GetTargetOfferConfigs();
            OfferStoreConfig offerCfg = targetOfferCfgs[data.PackageName];
            int c = 0;
            foreach (string rewardKey in offerCfg.Content.Keys)
            {
                listPhanThuong[c].SetItem(rewardKey, offerCfg.Content[rewardKey]);
                listPhanThuong[c].gameObject.SetActive(true);
                c++;
            }
            if (c < listPhanThuong.Count)
            {
                for (int i = c; i < listPhanThuong.Count; i++)
                {
                    listPhanThuong[i].gameObject.SetActive(false);
                }
            }
            int price = 0;

            if (price <= 0)
            {
                string priceStr = offerCfg.Price.Substring(1);
                price = (int)Mathf.Round(float.Parse(priceStr));
            }
            lbPrice.text = offerCfg.Price;


#if IAP_BUILD
            var local_price = UnityIAPManager.instance.GetLocalPrice(_data.PackageName);
            if (!string.IsNullOrEmpty(local_price))
            {
                this.lbPrice.text = local_price;
            }
            this.btnPurchase.interactable = !UnityIAPManager.instance.CheckPurchaseIsMissing(_data.PackageName);
#endif
        }

void UpdateTimer()
        {
            if (data != null)
            {
                var ts = data.EndTime - GameClient.instance.ServerTime;
                if (data.EndTime > GameClient.instance.ServerTime)
                {
                    lbExpireAfter.text = Localization.Get("lbExpireAfter") + " " + string.Format(Localization.Get("TimeCountDownHMS"),
                        Mathf.FloorToInt((float)ts.TotalHours), ts.Minutes, ts.Seconds);
                    if (btnPurchase.interactable == false) btnPurchase.interactable = true;
                }
                else if(btnPurchase.interactable)
                {
                    lbExpireAfter.text = Localization.Get("lbOfferExpired");
                    btnPurchase.interactable = false;
                }
            }
        }

        private void Update()
        {
            UpdateTimer();
        }
        
        public static void Dismiss()
        {
            if (instance != null)
            {
                instance.OnCloseBtnClick();
            }
        }

        [GUIDelegate]
        public void OnBuyBtnClick()
        {
#if IAP_BUILD
            if (UnityIAPManager.instance.CheckLoginToPurchase() == false)
            {
                return;
            }
#endif
            if (data == null) return;
            if (data.PackageName.StartsWith("H_"))
            {
                string heroName = data.PackageName.Substring(2);
                HeroData hero = GameClient.instance.UInfo.ListHeroes.Find(e => e.Name == heroName);
                if(hero != null)
                {
                    GameClient.instance.RequestGetUserInfo(new string[] { "targetoffers" }, false);
                    OnCloseBtnClick();
                    return;
                }
            }
            Dictionary<string, OfferStoreConfig> targetOfferCfgs = CashShopUtils.GetTargetOfferConfigs();
#if UNITY_STANDALONE || GM_BUILD
            //  string testReceip = "{\"Store\":\"GooglePlay\",\"TransactionID\":\"GPA.3338-2557-9554-32252\",\"Payload\":\"{\\\"json\\\":\\\"{\\\\\\\"orderId\\\\\\\":\\\\\\\"GPA.3338-2557-9554-32252\\\\\\\",\\\\\\\"packageName\\\\\\\":\\\\\\\"com.hikergames.caravanwar\\\\\\\",\\\\\\\"productId\\\\\\\":\\\\\\\"com.hikergames.caravanwar.personal_deal_2\\\\\\\",\\\\\\\"purchaseTime\\\\\\\":1506941883100,\\\\\\\"purchaseState\\\\\\\":0,\\\\\\\"purchaseToken\\\\\\\":\\\\\\\"dodinjlppgaikbpakneimfij.AO-J1Ozuie_qSeca-Tml9P51Gl2VSX1w_uygJ268khknuD8MDMR72ou81jBPA6bjIK_SIGjhl1w-ve8M5zKxmSAoLhSUEwqhWBemjWtFwufwhd-NsH6lyCsLicwarUS-GgGOUbe6d471ZHZnkRBvY7bkwtZwOpWcs9cuBEkIhhA5fSFrOGZxL5A\\\\\\\"}\\\",\\\"signature\\\":\\\"UMS1HqoIGhGV7e5fi8gzkruxZgaobuhQSca7AROB8QjP7u1r3qthJC+Qry1uqtTmuKj0dp220D9e7VJVQ5MzsdmIOj9TnMOY61MrkT6jF5j0vg5w93a0ddk\\\\/xs\\\\/oKhLnM5W5juus6eP2kvFwXtQ5SuNDYcP7kbHr8gJ6AAeJLYTeIWCZC6xHYfCb7bC4wy6rCz2tDzk8P1TWIckUP6er4QfknPDhewfjSGiw\\\\/jlKYsFfoM6X669JR6FgutWT2fSslNVRF+JQTfKOPuUrHvBvndJnUUbExiVC57lRoOfDYDfzK\\\\/WxtpzkTddrnd8UnOER+mgajHYbcBe8nb3ZvD0oDA==\\\"}\"}";
            //PopupConfirmInputText.Create("Buy Gem Pack", "Password require for sercurity!", (pass) =>
            //{
            GameClient.instance.RequestBuyTargetOffer(data.PackageName, string.Empty, "asvrtr4654hfghfgh");
            //});
            return;
#endif

#if IAP_BUILD
        if (UnityIAPManager.instance != null)
            UnityIAPManager.instance.BuyStorePack(data.PackageName);

        AnalyticsManager.LogEvent("IAP_CLICK",
            new AnalyticsParameter("Name", data.PackageName),
            new AnalyticsParameter("Value", targetOfferCfgs[data.PackageName].Price));
#endif
        }
    }
}