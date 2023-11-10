using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    public class PopupMailBox : PopupBase
    {
        public static PopupMailBox instance;
        public List<MailItem> mailItems;
        public GameObject mailItemPref, emptyTextObj;
        public VerticalLayoutGroup mailGroup;
        public static PopupMailBox Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupMailBox");
            instance = go.GetComponent<PopupMailBox>();
            instance.Init();
            return instance;
        }

        public void Init()
        {
            emptyTextObj.SetActive(GameClient.instance.UInfo.ListMails == null || GameClient.instance.UInfo.ListMails.Count == 0);
            for (int i = 0; i < GameClient.instance.UInfo.ListMails.Count; i++)
            {
                if(i < mailItems.Count)
                {
                    mailItems[i].SetItem(GameClient.instance.UInfo.ListMails[i]);
                    mailItems[i].gameObject.SetActive(true);
                }
                else
                {
                    GameObject obj = Instantiate(mailItemPref, mailGroup.transform) as GameObject;
                    obj.transform.localScale = Vector3.one;
                    MailItem item = obj.GetComponent<MailItem>();
                    item.SetItem(GameClient.instance.UInfo.ListMails[i]);
                    item.gameObject.SetActive(true);
                    mailItems.Add(item);
                }
            }
            for(int i = GameClient.instance.UInfo.ListMails.Count; i < mailItems.Count; i++)
            {
                mailItems[i].gameObject.SetActive(false);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(mailGroup.transform as RectTransform);
        }

        protected override void Hide()
        {
            base.Hide();
            GameClient.instance.RequestCloseMailBox();
        }
    }
}