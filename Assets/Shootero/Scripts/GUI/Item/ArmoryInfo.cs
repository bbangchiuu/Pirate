using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;

    public class ArmoryInfo : MonoBehaviour
    {
        public Text armoryName;
        public Text armoryDescStat;
        public GameObject grpMax;
        public GameObject grpRes;
        public Text textMaxUpgrade;
        public Text goldUpgrade;
        public Button btnUpgrade;
        public Text btnUpgradeLabel;

        ArmoryData mSelectedArmData;
        int mArmorySelectedLevel = 0;
        string mArmorySelected;
        public bool IsMaxLevel { get; private set; }
        public string ArmName { get { return mArmorySelected; } }

        void UpdateDescStat()
        {
            if (string.IsNullOrEmpty(mArmorySelected) == false)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                if (mArmorySelectedLevel > 0)
                {
                    //sb.AppendLine();
                    var basicStats = ConfigManager.GetArmoryStat(mArmorySelected, mArmorySelectedLevel);

                    //for (int i = 0; i < basicStats.Length; ++i)
                    int i = 0;
                    {
                        sb.AppendLine(ConfigManager.GetStatDesc(basicStats[i], mArmorySelectedLevel));
                    }

                    if (IsMaxLevel == false)
                    {
                        var nextStats = ConfigManager.GetArmoryStat(mArmorySelected, mArmorySelectedLevel + 1);
                        sb.AppendLine(string.Format(Localization.Get("NextLevelDesc"), ConfigManager.GetStatDesc(nextStats[i], mArmorySelectedLevel + 1)));
                    }
                }
                else
                {
                    var basicStats = ConfigManager.GetArmoryStat(mArmorySelected, 1);
                    int i = 0;
                    sb.AppendLine(string.Format("{0}", ConfigManager.GetStatDesc(basicStats[i], 1)));
                }

                armoryDescStat.text = sb.ToString();
            }
            else
            {
                armoryDescStat.text = string.Empty;
            }
        }

        public void SetInfo(string armName, ArmoryData data)
        {
            mArmorySelected = armName;
            mArmorySelectedLevel = 0;
            mSelectedArmData = data;
            if (mSelectedArmData != null)
            {
                mArmorySelectedLevel = mSelectedArmData.Level;
            }

            if (mArmorySelectedLevel > 0)
            {
                string lb = string.Format(Localization.Get("ShortLevelLabel"), mArmorySelectedLevel);
                armoryName.text = string.Format("{0} {1}", Localization.Get(armName + "_Name"), lb);

                btnUpgradeLabel.text = Localization.Get("Btn_Upgrade");
            }
            else
            {
                armoryName.text = Localization.Get(armName + "_Name");

                btnUpgradeLabel.text = Localization.Get("Btn_Unlock");
            }

            bool isMaxUpgrade = false;
            var requirements = ConfigManager.GetItemUpgradeRequirement(mArmorySelected, mArmorySelectedLevel, out isMaxUpgrade);
            long gold = 0;
            grpRes.SetActive(!isMaxUpgrade);
            grpMax.SetActive(isMaxUpgrade);

            IsMaxLevel = isMaxUpgrade;

            if (isMaxUpgrade)
            {
                btnUpgrade.interactable = false;
                textMaxUpgrade.text = Localization.Get("MaxLevel");
            }
            else if (requirements != null)
            {
                for (int i = 0; i < requirements.Length; ++i)
                {
                    if (requirements[i].Res == ConfigManager.GoldName)
                        gold += requirements[i].Num;
                }

                btnUpgrade.interactable = (gold <= GameClient.instance.UInfo.Gamer.Gold);
                goldUpgrade.text = gold.ToString();
            }
            else
            {
                btnUpgrade.interactable = false;
            }

            Hiker.HikerUtils.DoAction(this,
                () => LayoutRebuilder.ForceRebuildLayoutImmediate(goldUpgrade.transform.parent as RectTransform),
                0.01f, true);

            UpdateDescStat();
        }
        [GUIDelegate]
        public void OnBtnUpgradeClick()
        {
            if (mSelectedArmData != null)
            {
                GameClient.instance.RequestUpgradeArmory(mSelectedArmData.Name);
                AnalyticsManager.LogEvent("UPGRADE_ARMORY", new AnalyticsParameter("ARMORY_NAME", mSelectedArmData.Name));
            }
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            Hiker.HikerUtils.DoAction(this,
                () => LayoutRebuilder.ForceRebuildLayoutImmediate(goldUpgrade.transform.parent as RectTransform),
                0.01f, true);
            //GetComponent<CanvasGroup>().alpha = 0.5f;
            //TweenAlpha.Begin(gameObject, 0.1f, 1f);
            StartCoroutine(CoTweenEnable());
        }

        IEnumerator CoTweenEnable()
        {
            RectTransform trans = GetComponent<RectTransform>();
            float size = 215;
            float time = 0.1f;
            float delta = (360 - 215) / time;
            trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 215);
            while (size < 360)
            {
                yield return null;
                size += delta * Time.unscaledDeltaTime;
                if (size > 360) size = 360f;
                //int height = Mathf.FloorToInt(size);
                trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
            }
        }
    }
}
