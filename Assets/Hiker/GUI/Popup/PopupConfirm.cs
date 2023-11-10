using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker.GUI
{
    using UnityEngine.UI;

    public class PopupConfirm : PopupBase
    {
        public Text lblContent, titleText;
        public Button btnAccept, btnCancel;

        private System.Action YesAction { get; set; }
        private System.Action NoAction { get; set; }

        public static PopupConfirm Instance { get; set; }

        private static void Create()
        {
            //if (Instance == null)
            {
                GameObject obj = PopupManager.instance.GetPopup("PopupConfirm", false, Vector3.zero);
                Instance = obj.GetComponent<PopupConfirm>();
            }
        }

        public static void Create(string content, System.Action yes_action, bool hasNoBtn = false, string accept_label = "", string title = "")
        {
            Create();
            Instance.Init(content, yes_action, null, hasNoBtn, accept_label, title);
        }

        public static void Create(string content, System.Action yes_action, System.Action no_action, string accept_label = "")
        {
            Create();
            Instance.Init(content, yes_action, no_action, true, accept_label);
        }

        public static void CreateMultiColor(string content, System.Action yes_action, System.Action no_action, string accept_label = "")
        {
            Create();
            Instance.Init(content, yes_action, no_action, true, accept_label);
            Instance.lblContent.color = new Color32(255, 255, 255, 255);
        }

        private void Init(string content, System.Action yes_action, System.Action no_action, bool hasNoBtn, string accept_label, string title = "")
        {
            this.lblContent.text = content;
            this.btnAccept.GetComponentInChildren<Text>().text = string.IsNullOrEmpty(accept_label) ? Localization.Get("ok") : accept_label;
            
            this.btnCancel.gameObject.SetActive(hasNoBtn);
            if (hasNoBtn)
            {
                this.btnCancel.GetComponentInChildren<Text>().text = Localization.Get("cancel");
            }
            this.YesAction = yes_action;
            this.NoAction = no_action;
            //this.GetComponentInChildren<UIGrid>().Reposition();
            //this.titleText.text = title;
            if (string.IsNullOrEmpty(title))
            {
                this.titleText.gameObject.SetActive(false);
            }
            else
            {
                this.titleText.gameObject.SetActive(true);
                this.titleText.text = title;
            }
            //this.lblContent.transform.localPosition = string.IsNullOrEmpty(title) ? new Vector3(0, -90, 0) : new Vector3(0, -130, 0);
            this.lblContent.color = new Color32(57, 57, 57, 255);

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }

        public static void Dismiss(PopupConfirm instance)
        {
            if (Instance == instance)
            {
                Instance = null;
            }

            if (instance != null)
            {
                instance.OnCloseBtnClick();
            }
        }

        [GUIDelegate]
        public void OnYesClick()
        {
            Dismiss(this);
            if (this.YesAction != null)
            {
                this.YesAction();
            }
        }

        [GUIDelegate]
        public void OnNoClick()
        {
            Dismiss(this);
            if (this.NoAction != null)
            {
                this.NoAction();
            }
        }

        [GUIDelegate]
        public override void OnBackBtnClick()
        {
            if (btnCancel.gameObject.activeSelf)
            {
                OnCloseBtnClick();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            //if (PopupDownloadingAB.instance != null)
            //{
            //    foreach (UIPanel childPanel in this.panelsInPopup)
            //    {
            //        childPanel.depth += 5000;
            //        childPanel.sortingOrder = childPanel.depth;
            //    }
            //}
        }
    }
}