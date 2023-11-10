using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    public class PopupInfernalModifier : PopupBase
    {
        public Text msg;

        GameObject visual;

        public static PopupInfernalModifier instance;

        public static PopupInfernalModifier Create(GameObject visual)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupInfernalModifier");
            instance = go.GetComponent<PopupInfernalModifier>();
            instance.Init();
            instance.visual = visual;
            return instance;
        }

        private void Init()
        {
            ScreenBattle.PauseGame(true);
            msg.text = Localization.Get("PopupInfernalModifierMessage");
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
                var bh = visual.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
                bh.EnableBehavior();
                //visual.gameObject.SetActive(false);
                HikerUtils.DoAction(QuanlyNguoichoi.Instance, () =>
                {
                    QuanlyNguoichoi.Instance.PlayerUnit.GetInfernalModifier().InfernalSkill();
                }, 1.5f);
            }
        }
    }

}

