using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using System;
    using UnityEngine.UI;
    public class PopupAFKReward : PopupBase
    {
        public static PopupAFKReward instance;
        AFKRewardData rewardData;
        public List<PhanThuongItem> rewardItems;

        public Text GoldPerHourText, MatPerDayText, TimerText;
        public GameObject EmptyTextObj;
        public Button claimButton;

        public GameObject grpAFKExtra;
        public Button btnAFKExtra, btnAFKExtraGem;
        public Text lbNextClaimTime, lbClaimAttempt, lbExtraUseGem;
        public List<PhanThuongItem> extraRewardItems;
        DateTime mNextWatchAdsTime;

        TimeSpan maxAFKTime;
        public static PopupAFKReward Create()
        {
            AFKRewardData _rewardData = GameClient.instance.UInfo.afkRewardData;
            if (_rewardData == null) return null;

            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupAFKReward");
            instance = go.GetComponent<PopupAFKReward>();
            
            instance.Init(_rewardData);
            return instance;
        }

        public void Init(AFKRewardData _rewardData)
        {
            if (_rewardData != null)
            {
                rewardData = _rewardData;
            }
            ShowReward();
            ShowExtraReward();
        }

        void ShowExtraReward()
        {
            UserInfo uInfo = GameClient.instance.UInfo;
            int maxAttempt = ConfigManager.GetExtraAFKMaxTime();
            int remainAttempt = maxAttempt - (uInfo.adsData == null ? maxAttempt : uInfo.adsData.listAdsExtraAFKIDs.Count);
            int gemCost = ConfigManager.GetExtraAFKGemCost();
            lbExtraUseGem.text = gemCost + "";
            LayoutRebuilder.ForceRebuildLayoutImmediate(lbExtraUseGem.transform.parent as RectTransform);

            btnAFKExtraGem.interactable = uInfo.Gamer.Gem >= gemCost && remainAttempt > 0;
            string strNextWatchAdsTime = PlayerPrefs.GetString(uInfo.GID + "ExtraAFKNextWatchAdsTime", "");
            mNextWatchAdsTime = DateTime.Now;
            if(string.IsNullOrEmpty(strNextWatchAdsTime) == false)
            {
                mNextWatchAdsTime = DateTime.Parse(strNextWatchAdsTime);
            }
            btnAFKExtra.interactable = (remainAttempt > 0) && (mNextWatchAdsTime <= DateTime.Now);
            lbNextClaimTime.gameObject.SetActive(remainAttempt > 0 && mNextWatchAdsTime > DateTime.Now);

            string strAttempt = Localization.Get("popup_afk_extra_reward_attempt") + " ";
            if(remainAttempt > 0)
            {
                strAttempt += remainAttempt + "/" + maxAttempt;
            }
            else
            {
                strAttempt += "<color=#FF0000>" + remainAttempt +"</color>" + "/" + maxAttempt;
            }
            lbClaimAttempt.text = strAttempt;

            CardReward extraRewardData = new CardReward();
            int extraMin = ConfigManager.GetExtraAFKMin();
            extraRewardData.AddGold(Mathf.CeilToInt(extraMin * GameClient.instance.UInfo.GetAFKGoldPerMin()));
            extraRewardData.AddReward("M_Unknow", Mathf.CeilToInt(extraMin * GameClient.instance.UInfo.GetAFKMaterialPerMin()));

            int c = 0;
            foreach (string rKey in extraRewardData.Keys)
            {
                int num = extraRewardData[rKey];
                if (num > 0)
                {
                    extraRewardItems[c].SetItem(rKey, num);
                    c++;
                }
            }

            for (int i = 0; i < extraRewardItems.Count; i++)
            {
                extraRewardItems[i].gameObject.SetActive(i < c);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(extraRewardItems[0].transform.parent as RectTransform);
            grpAFKExtra.SetActive(false);
        }

        void UpdateTimer()
        {
            if (rewardData == null) return;

            TimeSpan afkDuration = GameClient.instance.ServerTime - rewardData.lastClaimTime;
            if (afkDuration > maxAFKTime) afkDuration = maxAFKTime;
            TimerText.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.FloorToInt((float)afkDuration.TotalHours), afkDuration.Minutes, afkDuration.Seconds);

            if (lbNextClaimTime.gameObject.activeInHierarchy)
            {
                TimeSpan ts = mNextWatchAdsTime - DateTime.Now;
                lbNextClaimTime.text = Localization.Get("lbNextClaimTime") + string.Format(" {0:00}:{1:00}:{2:00}", Mathf.FloorToInt((float)ts.TotalHours), ts.Minutes, ts.Seconds);
            }
        }

        private void Update()
        {
            UpdateTimer();
        }

        void ShowReward()
        {
            if (rewardData == null)
            {
                Dismiss();
                return;
            }
            int goldPerHour = Mathf.CeilToInt(GameClient.instance.UInfo.GetAFKGoldPerMin() * 60);
            int matPerDay = Mathf.CeilToInt(GameClient.instance.UInfo.GetAFKMaterialPerMin() * 60 * 24);

            maxAFKTime = ConfigManager.GetMaxAFKTime();

            GoldPerHourText.text =  string.Format(Localization.Get("afk_gold_per_hour"), goldPerHour);
            MatPerDayText.text = string.Format(Localization.Get("afk_mat_per_day"), matPerDay);
            int c = 0;
            foreach(string rKey in rewardData.afkRewards.Keys)
            {
                int num = rewardData.afkRewards[rKey];
                if (num > 0)
                {
                    rewardItems[c].SetItem(rKey, rewardData.afkRewards[rKey]);
                    c++;
                }
            }
            bool inActive = c == 0;
            EmptyTextObj.SetActive(inActive);
            claimButton.interactable = !inActive;

            for(int i = 0; i < rewardItems.Count; i++)
            {
                rewardItems[i].gameObject.SetActive(i < c);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(rewardItems[0].transform.parent as RectTransform);
        }

        bool preventDoubleClick;
        public void WatchAds()
        {
            if (preventDoubleClick) return;

            AdsData adsData = GameClient.instance.UInfo.adsData;

            AnalyticsManager.LogEvent("WATCH_ADS_AFK");

            if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
            {
                HikerAdsManager.instance.ShowVideoAds(this.RequestReceiveReward, this.RequestCancelVideo,
                                                      this.RequestFailVideo, this.RequestCancelVideo);
                StartCoroutine(PreventDoubleclick(5));
            }
            else
            {
                StartCoroutine(NoAdsCheck(4));
            }
        }

        private void RequestReceiveReward()
        {
            AnalyticsManager.LogEvent("FINISH_ADS_AFK");
            if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
            {
                Debug.Log("Ad Available");
            }
            OnBtnExtraCloseClick();
            GameClient.instance.RequestExtraAFKWatchAds();
        }

        private void RequestCancelVideo()
        {
            if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
            {
                Debug.Log("Ad Available");
            }
            PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("watch_ads_cancel"));
        }

        private void RequestFailVideo()
        {
            if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
            {
                Debug.Log("Ad Available");
            }
            PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("watch_ads_fail"));
        }

        private IEnumerator PreventDoubleclick(float time)
        {
            preventDoubleClick = true;
            yield return new WaitForSecondsRealtime(time);
            preventDoubleClick = false;
        }

        private IEnumerator NoAdsCheck(float time)
        {
            preventDoubleClick = true;

            if (Hiker.GUI.PopupNetworkLoading.instance != null &&
                    Hiker.GUI.PopupNetworkLoading.instance.gameObject.activeSelf)
            {
                //Debug.Log("requesting...");
            }
            else
            {
                Hiker.GUI.PopupNetworkLoading.Create(Localization.Get("NetworkLoading"));
            }

            yield return new WaitForSecondsRealtime(time);

            Hiker.GUI.PopupNetworkLoading.Dismiss();

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
                preventDoubleClick = false;
            }
        }

        public static void Dismiss()
        {
            if (instance != null)
            {
                instance.OnCloseBtnClick();
            }
        }

        [GUIDelegate]
        public void OnClaimBtnClick()
        {
            if (rewardData != null) GameClient.instance.RequestClaimAFKReward(rewardData);
        }

        [GUIDelegate]
        public void OnBtnExtraAFKClick()
        {
            grpAFKExtra.SetActive(true);
        }

        [GUIDelegate]
        public void OnBtnExtraWatchAdsClick()
        {
            WatchAds();
        }
        [GUIDelegate]
        public void OnBtnExtraUseGemClick()
        {
            OnBtnExtraCloseClick();
            GameClient.instance.RequestExtraAFKWatchAds("Gem-");
        }
        [GUIDelegate]
        public void OnBtnExtraCloseClick()
        {
            grpAFKExtra.SetActive(false);
        }
    }
}