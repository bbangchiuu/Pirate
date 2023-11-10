using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Hiker.Networks.Data.Shootero;
using Hiker.GUI.Shootero;
using Hiker.GUI;

public class PremiumCard : MonoBehaviour
{
    public Text lbGemPerDay;
    public Text lbHeartPerDay;
    public Text lbBonusResource;
    public Text[] lbDays;
    public Text[] lbPrices;
    public Text remainDay;
    public Button[] btnSubs;

    List<string> packageNames;
    Dictionary<string, OfferStoreConfig> listPremiumPackConfigs;

    public void SetItems()
    {
        UserInfo uInfo = GameClient.instance.UInfo;
        if (uInfo.Gamer.PremiumEndTime > GameClient.instance.ServerTime)
        {
            int remainDays = (uInfo.Gamer.PremiumEndTime.Date - GameClient.instance.ServerTime.Date).Days;
            remainDay.text = string.Format(Localization.Get("premium_remain_days"), remainDays);
            remainDay.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            remainDay.transform.parent.gameObject.SetActive(false);
        }
        packageNames = new List<string>();
        listPremiumPackConfigs = CashShopUtils.GetPremiumPackConfigs();
        if (listPremiumPackConfigs == null) return;
        lbGemPerDay.text = string.Format(Localization.Get("premium_benefit_gem"), ConfigManager.GetPremiumDailyGem());
        lbHeartPerDay.text = string.Format(Localization.Get("premium_benefit_heart"), ConfigManager.GetPremiumDailyTheLuc());
        lbBonusResource.text = string.Format(Localization.Get("premium_benefit_bonus"), ConfigManager.GetPremiumBonusResource());
        int c = 0;
        foreach(string packName in listPremiumPackConfigs.Keys)
        {
            packageNames.Add(packName);
            lbDays[c].text = string.Format(Localization.Get("premium_sub_btn_days"), ConfigManager.GetPremiumDays(packName));
            lbPrices[c].text = listPremiumPackConfigs[packName].Price;
#if IAP_BUILD
        var local_price = UnityIAPManager.instance.GetLocalPrice(packName);
        if (!string.IsNullOrEmpty(local_price))
        {
                lbPrices[c].text = local_price;
        }
        btnSubs[c].interactable = !UnityIAPManager.instance.CheckPurchaseIsMissing(packName);
#endif

            c++;
        }
    }

    [GUIDelegate]
    public void OnBtnSubClick(int packIdx)
    {
#if IAP_BUILD
        if (UnityIAPManager.instance.CheckLoginToPurchase() == false)
        {
            return;
        }
#endif
        RequestPurchase(packageNames[packIdx]);
    }

    private void RequestPurchase(string PackageName)
    {
#if UNITY_STANDALONE || GM_BUILD
        //  string testReceip = "{\"Store\":\"GooglePlay\",\"TransactionID\":\"GPA.3338-2557-9554-32252\",\"Payload\":\"{\\\"json\\\":\\\"{\\\\\\\"orderId\\\\\\\":\\\\\\\"GPA.3338-2557-9554-32252\\\\\\\",\\\\\\\"packageName\\\\\\\":\\\\\\\"com.hikergames.caravanwar\\\\\\\",\\\\\\\"productId\\\\\\\":\\\\\\\"com.hikergames.caravanwar.personal_deal_2\\\\\\\",\\\\\\\"purchaseTime\\\\\\\":1506941883100,\\\\\\\"purchaseState\\\\\\\":0,\\\\\\\"purchaseToken\\\\\\\":\\\\\\\"dodinjlppgaikbpakneimfij.AO-J1Ozuie_qSeca-Tml9P51Gl2VSX1w_uygJ268khknuD8MDMR72ou81jBPA6bjIK_SIGjhl1w-ve8M5zKxmSAoLhSUEwqhWBemjWtFwufwhd-NsH6lyCsLicwarUS-GgGOUbe6d471ZHZnkRBvY7bkwtZwOpWcs9cuBEkIhhA5fSFrOGZxL5A\\\\\\\"}\\\",\\\"signature\\\":\\\"UMS1HqoIGhGV7e5fi8gzkruxZgaobuhQSca7AROB8QjP7u1r3qthJC+Qry1uqtTmuKj0dp220D9e7VJVQ5MzsdmIOj9TnMOY61MrkT6jF5j0vg5w93a0ddk\\\\/xs\\\\/oKhLnM5W5juus6eP2kvFwXtQ5SuNDYcP7kbHr8gJ6AAeJLYTeIWCZC6xHYfCb7bC4wy6rCz2tDzk8P1TWIckUP6er4QfknPDhewfjSGiw\\\\/jlKYsFfoM6X669JR6FgutWT2fSslNVRF+JQTfKOPuUrHvBvndJnUUbExiVC57lRoOfDYDfzK\\\\/WxtpzkTddrnd8UnOER+mgajHYbcBe8nb3ZvD0oDA==\\\"}\"}";
        //PopupConfirmInputText.Create("Buy Gem Pack", "Password require for sercurity!", (pass) =>
        //{
        GameClient.instance.RequestBuyPremiumPack(PackageName, string.Empty, "asvrtr4654hfghfgh");
        //});
        return;
#endif

#if IAP_BUILD
        if (UnityIAPManager.instance != null)
            UnityIAPManager.instance.BuyStorePack(PackageName);

        AnalyticsManager.LogEvent("IAP_CLICK",
            new AnalyticsParameter("Name", PackageName),
            new AnalyticsParameter("Value", listPremiumPackConfigs[PackageName].Price));
#endif
    }
}
