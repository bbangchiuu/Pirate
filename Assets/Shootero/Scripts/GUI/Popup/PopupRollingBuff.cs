using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using DanielLochner.Assets.SimpleScrollSnap;
    using UnityEngine.UI;
    using UnityEngine.UI.Extensions;

    public class PopupRollingBuff : PopupBase
    {
        public BuffIcon[] slotSelected;

        //public BuffIcon buffIconPrefab;

        public Button btnInfo;
        public Button btnReroll;

        public static PopupRollingBuff instance;
        bool mSelected = false;
        int soOChon = 0;

        public static readonly List<BuffType> listAvailableType = new List<BuffType>()
        {
#if DEBUG
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
            //BuffType.SHIELD,
            //BuffType.RAGE_ATKSPD,
            //BuffType.HEAL,
            //BuffType.GIANT,
            //BuffType.DWARF,
            //BuffType.ELECTRIC_FIELD,
            //BuffType.FLAME_FIELD,
            //BuffType.FROZEN_FIELD,
            //BuffType.RUNNING_CHARGE,
            //BuffType.TRACKING_BULLET
#endif
        };

        SpriteCollection buffIconCol;

        public static List<BuffType> listCurBuffTypes = new List<BuffType>();
        
        public static PopupRollingBuff Create(int num = 3)
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupRollingBuff");
            instance = go.GetComponent<PopupRollingBuff>();
            instance.Init(num);
            ScreenBattle.PauseGame(true);
            return instance;
        }

        static List<BuffStat> listRandomBuffs = new List<BuffStat>();
        BuffType RandomBuff(List<BuffType> excludedBuff)
        {
            int randCount = 0;
            BuffType result = BuffType.HEAL;
            listRandomBuffs.Clear();
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
                //var r = Random.Range(0, randCount);
                var r = QuanlyNguoichoi.Instance.GetRandomRollingBuff(randCount);

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

        SpriteCollection GetBuffIconCollection()
        {
            if (buffIconCol == null)
            {
                buffIconCol = Resources.Load<SpriteCollection>("BuffIcons");
            }
            return buffIconCol;
        }

        private void OnBtnReroll()
        {
            QuanlyNguoichoi.Instance.RerollSkill();
            Init(soOChon);
            Start();
        }

        //private void InitScroll(ScrollRect scroll, List<BuffStat> listBuffStats)
        //{
        //    var scroller = scroll.GetComponent<SimpleScrollSnap>();

        //    buffIconPrefab.gameObject.SetActive(true);

        //    for (int i = 0; i < listBuffStats.Count; i++)
        //    {
        //        var buffStat = listBuffStats[i];

        //        var clone = Instantiate(buffIconPrefab, scroll.content);
        //        clone.iconSkill.sprite = GetBuffIconCollection().GetSprite(string.Format("{0}_Buff", buffStat.Type.ToString()));
        //        if (clone.lblName)
        //        {
        //            clone.lblName.text = buffStat.Type.ToString();
        //        }
        //        clone.name = buffStat.Type.ToString();
        //        clone.gameObject.SetActive(true);
        //        //listElements[i] = clone.gameObject;
        //        //scroller.AddToBack(buffIconPrefab.gameObject);
        //    }
        //    buffIconPrefab.gameObject.SetActive(false);
        //    scroll.GetComponent<UI_InfiniteScroll>().Init();
        //    scroll.GetComponent<UI_ScrollRectOcclusion>().Init();
        //}
        static List<BuffStat> m_ListAvailableBuffs = new List<BuffStat>();
        static List<BuffType> listExcluded = new List<BuffType>();
        private void Init(int num)
        {
            soOChon = num;
            mSelected = false;
            listCurBuffTypes.Clear();
            m_ListAvailableBuffs.Clear();
            foreach (var b in QuanlyNguoichoi.Instance.gameConfig.Buffs)
            {
                int curCount = QuanlyNguoichoi.Instance.PlayerUnit.GetBuffCount(b.Type);

                if (b.RandomRate > 0 && b.ChapUnlock <= QuanlyNguoichoi.Instance.ChapterIndex)
                {
                    m_ListAvailableBuffs.Add(b);
                }
            }
            listExcluded.Clear();

            var evationCfg = QuanlyNguoichoi.Instance.GetBuffStatByType(BuffType.EVASION);
            var curEvation = QuanlyNguoichoi.Instance.PlayerUnit.GetCurStat().EVASION;
            if (curEvation + evationCfg.Params[0] > ConfigManager.GetMaxEvasion())
            {
                listExcluded.Add(BuffType.EVASION);
            }

            if (QuanlyNguoichoi.Instance.HaveHeroPlus("Magician") &&
                QuanlyNguoichoi.Instance.PlayerUnit.UnitName == "Magician")
            {
                listExcluded.Add(BuffType.ELECTRIC_EFF);
                listExcluded.Add(BuffType.FLAME_EFF);
                listExcluded.Add(BuffType.FROZEN_EFF);
            }
            int maxSlot = Mathf.Min(slotSelected.Length, num);
            for (int i = 0; i < maxSlot; ++i)
            {
                //var slot = slots[i];
                var randomType = RandomBuff(listExcluded);
                listCurBuffTypes.Add(randomType);
                listExcluded.Add(randomType);
                //InitScroll(slot, m_ListAvailableBuffs);
                slotSelected[i].transform.parent.gameObject.SetActive(true);
                slotSelected[i].GetComponentInParent<Button>().interactable = false;
            }

            for (int i = maxSlot; i < slotSelected.Length; ++i)
            {
                slotSelected[i].transform.parent.gameObject.SetActive(false);
            }

            btnInfo.gameObject.SetActive(maxSlot <= 3);

            btnReroll.onClick.AddListener(OnBtnReroll);
            btnReroll.gameObject.SetActive(false);

            if (QuanlyNguoichoi.Instance)
            {
                int reRoll = 0;
                var listR = QuanlyNguoichoi.Instance.GetListRuntimeStatMods();
                if (listR != null)
                {
                    foreach (var mod in listR)
                    {
                        if (mod.Stat == EStatType.SKILLREROLL)
                        {
                            if (mod.Mod == EStatModType.ADD)
                            {
                                reRoll += (int)Mathf.Round((float)mod.Val);
                            }
                        }
                    }
                }
                btnReroll.gameObject.SetActive(QuanlyNguoichoi.Instance.LuotReroll < reRoll);
            }
#if DEBUG
            //listCurBuffTypes[0] = BuffType.ELECTRIC_EFF;
            //listCurBuffTypes[1] = BuffType.FLAME_FIELD;
            //listCurBuffTypes[2] = BuffType.ELECTRIC_FIELD;
#endif
            //foreach (var text in slotSelected)
            //{
            //    text.gameObject.SetActive(false);
            //}
        }

        public Vector2 EaseVector(float currentTime, Vector2 startValue, Vector2 changeInValue, float duration)
        {
            return new Vector2(
                changeInValue.x * Mathf.Sin(currentTime / duration * (Mathf.PI / 2)) + startValue.x,
                changeInValue.y * Mathf.Sin(currentTime / duration * (Mathf.PI / 2)) + startValue.y
                );
        }

        IEnumerator DoMove(ScrollRect scrollRect, Vector2 startPos, Vector2 targetPos, float duration)
        {

            // Abort if movement would be too short
            if (duration < 0.05f)
                yield break;

            Vector2 posOffset = targetPos - startPos;

            float currentTime = 0f;
            while (currentTime < duration)
            {
                Debug.LogFormat("{0} to {1} - {2}", scrollRect.normalizedPosition.y, startPos.y, targetPos.y);
                currentTime += Time.unscaledDeltaTime;
                scrollRect.normalizedPosition = EaseVector(currentTime, startPos, posOffset, duration);
                yield return null;
            }

            scrollRect.normalizedPosition = targetPos;
        }

        //IEnumerator CoTweenScroll(ScrollRect slot, Text selectedText, BuffType type)
        //{
        //    float time = 0f;
        //    while (time < 3f)
        //    {
        //        var vertical = slot.verticalNormalizedPosition;
        //        var count = slot.content.childCount;
        //        var speed = count == 0 ? 1 : rollingSpeed / count;
        //        vertical -= speed * Time.unscaledDeltaTime;
        //        while (vertical < 0)
        //        {
        //            vertical += 1f;
        //        }
        //        slot.verticalNormalizedPosition = vertical;
        //        yield return null;
        //        time += Time.unscaledDeltaTime;
        //    }
        //    var index = m_ListAvailableBuffs.FindIndex(e => e.Type == type);
        //    var target = 1f - (float)index / (m_ListAvailableBuffs.Count - 1);
        //    var startPos = slot.normalizedPosition;
        //    yield return StartCoroutine(DoMove(slot, startPos, Vector2.up * target, 0.5f));
        //    selectedText.text = type.ToString();
        //    selectedText.gameObject.SetActive(true);
        //    selectedText.GetComponentInParent<Button>().interactable = true;
        //}

        //private IEnumerator Start()
        private void Start()
        {
            //yield return new WaitForSecondsRealtime(0.15f);

            for (int i = 0; i < listCurBuffTypes.Count; ++i)
            {
                //var slot = slots[i];
                ////StartCoroutine(CoTweenScroll(slot, slotSelected[i], listCurBuffTypes[i]));
                //var index = m_ListAvailableBuffs.FindIndex(e => e.Type == listCurBuffTypes[i]);
                //var target = 1f - (float)index / (m_ListAvailableBuffs.Count - 1);
                //slot.normalizedPosition = Vector3.up * target;

                //int curr_buff_level = QuanlyNguoichoi.Instance.PlayerUnit.GetBuffCount(listCurBuffTypes[i]);

                //if(curr_buff_level==0 || listCurBuffTypes[i]== BuffType.HEAL)
                //    slotSelected[i].text = Localization.Get("Buff_" + listCurBuffTypes[i].ToString());
                //else
                //    slotSelected[i].text = Localization.Get("Buff_" + listCurBuffTypes[i].ToString()) + " " +  string.Format(Localization.Get("Skill_Level"), curr_buff_level+1);

                slotSelected[i].SetBuffType(listCurBuffTypes[i], true);
                slotSelected[i].ShowDesc(false);
                slotSelected[i].gameObject.SetActive(true);
                slotSelected[i].GetComponentInParent<Button>().interactable = true;
            }

            //btnInfo.gameObject.SetActive(true);
            btnInfo.gameObject.SetActive(listCurBuffTypes.Count <= 3);
        }

        [GUIDelegate]
        public void OnBtnBuffClick(int buffIndex)
        {
            if (mSelected) return;
            mSelected = true;
            var buff = listCurBuffTypes[buffIndex];
            if (QuanlyNguoichoi.Instance)
            {
                QuanlyNguoichoi.Instance.GetBuff(buff);
            }

            ScreenBattle.PauseGame(false);
            OnCloseBtnClick();
        }

        [GUIDelegate]
        public void OnBtnInfoClick()
        {
            for (int i = 0; i < slotSelected.Length; ++i)
            {
                slotSelected[i].ShowDesc(true);
            }
            btnInfo.gameObject.SetActive(false);
        }

        public static void Dismiss()
        {
            ScreenBattle.PauseGame(false);
            if (instance != null)
            {
                instance.OnCloseBtnClick();
            }
        }
    }
}