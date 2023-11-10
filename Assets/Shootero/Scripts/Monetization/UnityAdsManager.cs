using UnityEngine;
using System.Collections;
using Hiker.GUI;
#if UNITY_ADS
using UnityEngine.Advertisements;
public class UnityAdsManager : MonoBehaviour, IUnityAdsListener
{
    public static UnityAdsManager instance;

    public const string rewardVideo = "rewardedVideo";
    public const string video = "video";

    private const string AndroidGameId = "3432749";
    private const string IosGameId = "3432748";

    private System.Action<ShowResult> onFinishAds;

    private string gameId = "";
    public bool IsRewardVideoReady = false;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //string gameId = "";
        IsRewardVideoReady = false;
#if UNITY_ANDROID
        gameId = AndroidGameId;
#elif UNITY_IOS
        gameId = IosGameId;
#endif

#if !UNITY_EDITOR
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId);
        IsRewardVideoReady = Advertisement.IsReady(rewardVideo);
#endif
    }
    
    public void ShowVideoAds(System.Action<ShowResult> onFinish, string placementId = rewardVideo)
    {
        if (!Advertisement.IsReady(placementId)) return;
        AnalyticsManager.LogEvent("SHOW_UNITY_ADS");
        this.onFinishAds = onFinish;

        //var options = new ShowOptions();
        //options.gamerSid = gameId;
        //options.resultCallback = this.HandleShowVideoResult;
        Advertisement.Show(placementId);
    }

    public bool IsCanShowVideo(string placementId = rewardVideo)
    {
        if (Advertisement.IsReady(placementId))
        {
            return !requestingVideo;
        }
        else
        {
            return false;
        }
    }

    void IUnityAdsListener.OnUnityAdsReady(string placementId)
    {
        if (placementId == rewardVideo)
        {
            IsRewardVideoReady = true;
        }
    }

    void IUnityAdsListener.OnUnityAdsDidError(string message)
    {
        Debug.Log("UnityAds error: " + message);
    }

    void IUnityAdsListener.OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }

    void IUnityAdsListener.OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        onFinishAds?.Invoke(showResult);
        switch (showResult)
        {
            case ShowResult.Finished:
                Debug.Log("Video completed.");
                AnalyticsManager.LogEvent("FINISH_UNITY_ADS");
                break;
            case ShowResult.Skipped:
                Debug.Log("Video skipped.");
                AnalyticsManager.LogEvent("CANCEL_UNITY_ADS");
                break;
            case ShowResult.Failed:
                Debug.Log("Video failed");
                AnalyticsManager.LogEvent("FAILED_UNITY_ADS");
                break;
        }
    }
}
#endif