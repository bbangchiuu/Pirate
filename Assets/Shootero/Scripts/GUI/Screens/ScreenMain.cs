using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    using UnityEngine.UI.Extensions;
    using Hiker.Networks.Data.Shootero;
    using DanielLochner.Assets.SimpleScrollSnap;

    public class ScreenMain : ScreenBase
    {
        public static ScreenMain instance;

        public SimpleScrollSnap mainScroll;

        public GroupEquipment grpEquipment;
        public GroupChapter grpChapter;
        public GroupArmory grpArmory;
        public GroupTreasury grpTreasury;
        public GroupSettings grpSettings;

        public GameObject lockedEquipment;
        public GameObject lockedTreasury;
        public GameObject lockedArmory;

        public GameObject notifyShop;
        public GameObject notifyHeroes;
        public GameObject notifyPower;

        bool checkLinkedAccount = false;

        public override bool OnBackBtnClick()
        {
            if (mainScroll.CurrentPanel != 2)
            {
                mainScroll.GoToPanel(2);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsLockedEquipment(UserInfo uInfo)
        {
            var pref = PlayerPrefs.GetInt("NotifyEquipment" + GameClient.instance.GID, 0);

            return uInfo.ListTrangBi.Count <= 0 && uInfo.GetCurrentChapter() < 1 && pref == 0;
        }

        public static bool IsLockedTreasury(UserInfo uInfo)
        {
            return uInfo.GetCurrentChapter() < 1; // complete chap 1
        }

        public static bool IsLockArmory(UserInfo uInfo)
        {
            if (uInfo.ListChapters.Count == 0) return true;

            return uInfo.ListChapters.Find(e => e.ChapIdx == 0).NumBattle < 1;
        }

        public void SyncNetworkData()
        {
            var isLockShop = IsLockedTreasury(GameClient.instance.UInfo);
            grpTreasury.gameObject.SetActive(isLockShop == false);
            var isLockEquipment = IsLockedEquipment(GameClient.instance.UInfo);
            grpEquipment.gameObject.SetActive(isLockEquipment == false);
            var isLockPower = IsLockArmory(GameClient.instance.UInfo);
            grpArmory.gameObject.SetActive(isLockPower == false);

            lockedArmory.gameObject.SetActive(isLockPower);
            lockedEquipment.gameObject.SetActive(isLockEquipment);
            lockedTreasury.gameObject.SetActive(isLockShop);

            if (isLockPower == false)
            {
                var pref = PlayerPrefs.GetInt("NotifyArmor" + GameClient.instance.GID, 0);
                notifyPower.gameObject.SetActive(pref == 0);
            }
            else
            {
                notifyPower.gameObject.SetActive(false);
            }

            if (isLockEquipment == false)
            {
                var pref = PlayerPrefs.GetInt("NotifyEquipment" + GameClient.instance.GID, 0);
                notifyHeroes.gameObject.SetActive(pref == 0);
            }
            else
            {
                notifyHeroes.gameObject.SetActive(false);
            }

            mainScroll.Setup(false);
        }

        [GUIDelegate]
        public void OnToggleEquipment()
        {
            var isLockEquipment = IsLockedEquipment(GameClient.instance.UInfo);
            if (isLockEquipment)
            {
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("HeroTabLockMsg"));
            }
        }

        [GUIDelegate]
        public void OnToggleShop()
        {
            var isLockShop = IsLockedTreasury(GameClient.instance.UInfo);
            if (isLockShop)
            {
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("ShopTabLockMsg"));
            }
        }

        [GUIDelegate]
        public void OnToggleArmory()
        {
            var isLockPower = IsLockArmory(GameClient.instance.UInfo);
            if (isLockPower)
            {
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("PowerTabLockMsg"));
            }
        }

        private void Awake()
        {
            instance = this;

            //var toggleEquip = lockedEquipment.GetComponentInParent<Toggle>();
            //var toggleTreasury = lockedTreasury.GetComponentInParent<Toggle>();
            //var toggleArmory = lockedArmory.GetComponentInParent<Toggle>();

            //toggleEquip.onValueChanged.AddListener(OnToggleEquipment);
            //toggleTreasury.onValueChanged.AddListener(OnToggleShop);
            //toggleArmory.onValueChanged.AddListener(OnToggleArmory);
            //var scrollConflictMan1 = grpEquipment.scrollView.gameObject.AddMissingComponent<ScrollConflictManager>();
            //scrollConflictMan1.ParentScrollRect = mainScroll;

            //var scrollConflictMan2 = grpTreasury.scrollView.gameObject.AddMissingComponent<ScrollConflictManager>();
            //scrollConflictMan2.ParentScrollRect = mainScroll;
        }
        
        void CheckRewardInfo()
        {
            if (GameClient.instance && GameClient.instance.UInfo != null && GameClient.instance.UInfo.GID > 0)
            {
                GameClient.instance.CheckRewardInfo();
            }
        }

        public override void OnActive()
        {
            base.OnActive();

#if IRON_SOURCE
            if (IronSourceAdsManager.instance && IronSourceAdsManager.instance.isInited == false)
            {
                IronSourceAdsManager.instance.InitIronSource();
            }
#endif

            if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
            {
                Debug.Log("Ad Available");
            }

            if (string.IsNullOrEmpty(GameClient.instance.CurBattleID) == false)
            {
                PopupConfirm.Create(Localization.Get("MsgConfirmNotFinishedBattle"),
                    () => GameClient.instance.RequestStartBattle(GameClient.instance.CurBattleID),
                    () =>
                    {
                        GameClient.instance.CurBattleID = string.Empty;
                        CheckRewardInfo();
                    },
                    Localization.Get("BtnContinue"));
            }
            else
            {
                CheckRewardInfo();
            }

            if (ResourceBar.instance)
            {
                ResourceBar.instance.gameObject.SetActive(true);
            }

            GUIManager.Instance.StartFadeInMusic();

            SyncNetworkData();

            CheckUpdateUserInfo(true);

            if (GameClient.instance.UInfo != null &&
                GameClient.instance.UInfo.ListChapters != null &&
                GameClient.instance.UInfo.ListChapters.Count > 0)
            {
                var chap = GameClient.instance.UInfo.ListChapters[0];
                if (chap.NumBattle == 1)
                {
#if UNITY_ANDROID || UNITY_IOS
                    if (checkLinkedAccount == false)
                    {
                        checkLinkedAccount = true;
                        if (SocialManager.Instance && SocialManager.Instance.IsAuthenticated == false)
                        {
                            SocialManager.Instance.LinkAccount();
                        }
                    }
#endif
                }
                else if (chap.NumBattle < 1)
                {
                    checkLinkedAccount = false;
                }
            }

            CheckNotifyShop();

            grpTreasury.SyncNetworkData();
            if (GameClient.instance.UInfo.GetCurrentChapter() >= ConfigManager.GetChapterUnlockHero())
            {
                var unlockedPref = PlayerPrefs.GetInt("ShowUnlockHeroTooltip_" + GameClient.instance.UInfo.GID, 0);
                if (unlockedPref == 0) // firsttime
                {
                    PlayerPrefs.SetInt("ShowUnlockHeroTooltip_" + GameClient.instance.UInfo.GID, -1);
                    mainScroll.GoToPanel(1); // go to hero
                }
            }

            grpSettings.ApplySettingUI();
#if IAP_BUILD
            // duongrs move code recheck missing payment here
            if (UnityIAPManager.instance && UnityIAPManager.instance.CheckLoginToPurchase())
                UnityIAPManager.instance.CheckListProcessingProduct();
#endif
        }

        void CheckNotifyShop()
        {
            notifyShop.gameObject.SetActive(false);

            if (lockedTreasury.activeInHierarchy == false)
            {
                var pref = PlayerPrefs.GetInt("NotifyShop" + GameClient.instance.GID, 0);
                if (pref == 0)
                {
                    notifyShop.gameObject.SetActive(true);
                }
#if IAP_BUILD
                else
                if (UnityIAPManager.instance && UnityIAPManager.instance.HaveMissingProduct())
                {
                    notifyShop.gameObject.SetActive(true);
                }
#endif
            }
        }

        void CheckUpdateUserInfo(bool runInBackground, int expireTimeInSeconds = 60 * 3)
        {
            var t = System.TimeZoneInfo.Local.BaseUtcOffset.Hours;
            bool changedTimeZone = false;
            if (PlayerPrefs.GetInt("LastTimeZone", 72) != t)
            {
                PlayerPrefs.SetInt("LastTimeZone", t);
                changedTimeZone = true;
            }

            var ts = TimeUtils.Now - GameClient.instance.LastTimeUpdateInfo;

            if (ts.TotalSeconds > expireTimeInSeconds || changedTimeZone)
            {
                GameClient.instance.RequestGetUserInfo(UserInfo.ALL_PROPS, runInBackground);
            }
#if SERVER_1_3
            if (runInBackground == false)
            {
                string key = "LastTimeUpdateInfo_" + GameClient.instance.GID.ToString();
                if (PlayerPrefs.HasKey(key))
                {
                    System.DateTime lastTimeGetInfo;
                    if (System.DateTime.TryParse(PlayerPrefs.GetString(key),
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal,
                        out lastTimeGetInfo))
                    {
                        if ((TimeUtils.Now - lastTimeGetInfo).TotalDays > 1 &&
                            GameClient.instance.UInfo.Gamer.ComeBack == 0 &&
                            GameClient.instance.UInfo.Gamer.RegisterTime.AddDays(3) > GameClient.instance.ServerTime)
                        {
                            GameClient.instance.RequestGetComeBackMail();
                        }
                    }
                }
            }
#endif
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause == false)
            {
                CheckUpdateUserInfo(false);

                if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
                {
                    Debug.Log("Ad Available");
                }
            }
        }

        public override void OnDeactive()
        {
            base.OnDeactive();

            GUIManager.Instance.StartFadeOutMusic();
        }

        public void OnInitializedShop()
        {
            if (grpTreasury.gameObject.activeInHierarchy)
                grpTreasury.SyncNetworkData();
        }

        public bool IsShowingShopPanel()
        {
            return mainScroll.CurrentPanel == 0;
        }

        public bool IsShowingHeroesPanel()
        {
            return mainScroll.CurrentPanel == 1;
        }

        public bool IsShowingPowersPanel()
        {
            return mainScroll.CurrentPanel == 3;
        }
        [GUIDelegate]
        public void OnPanelChanged()
        {
            if (mainScroll.TargetPanel == 0) // panel shop
            {
                var pref = PlayerPrefs.GetInt("NotifyShop" + GameClient.instance.GID, 0);
                if (pref == 0)
                {
                    PlayerPrefs.SetInt("NotifyShop" + GameClient.instance.GID, 1);
                }

                CheckNotifyShop();
            }
            else
            if (mainScroll.TargetPanel == 3)
            {
                var pref = PlayerPrefs.GetInt("NotifyArmor" + GameClient.instance.GID, 0);
                if (pref == 0)
                {
                    PlayerPrefs.SetInt("NotifyArmor" + GameClient.instance.GID, 1);
                }

                notifyPower.gameObject.SetActive(false);
            }
            else
            if (mainScroll.TargetPanel == 1)
            {
                var pref = PlayerPrefs.GetInt("NotifyEquipment" + GameClient.instance.GID, 0);
                if (pref == 0)
                {
                    PlayerPrefs.SetInt("NotifyEquipment" + GameClient.instance.GID, 1);
                    Debug.Log("Turn off notify equipment");
                }

                notifyHeroes.gameObject.SetActive(false);

                if (GameClient.instance.UInfo.GetCurrentChapter() >= ConfigManager.GetChapterUnlockHero())
                {
                    var unlockedPref = PlayerPrefs.GetInt("ShowUnlockHeroTooltip_" + GameClient.instance.UInfo.GID, 0);
                    if (unlockedPref <= 0)
                    {
                        grpEquipment.CheckHeroUnlock();
                        Hiker.HikerUtils.DoAction(this, () =>
                        {
                            //PlayerPrefs.GetInt("ShowUnlockHeroTooltip_" + GameClient.instance.UInfo.GID, 1);
                            grpEquipment.unlockedHeroTooltip.gameObject.SetActive(false);
                        }, 4, true);
                    }
                }
            }
        }
    }
}