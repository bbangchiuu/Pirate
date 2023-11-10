using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Hiker.Networks.Data.Shootero;
using Hiker.GUI.Shootero;
using Hiker.GUI;

public class TreasureOfferItem : MonoBehaviour
{
    //public Image icon;
    public Text lbName;
    public Text lbPrice;
    public Transform gemGrp;
    public Text lbGem;
    public Text lbFree;
    public Button btn;

    public PhanThuongItem[] items;

    public string PackageName;
    OfferStoreData mData;

    public void SetItem(OfferStoreData data)
    {
        mData = data;
        PackageName = data.PackageName;
        lbName.text = data.GetConfig().GetName(Localization.language, PackageName);
        var cost = data.GetCost();
        if (cost > 0)
        {
            gemGrp.gameObject.SetActive(true);
            lbPrice.gameObject.SetActive(false);
            lbFree.gameObject.SetActive(false);
            lbGem.text = data.GetCost().ToString();
        }
        else if (cost == 0)
        {
            gemGrp.gameObject.SetActive(false);
            lbPrice.gameObject.SetActive(true);
            lbFree.gameObject.SetActive(false);
            var offerCfg = data.GetConfig();

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
            gemGrp.gameObject.SetActive(false);
            lbPrice.gameObject.SetActive(false);
            lbFree.gameObject.SetActive(true);
        }

#if IAP_BUILD
        var local_price = UnityIAPManager.instance.GetLocalPrice(data.PackageName);
        if (!string.IsNullOrEmpty(local_price))
        {
            this.lbPrice.text = local_price;
        }
        this.btn.interactable = !UnityIAPManager.instance.CheckPurchaseIsMissing(data.PackageName);
#endif

        var content = data.GetConfig().Content;
        var contentEnum = content.GetEnumerator();
        for (int i = 0; i < items.Length; ++i)
        {
            if (i < content.Count)
            {
                contentEnum.MoveNext();
                var curReward = contentEnum.Current;
                items[i].gameObject.SetActive(true);
                items[i].SetItem(curReward.Key, curReward.Value);
            }
            else
            {
                items[i].gameObject.SetActive(false);
            }
        }
    }

    [GUIDelegate]
    public void OnBtnClick()
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
        // Request Buy Offer;
        //PopupMessage.Create(MessagePopupType.TEXT, "Buy Click");
        RequestPurchase();
    }

    private void RequestPurchase()
    {
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
}
