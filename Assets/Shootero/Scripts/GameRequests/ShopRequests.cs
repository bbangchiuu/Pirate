using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hiker.Networks.Data;
using Hiker.Networks.Data.Shootero;
using Hiker.GUI.Shootero;
using Hiker.GUI;

public partial class GameClient : HTTPClient
{
    public void OnMissingOfferStorePurchaseSuccessful(string packageName, PurchaseResponse data)
    {
        //string offerName = CashShopUtils.GetSpecialOfferName(packType, index);
        //offerName = Localization.Get(offerName);

        var offer = UInfo.ListOffers.Find(e => e.PackageName == packageName);
        var config = offer.GetConfig();

        PopupConfirm.Create(
            string.Format(Localization.Get("MissingOrderOfferPurchasedMsg"), config.GetName(Localization.language, packageName)),
            () =>
            {
                OnPurchaseSuccess(data);
            }, false);
    }
    public void OnMissingGemPackPurchaseSuccessful(string packageName, PurchaseResponse response)
    {
        //int gem = CashShopUtils.GetGemDealValue(index);
        var offer = UInfo.ListOffers.Find(e => e.PackageName == packageName);

        var config = offer.GetConfig();

        PopupConfirm.Create(
            string.Format(Localization.Get("missing_oder_gem_pack_purchased_msg"), config.Content.GetGem()),
            () =>
            {
                OnPurchaseSuccess(response);
            },
            false);
    }

    public bool needCheckMailFromPurchase = false;
    public void OnPurchaseSuccess(PurchaseResponse response)
    {
//#if IAP_BUILD
//        if (UnityIAPManager.instance.processingProduct != null)
//        {
//            UnityIAPManager.instance.RemoveProductFromCache(UnityIAPManager.instance.processingProduct);
//            UnityIAPManager.instance.processingProduct = null;
//        }
//#endif
        if (response != null)
        {
            if (response.UInfo != null)
            {
                var game_data = response.UInfo;
                var add_gem = game_data.Gamer.Gem - this.UInfo.Gamer.Gem;
                needCheckMailFromPurchase = (needCheckMailFromPurchase || response.checkMail);

                if (game_data != null)
                {
                    this.UpdateUserInfo(game_data);
                }
                // PopupConfirm.Create(Localization.Get("thank_purchase"), null);

                if (this.UInfo.Gamer.PurchaseCount == 1)
                {
                    //AnalyticsManager.LogEvent("FIRST_PURCHASE",
                    //    new AnalyticsParameter("AddGem", add_gem),
                    //    new AnalyticsParameter("CurrentGem", this.gameData.user.gem));
                }

                if (add_gem > 0)
                {
                    if (this.UInfo.Gamer.PurchaseCount > 1)
                    {
                        //AnalyticsManager.LogEvent("PURCHASE",
                        //    new AnalyticsParameter("AddGem", add_gem),
                        //    new AnalyticsParameter("CurrentGem", this.gameData.user.gem));
                    }

                    if (add_gem > 6500)
                    {
                        //AnalyticsManager.LogEvent("WHALE_PURCHASE");
                    }

                    //AnalyticsManager.LogEvent("GEM_INCOME",
                    //    new AnalyticsParameter("From", "PURCHASE"),
                    //    new AnalyticsParameter("Value", add_gem));
                }

                //if (PopupShop.instance != null)
                //{
                //    PopupShop.instance.UpdateShopInfo();
                //}

                //if (PopupSpecialOffers.instance != null)
                //{
                //    PopupSpecialOffers.instance.UpdateShopInfo(false);
                //}

                //if (ScreenShop.instance && GUIManager.Instance.CurrentScreen == "Shop")
                //{
                //    ScreenShop.instance.UpdateShopGemInfo();
                //    ScreenShop.instance.UpdateShopInfo(false);
                //}
                if (PopupSpecialOffers.instance)
                {
                    PopupSpecialOffers.instance.Init();
                }
                if (PopupOfferDacBiet.instance)
                {
                    PopupOfferDacBiet.instance.Init();
                }
            }
        }

    }
    public bool OnPurchaseFail(PurchaseResponse response)
    {
        //ERROR_CODE code = response.ErrorCode;
//#if IAP_BUILD
//        if (code == ERROR_CODE.IAP_PACKAGE_NOT_TRUE ||
//            code == ERROR_CODE.IAP_TRANSACTION_EXISTED ||
//            code == ERROR_CODE.SPECIAL_OFFER_PURCHASED)
//        {
//            //Đã gửi recept lên server thành công nhưng server check ko hợp lệ
//            if (UnityIAPManager.instance.processingProduct != null)
//            {
//                UnityIAPManager.instance.RemoveProductFromCache(UnityIAPManager.instance.processingProduct);
//                UnityIAPManager.instance.processingProduct = null;
//            }
//            return true;
//        }
//        else
//#endif
        {
            ProcessErrorResponse(response);
        }

        return false;
    }

    public void RequestBuyGemPack(string packageName, string transID = "", string recept = "")
    {
        PurchaseRequestBase request = new PurchaseRequestBase();
        request.UpdateGIDData();
        //request.type = packType;
        //request.index = index;
        request.transID = transID;
        request.receipt = recept;
        request.packageName = packageName;
#if UNITY_EDITOR || UNITY_STANDALONE
        // cheat as windows platform if run game from Unity Editor
        request.platform = "windows";
        request.receipt = "asvrtr4654hfghfgh";
#endif

        this.SendRequest("RequestBuyGemPack", LitJson.JsonMapper.ToJson(request), (data) =>
        {
            var response = LitJson.JsonMapper.ToObject<PurchaseResponse>(data);
            if (response.ErrorCode == ERROR_CODE.OK)
            {
#if IAP_BUILD
                if (UnityIAPManager.instance != null && string.IsNullOrEmpty(response.transID) == false)
                {
                    UnityIAPManager.instance.ConfirmPendingPurchase(response.transID, response.isRestorePurchase == false);
                }
#endif
                //if (missedOrder) OnMissingGemPackPurchaseSuccessful(packageName, response);
                //else OnPurchaseSuccess(response);
                OnPurchaseSuccess(response);
            }
            else if (response.ErrorCode == ERROR_CODE.IAP_TRANSACTION_EXISTED)
            {
#if IAP_BUILD
                if (UnityIAPManager.instance != null && string.IsNullOrEmpty(response.transID) == false)
                {
                    UnityIAPManager.instance.ConfirmPendingPurchase(response.transID, response.isRestorePurchase == false);
                }
                if (ScreenMain.instance && ScreenMain.instance.gameObject.activeInHierarchy)
                {
                    ScreenMain.instance.grpTreasury.SyncNetworkData();
                }
#endif
            }
            else if (response.ErrorCode == ERROR_CODE.IAP_PACKAGE_NOT_TRUE)
            {
                Debug.Log("IAP_PACKAGE_NOT_TRUE");
#if IAP_BUILD
                if (UnityIAPManager.instance != null && string.IsNullOrEmpty(response.transID) == false)
                {
                    UnityIAPManager.instance.RemoveProcessingProduct(response.transID);
                }
                if (ScreenMain.instance && ScreenMain.instance.gameObject.activeInHierarchy)
                {
                    ScreenMain.instance.grpTreasury.SyncNetworkData();
                }
#endif
            }
            else
            {
                OnPurchaseFail(response);
            }
        });

        //AnalyticsManager.LogEvent("BUY_GEM_PACK",
        //        new AnalyticsParameter("Pack", packageName));
    }

    public void RequestBuyPremiumPack(string packageName, string transID = "", string recept = "")
    {
        PurchaseRequestBase request = new PurchaseRequestBase();
        request.UpdateGIDData();
        //request.type = packType;
        //request.index = index;
        request.transID = transID;
        request.receipt = recept;
        request.packageName = packageName;
#if UNITY_EDITOR || UNITY_STANDALONE
        // cheat as windows platform if run game from Unity Editor
        request.platform = "windows";
        request.receipt = "asvrtr4654hfghfgh";
#endif

        this.SendRequest("RequestBuyPremiumPack", LitJson.JsonMapper.ToJson(request), (data) =>
        {
            var response = LitJson.JsonMapper.ToObject<PurchaseResponse>(data);
            if (response.ErrorCode == ERROR_CODE.OK)
            {
#if IAP_BUILD
                if (UnityIAPManager.instance != null && string.IsNullOrEmpty(response.transID) == false)
                {
                    UnityIAPManager.instance.ConfirmPendingPurchase(response.transID, response.isRestorePurchase == false);
                }
#endif
                //if (missedOrder) OnMissingGemPackPurchaseSuccessful(packageName, response);
                //else OnPurchaseSuccess(response);
                OnPurchaseSuccess(response);
            }
            else if (response.ErrorCode == ERROR_CODE.IAP_TRANSACTION_EXISTED)
            {
#if IAP_BUILD
                if (UnityIAPManager.instance != null && string.IsNullOrEmpty(response.transID) == false)
                {
                    UnityIAPManager.instance.ConfirmPendingPurchase(response.transID, response.isRestorePurchase == false);
                }
                if (ScreenMain.instance && ScreenMain.instance.gameObject.activeInHierarchy)
                {
                    ScreenMain.instance.grpTreasury.SyncNetworkData();
                }
#endif
            }
            else if (response.ErrorCode == ERROR_CODE.IAP_PACKAGE_NOT_TRUE)
            {
                Debug.Log("IAP_PACKAGE_NOT_TRUE");
#if IAP_BUILD
                if (UnityIAPManager.instance != null && string.IsNullOrEmpty(response.transID) == false)
                {
                    UnityIAPManager.instance.RemoveProcessingProduct(response.transID);
                }
                if (ScreenMain.instance && ScreenMain.instance.gameObject.activeInHierarchy)
                {
                    ScreenMain.instance.grpTreasury.SyncNetworkData();
                }
#endif
            }
            else
            {
                OnPurchaseFail(response);
            }
        });

        //AnalyticsManager.LogEvent("BUY_GEM_PACK",
        //        new AnalyticsParameter("Pack", packageName));
    }

    public void RequestBuyTargetOffer(string packageName, string transID = "", string recept = "")
    {
        PurchaseRequestBase request = new PurchaseRequestBase();
        request.UpdateGIDData();
        //request.type = packType;
        //request.index = index;
        request.transID = transID;
        request.receipt = recept;
        request.packageName = packageName;
#if UNITY_EDITOR || UNITY_STANDALONE
        // cheat as windows platform if run game from Unity Editor
        request.platform = "windows";
        request.receipt = "asvrtr4654hfghfgh";
#endif

        this.SendRequest("RequestBuyTargetOffer", LitJson.JsonMapper.ToJson(request), (data) =>
        {
            var response = LitJson.JsonMapper.ToObject<PurchaseResponse>(data);
            if (PopupTargetOffer.instance) PopupTargetOffer.instance.OnCloseBtnClick();
            if (response.ErrorCode == ERROR_CODE.OK)
            {
#if IAP_BUILD
                if (UnityIAPManager.instance != null && string.IsNullOrEmpty(response.transID) == false)
                {
                    UnityIAPManager.instance.ConfirmPendingPurchase(response.transID);
                }
#endif
                //if (missedOrder) OnMissingGemPackPurchaseSuccessful(packageName, response);
                //else OnPurchaseSuccess(response);
                OnPurchaseSuccess(response);
            }
            else if (response.ErrorCode == ERROR_CODE.IAP_TRANSACTION_EXISTED)
            {
#if IAP_BUILD
                if (UnityIAPManager.instance != null && string.IsNullOrEmpty(response.transID) == false)
                {
                    UnityIAPManager.instance.ConfirmPendingPurchase(response.transID);
                }
                if (ScreenMain.instance && ScreenMain.instance.gameObject.activeInHierarchy)
                {
                    ScreenMain.instance.grpTreasury.SyncNetworkData();
                }
#endif
            }
            else if (response.ErrorCode == ERROR_CODE.IAP_PACKAGE_NOT_TRUE)
            {
                Debug.Log("IAP_PACKAGE_NOT_TRUE");
#if IAP_BUILD
                if (UnityIAPManager.instance != null && string.IsNullOrEmpty(response.transID) == false)
                {
                    UnityIAPManager.instance.RemoveProcessingProduct(response.transID);
                }
                if (ScreenMain.instance && ScreenMain.instance.gameObject.activeInHierarchy)
                {
                    ScreenMain.instance.grpTreasury.SyncNetworkData();
                }
#endif
            }
            else
            {
                OnPurchaseFail(response);
            }
        });

        //AnalyticsManager.LogEvent("BUY_GEM_PACK",
        //        new AnalyticsParameter("Pack", packageName));
    }

    public void RequestBuyTruongThanhOffer(string packageName, string transID = "", string recept = "")
    {
        PurchaseRequestBase request = new PurchaseRequestBase();
        request.UpdateGIDData();
        //request.type = packType;
        //request.index = index;
        request.transID = transID;
        request.receipt = recept;
        request.packageName = packageName;
#if UNITY_EDITOR || UNITY_STANDALONE
        // cheat as windows platform if run game from Unity Editor
        request.platform = "windows";
        request.receipt = "asvrtr4654hfghfgh";
#endif

        this.SendRequest("RequestBuyTruongThanhOffer", LitJson.JsonMapper.ToJson(request), (data) =>
        {
            var response = LitJson.JsonMapper.ToObject<PurchaseResponse>(data);
            if (PopupTruongThanhOffer.instance) PopupTruongThanhOffer.instance.OnCloseBtnClick();
            if (response.ErrorCode == ERROR_CODE.OK)
            {
#if IAP_BUILD
                if (UnityIAPManager.instance != null && string.IsNullOrEmpty(response.transID) == false)
                {
                    UnityIAPManager.instance.ConfirmPendingPurchase(response.transID);
                }
#endif
                //if (missedOrder) OnMissingGemPackPurchaseSuccessful(packageName, response);
                //else OnPurchaseSuccess(response);
                OnPurchaseSuccess(response);
            }
            else if (response.ErrorCode == ERROR_CODE.IAP_TRANSACTION_EXISTED)
            {
#if IAP_BUILD
                if (UnityIAPManager.instance != null && string.IsNullOrEmpty(response.transID) == false)
                {
                    UnityIAPManager.instance.ConfirmPendingPurchase(response.transID);
                }
                if (ScreenMain.instance && ScreenMain.instance.gameObject.activeInHierarchy)
                {
                    ScreenMain.instance.grpTreasury.SyncNetworkData();
                }
#endif
            }
            else if (response.ErrorCode == ERROR_CODE.IAP_PACKAGE_NOT_TRUE)
            {
                Debug.Log("IAP_PACKAGE_NOT_TRUE");
#if IAP_BUILD
                if (UnityIAPManager.instance != null && string.IsNullOrEmpty(response.transID) == false)
                {
                    UnityIAPManager.instance.RemoveProcessingProduct(response.transID);
                }
                if (ScreenMain.instance && ScreenMain.instance.gameObject.activeInHierarchy)
                {
                    ScreenMain.instance.grpTreasury.SyncNetworkData();
                }
#endif
            }
            else
            {
                OnPurchaseFail(response);
            }
        });

        //AnalyticsManager.LogEvent("BUY_GEM_PACK",
        //        new AnalyticsParameter("Pack", packageName));
    }

    public void RequestBuyTetEventOffer(string packageName, string transID = "", string recept = "")
    {
        PurchaseRequestBase request = new PurchaseRequestBase();
        request.UpdateGIDData();
        //request.type = packType;
        //request.index = index;
        request.transID = transID;
        request.receipt = recept;
        request.packageName = packageName;
#if UNITY_EDITOR || UNITY_STANDALONE
        // cheat as windows platform if run game from Unity Editor
        request.platform = "windows";
        request.receipt = "asvrtr4654hfghfgh";
#endif

        this.SendRequest("RequestBuyTetEventOffer", LitJson.JsonMapper.ToJson(request), (data) =>
        {
            var response = LitJson.JsonMapper.ToObject<PurchaseResponse>(data);
            if (PopupTetEventBattlePass.instance) PopupTetEventBattlePass.instance.Init();
            if (response.ErrorCode == ERROR_CODE.OK)
            {
#if IAP_BUILD
                if (UnityIAPManager.instance != null && string.IsNullOrEmpty(response.transID) == false)
                {
                    UnityIAPManager.instance.ConfirmPendingPurchase(response.transID);
                }
#endif
                //if (missedOrder) OnMissingGemPackPurchaseSuccessful(packageName, response);
                //else OnPurchaseSuccess(response);
                OnPurchaseSuccess(response);
            }
            else if (response.ErrorCode == ERROR_CODE.IAP_TRANSACTION_EXISTED)
            {
#if IAP_BUILD
                if (UnityIAPManager.instance != null && string.IsNullOrEmpty(response.transID) == false)
                {
                    UnityIAPManager.instance.ConfirmPendingPurchase(response.transID);
                }
                if (ScreenMain.instance && ScreenMain.instance.gameObject.activeInHierarchy)
                {
                    ScreenMain.instance.grpTreasury.SyncNetworkData();
                }
#endif
            }
            else if (response.ErrorCode == ERROR_CODE.IAP_PACKAGE_NOT_TRUE)
            {
                Debug.Log("IAP_PACKAGE_NOT_TRUE");
#if IAP_BUILD
                if (UnityIAPManager.instance != null && string.IsNullOrEmpty(response.transID) == false)
                {
                    UnityIAPManager.instance.RemoveProcessingProduct(response.transID);
                }
                if (ScreenMain.instance && ScreenMain.instance.gameObject.activeInHierarchy)
                {
                    ScreenMain.instance.grpTreasury.SyncNetworkData();
                }
#endif
            }
            else
            {
                OnPurchaseFail(response);
            }
        });

        //AnalyticsManager.LogEvent("BUY_GEM_PACK",
        //        new AnalyticsParameter("Pack", packageName));
    }

    public void RequestBuyOfferStore(string packageName, string transID, string recept, bool missedOrder = false)
    {
        PurchaseRequestBase request = new PurchaseRequestBase();
        request.UpdateGIDData();
        //request.type = packType;
        //request.index = index;
        request.receipt = recept;
        request.packageName = packageName;
        request.transID = transID;

#if UNITY_EDITOR || UNITY_STANDALONE
        // cheat as windows platform if run game from Unity Editor
        request.platform = "windows";
        request.receipt = "asvrtr4654hfghfgh";
#endif

        this.SendRequest("RequestBuyOrderStore", LitJson.JsonMapper.ToJson(request), (data) =>
        {
            var response = LitJson.JsonMapper.ToObject<PurchaseResponse>(data);
            if (response.ErrorCode == ERROR_CODE.OK)
            {
#if IAP_BUILD
                if (UnityIAPManager.instance != null && string.IsNullOrEmpty(response.transID) == false)
                {
                    UnityIAPManager.instance.ConfirmPendingPurchase(response.transID, response.isRestorePurchase == false);
                }
#endif
                //if (missedOrder) OnMissingOfferStorePurchaseSuccessful(packageName, response);
                //else OnPurchaseSuccess(response);
                OnPurchaseSuccess(response);


                if (PopupSelectHero.instance)
                {
                    PopupSelectHero.instance.Init();
                }
            }
            else if (response.ErrorCode == ERROR_CODE.IAP_TRANSACTION_EXISTED)
            {
#if IAP_BUILD
                if (UnityIAPManager.instance != null && string.IsNullOrEmpty(response.transID) == false)
                {
                    UnityIAPManager.instance.ConfirmPendingPurchase(response.transID, response.isRestorePurchase == false);
                }
                if (ScreenMain.instance && ScreenMain.instance.gameObject.activeInHierarchy)
                {
                    ScreenMain.instance.grpTreasury.SyncNetworkData();
                }
                if (PopupSpecialOffers.instance)
                {
                    PopupSpecialOffers.instance.Init();
                }
                if (PopupOfferDacBiet.instance)
                {
                    PopupOfferDacBiet.instance.Init();
                }
#endif

                if (PopupSelectHero.instance)
                {
                    PopupSelectHero.instance.Init();
                }
            }
            else if (response.ErrorCode == ERROR_CODE.IAP_PACKAGE_NOT_TRUE)
            {
                Debug.Log("IAP_PACKAGE_NOT_TRUE");
#if IAP_BUILD
                if (UnityIAPManager.instance != null && string.IsNullOrEmpty(response.transID) == false)
                {
                    UnityIAPManager.instance.RemoveProcessingProduct(response.transID);
                }
                if (ScreenMain.instance && ScreenMain.instance.gameObject.activeInHierarchy)
                {
                    ScreenMain.instance.grpTreasury.SyncNetworkData();
                }
#endif
            }
            else
            {
                OnPurchaseFail(response);
            }
        });

        //var offer = this.UInfo.ListOffers.Find(e => e.PackageName == packageName);
        //if (offer != null)
        //{
        //    //AnalyticsManager.LogEvent("SPECIAL_OFFER",
        //    //    new AnalyticsParameter("Pack", packageName),
        //    //    new AnalyticsParameter("Type", offer.Type));
        //}

    }

    public void RequestBuyOfferBuyGem(string packageName)
    {
        PurchaseRequestBase request = new PurchaseRequestBase();
        request.UpdateGIDData();
        request.packageName = packageName;

        this.SendRequest("RequestBuyOrderByGem", LitJson.JsonMapper.ToJson(request), (data) =>
        {
            var response = LitJson.JsonMapper.ToObject<PurchaseResponse>(data);
            if (response != null && response.ErrorCode == ERROR_CODE.OK)
            {
                UpdateUserInfo(response.UInfo);

                if (PopupSelectHero.instance)
                {
                    PopupSelectHero.instance.Init();
                }
                if (PopupSpecialOffers.instance)
                {
                    PopupSpecialOffers.instance.Init();
                }
                if (PopupTongHopQuangCao.instance) PopupTongHopQuangCao.instance.Init();
            }
            else
            {
                OnPurchaseFail(response);
            }
        });

        //var offer = this.UInfo.ListOffers.Find(e => e.PackageName == packageName);
        //if (offer != null)
        //{
        //    //AnalyticsManager.LogEvent("SPECIAL_OFFER",
        //    //    new AnalyticsParameter("Pack", packageName),
        //    //    new AnalyticsParameter("Type", offer.Type));
        //}
    }
}
