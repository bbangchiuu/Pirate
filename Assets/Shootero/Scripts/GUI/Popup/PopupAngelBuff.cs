using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    public class PopupAngelBuff : PopupBase
    {
        public BuffIcon[] buffIcons;

        GameObject visual;
        bool mSelected = false;
        public static PopupAngelBuff instance;

        public static readonly List<BuffType> listAvailableType = new List<BuffType>()
        {
            BuffType.HEAL,
            BuffType.ATK_UP_SMALL,
            BuffType.ATKSPD_UP_SMALL,
            BuffType.CRIT_UP_SMALL,
        };

        public List<BuffType> listCurBuffTypes = new List<BuffType>();

        public static PopupAngelBuff Create(GameObject visual)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupAngelBuff");
            instance = go.GetComponent<PopupAngelBuff>();
            instance.Init();
            instance.visual = visual;

            return instance;
        }

        BuffType RandomBuff(bool haveHeal)
        {
            var r = Random.Range(haveHeal ? 1 : 0, listAvailableType.Count);

            return listAvailableType[r];
        }

        private void Init()
        {
            mSelected = false;
            listCurBuffTypes.Clear();
            bool haveHeal = false;

            for (int i = buffIcons.Length - 1; i >= 0; --i)
            {
                BuffType randomType = BuffType.HEAL;
                if (haveHeal)
                {
                    randomType = RandomBuff(haveHeal);
                }

                if (randomType == BuffType.HEAL)
                {
                    haveHeal = true;
                }

                listCurBuffTypes.Insert(0, randomType);
                var icon = buffIcons[i];
                icon.SetBuffType(randomType,true);
            }
        }

        [GUIDelegate]
        public void OnBtnBuffClick(int buffIndex)
        {
            if (mSelected) return; // prevent multi select buff with multitouch
            mSelected = true;
            var buff = listCurBuffTypes[buffIndex];
            if (QuanlyNguoichoi.Instance)
            {
                QuanlyNguoichoi.Instance.GetBuff(buff);
            }

            Dismiss();
        }

        protected override void OnCleanUp()
        {
            QuanlyManchoi.instance.OnLevelClear();

            if (visual != null)
            {
                visual.gameObject.SetActive(false);
            }
        }

        public static void Dismiss()
        {
            if (instance != null)
            {
                instance.OnCloseBtnClick();
                instance = null;
            }
        }
    }

}

