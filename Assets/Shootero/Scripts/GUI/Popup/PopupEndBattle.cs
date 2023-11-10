using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hiker.Networks.Data.Shootero;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;

    public class PopupEndBattle : PopupBase
    {
        public Text lbTitleResult;
        public PhanThuongItem phanThuongPrefab;
        public Transform phanThuongParent;
        public Transform phanThuongPremiumBonusParent;
        public Transform phanThuongChestParent;
        public Text lbPointNienThuSumarize;

        public AudioSource WinSound;
        public AudioSource LoseSound;
        public AudioSource LoseSoundMale;

        public static PopupEndBattle instance;

        public Text lbMoreGoldAmount;
        public Button btnWatchAdsThemVang;
        bool preventDoubleClick;

        public List<TweenPosition> listGoldIcons;
        public TweenPosition twGrpGolds;
        public Text lbGetMore;
        PhanThuongItem goldItem;
        KeyValuePair<string, long> goldData;
        int vangThuongThem;
        int chapIdx;

        public static PopupEndBattle Create(bool isVictory)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupEndBattle");
            instance = go.GetComponent<PopupEndBattle>();
            EventDelegate.Add(instance.twGrpGolds.onFinished, () => 
            {
                instance.twGrpGolds.gameObject.SetActive(false);
                if(instance.goldItem != null)
                {
                    instance.goldItem.SetItem(instance.goldData.Key, instance.goldData.Value + instance.vangThuongThem);
                }
            });

            instance.Init(isVictory);

            return instance;
        }

        void PlayLoseSound()
        {
            string hero_name = QuanlyNguoichoi.Instance.PlayerUnit.UnitName;

            if (hero_name == "Zora" ||
                hero_name == "BeastMaster" ||
                hero_name == "Thor")
            {
                LoseSound.Play();
            }
            else
            {
                LoseSoundMale.Play();
            }
        }


        private void Init(bool isVictory)
        {
            ScreenBattle.PauseGame(true);

            btnWatchAdsThemVang.gameObject.SetActive(false);
            lbGetMore.gameObject.SetActive(false);

            if (QuanlyNguoichoi.Instance.IsLeoThapMode)
            {
                int missionComplete = QuanlyNguoichoi.Instance.MissionID;
                if (isVictory)
                {
                    //lbTitleResult.text = Localization.Get("BattleVictoryTitle");
                    WinSound.Play();
                }
                else
                {
                    //lbTitleResult.text = Localization.Get("BattleFailTitle");
                    PlayLoseSound();
                    missionComplete = QuanlyNguoichoi.Instance.MissionID - 1;
                }

                int floor = ConfigManager.LeoThapBattleCfg.GetTotalLevelFromMission(missionComplete);
                string floorStr = string.Format(Localization.Get("LeoThapLevel"), floor);
                long secondBattle = (long)System.Math.Floor(QuanlyNguoichoi.Instance.LastLvlBattleTime);
                var minutePlay = secondBattle / 60;
                var secondPlay = secondBattle % 60;
                string battleTimeStr = string.Format(Localization.Get("LeoThapTime"), minutePlay, secondPlay);
                lbTitleResult.text = string.Format("{0} - {1}", floorStr, battleTimeStr);
            }
            else
            {
                if (isVictory)
                {
                    lbTitleResult.text = Localization.Get("BattleVictoryTitle");
                    WinSound.Play();
                }
                else
                {
                    lbTitleResult.text = Localization.Get("BattleFailTitle");
                    PlayLoseSound();
                }
            }
            
            var playerLoot = QuanlyNguoichoi.Instance.GetPlayerLoot();
            CardReward chestReward = new CardReward();

            playerLoot.AddGold((int)QuanlyNguoichoi.Instance.PlayerGold);
            var rate = 1;
            if (QuanlyNguoichoi.Instance.FarmMode > 1)
            {
                rate = QuanlyNguoichoi.Instance.FarmMode;
            }

            // duongrs fix khong hien thi phan thuong chest o cuoi tran
            // feo hien thi rieng tung loai
            if (QuanlyNguoichoi.Instance.IsNormalMode)
            {
                List<string> listRuong = Hiker.Util.ListPool<string>.Claim();
                QuanlyNguoichoi.Instance.GetRuongDaMo(listRuong);

                var chapCfg = ConfigManager.chapterConfigs[QuanlyNguoichoi.Instance.ChapterIndex];

                foreach (var t in listRuong)
                {
                    if (chapCfg.Chests.ContainsKey(t))
                    {
                        var chestCfg = chapCfg.Chests[t];
                        chestReward.MergeReward(chestCfg);
                    }
                }
                Hiker.Util.ListPool<string>.Release(listRuong);
            }
            UserInfo uInfo = GameClient.instance.UInfo;
            //show btn ads thuong vang
            if (QuanlyNguoichoi.Instance.IsNormalMode || QuanlyNguoichoi.Instance.IsLeoThapMode || QuanlyNguoichoi.Instance.IsSanTheMode)
            {
                if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo()
                    && uInfo.adsData.listAdsThemVangChapterIDs.Count < ConfigManager.GetMaxAdsThemVangChapterPerDay())
                {
                    var chapCfg = ConfigManager.chapterConfigs[QuanlyNguoichoi.Instance.ChapterIndex];
                    btnWatchAdsThemVang.gameObject.SetActive(true);
                    lbGetMore.gameObject.SetActive(true);
                    chapIdx = QuanlyNguoichoi.Instance.ChapterIndex;
                    vangThuongThem = Mathf.CeilToInt(ConfigManager.GetWatchAdsThemVangPercent(chapIdx) * 0.01f * chapCfg.BaseGold);
                    lbMoreGoldAmount.text = "+" + vangThuongThem;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(lbMoreGoldAmount.transform.parent as RectTransform);
                }
                else
                {
                    btnWatchAdsThemVang.gameObject.SetActive(false);
                    lbGetMore.gameObject.SetActive(false);
                }
            }

            if (QuanlyNguoichoi.Instance.IsSanTheMode && isVictory)
            {
                var item = Instantiate(phanThuongPrefab, phanThuongParent);
                long quantity = 1;
                item.SetItem(CardReward.CARD_CHEST, quantity);
                item.gameObject.SetActive(true);
            }

            if (uInfo.halloweenServerData != null
                && uInfo.halloweenServerData.StartTime < TimeUtils.Now && TimeUtils.Now < uInfo.halloweenServerData.StopDropTime
                && playerLoot.ContainsKey("Gold"))
            {
                var chapCfg = ConfigManager.chapterConfigs[QuanlyNguoichoi.Instance.ChapterIndex];
                long quantity = (int)(playerLoot["Gold"] * rate * ConfigManager.HalloweenEventConfig.BaseCandy / chapCfg.BaseGold);
                if (quantity > 0)
                {
                    var item = Instantiate(phanThuongPrefab, phanThuongParent);
                    item.SetItem("M_HalloweenCandy", quantity);
                    item.gameObject.SetActive(true);
                }
            }

            if (uInfo.giangSinhSrvData != null
                && uInfo.giangSinhSrvData.StartTime < TimeUtils.Now && TimeUtils.Now < uInfo.giangSinhSrvData.EndTime
                && playerLoot.ContainsKey("Gold"))
            {
                var chapCfg = ConfigManager.chapterConfigs[QuanlyNguoichoi.Instance.ChapterIndex];
                long quantity = (int)(playerLoot["Gold"] * rate * ConfigManager.GiangSinhCfg.MaterialBase / chapCfg.BaseGold);
                if (quantity > 0)
                {
                    var item = Instantiate(phanThuongPrefab, phanThuongParent);
                    item.SetItem(ConfigManager.GiangSinhCfg.Material, quantity);
                    item.gameObject.SetActive(true);
                }
            }

            // duongrs fix khong hien thi phan thuong chest o cuoi tran
            goldItem = null;
            foreach (var t in playerLoot)
            {
                var item = Instantiate(phanThuongPrefab, phanThuongParent);
                long quantity = t.Value * (CardReward.IsResourceOrMaterial(t.Key) ? rate : 1);
                if(t.Key == "Gold")
                {
                    goldItem = item;
                    goldData = new KeyValuePair<string, long>(t.Key, quantity);
                }
                item.SetItem(t.Key, quantity);
                item.gameObject.SetActive(true);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(phanThuongParent.parent as RectTransform);
            
            if(playerLoot.Count > 0 && uInfo.Gamer.PremiumEndTime > GameClient.instance.ServerTime)
            {
                foreach (var t in playerLoot)
                {
                    if (!CardReward.IsGetBonusFromPremium(t.Key)) continue;

                    var item = Instantiate(phanThuongPrefab, phanThuongPremiumBonusParent);
                    long quantity = Mathf.CeilToInt(t.Value * (CardReward.IsResourceOrMaterial(t.Key) ? rate : 1) * ConfigManager.GetPremiumBonusResource() / 100f);
                    
                    item.SetItem(t.Key, quantity);
                    item.gameObject.SetActive(true);
                }
                phanThuongPremiumBonusParent.parent.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(phanThuongPremiumBonusParent.parent as RectTransform);
            }
            else
            {
                phanThuongPremiumBonusParent.parent.gameObject.SetActive(false);
            }

            if(chestReward.Count > 0)
            {
                foreach (var t in chestReward)
                {
                    var item = Instantiate(phanThuongPrefab, phanThuongChestParent);
                    long quantity = t.Value * (CardReward.IsResourceOrMaterial(t.Key) ? rate : 1);
                    item.SetItem(t.Key, quantity);
                    item.gameObject.SetActive(true);
                }
                phanThuongChestParent.parent.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(phanThuongChestParent.parent as RectTransform);
            }
            else
            {
                phanThuongChestParent.parent.gameObject.SetActive(false);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(phanThuongParent.parent.parent as RectTransform);

            if (QuanlyNguoichoi.Instance.IsNienThuMode)
            {
                phanThuongPremiumBonusParent.parent.gameObject.SetActive(false);
                phanThuongChestParent.parent.gameObject.SetActive(false);
                phanThuongParent.parent.gameObject.SetActive(false);
                lbPointNienThuSumarize.transform.parent.gameObject.SetActive(true);
                lbPointNienThuSumarize.text = string.Format(Localization.Get("NienThuPointSumarizeLabel"),
                    QuanlyNguoichoi.Instance.NienThuBattlePointSumarize);
            }
            else
            {
                lbPointNienThuSumarize.transform.parent.gameObject.SetActive(false);
            }
        }

        [GUIDelegate]
        public override void OnCloseBtnClick()
        {
            if (QuanlyNguoichoi.Instance)
            {
                QuanlyNguoichoi.Instance.StartCoroutine(QuanlyNguoichoi.Instance.CoBackToMain());
            }
            base.OnCloseBtnClick();
        }

        [GUIDelegate]
        public void OnBtnWatchAdsThemVangClick()
        {
            if (preventDoubleClick) return;
            AnalyticsManager.LogEvent("WATCH_ADS_THEMVANG");

            if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
            {
                HikerAdsManager.instance.ShowVideoAds(this.RequestReceiveReward, this.RequestCancelVideo,
                                                      this.RequestFailVideo, this.RequestCancelVideo);
                StartCoroutine(PreventDoubleclick(5));

            }
            else
            {
                AnalyticsManager.LogEvent("NO_ADS_AVAILABLE");
                GameClient.instance.RequestWatchAdsThemVang(chapIdx);
            }
        }

        private void RequestReceiveReward()
        {
            AnalyticsManager.LogEvent("FINISH_ADS_THEMVANG");
            GameClient.instance.RequestWatchAdsThemVang(chapIdx);
            //Dismiss();
        }

        private void RequestCancelVideo()
        {
            PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("watch_ads_cancel"));
            //Dismiss();
        }

        private void RequestFailVideo()
        {
            PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("watch_ads_fail"));
            //Dismiss();
        }

        public void OnCompleteWatchAdsThemVang(int _soLuongVangThem)
        {
            if (btnWatchAdsThemVang == null || btnWatchAdsThemVang.transform == null) return;

            btnWatchAdsThemVang.gameObject.SetActive(false);
            if (lbGetMore != null)
                lbGetMore.gameObject.SetActive(false);

            if (listGoldIcons != null)
            {
                foreach (TweenPosition twGold in listGoldIcons)
                {
                    if (twGold == null)
                    {
                        continue;
                    }
                    Vector3 startPos = new Vector3(Random.Range(-175f, 175f), Random.Range(-75f, 75f), 0);
                    twGold.transform.localPosition = startPos;
                    twGold.from = startPos;
                    twGold.to = Vector3.zero;
                    twGold.ResetToBeginning();
                    twGold.enabled = true;
                    twGold.PlayForward();
                }
            }
            if (twGrpGolds && twGrpGolds.transform)
            {
                twGrpGolds.transform.localPosition = btnWatchAdsThemVang.transform.localPosition;
                twGrpGolds.from = btnWatchAdsThemVang.transform.localPosition;
                if (goldItem == null || phanThuongParent == null)
                    twGrpGolds.to = new Vector3(0, -100, 0);
                else
                    twGrpGolds.to = new Vector3(0, -100, 0) + phanThuongParent.localPosition + goldItem.transform.localPosition;
                twGrpGolds.ResetToBeginning();
                twGrpGolds.enabled = true;
                twGrpGolds.gameObject.SetActive(true);
                twGrpGolds.PlayForward();
            }
        }

        private IEnumerator PreventDoubleclick(float time)
        {
            preventDoubleClick = true;
            yield return new WaitForSecondsRealtime(time);
            preventDoubleClick = false;
        }

        protected override void OnCleanUp()
        {
            if (PopupBattlePause.instance)
            {
                PopupBattlePause.instance.OnCloseBtnClick();
            }
            ScreenBattle.PauseGame(false);
        }
    }

}

