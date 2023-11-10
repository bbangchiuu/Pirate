using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hiker.GUI.Shootero;
using UnityEngine.UI;

public class TetNienThuBattlePoint : MonoBehaviour
{
    public GameObject grpBattleGold;
    public GameObject grpBattlePoint;
    public Text lbBattlePoint;
    public Text lbTimeCountDown;

    ScreenBattle screenBattle;
    private void Awake()
    {
        screenBattle = GetComponent<ScreenBattle>();
    }

    private void OnEnable()
    {
        if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.IsNienThuMode)
        {
            grpBattleGold.gameObject.SetActive(false);
            grpBattlePoint.gameObject.SetActive(true);
            lbTimeCountDown.gameObject.SetActive(true);
        }
        else
        {
            grpBattleGold.gameObject.SetActive(true);
            grpBattlePoint.gameObject.SetActive(false);
            lbTimeCountDown.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.IsNienThuMode)
        {
            lbBattlePoint.text = QuanlyNguoichoi.Instance.NienThuBattlePointSumarize.ToString();

            if (QuanlyNguoichoi.Instance.MissionID > 0)
            {
                if (lbTimeCountDown.gameObject.activeSelf == false)
                {
                    lbTimeCountDown.gameObject.SetActive(true);
                }
                var seconds = ConfigManager.TetEventCfg.ThoiGianDanhBoss -
                    Mathf.FloorToInt(QuanlyNguoichoi.Instance.ThoiGianNienThu);
                lbTimeCountDown.text = string.Format(Localization.Get("ThoiGianBattleNienThuCountDown"),
                    (int)Mathf.Round(seconds));
            }
            else
            {
                if (lbTimeCountDown.gameObject.activeSelf)
                {
                    lbTimeCountDown.gameObject.SetActive(false);
                }
            }
        }
    }
}
