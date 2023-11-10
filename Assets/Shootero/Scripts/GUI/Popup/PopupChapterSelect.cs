using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using DanielLochner.Assets.SimpleScrollSnap;
    using UnityEngine.UI;

    public class PopupChapterSelect : PopupBase
    {
        public ChapterInfo chapterInfoPrefab;
        public Transform chapterParents;
        public GameObject pageIndicatorPrefab;
        public SimpleScrollSnap snap;

        public static PopupChapterSelect instance;
        bool mInitedItems = false;

        public static PopupChapterSelect Create(int curChapterIdx)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupChapterSelect");
            instance = go.GetComponent<PopupChapterSelect>();
            instance.Init(curChapterIdx);
            return instance;
        }

        private void Init(int curChapIdx)
        {
            if (mInitedItems) return;
            if (GameClient.instance == null ||
                GameClient.instance.UInfo == null)
            {
                return;
            }

            for (int i = 0; i < ConfigManager.GetNumChapter(); ++i)
            {
                var newItem = Instantiate(chapterInfoPrefab, chapterParents);
                newItem.gameObject.SetActive(true);
                newItem.SetChapter(i);

                var pageIndicator = Instantiate(pageIndicatorPrefab, snap.pagination.transform);
                pageIndicator.gameObject.SetActive(true);
            }

            HikerUtils.DoAction(this, () => snap.GoToPanel(curChapIdx), 0.1f, true);
            

            mInitedItems = true;
        }

        public void OnSelectChapter(int chapterIdx)
        {
            ScreenMain.instance.grpChapter.OnSelectChapter(chapterIdx);
            OnCloseBtnClick();
        }
    }
}