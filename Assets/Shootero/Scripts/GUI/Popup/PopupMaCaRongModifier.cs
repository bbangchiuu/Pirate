using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    public class PopupMaCaRongModifier : PopupBase
    {
        public Text msg;
        public BuffIcon buffIcon;

        GameObject visual;

        public static PopupMaCaRongModifier instance;

        public static PopupMaCaRongModifier Create(GameObject visual)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupMaCaRongModifier");
            instance = go.GetComponent<PopupMaCaRongModifier>();
            instance.Init();
            instance.visual = visual;
            return instance;
        }

        private void Init()
        {
            if (QuanlyNguoichoi.Instance == null) return;
            ScreenBattle.PauseGame(true);

            //var curHP = QuanlyNguoichoi.Instance.PlayerUnit.GetCurHP();
            //var dmg = MaCaRongModifier.GetCursedDmgByDracula(curHP);
            //var hp = curHP - dmg;
            msg.text = PopupLeoThap.GetMessageByMod(2);// Localization.Get("PopupMaCaRongModifierMessage");
            buffIcon.SetBuffType(BuffType.LIFE_LEECH);
        }

        [GUIDelegate]
        public void OnBtnBuffClick()
        {
            if (QuanlyNguoichoi.Instance)
            {
                QuanlyNguoichoi.Instance.PlayerUnit.GetMaCaRongModifier().MaCaRongCurse();
                QuanlyNguoichoi.Instance.IncreaseGioiHanBuff(1);
                QuanlyNguoichoi.Instance.GetBuff(BuffType.LIFE_LEECH);
            }

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

