using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    public class PopupFlashModifier : PopupBase
    {
        public Text msg;

        GameObject visual;

        public static PopupFlashModifier instance;

        public static PopupFlashModifier Create(GameObject visual)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupFlashModifier");
            instance = go.GetComponent<PopupFlashModifier>();
            instance.Init();
            instance.visual = visual;
            return instance;
        }

        private void Init()
        {
            ScreenBattle.PauseGame(true);
            msg.text = Localization.Get("PopupFlashModifierMessage");
        }

        [GUIDelegate]
        public void OnBtnBuffClick()
        {
            QuanlyNguoichoi.Instance.PlayerUnit.ApplyFlashModifier();

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

