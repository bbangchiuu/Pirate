using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;
    public class PopupDevilBuff : PopupBase
    {
        public Text msg;
        public BuffIcon[] buffIcons;

        GameObject visual;

        public static PopupDevilBuff instance;

        public static readonly List<BuffType> listAvailableType = new List<BuffType>()
        {
            //BuffType.HEAL,
            //BuffType.ATK_UP_SMALL,
            //BuffType.ATKSPD_UP_SMALL,
            //BuffType.CRIT_UP_SMALL,
        };

        public List<BuffType> listCurBuffTypes = new List<BuffType>();

        public static PopupDevilBuff Create(GameObject visual)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupDevilBuff");
            instance = go.GetComponent<PopupDevilBuff>();
            instance.Init();
            instance.visual = visual;
            return instance;
        }

        BuffType RandomBuff(List<BuffType> excludedBuff)
        {
            int randCount = 0;
            BuffType result = BuffType.HEAL;
            List<BuffStat> listRandomBuffs = new List<BuffStat>();
            foreach (var b in QuanlyNguoichoi.Instance.gameConfig.Buffs)
            {
                if (excludedBuff.Contains(b.Type)) continue;

                int curCount = QuanlyNguoichoi.Instance.PlayerUnit.GetBuffCount(b.Type);
                if ((b.MaxCount == 0 ||
                    curCount < b.MaxCount) &&
                    b.RandomRate > 0 &&
                    b.ChapUnlock <= QuanlyNguoichoi.Instance.ChapterIndex)
                {
                    if (listAvailableType == null ||
                        listAvailableType.Count <= 0 ||
                        listAvailableType.Contains(b.Type))
                    {
                        randCount += b.RandomRate;
                        listRandomBuffs.Add(b);
                    }

                }
            }

            if (randCount > 0)
            {
                var r = QuanlyNguoichoi.Instance.GetRandomRollingBuff(randCount);//Random.Range(0, randCount);
                for (int i = 0; i < listRandomBuffs.Count; ++i)
                {
                    var b = listRandomBuffs[i];
                    if (b.RandomRate > 0)
                    {
                        if (r < b.RandomRate)
                        {
                            result = b.Type;
                            break;
                        }
                        else
                        {
                            r -= b.RandomRate;
                        }
                    }
                }
            }

            //#if UNITY_EDITOR
            //            // cheat
            //            result = BuffType.BACK_SHOT;
            //#endif

            return result;
        }

        private void Init()
        {
            listCurBuffTypes.Clear();
            List<BuffType> listExclude = new List<BuffType>();
            var buffHP = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.HP_UP);
            msg.text = string.Format(Localization.Get("PopupDevilBuffMessage"), buffHP.Params[0]);

            var evationCfg = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.EVASION);
            var curEvation = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat().EVASION;
            if (curEvation + evationCfg.Params[0] > ConfigManager.GetMaxEvasion())
            {
                listExclude.Add(BuffType.EVASION);
            }
            if (QuanlyNguoichoi.Instance.HaveHeroPlus("Magician") &&
                QuanlyNguoichoi.Instance.PlayerUnit.UnitName == "Magician")
            {
                listExclude.Add(BuffType.ELECTRIC_EFF);
                listExclude.Add(BuffType.FLAME_EFF);
                listExclude.Add(BuffType.FROZEN_EFF);
            }

            for (int i = buffIcons.Length - 1; i >= 0; --i)
            {
                BuffType randomType = RandomBuff(listExclude);

                listCurBuffTypes.Add(randomType);
                buffIcons[i].SetBuffType(randomType,true);
            }
        }
        [GUIDelegate]
        public void OnBtnBuffClick()
        {
            if (QuanlyNguoichoi.Instance)
            {
                var buffHP = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.HP_UP);
                QuanlyNguoichoi.Instance.PlayerUnit.DebuffHPUp((int)buffHP.Params[0]);

                foreach (var buff in listCurBuffTypes)
                {
                    QuanlyNguoichoi.Instance.IncreaseGioiHanBuff(1);
                    QuanlyNguoichoi.Instance.GetBuff(buff);
                }
            }

            OnCloseBtnClick();
        }

        protected override void OnCleanUp()
        {
            QuanlyManchoi.instance.OnLevelClear();

            if (visual != null)
            {
                visual.gameObject.SetActive(false);
            }
        }
    }

}

