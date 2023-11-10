using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Beebyte.Obfuscator;
using UnityEngine.Networking;
using LitJson;
using Hiker.Networks.Data.Shootero;
using Hiker.GUI.Shootero;

public class LoaderBundleManager : MonoBehaviour
{
    public static LoaderBundleManager instance;
    public string BundleURL;
    public int Version;
    public string storeURL;
    private static Dictionary<string, uint> CRCs = new Dictionary<string, uint>();
    // private static Dictionary<string, Object> assetsDict = new Dictionary<string, Object>();
    private static Dictionary<string, System.Action> endCallBacks = new Dictionary<string, System.Action>();
    private static List<string> nameBundlesLoaded = new List<string>();

    /// <summary>
    /// is loading config bundle
    /// </summary>
    public static bool IsLoading = false;
    /// <summary>
    /// -1 is error
    /// 0 is have not loaded 
    /// 1 is success
    /// </summary>
    public static int IsLoaded = 0;
    public float progress;
    float delayMicTime = 0.2f;
    private AssetBundle bundles;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    [SkipRename]
    public IEnumerator DownloadAssetBundle(string folder, string bundleName, System.Action callBack)
    {
        //uncmt this if want to test downloading bundle
#if UNITY_EDITOR
        // Caching.CleanCache();
#endif
        yield return new WaitForSecondsRealtime(delayMicTime);

        endCallBacks[bundleName] = callBack;



        // if (!nameBundlesLoaded.Exists(v => v == bundleName))
        //{
        //down load new update from server
        //if (SplashScreen.Instance.lblTip != null)
        //    SplashScreen.Instance.lblTip.text = Localization.Get("loading_game_configs");
        if (Hiker.GUI.Shootero.ScreenLoading.Instance.lblTip != null)
        {
            Hiker.GUI.Shootero.ScreenLoading.Instance.lblTip.text = Localization.Get("loading_game_configs");
        }
        yield return StartCoroutine(this.DownloadAndCache(folder, bundleName));

        //}
        //else
        //{
        //    //loaded from cache
        //    this.HandlerCallback(bundleName, false);
        //}
    }

    [SkipRename]
    private IEnumerator DownloadAndCache(string folder, string bundleName)
    {
        progress = 50;
        // Wait for the Caching system to be ready
        while (!Caching.ready)
            yield return null;

        if (string.IsNullOrEmpty(BundleURL))
        {
            if (GameClient.instance)
            {
                GameClient.instance.RestartGame();
                yield break;
            }
        }

        string url = string.Format("{0}/{1}/{2}", this.BundleURL, folder, bundleName);
        uint crc = 0; if (CRCs != null && CRCs.ContainsKey(bundleName)) crc = CRCs[bundleName];
        // Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
        float time = Time.realtimeSinceStartup;
#if DEBUG
        Debug.Log(url);
#endif
        using (var www = UnityWebRequestAssetBundle.GetAssetBundle(url, (uint)this.Version, crc))
        {
            www.timeout = 30;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                this.HandlerCallback(bundleName, false);
                if (string.IsNullOrEmpty(www.error) == false)
                    Debug.Log(www.error);

                //www.Dispose();
                //yield break;
            }
            else
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
                if (bundle != null)
                {
#if DEBUG
                    Debug.Log(bundle + " " + bundleName);
                    Debug.Log(Time.realtimeSinceStartup - time);
#endif
                    AssetBundleRequest bundleRequest = bundle.LoadAllAssetsAsync();
                    yield return bundleRequest;
                    this.OnLoadedSuccess(bundle, bundleName);
                    // config is loaded in callback from OnLoadedSuccess -> unload config bundle here
                    if (bundle != null)
                        bundle.Unload(false);
                }
                else
                {
                    this.HandlerCallback(bundleName, false);
                }
            }
        }
    }

    public void OnLoadedSuccess(AssetBundle bundles, string bundleName)
    {
        // if (!nameBundlesLoaded.Exists(v => v == bundles.name)) nameBundlesLoaded.Add(bundles.name);
        /*  foreach (var item in bundles.LoadAllAssets())
          {
             assetsDict[item.name] = item;
          } */
        this.bundles = bundles;

        this.HandlerCallback(bundleName, true);
    }

    private void HandlerCallback(string bundleName, bool success)
    {
        if (endCallBacks.ContainsKey(bundleName))
        {

            endCallBacks[bundleName]?.Invoke();
        }
        progress = 100;
        IsLoaded = success ? 1 : -1;

        if (success)
        {
            AnalyticsManager.LogEvent("LOAD_CONFIG_BUNDLE_SUCCESS");
        }
        else
        {
            GameClient.instance.isDownloadConfigBundleFailed = true;
            AnalyticsManager.LogEvent("LOAD_CONFIG_BUNDLE_FAILED");
        }
    }

    public TextAsset GetAssetFileConfig(string fileName)
    {
        if (this.bundles == null) return null;
        int index = fileName.LastIndexOf('/');
        if (index > 0)
        {
            fileName = fileName.Substring(index + 1);
        }
        if (!this.bundles.Contains(fileName)) return null;
        Object obj = this.bundles.LoadAsset(fileName);
        if (obj is TextAsset) return (obj as TextAsset);
        return null;
    }

    public GameObject GetAssetGameObject(string objName)
    {
        if (this.bundles == null) return null;
        if (!this.bundles.Contains(objName)) return null;
        Object obj = this.bundles.LoadAsset(objName);
        if (obj is GameObject) return (obj as GameObject);
        return null;
    }

    [SkipRename]
    public IEnumerator OnLoadedBundleConfig(string data)
    {
#if DEBUG
        Debug.Log(data);
#endif
        yield return new WaitForSecondsRealtime(delayMicTime);
        AssetBundleConfigResponse response = JsonMapper.ToObject<AssetBundleConfigResponse>(data);
        //Debug.Log(response.data);
        JsonData json = JsonMapper.ToObject(response.data);

#if UNITY_ANDROID
        if (json.Contains("androidStore"))
            this.storeURL = json["androidStore"].ToString();
#elif UNITY_IOS
        if (json.Contains("iosStore"))
            this.storeURL = json["iosStore"].ToString();
#else
		if (json.Contains("androidStore"))
			this.storeURL = json["androidStore"].ToString();
#endif
        //Debug.Log(this.storeURL);
        if (response.version > GameClient.GameVersion)
        {
            //client need upgrade
            //Debug.Log(response.version + " " + GameClient.GameVersion);
            GameClient.instance.ShowGameVersionUpdadePopup(storeURL);
            yield break;
        }

        this.BundleURL = json["url"].ToString();
        //Debug.Log(this.BundleURL);
        this.Version = GameClient.GameVersion;
        if (json.Contains("version"))
        {
            string jsVer = json["version"].ToString();
            //Debug.Log(jsVer);
            if (int.TryParse(jsVer, out Version) == false)
            {
                this.Version = GameClient.GameVersion;
            }
        }
        string platform = GameClient.instance.platform;

#if (DOWNLOAD_AB && UNITY_EDITOR) || ((UNITY_ANDROID || UNITY_IOS) && !IGNORE_AB)
        //string assetbundleURL = json["mappackurl"].ToString();
        //AssetBundleLoader.Initialization(assetbundleURL);
#endif
        var haveTos = Hiker.GUI.PopupToS.GetTOSPref();
        if (ScreenLoading.Instance && haveTos == 0)
        {
            AnalyticsManager.LogEvent("LOADED_ASSETBUNDLE_CONFIG");
        }

        //Debug.Log("Try to download config bundle of platform : " + platform);
        if (!string.IsNullOrEmpty(platform) && json.Contains(platform))
        {
            JsonData platformCrcs = json[platform];
            CRCs.Clear();
            //string[] flatforms = new string[platformCrcs.Count];
            //platformCrcs.GetKeys().CopyTo(flatforms, 0);
            string bundlerFileName = "";
            //for (int i = 0; i < flatforms.Length; i++)
            //Debug.Log(platform);
            foreach (var b in platformCrcs.GetKeys())
            {
                string bundleName = b as string;
                CRCs[bundleName] = (uint)(platformCrcs[bundleName].ToInt());
                bundlerFileName = bundleName;
            }
            Debug.Log(platform + " " + bundlerFileName);

            GameClient.instance.configVersion = bundlerFileName;
            //GameClient.instance.platform = platform;
            progress = 30;
            // string ConfigBundleName = string.Format("{0}/{1}/{2}", this.BundleURL, platform, bundlerFileName);
            yield return StartCoroutine(this.DownloadAssetBundle(platform, bundlerFileName, () =>
            {
                //apply new configs
                if (Hiker.GUI.Shootero.ScreenLoading.Instance.lblTip != null)
                {
                    Hiker.GUI.Shootero.ScreenLoading.Instance.lblTip.text = Localization.Get("loading_game_configs");
                }
                ConfigManager.LoadConfigs_Client();
                //Localization.LoadFromBundles();

                GameClient.instance.localConfigVersion = (string)ConfigManager.assetBundleConfig[platform].GetFirstKey();

                AnalyticsManager.LogEvent("LOADED_GAME_CONFIG");
#if IAP_BUILD
                //UnityIAPManager.Recreate();
#endif
            }));
        }
        else
        {
            Debug.LogError("Cant parse Bundle Config Data");
        }
    }
    bool ignoreBundle;

    public void LoadBundlesConfig()
    {
        Debug.Log("load bundle");
        IsLoading = true;
        progress = 0;

        ignoreBundle = false;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || TEST_BUILD

        //ignore bundle for test
        ignoreBundle = true;
        //GameClient.instance.platform = "android";
        //Caching.CleanCache();
#endif

#if LOCAL_BUNDLE
        ignoreBundle = true;
#endif

        if (ignoreBundle)
        {
            Debug.Log("Load Local Configs");
            IsLoading = true;
            IsLoaded = 1;
            ConfigManager.LoadConfigs_Client();

            GameClient.instance.configVersion = (string)ConfigManager.assetBundleConfig[GameClient.instance.platform].GetFirstKey();
            GameClient.instance.localConfigVersion = GameClient.instance.configVersion;

#if (DOWNLOAD_AB && UNITY_EDITOR) || ((UNITY_ANDROID || UNITY_IOS) && !IGNORE_AB)
            //string assetbundleURL = ConfigManager.assetBundleConfig["mappackurl"].ToString();
            //AssetBundleLoader.Initialization(assetbundleURL);
#endif
        }
        else
        {
            GameClient.instance.RequestAssetBundlesInfo(
                (data) =>
                {
                    if (data == null)
                    {
                        Debug.LogError("load bundle fail");

                        OnFailedRequestAssetBundleConfig();
                        return;
                    }
#if DEBUG
                    Debug.Log("AssetBundleConfig : " + data);
#endif
                    PlayerPrefs.SetString("AssetBundleConfigURL", data);

                    StartCoroutine(this.OnLoadedBundleConfig(data));
                },
                (code) =>
                {
                    OnFailedRequestAssetBundleConfig();
                });
        }
    }

    void OnFailedRequestAssetBundleConfig()
    {
        AnalyticsManager.LogEvent("FAILED_ASSETBUNDLE_CONFIG");

        string lastCfgResponse = PlayerPrefs.GetString("AssetBundleConfigURL", string.Empty);
        //Debug.Log("OnFailedRequestAssetBundleConfig");
        if (string.IsNullOrEmpty(lastCfgResponse))
        {
            AssetBundleConfigResponse response = new AssetBundleConfigResponse();
            response.data = ConfigManager.ReadConfig("AssetBundlesCfg").ToJson();
            response.version = GameClient.GameVersion;

            lastCfgResponse = JsonMapper.ToJson(response);
        }

        StartCoroutine(this.OnLoadedBundleConfig(lastCfgResponse));
    }
}
