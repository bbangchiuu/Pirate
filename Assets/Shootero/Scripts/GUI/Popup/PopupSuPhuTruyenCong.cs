using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    public class PopupSuPhuTruyenCong : PopupBase
    {
        public Text lbKinhNghiem;

        GameObject visual;

        public static PopupSuPhuTruyenCong instance;

        public static PopupSuPhuTruyenCong Create(GameObject visual, long kinhNghiem)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupSuPhuTruyenCong");
            instance = go.GetComponent<PopupSuPhuTruyenCong>();
            instance.Init(kinhNghiem);
            instance.visual = visual;
            return instance;
        }

        private void Init(long kinhNghiem)
        {
            lbKinhNghiem.text = "+" + kinhNghiem.ToString();
        }

        protected override void OnCleanUp()
        {
            if (visual != null)
            {
                visual.gameObject.SetActive(false);
            }
        }
    }

}

