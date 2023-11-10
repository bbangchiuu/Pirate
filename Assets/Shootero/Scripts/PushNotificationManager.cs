using UnityEngine;
using DateTime = System.DateTime;
using TimeSpan = System.TimeSpan;
using DayOfWeek = System.DayOfWeek;
using System.Collections;
using System.Collections.Generic;
using LitJson;
#if APPSFLYER
using AppsFlyerSDK;
#endif
#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif
#if FIRE_BASE
using Firebase;
#endif

[Beebyte.Obfuscator.Skip]
public class PushNotificationManager : MonoBehaviour
{
    public static PushNotificationManager Instance { get; set; }
    public LoginHistoryData LoginHistoryData { get; set; }
    public List<NotificationData> ListNotificationDatas { get; set; }
    public string DeviceToken { get; set; }
    //private int NotificationID { get; set; }

    void Awake()
    {
        if (Instance) return;

        DontDestroyOnLoad(this.gameObject);
        Instance = this;
#if UNITY_ANDROID
        var c = new AndroidNotificationChannel()
        {
            Id = "Default",
            Name = "Default",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(c);
#endif
    }

    void OnApplicationPause(bool pause)
    {
#if UNITY_ANDROID || UNITY_IOS
        if (pause)
        {
            //if (SplashScreen.Instance && SplashScreen.Instance.gameObject.activeSelf) return;
            this.ClearAllNotifications();
            if (ListNotificationDatas == null) LoadData();
            CheckNotificationComeBack(false);
            foreach (var notification_data in this.ListNotificationDatas)
            {
                if (notification_data != null)
                    notification_data.Push();
            }
            PlayerPrefs.SetString("LocalNotificationDatas", JsonMapper.ToJson(this.ListNotificationDatas));
        }
        else
        {
            this.ClearAllNotifications();
            CheckNotificationComeBack();
        }
#endif

    }

    public void CheckNotificationComeBack(bool updateHabitTime = true)
    {
#if SERVER_1_3
        if (GameClient.instance.GID > 0 &&
            GameClient.instance.UInfo != null &&
            GameClient.instance.UInfo.Gamer != null &&
            GameClient.instance.UInfo.Gamer.RegisterTime.AddDays(3) > GameClient.instance.ServerTime)
        {
            if (GameClient.instance.UInfo.Gamer.ComeBack == 0)
            {
                if (updateHabitTime)
                {
                    var habitTime = DateTime.Now.AddDays(1);//GetPushTimeByHabit(1, false);
                    SetPushNotificationByTime(NotificationCode.FirstAid, habitTime);
                }
                
            }
            else if (ListNotificationDatas != null)
            {
                ListNotificationDatas.RemoveAll(e => e.Code == NotificationCode.FirstAid);
            }
        }
        else
#endif
        {
            if (ListNotificationDatas != null)
            {
                ListNotificationDatas.RemoveAll(e => e.Code == NotificationCode.FirstAid);
            }
            if (updateHabitTime)
            {
                var habitTime = GetPushTimeByHabit(2, false);
                SetPushNotificationByTime(NotificationCode.Comeback, habitTime);
            }
        }
    }

    IEnumerator InitPushNotification()
    {
        yield return new WaitUntil(() => AnalyticsManager.FirebaseCheckDependenciesSuccess);
#if FIRE_BASE && (UNITY_ANDROID || UNITY_IOS)
        if (AnalyticsManager.FirebaseCheckDependenciesSuccess)
        {
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        }
#endif
    }

    void Start()
    {
        StartCoroutine(InitPushNotification());

#if UNITY_IOS
        //StartCoroutine(RequestAuthorization());
#endif
        this.LoadData();
        this.ClearAllNotifications();
        CheckNotificationComeBack();
    }

#if FIRE_BASE && (UNITY_ANDROID || UNITY_IOS)
    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        //UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
        this.DeviceToken = token.Token;
#if UNITY_ANDROID
        AppsFlyerAndroid.updateServerUninstallToken(token.Token);
#endif
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
    }
#endif

    public void LoadData()
    {
        var login_history_txt = PlayerPrefs.GetString("LoginHistory");
        if (string.IsNullOrEmpty(login_history_txt))
        {
            this.LoginHistoryData = new LoginHistoryData();
        }
        else
        {
            try
            {
                this.LoginHistoryData = JsonMapper.ToObject<LoginHistoryData>(login_history_txt);
            }
            catch
            {
                this.LoginHistoryData = new LoginHistoryData();
            }
        }

        var now = System.DateTime.Now;

        if (now.DayOfWeek == System.DayOfWeek.Saturday || now.DayOfWeek == System.DayOfWeek.Sunday)
        {
            this.LoginHistoryData.WeekendHistory[now.Hour]++;
        }
        else
        {
            this.LoginHistoryData.NormalHistory[now.Hour]++;
        }

        PlayerPrefs.SetString("LoginHistory", JsonMapper.ToJson(this.LoginHistoryData));

        var notification_data_txt = PlayerPrefs.GetString("LocalNotificationDatas");
        if (string.IsNullOrEmpty(notification_data_txt))
        {
            this.ListNotificationDatas = new List<NotificationData>();
        }
        else
        {
            try
            {
                this.ListNotificationDatas = JsonMapper.ToObject<List<NotificationData>>(notification_data_txt);
            }
            catch
            {
                this.ListNotificationDatas = new List<NotificationData>();
            }
        }
    }

    public void SetPushNotificationByTime(NotificationCode code, System.DateTime push_time)
    {
        if (ListNotificationDatas == null) LoadData();
        NotificationData notification_data = this.ListNotificationDatas.Find(n => n.Code == code);

        if (notification_data == null)
        {
            notification_data = new NotificationData();
            notification_data.Code = code;
            this.ListNotificationDatas.Add(notification_data);
        }

        notification_data.PushTime = push_time;
        PlayerPrefs.SetString("LocalNotificationDatas", JsonMapper.ToJson(this.ListNotificationDatas));
    }

    public void SetPushNotificationByDelay(NotificationCode code, int push_delay)
    {
        var push_time = System.DateTime.Now + new System.TimeSpan(0, 0, push_delay);
        this.SetPushNotificationByTime(code, push_time);
    }

    public void ClearAllNotifications()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        AndroidNotificationCenter.CancelAllScheduledNotifications();
        AndroidNotificationCenter.CancelAllNotifications();
#elif UNITY_IOS
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
        iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif
        //this.NotificationID = 0;

        //#if LOCAL_NOTIFICATION
        //#if UNITY_ANDROID
        //        //AndroidNotifications.cancelAll();
        //        //AndroidNotifications.clearAll();
        //        LocalNotification.ClearNotifications();

        //        for (int i = 0; i < this.ListNotificationDatas.Count; i++)
        //        {
        //            LocalNotification.CancelNotification(this.ListNotificationDatas[i].ID);
        //        }
        //#elif UNITY_IOS
        //		UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
        //        UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
        //#endif
        //#endif
    }

    public DateTime GetPushTimeByHabit(int wait_day, bool round)
    {
        var push_time = DateTime.Now.AddDays(wait_day);
        var hour_login_habit = 0;

        if (LoginHistoryData == null) LoadData();

        if (push_time.DayOfWeek == DayOfWeek.Saturday || push_time.DayOfWeek == DayOfWeek.Sunday)
        {
            var have_login_habit_data = this.LoginHistoryData.WeekendHistory[0] > 0;

            for (int i = 0; i < this.LoginHistoryData.WeekendHistory.Count; i++)
            {
                if (this.LoginHistoryData.WeekendHistory[i] > this.LoginHistoryData.WeekendHistory[hour_login_habit])
                {
                    hour_login_habit = i;
                    have_login_habit_data = true;
                }
            }

            if (!have_login_habit_data)
            {
                return push_time;
            }
        }
        else
        {
            var have_login_habit_data = this.LoginHistoryData.NormalHistory[0] > 0;

            for (int i = 0; i < this.LoginHistoryData.NormalHistory.Count; i++)
            {
                if (this.LoginHistoryData.NormalHistory[i] > this.LoginHistoryData.NormalHistory[hour_login_habit])
                {
                    hour_login_habit = i;
                }
            }

            if (!have_login_habit_data)
            {
                return push_time;
            }
        }

        return new DateTime(push_time.Year,
            push_time.Month,
            push_time.Day,
            hour_login_habit,
            round ? 0 : push_time.Minute,
            round ? 0 : push_time.Second,
            System.DateTimeKind.Local);
    }
#if UNITY_IOS
    IEnumerator RequestAuthorization()
    {
        using (var req = new Unity.Notifications.iOS.AuthorizationRequest(
            Unity.Notifications.iOS.AuthorizationOption.Alert | 
            Unity.Notifications.iOS.AuthorizationOption.Badge,
            true))
        {
            while (!req.IsFinished)
            {
                yield return null;
            };

#if DEBUG
            string res = "\n RequestAuthorization: \n";
            res += "\n finished: " + req.IsFinished;
            res += "\n granted :  " + req.Granted;
            res += "\n error:  " + req.Error;
            res += "\n deviceToken:  " + req.DeviceToken;
            Debug.Log(res);
#endif
            if (string.IsNullOrEmpty(req.Error) && string.IsNullOrEmpty(req.DeviceToken) == false)
            {
                AppsFlyeriOS.registerUninstall(System.Text.Encoding.UTF8.GetBytes(req.DeviceToken));
            }
        }
    }
#endif
}

public enum NotificationCode
{
    Comeback,
    ChestLegend,
    FirstAid
}

[Beebyte.Obfuscator.Skip]
public class NotificationData
{
    public int ID = -1;
    public NotificationCode Code { get; set; }
    public DateTime PushTime { get; set; }

    public void Push()
    {
        //if (GameClient.instance.gameData.user.CheckBlockPushNotification(this.Code)) return;
        //if (this.PushTime == null) return;
        var delay = this.PushTime - System.DateTime.Now;
        if (delay.TotalMilliseconds <= 0) return;
        var title = Localization.Get(string.Format("Notification_{0}_Title", this.Code.ToString()));
        var message = Localization.Get(string.Format("Notification_{0}_Message", this.Code.ToString()));

#if UNITY_ANDROID
        var notify = new AndroidNotification()
        {
            Title = title,
            Text = message
        };

        notify.SmallIcon = "icon_notify_small";
        notify.LargeIcon = "icon_notify";

        notify.FireTime = PushTime;

        ID = AndroidNotificationCenter.SendNotification(notify, "Default");

#elif UNITY_IOS
        var notifyIos = new Unity.Notifications.iOS.iOSNotification();
        notifyIos.Identifier = "Notify" + Code.ToString();
        notifyIos.Title = title;
        notifyIos.Body = message;
        notifyIos.ShowInForeground = false;
        notifyIos.CategoryIdentifier = "Default";
        notifyIos.ThreadIdentifier = "Default";
        notifyIos.Trigger = new Unity.Notifications.iOS.iOSNotificationCalendarTrigger
        {
            Year = PushTime.Year,
            Month = PushTime.Month,
            Day = PushTime.Day,
            Hour = PushTime.Hour,
            Minute = PushTime.Minute,
            Second = PushTime.Second,
            Repeats = false
        };
        Unity.Notifications.iOS.iOSNotificationCenter.ScheduleNotification(notifyIos);
#endif
        //#if LOCAL_NOTIFICATION
        //#if UNITY_ANDROID
        //        /*var flags = 0;
        //		flags |= NotificationBuilder.DEFAULT_ALL;

        //		var builder = new NotificationBuilder(notification_id, title, message);
        //		builder.setDefaults(flags)
        //			.setDelay((long)delay.TotalMilliseconds)
        //			.setAutoCancel(true)
        //			.setSmallIcon("notify_icon");

        //		AndroidNotifications.scheduleNotification(builder.build());*/
        //        if (this.ID >= 0)
        //        {
        //            LocalNotification.CancelNotification(this.ID);
        //        }
        //        this.ID = LocalNotification.SendNotification(delay, title, message,
        //            //new Color32(0xff, 0x44, 0x44, 255)
        //            new Color32(0x45, 0x7f, 0xf1, 0xff)
        //            );
        //#elif UNITY_IOS
        //		var notification = new UnityEngine.iOS.LocalNotification();
        //        notification.fireDate = this.PushTime;
        //        notification.alertBody = message;
        //        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notification);
        //#endif
        //#endif
    }
}

[Beebyte.Obfuscator.Skip]
public class LoginHistoryData
{
    public List<int> NormalHistory { get; set; }
    public List<int> WeekendHistory { get; set; }

    public LoginHistoryData()
    {
        this.NormalHistory = new List<int>();
        this.WeekendHistory = new List<int>();

        for (int i = 0; i < 24; i++)
        {
            this.NormalHistory.Add(0);
            this.WeekendHistory.Add(0);
        }
    }
}