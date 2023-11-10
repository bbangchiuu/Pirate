using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;
    public class PopupDatTen : PopupBase
    {
        public InputField inputField;

        public static PopupDatTen instance;

        System.Action onSuccessDatTen;

        public static PopupDatTen Create(System.Action onDatTenSuccess)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupDatTen");
            instance = go.GetComponent<PopupDatTen>();
            instance.Init();
            instance.onSuccessDatTen = onDatTenSuccess;
            return instance;
        }

        public void Init()
        {
            var userName = GameClient.instance.UInfo.Gamer.GetDisplayName();
            if (string.IsNullOrEmpty(userName) == false)
            {
                inputField.text = userName;
            }

            inputField.Select();
            inputField.ActivateInputField();
        }

        [GUIDelegate]
        public void OnOkBtnclick()
        {
            if (inputField.text != GameClient.instance.UInfo.Gamer.GetDisplayName())
            {
                var inputText = inputField.text;
                if (string.IsNullOrEmpty(inputText))
                {
                    PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("DoiTenValidateMessage"));
                    return;
                }
                else
                if (ConfigManager.ValidateUserName(inputField.text) == false)
                {
                    PopupMessage.Create(MessagePopupType.TEXT, Localization.Get("DoiTenValidateMessage"));
                    return;
                }

                GameClient.instance.RequestDoiTen(inputField.text, onSuccessDatTen);
            }
            OnCloseBtnClick();
        }
    }
}