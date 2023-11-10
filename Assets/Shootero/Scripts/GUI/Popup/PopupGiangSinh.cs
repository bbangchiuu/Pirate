using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hiker.Networks.Data.Shootero;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;

    public class PopupGiangSinh : PopupBase
    {
        public GiangSinhMergeController controller;

        //public Transform itemParent;
        //public DailyShopItem itemPrefabs;

        public Text lbGift1;
        public Text lbGift2;
        public Text lbGift3;

        public static PopupGiangSinh instance;
        public Text lbCostDraw;
        public Text lbResource;
        public Text lbTime;

        int NumResouce = 0;
        //List<DailyShopItem> mListItems = new List<DailyShopItem>();
        bool activeResourceBar = true;
        public static PopupGiangSinh Create(bool needUpdate)
        {
            UserInfo uInfo = GameClient.instance.UInfo;
            if (uInfo.giangSinhData == null || uInfo.giangSinhSrvData == null ||
                uInfo.giangSinhSrvData.EndTime <= GameClient.instance.ServerTime
                )
            {
                GameClient.instance.RequestGetGiangSinhData(true);
                return null;
            }

            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupGiangSinh");
            instance = go.GetComponent<PopupGiangSinh>();
            instance.Init(needUpdate);

            if (ResourceBar.instance)
            {
                instance.activeResourceBar = ResourceBar.instance.gameObject.activeSelf;
                //ResourceBar.instance.gameObject.SetActive(false);
            }
            return instance;
        }

        //public static string GetMotaGift(int num, int max)
        //{
        //    return string.Format("{0}/{1}", num, max > 1000 ? Localization.Get("Unlimited") : max.ToString());
        //}

        protected override void OnCleanUp()
        {
            //if (ResourceBar.instance)
            //{
            //    if (GUIManager.Instance.CurrentScreen == "Main")
            //    {
            //        if (PopupManager.instance.HaveBackStackPopup() == false)
            //            activeResourceBar = true;
            //    }
            //    ResourceBar.instance.gameObject.SetActive(activeResourceBar);
            //}
            base.OnCleanUp();
        }

        int GetResourceNum()
        {
            UserInfo uInfo = GameClient.instance.UInfo;
            var resMat = uInfo.ListMaterials.Find(e => e.Name == ConfigManager.GiangSinhCfg.Material);

            return resMat != null ? resMat.Num : 0;
        }

        public void Init(bool needUpdate)
        {
            UserInfo uInfo = GameClient.instance.UInfo;
            if (needUpdate)
            {
                GameClient.instance.RequestGetGiangSinhData(false);
            }
            var resMat = uInfo.ListMaterials.Find(e => e.Name == ConfigManager.GiangSinhCfg.Material);

            NumResouce = GetResourceNum();
            UpdateResource(NumResouce);
            UpdateCost(ConfigManager.GiangSinhCfg.CostDraw);

            if (uInfo == null || uInfo.giangSinhData == null) return;

            UpdateGifts(uInfo.giangSinhData);

            controller.Init(uInfo.giangSinhData);
        }

        public void UpdateGifts(GiangSinhGamerData data)
        {
            //lbGift1.text = GetMotaGift(data.GiftBox1, data.MaxGift1);
            //lbGift2.text = GetMotaGift(data.GiftBox2, data.MaxGift2);
            //lbGift3.text = GetMotaGift(data.GiftBox3, data.MaxGift3);
        }

        public void UpdateResource(int res)
        {
            lbResource.text = res.ToString();
            var parrentLayout = lbResource.gameObject.GetComponentInParent<LayoutGroup>().GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(parrentLayout);
            LayoutRebuilder.MarkLayoutForRebuild(parrentLayout);
        }

        public void UpdateCost(int res)
        {
            lbCostDraw.text = res.ToString();
            var parrentLayout = lbCostDraw.gameObject.GetComponentInParent<LayoutGroup>().GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(parrentLayout);
            LayoutRebuilder.MarkLayoutForRebuild(parrentLayout);
        }

        [GUIDelegate]
        public void OnBtnWish()
        {
            UserInfo uInfo = GameClient.instance.UInfo;
            if (uInfo == null || uInfo.giangSinhData == null || controller.IsFull() || uInfo.giangSinhData.IsFull())
            {
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("NoEmptyCell"));
                return;
            }

            int cost = ConfigManager.GiangSinhCfg.CostDraw;

            if (NumResouce >= cost)
            {
                var tables = uInfo.giangSinhData.Table;
                controller.AddRandomItem();
                NumResouce -= cost;
                UpdateResource(NumResouce);
            }
            else
            {
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("NotEnoughBongTuyet"));
            }
        }

        void UpdateTimeLeft()
        {
            if (GameClient.instance.UInfo != null && GameClient.instance.UInfo.giangSinhSrvData != null
                && GameClient.instance.ServerTime < GameClient.instance.UInfo.giangSinhSrvData.EndTime)
            {
                var tsDropTime = GameClient.instance.UInfo.giangSinhSrvData.EndTime - GameClient.instance.ServerTime;
                string eventTimeStr = Localization.Get("event_end_after") + " " + string.Format(Localization.Get("TimeCountDownHMS"),
                Mathf.CeilToInt((float)tsDropTime.TotalHours), tsDropTime.Minutes, tsDropTime.Seconds);
                lbTime.text = eventTimeStr;
            }
            else
            {
                OnCloseBtnClick();
            }
        }

        private void Update()
        {
            UpdateTimeLeft();
        }
    }

}

