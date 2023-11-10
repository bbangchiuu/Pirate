using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using DanielLochner.Assets.SimpleScrollSnap;
    using UnityEngine.UI;
    using UnityEngine.UI.Extensions;

    public class PopupChonDeTuIR : PopupBase
    {
        public static PopupChonDeTuIR instance;
        bool mSelected = false;

        SpriteCollection buffIconCol;
        
        public static PopupChonDeTuIR Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupChonDeTuIR");
            instance = go.GetComponent<PopupChonDeTuIR>();
            instance.Init();
            ScreenBattle.PauseGame(true);
            return instance;
        }

        SpriteCollection GetBuffIconCollection()
        {
            if (buffIconCol == null)
            {
                buffIconCol = Resources.Load<SpriteCollection>("BuffIcons");
            }
            return buffIconCol;
        }

        private void Init()
        {
            mSelected = false;
            
        }
        private void Start()
        {
        }

        [GUIDelegate]
        public void OnBtnBuffClick(int buffIndex)
        {
            if (mSelected) return;
            mSelected = true;

            string deTu = IronManSkill.GetDeTuNameByID(buffIndex);

            if (string.IsNullOrEmpty(deTu) == false &&
                QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.PlayerUnit &&
                QuanlyNguoichoi.Instance.PlayerUnit.UnitName == "IronMan")
            {
                var sk = QuanlyNguoichoi.Instance.PlayerUnit.GetKyNang().GetTuyetKy(0) as IronManSkill;

                sk.ThuPhucDeTu(deTu);
            }

            ScreenBattle.PauseGame(false);
            OnCloseBtnClick();
        }

        public static void Dismiss()
        {
            ScreenBattle.PauseGame(false);
            if (instance != null)
            {
                instance.OnCloseBtnClick();
            }
        }
    }
}