using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hiker.Networks.Data.Shootero;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;

    public class PopupFarmMode : PopupBase
    {
        public Text lbTheLucNorm;
        public Text lbTheLucDouble;
        public Text lbTheLucX4;
        public Button btnNorm;
        public Button btnDouble;
        public Button btnX4;

        int mChapIdx = 0;

        public static PopupFarmMode instance;

        public static PopupFarmMode Create(int curChap)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupFarmMode");
            instance = go.GetComponent<PopupFarmMode>();
            instance.Init(curChap);

            return instance;
        }

        public void Init(int curChap)
        {
            mChapIdx = curChap;
            var uInfo = GameClient.instance.UInfo;
            var theLucRegenTime = ConfigManager.GetGamerTheLucRegenSeconds();

            int theLucNorm = ConfigManager.GetTheLucFarmMode(1);
            int theLucDouble = ConfigManager.GetTheLucFarmMode(2);
            
            if (uInfo.Gamer.TheLuc.Val < theLucNorm ||
                uInfo.Gamer.TheLuc.Val < theLucDouble)
            {
                uInfo.Gamer.TheLuc.UpdateByTime(GameClient.instance.ServerTime, theLucRegenTime);
            }

            if(uInfo.Gamer.PremiumEndTime > GameClient.instance.ServerTime)
            {
                int theLucX4 = ConfigManager.GetTheLucFarmMode(4);
                lbTheLucX4.text = string.Format("x{0}", theLucX4);
                btnX4.gameObject.SetActive(true);
            }
            else
            {
                btnX4.gameObject.SetActive(false);
            }

            lbTheLucDouble.text = string.Format("x{0}", theLucDouble);
            lbTheLucNorm.text = string.Format("x{0}", theLucNorm);
        }

        void StartFarmMode(int farmMode)
        {
            var uInfo = GameClient.instance.UInfo;
            int theLuc = ConfigManager.GetTheLucFarmMode(farmMode);
            var theLucRegenTime = ConfigManager.GetGamerTheLucRegenSeconds();
            if (uInfo.Gamer.TheLuc.Val < theLuc)
            {
                uInfo.Gamer.TheLuc.UpdateByTime(GameClient.instance.ServerTime, theLucRegenTime);
            }

            if (uInfo.Gamer.TheLuc.Val >= theLuc)
            {
                // request Battle
                GameClient.instance.RequestStartBattle(mChapIdx, farmMode);
            }
            else
            {
                PopupMissingTheLuc.Create();
            }
        }

        [GUIDelegate]
        public void OnBtnSingleRate()
        {
            StartFarmMode(1);
            OnCloseBtnClick();
        }

        [GUIDelegate]
        public void OnBtnDoubleRate()
        {
            StartFarmMode(2);
            OnCloseBtnClick();
        }

        [GUIDelegate]
        public void OnBtn4Rate()
        {
            StartFarmMode(4);
            OnCloseBtnClick();
        }
    }

}

