using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hiker.Networks.Data.Shootero;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;

    public class PopupDailyShop : PopupBase
    {
        public Transform itemParent;
        public DailyShopItem itemPrefabs;
        public Button btnRefreshAds;
        public Text lbRefreshAds;
        public Text lbTime;
        public Text lbCandy;
        public Transform grpCandy;
        public Text lbEventTime;
        public GameObject grpStandard;
        public GameObject grpHalloween;
        public GameObject bgStandard;
        public GameObject bgHalloween;

        [HideInInspector]
        public int HaveCandy = 0;
        [HideInInspector]
        public bool IsInHalloweenEvent = false;

        bool preventDoubleClick;

        public static PopupDailyShop instance;
        List<DailyShopItem> mListItems = new List<DailyShopItem>();

        public static PopupDailyShop Create()
        {
            UserInfo uInfo = GameClient.instance.UInfo;
            if (uInfo == null || uInfo.dailyShopData == null) return null;
            if(uInfo.dailyShopData.ResetTime <= GameClient.instance.ServerTime)
            {
                GameClient.instance.RequestGetDailyShop();
                return null;
            }

            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupDailyShop");
            instance = go.GetComponent<PopupDailyShop>();
            instance.Init();
            return instance;
        }

        public void Init()
        {
            UserInfo uInfo = GameClient.instance.UInfo;
            if (uInfo == null || uInfo.dailyShopData == null) return;

            if(uInfo.halloweenServerData != null && uInfo.halloweenServerData.StartTime <= GameClient.instance.ServerTime && GameClient.instance.ServerTime <= uInfo.halloweenServerData.EndTime)
            {
                IsInHalloweenEvent = true;
                var matData = GameClient.instance.UInfo.ListMaterials.Find(e => e.Name == "M_HalloweenCandy");
                HaveCandy = matData != null ? matData.Num : 0;

                lbCandy.text = HaveCandy + "";
                LayoutRebuilder.ForceRebuildLayoutImmediate(grpCandy as RectTransform);
            }
            else
            {
                IsInHalloweenEvent = false;
            }

            grpHalloween.SetActive(IsInHalloweenEvent);
            bgHalloween.SetActive(IsInHalloweenEvent);
            grpStandard.SetActive(!IsInHalloweenEvent);
            bgStandard.SetActive(!IsInHalloweenEvent);

            PlayerPrefs.SetString(uInfo.GID + "dailyShopResetTime", uInfo.dailyShopData.ResetTime.ToString());
            if (ScreenMain.instance != null && ScreenMain.instance.grpChapter.dailyShopAlert != null) ScreenMain.instance.grpChapter.dailyShopAlert.SetActive(false);
             isRefreshing = false;

            int c = 0;
            foreach (var it in uInfo.dailyShopData.ListItems)
            {
                if (it == null) continue;
                DailyShopItem item;
                if (c < mListItems.Count)
                    item = mListItems[c];
                else
                {
                    item = Instantiate(itemPrefabs, itemParent);
                    mListItems.Add(item);
                }

                item.SetItem(it, c, uInfo.dailyShopData.ResetTime, IsInHalloweenEvent);
                item.gameObject.SetActive(true);
                c++;
            }

            if (c < mListItems.Count)
            {
                for (int i = c; i < mListItems.Count; i++)
                {
                    mListItems[i].gameObject.SetActive(false);
                }
            }

            //if (ResourceBar.instance)
            //{
            //    ResourceBar.instance.gameObject.SetActive(true);
            //}

            string text = Localization.Get("Btn_Refresh") + " ({0}/{1})";
            int watchAdsRemain = ConfigManager.DailyShopCfg.MaxWatchAds - uInfo.dailyShopData.WatchAdsCount;
            lbRefreshAds.text = string.Format(text, watchAdsRemain, ConfigManager.DailyShopCfg.MaxWatchAds);
            btnRefreshAds.interactable = (watchAdsRemain > 0);

        }

        [GUIDelegate]
        public void OnBtnWatchAdsClick()
        {
            if (preventDoubleClick) return;
            AnalyticsManager.LogEvent("WATCH_ADS_REFRESHDAILYSHOP");

            if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
            {
                HikerAdsManager.instance.ShowVideoAds(this.RequestReceiveReward, this.RequestCancelVideo,
                                                      this.RequestFailVideo, this.RequestCancelVideo);
                StartCoroutine(PreventDoubleclick(5));

            }
            else
            {
                AnalyticsManager.LogEvent("NO_ADS_AVAILABLE");
                GameClient.instance.RequestRefreshDailyShopByAds();
            }
        }

        private void RequestReceiveReward()
        {
            AnalyticsManager.LogEvent("FINISH_ADS_REFRESHDAILYSHOP");
            GameClient.instance.RequestRefreshDailyShopByAds();
            //Dismiss();
        }

        private void RequestCancelVideo()
        {
            PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("watch_ads_cancel"));
            //Dismiss();
        }

        private void RequestFailVideo()
        {
            PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("watch_ads_fail"));
            //Dismiss();
        }

        private IEnumerator PreventDoubleclick(float time)
        {
            preventDoubleClick = true;
            yield return new WaitForSecondsRealtime(time);
            preventDoubleClick = false;
        }

        bool isRefreshing = false;
        void UpdateTimeLeft()
        {
            if (GameClient.instance.UInfo.dailyShopData.ResetTime > GameClient.instance.ServerTime)
            {
                var ts = GameClient.instance.UInfo.dailyShopData.ResetTime - GameClient.instance.ServerTime;
                lbTime.text = Localization.Get("lbRefreshAfter") + string.Format(Localization.Get("TimeCountDownHMS"),
                    Mathf.FloorToInt((float)ts.TotalHours), ts.Minutes, ts.Seconds);
            }
            else if (isRefreshing == false)
            {
                isRefreshing = true;
                GameClient.instance.RequestGetDailyShop();
            }

            if (IsInHalloweenEvent)
            {
                string eventTimeStr = "";
                if (GameClient.instance.ServerTime < GameClient.instance.UInfo.halloweenServerData.StopDropTime)
                {
                    var tsDropTime = GameClient.instance.UInfo.halloweenServerData.StopDropTime - GameClient.instance.ServerTime;
                    eventTimeStr += Localization.Get("halloween_event_end_after") + string.Format(Localization.Get("TimeCountDownHMS"),
                    Mathf.FloorToInt((float)tsDropTime.TotalHours), tsDropTime.Minutes, tsDropTime.Seconds);
                }
                else
                {
                    eventTimeStr += Localization.Get("halloween_event_ended");
                }
                eventTimeStr += "\n";
                var tsEndTime = GameClient.instance.UInfo.halloweenServerData.EndTime - GameClient.instance.ServerTime;
                eventTimeStr += Localization.Get("halloween_event_shop_close_after") + string.Format(Localization.Get("TimeCountDownHMS"),
                    Mathf.CeilToInt((float)tsEndTime.TotalHours), tsEndTime.Minutes, tsEndTime.Seconds);
                lbEventTime.text = eventTimeStr;
            }
        }

        private void Update()
        {
            UpdateTimeLeft();
            //if (ResourceBar.instance && ResourceBar.instance.gameObject.activeSelf == false)
            //{
            //    ResourceBar.instance.gameObject.SetActive(true);
            //}
        }

    }

}

