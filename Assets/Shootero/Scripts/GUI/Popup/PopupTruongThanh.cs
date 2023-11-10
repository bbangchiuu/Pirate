using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    public class PopupTruongThanh : PopupBase
    {
        public static PopupTruongThanh instance;
        public List<PopupTruongThanhItem> listItems;
        public GameObject itemPref;
        public Transform grpItems;
        public GameObject grpLocked;
        public ScrollRect scrollView;

        public static PopupTruongThanh Create()
        {
            UserInfo uInfo = GameClient.instance.UInfo;
            if (uInfo == null || uInfo.Gamer == null || uInfo.Gamer.TruongThanhData == null) return null;

            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupTruongThanh");
            instance = go.GetComponent<PopupTruongThanh>();
            instance.Init();
            return instance;
        }

        public void Init()
        {
            UserInfo uInfo = GameClient.instance.UInfo;

            var gemRewards = ConfigManager.TruongThanhCfg.GemRewards;

            var listChapAva = Resources.Load<SpriteCollection>("ChapterAvatar");
            float scrollPosition = 1;
            for (int i = 0; i < gemRewards.Length; i++)
            {
                int crChapter = ConfigManager.TruongThanhCfg.ChapterUnlock + i;
                Sprite spr = listChapAva.GetSprite(string.Format("MainUI_Chapter{0:00}", crChapter + 1));
                bool claimed = uInfo.Gamer.TruongThanhData.receivedChapters.Contains(crChapter);
                bool completed = (crChapter <= uInfo.GetCurrentChapter());
                bool passed = (crChapter + 1 <= uInfo.GetCurrentChapter());
                bool isLastIdx = (crChapter == ConfigManager.TruongThanhCfg.ChapterUnlock + gemRewards.Length - 1);
                if(scrollPosition == 1)
                {
                    if (claimed == false)
                    {
                        scrollPosition = 0.999f - (1f * i / gemRewards.Length);
                        Debug.Log("scrollPosition=" + scrollPosition);
                    }
                }
                if (i < listItems.Count)
                {
                    listItems[i].SetItem(crChapter, gemRewards[i], spr, claimed, completed, passed, isLastIdx);
                    listItems[i].gameObject.SetActive(true);
                }
                else
                {
                    GameObject obj = Instantiate(itemPref, grpItems) as GameObject;
                    obj.transform.localScale = Vector3.one;
                    PopupTruongThanhItem item = obj.GetComponent<PopupTruongThanhItem>();
                    item.SetItem(crChapter, gemRewards[i], spr, claimed, completed, passed, isLastIdx);
                    item.gameObject.SetActive(true);
                    listItems.Add(item);
                }
            }
            itemPref.SetActive(false);
            for (int i = gemRewards.Length; i < listItems.Count; i++)
            {
                listItems[i].gameObject.SetActive(false);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(grpItems as RectTransform);

            grpLocked.SetActive(uInfo.Gamer.TruongThanhData.purchased == false);
            scrollView.verticalNormalizedPosition = scrollPosition;
            Debug.Log("Set scrollPosition=" + scrollPosition);
        }

        public override void OnCloseBtnClick()
        {
            base.OnCloseBtnClick();
        }
    }
}