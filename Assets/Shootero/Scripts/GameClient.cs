using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DateTime = System.DateTime;
using TimeSpan = System.TimeSpan;

using Hiker.Networks.Data;
using Hiker.Networks.Data.Shootero;
using Hiker.GUI.Shootero;
using Hiker.GUI;
using System.Linq;

public partial class GameClient : HTTPClient
{
    public enum TargetServer
    {
        LOCAL,
        PRIVATE_CLOUD,
        AMAZON
    }
    public static GameClient instance;
    public TargetServer targetServer;
    public QuanlyNguoichoi playerManager;
    public const int GameVersion = 1155;

    public static string GetGameVersionString()
    {
        if (GameVersion > 999)
        {
            return string.Format("v{0}.{1}.{2}", GameVersion / 1000, (GameVersion % 1000) / 10, GameVersion % 10);
        }

        return string.Format("v{0}.{1}.{2}", GameVersion / 100, (GameVersion % 100) / 10, GameVersion % 10);
    }
    public string GetConfigVersionString()
    {
        if (string.IsNullOrEmpty(configVersion)) return string.Empty;
        var sp = configVersion.Split('.');
        return sp[0];
    }

    public string configVersion;
    public string localConfigVersion;
    public string platform;

    public long ServerTimeDiffTick { get; private set; }
    public long GID;
    public string AccessToken { get; private set; }

    public UserInfo UInfo { get; set; }

    public bool OfflineMode = true;

    public DateTime ServerTime
    {
        get
        {
            return GetServerTime(TimeUtils.Now);
        }
    }

    public DateTime GetServerTime(DateTime clientTime)
    {
        return (clientTime + new TimeSpan(ServerTimeDiffTick));
    }

    public DateTime LastTimeUpdateInfo { get; private set; }
    public bool isDownloadConfigBundleFailed { get; set; }

    HikerKiemTraGiayPhepUngDung mLicenseChecker;

    void Awake()
    {
        if (instance != null)
        {
            //GameClient.instance.gameData = null;
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        GID = 0;
        //default target to amazon

        mLicenseChecker = GetComponent<HikerKiemTraGiayPhepUngDung>();

        CodeStage.AntiCheat.Storage.ObscuredPrefs.lockToDevice =
            CodeStage.AntiCheat.Storage.ObscuredPrefs.DeviceLockLevel.Strict;

        //ConfigManager.LoadConfigs();
        //#if UNITY_ANDROID || UNITY_IOS
        //        Application.targetFrameRate = 60;
        //#endif

#if UNITY_ANDROID
        platform = "android";
#elif UNITY_IOS
        platform = "ios";
#else
        platform = "windows";
#endif

        string url = PlayerPrefs.GetString("SERVER_URL");
        if (string.IsNullOrEmpty(url))
        {
            //var versionUrl = "localhost:54910";
            // version 104,106,111,130,150,170,182,183,191,192,196,1110
            //var versionUrl = "acardehunter-env1.vezwhiurms.us-west-2.elasticbeanstalk.com";
            // version 102,1120,1140,1153
            //var versionUrl = "arcadehunter-env2.us-west-2.elasticbeanstalk.com";
            // version 105,110,120,140,160,180,181,190,193,194,195,1100,1111,1130,1150,1153
            var versionUrl = "arcadehunter-env3.us-west-2.elasticbeanstalk.com";

            url = "http://" + versionUrl + "/Game/game.asmx/";

            switch (this.targetServer)
            {
                case TargetServer.LOCAL:
                    url = url.Replace(versionUrl, "localhost:54910");
                    url = url.Replace(@"https://", @"http://");
                    break;
                case TargetServer.PRIVATE_CLOUD:
                    //this.URL = this.URL.Replace(versionUrl, "192.168.1.5:54910");
                    url = url.Replace(versionUrl, "34.87.117.98:54910");
                    break;
                //case TargetServer.CHINA:
                //    this.URL = this.URL.Replace(versionUrl, "dev.caravan.teebikgame.com");
                //    break;
                //case TargetServer.CHINA_TEST:
                //    this.URL = this.URL.Replace(versionUrl, "120.132.95.32");
                //    break;
                //case TargetServer.KOREA:
                //    this.URL = this.URL.Replace(versionUrl, "120.132.95.32");
                //    break;
                //case TargetServer.KOREA_TEST:
                //    this.URL = this.URL.Replace(versionUrl, "211.252.86.106");
                //    break;
                default:
                    break;
            }
        }

        OverrideURL(url);
        DontDestroyOnLoad(this.gameObject);
    }

    public void OverrideURL(string url)
    {
        if (url.StartsWith("http://") || url.StartsWith("https://"))
        {
            this.URL = url;
            Debug.Log(this.URL);
        }
    }

    private void OnEnable()
    {
        UInfo = new UserInfo();
    }

    private void Start()
    {
        TestRequest();

        if (OfflineMode)
        {
            ConfigManager.LoadConfigs_Client();
            playerManager.gameObject.SetActive(true);
        }
        else
        {
            Hiker.GUI.GUIManager.Instance.SetScreen("Loading");

            if (mLicenseChecker && Application.internetReachability != NetworkReachability.NotReachable)
            {
#if UNITY_ANDROID
                mLicenseChecker.BatDauKiemTra();
#endif
            }
        }
    }

    public void UpdateUserInfo(UserInfo other)
    {
        if (other != null)
        {
            GameClient.instance.ServerTimeDiffTick = other.ServerTimeTick - TimeUtils.Now.Ticks;
            LastTimeUpdateInfo = TimeUtils.Now;
        }

        // Re-enable notify shop
        if (UInfo != null &&
            UInfo.ListOffers != null &&
            other != null && other.ListOffers != null
            )
        {
            var curAvai = UInfo.ListOffers.FindAll(e => e.BuyCount < e.StockCount && e.ExpireTime > ServerTime && e.StockCount > 0);
            var nexAvai = other.ListOffers.FindAll(e => e.BuyCount < e.StockCount && e.ExpireTime > ServerTime && e.StockCount > 0);
            if (nexAvai.Exists(e => curAvai.Exists(f => f.ID == e.ID) == false))
            {
                PlayerPrefs.SetInt("NotifyShop" + GameClient.instance.GID, 0);
            }
        }

        UInfo.Update(other);
        //        for (int i = UInfo.Rewards.Count - 1; i >= 0; --i)
        //        {
        //            var re = UInfo.Rewards[i];
        //            if (re.Chests != null && re.Chests.Length > 0)
        //            {
        //                for (int c = re.Chests.Length - 1; c >= 0; --c)
        //                {
        //                    var chest = re.Chests[c];
        //                    if (chest.Count > 0)
        //                    {
        //#if UNITY_EDITOR
        //                        Debug.Log("Have reward");
        //#endif
        //                        listRewards.Add(chest);
        //                    }
        //                }
        //            }

        //            if (re.Count > 0)
        //            {
        //#if UNITY_EDITOR
        //                Debug.Log("Have reward");
        //#endif
        //                listRewards.Add(re);
        //            }

        //            UInfo.Rewards.RemoveAt(i);
        //        }

        listRewards = UInfo.Rewards;

        OnUpdateUserInfo();
    }

    public bool CheckRewardInfo()
    {
        if (listRewards.Count > 0)
        {
            var lastReward = listRewards[listRewards.Count - 1];
#if UNITY_EDITOR
            Debug.Log("Display reward");
#endif
            if (!string.IsNullOrEmpty(lastReward.Source))
            {
                //chest
                if (ConfigManager.ChestCfg.ContainsKey(lastReward.Source))
                {
                    PopupBuyChest.Create(lastReward, lastReward.Source);
                }
                //mo card
                else if (lastReward.Source.StartsWith("Card_"))
                {
                    PopupOpenCard.Create(lastReward);
                }
                //battle
                else if(lastReward.Source == "Battle")
                {
                    GeneralReward cardReward = new GeneralReward();
                    foreach (string rwKey in lastReward.Keys)
                    {
                        if (rwKey.StartsWith("C_"))
                        {
                            cardReward.AddReward(rwKey, lastReward[rwKey]);
                            break;
                        }
                    }
                    
                    if (cardReward.Count > 0)
                    {
                        lastReward.Clear();
                        PopupOpenCard.Create(cardReward);
                    }
                }
                else
                {
                    PopupOpenChest_new.Create(lastReward);
                }
            }
            else
            {
                PopupOpenChest_new.Create(lastReward);
            }

            listRewards.RemoveAt(listRewards.Count - 1);
            return true;
        }

        return false;
    }

    string chestName;
    List<GeneralReward> listRewards = new List<GeneralReward>();

    public void OnUpdateUserInfo()
    {
        var networkData = NetworkDataSync.ListNetworkData.ToArray();
        for (int i = networkData.Length - 1; i >= 0; --i)
        {
            var g = networkData[i];
            if (g != null)
            {
                g.SyncNetworkData();
            }
        }

        if (GUIManager.Instance &&
            GUIManager.Instance.CurrentScreen == "Main") 
        {
            CheckRewardInfo();
        }
        else if (
            GUIManager.Instance &&
            GUIManager.Instance.CurrentScreen == "Battle" &&
            PopupBuyGem.Instance != null &&
            PopupBuyGem.Instance.gameObject.activeSelf)
        {
            CheckRewardInfo();
        }
    }

    public List<OfferStoreData> GetListOffers()
    {
        if (UInfo == null || UInfo.ListOffers == null) return new List<OfferStoreData>();

        var result = UInfo.ListOffers.FindAll(e =>
            e.Type == OfferType.FlashSale &&
            e.BuyCount < e.StockCount &&
            e.ExpireTime > GameClient.instance.ServerTime);

        var listkeyOffers = CashShopUtils.GetFlashSaleOfferConfigs().Keys.ToList();

        result.Sort((e1, e2) => listkeyOffers.IndexOf(e2.PackageName) - listkeyOffers.IndexOf(e1.PackageName));
        return result;
    }

    public List<OfferStoreData> GetListLimitedTimeOffers()
    {
        if (UInfo == null || UInfo.ListOffers == null) return new List<OfferStoreData>();

        var result = UInfo.ListOffers.FindAll(e =>
            e.Type == OfferType.LimitedTime  &&
            e.BuyCount < e.StockCount &&
            e.ExpireTime > GameClient.instance.ServerTime);

        //var listkeyOffers = CashShopUtils.GetFlashSaleOfferConfigs().Keys.ToList();

        //result.Sort((e1, e2) => listkeyOffers.IndexOf(e2.PackageName) - listkeyOffers.IndexOf(e1.PackageName));
        return result;
    }

    public List<OfferStoreData> GetListDailyOffers()
    {
        if (UInfo == null || UInfo.ListOffers == null) return new List<OfferStoreData>();
        if (UInfo.GetTotalBattle() < 5) return new List<OfferStoreData>();
        var result = UInfo.ListOffers.FindAll(e =>
            e.Type == OfferType.DailyOffer);

        var listkeyOffers = CashShopUtils.GetFlashSaleOfferConfigs().Keys.ToList();

        result.Sort((e1, e2) => listkeyOffers.IndexOf(e2.PackageName) - listkeyOffers.IndexOf(e1.PackageName));
        return result;
    }

    public static string GetHashStringListOffers(List<OfferStoreData> listOffers)
    {
        if (listOffers == null || listOffers.Count ==  0) return string.Empty;
        List<string> listResult = new List<string>();
        foreach (var offer in listOffers)
        {
            if (offer != null)
            {
                listResult.Add(Md5Hash.GetMd5Hash(LitJson.JsonMapper.ToJson(offer)));
            }
        }
        return LitJson.JsonMapper.ToJson(listResult);
    }

    public static bool CompareHashListMd5(string oldHash, string newHash)
    {
        if (oldHash == newHash) return true;
        if (string.IsNullOrEmpty(oldHash)) return false;
        if (string.IsNullOrEmpty(newHash)) return true;

        var list1 = LitJson.JsonMapper.ToObject<List<string>>(oldHash);
        var list2 = LitJson.JsonMapper.ToObject<List<string>>(newHash);
        if (list1.Count < list2.Count) return false; // if new hash have more offer item
        //foreach (var p1 in list1)
        //{
        //    if (list2.Contains(p1) == false) return false;
        //}
        foreach (var p2 in list2)
        {
            if (list1.Contains(p2) == false) return false; // have one item is new
        }
        return true;
    }

    void LogOut()
    {
        
    }

    IEnumerator CoRestartGame()
    {
        PopupNetworkLoading.Create(Localization.Get("RestartGame"));
        if (playerManager.gameObject.activeInHierarchy)
        {
            yield return StartCoroutine(playerManager.CoCleanUp());
            yield return StartCoroutine(playerManager.CoBackToMain(true));
        }
        playerManager.gameObject.SetActive(false);
        LogOut();
        PopupManager.instance.ClearAll();
        GUIManager.Instance.ClearScreen();
        ConfigManager.loaded = false;
        Hiker.GUI.GUIManager.Instance.SetScreen("Loading");

        ScreenBattle.PauseGame(false);
    }

    public void RestartGame()
    {
        StartCoroutine(CoRestartGame());
    }

    public void ShowInvalidInstallPopup(string url)
    {
        //client need upgrade
        PopupConfirm.Create(Localization.Get("client_invalid_desc"), () =>
        {
            Debug.Log(url);
            Application.OpenURL(url);
        }, false);
    }

    public void ShowGameVersionUpdadePopup(string url)
    {
        //client need upgrade
        PopupConfirm.Create(Localization.Get("client_upgrade_notify_desc"), () =>
        {
            Debug.Log(url);
            Application.OpenURL(url);
        }, false, "", Localization.Get("client_upgrade_notify_title"));
    }

    public void ShowConfigVersionUpdatePopup()
    {
        //client need upgrade
        PopupConfirm.Create(Localization.Get("config_upgrade_notify_desc"), () =>
        {
            RestartGame();
        }, false, string.Empty, Localization.Get("config_upgrade_notify_title"));
    }

    public void ShowPopupMessageAndQuit(string msg)
    {
        //client need upgrade
        PopupConfirm.Create(msg, () =>
        {
            Application.Quit();
        });
    }

    public void WaitUserPermissionIDFA(System.Action onComplete)
    {
        Debug.Log("WaitUserPermissionIDFA");
        StartCoroutine(CoWaitUserPermissionIDFA(onComplete));
    }
    IEnumerator CoWaitUserPermissionIDFA(System.Action onComplete)
    {
        yield return new WaitUntil(() => PopupAppTracking.GetAppTrackingTransparencySetting() != 0);
        Debug.Log("ATT Value = " + PopupAppTracking.GetAppTrackingTransparencySetting());
        onComplete?.Invoke();
    }

    public void LoginToServer()
    {
#if IRON_SOURCE
        if (IronSourceAdsManager.instance)
            IronSourceAdsManager.instance.InitIronSource();
#endif
        if (Hiker.GUI.Shootero.ScreenLoading.Instance && Hiker.GUI.Shootero.ScreenLoading.Instance.gameObject.activeSelf)
        {
            Hiker.GUI.Shootero.ScreenLoading.Instance.LoginToServer();
        }
    }

    public void CheckATTThenLogin()
    {
#if UNITY_IOS
        if (PopupAppTracking.NeedRequirePermisionTracking())
        {
            GameClient.instance.WaitUserPermissionIDFA(GameClient.instance.LoginToServer);
            PopupAppTracking.RequestAuthorizeTracking();
            return;
        }
#endif
        GameClient.instance.LoginToServer();
    }
}
