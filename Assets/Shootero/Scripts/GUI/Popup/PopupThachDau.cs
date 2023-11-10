using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    using UnityEngine.UI.Extensions;

    public class PopupThachDau : PopupBase
    {
        public Text lbTimeLeft;
        public ThachDauTopItem[] topItems;
        public ThachDauTopItem[] lastTopItems;

        public GameObject grpNormal;
        public GameObject grpLastTop;
        public GameObject grpRules;

        public Text lbThachDauRule;

        public Text lbRewardGem;
        public Button btnClamReward;

        public Button btnBattleTheLuc;
        public Button btnContinue;
        public Button btnBattleGem;
        public Button btnJoinThachDau;
        public Button btnJoinThachDauGem;

        public Text lbCostThamGia;
        public Transform grpVeThachDau;

        public Text lbNormalGemCost;
        public Text lbHetLuotChoi;

        public Text lbLuotConLai;

        //public MaterialAvatar avatarStoneReward;

        public RectTransform rewardGrp;

        //List<PhanThuongItem> mLastBattlePhanThuongItems = new List<PhanThuongItem>();

        public static PopupThachDau instance;

        bool activeResourceBar = false;

        System.DateTime expireTime;
        ThachDauDataResponse response;
        GamerThachDauData myData = null;
        int luotFree = 0;
        int curCost = 0;

        public static PopupThachDau Create(ThachDauDataResponse data)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupThachDau");
            instance = go.GetComponent<PopupThachDau>();
            instance.Init(data);
            return instance;
        }

        public void Init(ThachDauDataResponse data)
        {
            if (ScreenMain.instance && ScreenMain.instance.grpChapter)
                ScreenMain.instance.grpChapter.lastTimeCheck = GameClient.instance.ServerTime.AddHours(1);

            this.response = data;
            activeResourceBar = true;
            //avatarStoneReward.SetItem("M_HeroStone", 0);
            if (ResourceBar.instance)
            {
                activeResourceBar = ResourceBar.instance.gameObject.activeSelf;
                ResourceBar.instance.gameObject.SetActive(false);
            }

            var grpData = data.curGrp;
            var top = data.curTop;

            if (grpData != null)
            {
                expireTime = grpData.ETime;
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

                            GameClient.instance.UInfo.BattleThachDau = response.BattleID;
                            GameClient.instance.UInfo.LuotThachDau = playerData.LuotChoi;

                            btnBattleTheLuc.gameObject.SetActive(true);

                            var cost = ConfigManager.ThachDauCfg.GetCostStartBattle(playerData.LuotChoi);

                            if (playerData.LuotChoi < ConfigManager.ThachDauCfg.PlayCostPerDays.Length)
                            {
                                lbHetLuotChoi.gameObject.SetActive(false);
                                if (cost > 0)
                                {
                                    btnBattleGem.gameObject.SetActive(true);
                                    btnBattleTheLuc.gameObject.SetActive(false);
                                    lbNormalGemCost.text = cost.ToString();
                                    LayoutRebuilder.ForceRebuildLayoutImmediate(lbNormalGemCost.transform.parent as RectTransform);
                                }
                                else
                                {
                                    btnBattleGem.gameObject.SetActive(false);
                                    btnBattleTheLuc.gameObject.SetActive(true);
                                }
                            }
                            else
                            {
                                btnBattleGem.gameObject.SetActive(false);
                                btnBattleTheLuc.gameObject.SetActive(false);
                                lbHetLuotChoi.gameObject.SetActive(string.IsNullOrEmpty(response.BattleID));
                            }

                            curCost = cost;
                        }

                        topItems[i].SetInfo(top.Count, i + 1, playerData,
                            playerData.GID == GameClient.instance.GID);
                    }
                    else
                    {
                        topItems[i].SetInfo(top.Count, i + 1, null, false);
                    }
                }
            }

            UpdateTimeLeft();

            var lastTop = data.lastTop;
            var lastGrp = data.lastGrp;

            bool getRewarded = true;

            int lastPos = 0;
            if (lastTop != null)
            {
                for (int i = 0; i < lastTopItems.Length; ++i)
                {
                    if (lastTop.Count > i)
                    {
                        var playerData = lastTop[i];

                        if (playerData.GID == GameClient.instance.GID)
                        {
                            lastPos = i;
                            getRewarded = playerData.GetRewarded;
                        }

                        lastTopItems[i].SetInfo(lastTop.Count, i + 1, playerData,
                            playerData.GID == GameClient.instance.GID);
                    }
                    else
                    {
                        lastTopItems[i].SetInfo(lastTop.Count, i + 1, null, false);
                    }

                    lastTopItems[i].GetComponent<CanvasGroup>().alpha = 0;
                }
            }

            luotFree = 0;
            foreach (var c in ConfigManager.ThachDauCfg.PlayCostPerDays)
            {
                if (c == 0) luotFree++;
            }

            if (myData != null)
            {
                lbLuotConLai.text = string.Format(Localization.Get("ThachDauLuotConLai"), ConfigManager.ThachDauCfg.PlayCostPerDays.Length - myData.LuotChoi);
            }

            if (getRewarded)
            {
                var tweenPos = grpLastTop.GetComponent<TweenPosition>();
                tweenPos.PlayForward();
                tweenPos.Finish();
                grpLastTop.gameObject.SetActive(false);

                if (grpData != null)
                {
                    grpNormal.gameObject.SetActive(true);
                    grpRules.gameObject.SetActive(false);
                    var tweenpos2 = grpNormal.GetComponent<TweenPosition>();
                    tweenpos2.PlayForward();
                    tweenpos2.Finish();
                }
                else
                {
                    grpNormal.gameObject.SetActive(false);
                    grpRules.gameObject.SetActive(true);
                    var tweenpos2 = grpRules.GetComponent<TweenPosition>();
                    tweenpos2.PlayForward();
                    tweenpos2.Finish();

                    int haveNum = 0;
                    if (GameClient.instance.UInfo.ListMaterials != null)
                    {
                        var material = GameClient.instance.UInfo.ListMaterials.Find(e => e.Name == "M_VeThachDau");
                        if (material != null)
                        {
                            haveNum = material.Num;
                        }
                    }

                    btnJoinThachDau.interactable = (haveNum > 0);

                    lbCostThamGia.text = ConfigManager.ThachDauCfg.GetCostJoinThachDau().ToString();
                    LayoutRebuilder.ForceRebuildLayoutImmediate(lbCostThamGia.GetComponentInParent<HorizontalLayoutGroup>().transform as RectTransform);

                    LayoutRebuilder.ForceRebuildLayoutImmediate(grpVeThachDau as RectTransform);
                }
            }
            else
            {
                grpRules.gameObject.SetActive(false);
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

                var reward = ConfigManager.ThachDauCfg.GetTopRewards(lastTop.Count, lastPos);
                if (reward != null)
                {
                    rewardGrp.gameObject.SetActive(true);
                    lbRewardGem.text = reward.GetGem().ToString();
                }
                else
                {
                    rewardGrp.gameObject.SetActive(false);
                }
            }

            btnContinue.gameObject.SetActive(string.IsNullOrEmpty(response.BattleID) == false);

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponentInChildren<VerticalLayoutGroup>().transform as RectTransform);
            

            //rewardGrp.gameObject.SetActive(true);
        }

        public void TweenToNormalGrp(ThachDauDataResponse data)
        {
            response = data;

            grpLastTop.gameObject.SetActive(true);
            var tweenPos = grpLastTop.GetComponent<TweenPosition>();
            tweenPos.PlayForward();

            if (response.curGrp != null)
            {
                grpRules.gameObject.SetActive(false);
                grpNormal.gameObject.SetActive(true);
                var tweenpos2 = grpNormal.GetComponent<TweenPosition>();
                tweenpos2.PlayForward();
            }
            else
            {
                grpNormal.gameObject.SetActive(false);
                grpRules.gameObject.SetActive(true);
                var tweenpos2 = grpRules.GetComponent<TweenPosition>();
                tweenpos2.PlayForward();

                int haveNum = 0;
                if (GameClient.instance.UInfo.ListMaterials != null)
                {
                    var material = GameClient.instance.UInfo.ListMaterials.Find(e => e.Name == "M_VeThachDau");
                    if (material != null)
                    {
                        haveNum = material.Num;
                    }
                }

                btnJoinThachDau.interactable = (haveNum > 0);

                lbCostThamGia.text = ConfigManager.ThachDauCfg.GetCostJoinThachDau().ToString();
                LayoutRebuilder.ForceRebuildLayoutImmediate(lbCostThamGia.GetComponentInParent<HorizontalLayoutGroup>().transform as RectTransform);

                LayoutRebuilder.ForceRebuildLayoutImmediate(grpVeThachDau as RectTransform);
            }
        }

        void UpdateTimeLeft()
        {
            var ts = expireTime - GameClient.instance.ServerTime;
            lbTimeLeft.text = string.Format(Localization.Get("TimeCountDownHMS"),
                Mathf.FloorToInt((float)ts.TotalHours), ts.Minutes, ts.Seconds);
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
            GameClient.instance.RequestGetPhanThuongTopThachDau();
        }

        [GUIDelegate]
        public void OnBtnContinueBattle()
        {
            GameClient.instance.RequestStartBattleThachDau(response.BattleID);
        }

        [GUIDelegate]
        public void OnBtnPlayBattleTheLuc()
        {
            if (myData != null && myData.LuotChoi == 0)
            {
                // request Battle
                GameClient.instance.RequestStartBattleThachDau();
            }
        }

        [GUIDelegate]
        public void OnBtnPlayBattleGem()
        {
            var uInfo = GameClient.instance.UInfo;
            
            if (uInfo.Gamer.Gem >= curCost)
            {
                GameClient.instance.RequestStartBattleThachDau();
            }
            else
            {
                PopupBuyGem.Create();
            }
        }

        [GUIDelegate]
        public void OnBtnJoinThachDauClick()
        {
            var uInfo = GameClient.instance.UInfo;
            int haveNum = 0;
            if (uInfo.ListMaterials != null)
            {
                var material = uInfo.ListMaterials.Find(e => e.Name == "M_VeThachDau");
                if (material != null)
                {
                    haveNum = material.Num;
                }
            }

            if (haveNum > 0)
            {
                GameClient.instance.RequestJoinThachDau();
            }
        }

        [GUIDelegate]
        public void OnBtnJoinThachDauGemClick()
        {
            var uInfo = GameClient.instance.UInfo;
            
            if (uInfo.Gamer.Gem < ConfigManager.ThachDauCfg.GetCostJoinThachDau())
            {
                PopupBuyGem.Create();
            }
            else
            {
                GameClient.instance.RequestJoinThachDauGem();
            }
        }

        [GUIDelegate]
        public void OnBtnInfoClick()
        {
            PopupShowPhanThuongThachDau.Create(response.curTop.Count);
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