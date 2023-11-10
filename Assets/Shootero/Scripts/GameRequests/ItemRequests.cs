using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hiker.Networks.Data;
using Hiker.Networks.Data.Shootero;
using Hiker.GUI;
using Hiker.GUI.Shootero;

public partial class GameClient : HTTPClient
{
    public void RequestRecycleItems(string[] items)
    {
        RecycleItemRequest req = new RecycleItemRequest();
        req.UpdateGIDData();
        req.ItemIDs = items;
        SendRequest("RequestRecycleItems", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);

                    if (PopupRecycleItem.instance)
                    {
                        PopupRecycleItem.instance.Init();
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestFuseItems(string itemID, string[] materialsID)
    {
        FuseItemRequest req = new FuseItemRequest();
        req.UpdateGIDData();
        req.ItemID = itemID;
        req.MaterialIDs = materialsID;
        SendRequest("RequestFuseItem", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);

                    if (PopupTinhLuyenTrangBi.instance)
                    {
                        PopupTinhLuyenTrangBi.instance.Init();
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestUpgradeItem(string itemID)
    {
        UpgradeItemRequest req = new UpgradeItemRequest();
        req.UpdateGIDData();
        req.ItemID = itemID;
        SendRequest("RequestUpgradeItem", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);

                    if (Hiker.GUI.Shootero.PopupTrangBiInfo.instance)
                    {
                        Hiker.GUI.Shootero.PopupTrangBiInfo.instance.SyncNetworkData();
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestUpgradeArmory(string armory)
    {
        UpgradeArmoryRequest req = new UpgradeArmoryRequest();
        req.UpdateGIDData();
        req.ArmoryName = armory;
        SendRequest("RequestUpgradeArmory", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestSetHeroEquipment(string heroID, EquipmentSlot[] slots)
    {
        SetHeroEquipmentRequest req = new SetHeroEquipmentRequest();
        req.UpdateGIDData();
        req.HeroID = heroID;
        req.slots = slots;
        SendRequest("RequestSetHeroEquipment", LitJson.JsonMapper.ToJson(req),
            (data) =>
            {
                SetHeroEquipmentResponse response = LitJson.JsonMapper.ToObject<SetHeroEquipmentResponse>(data);
                if (ValidateResponse(response))
                {
                    if (response.ErrorCode == ERROR_CODE.OK)
                    {
                        UpdateUserInfo(response.UInfo);
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
            }, false);
    }

    public void RequestSelectHero(HeroData heroData, HeroData curHero)
    {
        SetHeroEquipmentRequest req = new SetHeroEquipmentRequest();
        req.UpdateGIDData();

        req.HeroID = heroData.ID;
        req.slots = curHero.ListSlots;

        SendRequest("RequestSelectHero", LitJson.JsonMapper.ToJson(req),
            (data) =>
            {
                SetHeroEquipmentResponse response = LitJson.JsonMapper.ToObject<SetHeroEquipmentResponse>(data);
                if (ValidateResponse(response))
                {
                    if (response.ErrorCode == ERROR_CODE.OK)
                    {
                        UpdateUserInfo(response.UInfo);

                        if (PopupSelectHero.instance)
                        {
                            PopupSelectHero.instance.Init();
                        }
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
            }, true);
    }

    public void RequestOpenChest(string chestName)
    {
        OpenChestRequest req = new OpenChestRequest();
        req.UpdateGIDData();
        req.ChestName = chestName;
        SendRequest("RequestOpenChest", LitJson.JsonMapper.ToJson(req),
            (data) =>
            {
                UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
                if (ValidateResponse(response))
                {
                    if (response.ErrorCode == ERROR_CODE.OK)
                    {
                        UpdateUserInfo(response.UInfo);
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
            }, true);
    }

    public void RequestForgeMysthicItem(string[] _itemIDs, string _selectedSlot)
    {
        ForgeMysthicItemRequest req = new ForgeMysthicItemRequest();
        req.UpdateGIDData();
        req.ItemIDs = _itemIDs;
        req.selectedSlot = _selectedSlot;
        SendRequest("RequestForgeMysthicItem", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);

                    if (PopupTinhLuyenTrangBi.instance)
                    {
                        PopupTinhLuyenTrangBi.instance.Init();
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestHeroUpgrade(string heroID)
    {
        HeroUpgradeRequest req = new HeroUpgradeRequest();
        req.UpdateGIDData();
        req.HeroID = heroID;
        SendRequest("RequestHeroUpgrade", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            HeroUpgradeResponse response = LitJson.JsonMapper.ToObject<HeroUpgradeResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);

                    
                    HeroData hero = GameClient.instance.UInfo.ListHeroes.Find(e => e.ID == response.HeroID);
                    if (hero != null)
                    {
                        if (Hiker.GUI.Shootero.PopupHeroUpgrade.instance)
                        {
                            Hiker.GUI.Shootero.PopupHeroUpgrade.instance.Init(hero);
                        }
                    }
                    else
                    {
                        if (Hiker.GUI.Shootero.PopupHeroUpgrade.instance)
                        {
                            Hiker.GUI.Shootero.PopupHeroUpgrade.instance.OnCloseBtnClick();
                        }
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestOpenUnitCardChestByDust()
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();
        SendRequest("RequestOpenUnitCardChestByDust", LitJson.JsonMapper.ToJson(req),
            (data) =>
            {
                UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
                if (ValidateResponse(response))
                {
                    if (response.ErrorCode == ERROR_CODE.OK)
                    {
                        UpdateUserInfo(response.UInfo);
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
            }, true);
    }

    public void RequestOpenUnitCardChestByGem()
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();
        SendRequest("RequestOpenUnitCardChestByGem", LitJson.JsonMapper.ToJson(req),
            (data) =>
            {
                UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
                if (ValidateResponse(response))
                {
                    if (response.ErrorCode == ERROR_CODE.OK)
                    {
                        UpdateUserInfo(response.UInfo);
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
            }, true);
    }

    public void RequestRecycleCard(string[] cardIDs)
    {
        RecycleCardRequest req = new RecycleCardRequest();
        req.cardIDs = cardIDs;
        req.UpdateGIDData();
        SendRequest("RequestRecycleCard", LitJson.JsonMapper.ToJson(req),
            (data) =>
            {
                UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
                if (ValidateResponse(response))
                {
                    if (response.ErrorCode == ERROR_CODE.OK)
                    {
                        UpdateUserInfo(response.UInfo);
                        if (PopupTheQuaiVat.instance)
                        {
                            PopupTheQuaiVat.instance.SyncNetworkData();
                        }
                        if (PopupRecycleCard.instance)
                        {
                            PopupRecycleCard.instance.Init();
                        }
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
            }, true);
    }
    
    public void RequestAddCardSlot(string itemID, string matID)
    {
        AddCardSlotRequest req = new AddCardSlotRequest();
        req.itemID = itemID;
        req.matItemID = matID;
        req.UpdateGIDData();
        SendRequest("RequestAddCardSlot", LitJson.JsonMapper.ToJson(req),
            (data) =>
            {
                UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
                if (ValidateResponse(response))
                {
                    if (response.ErrorCode == ERROR_CODE.OK)
                    {
                        UpdateUserInfo(response.UInfo);
                        if (Hiker.GUI.Shootero.PopupTrangBiInfo.instance)
                        {
                            Hiker.GUI.Shootero.PopupTrangBiInfo.instance.SyncNetworkData();
                        }
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
            }, true);
    }

    public void RequestUpdateCardSlot(string itemID, List<string> newCardSlots)
    {
        UpdateCardSlotRequest req = new UpdateCardSlotRequest();
        req.itemID = itemID;
        req.newCardSlots = newCardSlots;
        req.UpdateGIDData();
        SendRequest("RequestUpdateCardSlot", LitJson.JsonMapper.ToJson(req),
            (data) =>
            {
                UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
                if (ValidateResponse(response))
                {
                    if (response.ErrorCode == ERROR_CODE.OK)
                    {
                        UpdateUserInfo(response.UInfo);
                        if (Hiker.GUI.Shootero.PopupTrangBiInfo.instance)
                        {
                            Hiker.GUI.Shootero.PopupTrangBiInfo.instance.SyncNetworkData();
                        }
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
            }, true);
    }

    public void RequestUseGiftCode(string _maThuong)
    {
        UseGiftCodeRequest req = new UseGiftCodeRequest();
        req.giftcode = _maThuong;
        req.UpdateGIDData();
        SendRequest("RequestUseGiftCode", LitJson.JsonMapper.ToJson(req),
            (data) =>
            {
                UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
                if (ValidateResponse(response))
                {
                    if (response.ErrorCode == ERROR_CODE.OK)
                    {
                        UpdateUserInfo(response.UInfo);
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
            }, true);
    }

    public void RequestGetDailyShop()
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();
        SendRequest("RequestGetDailyShop", LitJson.JsonMapper.ToJson(req),
            (data) =>
            {
                UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
                if (ValidateResponse(response))
                {
                    if (response.ErrorCode == ERROR_CODE.OK)
                    {
                        UpdateUserInfo(response.UInfo);
                        if (PopupDailyShop.instance == null)
                        {
                            PopupDailyShop.Create();
                        }
                        else
                        {
                            PopupDailyShop.instance.Init();
                        }
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
            }, true);
    }

    public void RequestBuyDailyShopItem(int itemIdx, System.DateTime crResetTime)
    {
        BuyDailyShopItemRequest req = new BuyDailyShopItemRequest();
        req.itemIdx = itemIdx;
        req.crResetTime = crResetTime;
        req.UpdateGIDData();
        SendRequest("RequestBuyDailyShopItem", LitJson.JsonMapper.ToJson(req),
            (data) =>
            {
                UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
                if (ValidateResponse(response))
                {
                    if (response.ErrorCode == ERROR_CODE.OK)
                    {
                        UpdateUserInfo(response.UInfo);
                        if (PopupDailyShop.instance != null)
                        {
                            PopupDailyShop.instance.Init();
                        }
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
            }, true);
    }

    public void RequestBuyDailyShopItemByCandy(int itemIdx, System.DateTime crResetTime)
    {
        BuyDailyShopItemRequest req = new BuyDailyShopItemRequest();
        req.itemIdx = itemIdx;
        req.crResetTime = crResetTime;
        req.UpdateGIDData();
        SendRequest("RequestBuyDailyShopItemByCandy", LitJson.JsonMapper.ToJson(req),
            (data) =>
            {
                UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
                if (ValidateResponse(response))
                {
                    if (response.ErrorCode == ERROR_CODE.OK)
                    {
                        UpdateUserInfo(response.UInfo);
                        if (PopupDailyShop.instance != null)
                        {
                            PopupDailyShop.instance.Init();
                        }
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
            }, true);
    }

    public void RequestRefreshDailyShopByAds()
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();
        SendRequest("RequestRefreshDailyShopByAds", LitJson.JsonMapper.ToJson(req),
            (data) =>
            {
                UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
                if (ValidateResponse(response))
                {
                    if (response.ErrorCode == ERROR_CODE.OK)
                    {
                        UpdateUserInfo(response.UInfo);
                        if (PopupDailyShop.instance != null)
                        {
                            PopupDailyShop.instance.Init();
                        }
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
            }, true);
    }
}
