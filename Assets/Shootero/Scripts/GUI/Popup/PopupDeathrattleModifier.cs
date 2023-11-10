using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    public class PopupDeathrattleModifier : PopupBase
    {
        public Text msg;

        GameObject visual;

        public static PopupDeathrattleModifier instance;

        public static PopupDeathrattleModifier Create(GameObject visual)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupDeathrattleModifier");
            instance = go.GetComponent<PopupDeathrattleModifier>();
            instance.Init();
            instance.visual = visual;
            return instance;
        }

        private void Init()
        {
            ScreenBattle.PauseGame(true);
            msg.text = Localization.Get("PopupDeathrattleModifierMessage");
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

