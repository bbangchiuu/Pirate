using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using System;
    using UnityEngine.UI;
    public class GroupChapter : NetworkDataSync
    {
        public Text lbChapter;
        //public Text lbTopStage;
        public Image spChapterImage;
        public GameObject bossHuntAlert;
        public GameObject afkRewardAlert;
        public GameObject mailBoxAlert;
        public GameObject offerAlert;
        public GameObject leoThapAlert;
        public GameObject thachDauAlert;
        public GameObject sanTheAlert;
        public GameObject dailyShopAlert;
        public GameObject dailyShopAlert_Halloween;

        public GameObject[] btnAfkVisuals;
        bool isEnableAFKButton = true;

        public Button btnAFK;
        public Button btnVongQuay;
        public Button btnRaidMode;
        public Button btnLeoThap;
        public Button btnSanThe;
        public Button btnThachDau;
        public Button btnTongHopQuangCao;
        public TweenRotation twBtnTongHopQuangCao;

        public Button btnOffers;
        public Button btnDailyShop;
        public Button btnDailyShop_Halloween;
        public GameObject btnGiangSinh;
        public GameObject btnTetNienThu;
        public GameObject btnOfferDacBiet;

        public Button[] btnTargetOffers;
        public Text[] lbTargetOfferTimes;

        public Button btnTruongThanhOffer;
        public Text lbTruongThanhOfferTime;

        public GameObject grpLockVongQuay;
        public GameObject grpLockRaid;
        public GameObject grpLockLeoThap;
        public GameObject grpLockSanThe;

        public GameObject grpLockThachDau;

        public GameObject grpPremium;
        public Text lbPremiumDay;

        int CurChapter = -1;
        int MaxChapter = -1;

        bool mInitedItems = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            InitItems();
        }

        private void InitItems()
        {
            //if (mInitedItems) return;
            if (GameClient.instance == null ||
                GameClient.instance.UInfo == null)
            {
                return;
            }

            SyncNetworkData();

            mInitedItems = true;
        }

        public DateTime lastTimeCheck = new DateTime(2020, 01, 01, 0, 0, 0, DateTimeKind.Utc);

        public override void SyncNetworkData()
        {
            var uInfo = GameClient.instance.UInfo;
            int curMaxChapter = uInfo.GetCurrentChapter();
            if (CurChapter < 0 || CurChapter > curMaxChapter)
            {
                CurChapter = curMaxChapter;
                MaxChapter = curMaxChapter;
            }
            if (MaxChapter < 0)
            {
                MaxChapter = curMaxChapter;
            }

            if (MaxChapter < curMaxChapter)
            {
                CurChapter = curMaxChapter;
                MaxChapter = curMaxChapter;
            }

            if(uInfo.Gamer.PremiumEndTime > GameClient.instance.ServerTime)
            {
                int remainDays = (uInfo.Gamer.PremiumEndTime.Date - GameClient.instance.ServerTime.Date).Days;
                lbPremiumDay.text = string.Format(Localization.Get("premium_remain_days"), remainDays);
                grpPremium.SetActive(true);
            }
            else
            {
                grpPremium.SetActive(false);
            }

            string chapter_format = Localization.Get("ChapterNameFormat");
            lbChapter.text =  string.Format(chapter_format, CurChapter + 1, Localization.Get(string.Format("Chapter{0}_Name", CurChapter + 1)) );

            var chapCfg = ConfigManager.chapterConfigs[CurChapter];
            var chapData = uInfo.ListChapters.Find(e => e.ChapIdx == CurChapter);

            //lbTopStage.text = string.Format(Localization.Get("TopStage"),
            //        1,
            //        chapCfg.Missions.Length);
            var listChapAva = Resources.Load<SpriteCollection>("ChapterAvatar");
            spChapterImage.sprite = listChapAva.GetSprite(string.Format("MainUI_Chapter{0:00}", CurChapter + 1));


            var armoryData = uInfo.ListArmories.Find(e => e.Name == "AfkRewardTalent");
            isEnableAFKButton = true;
            if (armoryData == null || armoryData.Level <= 0) isEnableAFKButton = false;
            btnAfkVisuals[0].SetActive(isEnableAFKButton);
            btnAfkVisuals[1].SetActive(!isEnableAFKButton);

            if (bossHuntAlert != null)
            {
                if (uInfo.ListBossHunts != null)
                {
                    var t = uInfo.ListBossHunts.FindAll(e => e.Status == 1);
                    bossHuntAlert.SetActive(t.Count > 0);
                }
            }

            if (afkRewardAlert != null)
            {
                if (uInfo.afkRewardData != null)
                {
                    TimeSpan ts = GameClient.instance.ServerTime - uInfo.afkRewardData.lastClaimTime;
                    afkRewardAlert.SetActive(ts.TotalHours > 2);
                }
            }

            if (mailBoxAlert != null)
            {
                if (uInfo.ListMails != null)
                {
                    var t = uInfo.ListMails.FindAll(e => (e.status == MailStatus.Unread) 
                    || (e.content.reward != null && e.content.reward.Count > 0 && e.status != MailStatus.Received));
                    mailBoxAlert.SetActive(t.Count > 0);
                }
            }

            //chua unlock
            if(uInfo.GetCurrentChapter() < ConfigManager.GetDailyShopChapterUnlock())
            {
                btnDailyShop_Halloween.gameObject.SetActive(false);
                btnDailyShop.gameObject.SetActive(false);
            }
            //halloween
            else if (uInfo.halloweenServerData != null
                && uInfo.halloweenServerData.StartTime < TimeUtils.Now && TimeUtils.Now < uInfo.halloweenServerData.EndTime)
            {
                btnDailyShop.gameObject.SetActive(false);
                btnDailyShop_Halloween.gameObject.SetActive(true);

                if (btnDailyShop_Halloween.gameObject.activeSelf && dailyShopAlert_Halloween != null)
                {
                    dailyShopAlert_Halloween.gameObject.SetActive(false);
                    if (uInfo.dailyShopData != null)
                    {
                        string dailyShopResetTimeStr = PlayerPrefs.GetString(uInfo.GID + "dailyShopResetTime", "");
                        if (dailyShopResetTimeStr != uInfo.dailyShopData.ResetTime.ToString())
                        {
                            dailyShopAlert_Halloween.gameObject.SetActive(true);
                        }
                    }
                }
            }
            //hang ngay
            else
            {
                btnDailyShop_Halloween.gameObject.SetActive(false);
                btnDailyShop.gameObject.SetActive(true);

                if (btnDailyShop.gameObject.activeSelf && dailyShopAlert != null)
                {
                    dailyShopAlert.gameObject.SetActive(false);
                    if (uInfo.dailyShopData != null)
                    {
                        string dailyShopResetTimeStr = PlayerPrefs.GetString(uInfo.GID + "dailyShopResetTime", "");
                        if (dailyShopResetTimeStr != uInfo.dailyShopData.ResetTime.ToString())
                        {
                            dailyShopAlert.gameObject.SetActive(true);
                        }
                    }
                }
            }

            if (uInfo.giangSinhSrvData != null
                && uInfo.GetCurrentChapter() > 0
                && uInfo.giangSinhSrvData.StartTime < TimeUtils.Now && TimeUtils.Now < uInfo.giangSinhSrvData.EndTime)
            {
                btnGiangSinh.gameObject.SetActive(true);
            }
            else
            {
                btnGiangSinh.gameObject.SetActive(false);
            }

            if (uInfo.tetEventGamerData != null
                && uInfo.tetEventServerData != null
                && uInfo.tetEventServerData.StartTime < TimeUtils.Now && TimeUtils.Now < uInfo.tetEventServerData.EndTime)
            {
                btnTetNienThu.gameObject.SetActive(true);
            }
            else
            {
                btnTetNienThu.gameObject.SetActive(false);
            }

            var listOffersDacbiet = GameClient.instance.GetListLimitedTimeOffers();

            btnOfferDacBiet.SetActive(listOffersDacbiet.Count > 0);

            for (int i = 0; i < btnTargetOffers.Length; i++)
            {
                btnTargetOffers[i].gameObject.SetActive(false);
            }

            if (uInfo.targetOfferData != null)
            {
                for (int i = 0; i < btnTargetOffers.Length; i++)
                {
                    if (i < uInfo.targetOfferData.ListOffers.Count)
                    {
                        btnTargetOffers[i].gameObject.SetActive(true);
                        TargetOfferDataItem item = uInfo.targetOfferData.ListOffers[i];
                        string targetOfferKey = uInfo.GID + item.PackageName;
                        if(PlayerPrefs.GetInt(targetOfferKey, 0) == 0)
                        {
                            PlayerPrefs.SetInt(targetOfferKey, 1);
                            OnBtnTargetOfferClick(i);
                        }
                    }
                }
                IsRefreshingTargetOffer = false;
            }

            bool isShowBtnTruongThanh = false;
            if(uInfo.Gamer.TruongThanhData != null)
            {
                lbTruongThanhOfferTime.transform.parent.gameObject.SetActive(uInfo.Gamer.TruongThanhData.purchased == false);
                if (uInfo.Gamer.TruongThanhData.purchased == true)
                {
                    //da nhan het phan thuong
                    if(uInfo.Gamer.TruongThanhData.receivedChapters.Count < ConfigManager.TruongThanhCfg.GemRewards.Length)
                    {
                        isShowBtnTruongThanh = true;
                    }
                }
                else
                {
                    DateTime expiredTime = uInfo.Gamer.TruongThanhData.startDate.AddHours(ConfigManager.TruongThanhCfg.ExpiredHours);
                    if(expiredTime > GameClient.instance.ServerTime)
                    {
                        isShowBtnTruongThanh = true;
                    }
                }
            }
            
            btnTruongThanhOffer.gameObject.SetActive(isShowBtnTruongThanh);

            var isLockTongHopQuangCao = IsLockTongHopQuangCao(GameClient.instance.UInfo);
            twBtnTongHopQuangCao.gameObject.SetActive(isLockTongHopQuangCao == false);
            if (btnTongHopQuangCao.gameObject.activeInHierarchy)
            {
                string saveKey = GameClient.instance.GID + "_COMPLETED_ADS";
                if (GameClient.instance.ServerTime.DayOfYear != PlayerPrefs.GetInt(saveKey, -1))
                {
                    twBtnTongHopQuangCao.enabled = true;
                    twBtnTongHopQuangCao.PlayForward();
                }
                else
                {
                    twBtnTongHopQuangCao.enabled = false;
                    twBtnTongHopQuangCao.transform.localRotation = Quaternion.identity;
                }
            }

            var isLockPower = ScreenMain.IsLockArmory(GameClient.instance.UInfo);
            btnAFK.gameObject.SetActive(isLockPower == false);

            var isLockVongQuay = IsLockVongQuay(GameClient.instance.UInfo);
            btnVongQuay.gameObject.SetActive(isLockPower == false);
            grpLockVongQuay.gameObject.SetActive(isLockVongQuay);

            var isLockRaid = IsLockRaid(GameClient.instance.UInfo);
            btnRaidMode.gameObject.SetActive(isLockPower == false);
            grpLockRaid.gameObject.SetActive(isLockRaid);

            var isLockLeoThap = IsLockLeoThap(GameClient.instance.UInfo);
            btnLeoThap.gameObject.SetActive(isLockPower == false);
            grpLockLeoThap.gameObject.SetActive(isLockLeoThap);

            if (isLockLeoThap == false && btnLeoThap.gameObject.activeSelf)
            {
                if (GameClient.instance.UInfo.LuotLeoThap == 0 ||
                    GameClient.instance.UInfo.LeoThapRankIdx < 0 ||
                    string.IsNullOrEmpty(GameClient.instance.UInfo.BattleLeoThap) == false)
                {
                    // chua bao gio vao` leo thap, chua dat ten
                    leoThapAlert.SetActive(true);
                    //leoThapAlert.SetActive(lastTimeCheck < GameClient.instance.ServerTime); 
                }
                else
                {
                    leoThapAlert.SetActive(false);
                }
            }

            var isLockThachDau = IsLockThachDau(GameClient.instance.UInfo);
            btnThachDau.gameObject.SetActive(isLockThachDau == false);
            grpLockThachDau.gameObject.SetActive(isLockThachDau);

            if (isLockThachDau == false && btnThachDau.gameObject.activeSelf)
            {
                if (GameClient.instance.UInfo.LuotThachDau == 0 ||
                    string.IsNullOrEmpty(GameClient.instance.UInfo.BattleThachDau) == false)
                {
                    // chua bao gio vao` leo thap, chua dat ten
                    thachDauAlert.SetActive(true);
                    //leoThapAlert.SetActive(lastTimeCheck < GameClient.instance.ServerTime); 
                }
                else
                {
                    thachDauAlert.SetActive(false);
                }
            }

            var isLockSanThe = IsLockSanThe(GameClient.instance.UInfo);
            btnSanThe.gameObject.SetActive(isLockSanThe == false);
            grpLockSanThe.gameObject.SetActive(isLockSanThe);
            sanTheAlert.SetActive(false);

            if (isLockSanThe == false && btnSanThe.gameObject.activeSelf)
            {
                if (GameClient.instance.UInfo.LuotSanThe == 0 ||
                    string.IsNullOrEmpty(GameClient.instance.UInfo.BattleSanThe) == false)
                {
                    // chua bao gio vao` leo thap, chua dat ten
                    sanTheAlert.SetActive(true);
                    //leoThapAlert.SetActive(lastTimeCheck < GameClient.instance.ServerTime); 
                }
                else
                {
                    sanTheAlert.SetActive(false);
                }
            }

            offerAlert.gameObject.SetActive(false);
            var listOffers = GameClient.instance.GetListOffers();
            var listDailyOffers = GameClient.instance.GetListDailyOffers();

            btnOffers.gameObject.SetActive(false);
            if (listOffers != null)
            {
                var curHash = GameClient.GetHashStringListOffers(listOffers);
                if (listOffers.Count > 0)
                {
                    btnOffers.gameObject.SetActive(true);

                    if (listOffers.Count > 0)
                    {
                        var lastOffers = PlayerPrefs.GetString("OfferHash_" + uInfo.GID, string.Empty);
                        if (lastOffers.StartsWith("[") == false)
                        {
                            lastOffers = string.Empty;
                        }
                        if (string.IsNullOrEmpty(lastOffers) ||
                            GameClient.CompareHashListMd5(lastOffers, curHash) == false)
                        {
                            offerAlert.gameObject.SetActive(true);
                        }
                    }
                }
            }

            if (listDailyOffers != null)
            {
                if (listDailyOffers.Count > 0)
                {
                    btnOffers.gameObject.SetActive(true);
                    string saveKey = GameClient.instance.GID + "_" + "Daily_Offer_Free";
                    if (GameClient.instance.ServerTime.DayOfYear != PlayerPrefs.GetInt(saveKey, -1))
                    {
                        offerAlert.gameObject.SetActive(true);
                    }
                }
            }

            if (ScreenMain.instance) ScreenMain.instance.SyncNetworkData();
        }

        public static bool IsLockVongQuay(UserInfo uInfo)
        {
            if (uInfo.ListChapters.Count == 0) return true;
            var chap2 = uInfo.ListChapters.Find(e => e.ChapIdx == 1);
            if (chap2 == null) return true;
            return chap2.NumBattle < 1;
        }

        public static bool IsLockRaid(UserInfo uInfo)
        {
            if (uInfo.ListChapters.Count == 0) return true;
            var chap2 = uInfo.ListChapters.Find(e => e.ChapIdx == 1);
            if (chap2 == null) return true;
            return chap2.NumBattle < 1;
        }

        public static bool IsLockLeoThap(UserInfo uInfo)
        {
            if (uInfo.ListChapters.Count == 0) return true;
            var chapUnlock = uInfo.ListChapters.Find(e => e.ChapIdx == ConfigManager.LeoThapCfg.ChapterUnlock);
            if (chapUnlock == null) return true;
            return chapUnlock.NumBattle < 1;
        }

        public static bool IsLockThachDau(UserInfo uInfo)
        {
            if (uInfo.ListChapters.Count == 0) return true;
            var chapUnlock = uInfo.ListChapters.Find(e => e.ChapIdx == ConfigManager.ThachDauCfg.ChapterUnlock);
            if (chapUnlock == null) return true;
            return chapUnlock.NumBattle < 1;
        }

        public static bool IsLockSanThe(UserInfo uInfo)
        {
            if (uInfo.ListChapters.Count == 0) return true;
            var chapUnlock = uInfo.ListChapters.Find(e => e.ChapIdx == ConfigManager.SanTheCfg.ChapterUnlock);
            if (chapUnlock == null) return true;
            return chapUnlock.NumBattle < 1;
        }

        public static bool IsLockTongHopQuangCao(UserInfo uInfo)
        {
            if (uInfo.ListChapters.Count == 0) return true;
            var chapUnlock = uInfo.ListChapters.Find(e => e.ChapIdx == ConfigManager.GetTongHopQuangCaoChapterUnlock());
            if (chapUnlock == null) return true;
            return chapUnlock.NumBattle < 1;
        }

        private void Update()
        {
            if (mInitedItems == false)
            {
                InitItems();
                return;
            }

            if (mInitedItems == true)
            {
                UpdateTargetOfferTimer();
                UpdateTruongThanhButtonTimer();
            }
        }

        bool IsRefreshingTargetOffer = false;
        private void UpdateTargetOfferTimer()
        {
            if (GameClient.instance.UInfo.targetOfferData != null 
                && GameClient.instance.UInfo.targetOfferData.ListOffers.Count > 0
                && IsRefreshingTargetOffer == false)
            {
                for (int i = 0; i < GameClient.instance.UInfo.targetOfferData.ListOffers.Count; i++)
                {
                    TargetOfferDataItem offerItem = GameClient.instance.UInfo.targetOfferData.ListOffers[i];

                    var ts = offerItem.EndTime - GameClient.instance.ServerTime;
                    if (offerItem.EndTime > GameClient.instance.ServerTime)
                    {
                        lbTargetOfferTimes[i].text = string.Format(Localization.Get("TimeCountDownHMS"),
                            Mathf.FloorToInt((float)ts.TotalHours), ts.Minutes, ts.Seconds);
                    }
                    else
                    {
                        IsRefreshingTargetOffer = true;
                        GameClient.instance.RequestGetUserInfo(new string[] { "targetoffers" }, false);
                        break;
                    }
                }
            }
        }

        private void UpdateTruongThanhButtonTimer()
        {
            if (GameClient.instance.UInfo.Gamer.TruongThanhData != null
                && lbTruongThanhOfferTime.gameObject.activeInHierarchy)
            {
                DateTime expiredTime = GameClient.instance.UInfo.Gamer.TruongThanhData.startDate.AddHours(ConfigManager.TruongThanhCfg.ExpiredHours);
                var ts = expiredTime - GameClient.instance.ServerTime;
                if (expiredTime > GameClient.instance.ServerTime)
                {
                    if (ts.TotalDays > 1)
                    {
                        lbTruongThanhOfferTime.text = string.Format(Localization.Get("premium_remain_days"), Mathf.CeilToInt((float)ts.TotalDays));
                    }
                    else
                    {
                        lbTruongThanhOfferTime.text = string.Format(Localization.Get("TimeCountDownHMS"),
                            Mathf.FloorToInt((float)ts.TotalHours), ts.Minutes, ts.Seconds);
                    }
                }
                else
                {
                    btnTruongThanhOffer.gameObject.SetActive(false);
                }
            }
        }

        public static void GoToBattle(BattleData battleData)
        {
            PlayerPrefs.SetInt("FirstTimeShowAdsThisSessionStartBattle", 1);
            int chapIdx = battleData.ChapIdx;

            GameClient.instance.playerManager.ChapterIndex = chapIdx;
            //GameClient.instance.playerManager.IsFarmMode = string.IsNullOrEmpty(battleData.FarmCfg) == false;
            //GameClient.instance.playerManager.PassedMission = GameClient.instance.UInfo.GetPassedMission(chapIdx);
            //GameClient.instance.playerManager.CurrentPath = new List<int>();
            GameClient.instance.playerManager.battleData = battleData;
            GameClient.instance.playerManager.FarmName = battleData.FarmCfg;
            GameClient.instance.playerManager.LeoThapRankIdx = battleData.RankIdx;
            GameClient.instance.playerManager.LeoThapMod = battleData.LeoThapMod;

            GameClient.instance.playerManager.BattleMode = battleData.BattleMode;

            //if (battleData.IsFarmModeBattle())
            //{
            //    GameClient.instance.playerManager.BattleMode = 1;
            //}
            //else if (battleData.IsLeoThapBattle())
            //{
            //    GameClient.instance.playerManager.BattleMode = 2;
            //}
            //else if (battleData.SanThe != null)
            //{
            //    GameClient.instance.playerManager.BattleMode = 3;
            //}
            //else if (battleData.BattleMode == 4 &&
            //    string.IsNullOrEmpty(battleData.GrpThachDau) == false)
            //{
            //    GameClient.instance.playerManager.BattleMode = 4;
            //}
            //else
            //{
            //    GameClient.instance.playerManager.BattleMode = 0;
            //}

            GameClient.instance.playerManager.gameObject.SetActive(true);
            GUIManager.Instance.SetScreen("Battle");
        }

        public void OnSelectChapter(int chapIdx)
        {
            CurChapter = chapIdx;

            SyncNetworkData();
        }

        [GUIDelegate]
        public void OnChapterImageClicked()
        {
            PopupChapterSelect.Create(CurChapter);
        }

        [GUIDelegate]
        public void OnBtnGoClick()
        {
            var uInfo = GameClient.instance.UInfo;
            int theLuc = ConfigManager.GetTheLucCampaign();

            var theLucRegenTime = ConfigManager.GetGamerTheLucRegenSeconds();
            if (uInfo.Gamer.TheLuc.Val < theLuc)
            {
                uInfo.Gamer.TheLuc.UpdateByTime(GameClient.instance.ServerTime, theLucRegenTime);
            }

            if (uInfo.Gamer.TheLuc.Val >= theLuc)
            {
                // request Battle
                GameClient.instance.RequestStartBattle(CurChapter, 0);
            }
            else
            {
                PopupMissingTheLuc.Create();
            }
        }

        [GUIDelegate]
        public void OnBtnVongQuayClick()
        {
            if (IsLockVongQuay(GameClient.instance.UInfo))
            {
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("VongQuayLockMsg"));
                return;
            }
            PopupVongQuay.Create();

            //var uInfo = GameClient.instance.UInfo;
            //int theLuc = ConfigManager.GetTheLucVongQuay();
            //var theLucRegenTime = ConfigManager.GetGamerTheLucRegenSeconds();
            //if (uInfo.Gamer.TheLuc.Val < theLuc)
            //{
            //    uInfo.Gamer.TheLuc.UpdateByTime(GameClient.instance.ServerTime, theLucRegenTime);
            //}

            //if (uInfo.Gamer.TheLuc.Val >= theLuc)
            //{
            //    PopupVongQuay.Create();
            //    // request Battle
            //    //GameClient.instance.RequestStartVongQuay();
            //}
            //else
            //{
            //    PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("NotEnoughTheLuc"));
            //}
        }

        [GUIDelegate]
        public void OnBtnFarmModeClick()
        {
            if (IsLockRaid(GameClient.instance.UInfo))
            {
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("RaidModeLockMsg"));
                return;
            }
            PopupFarmMode.Create(CurChapter);
        }

        [GUIDelegate]
        public void OnBtnLeoThapClick()
        {
            if (IsLockLeoThap(GameClient.instance.UInfo))
            {
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("LeoThapLockMsg"));
                return;
            }

            if (string.IsNullOrEmpty(GameClient.instance.UInfo.Gamer.GetDisplayName()))
            {
                PopupDatTen.Create(GameClient.instance.RequestGetLeoThapData);
                return;
            }

            GameClient.instance.RequestGetLeoThapData();
        }

        [GUIDelegate]
        public void OnBtnThachDauClick()
        {
            if (IsLockThachDau(GameClient.instance.UInfo))
            {
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("ThachDauLockMsg"));
                return;
            }

            if (string.IsNullOrEmpty(GameClient.instance.UInfo.Gamer.GetDisplayName()))
            {
                PopupDatTen.Create(GameClient.instance.RequestGetThachDauData);
                return;
            }

            GameClient.instance.RequestGetThachDauData();
        }

        [GUIDelegate]
        public void OnBtnSanTheClick()
        {
            if (IsLockSanThe(GameClient.instance.UInfo))
            {
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("SanTheLockMsg"));
                return;
            }

            //if (string.IsNullOrEmpty(GameClient.instance.UInfo.Gamer.GetDisplayName()))
            //{
            //    PopupDatTen.Create(GameClient.instance.RequestGetLeoThapData);
            //    return;
            //}

            //GameClient.instance.RequestStartBattleSanThe();
            GameClient.instance.RequestGetSanTheData();
        }

        [GUIDelegate]
        public void OnBtnAFKRewardClick()
        {
            if (!isEnableAFKButton)
            {
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("LockAFKRewardMsg"));
                return;
            }
            GameClient.instance.RequestGetAFKReward();
        }

        [GUIDelegate]
        public void OnBtnBossHuntClick()
        {
            PopupBossHunt.Create();
        }

        [GUIDelegate]
        public void OnBtnMailBoxClick()
        {
            PopupMailBox.Create();
        }

        [GUIDelegate]
        public void OnBtnOffersClick()
        {
            //PopupMailBox.Create();
            PopupSpecialOffers.Create();
        }

        [GUIDelegate]
        public void OnBtnDailyShopClick()
        {
            PopupDailyShop.Create();
        }

        [GUIDelegate]
        public void OnBtnGiangSinhClick()
        {
            PopupGiangSinh.Create(true);
        }

        [GUIDelegate]
        public void OnBtnOfferDacBietClick()
        {
            PopupOfferDacBiet.Create();
        }

        [GUIDelegate]
        public void OnBtnTetNienThuClick()
        {
            var uInfo = GameClient.instance.UInfo;
            var srvTime = GameClient.instance.ServerTime;

            if (uInfo.tetEventServerData == null || uInfo.tetEventGamerData == null ||
                uInfo.tetEventServerData.StartTime > srvTime ||
                uInfo.tetEventServerData.EndTime < srvTime)
            {
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("EventNotAvailableNow"));
                return;
            }

            PopupTetNienThu.Create(uInfo.tetEventServerData, uInfo.tetEventGamerData);
        }

        [GUIDelegate]
        public void OnBtnTargetOfferClick(int idx)
        {
            if (GameClient.instance.UInfo.targetOfferData == null || GameClient.instance.UInfo.targetOfferData.ListOffers.Count <= idx) return;
            PopupTargetOffer.Create(GameClient.instance.UInfo.targetOfferData.ListOffers[idx]);
        }

        [GUIDelegate]
        public void OnBtnTruongThanhClicked()
        {
            if (lbTruongThanhOfferTime.gameObject.activeInHierarchy)
            {
                PopupTruongThanhOffer.Create();
            }
            else
            {
                PopupTruongThanh.Create();
            }
        }

        [GUIDelegate]
        public void OnBtnTongHopQuangCaoClicked()
        {
            PopupTongHopQuangCao.Create();
        }
    }
}