using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    using UnityEngine.UI.Extensions;

    public class PopupLeoThap : PopupBase
    {
        public Text lbDoKho;
        public Text lbRewardRate;
        public Text lbTimeLeft;
        public LeoThapTopItem[] topItems;
        public LeoThapTopItem[] lastTopItems;

        public GameObject grpNormal;
        public GameObject grpLastTop;

        public Button btnClamReward;

        public Button btnBattleTheLuc;
        public Button btnBattleGem;
        public Button btnContinue;

        public Text gemCost;
        public Text theLucCost;
        public Text lbAttempt;

        public MaterialAvatar avatarStoneReward;

        public RectTransform rewardGrp;

        public Image modAvatar;
        public Text modText;
        public RectTransform modGrp;
        public SpriteCollection modCollection;

        public GameObject grpLastBattleSummary;
        public PhanThuongItem phanThuongPrefab;
        public Transform phanThuongParent;
        public Text lbLastFloor;
        public Text lbNormalGemCost;
        public Text lbQuickGemCost;

        List<PhanThuongItem> mLastBattlePhanThuongItems = new List<PhanThuongItem>();

        public static PopupLeoThap instance;

        bool activeResourceBar = false;

        System.DateTime expireTime;
        LeoThapDataResponse response;
        int curCost = 0;
        GamerLeoThapData myData = null;
        int luotFree = 0;

        public static PopupLeoThap Create(LeoThapDataResponse data)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupLeoThap");
            instance = go.GetComponent<PopupLeoThap>();
            instance.Init(data);
            return instance;
        }

        public void Init(LeoThapDataResponse data)
        {
            HideLastBattleSummary();

            if (ScreenMain.instance && ScreenMain.instance.grpChapter)
                ScreenMain.instance.grpChapter.lastTimeCheck = GameClient.instance.ServerTime.AddHours(1);

            this.response = data;
            activeResourceBar = true;
            avatarStoneReward.SetItem("M_HeroStone", 0);
            if (ResourceBar.instance)
            {
                activeResourceBar = ResourceBar.instance.gameObject.activeSelf;
                ResourceBar.instance.gameObject.SetActive(false);
            }

            var grpData = data.curGrp;
            var top = data.curTop;

            if (grpData != null)
            {
                lbDoKho.text = Localization.Get("LeoThapName_Rank" + (grpData.RankIdx + 1));
                var rankCfg = ConfigManager.LeoThapCfg.GetLeoThapRankCfg(grpData.RankIdx);
                lbRewardRate.text = string.Format(Localization.Get("LeoThapRewardRate"),
                    rankCfg.RewardRate / 100f);
                expireTime = grpData.ETime;

                GameClient.instance.UInfo.LeoThapRankIdx = grpData.RankIdx;
            }

            if (top != null)
            {
                for (int i = 0; i < topItems.Length; ++i)
                {
                    if (top.Count > i)
                    {
                        var playerData = top[i];

                        if (playerData.GID == GameClient.instance.GID)
                        {
                            myData = playerData;
                            var cost = ConfigManager.LeoThapCfg.GetCostStartBattle(playerData.LuotChoi);

                            GameClient.instance.UInfo.BattleLeoThap = response.BattleID;
                            GameClient.instance.UInfo.LuotLeoThap = playerData.LuotChoi;

                            if (cost > 0)
                            {
                                btnBattleGem.gameObject.SetActive(true);
                                btnBattleTheLuc.gameObject.SetActive(false);
                                gemCost.text = cost.ToString();
                                LayoutRebuilder.ForceRebuildLayoutImmediate(gemCost.transform.parent as RectTransform);
                            }
                            else
                            {
                                btnBattleGem.gameObject.SetActive(false);
                                btnBattleTheLuc.gameObject.SetActive(true);
                                theLucCost.text = "x" + ConfigManager.LeoThapCfg.TheLucReq;
                            }
                            curCost = cost;
                        }

                        topItems[i].SetInfo(i + 1, grpData.RankIdx, playerData, playerData.GID == GameClient.instance.GID);
                    }
                    else
                    {
                        topItems[i].SetInfo(i + 1, grpData.RankIdx, null, false);
                    }
                }
            }

            luotFree = 0;
            foreach (var c in ConfigManager.LeoThapCfg.PlayCostPerDays)
            {
                if (c == 0) luotFree++;
            }

            UpdateTimeLeft();

            var lastTop = data.lastTop;
            var lastGrp = data.lastGrp;

            bool getRewarded = true;

            if (lastTop != null)
            {
                for (int i = 0; i < lastTopItems.Length; ++i)
                {
                    if (lastTop.Count > i)
                    {
                        var playerData = lastTop[i];

                        if (playerData.GID == GameClient.instance.GID)
                        {
                            getRewarded = playerData.GetRewarded;
                        }

                        lastTopItems[i].SetInfo(i + 1, lastGrp.RankIdx, playerData,
                            playerData.GID == GameClient.instance.GID);
                    }
                    else
                    {
                        lastTopItems[i].SetInfo(i + 1, lastGrp.RankIdx, null, false);
                    }

                    lastTopItems[i].GetComponent<CanvasGroup>().alpha = 0;
                }
            }

            if (getRewarded)
            {
                var tweenPos = grpLastTop.GetComponent<TweenPosition>();
                tweenPos.PlayForward();
                tweenPos.Finish();
                grpLastTop.gameObject.SetActive(false);
                grpNormal.gameObject.SetActive(true);
                var tweenpos2 = grpNormal.GetComponent<TweenPosition>();
                tweenpos2.PlayForward();
                tweenpos2.Finish();
            }
            else
            {
                grpLastTop.gameObject.SetActive(true);
                var tweenPos = grpLastTop.GetComponent<TweenPosition>();
                tweenPos.ResetToBeginning();
                
                grpNormal.gameObject.SetActive(true);
                var tweenpos2 = grpNormal.GetComponent<TweenPosition>();
                tweenpos2.ResetToBeginning();

                var tweenScale = btnClamReward.GetComponentInParent<TweenScale>();
                btnClamReward.interactable = false;
                tweenScale.ResetToBeginning();
                tweenScale.enabled = false;
                StartCoroutine(CoDisplayLastTop());
            }

            btnContinue.gameObject.SetActive(string.IsNullOrEmpty(response.BattleID) == false);

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponentInChildren<VerticalLayoutGroup>().transform as RectTransform);

            if (grpData.Mod > 0)
            {
                modGrp.gameObject.SetActive(true);
                modAvatar.sprite = modCollection.GetSprite("Modifier" + grpData.Mod);
                modText.text = string.Format(Localization.Get("LeoThapModifier"),
                    Localization.Get("LeoThapModifier" + grpData.Mod));
                rewardGrp.gameObject.SetActive(false);
                modAvatar.GetComponent<BoundTooltipTrigger>().text = GetMessageByMod(grpData.Mod);
            }
            else
            {
                rewardGrp.gameObject.SetActive(true);
                modGrp.gameObject.SetActive(false);
            }
        }

        public static string GetMessageByMod(int grpMod)
        {
            string key = MessageTooltipKey[grpMod];
            if (string.IsNullOrEmpty(key)) return key;
            if (grpMod == 2)
            {
                return string.Format(Localization.Get(key), 100 - ConfigManager.LeoThapCfg.HeSoMauDracula);
            }
            else
            {
                return Localization.Get(key);
            }
        }

        static readonly string[] MessageTooltipKey = new string[]
        {
            string.Empty,
            "PopupKhongLoModifierMessage",
            "PopupMaCaRongModifierMessage",
            "PopupPopoModifierMessage",
            "PopupFlashModifierMessage",
            "PopupInfernalModifierMessage",
            "PopupDeathrattleModifierMessage",
        };

        public void TweenToNormalGrp()
        {
            grpLastTop.gameObject.SetActive(true);
            var tweenPos = grpLastTop.GetComponent<TweenPosition>();
            tweenPos.PlayForward();

            grpNormal.gameObject.SetActive(true);
            var tweenpos2 = grpNormal.GetComponent<TweenPosition>();
            tweenpos2.PlayForward();
        }

        void UpdateTimeLeft()
        {
            var ts = expireTime - GameClient.instance.ServerTime;
            lbTimeLeft.text = string.Format(Localization.Get("TimeCountDownHMS"),
                Mathf.FloorToInt((float)ts.TotalHours), ts.Minutes, ts.Seconds);

            if (myData != null && myData.LuotChoi < luotFree)
            {
                lbAttempt.text = string.Format(Localization.Get("LuotLeoThapFree"), luotFree - myData.LuotChoi);
            }
            else
            {
                var curTime = GameClient.instance.ServerTime;
                var nextDay = curTime.Date.AddDays(1);
                var ts2 = nextDay - curTime;
                string timeCoolDown = string.Format(Localization.Get("TimeCountDownHMS"),
                    ts2.Hours, ts2.Minutes, ts2.Seconds);
                lbAttempt.text = string.Format(Localization.Get("LuotLeoThapCooldown"), timeCoolDown);
            }
        }

        IEnumerator CoDisplayLastTop()
        {
            for (int i = 0; i < lastTopItems.Length; ++i)
            {
                var canvasGrp = lastTopItems[i].GetComponent<CanvasGroup>();

                TweenAlpha.Begin(canvasGrp.gameObject, 0.15f, 1);
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.1f);
            var tweenScale = btnClamReward.GetComponentInParent<TweenScale>();
            btnClamReward.interactable = true;
            tweenScale.ResetToBeginning();
            tweenScale.enabled = true;
        }

        [GUIDelegate]
        public void OnBtnClaimReward()
        {
            //PopupMessage.Create(MessagePopupType.TEXT, "duong chua la`m");
            //TweenToNormalGrp();

            GameClient.instance.RequestGetPhanThuongTopLeoThap();
        }

        [GUIDelegate]
        public void OnBtnContinueBattle()
        {
            //GameClient.instance.UInfo.BattleLeoThap = string.Empty;
            GameClient.instance.RequestStartBattleLeoThap(response.BattleID);
        }

        [GUIDelegate]
        public void OnBtnPlayBattleTheLuc()
        {
            var uInfo = GameClient.instance.UInfo;
            int theLuc = ConfigManager.LeoThapCfg.TheLucReq;

            var theLucRegenTime = ConfigManager.GetGamerTheLucRegenSeconds();
            if (uInfo.Gamer.TheLuc.Val < theLuc)
            {
                uInfo.Gamer.TheLuc.UpdateByTime(GameClient.instance.ServerTime, theLucRegenTime);
            }

            if (uInfo.Gamer.TheLuc.Val >= theLuc)
            {
                // request Battle
                GameClient.instance.RequestStartBattleLeoThap();
            }
            else
            {
                PopupMissingTheLuc.Create();
            }
        }

        [GUIDelegate]
        public void OnBtnPlayBattleGem()
        {
            var uInfo = GameClient.instance.UInfo;
            // duongrs bo check gem o day
            //if (uInfo.Gamer.Gem >= curCost)
            {
                GameClient.instance.RequestGetLastBattleLeoThapReward();
            }
            //else
            //{
            //    PopupBuyGem.Create();
            //}
        }

        public void ShowLastBattleSummary(CardReward rewards, int lastFloor)
        {
            lbLastFloor.text = string.Format(Localization.Get("LeoThapLevel"), lastFloor);
            lbNormalGemCost.text = gemCost.text;
            lbQuickGemCost.text = gemCost.text;

            int c = 0;
            foreach (var t in rewards)
            {
                PhanThuongItem item = null;
                if (c < mLastBattlePhanThuongItems.Count) item = mLastBattlePhanThuongItems[c];
                if (item == null)
                {
                    item = Instantiate(phanThuongPrefab, phanThuongParent);
                    mLastBattlePhanThuongItems.Add(item);
                }
                item.SetItem(t.Key, t.Value);
                item.gameObject.SetActive(true);
                c++;
            }

            if(c < mLastBattlePhanThuongItems.Count - 1)
            {
                for(int i = c;i < mLastBattlePhanThuongItems.Count; i++)
                {
                    mLastBattlePhanThuongItems[i].gameObject.SetActive(false);
                }
            }

            grpLastBattleSummary.SetActive(true);

            LayoutRebuilder.ForceRebuildLayoutImmediate(phanThuongParent.parent as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(lbNormalGemCost.transform.parent as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(lbQuickGemCost.transform.parent as RectTransform);
        }

        [GUIDelegate]
        public void HideLastBattleSummary()
        {
            grpLastBattleSummary.SetActive(false);
        }

        [GUIDelegate]
        public void OnBtnPlayNormalBattleGem()
        {
            var uInfo = GameClient.instance.UInfo;
            if (uInfo.Gamer.Gem >= curCost)
            {
                GameClient.instance.RequestStartBattleLeoThap();
            }
            else
            {
                PopupBuyGem.Create();
            }
        }
        [GUIDelegate]
        public void OnBtnPlayQuickBattleGem()
        {
            var uInfo = GameClient.instance.UInfo;
            if (uInfo.Gamer.Gem >= curCost)
            {
                GameClient.instance.RequestQuickLeoThapBattle();
            }
            else
            {
                PopupBuyGem.Create();
            }
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