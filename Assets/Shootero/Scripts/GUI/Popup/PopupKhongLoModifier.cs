using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    public class PopupKhongLoModifier : PopupBase
    {
        public Text msg;

        GameObject visual;

        public static PopupKhongLoModifier instance;

        public static PopupKhongLoModifier Create(GameObject visual)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupKhongLoModifier");
            instance = go.GetComponent<PopupKhongLoModifier>();
            instance.Init();
            instance.visual = visual;
            return instance;
        }

        private void Init()
        {
            ScreenBattle.PauseGame(true);
            msg.text = Localization.Get("PopupKhongLoModifierMessage");
        }

        [GUIDelegate]
        public void OnBtnBuffClick()
        {
            OnCloseBtnClick();
        }

        protected override void OnCleanUp()
        {
            ScreenBattle.PauseGame(false);
            if (visual != null)
            {
                visual.gameObject.SetActive(false);
            }
        }
    }

}

