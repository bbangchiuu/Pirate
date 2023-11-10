using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Android;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;

public static class HikerBuildEditor
{
#if UNITY_IOS
    #region iOS Apple Config
    //const string AppleDeveloperTeamID = "68ZNNV3262";
    const string AppleDeveloperTeamID = "G3C8DQ223H"; // ABI Team
    //const string DevelopProvision = "0cbcd261-57d1-4049-9805-35aa40878451"; // Hiker Dev
    const string DevelopProvision = "3d858c27-ce41-4fad-8ad9-3582274d9131"; // ABI DEV
    //const string AdhocProvision = "5913f69b-1ad1-454a-b7a0-16c433bc2a6b";
    //const string StoreProvision = "235cd04a-c551-431c-8a61-ffa15c222cb4";
    #endregion
#endif
    static string GetProjPath()
    {
        string projPath = Application.dataPath;
        projPath = projPath.Substring(0, projPath.Length - 7);
        return projPath;
    }

#if UNITY_ANDROID
    #region BuildAndroid

    const string defaultApkName = "shootero";

    [MenuItem("Hiker/Build/Build Android Apk")]
    static public void BuildPlayerApk()
    {
        string projPath = GetProjPath();
        string sr = EditorUtility.SaveFilePanel(
            "Build Android Apk",
            projPath,
            defaultApkName,
            "apk");

        EditorUserBuildSettings.buildAppBundle = false;
        //OPS.Obfuscator.BuildPostProcessor.ManualRestore();
        BuildAndroid(projPath, sr,
            BuildOptions.StrictMode,
            true);
        //OPS.Obfuscator.BuildPostProcessor.ManualRestore();
    }

    [MenuItem("Hiker/Build/Build Android App Bundle")]
    static public void BuildPlayerAab()
    {
        string projPath = GetProjPath();
        string sr = EditorUtility.SaveFilePanel(
            "Build Android Apk",
            projPath,
            defaultApkName,
            "aab");

        EditorUserBuildSettings.buildAppBundle = true;
        
        BuildAndroid(projPath, sr,
            BuildOptions.StrictMode,
            true);
    }

    [MenuItem("Hiker/Build/Build Debug Android Apk")]
    static public void BuildDebugPlayerApk()
    {
        string projPath = GetProjPath();
        string sr = EditorUtility.SaveFilePanel(
            "Build Android Apk",
            projPath,
            defaultApkName + "_dev",
            "apk");

        EditorUserBuildSettings.buildAppBundle = false;
        BuildAndroid(projPath, sr,
            BuildOptions.StrictMode | BuildOptions.AllowDebugging | BuildOptions.Development,
            false);
    }

    static UnityEditor.Build.Reporting.BuildReport BuildAndroid(string projPath,
        string builtPath,
        BuildOptions buildOp,
        bool isIl2Cpp)
    {
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android){
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        }

        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android,
            isIl2Cpp ? ScriptingImplementation.IL2CPP : ScriptingImplementation.Mono2x);
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

        PlayerSettings.Android.keystoreName = projPath + "/hiker.keystore";
        PlayerSettings.Android.keystorePass = "eg7554";
        PlayerSettings.Android.keyaliasName = "shootero";
        PlayerSettings.Android.keyaliasPass = "eg7554";

        PlayerSettings.bundleVersion = GameClient.GetGameVersionString().Substring(1);
        //var splashScene = EditorBuildSettings.scenes[0];

        PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, out string[] symbolsScripts);
        List<string> symbolsDefine = new List<string>(symbolsScripts);
        if (symbolsDefine.Contains("ANTICHEAT"))
        {
            symbolsDefine.Add("ANTICHEAT");
        }

        string[] scenePaths = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenePaths.Length; ++i)
        {
            scenePaths[i] = EditorBuildSettings.scenes[i].path;
        }

        //var report = BuildPipeline.BuildPlayer(
        //    scenePaths,
        //    builtPath,
        //    BuildTarget.Android,
        //    buildOp
        //    );
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenePaths,
            locationPathName = builtPath,
            options = buildOp,
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android,
            extraScriptingDefines = symbolsDefine.ToArray(),
        });

        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            // build success
            Debug.LogFormat("Build apk success {0}", report.files[0].path);
        }
        else
        {
            Debug.LogFormat("Build apk failed result = {0}", report.summary.result.ToString());
        }

        return report;
    }

    static public void JenkinBuildApk()
    {
        string projPath = GetProjPath();

        var oldApk = System.IO.Directory.GetFiles(projPath, "*.apk");
        if (oldApk != null && oldApk.Length > 0)
        {
            foreach (var apk in oldApk)
            {
                System.IO.File.Delete(apk);
            }
        }

        var jenkinBuildNumber = System.Environment.GetEnvironmentVariable("BUILD_NUMBER");
        int bundleVersionCode = 0;

        if (int.TryParse(jenkinBuildNumber, out bundleVersionCode))
        {
            PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
        }

        //EditorUserBuildSettings.development = true;
        //EditorUserBuildSettings.allowDebugging = true;
        //EditorUserBuildSettings.androidDebugMinification = AndroidMinification.Gradle;

        EditorUserBuildSettings.buildAppBundle = false;

        string sr = string.Format("{0}/Build/{1}_b{2}.apk", projPath, defaultApkName, PlayerSettings.Android.bundleVersionCode);

        Debug.Log("Jenkin build number " + jenkinBuildNumber);
        Debug.Log("build player " + sr);

        var report = BuildAndroid(projPath, sr,
            BuildOptions.StrictMode | BuildOptions.CompressWithLz4HC,
            true);
        JenkinRestoreObfuscate();

        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("Build failed sumary: " + report.summary.result.ToString());
        }

        //catch (System.Exception e)
        //{
        //    throw new System.Exception(e.Message);
        //}
        //finally
        //{
        //    JenkinRestoreObfuscate();
        //}
    }

    static public void JenkinBuildApkScriptOnly()
    {
        string projPath = GetProjPath();

        var oldApk = System.IO.Directory.GetFiles(projPath, "*.apk");
        if (oldApk != null && oldApk.Length > 0)
        {
            foreach (var apk in oldApk)
            {
                System.IO.File.Delete(apk);
            }
        }

        //var jenkinBuildNumber = System.Environment.GetEnvironmentVariable("BUILD_NUMBER");
        //int bundleVersionCode = 0;

        //if (int.TryParse(jenkinBuildNumber, out bundleVersionCode))
        //{
        //    PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
        //}

        //EditorUserBuildSettings.development = true;
        //EditorUserBuildSettings.allowDebugging = true;
        //EditorUserBuildSettings.androidDebugMinification = AndroidMinification.Gradle;

        EditorUserBuildSettings.buildAppBundle = false;
        EditorUserBuildSettings.androidCreateSymbolsZip = false;

        string sr = string.Format("{0}/Build/{1}_b{2}.apk", projPath, defaultApkName, PlayerSettings.Android.bundleVersionCode);

        //Debug.Log("Jenkin build number " + jenkinBuildNumber);
        Debug.Log("build player " + sr);

        var report = BuildAndroid(projPath, sr,
            BuildOptions.StrictMode | BuildOptions.CompressWithLz4 | BuildOptions.BuildScriptsOnly,
            true);
        JenkinRestoreObfuscate();

        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("Build failed sumary: " + report.summary.result.ToString());
        }

        //catch (System.Exception e)
        //{
        //    throw new System.Exception(e.Message);
        //}
        //finally
        //{
        //    JenkinRestoreObfuscate();
        //}
    }

    //// Generate a random string with a given size  
    //public static string RandomString(int size, bool lowerCase)
    //{
    //    System.Text.StringBuilder builder = new System.Text.StringBuilder();
    //    Random random = new Random();
    //    char ch;
    //    for (int i = 0; i < size; i++)
    //    {
    //        ch = System.Convert.ToChar(Random.Range(0, 26) + 65);
    //        if (lowerCase && Random.Range(0, 100) < 50)
    //        {
    //            ch = char.ToLower(ch);
    //        }
    //        builder.Append(ch);
    //    }

    //    return builder.ToString();
    //}

    static public void JenkinBuildAab()
    {
        string projPath = GetProjPath();

        var oldApk = System.IO.Directory.GetFiles(projPath, "*.aab");
        if (oldApk != null && oldApk.Length > 0)
        {
            foreach (var apk in oldApk)
            {
                System.IO.File.Delete(apk);
            }
        }

        var jenkinBuildNumber = System.Environment.GetEnvironmentVariable("BUILD_NUMBER");
        int bundleVersionCode = 0;

        if (int.TryParse(jenkinBuildNumber, out bundleVersionCode))
        {
            PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
        }

        EditorUserBuildSettings.buildAppBundle = true;

        string sr = string.Format("{0}/Build/{1}_b{2}.aab", projPath, defaultApkName, PlayerSettings.Android.bundleVersionCode);

        Debug.Log("Jenkin build number " + jenkinBuildNumber);
        Debug.Log("build player " + sr);

        EditorUserBuildSettings.androidCreateSymbols = AndroidCreateSymbols.Debugging;
        EditorUserBuildSettings.il2CppCodeGeneration = UnityEditor.Build.Il2CppCodeGeneration.OptimizeSize;
        //EditorUserBuildSettings.androidCreateSymbolsZip = true;
        //EditorUserBuildSettings.androidReleaseMinification = AndroidMinification.Proguard;
        
        PlayerSettings.Android.buildApkPerCpuArchitecture = true;
        PlayerSettings.Android.useAPKExpansionFiles = true;

        PlayerSettings.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        PlayerSettings.SetStackTraceLogType(LogType.Assert, StackTraceLogType.None);
        PlayerSettings.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
        PlayerSettings.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);
        PlayerSettings.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);

        //OPS.Obfuscator.BuildPostProcessor.ManualRestore();

        var report = BuildAndroid(projPath, sr, BuildOptions.StrictMode | BuildOptions.CompressWithLz4HC, true);
        //OPS.Obfuscator.BuildPostProcessor.ManualRestore();
        JenkinRestoreObfuscate();

        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("Build failed sumary: " + report.summary.result.ToString());
        }
    }

    #endregion
#endif

#if UNITY_IOS
    #region IOS
    const string defaultIPAName = "shootero";
    static UnityEditor.Build.Reporting.BuildReport BuildiOS(string projPath, string builtPath, string[] scenes, BuildOptions op)
    {
        //var splashScene = EditorBuildSettings.scenes[0];

        var symbolScript = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
        if (symbolScript.Contains("ANTICHEAT") == false)
        {
            if (string.IsNullOrEmpty(symbolScript))
            {
                symbolScript = "ANTICHEAT";
            }
            else
            {
                symbolScript += ";ANTICHEAT";
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, symbolScript);
        }

        string[] scenePaths = null;
        if (scenes != null)
        {
            scenePaths = scenes;
        }
        else
        {
            scenePaths = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < scenePaths.Length; ++i)
            {
                scenePaths[i] = EditorBuildSettings.scenes[i].path;
            }
        }

        PlayerSettings.iOS.appleDeveloperTeamID = AppleDeveloperTeamID;
        PlayerSettings.iOS.appleEnableAutomaticSigning = false;
        PlayerSettings.iOS.iOSManualProvisioningProfileID = DevelopProvision;

        PlayerSettings.bundleVersion = GameClient.GetGameVersionString().Substring(1);

        var report = BuildPipeline.BuildPlayer(
            scenePaths,
            builtPath,
            BuildTarget.iOS,
            op);

        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            if (System.IO.File.Exists(builtPath + "/Unity-iPhone.xcodeproj"))
            {
                // build success
                Debug.LogFormat("Build ios success {0}", report.files[0].path);
            }
        }
        else
        {
            Debug.LogFormat("Build ios failed result = {0}", report.summary.result.ToString());
        }

        return report;
    }

    [MenuItem("Hiker/Build/Build iOS")]
    static public void BuildPlayerIOS()
    {
        string projPath = GetProjPath();

        string sr = EditorUtility.SaveFilePanel(
            "Build iOS",
            projPath,
            "boombattlefield_xcode",
            "");

        var op =
            BuildOptions.StrictMode | 
            BuildOptions.CompressWithLz4HC |
            BuildOptions.SymlinkLibraries;

        BuildiOS(projPath, sr, null, op);
    }

    //[MenuItem("Hiker/Build/Test Jenkin Build iOS")]
    static public void JenkinBuildiOS()
    {
        string projPath = GetProjPath();

        var jenkinBuildNumber = System.Environment.GetEnvironmentVariable("BUILD_NUMBER");
        string bundleVersion = GameClient.GetGameVersionString().Substring(1);
        PlayerSettings.bundleVersion = bundleVersion;
        PlayerSettings.iOS.buildNumber = bundleVersion + "." + jenkinBuildNumber;

        PlayerSettings.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        PlayerSettings.SetStackTraceLogType(LogType.Assert, StackTraceLogType.ScriptOnly);
        PlayerSettings.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
        PlayerSettings.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);
        PlayerSettings.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);

        string sr = string.Format("{0}/Build/{1}_b{2}_xcode", projPath, defaultIPAName, jenkinBuildNumber);

        Debug.Log("Jenkin build number " + jenkinBuildNumber);
        Debug.Log("build player " + sr);

        var op =
            BuildOptions.StrictMode | 
            BuildOptions.CompressWithLz4HC |
            BuildOptions.SymlinkLibraries;
        var report = BuildiOS(projPath, sr, null, op);
        
        JenkinRestoreObfuscate();

        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("Build failed sumary: " + report.summary.result.ToString());
        }
    }

    static public void JenkinBuildiOSScriptOnly()
    {
        string projPath = GetProjPath();

        var jenkinBuildNumber = System.Environment.GetEnvironmentVariable("BUILD_NUMBER");
        string bundleVersion = GameClient.GetGameVersionString().Substring(1);
        PlayerSettings.bundleVersion = bundleVersion;
        PlayerSettings.iOS.buildNumber = bundleVersion + "." + jenkinBuildNumber;

        PlayerSettings.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        PlayerSettings.SetStackTraceLogType(LogType.Assert, StackTraceLogType.ScriptOnly);
        PlayerSettings.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
        PlayerSettings.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);
        PlayerSettings.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);

        string sr = string.Format("{0}/Build/{1}_b{2}_xcode", projPath, defaultIPAName, jenkinBuildNumber);

        Debug.Log("Jenkin build number " + jenkinBuildNumber);
        Debug.Log("build player " + sr);

        var op =
            BuildOptions.BuildScriptsOnly |
            BuildOptions.StrictMode | 
            BuildOptions.CompressWithLz4HC |
            BuildOptions.SymlinkLibraries;

        var splashScene = EditorBuildSettings.scenes[0].path;

        var report = BuildiOS(projPath, sr, new string[] { splashScene },  op);
        
        JenkinRestoreObfuscate();

        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("Build failed sumary: " + report.summary.result.ToString());
        }
    }

    [PostProcessBuild(999)]
    public static void AddCapabilities(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            string projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";

            string entitlementName = "arcadehunter.entitlements";
            var targetName = PBXProject.GetUnityTargetName();
            ProjectCapabilityManager capMan = 
                new ProjectCapabilityManager(projPath,
                                             entitlementName,
                                             targetName);
            capMan.AddGameCenter();
            capMan.AddInAppPurchase();
            capMan.AddPushNotifications(true);

            capMan.WriteToFile();

            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            PlistElementDict rootDict = plist.root;
            rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);
            if (plist.root.values.ContainsKey("NSAppTransportSecurity"))
            {
                var ats = plist.root.values["NSAppTransportSecurity"].AsDict();
                if (ats != null)
                {
                    ats.values.Remove("NSAllowsArbitraryLoadsInWebContent");
                }
            }

            // Ironsource - Admob mediation network adapter (duongrs tich hop)
            //rootDict.SetString("GADApplicationIdentifier", "ca-app-pub-4792172123174060~7114204859");
            //rootDict.SetString("GADApplicationIdentifier", "ca-app-pub-9819920607806935~2404960158");
            var adNetworkItems = rootDict.CreateArray("SKAdNetworkItems");
            // ironsource adnetwork id
            var item = adNetworkItems.AddDict();
            item.SetString("SKAdNetworkIdentifier", "su67r6k2v3.skadnetwork");
            // admob
            item = adNetworkItems.AddDict();
            item.SetString("SKAdNetworkIdentifier", "cstr6suwn9.skadnetwork");
            // AppLovin
            item = adNetworkItems.AddDict();
            item.SetString("SKAdNetworkIdentifier", "ludvb6z3bs.skadnetwork");
            // Facebook Audience Network
            item = adNetworkItems.AddDict();
            item.SetString("SKAdNetworkIdentifier", "v9wttpbfk9.skadnetwork");
            item = adNetworkItems.AddDict();
            item.SetString("SKAdNetworkIdentifier", "n38lu8286q.skadnetwork");
            // Pangle
            item = adNetworkItems.AddDict();
            item.SetString("SKAdNetworkIdentifier", "22mmun2rn5.skadnetwork");
            // UnityAds
            item = adNetworkItems.AddDict();
            item.SetString("SKAdNetworkIdentifier", "4dzt52r2t5.skadnetwork");
            // vungle adapter version 4.3.6 (chua can thiet )
            item = adNetworkItems.AddDict();
            item.SetString("SKAdNetworkIdentifier", "gta9lk7p23.skadnetwork");

            rootDict.values.Remove("UIApplicationExitsOnSuspend");
            plist.WriteToFile(plistPath);

            var proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(projPath));
            string targetGuid = proj.TargetGuidByName(targetName);
            proj.SetTeamId(targetGuid, AppleDeveloperTeamID);
            proj.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");
            File.WriteAllText(projPath, proj.WriteToString());

            OnIOSBuild(buildTarget, pathToBuiltProject);
        }
    }

    private static void OnIOSBuild(BuildTarget target, string path)
    {
        NativeLocale.AddLocalizedStringsIOS(path, Path.Combine(Application.dataPath, "NativeLocale/iOS"));
    }

    #endregion
#endif
    static public void JenkinRestoreObfuscate()
    {
        //Debug.Log("Restore obfuscator");
        //Beebyte.Obfuscator.RestoreUtils.RestoreMonobehaviourSourceFiles();
    }

    [MenuItem("Hiker/Build/Test Jenkin Build AssetBundles")]
    static void JenkinBuildAssetBundle()
    {
        //JenkinBuildAssetBundlePri(BuildAssetBundleOptions.None);
    }

    static void JenkinReBuildAssetBundle()
    {
        //JenkinBuildAssetBundlePri(BuildAssetBundleOptions.ForceRebuildAssetBundle);
    }

    //static void JenkinBuildAssetBundlePri(BuildAssetBundleOptions option)
    //{
    //    string outputPath = Path.Combine(AssetBundles.Utility.AssetBundlesOutputPath, AssetBundles.Utility.GetPlatformName());
    //    if (!Directory.Exists(outputPath))
    //        Directory.CreateDirectory(outputPath);

    //    //@TODO: use append hash... (Make sure pipeline works correctly with it.)
    //    BuildPipeline.BuildAssetBundles(outputPath, option, EditorUserBuildSettings.activeBuildTarget);

    //    AssetBundles.AssetBundlesMenuItems.BuildAssetBundles();
    //    JenkinRestoreObfuscate();
    //}

    static public void BuildAssetBundleConfig(BuildTarget buildTarget, int configVer)
    {
        string outputPath = Path.Combine("AssetBundles", "Configs/" + buildTarget.ToString());
        outputPath = Path.Combine(GetProjPath(), outputPath);
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        AssetBundleBuild[] configs = new AssetBundleBuild[1];
        configs[0].assetBundleName = "v" + configVer + ".unity3d";

        bool b = AssetDatabase.IsValidFolder("Assets/Shootero/Resources/Configs_" + GameClient.GameVersion);

        if (b)
        {
            string[] assetsGUID = AssetDatabase.FindAssets(string.Empty, new string[] {
                "Assets/Shootero/Resources/Configs_" + GameClient.GameVersion,
                //"Assets/Shootero/Resources/Localization"
            });
            configs[0].assetNames = new string[assetsGUID.Length + 1];
            for (int i = 0; i < assetsGUID.Length; ++i)
            {
                var guid = assetsGUID[i];
                configs[0].assetNames[i] = AssetDatabase.GUIDToAssetPath(guid);
            }
            configs[0].assetNames[assetsGUID.Length] = "Assets/Shootero/Resources/Localization.csv";
        }

        BuildPipeline.BuildAssetBundles(outputPath, configs, BuildAssetBundleOptions.None, buildTarget);
    }

    static public void JenkinBuildAssetBundleConfig()
    {
        var jenkinBuildNumber = System.Environment.GetEnvironmentVariable("BUILD_NUMBER");
        var ver = int.Parse(jenkinBuildNumber);
        BuildAssetBundleConfig(EditorUserBuildSettings.activeBuildTarget, ver);
    }
}
