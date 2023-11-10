using Hiker.GUI;
using Hiker.Networks.Data.Shootero;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureChestItem : MonoBehaviour
{
    public Image icon;
    public string chestName;
    string unlockKey, unlockRes;
    int unlockResAmount;
    public Button btnOpenChestKey, btnOpenChestRes;
    public Text btnOpenChestKeyText, btnOpenChestResText, FreeAfterText;
    public GameObject keyIconObj;
    ChestData chestData;
    ChestConfig chestCfg;
    public void InitChest(string _chestName = "")
    {
        if(!string.IsNullOrEmpty(_chestName)) chestName = _chestName;
        //var chest3D = Instantiate(Resources.Load<GameObject>("Prefabs/Chests/" + chestName), Chest3DContainer);
        //animatorChest = chest3D.GetComponent<Animator>();
        chestData = GameClient.instance.UInfo.ListChests.Find(e => e.Name == chestName);
        chestCfg = ConfigManager.ChestCfg[chestName];
        bool haveFreeKey = false;
        TimeSpan ts = GameClient.instance.ServerTime - chestData.LastOpened;
        TimeSpan getFreeTime = new TimeSpan(chestCfg.freeHour, 0, 0);
        if (ts >= getFreeTime) haveFreeKey = true;
        else
        if (chestName == "Chest_Legend")
        {
            if (PushNotificationManager.Instance)
                PushNotificationManager.Instance.SetPushNotificationByTime(
                    NotificationCode.ChestLegend,
                    DateTime.Now + (getFreeTime - ts));
        }

        FreeAfterText.gameObject.SetActive(!haveFreeKey);
        if (haveFreeKey)
        {
            keyIconObj.SetActive(false);
            btnOpenChestKey.gameObject.SetActive(true);
            btnOpenChestRes.gameObject.SetActive(false);
            btnOpenChestKeyText.text = Localization.Get("Free");
            btnOpenChestKey.interactable = haveFreeKey;
            //LayoutRebuilder.ForceRebuildLayoutImmediate(btnOpenChestKeyText.rectTransform);
            Hiker.HikerUtils.DoAction(this,
                () => {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(btnOpenChestKeyText.transform.parent as RectTransform);
                }, 0.1f, true);
        }
        else
        {
            unlockKey = "";
            unlockRes = "";
            foreach (string itemName in chestCfg.unlockKey.Keys)
            {
                if (itemName.Contains("M_")) unlockKey = itemName;
                else
                {
                    unlockRes = itemName;
                    unlockResAmount = chestCfg.unlockKey[itemName];
                }
            }
            MaterialData matData = GameClient.instance.UInfo.ListMaterials.Find(e => e.Name == unlockKey);
            bool isUseKey = string.IsNullOrEmpty(unlockRes) || (matData != null && matData.Num > 0);
            btnOpenChestKey.gameObject.SetActive(isUseKey);
            btnOpenChestRes.gameObject.SetActive(!isUseKey);
            if (isUseKey)
            {
                keyIconObj.SetActive(true);
                int keyNum = (matData != null) ? matData.Num : 0;
                btnOpenChestKeyText.text = string.Format("{0}/1", keyNum);
                btnOpenChestKey.interactable = keyNum > 0;
                //LayoutRebuilder.ForceRebuildLayoutImmediate(btnOpenChestKeyText.rectTransform);
                Hiker.HikerUtils.DoAction(this,
                () => {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(btnOpenChestKeyText.transform.parent as RectTransform);
                }, 0.1f, true);
            }
            else
            {
                btnOpenChestResText.text = unlockResAmount + "";
                //LayoutRebuilder.ForceRebuildLayoutImmediate(btnOpenChestResText.rectTransform);
                //LayoutRebuilder.ForceRebuildLayoutImmediate(btnOpenChestResText.transform.parent as RectTransform);
                Hiker.HikerUtils.DoAction(this,
                () => {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(btnOpenChestResText.rectTransform);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(btnOpenChestResText.transform.parent as RectTransform);
                }, 0.1f, true);
                bool isEnougRes = false;
                if (unlockRes == CardReward.GOLD_CARD)
                {
                    isEnougRes = GameClient.instance.UInfo.Gamer.Gold >= unlockResAmount;
                }
                else if (unlockRes == CardReward.GEM_CARD)
                {
                    isEnougRes = GameClient.instance.UInfo.Gamer.Gem >= unlockResAmount;
                }
                btnOpenChestRes.interactable = isEnougRes;
            }
        }
        
    }

    [GUIDelegate]
    public void OnOpenChestKeyBtnClick()
    {
        GameClient.instance.RequestOpenChest(chestName);
    }
    [GUIDelegate]
    public void OnOpenChestResBtnClick()
    {
        GameClient.instance.RequestOpenChest(chestName);
    }

    private void Update()
    {
        if(chestData != null && FreeAfterText.gameObject.activeSelf)
        {
            TimeSpan ts = GameClient.instance.ServerTime - chestData.LastOpened;
            TimeSpan getFreeTime = new TimeSpan(chestCfg.freeHour, 0, 0);
            ts = getFreeTime - ts;
            int h = Mathf.FloorToInt((float)ts.TotalHours);
            int m = Mathf.CeilToInt((float)(ts.TotalMinutes - h * 60));
            FreeAfterText.text = string.Format(Localization.Get("free_after"), h, m);
        }
    }
    [GUIDelegate]
    public void OnBtnInfoClick()
    {
        Hiker.GUI.Shootero.PopupChestInfo.Create(chestName);
    }
}
