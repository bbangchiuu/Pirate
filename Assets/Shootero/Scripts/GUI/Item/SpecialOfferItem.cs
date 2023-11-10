using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using System;

    public class SpecialOfferItem : MonoBehaviour
    {
        OfferStoreData offerData;
        public Text lbName;
        public Text lbPrice;
        public Transform gemGrp;
        public Text lbGem;
        public Text lbFree;
        public Text lbPurchased;
        public Button btn;
        public Text promote;
        public Image bg;
        public PhanThuongItem[] items;
        public Text lbRemainTime;
        public bool promoteOriginPrice = false;

        public string PackageName;
        OfferStoreData mData;

        private void Update()
        {
            UpdateRemainTime();
        }

        void UpdateRemainTime()
        {
            if (lbRemainTime && mData != null)
            {
                var ts = mData.ExpireTime - GameClient.instance.ServerTime;
                var days = ts.Days;
                if (days > 100)
                {
                    lbRemainTime.text = string.Empty;
                }
                else
                {
                    string timeCountDown = string.Empty;
                    if (days >= 1)
                    {
                        timeCountDown = string.Format(Localization.Get("TimeCountDownDHMS"), days, ts.Hours, ts.Minutes, ts.Seconds);
                    }
                    else
                    {
                        timeCountDown = string.Format(Localization.Get("TimeCountDownHMS"), ts.Hours, ts.Minutes, ts.Seconds);
                    }

                    lbRemainTime.text = string.Format("{0} {1}",
                        Localization.Get("LeoThapTimeLeftLabel"),
                        timeCountDown);
                }
            }
        }

        public void SetItem(OfferStoreData data)
        {
            mData = data;
            PackageName = data.PackageName;
            lbName.text = data.GetConfig().GetName(Localization.language, PackageName);
            var cost = data.GetCost();
            var offerCfg = data.GetConfig();
            if (data.BuyCount >= data.StockCount)
            {
                if (btn) btn.interactable = false;
                if (lbPurchased) lbPurchased.gameObject.SetActive(true);
                if (gemGrp) gemGrp.gameObject.SetActive(false);
                lbPrice.gameObject.SetActive(false);
                if (lbFree) lbFree.gameObject.SetActive(false);
                if (lbGem) lbGem.gameObject.SetActive(false);
            }
            else
            {
                if (btn) btn.interactable = true;
                if (lbPurchased) lbPurchased.gameObject.SetActive(false);
                if (cost > 0)
                {
                    if (gemGrp) gemGrp.gameObject.SetActive(true);
                    lbPrice.gameObject.SetActive(false);
                    if (lbFree)
                        lbFree.gameObject.SetActive(false);
                    if (lbGem)
                        lbGem.text = data.GetCost().ToString();
                }
                else if (cost == 0)
                {
                    if (gemGrp)
                        gemGrp.gameObject.SetActive(false);
                    lbPrice.gameObject.SetActive(true);
                    if (lbFree)
                        lbFree.gameObject.SetActive(false);

                    //int price = 0;

                    //if (price <= 0)
                    //{
                    //    string priceStr = offerCfg.Price.Substring(1);
                    //    price = (int)Mathf.Round(float.Parse(priceStr));
                    //}
                    lbPrice.text = offerCfg.Price;
                }
                else
                {
                    if (gemGrp)
                        gemGrp.gameObject.SetActive(false);
                    lbPrice.gameObject.SetActive(false);
                    if (lbFree)
                        lbFree.gameObject.SetActive(true);
                }
            }

            decimal local_price_value = 0;

#if IAP_BUILD
            var local_price = UnityIAPManager.instance.GetLocalPrice(data.PackageName);
            if (!string.IsNullOrEmpty(local_price))
            {
                this.lbPrice.text = local_price;
            }

            if (this.btn.interactable)
            {
                this.btn.interactable = !UnityIAPManager.instance.CheckPurchaseIsMissing(data.PackageName);
            }
            local_price_value = UnityIAPManager.instance.GetLocalPriceValue(data.PackageName);
#endif
            if (promote != null)
            {
                if (promoteOriginPrice && lbPrice.gameObject.activeSelf)
                {
                    if (offerCfg.PromoSale > 0)
                    {
                        if (local_price_value > 0)
                        {
                            var sale = Mathf.Min(offerCfg.PromoSale, 99);
                            var originPrice = local_price_value * 100 / (100 - sale);

                            promote.text = originPrice.ToString("G");
                            promote.transform.parent.gameObject.SetActive(true);
                        }
                        else
                        {
                            if (decimal.TryParse(offerCfg.Price.Substring(1), out decimal configValue))
                            {
                                local_price_value = configValue;
                                var sale = Mathf.Min(offerCfg.PromoSale, 99);
                                var originPrice = local_price_value * 100 / (100 - sale);

                                promote.text = originPrice.ToString("C2", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                                promote.transform.parent.gameObject.SetActive(true);
                            }
                            else
                            {
                                promote.transform.parent.gameObject.SetActive(false);
                            }
                        }
                    }
                    else
                    {
                        promote.transform.parent.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (offerCfg.PromoSale > 0)
                    {
                        promote.text = string.Format("-{0}%", offerCfg.PromoSale);
                        promote.transform.parent.gameObject.SetActive(true);
                    }
                    else if (data.BonusRate != 0)
                    {
                        promote.text = string.Format("+{0}%", data.BonusRate);
                        promote.transform.parent.gameObject.SetActive(true);
                    }
                    else
                    {
                        promote.transform.parent.gameObject.SetActive(false);
                    }
                }
            }

            if (bg != null)
            {
                Color color = Color.white;
                if (offerCfg.PromoRarity == ERarity.Epic)
                {
                    ColorUtility.TryParseHtmlString("#C59ED3", out color);
                }
                else if (offerCfg.PromoRarity == ERarity.Legend)
                {
                    ColorUtility.TryParseHtmlString("#EFE6B1", out color);
                }
                else if (offerCfg.PromoRarity == ERarity.Rare)
                {
                    ColorUtility.TryParseHtmlString("#7ABBD6", out color);
                }
                bg.color = color;
            }

            var content = data.GetConfig().Content;
            var contentEnum = content.GetEnumerator();

            for (int i = 0; i < items.Length; ++i)
            {
                if (i < content.Count)
                {
                    contentEnum.MoveNext();
                    var curReward = contentEnum.Current;
                    int quantity = curReward.Value;
                    if (quantity < 0)
                    {
                        if(curReward.Key == CardReward.GOLD_CARD)
                            quantity = -quantity * ConfigManager.GetBasedGoldOffer(GameClient.instance.UInfo.GetCurrentChapter());
                        else if (curReward.Key.StartsWith("M_"))
                            quantity = -quantity * ConfigManager.GetBasedMaterialOffer(GameClient.instance.UInfo.GetCurrentChapter());
                    }
                    if (data.BonusRate != 0) quantity += Mathf.RoundToInt(quantity * data.BonusRate / 100f);
                    items[i].gameObject.SetActive(true);
                    items[i].SetItem(curReward.Key, quantity);
                }
                else
                {
                    items[i].gameObject.SetActive(false);
                }
            }

            UpdateRemainTime();
        }

        [GUIDelegate]
        public void OnBtnClick()
        {
            if (mData != null && mData.BuyCount >= mData.StockCount)
            {
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("Mes_OutOfStock"));
                return;
            }

            if (mData != null && mData.ExpireTime <= GameClient.instance.ServerTime)
            {
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("Mes_OfferTimeOut"));
                return;
            }

            // Request Buy Offer;
            //PopupMessage.Create(MessagePopupType.TEXT, "Buy Click");

            ///duongrs sua tam the nay
            /// dung' ra la` 1 offer item can` 2 button (button purchase va` buy offer bang gem hoac free rieng)
            if (mData != null && mData.GetCost() == 0)
            {
                RequestPurchase();
            }
            else
            {
                RequestBuyOfferByGem();
            }
        }

        private void RequestBuyOfferByGem()
        {
            if (mData != null && mData.GetCost() < 0)
            {
                if (preventDoubleClick) return;
                AnalyticsManager.LogEvent("WATCH_ADS_DAILYFREE");

                if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
                {
                    HikerAdsManager.instance.ShowVideoAds(this.RequestReceiveReward, this.RequestCancelVideo,
                                                          this.RequestFailVideo, this.OnCheatVideo);
                    StartCoroutine(PreventDoubleclick(5));

                }
                else
                {
                    AnalyticsManager.LogEvent("NO_ADS_AVAILABLE");
                    PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("no_ads_available"));
                }
            }
            else
            {
                GameClient.instance.RequestBuyOfferBuyGem(PackageName);
            }
        }

        private void RequestPurchase()
        {
#if IAP_BUILD
            if (UnityIAPManager.instance.CheckLoginToPurchase() == false)
            {
                return;
            }
#endif

#if UNITY_STANDALONE || GM_BUILD
            //  string testReceip = "{\"Store\":\"GooglePlay\",\"TransactionID\":\"GPA.3338-2557-9554-32252\",\"Payload\":\"{\\\"json\\\":\\\"{\\\\\\\"orderId\\\\\\\":\\\\\\\"GPA.3338-2557-9554-32252\\\\\\\",\\\\\\\"packageName\\\\\\\":\\\\\\\"com.hikergames.caravanwar\\\\\\\",\\\\\\\"productId\\\\\\\":\\\\\\\"com.hikergames.caravanwar.personal_deal_2\\\\\\\",\\\\\\\"purchaseTime\\\\\\\":1506941883100,\\\\\\\"purchaseState\\\\\\\":0,\\\\\\\"purchaseToken\\\\\\\":\\\\\\\"dodinjlppgaikbpakneimfij.AO-J1Ozuie_qSeca-Tml9P51Gl2VSX1w_uygJ268khknuD8MDMR72ou81jBPA6bjIK_SIGjhl1w-ve8M5zKxmSAoLhSUEwqhWBemjWtFwufwhd-NsH6lyCsLicwarUS-GgGOUbe6d471ZHZnkRBvY7bkwtZwOpWcs9cuBEkIhhA5fSFrOGZxL5A\\\\\\\"}\\\",\\\"signature\\\":\\\"UMS1HqoIGhGV7e5fi8gzkruxZgaobuhQSca7AROB8QjP7u1r3qthJC+Qry1uqtTmuKj0dp220D9e7VJVQ5MzsdmIOj9TnMOY61MrkT6jF5j0vg5w93a0ddk\\\\/xs\\\\/oKhLnM5W5juus6eP2kvFwXtQ5SuNDYcP7kbHr8gJ6AAeJLYTeIWCZC6xHYfCb7bC4wy6rCz2tDzk8P1TWIckUP6er4QfknPDhewfjSGiw\\\\/jlKYsFfoM6X669JR6FgutWT2fSslNVRF+JQTfKOPuUrHvBvndJnUUbExiVC57lRoOfDYDfzK\\\\/WxtpzkTddrnd8UnOER+mgajHYbcBe8nb3ZvD0oDA==\\\"}\"}";
            //PopupConfirmInputText.Create("Buy Gem Pack", "Password require for sercurity!", (pass) =>
            //{
                GameClient.instance.RequestBuyOfferStore(PackageName, string.Empty, "asvrtr4654hfghfgh");
            //});
            return;
#endif

#if IAP_BUILD
            
            if (UnityIAPManager.instance != null)
                UnityIAPManager.instance.BuyStorePack(PackageName);

            AnalyticsManager.LogEvent("IAP_CLICK",
                new AnalyticsParameter("Name", this.lbName.text),
                new AnalyticsParameter("Value", this.lbPrice.text));
#endif
        }

        bool preventDoubleClick;

        private void RequestReceiveReward()
        {
            AnalyticsManager.LogEvent("FINISH_ADS_DAILYFREE");
            GameClient.instance.RequestBuyOfferBuyGem(PackageName);
        }

        private void RequestCancelVideo()
        {
            PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("watch_ads_cancel"));
        }

        private void RequestFailVideo()
        {
            PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("watch_ads_fail"));
        }

        private void OnCheatVideo()
        {
            PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("watch_ads_cancel"));
        }

        private IEnumerator PreventDoubleclick(float time)
        {
            preventDoubleClick = true;
            yield return new WaitForSecondsRealtime(time);
            preventDoubleClick = false;
        }
    }
}

