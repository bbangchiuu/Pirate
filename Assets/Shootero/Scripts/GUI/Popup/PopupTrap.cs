using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    public class PopupTrap : PopupBase
    {
        public BuffIcon buffIcon;
        public GameObject goldGrp;
        public GameObject hpGrp;
        public Text hpAmount;
        public Text goldAmount;

        GameObject visual;

        public static PopupTrap instance;

        public static PopupTrap Create(GameObject visual, BuffType randomBuff, long hp, long gold)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupTrap");
            instance = go.GetComponent<PopupTrap>();
            instance.Init(randomBuff, hp, gold);
            instance.visual = visual;
            return instance;
        }

        private void Init(BuffType randomBuff, long hp, long gold)
        {
            if (hp > 0)
            {
                hpGrp.gameObject.SetActive(true);
                goldGrp.gameObject.SetActive(false);
                buffIcon.gameObject.SetActive(false);

                hpAmount.text = hp.ToString();
            }
            else if (gold > 0)
            {
                hpGrp.gameObject.SetActive(false);
                goldGrp.gameObject.SetActive(true);
                buffIcon.gameObject.SetActive(false);
                goldAmount.text = gold.ToString();
            }
            else
            {
                hpGrp.gameObject.SetActive(false);
                goldGrp.gameObject.SetActive(false);
                buffIcon.gameObject.SetActive(true);
                
                buffIcon.SetBuffType(randomBuff);
            }
        }

        protected override void Hide()
        {
            if (ScreenBattle.instance)
                ScreenBattle.instance.UpdatePlayerGold();

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

