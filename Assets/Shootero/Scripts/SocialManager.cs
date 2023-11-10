using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Collections;
using System.Collections.Generic;
#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif
using LitJson;

#if UNITY_ANDROID
using GooglePlayGames;
#endif

public class SocialManager : MonoBehaviour
{
    public static SocialManager Instance { get; set; }
	public bool IsAuthenticated { get { return !string.IsNullOrEmpty(this.SocialID); } }
    /// <summary>
    /// Email ID
    /// </summary>
    public string SocialID { get; set; }
    public string SocialUserID { get; set; }
    public string DeviceID { get; set; }
    public string UserName { get; set; }
    private string RESTART_TOKEN = "HKGAMTSDGAJ_";
	private float TimeOutElapsed = -1f;
    private bool NeedCheckTimeOut = false;
    private bool IsTimeOut { get; set; }

//#if UNITY_IOS
//    [DllImport("__Internal")]
//    private static extern void _ReportAchievement(string achievementID, float progress, bool showsCompletionBanner);
//#endif

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        this.DeviceID = SystemInfo.deviceUniqueIdentifier;

#if UNITY_ANDROID
        // use default config
        //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        ////.RequestEmail()
        ////.RequestIdToken()
        ////.RequestServerAuthCode(false)
        //.Build();

#if DEBUG
        //PlayGamesPlatform.DebugLogEnabled = true;
#endif
        //PlayGamesPlatform.InitializeInstance(config);
        //PlayGamesPlatform.Activate();
        //Debug.Log("Create and Active PlayGamesPlatform");
#endif

#if UNITY_IOS
		//neu la ios thi can lay device id cũ tránh lỗi
		this.DeviceID = PlayerPrefs.GetString("deviceID", SystemInfo.deviceUniqueIdentifier);
		PlayerPrefs.SetString("deviceID", this.DeviceID);
        GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
#endif
    }

    //int authenticatedGoogle = 0;
    private void Start()
    {
#if UNITY_ANDROID
        PlayGamesPlatform.Activate();
        //Social.localUser.Authenticate(ProcessAuthentication);
#endif
    }
    //#if UNITY_ANDROID
    //    internal void ProcessAuthentication(SignInStatus status)
    //    {
    //        if (status == SignInStatus.Success)
    //        {
    //            // Continue with Play Games Services
    //            authenticatedGoogle = 1;
    //        }
    //        else
    //        {
    //            // Disable your integration with Play Games Services or show a login button
    //            // to ask users to sign-in. Clicking it should call
    //            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
    //        }
    //    }
    //#endif

    private void Update()
    {
        if (this.NeedCheckTimeOut)
        {
            this.TimeOutElapsed += Time.deltaTime;
            if (this.TimeOutElapsed >= 15f)
            {
                //this.IsTimeOut = true;
                this.CancelCheckTimeOut();
                //this.ProcessLoginSocialFail();
				AnalyticsManager.LogEvent("LOGIN_SOCIAL_FAIL_BY_TIMEOUT");
            }
        }
        else
        {
            this.TimeOutElapsed = 0f;
        }
    }

    public void CancelCheckTimeOut()
    {
        this.NeedCheckTimeOut = false;
        this.TimeOutElapsed = 0f;
    }

    public void Logout()
    {
        SocialID = string.Empty;
//#if UNITY_ANDROID
//        PlayGamesPlatform.Instance.SignOut();
//#endif
    }

#if UNITY_ANDROID
     void LoginGooglePlayGames()
    {
        PlayGamesPlatform.Instance.Authenticate((signInStatus) =>
        {
            if (this.IsTimeOut == false)
            {
                if (Hiker.GUI.PopupLoginSocialFail.Instance && Hiker.GUI.PopupLoginSocialFail.Instance.gameObject.activeSelf)
                {
                    Hiker.GUI.PopupLoginSocialFail.Dismiss();
                }

                if (signInStatus == GooglePlayGames.BasicApi.SignInStatus.Success)
                {
                    Debug.Log("Authenticate Google Play success");

                    var google_user = PlayGamesPlatform.Instance.localUser as PlayGamesLocalUser;
                    var google_id = google_user.id;
                    //var email = google_user.Email;
                    //var id_token = google_user.GetIdToken();
                    var id_token = string.Empty;
                    var g_userName = google_user.userName;

                    //if (GameClient.instance.IsRestartGame)
                    //{
                    //    id_token += this.RESTART_TOKEN;
                    //}
#if DEBUG
                    Debug.Log("Try to Login with google user name: " + g_userName);
                    Debug.Log("Try to Login with googleID: " + google_id);
                    //Debug.Log("Try to Login with email: " + email);
                    //Debug.Log("Try to Login with id_token: " + id_token);
                    Debug.Log("Try to Login with DeviceID: " + this.DeviceID);
#endif
                    this.UserName = g_userName;

                    if (Hiker.GUI.Shootero.ScreenLoading.Instance.lblTip != null)
                    {
                        Hiker.GUI.Shootero.ScreenLoading.Instance.lblTip.text = Localization.Get("login_to_server");
                    }

                    //GameClient.instance.Login(LoginType.GOOGLE,
                    //    google_id,
                    //    PushNotificationManager.Instance.DeviceToken,
                    //    g_userName, // as email in previous version
                    //    id_token,
                    //    this.DeviceID);
                    GameClient.instance.LoginGoogle(google_id, this.DeviceID, new string[] {
                            g_userName,
                            PushNotificationManager.Instance.DeviceToken
                        });
                    this.SocialID = google_id;
                    this.SocialUserID = google_id;
                    if (string.IsNullOrEmpty(this.SocialID))
                    {
                        this.SocialID = Localization.Get("GameCenterConnected");
                    }
                    AnalyticsManager.LogEvent("LOGIN_SOCIAL_SUCCESS");
                }
                else
                {
                    Debug.Log("Authenticate To gamecenter/googleplay fail");
                    this.ProcessLoginSocialFail();
                    AnalyticsManager.LogEvent("LOGIN_SOCIAL_FAIL_BY_RESPONSE");
                }
            }
        });
    }
#endif
    
    void LoginGameCenter()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (this.IsTimeOut == false)
            {
                if (Hiker.GUI.PopupLoginSocialFail.Instance && Hiker.GUI.PopupLoginSocialFail.Instance.gameObject.activeSelf)
                {
                    Hiker.GUI.PopupLoginSocialFail.Dismiss();
                }

                if (success)
                {
                    Debug.Log("Authenticate Game Center success");
                    var apple_id = Social.localUser.id;
                    this.SocialUserID = apple_id;
                    var id_token = string.Empty;
                    var userName = Social.localUser.userName;
                    //GameClient.instance.Login(LoginType.APPLE,
                    //    apple_id,
                    //    PushNotificationManager.Instance.DeviceToken,
                    //    string.Format("{0}@icloud.com", apple_id), // as email in previous version
                    //    id_token,
                    //    this.DeviceID);
                    GameClient.instance.LoginApple(apple_id, this.DeviceID, new string[]
                    {
                        userName,
                        PushNotificationManager.Instance.DeviceToken
                    });
                    this.UserName = userName;
                    this.SocialID = apple_id;
                    if (string.IsNullOrEmpty(SocialID))
                        this.SocialID = Localization.Get("GameCenterConnected");
                    AnalyticsManager.LogEvent("LOGIN_SOCIAL_SUCCESS");
                }
                else
                {
                    Debug.Log("Authenticate To gamecenter/googleplay fail");
                    this.ProcessLoginSocialFail();
                    AnalyticsManager.LogEvent("LOGIN_SOCIAL_FAIL_BY_RESPONSE");
                }
            }
        });
    }

    public void DoLogin()
    {
        if (GameClient.instance.IsDisconnected)
        {
            Hiker.GUI.PopupConfirm.Create(
                Localization.Get("NetworkDisconnected"),
                () => this.DoLogin(),
                false,
                Localization.Get("TryAgain"));
            return;
        }

        LoginType defaultLoginType = LoginType.DEVICE;

//#if UNITY_ANDROID
//        defaultLoginType = LoginType.GOOGLE;
//#else
//        defaultLoginType = LoginType.APPLE;
//#endif

        var last_login_type = (LoginType)PlayerPrefs.GetInt("LoginType2", (int)defaultLoginType);
        if (last_login_type == LoginType.DEVICE)
        {
            System.Action loginNow = () =>
            {
                var login_datas_json = PlayerPrefs.GetString("LoginDatas2", string.Empty);

                if (string.IsNullOrEmpty(login_datas_json))
                {
                    //GameClient.instance.Login(LoginType.DEVICE, this.DeviceID, PushNotificationManager.Instance.DeviceToken);
                    GameClient.instance.LoginDevice(this.DeviceID);
                }
                else
                {
                    var login_datas = JsonMapper.ToObject<Hiker.Networks.Data.Shootero.LoginRequest>(login_datas_json);
                    var device_id = login_datas.DeviceID; //login_datas[0];
                    //var device_token = login_datas[1];
                    //GameClient.instance.Login(LoginType.DEVICE, device_id, device_token);
                    GameClient.instance.LoginDevice(device_id);
                }
            };

            //if (RemoteConfigManager.Instance.GetBoolConfigValue("DEBUG_LOGIN_DEVICE"))
            //{
            //    string cusDeviceID = PlayerPrefs.GetString("CUS_DEVICE_ID", string.Empty);
            //    if (string.IsNullOrEmpty(cusDeviceID) == false)
            //    {
            //        this.DeviceID = cusDeviceID;
            //    }

            //    PopupConfirmInputText.Create("Device ID", this.DeviceID, (input) =>
            //    {
            //        if (this.DeviceID != input)
            //        {
            //            this.DeviceID = input;
            //            PlayerPrefs.SetString("CUS_DEVICE_ID", input);
            //        }
            //        PlayerPrefs.SetString("LoginDatas", string.Empty);
            //        loginNow();
            //    }, loginNow);
            //}
            //else
            {
                loginNow();
            }
            return;
        }

        try
        {
//#if UNITY_ANDROID
//            if (!this.CheckGoogleServiceInstalled(true))
//            {
//                return;
//            }
//#endif
            this.NeedCheckTimeOut = true;
            this.IsTimeOut = false;

#if UNITY_ANDROID
            LoginGooglePlayGames();
#elif UNITY_IOS
            LoginGameCenter();
#endif

        }
        catch (System.Exception ex)
        {
            //MessagePopup2.Create(MessagePopupType.ERROR, ex.ToString());
            Hiker.GUI.PopupMessage.Create(Hiker.GUI.MessagePopupType.ERROR, ex.ToString());
        }
    }

    private void ProcessLoginSocialFail()
    {
        if (Hiker.GUI.Shootero.ScreenLoading.Instance == null) return;
        if (Hiker.GUI.Shootero.ScreenLoading.Instance.gameObject.activeSelf == false) return;

        var last_login_datas_json = PlayerPrefs.GetString("LoginDatas2", string.Empty);
        if (string.IsNullOrEmpty(last_login_datas_json))
        {
            var content = Localization.Get("LoginGoogleFailTryAgain");
            var confirm_label = Localization.Get("UpdateGooglePlay");
            System.Action confirm_action = () => {
                Application.OpenURL("http://play.google.com/store/apps/details?id=com.google.android.play.games");
                AnalyticsManager.LogEvent("PLAY_WITH_GAME_CENTER");
            };

#if UNITY_IOS
            content = Localization.Get("LoginAppleFailTryAgain");
            confirm_action = () => Application.Quit();
            confirm_label = Localization.Get("LoginGameCenterInSetting");
#endif

            System.Action cancel_action = () => {
                //GameClient.instance.Login(LoginType.DEVICE, this.DeviceID, PushNotificationManager.Instance.DeviceToken);
                GameClient.instance.LoginDevice(this.DeviceID);
                AnalyticsManager.LogEvent("PLAY_NOW_WITHOUT_GAME_CENTER");
            };

            Hiker.GUI.PopupLoginSocialFail.Create(content, confirm_label, confirm_action, cancel_action);
        }
        else
        {
            var last_login_type = (LoginType)PlayerPrefs.GetInt("LoginType2", (int)LoginType.DEVICE);
            var last_login_datas = JsonMapper.ToObject<Hiker.Networks.Data.Shootero.LoginRequest>(last_login_datas_json);
            //if (last_login_type == LoginType.DEVICE)
            {
                GameClient.instance.LoginDevice(last_login_datas.DeviceID);
            }
            //else
            //if (last_login_type == LoginType.GOOGLE)
            //{
            //    //var email = last_login_datas.Length > 2 ? last_login_datas[2] : string.Empty;
            //    this.SocialID = last_login_datas.GoogleID;
            //}
            //else if (last_login_type == LoginType.APPLE)
            //{
            //    this.SocialID = last_login_datas.AppleID;//Localization.Get("GameCenterConnected");
            //}
        }
    }

	private bool CheckGoogleServiceInstalled(bool login_start_game)
    {
        var service_version = GetPackageVersionName("com.google.android.play.games");

        if (string.IsNullOrEmpty(service_version) == false && string.Compare(service_version, "2019.") < 0)
        {
            var content = Localization.Get("LoginGoogleFailTryAgain");
            var confirm_label = string.IsNullOrEmpty(service_version) ? Localization.Get("InstallGooglePlay") : Localization.Get("UpdateGooglePlay");
            System.Action confirm_action = () => {
                Application.OpenURL("http://play.google.com/store/apps/details?id=com.google.android.play.games");
                AnalyticsManager.LogEvent("PLAY_WITH_GAME_CENTER");
            };
            System.Action cancel_action = () =>
            {
                //GameClient.instance.Login(LoginType.DEVICE, this.DeviceID, PushNotificationManager.Instance.DeviceToken);
                GameClient.instance.LoginDevice(this.DeviceID);

                AnalyticsManager.LogEvent("PLAY_NOW_WITHOUT_GAME_CENTER");
            };
            Debug.Log("Need to upgrade google play games");
            if (login_start_game)
            {
                Hiker.GUI.PopupLoginSocialFail.Create(content, confirm_label, confirm_action, cancel_action);
            }
            else
            {
                Hiker.GUI.PopupLoginSocialFail.Create(content, confirm_label, confirm_action, () =>
                {
                    //GameClient.instance.CheckChangeDisplayName();
                });
            }

            return false;
        }

        return true;
    }

	public bool GoogleServiceIsInstalled()
	{
		var service_version = GetPackageVersionName("com.google.android.play.games");
		return string.IsNullOrEmpty(service_version) || string.Compare (service_version, "2019.") >= 0;
	}

    public string[] LinkAccountDatas { get; set; }

    public void LinkAccount()
    {
//#if UNITY_EDITOR
//        return;
//#endif
        if (GameClient.instance.IsDisconnected)
        {
            Hiker.GUI.PopupConfirm.Create(
                Localization.Get("NetworkDisconnected"),
                () => this.LinkAccount(),
                false,
                Localization.Get("TryAgain"));
            return;
        }

//#if UNITY_ANDROID
//        if (!this.CheckGoogleServiceInstalled(false)) return;
//#endif
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
#if UNITY_ANDROID
                //if (PlayGamesPlatform.Instance.IsAuthenticated())
                {
                    var google_user = Social.localUser;
                    var google_id = google_user.id;
                    var g_userName = google_user.userName;
                    //var email = google_user.Email;
                    //var id_token = google_user.GetIdToken();
                    var id_token = string.Empty;
                    //this.LinkAccountAction = () => GameClient.instance.Login(LoginType.GOOGLE, google_id, PushNotificationManager.Instance.DeviceToken, email, id_token, this.DeviceID);
                    GameClient.instance.LinkAccount(
                        LoginType.GOOGLE,
                        google_id,
                        this.DeviceID,
                        new string[] { g_userName,
                            PushNotificationManager.Instance.DeviceToken
                    });
                    this.SocialID = google_id;
                    this.UserName = g_userName;
                    this.SocialUserID = google_id;
                    this.LinkAccountDatas = new string[] {
                        google_id,
                        string.Empty,//PushNotificationManager.Instance.DeviceToken,
                        g_userName,
                        id_token,
                        this.DeviceID };
                }
#elif UNITY_IOS
                {
                    var apple_id = Social.localUser.id;
                    this.SocialUserID = apple_id;
                    var id_token = string.Empty;
                    this.UserName = Social.localUser.userName;

                    //GameClient.instance.RequestLinkAccount(LoginType.APPLE,
                    //    apple_id,
                    //    string.Format("{0}@icloud.com", apple_id), // as email
                    //    id_token,
                    //    this.DeviceID);
                    GameClient.instance.LinkAccount(
                        LoginType.APPLE,
                        apple_id,
                        this.DeviceID,
                        new string[] { UserName, PushNotificationManager.Instance.DeviceToken });
                    //this.SocialID = Social.localUser.userName;//Localization.Get("GameCenterConnected");
                    this.SocialID = apple_id;
                    this.SocialUserID = apple_id;
                    this.LinkAccountDatas = new string[] {
                            apple_id,
                            string.Empty,//PushNotificationManager.Instance.DeviceToken,
                            this.UserName,
                            id_token,
                            this.DeviceID };
                }
#endif
            }
            else
            {
                var mess = Localization.Get("LoginGoogleFail");
#if UNITY_IOS
                mess = Localization.Get("LoginAppleFail");
#endif
                //MessagePopup2.Create(MessagePopupType.ERROR, mess);
                Hiker.GUI.PopupMessage.Create(Hiker.GUI.MessagePopupType.ERROR, mess);

                //GameClient.instance.CheckChangeDisplayName();
            }
        });
    }

//    public void SetAchievementProgress(AchievementData achievement_data, double achievement_progress = 100.0f)
//    {
//        if (!Social.localUser.authenticated)
//        {
//            Debug.Log("Not Authenticated");
//            return;
//        }

//        if (PlayerPrefs.GetInt(achievement_data.ID.ToString()) == 0)
//        {
//            PlayerPrefs.SetInt(achievement_data.ID.ToString(), 1);
//        }
//        else
//        {
//            return;
//        }

//        var achievement_id = AchievementsUtils.GetAchievementID(achievement_data.name);
//        if (string.IsNullOrEmpty(achievement_id)) return;

//#if UNITY_ANDROID
//        Social.ReportProgress(achievement_id, achievement_progress, (bool success) =>
//        {
//            this.OnIncrementAchievementCallback(success, achievement_id, (double)achievement_progress);
//        });
////#elif UNITY_IOS
////        _ReportAchievement(achievement_id, (float)achievement_progress, true);
//#endif
//    }

    private void OnIncrementAchievementCallback(bool success, string achievement_id, double current_progress)
    {
        if (success)
        {
        }
        else
        {
        }
    }

    public void ShowAchievement()
    {
        if (!Social.localUser.authenticated)
        {
            Debug.Log("Not Authenticated");
            return;
        }

        Social.ShowAchievementsUI();
    }

    public static string GetPackageVersionName(string packageName)
    {
#if UNITY_ANDROID
        try
        {
            AndroidJavaClass contextCls = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject context = contextCls.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageMngr = context.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaObject packageInfo = packageMngr.Call<AndroidJavaObject>("getPackageInfo", packageName, 0);

            if (packageInfo == null)
            {
                return string.Empty;
            }
            else
            {
                return packageInfo.Get<string>("versionName");
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
            return string.Empty;
        }
#else
        return string.Empty;
#endif
    }

}

public enum LoginType
{
    DEVICE,
    GOOGLE,
    APPLE
}
