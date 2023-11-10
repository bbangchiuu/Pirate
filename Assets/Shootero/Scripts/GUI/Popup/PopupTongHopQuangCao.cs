using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    public enum ADS_TYPE
    {
        GEM,
        DAILYFREE,
        DAILYGOLD,
        DAILYMAT,
        DAILYITEM,

    }
    public class PopupTongHopQuangCao : PopupBase
    {
        public static PopupTongHopQuangCao instance;
        public List<PopupTongHopQuangCaoItem> listItems;
        public GameObject itemPref;
        public Transform grpItems;

        public static PopupTongHopQuangCao Create()
        {
            UserInfo uInfo = GameClient.instance.UInfo;
            if (uInfo == null || uInfo.Gamer == null || uInfo.Gamer.TruongThanhData == null) return null;

            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupTongHopQuangCao");
            instance = go.GetComponent<PopupTongHopQuangCao>();
            instance.Init();
            return instance;
        }

        public void Init()
        {
            UserInfo uInfo = GameClient.instance.UInfo;
            bool isCompletedAds = true;
            //gem ads
            {
                CardReward adsReward = new CardReward();
                adsReward.AddGem(ConfigManager.GetFreeGemNumber());

                int countAd = 0;
                if (uInfo != null && uInfo.adsData != null && uInfo.adsData.listAdsGemIDs != null)
                {
                    countAd = uInfo.adsData.listAdsGemIDs.Count;
                }

                listItems[0].SetItem(adsReward, ConfigManager.GetFreeGemAdsMaxTime(),
                    ConfigManager.GetFreeGemAdsMaxTime() - countAd);
            }

            //dailyfree ads
            {
                CardReward adsReward = new CardReward();
                adsReward.AddGold(ConfigManager.GetBasedGoldOffer(uInfo.GetCurrentChapter()));
                adsReward.Add("M_Unknow", ConfigManager.GetBasedMaterialOffer(uInfo.GetCurrentChapter()));

                var listDailyOffers = GameClient.instance.GetListDailyOffers();
                OfferStoreData freeDailyOffer = listDailyOffers.Find(e => e.PackageName == "Daily_Offer_Free");
                
                if (freeDailyOffer != null)
                {
                    listItems[1].SetItem(adsReward, freeDailyOffer.StockCount,
                        freeDailyOffer.StockCount - freeDailyOffer.BuyCount);

                    if (freeDailyOffer.StockCount > freeDailyOffer.BuyCount) isCompletedAds = false;
                }
            }

            //dailygold ads
            {
                CardReward adsReward = new CardReward();
                adsReward.AddGold(ConfigManager.GetDailyGoldAdsBaseNumber() * ConfigManager.GetBasedGoldOffer(uInfo.GetCurrentChapter()));

                int countAd = 0;
                if (uInfo != null && uInfo.adsData != null && uInfo.adsData.listAdsDailyGoldIDs != null)
                {
                    countAd = uInfo.adsData.listAdsDailyGoldIDs.Count;
                }

                listItems[2].SetItem(adsReward, ConfigManager.GetDailyGoldAdsMax(),
                    ConfigManager.GetDailyGoldAdsMax() - countAd);

                if (ConfigManager.GetDailyGoldAdsMax() > countAd) isCompletedAds = false;
            }

            //dailymat ads
            {
                CardReward adsReward = new CardReward();
                adsReward.Add("M_Unknow", ConfigManager.GetDailyMatAdsBaseNumber() * ConfigManager.GetBasedMaterialOffer(uInfo.GetCurrentChapter()));

                int countAd = 0;
                if (uInfo != null && uInfo.adsData != null && uInfo.adsData.listAdsDailyMatIDs != null)
                {
                    countAd = uInfo.adsData.listAdsDailyMatIDs.Count;
                }

                listItems[3].SetItem(adsReward, ConfigManager.GetDailyMatAdsMax(),
                    ConfigManager.GetDailyMatAdsMax() - countAd);

                if (ConfigManager.GetDailyMatAdsMax() > countAd) isCompletedAds = false;
            }

            //dailyitem ads
            {
                CardReward adsReward = new CardReward();
                adsReward.Add(ConfigManager.GetDailyItemAdsName(), 1);

                int countAd = 0;
                if (uInfo != null && uInfo.adsData != null && uInfo.adsData.listAdsDailyItemIDs != null)
                {
                    countAd = uInfo.adsData.listAdsDailyItemIDs.Count;
                }

                listItems[4].SetItem(adsReward, ConfigManager.GetDailyItemAdsMax(),
                    ConfigManager.GetDailyItemAdsMax() - countAd,
                    ConfigManager.GetDailyItemAdsMax(),
                    countAd);

                if (ConfigManager.GetDailyItemAdsMax() > countAd) isCompletedAds = false;
            }

            if (isCompletedAds)
            {
                string saveKey = GameClient.instance.GID + "_COMPLETED_ADS";
                PlayerPrefs.SetInt(saveKey, GameClient.instance.ServerTime.DayOfYear);
                if(ScreenMain.instance != null && ScreenMain.instance.grpChapter != null)
                {
                    ScreenMain.instance.grpChapter.twBtnTongHopQuangCao.ResetToBeginning();
                    ScreenMain.instance.grpChapter.twBtnTongHopQuangCao.transform.localRotation = Quaternion.identity;
                    ScreenMain.instance.grpChapter.twBtnTongHopQuangCao.enabled = false;
                }
            }
        }

        bool preventDoubleClick;
        ADS_TYPE mAdsType;
        [GUIDelegate]
        public void WatchAds(int adsTypeIdx)
        {
            if (preventDoubleClick) return;

            AdsData adsData = GameClient.instance.UInfo.adsData;

            mAdsType = (ADS_TYPE)adsTypeIdx;
            AnalyticsManager.LogEvent("WATCH_ADS_" + mAdsType.ToString());
            

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
            AnalyticsManager.LogEvent("FINISH_ADS_" + mAdsType.ToString());
            if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
            {
                Debug.Log("Ad Available");
            }
            if (mAdsType == ADS_TYPE.GEM)
            {
                GameClient.instance.RequestWatchAdsGem();
            }
            else if(mAdsType == ADS_TYPE.DAILYFREE)
            {
                GameClient.instance.RequestBuyOfferBuyGem("Daily_Offer_Free");
            }
            else if (mAdsType == ADS_TYPE.DAILYGOLD)
            {
                GameClient.instance.RequestDailyGoldWatchAds();
            }
            else if (mAdsType == ADS_TYPE.DAILYMAT)
            {
                GameClient.instance.RequestDailyMatWatchAds();
            }
            else if (mAdsType == ADS_TYPE.DAILYITEM)
            {
                GameClient.instance.RequestDailyItemWatchAds();
            }
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

        [GUIDelegate]
        public override void OnCloseBtnClick()
        {
            base.OnCloseBtnClick();
        }
    }
}