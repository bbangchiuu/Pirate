using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hiker.Networks.Data.Shootero;
using Hiker.GUI;
using Hiker.GUI.Shootero;

public class ThachDauTopItem : MonoBehaviour
{
    public Text lbPos;
    public Image iconPos1;
    public Image iconPos2;
    public Image iconPos3;

    public GameObject playerTick;

    public Text lbUserName;
    public Text lbLevel;
    public Text lbTime;
    public GameObject btnReward;

    int pos;
    int numPlayer;

    public void SetInfo(int numPlayer, int pos,GamerThachDauData playerInfo, bool isPlayer)
    {
        this.numPlayer = numPlayer;
        this.pos = pos;
        lbPos.text = pos.ToString();

        var reward = ConfigManager.ThachDauCfg.GetTopRewards(numPlayer, pos - 1);
        if (reward != null)
        {
            iconPos1.gameObject.SetActive(pos == 1);
            iconPos2.gameObject.SetActive(pos == 2);
            iconPos3.gameObject.SetActive(pos == 3);
        }
        else
        {
            iconPos1.gameObject.SetActive(pos == 1);
            iconPos2.gameObject.SetActive(false);
            iconPos3.gameObject.SetActive(false);
        }

        lbPos.gameObject.SetActive(true);

        if (playerInfo != null)
        {
            lbUserName.text = playerInfo.Name;
            if (playerInfo.Level != 0 || playerInfo.ClearedTime != 0)
            {
                lbLevel.text = string.Format(Localization.Get("ThachDauLevel"), playerInfo.Level);
                var minutePlay = playerInfo.ClearedTime / 60;
                var secondPlay = playerInfo.ClearedTime % 60;
                lbTime.text = string.Format(Localization.Get("ThachDauTime"), minutePlay, secondPlay);
            }
            else
            {
                lbLevel.text = string.Format(Localization.Get("ThachDauLevel"), "---");
                lbTime.text = "--- ---";
            }

            playerTick.gameObject.SetActive(isPlayer);
        }
        else
        {
            lbUserName.text = "---";
            lbLevel.text = string.Format(Localization.Get("ThachDauLevel"), "---");
            lbTime.text = "--- ---";
        }
    }

    [GUIDelegate]
    public void OnBtnRewardClick()
    {
        //PopupShowPhanThuongTop.Create(ConfigManager.ThachDauCfg.GetTopRewards(numPlayer, pos - 1));
        PopupShowPhanThuongThachDau.Create(numPlayer);
    }
}
