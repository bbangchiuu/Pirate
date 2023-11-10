using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    using UnityEngine.UI.Extensions;
    public class GroupSettings : NetworkDataSync
    {
        public Text StoreText;
        public Text SocialIDText;
        public GameObject connectedObj;
        public GameObject googlePlayObj;
        public GameObject gameCenterObj;
        public Text lbVersion;

        bool mInitedItems = false;

        public Toggle SoundToogle;
        public Toggle MusicToogle;
        public Toggle JoyStickToggle;
        public Toggle SkillButtonToggle;
        public Toggle ShowHUDToggle;
        public Button SocialBtn;
        public Text lblLanguage;

        public Button btnFanpage;
        public Button btnFbgroup;
        public Button btnReddit;
        public Button btnDiscord;
        public GameObject grpCommunity;

        public GameObject grpMaThuong;

        public GameObject grpRestoration;
        public GameObject grpAdsExp;
        public Button btnImproveAds;
        public Button btnWithdrawConsent;

        public ScrollRect scroll;

        protected void Start()
        {
            InitItems();
            if (scroll)
                scroll.verticalNormalizedPosition = 0.5f;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        [GUIDelegate]
        public void OnSoundChange()
        {
            if (GUIManager.Instance.SoundEnable != SoundToogle.isOn)
            {
                GUIManager.Instance.SoundEnable = SoundToogle.isOn;

                ApplySettingUI();

                GUIManager.Instance.ApplySoundAndMusicSetting();
                GUIManager.Instance.SaveSoundAndMusicSetting();
            }
        }

        [GUIDelegate]
        public void OnMusicChange()
        {
            if (GUIManager.Instance.MusicEnable != MusicToogle.isOn)
            {
                GUIManager.Instance.MusicEnable = MusicToogle.isOn;

                ApplySettingUI();

                GUIManager.Instance.ApplySoundAndMusicSetting();
                GUIManager.Instance.SaveSoundAndMusicSetting();
            }
        }

        [GUIDelegate]
        public void OnJoystickChange()
        {
            if (GameManager.instance.FixedJoystick != JoyStickToggle.isOn)
            {
                GameManager.instance.FixedJoystick = JoyStickToggle.isOn;

                if (ScreenBattle.instance && GUIManager.Instance.CurrentScreen == "Battle")
                {
                    ScreenBattle.instance.ApplyJoystickCfg();
                }

                ApplySettingUI();
                GameManager.instance.SaveSetting();
            }
        }

        [GUIDelegate]
        public void OnSkillButtonChange()
        {
            if (GameManager.instance.RightActiveSkill != SkillButtonToggle.isOn)
            {
                GameManager.instance.RightActiveSkill = SkillButtonToggle.isOn;

                ApplySettingUI();
                GameManager.instance.SaveSetting();
            }
        }

        [GUIDelegate]
        public void OnShowDmgHUDChange()
        {
            if (GameManager.instance.ShowDamageHUD != ShowHUDToggle.isOn)
            {
                GameManager.instance.ShowDamageHUD = ShowHUDToggle.isOn;

                ApplySettingUI();
                GameManager.instance.SaveSetting();
            }
        }

        [GUIDelegate]
        public void OnBtnChangeLanguage()
        {
            PopupSelectLanguage.Create();
        }

        [GUIDelegate]
        public void OnSocialBtnClick()
        {
            if (SocialManager.Instance && SocialManager.Instance.IsAuthenticated == false)
            {
                SocialManager.Instance.LinkAccount();
            }
        }

        [GUIDelegate]
        public void OnBtnSendBugReport()
        {
            SendMail(string.Empty);
        }

        static string MyEscapeURL(string url)
        {
            return UnityEngine.Networking.UnityWebRequest.EscapeURL(url).Replace("+", "%20");
        }

        public static void SendMail(string message)
        {
            string email = "arcadehunter@hikergames.com";
            if (ConfigManager.otherConfig.Contains("SuportMail"))
                email = ConfigManager.otherConfig["SuportMail"].ToString();

            var subject = MyEscapeURL(Localization.Get("ReportBugSubject"));
            var body = string.Format(
                "Device Model: {0} \nOS: {1} \nID: {2} \nUserID: {3} \nGame Version: {4} \nBugs: {5}",
                SystemInfo.deviceModel,
                SystemInfo.operatingSystem,
                string.IsNullOrEmpty(SocialManager.Instance.SocialID) ? SocialManager.Instance.DeviceID : SocialManager.Instance.SocialID,
                GameClient.instance.UInfo.GID,
                GameClient.GetGameVersionString(),
                message);
            body = MyEscapeURL(body);
            string urlMailTo = "mailto:" + email + "?subject=" + subject + "&body=" + body;
            Application.OpenURL(urlMailTo);
        }

        public void ApplySettingUI()
        {
            if (SoundToogle && SoundToogle.isOn != GUIManager.Instance.SoundEnable)
                SoundToogle.isOn = GUIManager.Instance.SoundEnable;
            if (MusicToogle && MusicToogle.isOn != GUIManager.Instance.MusicEnable)
                MusicToogle.isOn = GUIManager.Instance.MusicEnable;
            if (JoyStickToggle && JoyStickToggle.isOn != GameManager.instance.FixedJoystick)
            {
                JoyStickToggle.isOn = GameManager.instance.FixedJoystick;
            }
            if (SkillButtonToggle && SkillButtonToggle.isOn != GameManager.instance.RightActiveSkill)
            {
                SkillButtonToggle.isOn = GameManager.instance.RightActiveSkill;
            }

            if (ShowHUDToggle && ShowHUDToggle.isOn != GameManager.instance.ShowDamageHUD)
            {
                ShowHUDToggle.isOn = GameManager.instance.ShowDamageHUD;
            }

            if (lblLanguage)
            {
                lblLanguage.text = Localization.Get(ConfigManager.language);
            }
        }

        private void InitItems()
        {
            if (grpMaThuong) grpMaThuong.SetActive(true);

            if (GameClient.instance != null
                && GameClient.instance.UInfo != null
                && string.IsNullOrEmpty(GameClient.instance.UInfo.GhiChuServer) == false)
            {
                string ghichu = GameClient.instance.UInfo.GhiChuServer;
                string versionText = "GC_" + GameClient.GameVersion;
                if (ghichu.Contains(versionText))
                {
                    if (grpMaThuong) grpMaThuong.SetActive(false);
                }
            }
            ApplySettingUI();

            if (mInitedItems) return;

            mInitedItems = true;

            if (lbVersion)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                string ver = AndroidNativeBride.GetPackageVersionCode(AndroidNativeBride.GetPackageName());
                string gameVer = GameClient.GetGameVersionString() + ".b" + ver;
                lbVersion.text = gameVer + " - cfg." + GameClient.instance.GetConfigVersionString();
#else
                string gameVer = GameClient.GetGameVersionString();
                lbVersion.text = gameVer + " - cfg." + GameClient.instance.GetConfigVersionString();
#endif
            }

            if (StoreText)
            {
#if UNITY_IOS
                StoreText.text = Localization.Get("GameCenter_Label");
                gameCenterObj.SetActive(true);
                googlePlayObj.SetActive(false);
#else
                StoreText.text = Localization.Get("GooglePlay_Label");
                gameCenterObj.SetActive(false);
                googlePlayObj.SetActive(true);
#endif
            }

            SyncNetworkData();

            string urlFanpage = Localization.GetLocalization("FanpageURL", ConfigManager.language);
            string urlFbgroup = Localization.GetLocalization("FbGroupURL", ConfigManager.language);
            string urlReddit = Localization.GetLocalization("RedditURL", ConfigManager.language);
            string urlDiscord = Localization.GetLocalization("DiscordURL", ConfigManager.language);
            if (btnFanpage)
            {
                btnFanpage.gameObject.SetActive(string.IsNullOrEmpty(urlFanpage) == false);
            }
            if (btnFbgroup)
            {
                btnFbgroup.gameObject.SetActive(string.IsNullOrEmpty(urlFbgroup) == false);
            }
            if (btnReddit)
            {
                btnReddit.gameObject.SetActive(string.IsNullOrEmpty(urlReddit) == false);
            }
            if (btnDiscord)
            {
                btnDiscord.gameObject.SetActive(string.IsNullOrEmpty(urlDiscord) == false);
            }
            if (grpCommunity)
            {
                bool disableCommunity = ConfigManager.GetChapUnlockCommunity() > GameClient.instance.UInfo.GetCurrentChapter();
                grpCommunity.gameObject.SetActive(disableCommunity == false);
            }

            CheckAdsExperience();
        }

        public void CheckAdsExperience()
        {
#if UNITY_IOS
            //if (PopupAppTracking.NeedRequirePermisionTracking())
            //{
            //    if (grpAdsExp)
            //        grpAdsExp.gameObject.SetActive(true);

            //    var attPref = PopupAppTracking.GetAppTrackingTransparencyPref();
            //    if (btnWithdrawConsent)
            //        btnWithdrawConsent.gameObject.SetActive(attPref > 0);

            //    if (btnImproveAds)
            //    {
            //        btnImproveAds.gameObject.SetActive(attPref <= 0);
            //    }
            //}
            //else
            {
                if (grpAdsExp)
                    grpAdsExp.gameObject.SetActive(false);
            }
#else
            if (grpAdsExp)
                grpAdsExp.gameObject.SetActive(false);
#endif
        }

        [GUIDelegate]
        public void OnBtnFanpage()
        {
            string urlFanpage = Localization.GetLocalization("FanpageURL", ConfigManager.language);
            Debug.Log(urlFanpage);
            Application.OpenURL(urlFanpage);
        }
        [GUIDelegate]
        public void OnBtnFangroup()
        {
            string urlFanpage = Localization.GetLocalization("FbGroupURL", ConfigManager.language);
            Debug.Log(urlFanpage);
            Application.OpenURL(urlFanpage);
        }
        [GUIDelegate]
        public void OnBtnReddit()
        {
            string urlFanpage = Localization.GetLocalization("RedditURL", ConfigManager.language);
            Debug.Log(urlFanpage);
            Application.OpenURL(urlFanpage);
        }
        [GUIDelegate]
        public void OnBtnDiscord()
        {
            string urlFanpage = Localization.GetLocalization("DiscordURL", ConfigManager.language);
            Debug.Log(urlFanpage);
            Application.OpenURL(urlFanpage);
        }
        [GUIDelegate]
        public void OnBtnMaThuong()
        {
            PopupMaThuong.Create();
        }

        [GUIDelegate]
        public void OnBtnKhoiPhucGoiMua()
        {
#if IAP_BUILD
            UnityIAPManager.instance.RestoreTransactions();
#endif
        }

        [GUIDelegate]
        public void OnBtnImproveAds()
        {
            PopupAppTracking.Create();
        }

        [GUIDelegate]
        public void OnBtnWidthDrawAds()
        {
            PopupConfirm.Create(Localization.Get("MessageWithdrawConsent"), 
                () => {
                    PopupAppTracking.NoATT();
                    CheckAdsExperience();
                },
                true,
                Localization.Get("ok"),
                Localization.Get("btnWithdrawConsent")
                );
        }

        public override void SyncNetworkData()
        {
            if (SocialIDText)
            {
                if (SocialManager.Instance)
                {
                    if (string.IsNullOrEmpty(SocialManager.Instance.SocialID) == false)
                    {
                        SocialIDText.text = string.Format("ID: {0} - {1}", GameClient.instance.GID, SocialManager.Instance.SocialID);
                    }
                    else
                    {
                        SocialIDText.text = string.Format("ID: {0}", GameClient.instance.GID);
                    }
                    //SocialIDText.text = SocialManager.Instance.SocialID;
                    connectedObj.gameObject.SetActive(SocialManager.Instance.IsAuthenticated);
                }
                else
                {
                    SocialIDText.text = string.Format("ID: {0}", GameClient.instance.GID);
                    //SocialIDText.text = string.Empty;
                    connectedObj.gameObject.SetActive(false);
                }
            }

#if IAP_BUILD && UNITY_IOS
            if (grpRestoration)
            {
                string ghiChuText = "IAP_" + GameClient.GameVersion + ";";
                grpRestoration.SetActive(GameClient.instance.UInfo.GhiChuServer.Contains(ghiChuText));
            }
#else
            if (grpRestoration)
            {
                grpRestoration.SetActive(false);
            }
#endif
        }
    }
}