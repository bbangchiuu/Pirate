using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hiker.Networks.Data.Shootero;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;

    public class PopupRequestHoiSinh : PopupBase
    {
        public Text lbGemRevive;
        public Text lbMessage;
        public Button btnGemRevive;
        public Button btnWatchAds;
        public Text lbCountDown;
        public Image imgCoolDown;

        public static PopupRequestHoiSinh instance;

        public float TimeCountDown { get; private set; }
        public bool CountDown { get; private set; }

        public static PopupRequestHoiSinh Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupRequestHoiSinh");
            instance = go.GetComponent<PopupRequestHoiSinh>();
            instance.Init();

            return instance;
        }

        private void Init()
        {
            TimeCountDown = ConfigManager.GetReviveCountDown();
            CountDown = true;
            ScreenBattle.PauseGame(true);
            if (lbGemRevive)
                lbGemRevive.text = ConfigManager.GetGemRevive().ToString();
            if (lbCountDown)
                lbCountDown.text = Mathf.FloorToInt(TimeCountDown).ToString();
            if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.IsLeoThapMode)
            {
                int missionComplete = QuanlyNguoichoi.Instance.MissionID;
                //if (isVictory)
                //{
                //    //lbTitleResult.text = Localization.Get("BattleVictoryTitle");
                //    WinSound.Play();
                //}
                //else
                {
                    //lbTitleResult.text = Localization.Get("BattleFailTitle");
                    //LoseSound.Play();
                    missionComplete = QuanlyNguoichoi.Instance.MissionID - 1;
                }

                int floor = ConfigManager.LeoThapBattleCfg.GetTotalLevelFromMission(missionComplete);
                string floorStr = string.Format(Localization.Get("LeoThapLevel"), floor);
                long secondBattle = (long)System.Math.Floor(QuanlyNguoichoi.Instance.LastLvlBattleTime);
                var minutePlay = secondBattle / 60;
                var secondPlay = secondBattle % 60;
                string battleTimeStr = string.Format(Localization.Get("LeoThapTime"), minutePlay, secondPlay);

                if (lbMessage)
                    lbMessage.text = string.Format("{0} - {1}", floorStr, battleTimeStr);
            }
            else
            {
                if (lbMessage)
                    lbMessage.text = string.Empty;
            }

            bool showAds = false;
            if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
            {
                showAds = true;
            }

            if (showAds)
            {
                if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.IsNormalMode)
                {
                    if (GameClient.instance &&
                        GameClient.instance.OfflineMode == false &&
                        GameClient.instance.UInfo != null &&
                        GameClient.instance.UInfo.Gamer != null &&
                        GameClient.instance.UInfo.Gamer.ReviveAds != null)
                    {
                        if (GameClient.instance.UInfo.Gamer.ReviveAds.Val <= 0)
                        {
                            showAds = false;
                        }
                        else
                        {
                            if (GameClient.instance.UInfo.Gamer.ReviveAds.Val < GameClient.instance.UInfo.Gamer.ReviveAds.MaxVal)
                            {
                                showAds = Random.Range(0, 100) < ConfigManager.GetAdReviveMaxRate();
                            }
                        }
                    }
                }
            }

            if (btnWatchAds)
                btnWatchAds.gameObject.SetActive(showAds);
        }

        [GUIDelegate]
        public void OnBtnGemClick()
        {
            if (GameClient.instance.UInfo.Gamer.Gem < ConfigManager.GetGemRevive())
            {
                CountDown = false;
                PopupBuyGem.Create(()=> { Init(); });
            }
            else
            {
                CountDown = false;
                Dismiss();

                GameClient.instance.RequestReviveBattle(QuanlyNguoichoi.Instance.BattleMode);
                AnalyticsManager.LogEvent("REVIVE_GEM");
            }
        }

        [GUIDelegate]
        public override void OnCloseBtnClick()
        {
            base.OnCloseBtnClick();
            QuanlyNguoichoi.Instance.OnPlayerDied();
        }

        [GUIDelegate]
        public void OnBtnWatchAds()
        {
            if (preventDoubleClick) return;
            CountDown = false;
            AnalyticsManager.LogEvent("WATCH_ADS_REVIVE");
            if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
            {
                HikerAdsManager.instance.ShowVideoAds(RequestReviveByAds, RequestCancelVideo,
                    RequestFailVideo, RequestCancelVideo);
                StartCoroutine(PreventDoubleclick(5));
            }
            else
            {
                // van xu ly nhu la` da request video nhung fail
                // -> revive unit
                RequestFailVideo();
            }
        }

        private void RequestReviveByAds()
        {
            AnalyticsManager.LogEvent("FINISH_ADS_REVIVE");
            Dismiss();
            QuanlyNguoichoi.Instance.ReviveNow();
            PopupBattlePause.Create();

            if (QuanlyNguoichoi.Instance.IsNormalMode)
            {
                if (GameClient.instance.OfflineMode == false)
                {
                    GameClient.instance.UInfo.Gamer.ReviveAds.Val--;
                    GameClient.instance.RequestWatchAdsRevive();
                }
            }
        }

        private void RequestCancelVideo()
        {
            PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("watch_ads_cancel"));
            Init();
        }

        private void RequestFailVideo()
        {
            //PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("watch_ads_fail"));
            //Init();
            // van cho phep hoi sinh du` fail xem video
            AnalyticsManager.LogEvent("FAIL_ADS_REVIVE");
            Dismiss();
            QuanlyNguoichoi.Instance.ReviveNow();
            PopupBattlePause.Create();

            if (QuanlyNguoichoi.Instance.IsNormalMode)
            {
                if (GameClient.instance.OfflineMode == false)
                {
                    GameClient.instance.UInfo.Gamer.ReviveAds.Val--;
                    GameClient.instance.RequestWatchAdsRevive();
                }
            }
        }
        bool preventDoubleClick = false;
        private IEnumerator PreventDoubleclick(float time)
        {
            preventDoubleClick = true;
            yield return new WaitForSecondsRealtime(time);
            preventDoubleClick = false;
        }

        private void Update()
        {
            if (CountDown)
            {
                if (TimeCountDown > 0)
                {
                    TimeCountDown -= Time.unscaledDeltaTime;
                }
                else
                if (TimeCountDown <= 0)
                {
                    TimeCountDown = 0;
                    OnCloseBtnClick();
                }
                imgCoolDown.fillAmount = TimeCountDown / ConfigManager.GetReviveCountDown();
                lbCountDown.text = ((int)Mathf.Round(TimeCountDown)).ToString();
            }
        }

        protected override void OnCleanUp()
        {
            ScreenBattle.PauseGame(false);
        }

        public static void Dismiss()
        {
            if (instance != null)
            {
                instance.OnCleanUp();
                instance.Hide();
                instance = null;
            }
        }
    }

}

