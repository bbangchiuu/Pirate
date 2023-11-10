using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hiker.GUI.Shootero;
public class NodeLevel : MonoBehaviour
{
    public bool IsRevealed = false;
    public GameObject[] LevelTypeObjects;
    public GameObject UnRevealMask;
    public NodeLevelConfig levelConfig;
    public int DungeonId;

    public NodePoint Point;

    private void Awake()
    {
    }

    public void SetLevelConfig(NodeLevelConfig _levelConfig)
    {
        levelConfig = _levelConfig;
        SetType(levelConfig.LevelType.ToString());
    }
    public void SetType(string type)
    {
        UnRevealMask.gameObject.SetActive(IsRevealed == false);
        foreach (var obj in LevelTypeObjects)
        {
            if (obj.name == type)
            {
                obj.gameObject.SetActive(IsRevealed);
                if (type == ENodeType.Ruong.ToString())
                {
                    var chapData = GameClient.instance.UInfo.ListChapters.Find(e => e.ChapIdx == QuanlyNguoichoi.Instance.ChapterIndex);
                    if (chapData != null)
                    {
                        bool isOpenned = chapData.RuongOpened.Contains(levelConfig.RuongConfig);
                        if (isOpenned == false)
                        {
                            isOpenned = QuanlyNguoichoi.Instance.IsMoRuong(levelConfig.RuongConfig);
                        }
                        obj.transform.GetChild(0).gameObject.SetActive(!isOpenned);
                        obj.transform.GetChild(1).gameObject.SetActive(isOpenned);
                    }
                }
            }
            else
            {
                obj.gameObject.SetActive(false);
            }
        }

        Highlight(false);
    }

    public void Highlight(bool isOn)
    {
        var tweenScale = gameObject.GetComponent<TweenScale>();
        if (tweenScale)
        {
            tweenScale.ResetToBeginning();
            tweenScale.enabled = isOn;
        }
    }

    public void Reaveal()
    {
        IsRevealed = true;

        SetType(levelConfig.LevelType.ToString());
    }

    [Hiker.GUI.GUIDelegate]
    public void OnBtnClick()
    {
        // tranh viec co tinh touch vao 2 node cung` luc
        if (QuanlyNguoichoi.Instance.IsLoadingMission) return;

#if UNITY_EDITOR
        Debug.Log("OnSelect DungeonIdx");
#endif
        var explorer = PopupDungeonSelect.instance.DungeonExplorer;
        if (explorer.GoToNode(Point))
        {
            PopupDungeonSelect.instance.OnCloseBtnClick();
            QuanlyNguoichoi.Instance.GoToDungeon(DungeonId);
            AnalyticsManager.LogEvent(string.Format("GO_CHAP{0}_D{1}", QuanlyNguoichoi.Instance.ChapterIndex, QuanlyNguoichoi.Instance.GetCurPathLength()));
        }
    }
}
