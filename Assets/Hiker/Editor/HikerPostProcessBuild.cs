using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;

public class HikerPostProcessBuild
{
#if UNITY_IOS
    /// <summary>
    /// Description for IDFA request notification 
    /// [sets NSUserTrackingUsageDescription]
    /// </summary>
    const string TrackingDescription =
        "Your data will be only used to provide you a better and personalized ad experience.";

    [PostProcessBuild(0)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToXcode)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            AddPListValues(pathToXcode);
            AddFrameWorks(pathToXcode);
        }
    }

    static void AddPListValues(string pathToXcode)
    {
        // Get Plist from Xcode project 
        string plistPath = pathToXcode + "/Info.plist";

        // Read in Plist 
        PlistDocument plistObj = new PlistDocument();
        plistObj.ReadFromString(File.ReadAllText(plistPath));

        // set values from the root obj
        PlistElementDict plistRoot = plistObj.root;

        // Set value in plist
        plistRoot.SetString("NSUserTrackingUsageDescription", TrackingDescription);

        // save
        File.WriteAllText(plistPath, plistObj.WriteToString());
    }

    static void AddFrameWorks(string pathToXcode)
    {
        //string projPath = pathToXcode + "/Unity-iPhone.xcodeproj/project.pbxproj";

        string entitlementName = "arcadehunter.entitlements";
        var targetName = PBXProject.GetUnityTargetName();
        //EmbedFrameworks
        string projPath = PBXProject.GetPBXProjectPath(pathToXcode);
        PBXProject proj = new PBXProject();
        proj.ReadFromString(File.ReadAllText(projPath));
        string targetGUID = proj.TargetGuidByName(targetName);
        proj.AddFrameworkToProject(targetGUID, "AppTrackingTransparency.framework", true);
        proj.WriteToFile(projPath);
        //EmbedFrameworks end
    }
#endif
}
