using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    using UnityEngine.UI.Extensions;

    public class PopupTetNienThu : PopupBase
    {
        public Text lbTimeLeft;
        public Text lbClaimRewardTime;
        public Text lbDescription;

        public Button btnRewards;

        public Button btnBattleTheLuc;

        public Text lbLuotConLai;
        public GameObject alertReward;

        public static PopupTetNienThu instance;

        bool activeResourceBar = false;

        System.DateTime expireTime;
        System.DateTime claimExpireTime;

        TetEventServerData srvData;
        TetEventGamerData gamerData;

        //int luotFree = 0;
        //int curCost = 0;

        public static PopupTetNienThu Create(TetEventServerData srvData, TetEventGamerData gamerData)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupTetNienThu");
            instance = go.GetComponent<PopupTetNienThu>();
            instance.Init(srvData, gamerData);
            return instance;
        }

        public void Init(TetEventServerData srvData, TetEventGamerData gamerData)
        {
            this.srvData = srvData;
            this.gamerData = gamerData;

            activeResourceBar = true;
            if (ResourceBar.instance)
            {
                activeResourceBar = ResourceBar.instance.gameObject.activeSelf;
                ResourceBar.instance.gameObject.SetActive(false);
            }

            expireTime = srvData.StopDropTime;
            claimExpireTime = srvData.EndTime;

            UpdateTimeLeft();

            //luotFree = 0;
            //foreach (var c in ConfigManager.ThachDauCfg.PlayCostPerDays)
            //{
            //    if (c == 0) luotFree++;
            //}

            if (gamerData != null)
            {
                var luotConLai = ConfigManager.TetEventCfg.LuotDanhMotNgay - gamerData.LuotChoi;

                if(luotConLai > 0)
                    lbLuotConLai.text = string.Format(Localization.Get("TetNienThuLuotConLaiLabel"), luotConLai, ConfigManager.TetEventCfg.LuotDanhMotNgay);
                else
                    lbLuotConLai.text = string.Format(Localization.Get("TetNienThuLuotConLaiLabelRed"), luotConLai, ConfigManager.TetEventCfg.LuotDanhMotNgay);
                btnBattleTheLuc.interactable = luotConLai > 0;

                bool haveUnclaimRewards = false;
                int crLevel = gamerData.GetCurrentLevel();
                for(int i = 0;i < crLevel; i++)
                {
                    if(gamerData.receivedRewards.Contains(i) == false || gamerData.receivedPremiumRewards.Contains(i) == false)
                    {
                        haveUnclaimRewards = true;
                        break;
                    }
                }
                alertReward.SetActive(haveUnclaimRewards);

            }
        }

        void UpdateTimeLeft()
        {
            var srvTime = GameClient.instance.ServerTime;
            var ts = expireTime - srvTime;

            lbTimeLeft.text = Localization.Get("EventExpireTime") + " " +
                string.Format(Localization.Get("TimeCountDownDHMS"),
                    Mathf.FloorToInt((float)ts.TotalDays),
                    ts.Hours, ts.Minutes, ts.Seconds);

            ts = claimExpireTime - srvTime;

            lbClaimRewardTime.text = Localization.Get("EventClaimExpireTime") + " " +
                string.Format(Localization.Get("TimeCountDownDHMS"),
                    Mathf.FloorToInt((float)ts.TotalDays),
                    ts.Hours, ts.Minutes, ts.Seconds);
        }

        [GUIDelegate]
        public void OnBtnRewards()
        {
            PopupTetEventBattlePass.Create();
        }

        [GUIDelegate]
        public void OnBtnPlayBattleTheLuc()
        {
            if (gamerData != null && gamerData.LuotChoi < ConfigManager.TetEventCfg.LuotDanhMotNgay)
            {
                // request Battle
                GameClient.instance.RequestStartBattleNienThu();
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