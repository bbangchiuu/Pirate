using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    public class ChapterInfo : MonoBehaviour
    {
        public Text lbChapter;
        public Text lbFoundTreasure;
        public Image spChapterImage;
        public Image spChapterLocked;
        public Text lbDesc;
        public Button btnSelect;

        int chapterIdx;

        void SetFoundTreasure(int chapIdx)
        {
            int found = 0;
            int totalTreasure = 0;

            var uInfo = GameClient.instance.UInfo;
            var chap = uInfo.ListChapters != null ? uInfo.ListChapters.Find(e => e.ChapIdx == chapterIdx) : null;

            var m_NodeMap = Resources.Load<NodeMap>("MapDungeon/Chapter" + (chapIdx + 1));// GetComponentInChildren<NodeMap>();

            for (int i = 0; i < m_NodeMap.points.Count; ++i)
            {
                var p = m_NodeMap.points[i];
                var cfg = p.GetComponent<NodeLevelConfig>();
                if (cfg.LevelType == ENodeType.Ruong)
                {
                    totalTreasure++;
                    if (chap != null &&
                        chap.RuongOpened.Contains(cfg.RuongConfig))
                    {
                        found++;
                    }
                }
            }
            lbFoundTreasure.text = string.Format(Localization.Get("FoundTreasureLabel"), found, totalTreasure);
        }

        public void SetChapter(int chapterIdx)
        {
            this.chapterIdx = chapterIdx;


            //lbChapter.text = (chapterIdx + 1) + ". " + Localization.Get(string.Format("Chapter{0}_Name", chapterIdx + 1));
            string chapter_format = Localization.Get("ChapterNameFormat");
            lbChapter.text = string.Format(chapter_format, chapterIdx + 1, Localization.Get(string.Format("Chapter{0}_Name", chapterIdx + 1)));


            lbDesc.text = Localization.Get(string.Format("Chapter{0}_Desc", chapterIdx + 1));

            var uInfo = GameClient.instance.UInfo;
            if (uInfo.IsChapterUnLock(chapterIdx))
            {
                //lbTopStage.text = string.Format(Localization.Get("TopStage"),
                //    1,
                //    ConfigManager.chapterConfigs[chapterIdx].Missions.Length);
                spChapterLocked.gameObject.SetActive(false);
                btnSelect.interactable = true;
            }
            else
            {
                lbFoundTreasure.text = Localization.Get("Locked");
                btnSelect.interactable = false;
                spChapterLocked.gameObject.SetActive(true);
            }

            SetFoundTreasure(chapterIdx);

            var listChapAva = Resources.Load<SpriteCollection>("ChapterAvatar");
            spChapterImage.sprite = listChapAva.GetSprite(string.Format("MainUI_Chapter{0:00}", chapterIdx + 1));
            spChapterLocked.sprite = spChapterImage.sprite;
        }

        [GUIDelegate]
        public void OnBtnSelectClick()
        {
            PopupChapterSelect.instance.OnSelectChapter(chapterIdx);
        }
    }
}