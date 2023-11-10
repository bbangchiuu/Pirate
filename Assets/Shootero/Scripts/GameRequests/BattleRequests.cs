using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hiker.Networks.Data;
using Hiker.Networks.Data.Shootero;
using Hiker.GUI.Shootero;

public partial class GameClient : HTTPClient
{
    public void RequestStartVongQuay()
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();

        SendRequest("RequestStartVongQuay", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            VongQuayResponse response = LitJson.JsonMapper.ToObject<VongQuayResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    if (PopupVongQuay.instance)
                    {
                        PopupVongQuay.instance.GetResponse(response);
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                    PopupVongQuay.Dismiss();
                }
            }
            else
            {
                PopupVongQuay.Dismiss();
            }
        }, false);
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public UpdateBattleRequest GetLastUpdateReq()
    {
        var reqUpdateJson = QuanlyNguoichoi.GetStringObsPref("LastBattleReq" + GID, string.Empty);
        if (string.IsNullOrEmpty(reqUpdateJson) == false)
        {
            var updateReq = LitJson.JsonMapper.ToObject<UpdateBattleRequest>(reqUpdateJson);
            return updateReq;
        }
        return null;
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public void SaveUpdateBattleReq(string reqJson, bool writeToDisk = true)
    {
        CodeStage.AntiCheat.Storage.ObscuredPrefs.SetString("LastBattleReq" + GID, reqJson);
        if (writeToDisk)
        {
            CodeStage.AntiCheat.Storage.ObscuredPrefs.Save();
        }
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public UpdateBattleRequest GetLastUpdateReqLeoThap()
    {
        var reqUpdateJson = QuanlyNguoichoi.GetStringObsPref("LastBattleLeoThapReq" + GID, string.Empty);
        if (string.IsNullOrEmpty(reqUpdateJson) == false)
        {
            var updateReq = LitJson.JsonMapper.ToObject<UpdateBattleRequest>(reqUpdateJson);
            return updateReq;
        }
        return null;
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public void SaveUpdateBattleReqLeoThap(string reqJson, bool writeToDisk = true)
    {
        CodeStage.AntiCheat.Storage.ObscuredPrefs.SetString("LastBattleLeoThapReq" + GID, reqJson);
        if (writeToDisk)
        {
            CodeStage.AntiCheat.Storage.ObscuredPrefs.Save();
        }
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public UpdateBattleRequest GetLastUpdateReqThachDau()
    {
        var reqUpdateJson = QuanlyNguoichoi.GetStringObsPref("LastBattleThachDauReq" + GID, string.Empty);
        if (string.IsNullOrEmpty(reqUpdateJson) == false)
        {
            var updateReq = LitJson.JsonMapper.ToObject<UpdateBattleRequest>(reqUpdateJson);
            return updateReq;
        }
        return null;
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public void SaveUpdateBattleReqThachDau(string reqJson, bool writeToDisk = true)
    {
        CodeStage.AntiCheat.Storage.ObscuredPrefs.SetString("LastBattleThachDauReq" + GID, reqJson);
        if (writeToDisk)
        {
            CodeStage.AntiCheat.Storage.ObscuredPrefs.Save();
        }
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public UpdateBattleRequest GetLastUpdateReqNienThu()
    {
        var reqUpdateJson = QuanlyNguoichoi.GetStringObsPref("LastBattleNienThuReq" + GID, string.Empty);
        if (string.IsNullOrEmpty(reqUpdateJson) == false)
        {
            var updateReq = LitJson.JsonMapper.ToObject<UpdateBattleRequest>(reqUpdateJson);
            return updateReq;
        }
        return null;
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public void SaveUpdateBattleReqNienThu(string reqJson, bool writeToDisk = true)
    {
        CodeStage.AntiCheat.Storage.ObscuredPrefs.SetString("LastBattleNienThuReq" + GID, reqJson);
        if (writeToDisk)
        {
            CodeStage.AntiCheat.Storage.ObscuredPrefs.Save();
        }
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public UpdateBattleRequest GetLastUpdateReqSanThe()
    {
        var reqUpdateJson = QuanlyNguoichoi.GetStringObsPref("LastBattleSanTheReq" + GID, string.Empty);
        if (string.IsNullOrEmpty(reqUpdateJson) == false)
        {
            var updateReq = LitJson.JsonMapper.ToObject<UpdateBattleRequest>(reqUpdateJson);
            return updateReq;
        }
        return null;
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public void SaveUpdateBattleReqSanThe(string reqJson, bool writeToDisk = true)
    {
        CodeStage.AntiCheat.Storage.ObscuredPrefs.SetString("LastBattleSanTheReq" + GID, reqJson);
        if (writeToDisk)
        {
            CodeStage.AntiCheat.Storage.ObscuredPrefs.Save();
        }
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public void RequestStartBattle(string battleID)
    {
        StartBattleRequest req = new StartBattleRequest();
        req.UpdateGIDData();
        req.BattleID = battleID;
        req.UpdateSignatureData();

        var updateReq = GetLastUpdateReq();

        if (updateReq != null && updateReq.BattleID == battleID)
        {
            req.UpdateReq = updateReq;
        }

        SendRequest("RequestStartBattle", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            StartBattleResponse response = LitJson.JsonMapper.ToObject<StartBattleResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    SaveUpdateBattleReq(string.Empty); // clear last update battle request when start new battle
                    UpdateUserInfo(response.UInfo);
                    Hiker.GUI.Shootero.GroupChapter.GoToBattle(response.battleData);
                }
                else if (response.ErrorCode == ERROR_CODE.NOT_FINISHED_BATTLE)
                {
                    if (string.IsNullOrEmpty(response.ErrorMessage) == false)
                    {
                        Hiker.GUI.PopupConfirm.Create(Localization.Get("MsgConfirmNotFinishedBattle"),
                            () => GameClient.instance.RequestStartBattle(response.ErrorMessage),
                            null,
                            Localization.Get("BtnContinue"));
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public void RequestStartBattle(int chapIdx, int farmModeRate)
    {
        StartBattleRequest req = new StartBattleRequest();
        req.UpdateGIDData();
        req.ChapIdx = chapIdx;
        req.FarmMode = farmModeRate;
        req.UpdateSignatureData();

        SendRequest("RequestStartBattle", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            StartBattleResponse response = LitJson.JsonMapper.ToObject<StartBattleResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    SaveUpdateBattleReq(string.Empty); // clear last update battle request when start new battle
                    UpdateUserInfo(response.UInfo);
                    Hiker.GUI.Shootero.GroupChapter.GoToBattle(response.battleData);
                }
                else if (response.ErrorCode == ERROR_CODE.NOT_FINISHED_BATTLE)
                {
                    if (string.IsNullOrEmpty(response.ErrorMessage) == false)
                    {
                        Hiker.GUI.PopupConfirm.Create(Localization.Get("MsgConfirmNotFinishedBattle"),
                            () => GameClient.instance.RequestStartBattle(response.ErrorMessage),
                            null,
                            Localization.Get("BtnContinue"));
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public void RequestEndBattle(UpdateBattleRequest req)
    {
        req.UpdateGIDData();
        bool isCompleteMap = req.IsCompleteMap;
        if (CurBattleID == req.BattleID)
        {
            CurBattleID = string.Empty;
        }

        if (req.PlayerStat.HP <= 0) // fail battle
        {
            if (playerManager.IsNormalMode && // is campaign mode
                playerManager.ChapterIndex == UInfo.GetCurrentChapter()) // current chapter
            {
                AnalyticsManager.LogEvent("FAIL_CHAPTER_" + playerManager.ChapterIndex,
                    new AnalyticsParameter("DungeonID", playerManager.CurDungeonId));
            }
            
        }

        // gen secretKey
        req.SecKey = "HkSecReqBattle";
        var s = LitJson.JsonMapper.ToJson(req);
        var secretKey = Md5Hash.GetMd5Hash(s);
        req.SecKey = secretKey;

        SendRequest("RequestUpdateBattle", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            req.IsEnd = false; // use this field to track request end is finish;

            UpdateBattleResponse response = LitJson.JsonMapper.ToObject<UpdateBattleResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    if (req.BattleMode == 2)
                    {
                        GameClient.instance.UInfo.BattleLeoThap = response.UInfo.BattleLeoThap;
                        GameClient.instance.UInfo.LuotLeoThap = response.UInfo.LuotLeoThap;
                    }
                    else if (req.BattleMode == 3)
                    {
                        GameClient.instance.UInfo.BattleSanThe = response.UInfo.BattleSanThe;
                        GameClient.instance.UInfo.LuotSanThe = response.UInfo.LuotSanThe;
                    }
                    else if (req.BattleMode == 4)
                    {
                        GameClient.instance.UInfo.BattleThachDau = response.UInfo.BattleThachDau;
                        GameClient.instance.UInfo.LuotThachDau = response.UInfo.LuotThachDau;
                    }

                    UpdateUserInfo(response.UInfo);
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public void RequestUpdateBattle(UpdateBattleRequest req)
    {
        req.UpdateGIDData();
        req.UpdateSignatureData();

        // gen secretKey
        req.SecKey = "HkSecReqBattle";
        var s = LitJson.JsonMapper.ToJson(req);
        var secretKey = Md5Hash.GetMd5Hash(s);
        req.SecKey = secretKey;

        var reqJson = LitJson.JsonMapper.ToJson(req);
        SendRequest("RequestUpdateBattle", reqJson, (data) =>
        {
            UpdateBattleResponse response = LitJson.JsonMapper.ToObject<UpdateBattleResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);

                    if (response.TKSTNumber > 0)
                    {
                        QuanlyNguoichoi.Instance.ClearOldTK(response.TKSTNumber);
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, false, true);

#if DEBUG
        Debug.Log("Update Battle Request to Pref " + req.Exp);
#endif
        SaveLastBattleRequest(req, reqJson);
    }

    public void SaveLastBattleRequest(UpdateBattleRequest req, string reqJson)
    {
        switch (req.BattleMode)
        {
            case 2:
                SaveUpdateBattleReqLeoThap(reqJson);
                break;
            case 3:
                SaveUpdateBattleReqSanThe(reqJson);
                break;
            case 4:
                SaveUpdateBattleReqThachDau(reqJson);
                break;
            case 5:
                SaveUpdateBattleReqNienThu(reqJson);
                break;
            default:
                SaveUpdateBattleReq(reqJson);
                break;
        }
    }

    public UpdateBattleRequest GetLastBattleRequest(int battleMode)
    {
        switch (battleMode)
        {
            case 2:
                return GetLastUpdateReqLeoThap();
            case 3:
                return GetLastUpdateReqSanThe();
            case 4:
                return GetLastUpdateReqThachDau();
            case 5:
                return GetLastUpdateReqNienThu();
            default:
                return GetLastUpdateReq();
        }
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public void RequestReviveBattle(int battleMode)
    {
        ScreenBattle.PauseGame(true);

        var req = GetLastBattleRequest(battleMode);
        
        if (req != null)
        {
            req.UpdateGIDData();
            req.LuotHS++;

            var reqJson = LitJson.JsonMapper.ToJson(req);
            SendRequest("RequestReviveInBattle", reqJson, (data) =>
            {
                UpdateBattleResponse response = LitJson.JsonMapper.ToObject<UpdateBattleResponse>(data);
                if (ValidateResponse(response))
                {
                    ScreenBattle.PauseGame(false);
                    if (response.ErrorCode == ERROR_CODE.OK)
                    {
                        QuanlyNguoichoi.Instance.ReviveNow();

                        UpdateUserInfo(response.UInfo);
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
            }, true, false);

#if DEBUG
            Debug.Log("Update Battle Request to Pref " + req.Exp);
#endif

            SaveLastBattleRequest(req, reqJson);
            
        }
    }

    //public void RequestCompleteChapter(StartBattleRequest req)
    //{
    //    req.UpdateGIDData();

    //    SendRequest("RequestCompleteChapter", LitJson.JsonMapper.ToJson(req), (data) =>
    //    {
    //        UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
    //        if (ValidateResponse(response))
    //        {
    //            if (response.ErrorCode == ERROR_CODE.OK)
    //            {
    //                UpdateUserInfo(response.UInfo);
    //            }
    //            else
    //            {
    //                ProcessErrorResponse(response);
    //            }
    //        }
    //    }, true);
    //}

    //public void RequestGetRuongBattle(GetRuongBattleRequest req)
    //{
    //    req.UpdateGIDData();
    //    string key = string.Format("HSh00t3roK_{0}_{1}_{2}_{3};", req.BattleID, req.ChapIdx, req.Ruong, req.token);
    //    string token = Md5Hash.GetMd5Hash(key);
    //    req.VerifyKey = token;

    //    SendRequest("RequestGetRuongBattle", LitJson.JsonMapper.ToJson(req), (data) =>
    //    {
    //        GetRuongBattlResponse response = LitJson.JsonMapper.ToObject<GetRuongBattlResponse>(data);
    //        if (ValidateResponse(response))
    //        {
    //            if (response.ErrorCode == ERROR_CODE.OK)
    //            {
    //                UpdateUserInfo(response.UInfo);
    //                Hiker.GUI.Shootero.PopupOpenChest.Create(response.reward);
    //            }
    //            else
    //            {
    //                ProcessErrorResponse(response);
    //            }
    //        }
    //    }, true);
    //}

    //public StartBattleEventResponse responseBattleEventRoom { get; private set; }

    //public void RequestStartBattleEventRoom(string battleID)
    //{
    //    var req = new StartBattleRequest();
    //    req.UpdateGIDData();
    //    req.BattleID = battleID;
    //    //responseBattleEventRoom = null;
    //    SendRequest("RequestStartBattleEvent", LitJson.JsonMapper.ToJson(req), (data) =>
    //    {
    //        StartBattleEventResponse response = LitJson.JsonMapper.ToObject<StartBattleEventResponse>(data);
    //        if (response.ErrorCode == ERROR_CODE.OK)
    //        {
    //            //responseBattleEventRoom = response;

    //            if (BattleEventController.instance)
    //            {
    //                BattleEventController.instance.GetBattleEventResponse(response);
    //            }
    //        }
    //        else
    //        {
    //            ProcessErrorResponse(response);
    //        }
    //    }, true);
    //}

    public void RequestBuyInSecretShop(string itemID)
    {
        BuySecretShopRequest req = new BuySecretShopRequest();
        req.UpdateGIDData();
        req.ItemID = itemID;
        SendRequest("RequestBuyInSecretShop", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    if (PopupBattleSecretShop.instance)
                    {
                        PopupBattleSecretShop.instance.OnTradeInSuccess(req.ItemID);
                    }
                    if (response.UInfo.Rewards != null && response.UInfo.Rewards.Count > 0)
                    {
                        Hiker.GUI.Shootero.PopupOpenChest_new.Create(response.UInfo.Rewards[0]);
                    }
                    response.UInfo.Rewards.Clear();
                    UpdateUserInfo(response.UInfo);
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestClaimBossHunt(string boss)
    {
        ClaimBossHuntRequest req = new ClaimBossHuntRequest();
        req.UpdateGIDData();
        req.Boss = boss;

        SendRequest("RequestClaimBossHunt", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);
                    if (PopupBossHunt.instance)
                    {
                        PopupBossHunt.instance.Init();
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestGetAFKReward()
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();

        SendRequest("RequestGetAFKReward", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);
                    
                    PopupAFKReward.Create();
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestClaimAFKReward(AFKRewardData _afkRewardData)
    {
        ClaimAFKRewardResquest req = new ClaimAFKRewardResquest();
        req.UpdateGIDData();
        req.afkRewardData = _afkRewardData;

        SendRequest("RequestClaimAFKReward", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);
                    //Close AFK reward popup
                    PopupAFKReward.Dismiss();
                }
                else
                {
                    PopupAFKReward.Dismiss();
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestCloseMailBox()
    {
        if (UInfo == null) return;

        CloseMailBoxRequest req = new CloseMailBoxRequest();
        req.UpdateGIDData();
        req.listMails = UInfo.ListMails;

        SendRequest("RequestCloseMailBox", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (response.ErrorCode == ERROR_CODE.OK)
            {
                UpdateUserInfo(response.UInfo);
            }
            //else
            //{
            //    ProcessErrorResponse(response);
            //}
        }, false);
    }

    public void RequestClaimMailReward(MailData _mailData)
    {
        ClaimMailRewardRequest req = new ClaimMailRewardRequest();
        req.UpdateGIDData();
        req.claimMailData = _mailData;

        SendRequest("RequestClaimMailReward", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    //Update mail box
                    UpdateUserInfo(response.UInfo);
                    if (PopupMailBox.instance)
                        PopupMailBox.instance.Init();
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestAddTheLuc()
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();
        
        SendRequest("RequestAddTheLuc", LitJson.JsonMapper.ToJson(req), (data) =>
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

    public void RequestWatchAdsTheLuc()
    {
        WatchAdsRequest req = new WatchAdsRequest();
        req.UpdateGIDData();
        req.adsId = System.Guid.NewGuid().ToString();

        SendRequest("RequestWatchAdsTheLuc", LitJson.JsonMapper.ToJson(req), (data) =>
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

    public void RequestWatchAdsRevive()
    {
        WatchAdsRequest req = new WatchAdsRequest();
        req.UpdateGIDData();
        req.adsId = System.Guid.NewGuid().ToString();

        SendRequest("RequestWatchAdsRevive", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);

            if (response.ErrorCode == ERROR_CODE.OK)
            {
                UpdateUserInfo(response.UInfo);
            }
            else
            {
                ProcessErrorResponse(response);
            }
        }, false, true);
    }

    public void RequestFullRefreshTheLuc()
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();
        
        SendRequest("RequestFullRefreshTheLuc", LitJson.JsonMapper.ToJson(req), (data) =>
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

    public void RequestWatchAdsGem()
    {
        WatchAdsRequest req = new WatchAdsRequest();
        req.UpdateGIDData();
        req.adsId = System.Guid.NewGuid().ToString();

        SendRequest("RequestWatchAdsGem", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);
                    if (Hiker.GUI.PopupBuyGem.Instance) { Hiker.GUI.PopupBuyGem.Instance.InitItems(); }
                    if (PopupTongHopQuangCao.instance) PopupTongHopQuangCao.instance.Init();
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestStartVongQuayAds()
    {
        WatchAdsRequest req = new WatchAdsRequest();
        req.UpdateGIDData();
        req.adsId = System.Guid.NewGuid().ToString();

        SendRequest("RequestStartVongQuayAds", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            VongQuayResponse response = LitJson.JsonMapper.ToObject<VongQuayResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    if (PopupVongQuay.instance)
                    {
                        PopupVongQuay.instance.GetResponse(response);
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestWatchAdsThemVang(int _chapIdx)
    {
        WatchAdsThemVangRequest req = new WatchAdsThemVangRequest();
        req.UpdateGIDData();
        req.adsId = System.Guid.NewGuid().ToString();
        req.chapIdx = _chapIdx;

        SendRequest("RequestWatchAdsThemVang", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            WatchAdsThemVangResponse response = LitJson.JsonMapper.ToObject<WatchAdsThemVangResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);
                    if (PopupEndBattle.instance)
                        PopupEndBattle.instance.OnCompleteWatchAdsThemVang(response.soLuongVangThem);
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestExtraAFKWatchAds(string prefix = "")
    {
        WatchAdsRequest req = new WatchAdsRequest();
        req.UpdateGIDData();
        req.adsId = prefix + System.Guid.NewGuid().ToString();

        SendRequest("RequestExtraAFKWatchAds", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    if (string.IsNullOrEmpty(prefix)) PlayerPrefs.SetString(response.UInfo.GID + "ExtraAFKNextWatchAdsTime", 
                        System.DateTime.Now.AddMinutes(ConfigManager.GetExtraAFKWatchAdsDelayMin()).ToString());

                    UpdateUserInfo(response.UInfo);
                    if (PopupAFKReward.instance) { PopupAFKReward.instance.Init(null); ; }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestDailyGoldWatchAds()
    {
        WatchAdsRequest req = new WatchAdsRequest();
        req.UpdateGIDData();
        req.adsId = System.Guid.NewGuid().ToString();

        SendRequest("RequestDailyGoldWatchAds", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);
                    if (PopupTongHopQuangCao.instance) PopupTongHopQuangCao.instance.Init();
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestDailyMatWatchAds()
    {
        WatchAdsRequest req = new WatchAdsRequest();
        req.UpdateGIDData();
        req.adsId = System.Guid.NewGuid().ToString();

        SendRequest("RequestDailyMatWatchAds", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);
                    if (PopupTongHopQuangCao.instance) PopupTongHopQuangCao.instance.Init();
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestDailyItemWatchAds()
    {
        WatchAdsRequest req = new WatchAdsRequest();
        req.UpdateGIDData();
        req.adsId = System.Guid.NewGuid().ToString();

        SendRequest("RequestDailyItemWatchAds", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);
                    if (PopupTongHopQuangCao.instance) PopupTongHopQuangCao.instance.Init();
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestUpStarMysthicItem(string itemID, string matID)
    {
        UpStarMysthicItemRequest req = new UpStarMysthicItemRequest();
        req.UpdateGIDData();
        req.itemID = itemID;
        req.matItemID = matID;

        SendRequest("RequestUpStarMysthicItem", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UpStarMysthicItemResponse response = LitJson.JsonMapper.ToObject<UpStarMysthicItemResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    if (PopupTinhLuyenNangSao.instance)
                    {
                        TrangBiData tbData = response.UInfo.ListTrangBi.Find(e => e.ID == response.itemID);
                        PopupTinhLuyenNangSao.instance.ReceiveItem(tbData);
                        PopupTinhLuyenNangSao.instance.onPopupClosed.AddListener(() =>
                        {
                            UpdateUserInfo(response.UInfo);
                        });
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                    if (PopupTinhLuyenNangSao.instance)
                    {
                        PopupTinhLuyenNangSao.instance.OnCloseBtnClick();
                    }
                }
            }
        }, true);
    }

    public void RequestReforgeMysthicItem(string itemID, string matID)
    {
        ReforgeMysthicItemRequest req = new ReforgeMysthicItemRequest();
        req.UpdateGIDData();
        req.itemID = itemID;
        req.matItemID = matID;

        SendRequest("RequestReforgeMysthicItem", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            ReforgeMysthicItemResponse response = LitJson.JsonMapper.ToObject<ReforgeMysthicItemResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    if (PopupTinhLuyenDoiTB.instance)
                    {
                        TrangBiData tbData = response.UInfo.ListTrangBi.Find(e => e.ID == response.itemID);
                        PopupTinhLuyenDoiTB.instance.ReceiveItem(tbData);
                        PopupTinhLuyenDoiTB.instance.onPopupClosed.AddListener(() =>
                        {
                            UpdateUserInfo(response.UInfo);
                        });
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                    if (PopupTinhLuyenNangSao.instance)
                    {
                        PopupTinhLuyenNangSao.instance.OnCloseBtnClick();
                    }
                }
            }
        }, true);
    }

    public void RequestChangeStyleMysthicItem(string itemID, string newStyleName)
    {
        ChangeStyleMysthicItemRequest req = new ChangeStyleMysthicItemRequest();
        req.UpdateGIDData();
        req.itemID = itemID;
        req.newStyleName = newStyleName;

        SendRequest("RequestChangeStyleMysthicItem", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            ChangeStyleMysthicItemResponse response = LitJson.JsonMapper.ToObject<ChangeStyleMysthicItemResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    if (PopupTinhLuyenDoiTB.instance)
                    {
                        TrangBiData tbData = response.UInfo.ListTrangBi.Find(e => e.ID == response.itemID);
                        PopupTinhLuyenDoiTB.instance.ReceiveItem(tbData);
                        PopupTinhLuyenDoiTB.instance.onPopupClosed.AddListener(() =>
                        {
                            UpdateUserInfo(response.UInfo);
                        });
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                    if (PopupTinhLuyenNangSao.instance)
                    {
                        PopupTinhLuyenNangSao.instance.OnCloseBtnClick();
                    }
                }
            }
        }, true);
    }

    public void RequestGetLeoThapData()
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();

        SendRequest("RequestGetLeoThapData", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            LeoThapDataResponse response = LitJson.JsonMapper.ToObject<LeoThapDataResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);

                    if (PopupLeoThap.instance == null)
                    {
                        PopupLeoThap.Create(response);
                    }
                    else
                    {
                        PopupLeoThap.instance.Init(response);
                        PopupLeoThap.instance.gameObject.SetActive(true);
                    }

                    ScreenMain.instance.grpChapter.SyncNetworkData();
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestGetThachDauData()
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();

        SendRequest("RequestGetThachDauData", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            ThachDauDataResponse response = LitJson.JsonMapper.ToObject<ThachDauDataResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);

                    if (PopupThachDau.instance == null)
                    {
                        PopupThachDau.Create(response);
                    }
                    else
                    {
                        PopupThachDau.instance.Init(response);
                        PopupThachDau.instance.gameObject.SetActive(true);
                    }

                    ScreenMain.instance.grpChapter.SyncNetworkData();
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestJoinThachDau()
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();

        SendRequest("RequestJoinThachDau", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            ThachDauDataResponse response = LitJson.JsonMapper.ToObject<ThachDauDataResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);

                    if (PopupThachDau.instance == null)
                    {
                        PopupThachDau.Create(response);
                    }
                    else
                    {
                        PopupThachDau.instance.Init(response);
                        PopupThachDau.instance.gameObject.SetActive(true);
                    }

                    ScreenMain.instance.grpChapter.SyncNetworkData();
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestJoinThachDauGem()
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();

        SendRequest("RequestJoinThachDauGem", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            ThachDauDataResponse response = LitJson.JsonMapper.ToObject<ThachDauDataResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);

                    if (PopupThachDau.instance == null)
                    {
                        PopupThachDau.Create(response);
                    }
                    else
                    {
                        PopupThachDau.instance.Init(response);
                        PopupThachDau.instance.gameObject.SetActive(true);
                    }

                    ScreenMain.instance.grpChapter.SyncNetworkData();
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestGetPhanThuongTopLeoThap()
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();

        SendRequest("RequestGetPhanThuongTopLeoThap", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            LeoThapDataResponse response = LitJson.JsonMapper.ToObject<LeoThapDataResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);
                    if (PopupLeoThap.instance != null)
                    {
                        PopupLeoThap.instance.TweenToNormalGrp();
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestGetPhanThuongTopThachDau()
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();

        SendRequest("RequestGetPhanThuongTopThachDau", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            ThachDauDataResponse response = LitJson.JsonMapper.ToObject<ThachDauDataResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);
                    if (PopupThachDau.instance != null)
                    {
                        PopupThachDau.instance.TweenToNormalGrp(response);
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public void RequestStartBattleLeoThap(string battleID)
    {
        StartBattleRequest req = new StartBattleRequest();
        req.UpdateGIDData();
        req.UpdateSignatureData();
        req.BattleID = battleID;

        var updateReq = GetLastUpdateReqLeoThap();

        if (updateReq != null && updateReq.BattleID == battleID)
        {
            req.UpdateReq = updateReq;
        }

        SendRequest("RequestStartBattleLeoThap", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            StartBattleResponse response = LitJson.JsonMapper.ToObject<StartBattleResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    SaveUpdateBattleReqLeoThap(string.Empty); // clear last update battle request when start new battle
                    UpdateUserInfo(response.UInfo);
                    if (PopupLeoThap.instance)
                    {
                        PopupLeoThap.instance.OnCloseBtnClick();
                    }
                    Hiker.GUI.Shootero.GroupChapter.GoToBattle(response.battleData);
                }
                else if (response.ErrorCode == ERROR_CODE.NOT_FINISHED_BATTLE)
                {
                    if (string.IsNullOrEmpty(response.ErrorMessage) == false)
                    {
                        Hiker.GUI.PopupConfirm.Create(Localization.Get("MsgConfirmNotFinishedBattle"),
                            () => GameClient.instance.RequestStartBattleLeoThap(response.ErrorMessage),
                            null,
                            Localization.Get("BtnContinue"));
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public void RequestStartBattleThachDau(string battleID)
    {
        StartBattleRequest req = new StartBattleRequest();
        req.UpdateGIDData();
        req.UpdateSignatureData();
        req.BattleID = battleID;

        var updateReq = GetLastUpdateReqThachDau();

        if (updateReq != null && updateReq.BattleID == battleID)
        {
            req.UpdateReq = updateReq;
        }

        SendRequest("RequestStartBattleThachDau", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            StartThachDauBattleResponse response = LitJson.JsonMapper.ToObject<StartThachDauBattleResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    SaveUpdateBattleReqThachDau(string.Empty); // clear last update battle request when start new battle
                    UpdateUserInfo(response.UInfo);
                    if (PopupThachDau.instance)
                    {
                        PopupThachDau.instance.OnCloseBtnClick();
                    }

                    playerManager.ThachDauScenes.Clear();
                    playerManager.ThachDauScenes.AddRange(response.Levels);
                    Hiker.GUI.Shootero.GroupChapter.GoToBattle(response.battleData);
                }
                else if (response.ErrorCode == ERROR_CODE.NOT_FINISHED_BATTLE)
                {
                    if (string.IsNullOrEmpty(response.ErrorMessage) == false)
                    {
                        Hiker.GUI.PopupConfirm.Create(Localization.Get("MsgConfirmNotFinishedBattle"),
                            () => GameClient.instance.RequestStartBattleThachDau(response.ErrorMessage),
                            null,
                            Localization.Get("BtnContinue"));
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public void RequestStartBattleLeoThap()
    {
        StartBattleRequest req = new StartBattleRequest();
        req.UpdateGIDData();
        req.UpdateSignatureData();
        //req.ChapIdx = chapIdx;

        SendRequest("RequestStartBattleLeoThap", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            StartBattleResponse response = LitJson.JsonMapper.ToObject<StartBattleResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    SaveUpdateBattleReqLeoThap(string.Empty); // clear last update battle request when start new battle
                    UpdateUserInfo(response.UInfo);
                    if (PopupLeoThap.instance)
                    {
                        PopupLeoThap.instance.OnCloseBtnClick();
                    }
                    Hiker.GUI.Shootero.GroupChapter.GoToBattle(response.battleData);
                }
                else if (response.ErrorCode == ERROR_CODE.NOT_FINISHED_BATTLE)
                {
                    if (string.IsNullOrEmpty(response.ErrorMessage) == false)
                    {
                        Hiker.GUI.PopupConfirm.Create(Localization.Get("MsgConfirmNotFinishedBattle"),
                            () => GameClient.instance.RequestStartBattleLeoThap(response.ErrorMessage),
                            null,
                            Localization.Get("BtnContinue"));
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public void RequestStartBattleThachDau()
    {
        StartBattleRequest req = new StartBattleRequest();
        req.UpdateGIDData();
        req.UpdateSignatureData();
        //req.ChapIdx = chapIdx;

        SendRequest("RequestStartBattleThachDau", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            StartThachDauBattleResponse response = LitJson.JsonMapper.ToObject<StartThachDauBattleResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    SaveUpdateBattleReqThachDau(string.Empty); // clear last update battle request when start new battle
                    UpdateUserInfo(response.UInfo);
                    if (PopupThachDau.instance)
                    {
                        PopupThachDau.instance.OnCloseBtnClick();
                    }
                    playerManager.ThachDauScenes.Clear();
                    playerManager.ThachDauScenes.AddRange(response.Levels);
                    Hiker.GUI.Shootero.GroupChapter.GoToBattle(response.battleData);
                }
                else if (response.ErrorCode == ERROR_CODE.NOT_FINISHED_BATTLE)
                {
                    if (string.IsNullOrEmpty(response.ErrorMessage) == false)
                    {
                        Hiker.GUI.PopupConfirm.Create(Localization.Get("MsgConfirmNotFinishedBattle"),
                            () => GameClient.instance.RequestStartBattleThachDau(response.ErrorMessage),
                            null,
                            Localization.Get("BtnContinue"));
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public void RequestStartBattleNienThu()
    {
        StartBattleRequest req = new StartBattleRequest();
        req.UpdateGIDData();
        req.UpdateSignatureData();
        //req.ChapIdx = chapIdx;

        SendRequest("RequestStartBattleNienThu", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            StartNienThuBattleResponse response = LitJson.JsonMapper.ToObject<StartNienThuBattleResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    SaveUpdateBattleReqNienThu(string.Empty); // clear last update battle request when start new battle
                    UpdateUserInfo(response.UInfo);
                    if (PopupTetNienThu.instance)
                    {
                        PopupTetNienThu.instance.OnCloseBtnClick();
                    }
                    //playerManager.ThachDauScenes.Clear();
                    //playerManager.ThachDauScenes.AddRange(response.Levels);
                    playerManager.NienThuScene = response.Level;
                    Hiker.GUI.Shootero.GroupChapter.GoToBattle(response.battleData);
                }
                else if (response.ErrorCode == ERROR_CODE.NOT_FINISHED_BATTLE)
                {
                    if (string.IsNullOrEmpty(response.ErrorMessage) == false)
                    {
                        Hiker.GUI.PopupConfirm.Create(Localization.Get("MsgConfirmNotFinishedBattle"),
                            () => GameClient.instance.RequestStartBattleThachDau(response.ErrorMessage),
                            null,
                            Localization.Get("BtnContinue"));
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestGetLastBattleLeoThapReward()
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();
        //req.ChapIdx = chapIdx;

        SendRequest("RequestGetLastBattleLeoThapReward", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            GetLastBattleLeoThapRewardResponse response = LitJson.JsonMapper.ToObject<GetLastBattleLeoThapRewardResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    //hien thi tong hop tran truoc PopupLeoThap
                    if(PopupLeoThap.instance != null)
                    {
                        PopupLeoThap.instance.ShowLastBattleSummary(response.lastBattleRewards, response.lastBattleFloor);
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestQuickLeoThapBattle()
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();
        //req.ChapIdx = chapIdx;

        SendRequest("RequestQuickLeoThapBattle", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    if (PopupLeoThap.instance != null)
                    {
                        PopupLeoThap.instance.OnCloseBtnClick();
                    }
                    UpdateUserInfo(response.UInfo);
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestGetSanTheData()
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();

        SendRequest("RequestGetSanTheData", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            SanTheDataResponse response = LitJson.JsonMapper.ToObject<SanTheDataResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);

                    if (PopupSanThe.instance == null)
                    {
                        PopupSanThe.Create(response);
                    }
                    else
                    {
                        PopupSanThe.instance.Init(response);
                        PopupSanThe.instance.gameObject.SetActive(true);
                    }

                    ScreenMain.instance.grpChapter.SyncNetworkData();
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestResetSanTheData()
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();

        SendRequest("RequestResetSanTheData", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            SanTheDataResponse response = LitJson.JsonMapper.ToObject<SanTheDataResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);

                    if (PopupSanThe.instance == null)
                    {
                        PopupSanThe.Create(response);
                    }
                    else
                    {
                        PopupSanThe.instance.Init(response);
                        PopupSanThe.instance.gameObject.SetActive(true);
                    }

                    ScreenMain.instance.grpChapter.SyncNetworkData();
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public void RequestStartBattleSanThe(string battleID)
    {
        StartBattleRequest req = new StartBattleRequest();
        req.UpdateGIDData();
        req.UpdateSignatureData();
        req.BattleID = battleID;

        var updateReq = GetLastUpdateReqSanThe();

        if (updateReq != null && updateReq.BattleID == battleID)
        {
            req.UpdateReq = updateReq;
        }

        SendRequest("RequestStartBattleSanThe", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            StartBattleResponse response = LitJson.JsonMapper.ToObject<StartBattleResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    SaveUpdateBattleReqSanThe(string.Empty); // clear last update battle request when start new battle
                    UpdateUserInfo(response.UInfo);
                    if (PopupSanThe.instance)
                    {
                        PopupSanThe.instance.OnCloseBtnClick();
                    }
                    Hiker.GUI.Shootero.GroupChapter.GoToBattle(response.battleData);
                }
                else if (response.ErrorCode == ERROR_CODE.NOT_FINISHED_BATTLE)
                {
                    if (string.IsNullOrEmpty(response.ErrorMessage) == false)
                    {
                        Hiker.GUI.PopupConfirm.Create(Localization.Get("MsgConfirmNotFinishedBattle"),
                            () => GameClient.instance.RequestStartBattleSanThe(response.ErrorMessage),
                            null,
                            Localization.Get("BtnContinue"));
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    [Beebyte.Obfuscator.ObfuscateLiterals]
    public void RequestStartBattleSanThe()
    {
        StartBattleRequest req = new StartBattleRequest();
        req.UpdateGIDData();
        req.UpdateSignatureData();
        //req.ChapIdx = chapIdx;

        SendRequest("RequestStartBattleSanThe", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            StartBattleResponse response = LitJson.JsonMapper.ToObject<StartBattleResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    SaveUpdateBattleReqSanThe(string.Empty); // clear last update battle request when start new battle
                    UpdateUserInfo(response.UInfo);
                    if (PopupSanThe.instance)
                    {
                        PopupSanThe.instance.OnCloseBtnClick();
                    }
                    Hiker.GUI.Shootero.GroupChapter.GoToBattle(response.battleData);
                }
                else if (response.ErrorCode == ERROR_CODE.NOT_FINISHED_BATTLE)
                {
                    if (string.IsNullOrEmpty(response.ErrorMessage) == false)
                    {
                        Hiker.GUI.PopupConfirm.Create(Localization.Get("MsgConfirmNotFinishedBattle"),
                            () => GameClient.instance.RequestStartBattleSanThe(response.ErrorMessage),
                            null,
                            Localization.Get("BtnContinue"));
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestClaimTruongThanh(int chapIdx)
    {
        ClaimTruongThanhChapterRequest req = new ClaimTruongThanhChapterRequest();
        req.UpdateGIDData();
        req.chapIdx = chapIdx;

        SendRequest("RequestClaimTruongThanh", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);
                    if (PopupTruongThanh.instance != null)
                    {
                        PopupTruongThanh.instance.Init();
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestClaimTetEventRewards()
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();

        SendRequest("RequestClaimTetEventRewards", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);
                    if (PopupTetEventBattlePass.instance != null)
                    {
                        PopupTetEventBattlePass.instance.Init();
                    }
                    if (PopupTetNienThu.instance != null)
                    {
                        PopupTetNienThu.instance.alertReward.SetActive(false);
                    }
                }
                else
                {
                    ProcessErrorResponse(response);
                }
            }
        }, true);
    }

    public void RequestBuyTetEventLevel()
    {
        GIDRequest req = new GIDRequest();
        req.UpdateGIDData();

        SendRequest("RequestBuyTetEventLevel", LitJson.JsonMapper.ToJson(req), (data) =>
        {
            UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
            if (ValidateResponse(response))
            {
                if (response.ErrorCode == ERROR_CODE.OK)
                {
                    UpdateUserInfo(response.UInfo);
                    if (PopupTetEventBattlePass.instance != null)
                    {
                        PopupTetEventBattlePass.instance.Init();
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
