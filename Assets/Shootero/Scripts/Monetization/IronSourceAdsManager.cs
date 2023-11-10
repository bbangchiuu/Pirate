using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronSourceAdsManager : MonoBehaviour
{
    public static IronSourceAdsManager instance;

    public bool isVideoAvailable { get; private set; }

#if UNITY_ANDROID
    //const string AppKey = "b5f249f5";
    const string AppKey = "b685effd"; // onesoft
#elif UNITY_IOS
    //const string AppKey = "b5f2bd05";
    const string AppKey = "b6ddceb5"; // onesoft
#else
    const string AppKey = "pctest";
#endif

    private System.Action finishedCallback;
    private System.Action skippedCallback;
    private System.Action failedCallback;

    bool shouldReward = false;
    bool isClosedAds = false;
    bool isFinishedPlaying = false;

    bool isInterstitialReady = false;
    bool isLoadingInterstitial = false;

    public bool isInited { get; private set; }

    private void Awake()
    {
        instance = this;
        isInited = false;
    }

    private void Start()
    {
        
    }

    public void InitIronSource()
    {
        if (isInited) return;

#if UNITY_IOS && !UNITY_EDITOR
        var att = Hiker.GUI.PopupAppTracking.GetAppTrackingTransparencyPref();
        IronSource.Agent.setConsent(att > 0);

        var attiOS = Hiker.GUI.PopupAppTracking.GetAppTrackingTransparencySetting();
        AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(attiOS > 2);
#endif
        //IronSource.Agent.setMetaData("do_not_sell", "true");

        IronSource.Agent.init(AppKey, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL);
        IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
        IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
        IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;

        IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
        IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
        IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
        IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
        IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
        IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
        IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;

        IronSourceEvents.onImpressionSuccessEvent += IronSourceEvents_onImpressionSuccessEvent;

        LoadInterstitial();
#if DEBUG
        IronSource.Agent.validateIntegration();
        IronSource.Agent.setAdaptersDebug(true);
#endif
        isInited = true;
    }

    private void IronSourceEvents_onImpressionSuccessEvent(IronSourceImpressionData impressionData)
    {

#if DEBUG
        Debug.Log("unity-script:  ImpressionSuccessEvent impressionData = " + impressionData);
#endif
        if (impressionData != null)
        {
#if FIRE_BASE
            Firebase.Analytics.Parameter[] AdParameters = {
                new Firebase.Analytics.Parameter("ad_platform", "ironSource"),
                new Firebase.Analytics.Parameter("ad_source", impressionData.adNetwork),
                new Firebase.Analytics.Parameter("ad_unit_name", impressionData.adUnit),
                new Firebase.Analytics.Parameter("ad_format", impressionData.instanceName),
                new Firebase.Analytics.Parameter("currency","USD"),
                new Firebase.Analytics.Parameter("value", impressionData.revenue.HasValue ? impressionData.revenue.Value : 0d)
            };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", AdParameters);
#endif
        }
    }

    public bool IsInterstitialReady { get { return isInterstitialReady; } }

    public void LoadInterstitial()
    {
        if (isLoadingInterstitial == false)
        {
            isInterstitialReady = false;
            isLoadingInterstitial = true;
            IronSource.Agent.loadInterstitial();
            //Debug.Log("loadInterstitial");
        }
    }

    // Invoked when end user clicked on the interstitial ad
    [Beebyte.Obfuscator.SkipRename]
    private void InterstitialAdClickedEvent()
    {
        AnalyticsManager.LogEvent("IR_INTERSTITIAL_CLICKED");
    }

    //Invoked when the Interstitial Ad Unit has opened
    [Beebyte.Obfuscator.SkipRename]
    private void InterstitialAdOpenedEvent()
    {
        
    }

    //Invoked when the interstitial ad closed and the user goes back to the application screen.
    [Beebyte.Obfuscator.SkipRename]
    private void InterstitialAdClosedEvent()
    {
        isClosedAds = true;
        shouldReward = true;
        if (finishedCallback != null) finishedCallback();
        AnalyticsManager.LogEvent("IR_INTERSTITIAL_FINISHED");
        //Debug.Log("InterstitialAdClosedEvent");
        LoadInterstitial();
    }

    //Invoked when the ad fails to show.
    //@param description - string - contains information about the failure.
    [Beebyte.Obfuscator.SkipRename]
    private void InterstitialAdShowFailedEvent(IronSourceError obj)
    {
        if (failedCallback != null) this.failedCallback();
        shouldReward = false;
        AnalyticsManager.LogEvent("IR_INTERSTITIAL_FAILED");
        //Debug.Log("InterstitialAdShowFailedEvent");
    }

    //Invoked right before the Interstitial screen is about to open.
    [Beebyte.Obfuscator.SkipRename]
    private void InterstitialAdShowSucceededEvent()
    {
        
    }

    //Invoked when the initialization process has failed.
    //@param description - string - contains information about the failure.
    [Beebyte.Obfuscator.SkipRename]
    private void InterstitialAdLoadFailedEvent(IronSourceError obj)
    {
        isInterstitialReady = false;
        isLoadingInterstitial = false;
        //Debug.Log("InterstitialAdLoadFailedEvent");
    }

    //Invoked when the Interstitial is Ready to shown after load function is called
    [Beebyte.Obfuscator.SkipRename]
    private void InterstitialAdReadyEvent()
    {
        isInterstitialReady = true;
        isLoadingInterstitial = false;
    }

    //  Note: the events below are not available for all supported rewarded video 
    //   ad networks. Check which events are available per ad network you choose 
    //   to include in your build.
    //   We recommend only using events which register to ALL ad networks you 
    //   include in your build.
    //Invoked when the video ad starts playing.
    [Beebyte.Obfuscator.SkipRename]
    void RewardedVideoAdStartedEvent()
    {
    }
    //Invoked when the video ad finishes playing.
    [Beebyte.Obfuscator.SkipRename]
    void RewardedVideoAdEndedEvent()
    {
        isFinishedPlaying = true;
    }

    //Invoked when the user completed the video and should be rewarded. 
    //If using server-to-server callbacks you may ignore this events and wait for the callback from the  ironSource server.
    //
    //@param - placement - placement object which contains the reward data
    //
    [Beebyte.Obfuscator.SkipRename]
    private void RewardedVideoAdRewardedEvent(IronSourcePlacement obj)
    {
        shouldReward = true;
        if (finishedCallback != null) finishedCallback();
        AnalyticsManager.LogEvent("IRON_SOURCE_FINISHED");
    }
    //Invoked when the Rewarded Video failed to show
    //@param description - string - contains information about the failure.
    [Beebyte.Obfuscator.SkipRename]
    void RewardedVideoAdShowFailedEvent(IronSourceError error)
    {
        if (failedCallback != null) this.failedCallback();
        shouldReward = false;
        AnalyticsManager.LogEvent("IRON_SOURCE_FAILED");
    }

    //Invoked when there is a change in the ad availability status.
    //@param - available - value will change to true when rewarded videos are available. 
    //You can then show the video by calling showRewardedVideo().
    //Value will change to false when no videos are available.
    [Beebyte.Obfuscator.SkipRename]
    private void RewardedVideoAvailabilityChangedEvent(bool obj)
    {
        isVideoAvailable = obj;
        Debug.LogFormat("IR_A_{0}", obj);
    }

    //Invoked when the RewardedVideo ad view has opened.
    //Your Activity will lose focus. Please avoid performing heavy 
    //tasks till the video ad will be closed.
    [Beebyte.Obfuscator.SkipRename]
    void RewardedVideoAdOpenedEvent()
    {
        isClosedAds = false;
    }
    //Invoked when the RewardedVideo ad view is about to be closed.
    //Your activity will now regain its focus.
    [Beebyte.Obfuscator.SkipRename]
    void RewardedVideoAdClosedEvent()
    {
        isClosedAds = true;
        if (shouldReward == false)
        {
            if (skippedCallback != null) this.skippedCallback();

            AnalyticsManager.LogEvent("IRON_SOURCE_SKIP");
        }
    }

    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    public bool IsCanShowVideo()
    {
        return //isVideoAvailable || 
            IronSource.Agent.isRewardedVideoAvailable();
    }

    public void SetConsent(bool flag)
    {
        IronSource.Agent.setConsent(flag);
    }

    public bool ShowVideoAds(System.Action finishEvent, System.Action skippedEvent, System.Action failedEvent,
        string placementId = "")
    {
        shouldReward = false;
        isClosedAds = false;
        isFinishedPlaying = false;
        if (//isVideoAvailable == false &&
            IronSource.Agent.isRewardedVideoAvailable() == false) return false;

        AnalyticsManager.LogEvent("SHOW_IRON_SOURCE_ADS");
        this.finishedCallback = finishEvent;
        this.skippedCallback = skippedEvent;
        this.failedCallback = failedEvent;

        IronSource.Agent.showRewardedVideo();
        isVideoAvailable = false;
        return true;
    }

    public bool ShowIntersitialAds(
        System.Action finishEvent,
        System.Action skippedEvent,
        System.Action failedEvent,
        string placementId = "")
    {
        shouldReward = false;
        isClosedAds = false;
        isFinishedPlaying = false;
        if (IronSource.Agent.isInterstitialReady() == false) return false;

        AnalyticsManager.LogEvent("SHOW_IR_INTERSTITIAL_ADS");

        this.finishedCallback = finishEvent;
        this.skippedCallback = skippedEvent;
        this.failedCallback = failedEvent;

        IronSource.Agent.showInterstitial();
        isInterstitialReady = false;
        Debug.Log("ShowIntersitialAds");
        return true;
    }
}
