using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using System;
    using UnityEngine.UI;

    public class PopupTruongThanhOffer : PopupBase
    {
        public static PopupTruongThanhOffer instance;

        public List<PhanThuongItem> listPhanThuong;
        public Text lbExpireAfter;
        public Text lbPrice;
        public Button btnPurchase;

        string mPackageName = "TruongThanh";

        public static PopupTruongThanhOffer Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupTruongThanhOffer");
            instance = go.GetComponent<PopupTruongThanhOffer>();
            instance.Init();

            return instance;
        }

        private void Init()
        {
            Dictionary<string, OfferStoreConfig> truongThanhOfferCfgs = CashShopUtils.GetTruongThanhOfferConfigs();
            OfferStoreConfig offerCfg = truongThanhOfferCfgs[mPackageName];
            int c = 0;
            foreach (string rewardKey in offerCfg.Content.Keys)
            {
                listPhanThuong[c].SetItem(rewardKey, offerCfg.Content[rewardKey]);
                listPhanThuong[c].gameObject.SetActive(true);
                c++;
            }

            //show total gems
            listPhanThuong[c].SetItem("Gem", 12500);
            listPhanThuong[c].gameObject.SetActive(true);
            c++;

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
            var local_price = UnityIAPManager.instance.GetLocalPrice(mPackageName);
            if (!string.IsNullOrEmpty(local_price))
            {
                this.lbPrice.text = local_price;
            }
            this.btnPurchase.interactable = !UnityIAPManager.instance.CheckPurchaseIsMissing(mPackageName);
#endif
        }

        void UpdateTimer()
        {
            if (GameClient.instance != null && GameClient.instance.UInfo != null && GameClient.instance.UInfo.Gamer.TruongThanhData != null)
            {
                DateTime expiredTime = GameClient.instance.UInfo.Gamer.TruongThanhData.startDate.AddHours(ConfigManager.TruongThanhCfg.ExpiredHours);
                var ts = expiredTime - GameClient.instance.ServerTime;
                if (expiredTime > GameClient.instance.ServerTime)
                {
                    if (ts.TotalDays < 1)
                    {
                        lbExpireAfter.text = Localization.Get("lbExpireAfter") + " " + string.Format(Localization.Get("TimeCountDownHMS"),
                            Mathf.FloorToInt((float)ts.TotalHours), ts.Minutes, ts.Seconds);
                    }
                    else
                    {
                        lbExpireAfter.text = Localization.Get("lbExpireAfter") + " " + string.Format(Localization.Get("TimeCountDownDHMS"),
                            Mathf.FloorToInt((float)ts.TotalDays), ts.Hours, ts.Minutes, ts.Seconds);
                    }
                    if (btnPurchase.interactable == false) btnPurchase.interactable = true;
                }
                else if (btnPurchase.interactable)
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
        public void OnBtnInfoClick()
        {
            PopupTruongThanh.Create();
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
            Dictionary<string, OfferStoreConfig> offerCfgs = CashShopUtils.GetTruongThanhOfferConfigs();
#if UNITY_STANDALONE || GM_BUILD
            //  string testReceip = "{\"Store\":\"GooglePlay\",\"TransactionID\":\"GPA.3338-2557-9554-32252\",\"Payload\":\"{\\\"json\\\":\\\"{\\\\\\\"orderId\\\\\\\":\\\\\\\"GPA.3338-2557-9554-32252\\\\\\\",\\\\\\\"packageName\\\\\\\":\\\\\\\"com.hikergames.caravanwar\\\\\\\",\\\\\\\"productId\\\\\\\":\\\\\\\"com.hikergames.caravanwar.personal_deal_2\\\\\\\",\\\\\\\"purchaseTime\\\\\\\":1506941883100,\\\\\\\"purchaseState\\\\\\\":0,\\\\\\\"purchaseToken\\\\\\\":\\\\\\\"dodinjlppgaikbpakneimfij.AO-J1Ozuie_qSeca-Tml9P51Gl2VSX1w_uygJ268khknuD8MDMR72ou81jBPA6bjIK_SIGjhl1w-ve8M5zKxmSAoLhSUEwqhWBemjWtFwufwhd-NsH6lyCsLicwarUS-GgGOUbe6d471ZHZnkRBvY7bkwtZwOpWcs9cuBEkIhhA5fSFrOGZxL5A\\\\\\\"}\\\",\\\"signature\\\":\\\"UMS1HqoIGhGV7e5fi8gzkruxZgaobuhQSca7AROB8QjP7u1r3qthJC+Qry1uqtTmuKj0dp220D9e7VJVQ5MzsdmIOj9TnMOY61MrkT6jF5j0vg5w93a0ddk\\\\/xs\\\\/oKhLnM5W5juus6eP2kvFwXtQ5SuNDYcP7kbHr8gJ6AAeJLYTeIWCZC6xHYfCb7bC4wy6rCz2tDzk8P1TWIckUP6er4QfknPDhewfjSGiw\\\\/jlKYsFfoM6X669JR6FgutWT2fSslNVRF+JQTfKOPuUrHvBvndJnUUbExiVC57lRoOfDYDfzK\\\\/WxtpzkTddrnd8UnOER+mgajHYbcBe8nb3ZvD0oDA==\\\"}\"}";
            //PopupConfirmInputText.Create("Buy Gem Pack", "Password require for sercurity!", (pass) =>
            //{
            GameClient.instance.RequestBuyTruongThanhOffer(mPackageName, string.Empty, "asvrtr4654hfghfgh");
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
    }
}