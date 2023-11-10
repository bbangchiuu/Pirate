using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    public class PopupSelectLanguage : PopupBase
    {
        public static PopupSelectLanguage instance;

        public VerticalLayoutGroup itemsGroup;
        public SelectLanguageItem itemPref;

        public List<SelectLanguageItem> listItems;

        public static PopupSelectLanguage Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupSelectLanguage");
            instance = go.GetComponent<PopupSelectLanguage>();
            instance.Init();
            return instance;
        }

        public void Init()
        {
            for (int i = 0; i < ConfigManager.supportLangues.Count; ++i)
            {
                if (i < listItems.Count)
                {
                    listItems[i].SetItem(ConfigManager.supportLangues[i]);
                }
                else
                {
                    var item = Instantiate(itemPref, itemsGroup.transform);
                    item.transform.localScale = Vector3.one;
                    item.gameObject.SetActive(true);
                    item.SetItem(ConfigManager.supportLangues[i]);
                    listItems.Add(item);
                }
            }
            for (int i = ConfigManager.supportLangues.Count; i < listItems.Count; ++i)
            {
                listItems[i].gameObject.SetActive(false);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(itemsGroup.transform as RectTransform);
        }

        public static void Dismiss()
        {
            if (instance != null)
            {
                instance.OnCloseBtnClick();
            }
        }
    }
}