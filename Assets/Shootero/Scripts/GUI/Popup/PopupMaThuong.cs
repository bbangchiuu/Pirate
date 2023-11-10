using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    public class PopupMaThuong : PopupBase
    {
        public InputField inputField;

        public static PopupMaThuong instance;

        public static PopupMaThuong Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupMaThuong");
            instance = go.GetComponent<PopupMaThuong>();
            instance.Init();
            return instance;
        }

        public void Init()
        {
            inputField.Select();
            inputField.ActivateInputField();
        }

        [GUIDelegate]
        public void OnOkBtnclick()
        {
            if (string.IsNullOrEmpty(inputField.text) == false)
            {
                GameClient.instance.RequestUseGiftCode(inputField.text);
            }
            OnCloseBtnClick();
        }
    }
}