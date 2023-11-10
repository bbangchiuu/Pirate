using Hiker.GUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HikerAdsManager : MonoBehaviour
{
    public static HikerAdsManager instance;
    bool requestingVideo = false;
    float requestTime = 0;

    System.DateTime startTime;

    private System.Action finishedCallback;
    private System.Action skippedCallback;
    private System.Action failedCallback;
    private System.Action cheatCallback;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ADS
        EnableUnityAds();
#endif
#if IRON_SOURCE
        EnableIronSourceAds();
#endif
    }

    void Update()
    {
        if (!requestingVideo) return;
        requestTime += Time.unscaledDeltaTime;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (requestTime >= 3f)
        {
            HandleShowVideoResult(0);
            requestingVideo = false;
            return;
        }
#endif
    }

    public void ShowVideoAds(System.Action finishEvent,
        System.Action skippedEvent,
        System.Action failedEvent,
        System.Action cheatEvent)
    {
#if IRON_SOURCE
        if (IronSourceAdsManager.instance && IronSourceAdsManager.instance.isInited == false)
        {
            IronSourceAdsManager.instance.InitIronSource();
        }
#endif
        requestingVideo = true;
        requestTime = 0;

        startTime = System.DateTime.UtcNow;

#if UNITY_EDITOR || UNITY_STANDALONE
        this.finishedCallback = finishEvent;
        //return;
#endif

        bool showAds = false;
#if UNITY_ADS
        if (showAds == false && 
            UnityAdsManager.instance.IsCanShowVideo())
        {
            showAds = true;
            this.finishedCallback = finishEvent;
            this.skippedCallback = skippedEvent;
            this.failedCallback = failedEvent;
            this.cheatCallback = cheatEvent;
#if UNITY_ANDROID || UNITY_IOS
            //Hiker.QualityRenderSetting.instance.SetTargetFPS(30);
#endif
            UnityAdsManager.instance.ShowVideoAds((result) =>
            {
                requestingVideo = false;
                
                switch (result)
                {
                    case ShowResult.Finished:
                        HandleShowVideoResult(0);
                        break;
                    case ShowResult.Skipped:
                        HandleShowVideoResult(1);
                        break;
                    case ShowResult.Failed:
                        HandleShowVideoResult(-1);
                        break;
                    default:
                        break;
                }
            },
            placementId);
        }
#endif
#if IRON_SOURCE
        if (showAds == false)
        {
            if (IronSourceAdsManager.instance.IsCanShowVideo())
            {
                this.finishedCallback = finishEvent;
                this.skippedCallback = skippedEvent;
                this.failedCallback = failedEvent;
                this.cheatCallback = cheatEvent;
#if UNITY_ANDROID || UNITY_IOS
                //Hiker.QualityRenderSetting.instance.SetTargetFPS(30);
#endif
                GUIManager.Instance.MuteAudio();
                showAds = IronSourceAdsManager.instance.ShowVideoAds(
                    () => {
                        float checkTime = ConfigManager.GetThoiGianBatCheatVideoQuangCao();

                        var curTime = System.DateTime.UtcNow;
                        var duration = (curTime - startTime).TotalSeconds;
                        if (duration < checkTime)
                        {
                            HandleShowVideoResult(2);
                        }
                        else
                        {
                            HandleShowVideoResult(0);
                        }
                    },
                    () => HandleShowVideoResult(1),
                    () => HandleShowVideoResult(-1));
                if (showAds == false)
                {
                    GUIManager.Instance.UnmuteAudio();
                    //Hiker.QualityRenderSetting.instance.SetTargetFPS();
                }
            }
            else if (IronSourceAdsManager.instance.IsInterstitialReady)
            {
                this.finishedCallback = finishEvent;
                this.skippedCallback = skippedEvent;
                this.failedCallback = failedEvent;

                GUIManager.Instance.MuteAudio();
                showAds = IronSourceAdsManager.instance.ShowIntersitialAds(
                    () => {
                        float checkTime = ConfigManager.GetThoiGianBatCheatVideoQuangCaoInters();

                        var curTime = System.DateTime.UtcNow;
                        var duration = (curTime - startTime).TotalSeconds;
                        if (duration < checkTime)
                        {
                            HandleShowVideoResult(2);
                        }
                        else
                        {
                            HandleShowVideoResult(0);
                        }
                    },
                    () => HandleShowVideoResult(1),
                    () => HandleShowVideoResult(-1));
                if (showAds == false)
                {
                    GUIManager.Instance.UnmuteAudio();
                    //Hiker.QualityRenderSetting.instance.SetTargetFPS();
                }
            }
            else
            {
                IronSourceAdsManager.instance.LoadInterstitial();
            }
        }
#endif

    }

    /// <summary>
    /// result = 0 => finished, 1 => skip, -1 => fail
    /// </summary>
    /// <param name="result">0 = finished, 1 = skip, -1 = fail</param>
    void HandleShowVideoResult(int result)
    {
        requestingVideo = false;
        GUIManager.Instance.UnmuteAudio();
#if UNITY_ANDROID || UNITY_IOS
        //Hiker.QualityRenderSetting.instance.SetTargetFPS(); // reset back to default
#endif
        if (IsCanShowVideo())
        {
#if DEBUG
            Debug.Log("Ad Available");
#endif
        }
        switch (result)
        {
            case 0:
#if DEBUG
                Debug.Log("Video completed.");
#endif
                AnalyticsManager.LogEvent("FINISH_ADS");
                this.finishedCallback?.Invoke();
                break;
            case 1:
#if DEBUG
                Debug.Log("Video skipped.");
#endif
                AnalyticsManager.LogEvent("CANCEL_ADS");
                this.skippedCallback?.Invoke();
                break;
            case -1:
#if DEBUG
                Debug.Log("Video failed");
#endif
                AnalyticsManager.LogEvent("FAILED_ADS");
                PopupMessage.Create(MessagePopupType.ERROR, Localization.Get("watch_ads_fail"));
                this.failedCallback?.Invoke();
                break;
            case 2:
                AnalyticsManager.LogEvent("CHEAT_ADS");
                PopupMessage.Create(MessagePopupType.ERROR, Localization.Get("watch_ads_fail"));
                this.cheatCallback?.Invoke();
                break;
            default:
                break;
        }
    }

    public bool IsCanShowVideo()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return !requestingVideo;
#endif
        if (requestingVideo) return false;
        bool haveAds = false;
#if UNITY_ADS
        if (UnityAdsManager.instance.IsCanShowVideo())
        {
            haveAds = true;
        }
#endif
#if IRON_SOURCE
        if (IronSourceAdsManager.instance.IsCanShowVideo())
        {
            haveAds = true;
        }
        else if (IronSourceAdsManager.instance.IsInterstitialReady)
        {
            haveAds = true;
        }
        else
        {
            IronSourceAdsManager.instance.LoadInterstitial();
        }
#endif
        return haveAds;
    }

#if IRON_SOURCE
    private void EnableIronSourceAds()
    {
        if (IronSourceAdsManager.instance == null)
        {
            GameObject ironSourceManager = new GameObject("IronSourceManager");
            ironSourceManager.transform.SetParent(transform);
            ironSourceManager.AddComponent<IronSourceAdsManager>();
        }
    }
#endif

#if UNITY_ADS
    private void EnableUnityAds()
    {
        if (UnityAdsManager.instance == null)
        {
            GameObject manager = new GameObject("UnityAdsManager");
            manager.transform.SetParent(transform);
            manager.AddComponent<UnityAdsManager>();
        }
    }
#endif
}
