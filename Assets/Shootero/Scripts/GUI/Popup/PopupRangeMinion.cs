using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    public class PopupRangeMinion : PopupBase
    {
        GameObject visual;

        public static PopupRangeMinion instance;

        public static PopupRangeMinion Create(GameObject visual)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupRangeMinion");
            instance = go.GetComponent<PopupRangeMinion>();
            instance.Init();
            instance.visual = visual;
            return instance;
        }

        private void Init()
        {
        }

        [GUIDelegate]
        public void OnBtnOkClick()
        {
            if (QuanlyNguoichoi.Instance)
                QuanlyNguoichoi.Instance.ThuPhucDeTu("ThoNgoc");

            OnCloseBtnClick();
        }

        protected override void Hide()
        {
            if (visual != null)
            {
                visual.gameObject.SetActive(false);
            }
            base.Hide();
            if (QuanlyManchoi.instance)
                QuanlyManchoi.instance.OnLevelClear();
        }
    }

}

