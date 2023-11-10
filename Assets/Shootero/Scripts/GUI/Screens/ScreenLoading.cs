using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;

    public class ScreenLoading : ScreenBase
    {
        public Slider slProgress;
        public Text lblTip;
        public float delayLoadTime = 0.2f;

        public static ScreenLoading Instance;

        public override void OnActive()
        {
            base.OnActive();
            if (ResourceBar.instance)
            {
                ResourceBar.instance.gameObject.SetActive(false);
            }

            StartCoroutine(CoStartLoading());
        }

        public override bool OnBackBtnClick()
        {
            return true;
        }

        IEnumerator CoStartLoading()
        {
            //UnityEditor.EditorApplication.isPaused = true;
            lblTip.text = Localization.Get("loading");
            slProgress.value = 0;
            slProgress.gameObject.SetActive(false);

#if FIRE_BASE
            if (RemoteConfigManager.Instance)
            {
                float timeOut = 10;
#if UNITY_EDITOR
                timeOut = 1f;
#endif
                while (RemoteConfigManager.Instance.isReady == false)
                {
                    timeOut -= Time.unscaledDeltaTime;
                    yield return null;
                    if (timeOut <= 0)
                    {
                        break;
                    }
                }
                var remoteURL = string.Empty;
#if !UNITY_EDITOR
                remoteURL = RemoteConfigManager.Instance.GetStringConfigValue(RemoteConfigManager.HostURL);
#endif
                if (string.IsNullOrEmpty(remoteURL) == false)
                {
                    GameClient.instance.OverrideURL(remoteURL);
                }
            }
#endif

            yield return new WaitForSeconds(delayLoadTime);
            lblTip.text = Localization.Get("loading_game_configs");
            yield return null;
            LoaderBundleManager.instance.LoadBundlesConfig();
            while (ConfigManager.loaded == false)
            {
                yield return null;
            }
            //ConfigManager.LoadConfigs_Client();
            yield return null;

            var haveTos = PopupToS.GetTOSPref();
            var haveATT = 0;
            if (PopupAppTracking.NeedRequirePermisionTracking())
                haveATT = PopupAppTracking.GetAppTrackingTransparencySetting();

            //PopupMessage.Create(MessagePopupType.TEXT, "ATT " + haveATT);

            if (haveTos == 0)
            {
                PopupToS.Create();
            }
            else
            {
#if UNITY_IOS
                if (haveATT == 0 &&
                    PopupAppTracking.NeedRequirePermisionTracking())
                {
                    PopupAppTracking.Create();
                }
                else
#endif
                {
                    GameClient.instance.LoginToServer();
                }
            }

            
        }

        private void Awake()
        {
            Instance = this;
            ConfigManager.InitLanguage();
        }

        private void Update()
        {
            if (ConfigManager.loaded)
            {
                //slProgress.value = 0.1f;
                slProgress.value += Time.deltaTime * 1f;

                //if (slProgress.value >= 1f)
                //{
                //    GUIManager.Instance.SetScreen("Login");
                //}
            }
        }

        public void LoginToServer()
        {
            //Debug.Log("LoginToServer 1");
            if (this.lblTip != null)
                this.lblTip.text = Localization.Get("login_to_server");
            //Debug.Log("LoginToServer 2");
            //GameClient.instance.logging = true;

            if (PlayerPrefs.GetInt("FirstLogin", 0) == 0)
            {
                AnalyticsManager.LogEvent("POP_UP_FIRST_LOGIN");
                PlayerPrefs.SetInt("FirstLogin", 1);
            }

#if LOGIN_GM || UNITY_EDITOR || UNITY_STANDALONE
            bool autoLogin = false;
            if (autoLogin)
            {
                var user = PlayerPrefs.GetString("DeviceID", string.Empty);

#if UNITY_EDITOR || UNITY_STANDALONE
                if (user == "testtest")
                {
                    user = string.Empty;
                    PlayerPrefs.SetString("DeviceID", user);
                }
#endif

                if (string.IsNullOrEmpty(user) == false)
                {
                    //Debug.Log("LoginToServer 5");
                    GameClient.instance.LoginDevice(user);
                }
                else
                {
                    //            GameClient.instance.logging = true;
                    GUIManager.Instance.SetScreen("Login");
                }
            }
            else
            {
                GUIManager.Instance.SetScreen("Login");
            }
#else
    #if UNITY_ANDROID
            if (this.lblTip != null)
                this.lblTip.text = Localization.Get("LoginGoogle");
    #elif UNITY_IOS
            if (this.lblTip != null)
                this.lblTip.text = Localization.Get("LoginIOS");
    #endif
            //Debug.Log("LoginToServer 3");
            SocialManager.Instance.DoLogin();
            Debug.Log("LoginToServer 4");
    #endif
        }
    }
}
