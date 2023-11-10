using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using UnityEngine.UI;

    public class PopupBlackSmith : PopupBase
    {
        public Transform itemParent;
        public BuffIcon buffIcon;
        public Button okBtn;
        public Button cancelBtn;
        List<BuffIcon> buffIcons = new List<BuffIcon>();

        public static readonly List<BuffType> listAvailableType = new List<BuffType>()
        {
            //BuffType.HEAL,
            //BuffType.HP_UP,
            //BuffType.ATK_UP,
            //BuffType.ATKSPD_UP,
            //BuffType.CRIT_UP,
            //BuffType.MULTI_SHOT,
            //BuffType.ADD_SHOT,
            //BuffType.SIDE_SHOT,
            //BuffType.CHEO_SHOT,
            //BuffType.BACK_SHOT,
            //BuffType.RICOCHET,
            //BuffType.PIERCING,
            //BuffType.BOUNCING,
            //BuffType.SMART,
            //BuffType.FLY,
            //BuffType.SLOW_PROJECTILE,
            //BuffType.LINKEN,
            //BuffType.REFLECT,
            //BuffType.LIFE,
            //BuffType.ELECTRIC_EFF,
            //BuffType.FROZEN_EFF,
            //BuffType.FLAME_EFF,
            //BuffType.ELECTRIC_FIELD,
            //BuffType.FROZEN_FIELD,
            //BuffType.FLAME_FIELD,
            //BuffType.RAGE_ATK,
            //BuffType.RAGE_ATKSPD,
        };

        public GameObject visual;

        public static PopupBlackSmith instance;

        public static PopupBlackSmith Create(GameObject visual)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupBlackSmith");
            instance = go.GetComponent<PopupBlackSmith>();
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
                var r = QuanlyNguoichoi.Instance.GetRandomRollingBuff(randCount); //Random.Range(0, randCount);
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

        private static int BuffCompare(Transform lhs, Transform rhs)
        {
            if (lhs == rhs) return 0;
                        
            return string.Compare(lhs.name,rhs.name);
        }

        public static void SortBuff(GameObject o)
        {
            var children = new List<Transform>(o.GetComponentsInChildren<Transform>() );
            children.Remove(o.transform);
            children.Sort(BuffCompare);
            for (int i = 0; i < children.Count; i++)
                children[i].SetSiblingIndex(i);
        }

        private void Init()
        {
            selectedIndex = -1;
            okBtn.interactable = false;
            buffIcon.gameObject.SetActive(false);
            for (int i = 0; i < QuanlyNguoichoi.Instance.ListBuffs.Count; ++i)
            {
                var b = QuanlyNguoichoi.Instance.ListBuffs[i];
                BuffIcon icon = null;
                if (buffIcons.Count > i)
                {
                    icon = buffIcons[i];
                }
                else
                {
                    icon = Instantiate(buffIcon, itemParent);
                    icon.gameObject.SetActive(true);
                    icon.name = b.Type.ToString();
                    buffIcons.Add(icon);
                }

                icon.SetBuffType(b.Type);
                var btn = icon.GetComponent<Button>();
                if (b.Type == BuffType.HEAL)
                {
                    btn.interactable = false;
                }
                else if (b.Type == BuffType.LIFE)
                {
                    btn.interactable = false;
                }
                else
                {
                    int index = i;
                    btn.onClick.AddListener(() => OnBtnBuffClick(index));
                    btn.interactable = true;
                }
            }

            SortBuff(itemParent.gameObject);


            //listCurBuffTypes.Clear();

            //bool haveHeal = false;

            //for (int i = buffIcons.Length - 1; i >= 0; --i)
            //{
            //    BuffType randomType = BuffType.HEAL;
            //    if (haveHeal)
            //    {
            //        randomType = RandomBuff(haveHeal);
            //    }

            //    if (randomType == BuffType.HEAL)
            //    {
            //        haveHeal = true;
            //    }

            //    listCurBuffTypes.Add(randomType);
            //    var icon = buffIcons[i];
            //    icon.iconSkill.sprite = Resources.Load<Sprite>(string.Format("BuffIcons/{0}_Buff", randomType.ToString()));
            //    icon.lblName.text = randomType.ToString();
            //}
        }

        int selectedIndex = -1;
        BuffType randomType = BuffType.HEAL;

        void OnBtnBuffClick(int buffIndex)
        {
            if (buffIndex < 0 || buffIndex >= QuanlyNguoichoi.Instance.ListBuffs.Count) return;
            if (selectedIndex >= 0) return;
            selectedIndex = buffIndex;

            var listCurBuffTypes = new List<BuffType>();

            var evationCfg = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.EVASION);
            var curEvation = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat().EVASION;
            if (curEvation + evationCfg.Params[0] > ConfigManager.GetMaxEvasion())
            {
                listCurBuffTypes.Add(BuffType.EVASION);
            }
            if (QuanlyNguoichoi.Instance.HaveHeroPlus("Magician") &&
                QuanlyNguoichoi.Instance.PlayerUnit.UnitName == "Magician")
            {
                listCurBuffTypes.Add(BuffType.ELECTRIC_EFF);
                listCurBuffTypes.Add(BuffType.FLAME_EFF);
                listCurBuffTypes.Add(BuffType.FROZEN_EFF);
            }

            var buff = QuanlyNguoichoi.Instance.ListBuffs[buffIndex];
            listCurBuffTypes.Add(buff.Type);

            randomType = RandomBuff(listCurBuffTypes);
            buffIcon.gameObject.SetActive(true);
            buffIcon.SetBuffType(randomType,true);

            foreach (var icon in buffIcons)
            {
                var btn = icon.GetComponent<Button>();
                btn.interactable = false;
            }

            okBtn.interactable = true;
            //Dismiss();
        }

        [GUIDelegate]
        public void OnBtnOKClick()
        {
            if (selectedIndex < 0 || selectedIndex >= QuanlyNguoichoi.Instance.ListBuffs.Count) return;

            if (QuanlyNguoichoi.Instance)
            {
                BuffStat buff = new BuffStat();
                bool isRemoved = QuanlyNguoichoi.Instance.RemoveBuff(selectedIndex, ref buff);
                if (isRemoved)
                {
                    QuanlyNguoichoi.Instance.IncreaseGioiHanBuff(1);
                    QuanlyNguoichoi.Instance.GetBuff(randomType);
                }
            }

            Dismiss();
        }

        protected override void OnCleanUp()
        {
            QuanlyManchoi.instance.OnLevelClear();

            if (visual)
            {
                visual.gameObject.SetActive(false);
            }
        }

        public static void Dismiss()
        {
            if (instance != null)
            {
                instance.OnCloseBtnClick();
            }
        }
    }

}

