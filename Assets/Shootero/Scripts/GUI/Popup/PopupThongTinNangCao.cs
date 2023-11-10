using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hiker.GUI.Shootero
{
    using Hiker.Networks.Data.Shootero;
    using System.Linq;
    using UnityEngine.UI;
    public class PopupThongTinNangCao : PopupBase
    {
        public Text lbStatDesc;

        public static PopupThongTinNangCao instance;

        static readonly EStatType[] AdvanceStatShow = new EStatType[]
        {
            EStatType.GETSKILL,
            EStatType.HEALING,
            EStatType.EXP,
            EStatType.ELE_DMG,
            EStatType.DEF_ON,
            EStatType.ATK_ON,
            EStatType.HPLEVELUP,
            EStatType.HEALORB,
            EStatType.REGEN,
            EStatType.SKILLPLUS,
        };

        public static PopupThongTinNangCao Create(string heroName, UnitStat basicStat, int vkAtkSpd, List<StatMod> listRuntimeMods)
        {
            var popup = Create();
            popup.Init(heroName, basicStat, vkAtkSpd, listRuntimeMods);
            return popup;
        }

        private static PopupThongTinNangCao Create()
        {
            if (instance)
            {
                Destroy(instance.gameObject);
                instance = null;
            }

            var go = PopupManager.instance.GetPopup("PopupThongTinNangCao");
            instance = go.GetComponent<PopupThongTinNangCao>();
            return instance;
        }

        StatMod[] GetStatModsFromUnitStat(UnitStat basicStat, int vkAtkSpd, float proj_spd, float mv_spd)
        {
            StatMod[] mods = new StatMod[]
            {
                new StatMod { Stat = EStatType.HP, Mod = EStatModType.ADD, Val = basicStat.HP },
                new StatMod { Stat = EStatType.ATK, Mod = EStatModType.ADD, Val = basicStat.DMG },
                new StatMod { Stat = EStatType.ATK_SPD, Mod = EStatModType.BUFF, Val = Mathf.Round(Mathf.Round(basicStat.ATK_SPD * 100 - vkAtkSpd) * 100 / vkAtkSpd) },
                new StatMod { Stat = EStatType.CRIT, Mod = EStatModType.BUFF, Val = basicStat.CRIT },
                new StatMod { Stat = EStatType.CRIT_DMG, Mod = EStatModType.ADD, Val = basicStat.CRIT_DMG - 200 },
                new StatMod { Stat = EStatType.EVASION, Mod = EStatModType.ADD, Val = basicStat.EVASION },
                new StatMod { Stat = EStatType.PRJ_SPD, Mod = EStatModType.BUFF, Val = Mathf.Round((basicStat.PROJ_SPD - proj_spd) * 100 / proj_spd) },
                new StatMod { Stat = EStatType.MOVE_SPD, Mod = EStatModType.BUFF, Val = Mathf.Round((basicStat.SPD - mv_spd) * 100 / mv_spd) },
                new StatMod { Stat = EStatType.ELE_DMG, Mod = EStatModType.ADD, Val = basicStat.ELE_DMG },
            };

            return mods;
        }

        void Init(string heroName, UnitStat basicStat, int vkAtkSpd, List<StatMod> listRuntimeMods)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine();

            var heroStat = ConfigManager.GetUnitStat(heroName);

            var basicStats = GetStatModsFromUnitStat(basicStat, vkAtkSpd, heroStat.PROJ_SPD, heroStat.SPD);

            for (int i = 0; i < basicStats.Length; ++i)
            {
                if (basicStats[i].Val > 0)
                {
                    string basic_stats_str = ConfigManager.GetStatDesc(basicStats[i]);

                    sb.AppendLine(basic_stats_str);
                }
            }

            //if (advStats.Length > 0)
            //{
            //    sb.AppendLine("---");
            //}

            var checkShowAdvance = ConfigManager.GetAdvanceStatShow();
            if (checkShowAdvance == null)
                checkShowAdvance = AdvanceStatShow;

            for (int i = 0; i < listRuntimeMods.Count; ++i)
            {
                var statCfg = listRuntimeMods[i];
                if (AdvanceStatShow.Contains(statCfg.Stat))
                {
                    sb.AppendLine(ConfigManager.GetStatDesc(statCfg));
                }
            }
            lbStatDesc.text = sb.ToString();
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