using UnityEngine;
using UnityEngine.UI;

namespace Hiker.GUI
{
    public class PopupLoginSocialFail : PopupBase
    {
        public static PopupLoginSocialFail Instance { get; set; }
        public Text lblContent, lblLoginDevice, lbConfirm;
        public Button btnConfirm, btnCancel;//, btnRestart;
        public Image iconGameCenter;
        public Image iconGooglePlay;
        private System.Action ConfirmAction { get; set; }
        private System.Action CancelAction { get; set; }

        protected override void Awake()
        {
            base.Awake();
#if UNITY_IOS
            iconGameCenter.gameObject.SetActive(true);
            iconGooglePlay.gameObject.SetActive(false);
#else
            iconGameCenter.gameObject.SetActive(false);
            iconGooglePlay.gameObject.SetActive(true);
#endif
        }

        public static void Create(string content, string confirm_label, System.Action confirm_action, System.Action cancel_action)
        {
            if (Instance == null)
            {
                var go = PopupManager.instance.GetPopup("PopupLoginSocialFail", false, Vector3.zero);
                Instance = go.GetComponent<PopupLoginSocialFail>();
            }
            Instance.Init(content, confirm_label, confirm_action, cancel_action);
        }

        private void Init(string content, string confirm_label, System.Action confirm_action, System.Action cancel_action)
        {
            this.lblContent.text = content;
            this.lbConfirm.text = confirm_label;
            this.ConfirmAction = confirm_action;
            this.CancelAction = cancel_action;
            this.btnConfirm.gameObject.SetActive(true);
            //this.btnRestart.gameObject.SetActive(false);

            if (GameClient.instance.UInfo != null &&
                GameClient.instance.UInfo.Gamer != null &&
                GameClient.instance.UInfo.Gamer.Name != null)
            {
                this.lblLoginDevice.text = string.Format("{0} ({1})", Localization.Get("LoginDefault"), GameClient.instance.UInfo.Gamer.Name);
            }
            else
            {
                this.lblLoginDevice.text = string.Format(Localization.Get("LoginWithDeviceID"));
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (!pause && GUIManager.Instance.CurrentScreen == "Loading")
            {
#if UNITY_ANDROID
                if (SocialManager.Instance.GoogleServiceIsInstalled())
                {
                    GUIManager.Instance.RestartGame();
                }
#endif
            }
        }

        public static void Dismiss()
        {
            if (Instance != null)
            {
                Instance.Hide();
                Instance = null;
            }
        }

        [GUIDelegate]
        public void OnConfirmClick()
        {
            //Dismiss();
            this.ConfirmAction?.Invoke();
        }

        [GUIDelegate]
        public void OnCancelClick()
        {
            Dismiss();
            this.CancelAction?.Invoke();
        }

        //[GUIDelegate]
        //public void OnRestartClick()
        //{
        //    Dismiss();
        //    GUIManager.Instance.RestartGame();
        //}
    }
}
