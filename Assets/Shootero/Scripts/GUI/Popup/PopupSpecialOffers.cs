using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    public class PopupSpecialOffers : PopupBase
    {
        public static PopupSpecialOffers instance;
        public List<SpecialOfferItem> items;
        public SpecialOfferItem itemPrefab;
        public GameObject emptyTextObj;
        public VerticalLayoutGroup itemGroup;

        public GameObject grpOffers;
        public GameObject grpDaily;
        public GameObject grpOffers_disable;
        public GameObject grpDaily_disable;
        public GameObject grpDailyAlert;

        public List<SpecialOfferItem> dailyOfferItems;

        public GameObject tabOffers;
        public GameObject tabDaily;

        public static PopupSpecialOffers Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupSpecialOffers");
            instance = go.GetComponent<PopupSpecialOffers>();
            instance.Init();
            if (instance.emptyTextObj.activeSelf && instance.tabDaily.activeSelf)
            {
                instance.OnDailyOfferTabBtnClick();
            }
            else
            {
                instance.OnSpecialOfferTabBtnClick();
            }
            return instance;
        }

        public void Init()
        {
            var uInfo = GameClient.instance.UInfo;

            //special offers
            var listOffers = GameClient.instance.GetListOffers();

            emptyTextObj.SetActive(listOffers == null || listOffers.Count == 0);

            for (int i = 0; i < listOffers.Count; i++)
            {
                if(i >= items.Count)
                {
                    var item = Instantiate(itemPrefab, itemGroup.transform);
                    var obj = item.gameObject;
                    obj.transform.localScale = Vector3.one;
                    items.Add(item);
                }

                items[i].SetItem(listOffers[i]);
                items[i].gameObject.SetActive(true);
            }
            for(int i = listOffers.Count; i < items.Count; i++)
            {
                items[i].gameObject.SetActive(false);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(itemGroup.transform as RectTransform);

            PlayerPrefs.SetString("OfferHash_" + uInfo.GID,
                GameClient.GetHashStringListOffers(listOffers));// Md5Hash.GetMd5Hash(LitJson.JsonMapper.ToJson(listOffers)));

            //daily offers
            var listDailyOffers = GameClient.instance.GetListDailyOffers();

            grpDailyAlert.SetActive(false);
            
            if (listDailyOffers.Count > 0)
            {
                tabDaily.SetActive(true);
                string saveKey = GameClient.instance.GID + "_" + "Daily_Offer_Free";
                if (GameClient.instance.ServerTime.DayOfYear != PlayerPrefs.GetInt(saveKey, -1))
                {
                    grpDailyAlert.SetActive(true);
                }

                for (int i = 0; i < dailyOfferItems.Count; i++)
                {
                    if (i < listDailyOffers.Count)
                    {
                        dailyOfferItems[i].SetItem(listDailyOffers[i]);
                        dailyOfferItems[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        dailyOfferItems[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                tabDaily.SetActive(false);
            }
            
        }
        [GUIDelegate]
        public void OnDailyOfferTabBtnClick()
        {
            grpDaily.SetActive(true);
            grpOffers.SetActive(false);
            grpDaily_disable.SetActive(false);
            grpOffers_disable.SetActive(true);

            string saveKey = GameClient.instance.GID + "_" + "Daily_Offer_Free";
            if (GameClient.instance.ServerTime.DayOfYear != PlayerPrefs.GetInt(saveKey, -1))
            {
                grpDailyAlert.SetActive(false);
                PlayerPrefs.SetInt(saveKey, GameClient.instance.ServerTime.DayOfYear);
            }
        }

        [GUIDelegate]
        public void OnSpecialOfferTabBtnClick()
        {
            grpDaily.SetActive(false);
            grpOffers.SetActive(true);
            grpDaily_disable.SetActive(true);
            grpOffers_disable.SetActive(false);
        }

        protected override void Hide()
        {
            base.Hide();
            ScreenMain.instance.grpChapter.SyncNetworkData();
        }
    }
}