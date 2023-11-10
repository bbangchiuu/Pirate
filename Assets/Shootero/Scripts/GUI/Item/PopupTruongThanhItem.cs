using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;

    public class PopupTruongThanhItem : MonoBehaviour
    {
        public Text lbGem;
        public Text lbChapter;
        public Image imgLocked;
        public Image imgChapter;
        public Image imgCompleted;
        public Image imgProgressBar, imgProgressBarBG;
        public Button btnClaim;

        int mChapter;

        public void SetItem(int chapter, int gem, Sprite sprite, bool claimed = false, bool completed = false, bool passed = false, bool isLastIdx = false)
        {
            mChapter = chapter;
            lbGem.text = "" + gem;
            imgChapter.sprite = sprite;

            lbChapter.text = (mChapter + 1) + "";
            btnClaim.gameObject.SetActive(claimed == false);
            imgProgressBarBG.gameObject.SetActive(isLastIdx == false);
            imgProgressBar.gameObject.SetActive(passed);
            imgCompleted.gameObject.SetActive(completed);
            btnClaim.interactable = completed;
            imgLocked.gameObject.SetActive(!completed);
        }

        [GUIDelegate]
        public void OnClaimBtnClick()
        {
            GameClient.instance.RequestClaimTruongThanh(mChapter);
        }
    }
}

