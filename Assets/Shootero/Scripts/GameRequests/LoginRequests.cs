using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hiker.Networks.Data;
using Hiker.Networks.Data.Shootero;

public partial class GameClient : HTTPClient
{
    public void TestRequest()
    {
        //SendRequest("TestRequest1", "duongrs", (data) =>
        //{
        //    Debug.Log(data);
        //});
    }

    public void RequestAssetBundlesInfo(System.Action<string> onsuccess, System.Action<string> onfail)
    {
        SendRequest("RequestAssetBundlesInfo",
            string.Empty,
            (data) =>
            {
                AssetBundleConfigResponse response = LitJson.JsonMapper.ToObject<AssetBundleConfigResponse>(data);
                if (response != null && response.ErrorCode == ERROR_CODE.OK)
                {
                    onsuccess(data);
                }
                else
                {
                    onfail(data);
                }
            },
            false,
            true,
            (error) =>
            {
                onfail(string.Empty);
            }, custom_timeOut: 20);
    }

    public void RequestGetUserInfo(string[] props, bool isRunInBackground)
    {
        GetUserInfoRequest request = new GetUserInfoRequest();
        request.UpdateGIDData();
        request.Props = props;

        SendRequest("RequestGetUserInfo",
            LitJson.JsonMapper.ToJson(request),
            (data) =>
            {
                LoginResponse response = LitJson.JsonMapper.ToObject<LoginResponse>(data);
                if (ValidateResponse(response))
                {
                    if (response.ErrorCode == ERROR_CODE.OK)
                    {
                        foreach (var s in props)
                        {
                            if (s == "leothap")
                            {
                                UInfo.LuotLeoThap = response.UInfo.LuotLeoThap;
                                UInfo.LeoThapRankIdx = response.UInfo.LeoThapRankIdx;
                                UInfo.BattleLeoThap = response.UInfo.BattleLeoThap;
                            }
                            else if (s == "santhe")
                            {
                                UInfo.LuotSanThe = response.UInfo.LuotSanThe;
                                UInfo.BattleSanThe = response.UInfo.BattleSanThe;
                            }
                            else if (s == "thachdau")
                            {
                                UInfo.LuotThachDau = response.UInfo.LuotThachDau;
                                UInfo.BattleThachDau = response.UInfo.BattleThachDau;
                            }
                        }

                        UpdateUserInfo(response.UInfo);
                        
                        PlayerPrefs.SetString("LastTimeUpdateInfo_" + response.UInfo.GID.ToString(),
                            TimeUtils.Now.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        if (PushNotificationManager.Instance)
                        {
                            PushNotificationManager.Instance.CheckNotificationComeBack();
                        }
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
            },
            isRunInBackground == false,
            isRunInBackground);
    }

    public void RequestDoiTen(string newName, System.Action onSuccess)
    {
        DoiTenRequest request = new DoiTenRequest();
        request.UpdateGIDData();
        request.newName = newName;

        SendRequest("RequestDoiTen",
            LitJson.JsonMapper.ToJson(request),
            (data) =>
            {
                UserInfoResponse response = LitJson.JsonMapper.ToObject<UserInfoResponse>(data);
                if (ValidateResponse(response))
                {
                    if (response.ErrorCode == ERROR_CODE.OK)
                    {
                        UpdateUserInfo(response.UInfo);
                        onSuccess?.Invoke();
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
            },
            true);
    }

    public void RequestGetComeBackMail()
    {
        GIDRequest request = new GIDRequest();
        request.UpdateGIDData();

        SendRequest("RequestGetComeBackMail",
            LitJson.JsonMapper.ToJson(request),
            (data) =>
            {
                LoginResponse response = LitJson.JsonMapper.ToObject<LoginResponse>(data);
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
            },
            true);

        AnalyticsManager.LogEvent("COME_BACK");
    }

    void DisplayerResponseMessage(Hiker.GUI.MessagePopupType msgType, string srvMessage)
    {
#if DEBUG
        Debug.Log(srvMessage);
#endif
        if (srvMessage.StartsWith("[LOCAL]"))
        {
            Hiker.GUI.PopupMessage.Create(Hiker.GUI.MessagePopupType.ERROR,
                Localization.Get(srvMessage.Substring(7)));
        }
        else
        {
            Hiker.GUI.PopupMessage.Create(Hiker.GUI.MessagePopupType.ERROR, srvMessage);
        }
    }

    bool ValidateResponse(ExDataBase response)
    {
        if (response == null)
        {
            Debug.Log("response is null");
            return false;
        }

        switch (response.ErrorCode)
        {
            case ERROR_CODE.OK:
                return true;
            case ERROR_CODE.INVALID_REQUEST:
                RequestGetUserInfo(UserInfo.ALL_PROPS, true);
                if (string.IsNullOrEmpty(response.ErrorMessage) == false)
                {
                    DisplayerResponseMessage(Hiker.GUI.MessagePopupType.TEXT, response.ErrorMessage);
                }
                return false;
            case ERROR_CODE.INVALIDATE_ACCESS_TOKEN:
                Debug.Log("Invalid Access Token");
                Hiker.GUI.PopupConfirm.Create(Localization.Get("INVALID_ACCESS_TOKEN"),
                    () =>
                    {
                        RestartGame();
                    },
                    false,
                    Localization.Get("BtnOk"));
                //Hiker.GUI.PopupMessage.Create(Hiker.GUI.MessagePopupType.ERROR,
                //    Localization.Get("INVALID_ACCESS_TOKEN"));
                return false;
            case ERROR_CODE.GAME_VERSION_INVALID:
                ShowGameVersionUpdadePopup(response.ErrorMessage);
                return false;
            case ERROR_CODE.INVALID_INSTALL:
                ShowInvalidInstallPopup(response.ErrorMessage);
                return false;
            case ERROR_CODE.CONFIG_VERSION_INVALID:
                ShowConfigVersionUpdatePopup();
                return false;
            case ERROR_CODE.DISPLAY_MESSAGE:
                if (string.IsNullOrEmpty(response.ErrorMessage) == false)
                {
                    DisplayerResponseMessage(Hiker.GUI.MessagePopupType.TEXT, response.ErrorMessage);
                }
                return false;
            case ERROR_CODE.DISPLAY_MESSAGE_AND_QUIT:
                ShowPopupMessageAndQuit(response.ErrorMessage);
                return false;
            case ERROR_CODE.UNKNOW_ERROR:
                ProcessErrorResponse(response);
                return false;
            default:
                return true;
        }
    }

    void ProcessErrorResponse(ExDataBase response)
    {
        if (response == null)
        {
            DisplayerResponseMessage(Hiker.GUI.MessagePopupType.ERROR, "Response is null");
        }
        if (response.ErrorCode != ERROR_CODE.OK)
        {
            if (string.IsNullOrEmpty(response.ErrorMessage) == false)
            {
                DisplayerResponseMessage(Hiker.GUI.MessagePopupType.ERROR, response.ErrorMessage);
            }
            else
            {
                DisplayerResponseMessage(Hiker.GUI.MessagePopupType.ERROR,
                    Localization.Get(response.ErrorCode.ToString()));
            }
        }
    }

    public string CurBattleID { get; set; }

    public void LoginDevice(string deviceID)
    {
        Login(LoginType.DEVICE, string.Empty, deviceID, null);
    }
    public void LoginApple(string appleID, string deviceID, string[] datas)
    {
        Login(LoginType.APPLE, appleID, deviceID, datas);
    }
    public void LoginGoogle(string googleID, string deviceID, string[] datas)
    {
        Login(LoginType.GOOGLE, googleID, deviceID, datas);
    }

    void Login(LoginType loginType, string userName, string deviceID, string[] datas)
    {
        LoginRequest request = new LoginRequest();
        request.UpdateGIDData();
        request.GID = 0;
        request.token = string.Empty;

        if (loginType == LoginType.APPLE)
        {
            request.AppleID = userName;
        }
        else if (loginType == LoginType.GOOGLE)
        {
            request.GoogleID = userName;
        }
        request.DeviceID = deviceID;
        request.Datas = datas;
        request.DeviceModel = SystemInfo.deviceModel;
        request.OperatingSystem = SystemInfo.operatingSystem;

        //LastLoginType = login_type;
        PlayerPrefs.SetInt("LoginType2", (int)loginType);
        PlayerPrefs.SetString("LoginDatas2", LitJson.JsonMapper.ToJson(request));
        SendRequest("RequestLogin",
            LitJson.JsonMapper.ToJson(request),
            (data) =>
            {
//#if DEBUG
//                Debug.Log(data.ToString());
//#endif
                LoginResponse response = LitJson.JsonMapper.ToObject<LoginResponse>(data);
                if (ValidateResponse(response))
                {
                    if (response.ErrorCode == ERROR_CODE.OK)
                    {
                        PlayerPrefs.SetInt("FirstTimeShowAdsThisSession", 0);
                        PlayerPrefs.SetInt("FirstTimeShowAdsThisSessionStartBattle", 0);

                        UInfo.LuotLeoThap = response.UInfo.LuotLeoThap;
                        UInfo.LeoThapRankIdx = response.UInfo.LeoThapRankIdx;
                        UInfo.BattleLeoThap = response.UInfo.BattleLeoThap;

                        UInfo.LuotSanThe = response.UInfo.LuotSanThe;
                        UInfo.BattleSanThe = response.UInfo.BattleSanThe;

                        UInfo.LuotThachDau = response.UInfo.LuotThachDau;
                        UInfo.BattleThachDau = response.UInfo.BattleThachDau;

                        UpdateUserInfo(response.UInfo);
                        UInfo.GID = response.UInfo.GID;


                        PlayerPrefs.SetString("DeviceID", request.DeviceID);
                        GID = UInfo.GID;
                        AccessToken = response.Token;
                        this.CurBattleID = response.BattleID;
#if IAP_BUILD
                        UnityIAPManager.CreateInstance();
#endif
                        Hiker.GUI.GUIManager.Instance.SetScreen("Main");
#if FIRE_BASE
                        Firebase.Crashlytics.Crashlytics.SetUserId("GID" + GID);
#endif
                        if (PushNotificationManager.Instance)
                        {
                            PushNotificationManager.Instance.CheckNotificationComeBack();
                        }
#if SERVER_1_3
                        string key = "LastTimeUpdateInfo_" + GID.ToString();
                        if (PlayerPrefs.HasKey(key))
                        {
                            System.DateTime lastTimeGetInfo;
                            if (System.DateTime.TryParse(PlayerPrefs.GetString(key),
                                System.Globalization.CultureInfo.InvariantCulture,
                                System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal,
                                out lastTimeGetInfo))
                            {
                                var ts = (TimeUtils.Now - lastTimeGetInfo);
                                if (ts.TotalDays > 1 && 
                                    UInfo.Gamer.ComeBack == 0 &&
                                    UInfo.Gamer.RegisterTime.AddDays(3) > GameClient.instance.ServerTime)
                                {
                                    GameClient.instance.RequestGetComeBackMail();
                                }
                            }
                        }

                        PlayerPrefs.SetString(key,
                            TimeUtils.Now.ToString(System.Globalization.CultureInfo.InvariantCulture));
#endif
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
            },
            true,
            ignoreError: false,
            custom_timeOut: 20);
    }

    public void LinkAccount(LoginType loginType, string socialID, string deviceID, string[] datas)
    {
        LoginRequest request = new LoginRequest();
        request.UpdateGIDData();
        if (loginType == LoginType.APPLE)
        {
            request.AppleID = socialID;
        }
        else if (loginType == LoginType.GOOGLE)
        {
            request.GoogleID = socialID;
        }
        else
        {
            return;
        }
        request.DeviceID = deviceID;
        request.Datas = datas;

        SendRequest("RequestLinkAccount",
            LitJson.JsonMapper.ToJson(request),
            (data) =>
            {
                LoginResponse response = LitJson.JsonMapper.ToObject<LoginResponse>(data);
                if (ValidateResponse(response))
                {
                    if (response.ErrorCode == ERROR_CODE.OK)
                    {
                        if (response.UInfo != null && response.UInfo.GID == GID)
                        {
                            UpdateUserInfo(response.UInfo);
                            UInfo.GID = response.UInfo.GID;
                            PlayerPrefs.SetString("DeviceID", request.DeviceID);
                            PlayerPrefs.SetInt("LoginType2", (int)loginType);
                            PlayerPrefs.SetString("LoginDatas2", LitJson.JsonMapper.ToJson(request));
                        }
                        else
                        {
                            if (response.UInfo != null && response.UInfo.GID > 0)
                            {
                                Hiker.GUI.PopupConfirm.Create(string.Format(Localization.Get("LinkAccountNoneName"), response.UInfo.GetCurrentChapter()), () =>
                                {
                                    PlayerPrefs.SetInt("LoginType2", (int)loginType);
                                    PlayerPrefs.SetString("LoginDatas2", LitJson.JsonMapper.ToJson(request));
                                    GameClient.instance.RestartGame();
                                }, () =>
                                {
                                    SocialManager.Instance.Logout();
                                },
                                Localization.Get("RestartGameLabel"));
                            }
                            else
                            {
                                string mess = Localization.Get("LinkAccountGoogleFailed");
#if UNITY_ANDROID
                                mess = Localization.Get("LinkAccountGoogleFailed");
#elif UNITY_IOS
                                mess = Localization.Get("LinkAccountAppleFailed");
#endif
                                Hiker.GUI.PopupConfirm.Create(mess, () =>
                                {
                                    PlayerPrefs.SetInt("LoginType2", (int)loginType);
                                    PlayerPrefs.SetString("LoginDatas2", LitJson.JsonMapper.ToJson(request));
                                    GameClient.instance.RestartGame();
                                }, () =>
                                {
                                    SocialManager.Instance.Logout();
                                },
                                Localization.Get("RestartGameLabel"));
                            }
                        }
                    }
                    else
                    {
                        ProcessErrorResponse(response);
                    }
                }
            },
            true);
    }
//    public void Login(LoginType login_type, params string[] datas)
//    {
//        var is_first_login = PlayerPrefs.GetInt("FIRST_LOGIN_SUCCESS", 0) == 0;
//        if (is_first_login)
//        {
//            AnalyticsManager.LogEvent("FIRST_LOGIN_TO_SERVER");
//            PlayerPrefs.SetInt("FIRST_LOGIN_SUCCESS", 1);
//        }

//        LastLoginType = login_type;
//        PlayerPrefs.SetInt("LoginType", (int)login_type);
//        PlayerPrefs.SetString("LoginDatas", JsonMapper.ToJson(datas));
//#if DEBUG
//        Debug.Log("Login with data : " + datas.ToString());
//#endif
//        if (this.IsRestartGame)
//        {
//            Debug.Log("RESTART!!!");
//        }

//        this.IsNewGamer = false;

//        GameClient.instance.GID = 0;
//        var request = new LoginRequest();
//        request.UpdateGIDData();
//        request.Type = login_type;
//        request.Datas = datas;
//        request.server = GameClient.instance.CurrentServer;//server;

//        //request.languageIdx = ConfigManager.languageIdx;
//        if (login_type == LoginType.NORMAL)
//        {
//            var username = datas[0];
//            var password = datas[1];

//            PlayerPrefs.SetString("UserAccount", username);
//            PlayerPrefs.SetString("UserPassword", password);
//        }

//        bool register = datas.Length > 2 && datas[2] == "register";
//        this.user = datas[0];

//        string cachedGIDStr = PlayerPrefs.GetString("user_" + this.user);
//        if (!register && !string.IsNullOrEmpty(cachedGIDStr))
//        {
//            long gid = long.Parse(cachedGIDStr);
//            this.gameData = GetCachedGameData(gid);
//            if (this.gameData != null)
//            {
//                request.cachedMd5s = GetCachedDataByProp(GameRequests.START_GAME_PROPS);
//            }
//        }
//        else
//        {
//            this.gameData = null;
//        }

//        request.Version = GameClient.GameVersion;
//#if DEBUG
//        Debug.Log("RequestLogin : Json : " + JsonMapper.ToJson(request));
//#endif
//        this.SendRequest(GameRequests.RequestLogin, JsonMapper.ToJson(request),
//            (data) =>
//            {
//                OnFirstLogonSuccess(data);
//                if (is_first_login)
//                {
//                    AnalyticsManager.LogEvent("SUCCESS_FIRST_LOGIN_TO_SERVER");
//                }
//            },
//            (data) =>
//            {
//                if (is_first_login)
//                {
//                    AnalyticsManager.LogEvent("FAIL_FIRST_LOGIN_TO_SERVER");
//                }
//                return false;
//            }, true);
//        this.firstTimeLogin = true;

//        if (TownBuildingManager.Instance)
//        {
//            TownBuildingManager.Instance.IsFirstTimeSyncData = true;
//        }

//        if (SocialManager.Instance)
//        {
//            SocialManager.Instance.CancelCheckTimeOut();
//        }
//    }

//    public void OnFirstLogonSuccess(string data)
//    {
//#if DEBUG
//        Debug.Log("OnFirstLogonSuccess" + data);
//#endif
//        GameClient.instance.CurrentServer = 0;

//#if CHINA && UNITY_ANDROID
//        if (TeeBikSDKManager.instance)
//        {
//            TeeBikSDKManager.instance.RequestSendY2Data("tag 80");
//        }
//#endif

//        var response = JsonMapper.ToObject<LoginResponse>(data);
//        GameClient.instance.accessToken = response.AccessToken;
//        GameClient.instance.GID = response.GID;


//        //#if CHINA && !UNITY_EDITOR && UNITY_IOS
//        //        if(TeeBikSDKManager.instance != null)
//        //            TeeBikSDKManager.instance.SetRoleID(response.GID);
//        //#endif



//        //luu lai gid cuar user nay de preload data
//        PlayerPrefs.SetString("user_" + this.user, response.GID.ToString());

//        if (response.gameData != null)
//        {
//            this.IsNewGamer = response.newGamer;
//            if (IsNewGamer)
//            {
//                AnalyticsManager.LogEvent("CREATE_ACCOUNT_SUCCESS");
//                AnalyticsManager.SetProperty("DateOfBirth", string.Format("{0}{1:00}{2:00}", GameData.TimeNow.Year, GameData.TimeNow.Month, GameData.TimeNow.Day));
//            }

//            if (response.gameData.user != null &&
//                response.gameData.user.TutorialGroupIndex == 0 &&
//                response.gameData.user.TutorialActionIndex == 0)
//            {
//                if (TutorialManager.Instance != null)
//                {
//                    TutorialManager.Instance.TutorialCompleted = false;
//                    TutorialManager.Instance.GroupIndex = 0;
//                    TutorialManager.Instance.ActionIndex = 0;
//                    TutorialManager.Instance.Reset();
//                    TutorialManager.Instance.StopAllCoroutines();
//                }
//            }

//            if (this.gameData != null && this.gameData.user.ID != response.GID)
//            {
//                //game data get ve của GID khác với đang cache
//                this.gameData = null;
//            }

//            StartCoroutine(WaitLoadconfig(response.gameData));
//        }
//    }
}
