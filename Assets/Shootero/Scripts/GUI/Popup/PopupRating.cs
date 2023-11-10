using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hiker.Networks.Data.Shootero;
namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;

    public class PopupRating : PopupBase
    {
        public static PopupRating Instance;
        public List<GameObject> ListStars;
        public Button btnRate;
        private int Star { get; set; }
        public System.Action OnComplete;

        public static void Create(System.Action _OnComplete = null)
        {
            if (Instance == null)
            {
                var go = PopupManager.instance.GetPopup("PopupRating");
                Instance = go.GetComponent<PopupRating>();
            }
            Instance.OnComplete = _OnComplete;
            Instance.Init();
        }

        private void Init()
        {
            foreach (var star in this.ListStars)
            {
                star.SetActive(false);
            }

            this.Star = 0;
            this.btnRate.interactable = false;
        }

        public static void Dismiss()
        {
            if (Instance != null)
            {
                Instance.OnCloseBtnClick();
                Instance = null;
            }
        }

        protected override void OnCleanUp()
        {
            OnComplete?.Invoke();
            OnComplete = null;
        }

        [GUIDelegate]
        public void OnStarToggleChanged(int _index)
        {
            var current_star_index = _index;
            if (current_star_index + 1 == this.Star) return;
            this.Star = current_star_index + 1;
            this.btnRate.interactable = true;

            for (int i = 0; i < this.ListStars.Count; i++)
            {
                this.ListStars[i].SetActive(i <= current_star_index);
            }
        }

        [GUIDelegate]
        public void OnRateClick()
        {
            if (this.Star == 5)
            {

#if UNITY_ANDROID
            string storeURL = (string)ConfigManager.otherConfig["androidStore"];
            Application.OpenURL(storeURL);
#elif UNITY_IOS
            string storeURL = (string)ConfigManager.otherConfig["iosStore"];
            Application.OpenURL(storeURL);
#else
                string storeURL = (string)ConfigManager.otherConfig["androidStore"];
                Application.OpenURL(storeURL);
                Debug.Log("Open URL " + storeURL);
#endif
                Dismiss();
            }
            else
            {
                Dismiss();
                //PopupRating.Create(true);
            }
        }

        //[Beebyte.Obfuscator.SkipRename]
        //public void OnReviewClick()
        //{
        //    var content = this.InputReview.text.Trim();

        //    if (string.IsNullOrEmpty(content))
        //    {
        //        PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("ReviewEmptyWarning"));
        //    }
        //    else
        //    {
        //        //GameClient.instance.RequestSendReview(content);
        //        Dismiss();
        //        PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("ThankForReview"));
        //    }
        //}
    }
}
