using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool FixedJoystick { get; set; }
    public bool RightActiveSkill { get; set; }
    public bool ShowDamageHUD { get; set; }

    public static GameManager instance = null;
    private void Awake()
    {
        if (instance) return;
        instance = this;
        LoadSetting();
    }

    public void LoadSetting()
    {
        var isFixed = PlayerPrefs.GetInt("FixedJoyStick", 1);
        FixedJoystick = (isFixed == 1);

        var isRightButton = PlayerPrefs.GetInt("RightSkillButton", 0);
        RightActiveSkill = (isRightButton == 1);

        var isShowDmgHUD = PlayerPrefs.GetInt("ShowDmgHUD", 1);
        ShowDamageHUD = (isShowDmgHUD == 1);
    }

    public void SaveSetting()
    {
        PlayerPrefs.SetInt("FixedJoyStick", FixedJoystick ? 1 : 0);
        PlayerPrefs.SetInt("RightSkillButton", RightActiveSkill ? 1 : 0);
        PlayerPrefs.SetInt("ShowDmgHUD", ShowDamageHUD ? 1 : 0);
    }
}
