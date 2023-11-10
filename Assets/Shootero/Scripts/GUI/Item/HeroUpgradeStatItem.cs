using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Hiker.Networks.Data.Shootero;
using Hiker.GUI.Shootero;
using Hiker.GUI;

public class HeroUpgradeStatItem : MonoBehaviour
{
    public GameObject disableBlurObj, allHeroesObj, lockObj;
    public Text lbStatDesc, lbUnlockLevel;

    public void SetBasicItem(StatMod stat, int crLevel, bool isAllHero = true)
    {
        disableBlurObj.SetActive(false);
        allHeroesObj.SetActive(isAllHero);
        lockObj.SetActive(false);
        lbUnlockLevel.gameObject.SetActive(false);

        string desc = ConfigManager.GetStatDesc(stat, crLevel);
        int crNum = (int)stat.GetTotalIncByLevel(crLevel);
        int nextNum = (int)stat.GetTotalIncByLevel(crLevel + 1);
        int diff = nextNum - crNum;
        if(diff != 0) desc += string.Format(" (<color=lime>+{0}</color>)", diff);
        lbStatDesc.text = desc;
    }

    public void SetAdvancedItem(StatMod stat, int crLevel, int unlockLevel, bool isAllHero = true)
    {
        bool isUnlock = crLevel >= unlockLevel;
        disableBlurObj.SetActive(!isUnlock);
        allHeroesObj.SetActive(isAllHero);
        lockObj.SetActive(!isUnlock);
        lbUnlockLevel.gameObject.SetActive(true);

        lbUnlockLevel.text = string.Format(Localization.Get("ShortLevelLabel"), unlockLevel);
        lbStatDesc.text = ConfigManager.GetStatDesc(stat);
    }
}
