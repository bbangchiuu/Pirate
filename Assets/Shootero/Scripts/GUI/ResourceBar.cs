using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;

    public class ResourceBar : NetworkDataSync
    {
        public Text lbGem;
        public Text lbGold;
        public Text lbTheLuc;
        public Text lbTheLucRemainTime;
        public static ResourceBar instance;

        private void Awake()
        {
            instance = this;
            gameObject.SetActive(false);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SyncNetworkData();
        }

        private void Update()
        {
            if (GameClient.instance == null) return;

            var uInfo = GameClient.instance.UInfo;
            if (uInfo == null || uInfo.Gamer == null) return;

            if (uInfo.Gamer.TheLuc.Val < uInfo.Gamer.TheLuc.MaxVal)
            {
                var theLucRegenTime = ConfigManager.GetGamerTheLucRegenSeconds();
                if (GameClient.instance.ServerTime < uInfo.Gamer.TheLuc.LastUpdate.AddSeconds(theLucRegenTime))
                {
                    SyncNetworkData();
                }
                else
                {
                    uInfo.Gamer.TheLuc.UpdateByTime(GameClient.instance.ServerTime, theLucRegenTime);
                }
            }
        }

        public override void SyncNetworkData()
        {
            if (GameClient.instance == null) return;

            var uInfo = GameClient.instance.UInfo;
            if (uInfo == null || uInfo.Gamer == null) return;
            var theLucRegenTime = ConfigManager.GetGamerTheLucRegenSeconds();
            uInfo.Gamer.TheLuc.UpdateByTime(GameClient.instance.ServerTime, theLucRegenTime);

            var gamer = uInfo.Gamer;
            lbGem.text = gamer.Gem.ToString();
            lbGold.text = gamer.Gold.ToString();
            lbTheLuc.text = gamer.TheLuc.Val.ToString();// string.Format("{0}/{1}", gamer.TheLuc.Val, gamer.TheLuc.MaxVal);
            if (gamer.TheLuc.Val >= gamer.TheLuc.MaxVal)
            {
                lbTheLucRemainTime.text = Localization.Get("full").ToUpper();
            }
            else
            {
                var ts = uInfo.Gamer.TheLuc.LastUpdate.AddSeconds(theLucRegenTime) - GameClient.instance.ServerTime;
                lbTheLucRemainTime.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
            }
        }
        [GUIDelegate]
        public void OnBtnAddTheLuc()
        {
            //GameClient.instance.RequestAddTheLuc();
        }
        [GUIDelegate]
        public void OnBtnAddGem()
        {
            PopupBuyGem.Create();
        }
    }
}