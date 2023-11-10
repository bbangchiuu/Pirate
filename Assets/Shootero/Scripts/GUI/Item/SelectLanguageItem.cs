using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;

    public class SelectLanguageItem : MonoBehaviour
    {
        public Text lblLang;
        public string Language { get; private set; }

        public void SetItem(string lang)
        {
            Language = lang;
            lblLang.text = Localization.Get(lang);
        }

        [GUIDelegate]
        public void OnBtnClick()
        {
            if (Language != ConfigManager.language)
            {
                ConfigManager.SetLanguage(Language);
            }
            else
            {
                PopupSelectLanguage.Dismiss();
            }
        }
    }
}

