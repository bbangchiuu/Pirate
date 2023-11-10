using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Hiker.Networks.Data.Shootero;
using Hiker.GUI;
using UnityEngine.UI.Extensions;

public class BossHuntItem : MonoBehaviour
{
    public Text chapTitle;
    public Text gemLabel;
    public SpriteCollection bossAvatarCol;
    public Image bossAva;
    public Button claimBtn;
    public TweenScale tweenClaim;

    BossHuntData bossData;

    public bool SetItem(BossHuntData data)
    {
        int chapIdx = -1;
        ChapterConfig.BossHuntConfig cfg = null;
        for (int i = 0; i < ConfigManager.chapterConfigs.Length; ++i)
        {
            var chapCfg = ConfigManager.chapterConfigs[i];
            if (chapCfg.BossHunt.ContainsKey(data.Name))
            {
                chapIdx = i;
                cfg = chapCfg.BossHunt[data.Name];
                break;
            }
        }
        if (chapIdx >= 0 && cfg != null)
        {
            bossData = data;
            chapTitle.text = string.Format(Localization.Get("ShortChapterNameFormat"), chapIdx + 1);
            gemLabel.text = cfg.Reward.GetGem().ToString();

            bossAva.sprite = bossAvatarCol.GetSprite(cfg.Avatar);

            var grayScale = bossAva.GetComponent<UIGrayScaleEffect>();
            grayScale.SetGrayScale(data.Status == 0 ? 0f : 1f);

            claimBtn.interactable = (data.Status == 1);
            if (data.Status == 1)
            {
                tweenClaim.enabled = true;
            }
            else
            {
                tweenClaim.ResetToBeginning();
                tweenClaim.enabled = false;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(gemLabel.transform.parent as RectTransform);
            return true;
        }
        else
        {
            return false;
        }
    }

    [GUIDelegate]
    public void OnBtnClaim()
    {
        if (bossData != null)
        {
            GameClient.instance.RequestClaimBossHunt(bossData.Name);
        }
    }
}
