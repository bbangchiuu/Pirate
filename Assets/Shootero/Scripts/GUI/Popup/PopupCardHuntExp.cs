using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    public class PopupCardHuntExp : PopupBase
    {
        //public Text lbKinhNghiem;

        GameObject visual;

        public static PopupCardHuntExp instance;

        public static PopupCardHuntExp Create(GameObject visual)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupCardHuntExp");
            instance = go.GetComponent<PopupCardHuntExp>();
            instance.Init();
            instance.visual = visual;
            return instance;
        }

        private void Init()
        {
            //lbKinhNghiem.text = "+" + kinhNghiem.ToString();
        }

        protected override void OnCleanUp()
        {
            //if (visual != null)
            //{
            //    visual.gameObject.SetActive(false);
            //}
            if (QuanlyNguoichoi.Instance)
            {
                if (QuanlyNguoichoi.Instance.IsSanTheMode)
                {
                    QuanlyNguoichoi.Instance.OnPlayerGetCardHuntExp(visual);
                }
                else if (QuanlyNguoichoi.Instance.IsNienThuMode)
                {
                    QuanlyNguoichoi.Instance.OnPlayerGetNienThuExp(visual);
                }
            }
        }
    }

}

