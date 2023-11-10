using Hiker.Networks.Data.Shootero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    public class PopupVongQuay : PopupBase
    {
        public Button btnRotate, btnWatchAds;
        public Transform[] SlotItems;

        public Transform vongQuayTrans;
        public float rotateSpeed = 720f;
        public float giatocGoc = 1440f;
        public float timeToStop = 3f;

        public Text btnQuayText, btnQuayResText;
        public Text[] goldTexts;

        bool rotating = false;
        float curSpeed = 0f;

        bool isRotated = false;

        const float MIN_TIME_TO_STOP = 1f;
        float timeRotate = 0f;
        float checkAngle = 30f;
        VongQuayResponse response;

        bool preventDoubleClick;

        public static PopupVongQuay instance;

        public static PopupVongQuay Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupVongQuay");
            instance = go.GetComponent<PopupVongQuay>();
            instance.Init();
            return instance;
        }

        private void Init()
        {
            isRotated = false;
            curSpeed = 0f;
            rotating = false;
            timeRotate = 0f;
            btnRotate.interactable = true;
            btnWatchAds.interactable = true;
            checkAngle = 360f / SlotItems.Length;
            UpdateBtnTheLuc();
            btnQuayResText.text = ConfigManager.GetTheLucVongQuay() + "";
            UpdateGoldTexts();
        }
        private void UpdateBtnTheLuc()
        {
            var uInfo = GameClient.instance.UInfo;
            int theLuc = ConfigManager.GetTheLucVongQuay();
            var theLucRegenTime = ConfigManager.GetGamerTheLucRegenSeconds();
            if (uInfo.Gamer.TheLuc.Val < theLuc)
            {
                uInfo.Gamer.TheLuc.UpdateByTime(GameClient.instance.ServerTime, theLucRegenTime);
            }

            btnQuayText.text = Localization.Get("BtnSpin"); // + string.Format("({0}/{1})", theLuc, uInfo.Gamer.TheLuc.Val);
            
            btnRotate.interactable = true;
            btnWatchAds.interactable = true;

            if (uInfo.adsData.listAdsVongQuayIDs.Count >= ConfigManager.GetMaxAdsVongQuayPerDay())
            {
                btnWatchAds.gameObject.SetActive(false);
            }
            
            //if (uInfo.Gamer.TheLuc.Val >= theLuc)
            //{
            //    btnRotate.interactable = true;
            //}
            //else
            //{
            //    btnRotate.interactable = false;
            //}
        }

        public void UpdateGoldTexts()
        {
            var maxChapIdx = GameClient.instance.UInfo.GetCurrentChapter();
            if (maxChapIdx >= ConfigManager.VongQuay.Chapters.Length) maxChapIdx = ConfigManager.VongQuay.Chapters.Length - 1;
            var vongQuayCfg = ConfigManager.VongQuay.Chapters[maxChapIdx];
            for(int i = 0; i < goldTexts.Length; i++)
            {
                int gold = (int)(vongQuayCfg.BaseGold[i] * ConfigManager.chapterConfigs[maxChapIdx].BaseGold / 100f);
                goldTexts[i].text = gold + "";
                goldTexts[i].gameObject.SetActive(true);
            }
        }

        public void GetResponse(VongQuayResponse res)
        {
            //rotating = false;
            response = res;
#if UNITY_EDITOR
            Debug.Log("Slot = " + res.Slot);
#endif
        }

        bool isHam = false;

        private void Update()
        {
            if (rotating)
            {
                if (curSpeed < rotateSpeed)
                {
                    curSpeed += giatocGoc * Time.deltaTime;
                }
                if (curSpeed > rotateSpeed) curSpeed = rotateSpeed;

                vongQuayTrans.Rotate(Vector3.forward, curSpeed * Time.deltaTime, Space.Self);

                timeRotate += Time.deltaTime;
                if (response != null)
                {
                    if (timeRotate >= MIN_TIME_TO_STOP)
                    {
                        rotating = false;
                    }
                }
            }
            else
            {
                if (curSpeed > 0)
                {
                    float lastSpeed = curSpeed;
                    curSpeed -= rotateSpeed / timeToStop * Time.deltaTime;

                    if (curSpeed <= 0)
                    {
                        curSpeed = 0;
                    }

                    float remainTime = curSpeed / rotateSpeed * timeToStop;
                    float angle = lastSpeed * remainTime - 0.5f * rotateSpeed / timeToStop * remainTime * remainTime;
                    if (curSpeed < 180f && angle < checkAngle * 0.5f && isHam == false)
                    {
                        float difAngle = Vector3.SignedAngle(SlotItems[response.Slot].up, Vector3.up, Vector3.forward);
                        if (difAngle < 0f)
                        {
                            difAngle += 360f;
                        }
                        //Debug.Log("difAngle " + difAngle);
                        if (difAngle > 30)
                        {
                            curSpeed = lastSpeed;
                        }
                        else
                        {
                            //Debug.Log("Ham");
                            isHam = true;
                        }
                    }

                    //if (isHam)
                    //{
                    //    float difAngle = Vector3.SignedAngle(SlotItems[response.Slot].up, Vector3.up, Vector3.forward);
                    //    Debug.Log("difAngle " + difAngle);
                    //}

                    vongQuayTrans.Rotate(Vector3.forward, curSpeed * Time.deltaTime, Space.Self);
                }
                else
                {
                    if (response != null && response.reward != null)
                    {
                        var reward = response.reward;
                        GameClient.instance.UpdateUserInfo(response.UInfo);
                        HikerUtils.DoAction(this, () =>
                        {
                            Dismiss();
                            PopupOpenChest_new.Create(reward);
                            UpdateBtnTheLuc();
                        }, 0.5f, true);
                        response = null;
                    }
                }
            }
        }

        [GUIDelegate]
        public void OnBtnRotateClick()
        {
            if (isRotated == false)
            {
                int theluc = ConfigManager.GetTheLucVongQuay();
                if (theluc <= GameClient.instance.UInfo.Gamer.TheLuc.Val)
                {
                    StartSpin();
                }
                else
                {
                    PopupMissingTheLuc.Create();
                }
            }
        }
        [GUIDelegate]
        public void OnBtnWatchAdsClick()
        {
            if (preventDoubleClick) return;
            AnalyticsManager.LogEvent("WATCH_ADS_VONGQUAY");

            if (HikerAdsManager.instance && HikerAdsManager.instance.IsCanShowVideo())
            {
                HikerAdsManager.instance.ShowVideoAds(this.RequestReceiveReward, this.RequestCancelVideo,
                                                      this.RequestFailVideo, this.RequestCancelVideo);
                StartCoroutine(PreventDoubleclick(5));

            }
            else
            {
                AnalyticsManager.LogEvent("NO_ADS_AVAILABLE");
                PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("no_ads_available"));
            }
        }

        private void RequestReceiveReward()
        {
            AnalyticsManager.LogEvent("FINISH_ADS_VONGQUAY");
            StartSpin(true);
            //Dismiss();
        }

        private void RequestCancelVideo()
        {
            PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("watch_ads_cancel"));
            //Dismiss();
        }

        private void RequestFailVideo()
        {
            PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("watch_ads_fail"));
            //Dismiss();
        }

        private IEnumerator PreventDoubleclick(float time)
        {
            preventDoubleClick = true;
            yield return new WaitForSecondsRealtime(time);
            preventDoubleClick = false;
        }

        void StartSpin(bool isAds = false)
        {
            rotating = true;
            if (isAds)
            {
                GameClient.instance.RequestStartVongQuayAds();
            }
            else
            {
                GameClient.instance.RequestStartVongQuay();
            }
            isRotated = true;
            if (btnRotate)
                btnRotate.interactable = false;
            if (btnQuayText)
                btnQuayText.text = Localization.Get("BtnSpining");
            if (btnWatchAds)
                btnWatchAds.interactable = false;
            AnalyticsManager.LogEvent("START_VONG_QUAY");
        }

        [GUIDelegate]
        public override void OnBackBtnClick()
        {
            if (instance.isRotated) return;

            base.OnBackBtnClick();
        }

        public static void Dismiss()
        {
            if (instance != null)
            {
                instance.OnCloseBtnClick();
            }
        }
    }
}
