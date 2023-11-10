using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    public class PopupCardHuntBoss : PopupBase
    {
        //public Text lbKinhNghiem;

        GameObject visual;

        public static PopupCardHuntBoss instance;

        public static PopupCardHuntBoss Create(GameObject visual)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupCardHuntBoss");
            instance = go.GetComponent<PopupCardHuntBoss>();
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
            if (QuanlyNguoichoi.Instance)
            {
                QuanlyNguoichoi.Instance.OnPlayerGetCardHuntBoss(visual);
            }
        }
    }

}

