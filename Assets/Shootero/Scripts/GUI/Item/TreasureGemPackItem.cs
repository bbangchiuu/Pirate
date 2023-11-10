using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Hiker.Networks.Data.Shootero;
using Hiker.GUI.Shootero;
using Hiker.GUI;

public class TreasureGemPackItem : MonoBehaviour
{
    public Image icon;
    public Text lbName;
    public Text lbPrice;
    public Text lbGem;
    public Button btn;

    public string PackageName;
    OfferStoreData mData;

    bool preventDoubleClick;

    public void SetItem(OfferStoreData data)
    {
        //FREE GEM PACK
        if (PackageName == "FreeGem")
        {
            AdsData adsData = GameClient.instance.UInfo.adsData;
            lbGem.text = string.Format(Localization.Get("GemValue"), ConfigManager.GetFreeGemNumber());
            //int remainTime = ConfigManager.GetFreeGemAdsMaxTime() - adsData.listAdsGemIDs.Count;
            //string str = (remainTime > 0) ? remainTime + "" : "<color=red>" + remainTime + "</color>";
            //str = str + "/" + ConfigManager.GetFreeGemAdsMaxTime();
            //lbPrice.text = Localization.Get("Free") + "(" + str + ")";
            string str = Localization.Get("Free") + " (" + adsData.listAdsGemIDs.Count + "/" + ConfigManager.GetFreeGemAdsMaxTime() + ")";
            if (adsData.listAdsGemIDs.Count >= ConfigManager.GetFreeGemAdsMaxTime())
            {
                str = "<color=#FF5D00>" + str + "</color>";
            }
            else
            {
                str = "<color=#00FF30>" + str + "</color>";
            }
            lbPrice.text = str;
        }
        else
        {
            mData = data;
            PackageName = data.PackageName;
            lbName.text = data.GetConfig().GetName(Localization.language, PackageName);
        
            //var cost = data.GetCost();
            //if (cost > 0)
            //{
            //    lbGem.gameObject.SetActive(true);
            //    lbGem.text = data.GetCost().ToString();
            //}
            //else
            {
                var offerCfg = data.GetConfig();

                //int price = 0;

                //if (price <= 0)
                //{
                //    string priceStr = offerCfg.Price.Substring(1);
                //    price = (int)Mathf.Round(float.Parse(priceStr, System.Globalization.CultureInfo.InvariantCulture));
                //}
                lbPrice.text = offerCfg.Price;
            }
            lbGem.text = string.Format(Localization.Get("GemValue"), data.GetConfig().Content.GetGem());

#if IAP_BUILD
            var local_price = UnityIAPManager.instance.GetLocalPrice(data.PackageName);
            if (!string.IsNullOrEmpty(local_price))
            {
                this.lbPrice.text = local_price;
            }
            this.btn.interactable = !UnityIAPManager.instance.CheckPurchaseIsMissing(data.PackageName);
#endif
        }
    }

    [GUIDelegate]
    public void OnBtnBuyClick()
    {
        //FREE GEM PACK
        if (PackageName == "FreeGem")
        {
            WatchAdsGem();
        }
        else
        {
#if IAP_BUILD
        if (UnityIAPManager.instance.CheckLoginToPurchase() == false)
        {
            return;
        }
#endif

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

            //PopupMessage.Create(MessagePopupType.TEXT, "Buy Click");
            RequestPurchase();
        }
    }
    
    public void WatchAdsGem()
    {
        if (preventDoubleClick) return;

        AdsData adsData = GameClient.instance.UInfo.adsData;
        if (adsData != null && adsData.listAdsGemIDs.Count >= ConfigManager.GetFreeGemAdsMaxTime())
        {
            System.TimeSpan ts = adsData.nextResetTime - GameClient.instance.ServerTime;
            PopupMessage.Create(MessagePopupType.TEXT, string.Format(Localization.Get("reach_limit_ads"), Mathf.FloorToInt((float)ts.TotalHours), Mathf.FloorToInt((float)ts.Minutes)));
            return;
        }
        AnalyticsManager.LogEvent("WATCH_ADS_GEM");

        if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
        {
            HikerAdsManager.instance.ShowVideoAds(this.RequestReceiveReward, this.RequestCancelVideo,
                                                  this.RequestFailVideo,
                                                  this.RequestCancelVideo);
            StartCoroutine(PreventDoubleclick(5));
        }
        else
        {
            StartCoroutine(NoAdsCheck(4));
        }
    }

    private void RequestReceiveReward()
    {
        AnalyticsManager.LogEvent("FINISH_ADS_GEM");
        if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
        {
            Debug.Log("Ad Available");
        }
        GameClient.instance.RequestWatchAdsGem();
    }

    private void RequestCancelVideo()
    {
        if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
        {
            Debug.Log("Ad Available");
        }
        PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("watch_ads_cancel"));
    }

    private void RequestFailVideo()
    {
        if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
        {
            Debug.Log("Ad Available");
        }
        PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("watch_ads_fail"));
    }

    private IEnumerator PreventDoubleclick(float time)
    {
        preventDoubleClick = true;
        yield return new WaitForSecondsRealtime(time);
        preventDoubleClick = false;
    }

    private IEnumerator NoAdsCheck(float time)
    {
        preventDoubleClick = true;

        if (Hiker.GUI.PopupNetworkLoading.instance != null &&
                Hiker.GUI.PopupNetworkLoading.instance.gameObject.activeSelf)
        {
            //Debug.Log("requesting...");
        }
        else
        {
            Hiker.GUI.PopupNetworkLoading.Create(Localization.Get("NetworkLoading"));
        }

        yield return new WaitForSecondsRealtime(time);

        Hiker.GUI.PopupNetworkLoading.Dismiss();

        if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
        {
            HikerAdsManager.instance.ShowVideoAds(this.RequestReceiveReward, this.RequestCancelVideo,
                                                  this.RequestFailVideo, this.RequestCancelVideo);
            StartCoroutine(PreventDoubleclick(5));
        }
        else
        {
            AnalyticsManager.LogEvent("NO_ADS_AVAILABLE");
            PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("no_ads_available"));
            preventDoubleClick = false;
        }
    }

    private void RequestPurchase()
    {

#if UNITY_STANDALONE || GM_BUILD
        //  string testReceip = "{\"Store\":\"GooglePlay\",\"TransactionID\":\"GPA.3338-2557-9554-32252\",\"Payload\":\"{\\\"json\\\":\\\"{\\\\\\\"orderId\\\\\\\":\\\\\\\"GPA.3338-2557-9554-32252\\\\\\\",\\\\\\\"packageName\\\\\\\":\\\\\\\"com.hikergames.caravanwar\\\\\\\",\\\\\\\"productId\\\\\\\":\\\\\\\"com.hikergames.caravanwar.personal_deal_2\\\\\\\",\\\\\\\"purchaseTime\\\\\\\":1506941883100,\\\\\\\"purchaseState\\\\\\\":0,\\\\\\\"purchaseToken\\\\\\\":\\\\\\\"dodinjlppgaikbpakneimfij.AO-J1Ozuie_qSeca-Tml9P51Gl2VSX1w_uygJ268khknuD8MDMR72ou81jBPA6bjIK_SIGjhl1w-ve8M5zKxmSAoLhSUEwqhWBemjWtFwufwhd-NsH6lyCsLicwarUS-GgGOUbe6d471ZHZnkRBvY7bkwtZwOpWcs9cuBEkIhhA5fSFrOGZxL5A\\\\\\\"}\\\",\\\"signature\\\":\\\"UMS1HqoIGhGV7e5fi8gzkruxZgaobuhQSca7AROB8QjP7u1r3qthJC+Qry1uqtTmuKj0dp220D9e7VJVQ5MzsdmIOj9TnMOY61MrkT6jF5j0vg5w93a0ddk\\\\/xs\\\\/oKhLnM5W5juus6eP2kvFwXtQ5SuNDYcP7kbHr8gJ6AAeJLYTeIWCZC6xHYfCb7bC4wy6rCz2tDzk8P1TWIckUP6er4QfknPDhewfjSGiw\\\\/jlKYsFfoM6X669JR6FgutWT2fSslNVRF+JQTfKOPuUrHvBvndJnUUbExiVC57lRoOfDYDfzK\\\\/WxtpzkTddrnd8UnOER+mgajHYbcBe8nb3ZvD0oDA==\\\"}\"}";
        //PopupConfirmInputText.Create("Buy Gem Pack", "Password require for sercurity!", (pass) =>
        //{
        GameClient.instance.RequestBuyGemPack(PackageName, "asvrtr4654hfghfgh");
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
}
