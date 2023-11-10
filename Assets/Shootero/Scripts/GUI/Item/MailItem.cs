using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using TimeSpan = System.TimeSpan;

    public class MailItem : MonoBehaviour
    {
        MailData mailData;
        public Text titleText, timeText, contentText, claimText;
        public GameObject claimButtonObj, claimedTextObj;
        public List<PhanThuongItem> rewardItems;
        public GameObject rewardGroup;

        public Button btnGoLink;

        public void SetItem(MailData _mailData)
        {
            claimButtonObj.SetActive(false);
            btnGoLink.gameObject.SetActive(false);
            mailData = _mailData;
            titleText.text = mailData.name;
            TimeSpan ts = GameClient.instance.ServerTime - mailData.sendTime;
            string text;
            if (ts.TotalHours < 1)
            {
                text = Localization.Get("less_than_an_hour");
            }
            else if (ts.TotalDays < 1)
            {
                text = string.Format(Localization.Get("hour_ago"), (int)ts.TotalHours);
            }
            else
            {
                text = string.Format(Localization.Get("day_ago"), Mathf.Ceil((float)ts.TotalDays));
            }
            timeText.text = text;
            contentText.text = mailData.content.text;

            rewardGroup.SetActive(mailData.content.reward != null);
            if (mailData.content.reward != null)
            {
                CardReward rewards = mailData.content.reward.ShortenToCardReward();
                int c = 0;
                foreach (string rKey in rewards.Keys)
                {
                    int num = rewards[rKey];
                    if (num > 0)
                    {
                        if (c < rewardItems.Count)
                        {
                            rewardItems[c].SetItem(rKey, rewards[rKey]);
                        }
                        
                        c++;
                    }
                }
                for (int i = 0; i < rewardItems.Count; i++)
                {
                    rewardItems[i].gameObject.SetActive(i < c);
                }
                bool isRecaived = mailData.status == MailStatus.Received;
                claimButtonObj.SetActive(!isRecaived);
                claimedTextObj.SetActive(isRecaived);
                if (!isRecaived)
                {
                    claimText.text = (mailData.clientVersion > GameClient.GameVersion) ? Localization.Get("BtnUpdateGameClient") : Localization.Get("BtnClaim");
                }
            }
            if (!string.IsNullOrEmpty(mailData.content.link))
            {
                btnGoLink.gameObject.SetActive(true);
                claimButtonObj.SetActive(false);
                claimedTextObj.SetActive(false);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);
        }

        [GUIDelegate]
        public void OnClaimBtnClick()
        {
            if (mailData == null) return;
            if (mailData.clientVersion > GameClient.GameVersion)
            {
                #if UNITY_ANDROID
            string storeURL = (string)ConfigManager.otherConfig["androidStore"];
            Application.OpenURL(storeURL);
#elif UNITY_IOS
            string storeURL = (string)ConfigManager.otherConfig["iosStore"];
            Application.OpenURL(storeURL);
#else
                string storeURL = (string)ConfigManager.otherConfig["androidStore"];
                Application.OpenURL(storeURL);
                Debug.Log("Open URL " + storeURL);
#endif
            }
            else
            {
                GameClient.instance.RequestClaimMailReward(mailData);
            }
        }

        [GUIDelegate]
        public void OnGoLinkBtnClick()
        {
            if (mailData == null || mailData.content == null) return;
            if (!string.IsNullOrEmpty(mailData.content.link))
            {
                string storeURL = mailData.content.link;
                Application.OpenURL(storeURL);
                Debug.Log("Open URL " + storeURL);
            }
        }
    }
}

