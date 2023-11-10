using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hiker.GUI;
using Hiker;
public class HPBar : MonoBehaviour
{
    public Slider slider;
    public Image imgSlider;
    public Text txtHP;
    public TMPro.TextMeshProUGUI txtHPPro;

    DonViChienDau unit;
    UIFollowTarget uiFollow;

    // PhuongTD : support multi-unit hp bar ( boss )
    List<DonViChienDau> unitList = null;
    bool autoUpdateHP = false;

    public bool IsAutoUpdateHPBar()
    {
        return autoUpdateHP;
    }

    public UIFollowTarget getUIFollow()
    {
        if (uiFollow == null)
        {
            uiFollow = GetComponent<UIFollowTarget>();
        }
        return uiFollow;
    }

    public bool RemoveUnit(DonViChienDau u)
    {
        if(unitList!=null)
        {
            unitList.Remove(u);

            if (unitList.Count == 0)
                return true;
        }
        return false;
    }

    public void ResetBossHPBar()
    {
        previousMaxHP = 0f;
    }


    int countToDisableHP;
    private void Update()
    {
        if (unit == null)
        {
            countToDisableHP++;
            if (countToDisableHP > 3)
            {
                countToDisableHP = 0;
                gameObject.SetActive(false); // 3 frame after unit is null -> auto disable to clean ui
            }
        }
        else
        {
            countToDisableHP = 0;
        }

        if (autoUpdateHP)
        {
            InternalUpdateHP();
        }
    }
    public void Init(DonViChienDau unit, bool isTotalBar = false)
    {
        if (isTotalBar)
        {
            if (unitList == null)
                unitList = new List<DonViChienDau>();

            this.unit = unit;
            unitList.Add(unit);
        }
        else
        {
            this.unit = unit;
            unitList = null;
        }

        autoUpdateHP = isTotalBar;

        UpdateColorByTeam(unit);

        //var bar_y = unit.GetBodyTall();
        //var bar_y = 0;
        //var graphic = gameObject.GetComponent<Graphic>();
        //var c = graphic.color;
        //graphic.color = new Color(c.r, c.g, c.b, 1f);
        getUIFollow()?.SetTarget(unit.transform, 0.55f, 1f, unit.OffsetHUD);
        //if (unit != null)
        //{
        //    gameObject.SetActive(true);
        //}
        //else if (unit == null)
        //{
        //    gameObject.SetActive(false);
        //}
    }    

    public void UpdateColorByTeam(DonViChienDau unit)
    {
        this.imgSlider.color = unit.TeamID == QuanlyManchoi.EnemyTeam ? new Color32(255, 128, 0, 255) : new Color32(0, 255, 28, 255);
    }

    public void UpdateHP(float hp_rate)
    {
        StopAllCoroutines();
        //this.gameObject.SetActive(hp_rate > 0f && hp_rate < 1f);
        this.slider.value = hp_rate;
        //var graphic = gameObject.GetComponent<Graphic>();
        //var c = graphic.color;
        //graphic.color = new Color(c.r, c.g, c.b, 1f);
        //var fadeTween = TweenAlpha.Begin(this.gameObject, 1f, 0, 3);
        //if (gameObject.activeInHierarchy)
        //{
        //    HikerUtils.DoAction(this, () => gameObject.SetActive(false), 4);
        //}
    }

    private float previousMaxHP = 0;

    private void InternalUpdateHP()
    {
        if (unitList != null)
        {
            if (unitList.Count > 0)
            {
                float curHP = 0f;
                float maxHP = 0f;
                for (int i = 0; i < unitList.Count; i++)
                {
                    curHP += unitList[i].GetCurHP();
                    maxHP += unitList[i].GetMaxHP();
                }

                // PhuongTD : dont reduce maxHP when coop boss die
                if (maxHP > previousMaxHP)
                    previousMaxHP = maxHP;
                else
                    maxHP = previousMaxHP;

                UpdateHP(Mathf.Clamp01(curHP / maxHP));
                var hpStr = Mathf.Max((int)Mathf.Round(curHP), 0).ToString();
                if (txtHP != null)
                {
                    txtHP.text = hpStr;
                }
                if (txtHPPro != null)
                {
                    txtHPPro.text = hpStr;
                }
            }
        }
        else if (unit)
        {
            var curHP = unit.GetCurHP();
            //slider.value = Mathf.Clamp01(curHP / unit.GetMaxHP());
            if (unit.GetMaxHP() == 0)
            {
                return;
            }
            UpdateHP(Mathf.Clamp01((float)curHP / unit.GetMaxHP()));
            var hpStr = Mathf.Max((int)Mathf.Round(curHP), 0).ToString();
            if (txtHP != null)
            {
                txtHP.text = hpStr;
            }
            if (txtHPPro != null)
            {
                txtHPPro.text = hpStr;
            }
        }
    }


    public void UpdateHP()
    {
        if (autoUpdateHP == false)
            InternalUpdateHP();
    }
}
