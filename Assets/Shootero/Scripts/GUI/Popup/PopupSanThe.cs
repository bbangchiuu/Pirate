using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    using UnityEngine.UI.Extensions;

    public class PopupSanThe : PopupBase
    {
        public Button btnBattleTheLuc;
        public Button btnBattleGem;
        public Button btnContinue;
        public Button btnBattleAds;
        public Button btnReset;
        public Text gemResetCost;

        public Image bossAvaKilled;
        public Image bossAvatar;
        public Text bossName;

        public Text gemCost;
        public Text theLucCost;
        public Text lbAttempt;

        public Transform grpCard;
        public CardAvatar cardAvatarSample;
        public GameObject goldReward;

        public SpriteCollection bossAvaCol;

        public static PopupSanThe instance;

        bool activeResourceBar = false;
        SanTheDataResponse response;
        System.DateTime expireTime;
        int curCost = 0;
        int luotFree = 0;
        int curCostReset = 0;

        List<CardAvatar> listCards = new List<CardAvatar>();

        public static PopupSanThe Create(SanTheDataResponse data)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupSanThe");
            instance = go.GetComponent<PopupSanThe>();
            instance.Init(data);
            return instance;
        }

        public void Init(SanTheDataResponse data)
        {
            if (ScreenMain.instance && ScreenMain.instance.grpChapter)
                ScreenMain.instance.grpChapter.lastTimeCheck = GameClient.instance.ServerTime.AddHours(1);

            this.response = data;
            activeResourceBar = true;

            var sp = bossAvaCol.GetSprite(ConfigManager.GetBossAvatar(data.BossName));
            bossAvatar.sprite = sp;
            bossAvaKilled.sprite = sp;
            bossName.text = ConfigManager.GetBossName(data.BossName);

            if (ResourceBar.instance)
            {
                activeResourceBar = ResourceBar.instance.gameObject.activeSelf;
                ResourceBar.instance.gameObject.SetActive(false);
            }

            luotFree = 0;
            foreach (var c in ConfigManager.SanTheCfg.PlayCostPerDays)
            {
                if (c == 0) luotFree++;
            }

            UpdateTimeLeft();

            var cost = ConfigManager.SanTheCfg.GetCostStartBattle(data.LuotSanThe);

            GameClient.instance.UInfo.BattleSanThe = response.BattleID;
            GameClient.instance.UInfo.LuotSanThe = data.LuotSanThe;

            if (cost > 0)
            {
                btnBattleGem.gameObject.SetActive(true);
                btnBattleTheLuc.gameObject.SetActive(false);
                btnBattleAds.gameObject.SetActive(false);

                gemCost.text = cost.ToString();
                LayoutRebuilder.ForceRebuildLayoutImmediate(gemCost.transform.parent as RectTransform);
            }
            else
            {
                btnBattleGem.gameObject.SetActive(false);
                if (ConfigManager.SanTheCfg.LuotRequireAds <= 0 ||
                    (data.LuotSanThe + 1) < ConfigManager.SanTheCfg.LuotRequireAds)
                {
                    btnBattleTheLuc.gameObject.SetActive(true);
                    btnBattleAds.gameObject.SetActive(false);
                }
                else
                {
                    btnBattleTheLuc.gameObject.SetActive(false);
                    btnBattleAds.gameObject.SetActive(true);
                }
                theLucCost.text = "x" + ConfigManager.SanTheCfg.TheLucReq;
            }
            curCost = cost;

            btnContinue.gameObject.SetActive(string.IsNullOrEmpty(data.BattleID) == false);

            bossAvaKilled.gameObject.SetActive(data.IsWin);
            btnReset.gameObject.SetActive(data.IsWin);
            curCostReset = ConfigManager.SanTheCfg.GetCostReset(data.LuotReset);
            gemResetCost.text = curCostReset.ToString();
            if (btnReset.gameObject.activeInHierarchy)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(gemResetCost.transform.parent as RectTransform);
            }

            if (btnBattleTheLuc.gameObject.activeSelf)
            {
                btnBattleTheLuc.interactable = data.IsWin == false;
            }
            if (btnBattleGem.gameObject.activeSelf)
            {
                btnBattleGem.interactable = data.IsWin == false;
            }
            if (btnBattleAds.gameObject.activeSelf)
            {
                btnBattleAds.interactable = data.IsWin == false;
            }

            for (int i = 0; i < response.listCards.Length; ++i)
            {
                string cardName = response.listCards[i];
                var card = ConfigManager.GetCardConfig(cardName);
                if (i < listCards.Count)
                {
                    listCards[i].SetItem(cardName, card.Rarity);
                    listCards[i].gameObject.SetActive(true);
                }
                else
                {
                    var newCard = Instantiate(cardAvatarSample, grpCard);
                    newCard.SetItem(cardName, card.Rarity);
                    newCard.GetComponent<Button>().onClick.AddListener(() => OnListCardClick(newCard));
                    listCards.Add(newCard);
                    newCard.gameObject.SetActive(true);
                }
            }

            for (int i = response.listCards.Length; i < listCards.Count; ++i)
            {
                listCards[i].gameObject.SetActive(false);
            }
            goldReward.transform.SetAsLastSibling();

            cardAvatarSample.gameObject.SetActive(false);

            LayoutRebuilder.ForceRebuildLayoutImmediate(grpCard as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponentInChildren<VerticalLayoutGroup>().transform as RectTransform);
        }

        void OnListCardClick(CardAvatar card)
        {
            PopupCardInfo.CreateViewOnly(card.TBName);
        }

        void UpdateTimeLeft()
        {
            if (response != null && response.LuotSanThe < luotFree)
            {
                lbAttempt.text = string.Format(Localization.Get("LuotSanTheFree"), luotFree - response.LuotSanThe);
            }
            else
            {
                var curTime = GameClient.instance.ServerTime;
                var nextDay = curTime.Date.AddDays(1);
                var ts2 = nextDay - curTime;
                string timeCoolDown = string.Format(Localization.Get("TimeCountDownHMS"),
                    ts2.Hours, ts2.Minutes, ts2.Seconds);
                lbAttempt.text = string.Format(Localization.Get("LuotSanTheCooldown"), timeCoolDown);
            }
        }

        [GUIDelegate]
        public void OnBtnContinueBattle()
        {
            GameClient.instance.RequestStartBattleSanThe(response.BattleID);
        }

        [GUIDelegate]
        public void OnBtnPlayBattleTheLuc()
        {
            var uInfo = GameClient.instance.UInfo;
            int theLuc = ConfigManager.SanTheCfg.TheLucReq;

            var theLucRegenTime = ConfigManager.GetGamerTheLucRegenSeconds();
            if (uInfo.Gamer.TheLuc.Val < theLuc)
            {
                uInfo.Gamer.TheLuc.UpdateByTime(GameClient.instance.ServerTime, theLucRegenTime);
            }

            if (uInfo.Gamer.TheLuc.Val >= theLuc)
            {
                // request Battle
                GameClient.instance.RequestStartBattleSanThe();
            }
            else
            {
                PopupMissingTheLuc.Create();
            }
        }

        [GUIDelegate]
        public void OnBtnResetSanThe()
        {
            var uInfo = GameClient.instance.UInfo;
            if (uInfo.Gamer.Gem >= curCostReset)
            {
                GameClient.instance.RequestResetSanTheData();
            }
            else
            {
                PopupBuyGem.Create();
            }
        }

        [GUIDelegate]
        public void OnBtnPlayBattleGem()
        {
            var uInfo = GameClient.instance.UInfo;
            if (uInfo.Gamer.Gem >= curCost)
            {
                GameClient.instance.RequestStartBattleSanThe();
            }
            else
            {
                PopupBuyGem.Create();
            }
        }

        bool preventDoubleClick;
        [GUIDelegate]
        public void OnBattleXemQuangCao()
        {
            if (preventDoubleClick) return;
            AnalyticsManager.LogEvent("WATCH_ADS_BATTLE_CARD_HUNT");

            if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
            {
                HikerAdsManager.instance.ShowVideoAds(this.RequestReceiveReward, this.RequestCancelVideo,
                                                      this.RequestFailVideo, this.RequestCancelVideo);
                StartCoroutine(PreventDoubleclick(5));

            }
            else
            {
                AnalyticsManager.LogEvent("NO_ADS_AVAILABLE");
                RequestFailVideo();
            }
        }
        private IEnumerator PreventDoubleclick(float time)
        {
            preventDoubleClick = true;
            yield return new WaitForSecondsRealtime(time);
            preventDoubleClick = false;
        }

        private void RequestReceiveReward()
        {
            AnalyticsManager.LogEvent("FINISH_ADS_BATTLE_CARD_HUNT");
            GameClient.instance.RequestStartBattleSanThe();
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

        protected override void OnCleanUp()
        {
            if (ResourceBar.instance)
            {
                if (GUIManager.Instance.CurrentScreen == "Main")
                    activeResourceBar = true;

                ResourceBar.instance.gameObject.SetActive(activeResourceBar);
            }
        }

        private void Update()
        {
            UpdateTimeLeft();
        }
    }
}