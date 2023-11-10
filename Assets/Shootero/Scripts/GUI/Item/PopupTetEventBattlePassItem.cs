using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;

    public class PopupTetEventBattlePassItem : MonoBehaviour
    {
        public PhanThuongItem freeRewardItem, premiumRewardItem;
        public GameObject imgClaimedFree, imgClaimedPremium;
        public GameObject imgLockFree, imgLockPremium;
        public GameObject imgProgressBar, imgProgressBarBG;
        public Text lbLevel;
        public GameObject imgLevelCompleted;
        public Button btnClaimFree, btnClaimPremium;

        public void SetItem(int rewardIndex, int crEventLevel)
        {
            TetEventGamerData eventGamerData = GameClient.instance.UInfo.tetEventGamerData;
            if (eventGamerData == null) return;

            CardReward freeReward = ConfigManager.TetEventCfg.Rewards[rewardIndex];
            CardReward premiumReward = ConfigManager.TetEventCfg.PremiumRewards[rewardIndex];
            foreach (string rwKey in freeReward.Keys)
            {
                freeRewardItem.SetItem(rwKey, freeReward[rwKey]);
                break;
            }
            foreach (string rwKey in premiumReward.Keys)
            {
                premiumRewardItem.SetItem(rwKey, premiumReward[rwKey]);
                break;
            }

            int requiredLevel = rewardIndex + 1;
            lbLevel.text = requiredLevel + "";

            if (crEventLevel < requiredLevel)
            {
                imgClaimedFree.SetActive(false);
                imgClaimedPremium.SetActive(false);
                imgLockFree.SetActive(true);
                imgLockPremium.SetActive(true);
                imgProgressBar.SetActive(false);
                imgProgressBarBG.SetActive(true);
                imgLevelCompleted.SetActive(false);
            }
            else
            {
                imgLockFree.SetActive(false);
                imgLockPremium.SetActive(eventGamerData.purchased == false);
                imgClaimedFree.SetActive(eventGamerData.receivedRewards.Contains(rewardIndex));
                imgClaimedPremium.SetActive(eventGamerData.receivedPremiumRewards.Contains(rewardIndex));
                imgProgressBar.SetActive(requiredLevel < crEventLevel);
                imgProgressBarBG.SetActive(true);
                imgLevelCompleted.SetActive(true);
            }

            //last reward
            if (rewardIndex == ConfigManager.TetEventCfg.Rewards.Count - 1)
            {
                imgProgressBar.SetActive(false);
                imgProgressBarBG.SetActive(false);
            }
        }

        [GUIDelegate]
        public void OnBtnClaimFreeClick()
        {
            if (imgClaimedFree.activeSelf == true || imgLockFree.activeSelf == true || PopupTetEventBattlePass.instance == null) return;
            PopupTetEventBattlePass.instance.RequestClaimRewards();
        }

        [GUIDelegate]
        public void OnBtnClaimPremiumClick()
        {
            if (imgClaimedPremium.activeSelf == true || imgLockPremium.activeSelf == true || PopupTetEventBattlePass.instance == null) return;
            PopupTetEventBattlePass.instance.RequestClaimRewards();
        }
    }
}

