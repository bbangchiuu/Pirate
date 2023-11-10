using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;

    public class ScreenLogin : ScreenBase
    {
        public InputField deviceID;

        public override void OnActive()
        {
            if (ResourceBar.instance)
            {
                ResourceBar.instance.gameObject.SetActive(false);
            }

            string id = PlayerPrefs.GetString("DeviceID", string.Empty);
            if (string.IsNullOrEmpty(id) == false)
            {
                this.deviceID.text = id;
            }
            base.OnActive();
        }

        public override bool OnBackBtnClick()
        {
            return true;
        }

        [GUIDelegate]
        public void OnBtnLoginClick()
        {
            var curVal = deviceID.text;
            curVal = curVal.Trim();
            if (string.IsNullOrEmpty(curVal) == false)
            {
                GameClient.instance.LoginDevice(curVal);
            }
        }
    }
}