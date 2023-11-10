using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using Hiker.UI;
    using UnityEngine.UI;
    using UnityEngine.UI.Extensions;

    public class GroupArmory : NetworkDataSync
    {
        public HikerScrollRect scrollView;
        //public Text armoryDesc;
        //public Text armoryName;
        //public Text armoryDescStat;
        //public Image spSelected;
        //public CanvasGroup grpInfo;
        //public Text goldUpgrade;
        //public Button btnUpgrade;
        //public GameObject grpRes;
        //public GameObject grpMax;
        //public Text textMaxUpgrade;
        //public Text btnUpgradeLabel;

        public ArmoryAvatar[] avatars;
        public ArmoryInfo[] armInfos;

        bool mInitedItems = false;

        string mArmorySelected;
        int mArmorySelectedLevel = 0;
        bool IsMaxLevel { get; set; }
        ArmoryData mSelectedArmData;

        protected override void OnEnable()
        {
            base.OnEnable();
            InitItems();

            SyncNetworkData();
        }

        public override void SyncNetworkData()
        {
            if (GameClient.instance == null ||
                GameClient.instance.UInfo == null ||
                GameClient.instance.UInfo.ListArmories == null)
            {
                return;
            }

            var uInfo = GameClient.instance.UInfo;

            for (int i = 0; i < avatars.Length; ++i)
            {
                var arm = avatars[i];

                var armData = uInfo.ListArmories.Find(e => e.Name == arm.name);
                if (armData != null)
                {
                    arm.SetItem(armData.Name, ERarity.Common, armData.Level);
                    if (armInfos.Length > i)
                        armInfos[i].SetInfo(arm.name, armData);
                }
                else
                {
                    arm.SetItem(arm.name, ERarity.Common, 0);

                    if (armInfos.Length > i)
                        armInfos[i].SetInfo(arm.name, null);
                }
            }
        }

        private void InitItems()
        {
            if (mInitedItems)
                return;
            if (GameClient.instance == null ||
                GameClient.instance.UInfo == null ||
                GameClient.instance.UInfo.ListArmories == null)
            {
                return;
            }

            SyncNetworkData();

            if (string.IsNullOrEmpty(mArmorySelected))
            {
                TurnOffInfoGrp();
                //spSelected.gameObject.SetActive(false);
                //HikerUtils.DoAction(this, () => OnArmoryClicked(avatars[0]), 0.01f, true);
            }

            mInitedItems = true;
        }

        [GUIDelegate]
        public void TurnOffInfoGrp()
        {
            UnSelectArmory(mArmorySelected);
            //var tweenAlpha = TweenAlpha.Begin(grpInfo.gameObject, 0.1f, 0f);
            //Hiker.HikerUtils.DoAction(this, () =>
            //{
            //    if (string.IsNullOrEmpty(mArmorySelected))
            //    {
            //        grpInfo.interactable = false;
            //        grpInfo.blocksRaycasts = false;
            //    }
            //    else
            //    {
            //        grpInfo.interactable = true;
            //        grpInfo.blocksRaycasts = true;
            //    }
            //}, 0.1f, true);
            
        }

        [GUIDelegate]
        public void OnArmoryClicked(ArmoryAvatar avatared)
        {
            if (mArmorySelected == avatared.name)
            {
                UnSelectArmory(mArmorySelected);
            }
            else
            {
                OnSelectedArmory(avatared.name);
            }
            
            ////if (spSelected.gameObject.activeSelf == false)
            //{
            //    var pos = grpInfo.transform.position;

            //    //if (avatared.name == "StartSkillTalent")
            //    //{
            //    //    pos.y = avatared.transform.position.y + 290;
            //    //    spSelected.gameObject.SetActive(false);
            //    //}
            //    //else
            //    //{
            //    //    pos.y = avatared.transform.position.y - 65;
            //    //    spSelected.gameObject.SetActive(true);
            //    //}

            //    grpInfo.transform.position = pos;

            //    var tween2 = TweenAlpha.Begin(grpInfo.gameObject, 0.15f, 1f);
            //    //spSelected.transform.position = new Vector3(avatared.transform.position.x, pos.y);
            //    //spSelected.gameObject.SetActive(true);
            //}
            ////grpInfo.alpha = 1;
            //grpInfo.interactable = true;
            //grpInfo.blocksRaycasts = true;
            ////var tween = TweenPosition.Begin(spSelected.gameObject, 0.15f, avatared.transform.position);
            ////tween.worldSpace = true;
            ////tween.ignoreTimeScale = true;
            ////HikerUtils.DoAction(this, () => { spSelected.transform.position = avatared.transform.position;  }, 0.2f, true);
        }

        void UnSelectArmory(string armName)
        {
            mArmorySelected = string.Empty;
            //armoryDesc.text = Localization.Get(armName + "_Desc");

            for (int i = 0; i < armInfos.Length; ++i)
            {
                if (armInfos[i].ArmName == armName)
                {
                    armInfos[i].gameObject.SetActive(false);
                    return;
                }
            }
            //centerView = null;
        }

        void OnSelectedArmory(string armName)
        {
            mArmorySelected = armName;
            //armoryDesc.text = Localization.Get(armName + "_Desc");

            for (int i = 0; i < armInfos.Length; ++i)
            {
                if (armInfos[i].ArmName == armName)
                {
                    armInfos[i].gameObject.SetActive(true);
                    //centerView = armInfos[i].armoryDescStat.GetComponent<RectTransform>();
                }
                else
                {
                    armInfos[i].gameObject.SetActive(false);
                }
            }

            //Hiker.HikerUtils.DoAction(this, () =>
            //{
            //    //centerView = null;
            //    //scrollView.vertical = false;

            //    for (int i = 0; i < armInfos.Length; ++i)
            //    {
            //        if (armInfos[i].gameObject.activeInHierarchy &&
            //            armInfos[i].ArmName == armName)
            //        {
            //            var centerView = armInfos[i].armoryDescStat.GetComponent<RectTransform>();
            //            scrollView.GetComponent<HikerScrollRectTweener>().ScrollVertical(
            //                Mathf.Clamp01(scrollView.GetNormalizePosition(centerView, 1)));
            //            return;
            //        }
            //    }
            //}, 0.11f, true);

            //Hiker.HikerUtils.DoAction(this, () =>
            //{
            //    centerView = null;
            //    //for (int i = 0; i < armInfos.Length; ++i)
            //    //{
            //    //    if (armInfos[i].gameObject.activeInHierarchy &&
            //    //        armInfos[i].ArmName == armName)
            //    //    {
            //    //        scrollView.CenterOnRectTransform(armInfos[i].armoryDescStat.GetComponent<RectTransform>());
            //    //    }
            //    //}
            //    scrollView.vertical = true;
            //}, 0.3f, true);
        }

        //RectTransform centerView = null;
        private void Update()
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //    if (string.IsNullOrEmpty(mArmorySelected) == false)
            //    {
            //        TurnOffInfoGrp();
            //    }
            //}

            if (mInitedItems == false)
            {
                InitItems();
                return;
            }

            //if (centerView != null)
            //{

            //    scrollView.CenterOnRectTransform(centerView);
            //}
        }
    }
}