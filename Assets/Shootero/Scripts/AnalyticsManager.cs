using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if FIRE_BASE
using Firebase.Extensions;
#endif

#if ANALYTICS_UNITY
using UnityEngine.Analytics;
#endif

#if APPSFLYER
using AppsFlyerSDK;
#endif


public class AnalyticsManager : MonoBehaviour
{
    private DateTime FirstSessionStartTime;
    public static bool FirebaseCheckDependenciesSuccess { get; set; }

    private static string AF_DEV_KEY = "Gio8zFFVzAZJGr9sbHXoVb";

    void Start()
    {
#if APPSFLYER && (UNITY_ANDROID || UNITY_IOS)

        //global 
        //AppsFlyer.setAppsFlyerKey(AF_DEV_KEY);
        //AppsFlyer.setCurrencyCode("USD");
#if DEBUG
        //AppsFlyer.setIsDebug(true);
#endif
#if UNITY_IOS
        /* Mandatory - set your apple app ID
            NOTE: You should enter the number only and not the "ID" prefix */
        //AppsFlyer.setAppID ("1495454611");
        //AppsFlyer.getConversionData();
        //AppsFlyer.trackAppLaunch ();
#elif UNITY_ANDROID
        /* Mandatory - set your Android package name */
        //AppsFlyer.setAppID("com.hikergames.ArcadeHunter");
        /* For getting the conversion data in Android, you need to add the "AppsFlyerTrackerCallbacks" listener.*/
        //AppsFlyer.init(AF_DEV_KEY, "AppsFlyerTrackerCallbacks");
#endif
#endif

#if FIRE_BASE && !UNITY_EDITOR
        
        // Initialize Firebase
        if (FirebaseCheckDependenciesSuccess == false) {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    // Crashlytics will use the DefaultInstance, as well;
                    // this ensures that Crashlytics is initialized.
                    Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                    FirebaseCheckDependenciesSuccess = true;
                    // Set a flag here for indicating that your project is ready to use Firebase.
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }

        var first_session_start_time_txt = PlayerPrefs.GetString("FirstSessionStartTime");
        if (string.IsNullOrEmpty(first_session_start_time_txt))
        {
            this.FirstSessionStartTime = DateTime.Now;
            PlayerPrefs.SetString("FirstSessionStartTime", this.FirstSessionStartTime.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
#endif
    }

    public static void SetUserProperty(string propName, string value)
    {
#if FIRE_BASE
        if (FirebaseCheckDependenciesSuccess)
            Firebase.Analytics.FirebaseAnalytics.SetUserProperty(propName, value);
#endif
    }

    private static bool IsInited = false;

    public static void LogEvent(string event_name, params AnalyticsParameter[] parameters)
    {
#if UNITY_EDITOR
        Debug.Log("ANALYTICS - " + event_name + ": " + LitJson.JsonMapper.ToJson(parameters));
        return;
#endif

#if UNITY_ANDROID || UNITY_IOS

        if (!IsInited && GameClient.instance != null && GameClient.instance.UInfo != null && GameClient.instance.UInfo.Gamer != null)
        {
            IsInited = true;
#if APPSFLYER
            AppsFlyer.setCustomerUserId(GameClient.instance.UInfo.GID.ToString());
#endif

#if FIRE_BASE
            if (FirebaseCheckDependenciesSuccess) {

                Firebase.Analytics.FirebaseAnalytics.SetUserId(GameClient.instance.UInfo.GID.ToString());
                Firebase.Analytics.FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 10, 0));
                if (GameClient.instance.UInfo.Gamer.RegisterTime != null)
                {
                    var register_time = GameClient.instance.UInfo.Gamer.RegisterTime;
                    Firebase.Analytics.FirebaseAnalytics.SetUserProperty("DateOfBirth", register_time.ToString("yyyyMMdd"));
                }
            }
#endif
        }
#endif

#if ANALYTICS_UNITY || FACEBOOK
        var event_parameters = new Dictionary<string, object>();

        for (int i = 0; i < parameters.Length; i++)
        {
            var parameter_name = parameters[i].Name;
            var parameter_value = parameters[i].Value;
            event_parameters[parameter_name] = parameter_value;
        }

#if ANALYTICS_UNITY
        UnityEngine.Analytics.Analytics.CustomEvent(event_name, event_parameters);
#endif

#if FACEBOOK
        // turn off fb log event to use events from Appsflyer
        //Facebook.Unity.FB.LogAppEvent(event_name, null, event_parameters);
#endif

#endif

#if FIRE_BASE && (UNITY_ANDROID || UNITY_IOS)
        if (FirebaseCheckDependenciesSuccess) {
            var firebase_parameters = new List<Firebase.Analytics.Parameter>();

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter_name = parameters[i].Name;
                var parameter_value = parameters[i].Value;
                Firebase.Analytics.Parameter firebase_parameter = null;

                if (parameter_value is double)
                {
                    firebase_parameter = new Firebase.Analytics.Parameter(parameter_name, Convert.ToDouble(parameter_value));
                }
                else if (parameter_value is int || parameter_value is long)
                {
                    firebase_parameter = new Firebase.Analytics.Parameter(parameter_name, Convert.ToInt64(parameter_value));
                }
                else
                {
                    firebase_parameter = new Firebase.Analytics.Parameter(parameter_name, parameter_value.ToString());
                }

                if (firebase_parameter != null)
                {
                    firebase_parameters.Add(firebase_parameter);
                }
            }

            Firebase.Analytics.FirebaseAnalytics.LogEvent(event_name, firebase_parameters.ToArray());
        }
#endif

        //[ 01/26/2018 10:15:16 AM: PhuongND] - APPFLYER
#if APPSFLYER //&& (UNITY_ANDROID || UNITY_IOS)
        var ev = event_name.ToLowerInvariant();
        if (System.Array.IndexOf(AppsflyerEvents, ev) > -1)
        {
            Dictionary<string, string> appflyerParameters = new Dictionary<string, string>();
            for (int i = 0; i < parameters.Length; i++)
            {
                appflyerParameters.Add(parameters[i].Name, parameters[i].Value.ToString());
            }
            AppsFlyer.sendEvent(event_name, appflyerParameters);
        }
#endif
    }

    static readonly string[] AppsflyerEvents = new string[]
    {
        "af_purchase",
        "af_ad_revenue",
        "iron_source_finished",
        //"iron_source_failed",
        "iron_source_skip",
        "show_iron_source_ads",
        "ir_interstitial_finished",
        "ir_interstitial_clicked",
        //"ir_interstitial_failed",
        "show_ir_interstitial_ads",
    };

    public static void LogPurchase(string type, string product_id, string price, string currencyCode = "USD")
    {
#if FACEBOOK
//#if UNITY_ANDROID || UNITY_IOS
//        var parameters = new Dictionary<string, object>();
//        parameters[Facebook.Unity.AppEventParameterName.ContentID] = product_id;
//        parameters[Facebook.Unity.AppEventParameterName.NumItems] = 1;
//        parameters[Facebook.Unity.AppEventParameterName.ContentType] = type;
//        parameters[Facebook.Unity.AppEventParameterName.Currency] = currencyCode;
//        Facebook.Unity.FB.LogPurchase(price, "USD", parameters);
//#endif
#endif
#if APPSFLYER && (UNITY_ANDROID || UNITY_IOS)
        AppsFlyer.sendEvent(AFInAppEvents.PURCHASE, new Dictionary<string, string>()
        {
            { AFInAppEvents.CONTENT_ID, product_id },
            { AFInAppEvents.QUANTITY, "1" },
            { AFInAppEvents.REVENUE,  price },
            { AFInAppEvents.CONTENT_TYPE, type },
            { AFInAppEvents.CURRENCY, currencyCode }
        });
#endif
    }

    void OnApplicationPause(bool pause)
    {
#if FIRE_BASE && !UNITY_EDITOR
        if (pause)
        {
            this.ProcessEndFirstSession();
        }
        else
        {
            this.ProcessContinueFirstSession();
        }
#endif
    }

    private void OnApplicationQuit()
    {
#if FIRE_BASE && !UNITY_EDITOR
        this.ProcessEndFirstSession();
#endif
    }

    private void ProcessEndFirstSession()
    {
        var first_session_end_time_txt = PlayerPrefs.GetString("FirstSessionEndTime");
        if (string.IsNullOrEmpty(first_session_end_time_txt))
        {
            var first_session_end_time = DateTime.Now;
            PlayerPrefs.SetString("FirstSessionEndTime", first_session_end_time.ToString(System.Globalization.CultureInfo.InvariantCulture));
            var first_session_length = (first_session_end_time - this.FirstSessionStartTime).TotalSeconds;
#if FIRE_BASE && !UNITY_EDITOR
            if (FirebaseCheckDependenciesSuccess) {
                //Debug.Log("FIRST_SESSION_LENGTH: " + first_session_length);
                Firebase.Analytics.FirebaseAnalytics.SetUserProperty("FirstSessionLength", first_session_length.ToString());
            }
#endif
        }
    }

    private void ProcessContinueFirstSession()
    {
        var first_session_start_time_txt = PlayerPrefs.GetString("FirstSessionStartTime");
        var first_session_end_time_txt = PlayerPrefs.GetString("FirstSessionEndTime");

        if (string.IsNullOrEmpty(first_session_start_time_txt) == false &&
            string.IsNullOrEmpty(first_session_end_time_txt) == false)
        {
            if (DateTime.TryParse(first_session_start_time_txt,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.AssumeLocal,
                    out DateTime first_session_start_time))
            {
                if (DateTime.TryParse(first_session_end_time_txt,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.AssumeLocal,
                    out DateTime first_session_end_time))
                {
                    var delta_time = (DateTime.Now - first_session_end_time).TotalMinutes;
                    if (delta_time < 10f)
                    {
                        PlayerPrefs.DeleteKey("FirstSessionEndTime");
                        this.FirstSessionStartTime = first_session_start_time;
                    }
                }
            }
        }
    }
}

public class AnalyticsParameter
{
    public string Name { get; set; }
    public object Value { get; set; }

    public AnalyticsParameter(string name, object data)
    {
        this.Name = name;
        this.Value = data;
    }
}
