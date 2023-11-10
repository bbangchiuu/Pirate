using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using DanielLochner.Assets.SimpleScrollSnap;
    using UnityEngine.UI;

    public class PopupDungeonSelect : PopupBase
    {
        public static PopupDungeonSelect instance;

        public Hiker.UI.HikerScrollRect scrollRect;
        public Transform NodeMapContainer;
        public MapExplorer DungeonExplorer;
        public NodeMapVisual MapVisual;
        public Text lbFoundTreasure;
        public GameObject finalBossTutorial;
        public GameObject selectNodeTutorial;

        NodeMap m_NodeMap;

        bool mInitedItems = false;

        public static PopupDungeonSelect Create(int curChapterIdx, int dungeonId, List<int> path)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupDungeonSelect");
            instance = go.GetComponent<PopupDungeonSelect>();
            instance.Init(curChapterIdx, dungeonId, path);
            ScreenBattle.PauseGame(true);
            return instance;
        }

        private void Init(int curChapIdx, int dungeonId, List<int> path)
        {
            if (mInitedItems) return;
            if (GameClient.instance == null ||
                GameClient.instance.UInfo == null)
            {
                return;
            }

            m_NodeMap = Instantiate(Resources.Load<NodeMap>("MapDungeon/Chapter" + (curChapIdx + 1)), NodeMapContainer);// GetComponentInChildren<NodeMap>();

            MapVisual.map = m_NodeMap;

            var listRevealed = new List<int>(path);
            Hiker.Networks.Data.Shootero.ChapterData chap = null;
            if (GameClient.instance && GameClient.instance.OfflineMode == false)
            {
                chap = GameClient.instance.UInfo.ListChapters[curChapIdx];
                foreach (var n in chap.Revealed)
                {
                    listRevealed.Add(n);
                }
            }
            SetFoundTreasure(chap, path);
            bool isShowFinalBoss = curChapIdx == 0;
            selectNodeTutorial.SetActive(false);
            finalBossTutorial.SetActive(false);

            //feotest
            //PlayerPrefs.SetInt("Chapter1DungeonTutorial" + GameClient.instance.UInfo.GID, 0);
            if (isShowFinalBoss && PlayerPrefs.GetInt("Chapter1DungeonTutorial" + GameClient.instance.UInfo.GID, 0) == 0)
            {
                selectNodeTutorial.SetActive(true);
                finalBossTutorial.SetActive(true);
                PlayerPrefs.SetInt("Chapter1DungeonTutorial" + GameClient.instance.UInfo.GID, 1);
            }
            MapVisual.Init(listRevealed, dungeonId, path, isShowFinalBoss);
            DungeonExplorer.map = m_NodeMap;

            var nodeLevel = MapVisual.ListPoints[dungeonId];

            DungeonExplorer.WrapToNode(nodeLevel.Point);

            foreach (var t in nodeLevel.Point.connections)
            {
                var endPoint = t.endPoint;
                if (DungeonExplorer.CanGoNode(endPoint))
                {
                    for(int i = 0; i < MapVisual.NodeContainer.childCount; ++i)
                    {
                        var child = MapVisual.NodeContainer.GetChild(i);
                        var node = child.GetComponent<NodeLevel>();
                        if (node != null && node.Point == endPoint)
                        {
                            node.Highlight(true);
                        }
                    }
                }
            }

            Hiker.HikerUtils.DoAction(this,
                () => {
                    scrollRect.CenterOnRectTransform(DungeonExplorer.transform as RectTransform);
                },
                0.06f, true);

            mInitedItems = true;
        }

        void SetFoundTreasure(Hiker.Networks.Data.Shootero.ChapterData chap,
            List<int> path)
        {
            int found = 0;
            int totalTreasure = 0;

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
                    else if (path.Contains(i))
                    {
                        found++;
                    }
                }
            }
            lbFoundTreasure.text = string.Format(Localization.Get("FoundTreasureLabel"), found, totalTreasure);
        }
        //public void OnSelectChapter(int chapterIdx)
        //{
        //    ScreenMain.instance.grpChapter.OnSelectChapter(chapterIdx);
        //    OnCloseBtnClick();
        //}

        public NodeMap GetNodeMap()
        {
            return m_NodeMap;
        }
    }
}