using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hiker.Networks.Data.Shootero;
using Hiker.GUI;
using Hiker.GUI.Shootero;

public class LeoThapTopItem : MonoBehaviour
{
    public Text lbPos;
    public Image iconPos;

    public GameObject playerTick;

    public Text lbUserName;
    public Text lbLevel;
    public Text lbTime;
    public GameObject btnReward;

    int pos;
    int rankIdx;

    public void SetInfo(int pos, int rankIdx, GamerLeoThapData playerInfo, bool isPlayer)
    {
        this.rankIdx = rankIdx;
        this.pos = pos;
        lbPos.text = pos.ToString();

        if (playerInfo != null)
        {
            lbUserName.text = playerInfo.Name;
            if (playerInfo.Level != 0 || playerInfo.ClearedTime != 0)
            {
                lbLevel.text = string.Format(Localization.Get("LeoThapLevel"), playerInfo.Level);
                var minutePlay = playerInfo.ClearedTime / 60;
                var secondPlay = playerInfo.ClearedTime % 60;
                lbTime.text = string.Format(Localization.Get("LeoThapTime"), minutePlay, secondPlay);
            }
            else
            {
                lbLevel.text = string.Format(Localization.Get("LeoThapLevel"), "---");
                lbTime.text = "--- ---";
            }

            playerTick.gameObject.SetActive(isPlayer);
        }
        else
        {
            lbUserName.text = "---";
            lbLevel.text = string.Format(Localization.Get("LeoThapLevel"), "---");
            lbTime.text = "--- ---";
        }
    }

    [GUIDelegate]
    public void OnBtnRewardClick()
    {
        PopupShowPhanThuongTop.Create(ConfigManager.LeoThapCfg.GetTopRewards(pos - 1, rankIdx));
    }
}
