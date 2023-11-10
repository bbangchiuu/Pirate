using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;

    public class PopupTongHopQuangCaoItem : MonoBehaviour
    {
        public Button btnWatchAds;
        public Text lbRemainingToday, lbWatchedAdsCount;
        public List<PhanThuongItem> listPhanThuongItems;
        public GameObject grpWatchAdsCount;

        public void SetItem(CardReward rewardData, int remainTodayMax, int remainToday, int adsCountMax = 1, int adsCount = 0)
        {
            int c = 0;
            foreach(string rwKey in rewardData.Keys)
            {
                if(c < listPhanThuongItems.Count)
                {
                    listPhanThuongItems[c].SetItem(rwKey, rewardData[rwKey]);
                    listPhanThuongItems[c].gameObject.SetActive(true);
                    c++;
                }
            }
            for(int i = c; i < listPhanThuongItems.Count; i++)
            {
                listPhanThuongItems[i].gameObject.SetActive(false);
            }
            lbRemainingToday.text = string.Format(Localization.Get("lbRemainingToday"), remainToday, remainTodayMax);
            btnWatchAds.interactable = remainToday > 0;

            if(adsCountMax > 1 && adsCount < adsCountMax)
            {
                grpWatchAdsCount.SetActive(true);
                lbWatchedAdsCount.text = string.Format("{0}/{1}", adsCount, adsCountMax);
            }
            else
            {
                grpWatchAdsCount.SetActive(false);
            }
        }

        [GUIDelegate]
        public void OnBtnWatchAdsClick()
        {

        }
    }
}

