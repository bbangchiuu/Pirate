using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker.GUI
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;

    public class PopupMissingTheLuc : PopupBase
    {
        public Text watchEnergyText, fullRefreshGemText;
        public Button btnWatchAds, btnFullRefresh;

        bool preventDoubleClick;

        public static PopupMissingTheLuc Instance { get; set; }

        public static void Create()
        {
            if (Instance == null)
            {
                GameObject obj = PopupManager.instance.GetPopup("PopupMissingTheLuc", false, Vector3.zero);
                Instance = obj.GetComponent<PopupMissingTheLuc>();
            }
            Instance.watchEnergyText.text = "x" + ConfigManager.GetWatchAdsTheLuc() + "";
            int gemCost = ConfigManager.GetFullRefreshTheLucGemCost();
            Instance.fullRefreshGemText.text = gemCost + "";
            Instance.btnFullRefresh.interactable = (gemCost <= GameClient.instance.UInfo.Gamer.Gem);
        }

        public static void Dismiss()
        {
            if (Instance != null)
            {
                Instance.OnCloseBtnClick();
                Instance = null;
            }
        }

        [GUIDelegate]
        public void OnWatchAdsBtnClick()
        {
            AdsData adsData = GameClient.instance.UInfo.adsData;
            if (adsData != null && adsData.listAdsIDs.Count >= ConfigManager.GetMaxAdsPerDay())
            {
                System.TimeSpan ts = adsData.nextResetTime - GameClient.instance.ServerTime;
                PopupMessage.Create(MessagePopupType.TEXT, string.Format(Localization.Get("reach_limit_ads"), Mathf.FloorToInt((float)ts.TotalHours), Mathf.FloorToInt((float)ts.Minutes)));
                return;
            }

            if (preventDoubleClick) return;
            AnalyticsManager.LogEvent("WATCH_ADS_THELUC");

            if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
            {
                HikerAdsManager.instance.ShowVideoAds(this.RequestReceiveReward, this.RequestCancelVideo,
                                                      this.RequestFailVideo, this.RequestCancelVideo);
                StartCoroutine(PreventDoubleclick(5));

            }
            else
            {
                AnalyticsManager.LogEvent("NO_ADS_AVAILABLE");
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("no_ads_available"));
            }
        }

        private void RequestReceiveReward()
        {
            AnalyticsManager.LogEvent("FINISH_ADS_THELUC");
            GameClient.instance.RequestWatchAdsTheLuc();
            Dismiss();
        }

        private void RequestCancelVideo()
        {
            PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("watch_ads_cancel"));
            Dismiss();
        }

        private void RequestFailVideo()
        {
            PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("watch_ads_fail"));
            Dismiss();
        }

        [GUIDelegate]
        public void OnFullRefeshBtnClick()
        {
            PopupConfirm.Create(Localization.Get("confirm_full_refresh_the_luc"), () =>
            {
                GameClient.instance.RequestFullRefreshTheLuc();
            }, true);
            Dismiss();
        }

        protected override void OnCleanUp()
        {
            if (Instance == this) Instance = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        private IEnumerator PreventDoubleclick(float time)
        {
            preventDoubleClick = true;
            yield return new WaitForSecondsRealtime(time);
            preventDoubleClick = false;
        }
    }
}