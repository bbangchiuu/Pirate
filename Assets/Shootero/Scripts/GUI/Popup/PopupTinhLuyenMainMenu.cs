using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using UnityEngine.UI;

    public class PopupTinhLuyenMainMenu : PopupBase
    {
        public GameObject btnForge, btnUpStar, btnReforge;
        public static PopupTinhLuyenMainMenu instance;
        

        public static PopupTinhLuyenMainMenu Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupTinhLuyenMainMenu");
            instance = go.GetComponent<PopupTinhLuyenMainMenu>();
            instance.Init();
            return instance;
        }

        bool activeResourceBar = false;

        public void Init()
        {
            activeResourceBar = true;
            if (ResourceBar.instance)
            {
                activeResourceBar = ResourceBar.instance.gameObject.activeSelf;
                ResourceBar.instance.gameObject.SetActive(false);
            }

        }

        [GUIDelegate]
        public void OnForgeBtnClick()
        {
            OnBackBtnClick();
            PopupTinhLuyenTrangBi.Create();
        }

        [GUIDelegate]
        public void OnUpStarBtnClick()
        {
            OnBackBtnClick();
            PopupTinhLuyenNangSao.Create();
        }

        [GUIDelegate]
        public void OnReforgeBtnClick()
        {
            OnBackBtnClick();
            PopupTinhLuyenDoiTB.Create();
        }



        [GUIDelegate]
        public override void OnBackBtnClick()
        {
            base.OnBackBtnClick();
        }

        protected override void Hide()
        {
            base.Hide();

            if (ResourceBar.instance)
            {
                if (GUIManager.Instance.CurrentScreen == "Main")
                    activeResourceBar = true;

                ResourceBar.instance.gameObject.SetActive(activeResourceBar);
            }
        }
    }
}