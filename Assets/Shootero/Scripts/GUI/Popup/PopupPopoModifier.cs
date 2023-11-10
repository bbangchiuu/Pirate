using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    public class PopupPopoModifier : PopupBase
    {
        public Text msg;

        GameObject visual;

        public static PopupPopoModifier instance;

        public static PopupPopoModifier Create(GameObject visual)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupPopoModifier");
            instance = go.GetComponent<PopupPopoModifier>();
            instance.Init();
            instance.visual = visual;
            return instance;
        }

        private void Init()
        {
            ScreenBattle.PauseGame(true);
            msg.text = Localization.Get("PopupPopoModifierMessage");
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

