using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker
{
    public class QualityRenderSetting : MonoBehaviour
    {
        public int DefaultCapFrameAndroid = 60;
        public int DefaultCapFrameIOS = 60;
        public int DefaultQualityLevel = 2; // medium

        int mCurCapFrameSetting = 0;

        public static QualityRenderSetting instance = null;
        private void Awake()
        {
            instance = this;

            SetTargetFPS();

#if UNITY_STANDALONE
            DefaultQualityLevel = 5;
#endif
            int defaultLevel = DefaultQualityLevel;
            if (QualitySettings.GetQualityLevel() != defaultLevel)
                QualitySettings.SetQualityLevel(defaultLevel);

            QualitySettings.vSyncCount = 0;
#if UNITY_IOS
            QualitySettings.antiAliasing = 0;
            QualitySettings.shadowResolution = ShadowResolution.High;
#endif
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            #region Scale Resolution
            int fixedWidth = 1080;
            // kich thuoc be ngang battle
            var battleWidth = Screen.currentResolution.width;
            var varW = Screen.currentResolution.height * 9 / 16;
            if (varW < Screen.currentResolution.width) // larger 16:9 ratio
            {
                battleWidth = varW;
                if (battleWidth > fixedWidth)
                {
                    var newHeight = fixedWidth * 16 / 9;
                    var newWidth = newHeight * Screen.currentResolution.width / Screen.currentResolution.height;
                    Screen.SetResolution(newWidth, newHeight, true);
                }
            }
            else
            {
                if (battleWidth > fixedWidth)
                {
                    var newWidth = fixedWidth;
                    var newHeight = newWidth * Screen.currentResolution.height / Screen.currentResolution.width;
                    Screen.SetResolution(newWidth, newHeight, true);
                }
            }
            #endregion
        }

        public void SetTargetFPS(int remoteTargetFPS = 0)
        {
            if (mCurCapFrameSetting == 0)
            {
#if UNITY_ANDROID
                mCurCapFrameSetting = DefaultCapFrameAndroid;
#elif UNITY_IOS
                mCurCapFrameSetting = DefaultCapFrameIOS;
#endif
            }

            if (remoteTargetFPS == 0)
            {
                remoteTargetFPS = mCurCapFrameSetting;
            }

            if (remoteTargetFPS > 0 && Application.targetFrameRate != remoteTargetFPS)
            {
                Application.targetFrameRate = remoteTargetFPS;
            }
        }

        public void OnFetchRemoteConfig()
        {
            int remoteTargetFPS = 0;
#if UNITY_IOS
            remoteTargetFPS = (int)RemoteConfigManager.Instance.GetLongConfigValue("IOS_TARGET_FPS");

            int generation = (int)RemoteConfigManager.Instance.GetLongConfigValue("IOS_LIMIT_CAPFRAME");

            if (remoteTargetFPS > 0 && generation > 0 && (int)UnityEngine.iOS.Device.generation < generation)
            {
                remoteTargetFPS = 0;
            }
#elif UNITY_ANDROID
            remoteTargetFPS = (int)RemoteConfigManager.Instance.GetLongConfigValue("ANDROID_TARGET_FPS");

            string androidModels = RemoteConfigManager.Instance.GetStringConfigValue("ANDROID_LIMIT_CAPFRAME");
            if (string.IsNullOrEmpty(androidModels) == false)
            {
                string[] modelNames = androidModels.Split(';');

                if (remoteTargetFPS > 0 &&
                    System.Array.Exists(modelNames, e => SystemInfo.deviceModel.Contains(e)) == false)
                {
                    remoteTargetFPS = 0;
                }
            }
            else
            {
                remoteTargetFPS = 0;
            }
#endif
            if (remoteTargetFPS > 0)
            {
                mCurCapFrameSetting = remoteTargetFPS;
            }

            SetTargetFPS(remoteTargetFPS);

            int remoteQualityAA = -1;
#if UNITY_IOS
            remoteQualityAA = (int)RemoteConfigManager.Instance.GetLongConfigValue("IOS_QUALITY_AA");
#endif
            if (remoteQualityAA >= 0 && remoteQualityAA != QualitySettings.antiAliasing)
            {
                QualitySettings.antiAliasing = remoteQualityAA;
            }

            int remoteShadowQuality = -1;
            remoteShadowQuality = (int)RemoteConfigManager.Instance.GetLongConfigValue("QUALITY_SHADOW");
            if (remoteShadowQuality >= (int)ShadowQuality.Disable && remoteShadowQuality <= (int)ShadowQuality.All)
            {
                QualitySettings.shadows = (ShadowQuality)remoteShadowQuality;
            }

            int remoteShadowRes = -1;
            remoteShadowRes = (int)RemoteConfigManager.Instance.GetLongConfigValue("SHADOW_RES");
            if (remoteShadowRes >= (int)ShadowResolution.Low && remoteShadowRes <= (int)ShadowResolution.VeryHigh)
            {
                QualitySettings.shadowResolution = (ShadowResolution)remoteShadowRes;
            }
        }

        //// Start is called before the first frame update
        //void Start()
        //{

        //}

        //// Update is called once per frame
        //void Update()
        //{

        //}
    }
}

