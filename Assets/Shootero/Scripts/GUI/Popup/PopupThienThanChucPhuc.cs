using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    public class PopupThienThanChucPhuc : PopupBase
    {
        public Text lbHp;
        public Text lbAtk;

        public Button btnHP;
        public Button btnAtk;

        GameObject visual;
        CodeStage.AntiCheat.ObscuredTypes.ObscuredLong atkBonus;
        CodeStage.AntiCheat.ObscuredTypes.ObscuredLong hpBonus;

        public static PopupThienThanChucPhuc instance;

        public static PopupThienThanChucPhuc Create(GameObject visual, long atk, long hp)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupThienThanChucPhuc");
            instance = go.GetComponent<PopupThienThanChucPhuc>();
            instance.Init(atk, hp);
            instance.visual = visual;
            instance.btnAtk.onClick.AddListener(instance.OnBtnAtkClick);
            instance.btnHP.onClick.AddListener(instance.OnBtnHPClick);

            return instance;
        }

        private void Init(long atk, long hp)
        {
            this.atkBonus = atk;
            this.hpBonus = hp;

            lbHp.text = string.Format("+{0}", hp);
            lbAtk.text = string.Format("+{0}", atk);
        }

        void OnBtnHPClick()
        {
            if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.PlayerUnit)
                QuanlyNguoichoi.Instance.PlayerUnit.IncHPUp(hpBonus);

            OnCloseBtnClick();
        }

        void OnBtnAtkClick()
        {
            if (QuanlyNguoichoi.Instance && QuanlyNguoichoi.Instance.PlayerUnit)
                QuanlyNguoichoi.Instance.PlayerUnit.IncATKUp(atkBonus);

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

