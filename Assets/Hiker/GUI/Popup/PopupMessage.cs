using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker.GUI
{
    using UnityEngine.UI;

    public class PopupMessage : PopupBase
    {
        public Image top, bottom;
        public Text lblMessage;
        static PopupMessage Instance = null;

        public static void Create(MessagePopupType message_type, string message_content)
        {
            //if (PopupCloudEffect.instance)
            //{
            //    PopupCloudEffect.instance.CloseEffect();
            //}

            if (Instance == null)
            {
                GameObject go = PopupManager.instance.GetPopup("PopupMessage");
                Instance = go.GetComponent<PopupMessage>();
            }

            Instance.Show(message_type, message_content);
        }

        private void Show(MessagePopupType message_type, string message_content)
        {
            this.top.color = new Color32(255, 255, 255, 200);
            this.bottom.color = new Color32(255, 255, 255, 200);

            if (message_type == MessagePopupType.TEXT)
            {
                this.lblMessage.color = new Color32(255, 255, 255, 255);
            }
            else if (message_type == MessagePopupType.ERROR)
            {
                this.lblMessage.color = new Color32(255, 137, 137, 255);
            }

            this.lblMessage.text = message_content;
            this.StopAllCoroutines();

            var tween_position = this.lblMessage.GetComponent<TweenPosition>();
            if (tween_position != null) tween_position.enabled = false;

            var tween_scale = this.lblMessage.GetComponent<TweenScale>();
            if (tween_scale != null) tween_scale.enabled = false;

            var tween_alpha = this.lblMessage.GetComponent<TweenAlpha>();
            if (tween_alpha != null) tween_alpha.enabled = false;

            var tween_alpha_top = this.top.GetComponent<TweenAlpha>();
            if (tween_alpha_top != null) tween_alpha_top.enabled = false;

            var tween_alpha_btm = this.bottom.GetComponent<TweenAlpha>();
            if (tween_alpha_btm != null) tween_alpha_btm.enabled = false;

            //this.lblMessage.alpha = 1f;
            this.lblMessage.transform.localPosition = new Vector3(0, 30, 0);
            this.lblMessage.transform.localScale = Vector3.one;
            this.gameObject.SetActive(true);
            this.StopAllCoroutines();
            this.StartCoroutine(this.CoShow());

            this.transform.SetAsLastSibling();
        }

        private IEnumerator CoShow()
        {
            TweenScale twScale = TweenScale.Begin(this.lblMessage.gameObject, 0.1f, Vector3.one * 1.2f);
            twScale.ignoreTimeScale = true;

            yield return new WaitForSecondsRealtime(0.2f);
            TweenPosition twPos = TweenPosition.Begin(this.lblMessage.gameObject, .6f, new Vector3(0, 40, 0));
            twPos.ignoreTimeScale = true;

            yield return new WaitForSecondsRealtime(1f);
            TweenAlpha twAlpha = TweenAlpha.Begin(this.lblMessage.gameObject, .6f, 0f);
            twAlpha.ignoreTimeScale = true;
            TweenAlpha twAlpha_top = TweenAlpha.Begin(this.top.gameObject, .6f, 0f);
            twAlpha_top.ignoreTimeScale = true;
            TweenAlpha twAlpha_btm = TweenAlpha.Begin(this.bottom.gameObject, .6f, 0f);
            twAlpha_btm.ignoreTimeScale = true;

            yield return new WaitForSecondsRealtime(.6f);
            this.gameObject.SetActive(false);
        }
    }

    public enum MessagePopupType
    {
        TEXT,
        ERROR,
    }
}