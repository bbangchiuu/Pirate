using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if FIRE_BASE
using Firebase.Extensions;
#endif
public class RemoteConfigManager : MonoBehaviour
{
    public static RemoteConfigManager Instance = null;
    public const string HourDelayCancelLoginSocial = "HourDelayCancelLoginSocial";
    public const string HostURL = "HostURL";

    public bool isActivated { get; set; }
    public bool isReady { get; set; }


    Dictionary<string, object> defaults =
        new Dictionary<string, object>();

    private void Awake()
    {
        Instance = this;
        defaults.Clear();

        defaults.Add(HourDelayCancelLoginSocial, 8);
        defaults.Add(HostURL, string.Empty);
        //defaults.Add("CHAT_DB_ROOT", string.Empty);
        defaults.Add("FPS_COUNTER", false);
        defaults.Add("IOS_TARGET_FPS", 0);
        defaults.Add("IOS_QUALITY_AA", 2);
        defaults.Add("QUALITY_SHADOW", -1);
        defaults.Add("SHADOW_RES", -1);
        defaults.Add("IOS_LIMIT_CAPFRAME", 0);
        defaults.Add("ANDROID_TARGET_FPS", 0);
        defaults.Add("ANDROID_LIMIT_CAPFRAME", string.Empty);
        defaults.Add("DISABLE_IRON_SOURCE", false);
        defaults.Add("DISABLE_UNITY_ADS", false);

//#if FIRE_BASE
//        Firebase.RemoteConfig.FirebaseRemoteConfig.SetDefaults(defaults);
//#endif
    }

    private void ActivateComplete(System.Threading.Tasks.Task<bool> fetchTask)
    {
        isActivated = fetchTask.Result;
        if (this != null)
        {
            isReady = true;

            if (GetBoolConfigValue("FPS_COUNTER"))
            {
                if (FPSCounter.instance)
                {
                    FPSCounter.instance.gameObject.SetActive(true);
                }
            }
            if (Hiker.QualityRenderSetting.instance)
            {
                Hiker.QualityRenderSetting.instance.OnFetchRemoteConfig();
            }
        }
    }

#if FIRE_BASE

    private System.Threading.Tasks.Task FetchRemoteConfig(float minTimeSpan)
    {
        var fetch = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(System.TimeSpan.FromMinutes(minTimeSpan));
        return fetch.ContinueWithOnMainThread(FetchComlete);
    }

    private void FetchComlete(System.Threading.Tasks.Task fetchTask)
    {
        if (fetchTask.IsCanceled)
        {
            Debug.Log("Fetch canceled");
        }
        else if (fetchTask.IsFaulted)
        {
            Debug.Log("Fetch encountered an error");
        }
        else if (fetchTask.IsCompleted)
        {
            Debug.Log("Fetch completed successfully!");
        }

        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
        switch (info.LastFetchStatus)
        {
            case Firebase.RemoteConfig.LastFetchStatus.Success:
                Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(ActivateComplete);
                //Debug.Log(string.Format("Remote data loaded and ready (last fetch time {0}).",
                //                       info.FetchTime));
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason)
                {
                    case Firebase.RemoteConfig.FetchFailureReason.Error:
                        Debug.Log("Fetch failed for unknown reason");
                        break;
                    case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                        Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                        break;
                }
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Pending:
                Debug.Log("Latest Fetch call still pending.");
                break;
        }

        isReady = true;

        if (GetBoolConfigValue("FPS_COUNTER"))
        {
            if (FPSCounter.instance)
            {
                FPSCounter.instance.gameObject.SetActive(true);
            }
        }
        if (Hiker.QualityRenderSetting.instance)
        {
            Hiker.QualityRenderSetting.instance.OnFetchRemoteConfig();
        }
    }

#endif
    // Use this for initialization
    IEnumerator Start()
    {
#if FIRE_BASE //&& !UNITY_EDITOR
        yield return new WaitUntil(() => AnalyticsManager.FirebaseCheckDependenciesSuccess);
        if (AnalyticsManager.FirebaseCheckDependenciesSuccess)
        {
            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).ContinueWithOnMainThread(task => {
                FetchRemoteConfig(
                    20
                    );
            });
        }
#else
        isReady = true;
        yield return null;
#endif
    }

    public long GetLongConfigValue(string key)
    {
#if FIRE_BASE
        if (AnalyticsManager.FirebaseCheckDependenciesSuccess)
        {
            return Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).LongValue;
        }
        else
#endif
        if (defaults.TryGetValue(key, out object value))
        {
            var val = (long)value;
            return val;
        }
        else
        {
            return 0;
        }
    }

    public string GetStringConfigValue(string key)
    {
#if FIRE_BASE
        if (AnalyticsManager.FirebaseCheckDependenciesSuccess)
        {
            return Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
        }
        else
#endif
        if (defaults.TryGetValue(key, out object value))
        {
            var val = (string)value;
            return val;
        }
        else
        {
            return string.Empty;
        }
    }

    public double GetDoubleConfigValue(string key)
    {
#if FIRE_BASE
        if (AnalyticsManager.FirebaseCheckDependenciesSuccess)
        {
            return Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).DoubleValue;
        }
        else
#endif
        if (defaults.TryGetValue(key, out object value))
        {
            var val = (double)value;
            return val;
        }
        else
        {
            return 0;
        }
    }

    public bool GetBoolConfigValue(string key)
    {
#if FIRE_BASE
        if (AnalyticsManager.FirebaseCheckDependenciesSuccess)
        {
            return Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).BooleanValue;
        }
        else
#endif
        if (defaults.TryGetValue(key, out object value))
        {
            var val = (bool)value;
            return val;
        }
        else
        {
            return false;
        }
    }
}
