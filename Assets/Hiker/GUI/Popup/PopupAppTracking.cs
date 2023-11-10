using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif
namespace Hiker.GUI
{
    using UnityEngine.UI;

    public class PopupAppTracking : PopupBase
    {
        public GameObject grpTos;

        public static PopupAppTracking instance;

        public static void Create()
        {
            if (instance == null)
            {
                GameObject gObj = PopupManager.instance.GetPopup("PopupAppTracking");
                instance = gObj.GetComponent<PopupAppTracking>();
            }
            instance.grpTos.SetActive(true);
            //instance.grpDetail.SetActive(false);
        }

        static int checkStatus = -1;
        static bool needRequirePersmision = false;
        public static bool NeedRequirePermisionTracking()
        {
#if UNITY_IOS && !UNITY_EDITOR
            if (checkStatus < 0)
            {
                var iosversion = UnityEngine.iOS.Device.systemVersion;
                
                Debug.Log("ios version " + iosversion);

                var ver = iosversion.Split('.');

                if (ver.Length > 0 && string.IsNullOrEmpty(ver[0]) == false)
                {
                    var intVer = int.Parse(ver[0]);
                    if (intVer > 14)
                    {
                        needRequirePersmision = true;
                    }
                    else if (intVer == 14)
                    {
                        if (ver.Length > 1 && string.IsNullOrEmpty(ver[1]) == false && int.Parse(ver[1]) > 4)
                        {
                            needRequirePersmision = true;
                        }
                    }
                }
                else
                {
                    needRequirePersmision = false;
                }

                checkStatus = 0;
                return needRequirePersmision;
            }

            return needRequirePersmision;
#else
            return false;
#endif
        }

        public static int GetAppTrackingTransparencySetting()
        {
#if UNITY_IOS
            if (NeedRequirePermisionTracking())
            {
                return (int)ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            }
            else
            {
                return 0;
            }
#else
            return 0;
#endif
        }

        public static void RequestAuthorizeTracking()
        {
#if UNITY_IOS
            if (NeedRequirePermisionTracking())
            {
                Debug.Log("RequestAuthorizeTracking");
                ATTrackingStatusBinding.RequestAuthorizationTracking();
            }
#endif
        }

        public static int GetAppTrackingTransparencyPref()
        {
            return PlayerPrefs.GetInt("AppTrackingTransparency", 0);
        }

        [GUIDelegate]
        public override void OnCloseBtnClick()
        {
            if (GetAppTrackingTransparencyPref() == 0)
            {
                AnalyticsManager.LogEvent("AGREE_ATT");
            }
            PlayerPrefs.SetInt("AppTrackingTransparency", 1);
            if (Hiker.GUI.Shootero.ScreenMain.instance && Hiker.GUI.Shootero.ScreenMain.instance.grpSettings)
            {
                Hiker.GUI.Shootero.ScreenMain.instance.grpSettings.CheckAdsExperience();
            }

            //if (IronSourceAdsManager.instance)
            //{
            //    IronSourceAdsManager.instance.SetConsent(true);
            //}

            base.OnCloseBtnClick();

            if (GameClient.instance.GID <= 0)
            {
                GameClient.instance.CheckATTThenLogin();
            }
        }

        [GUIDelegate]
        public override void OnBackBtnClick()
        {
        }

        [GUIDelegate]
        public void OnDetailClick()
        {
            //Dismiss();
            if (GetAppTrackingTransparencyPref() == 0)
            {
                AnalyticsManager.LogEvent("NO_ATT");
            }
            NoATT();
            if (Hiker.GUI.Shootero.ScreenMain.instance && Hiker.GUI.Shootero.ScreenMain.instance.grpSettings)
            {
                Hiker.GUI.Shootero.ScreenMain.instance.grpSettings.CheckAdsExperience();
            }
            //PopupTermOfServicesReadMore.Create();
            //grpTos.SetActive(false);

            if (GameClient.instance.GID <= 0)
            {
                GameClient.instance.CheckATTThenLogin();
            }
            base.OnCloseBtnClick();
        }

        public static void NoATT()
        {
            PlayerPrefs.SetInt("AppTrackingTransparency", -1);
            if (IronSourceAdsManager.instance)
            {
                IronSourceAdsManager.instance.SetConsent(false);
            }
        }
    }
}