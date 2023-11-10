using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker.GUI
{
    using UnityEngine.UI;

    public class PopupToS : PopupBase
    {
        public GameObject grpTos;
        public GameObject grpDetail;

        public static PopupToS instance;

        public static void Create()
        {
            if (instance == null)
            {
                GameObject gObj = PopupManager.instance.GetPopup("PopupToS");
                instance = gObj.GetComponent<PopupToS>();
            }
            instance.grpTos.SetActive(true);
            instance.grpDetail.SetActive(false);
        }

        public static int GetTOSPref()
        {
            return PlayerPrefs.GetInt("TOS", 0);
        }

        [GUIDelegate]
        public override void OnCloseBtnClick()
        {
            PlayerPrefs.SetInt("TOS", 1);

            AnalyticsManager.LogEvent("AGREE_TOS");
#if UNITY_IOS
            if (PopupAppTracking.NeedRequirePermisionTracking())
            {
                var haveATT = PopupAppTracking.GetAppTrackingTransparencySetting();
            
                //PopupMessage.Create(MessagePopupType.TEXT, "ATT " + haveATT);
                
                if (haveATT == 0)
                {
                    PopupAppTracking.Create();
                }
                else
                {
                    GameClient.instance.LoginToServer();
                }
            }
            else
#endif
            {
                GameClient.instance.LoginToServer();
                //if (Hiker.GUI.Shootero.ScreenLoading.Instance && Hiker.GUI.Shootero.ScreenLoading.Instance.gameObject.activeSelf)
                //{
                //    Hiker.GUI.Shootero.ScreenLoading.Instance.LoginToServer();
                //}
            }

            base.OnCloseBtnClick();
        }

        [GUIDelegate]
        public override void OnBackBtnClick()
        {
            if (grpDetail.gameObject.activeSelf)
            {
                grpTos.SetActive(true);
                grpDetail.SetActive(false);
            }
            else
            {
                base.OnBackBtnClick();
            }
        }

        [GUIDelegate]
        public void OnDetailClick()
        {
            //Dismiss();
            AnalyticsManager.LogEvent("DETAIL_TOS");
            //PopupTermOfServicesReadMore.Create();
            grpTos.SetActive(false);
            grpDetail.SetActive(true);
        }

        [GUIDelegate]
        public void OnTOSClick()
        {
            Application.OpenURL(Localization.Get("TOS_url_tos"));
        }

        [GUIDelegate]
        public void OnPolicyClick()
        {
            Application.OpenURL(Localization.Get("TOS_url_policy"));
        }
    }
}