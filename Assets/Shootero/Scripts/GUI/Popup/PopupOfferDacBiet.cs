using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    public class PopupOfferDacBiet : PopupBase
    {
        public static PopupOfferDacBiet instance;
        public List<SpecialOfferItem> items;
        public SpecialOfferItem itemPrefab;
        public GameObject emptyTextObj;
        public VerticalLayoutGroup itemGroup;

        public Text lbTitle;

        bool activeResourceBar;

        public static PopupOfferDacBiet Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupOfferDacBiet");
            instance = go.GetComponent<PopupOfferDacBiet>();
            instance.Init();
            return instance;
        }

        public void Init()
        {
            activeResourceBar = true;
            if (ResourceBar.instance)
            {
                activeResourceBar = ResourceBar.instance.gameObject.activeSelf;
                ResourceBar.instance.gameObject.SetActive(false);
            }

            var uInfo = GameClient.instance.UInfo;

            //special offers
            var listOffers = GameClient.instance.GetListLimitedTimeOffers();

            emptyTextObj.SetActive(listOffers == null || listOffers.Count == 0);

            string titlePromo = string.Empty;
            if (listOffers.Count > 0)
            {
                titlePromo = listOffers[0].GetConfig().GetPromo(Localization.language);
            }

            if (string.IsNullOrEmpty(titlePromo) == false)
            {
                lbTitle.text = titlePromo;
            }

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
            //LayoutRebuilder.ForceRebuildLayoutImmediate(itemGroup.transform as RectTransform);

            //PlayerPrefs.SetString("OfferHash_" + uInfo.GID,
            //    GameClient.GetHashStringListOffers(listOffers));
        }

        protected override void Hide()
        {
            base.Hide();
            if (ResourceBar.instance)
            {
                if (GUIManager.Instance.CurrentScreen == "Main")
                    activeResourceBar = true;

                ResourceBar.instance.gameObject.SetActive(activeResourceBar);
            }

            ScreenMain.instance.grpChapter.SyncNetworkData();
        }
    }
}