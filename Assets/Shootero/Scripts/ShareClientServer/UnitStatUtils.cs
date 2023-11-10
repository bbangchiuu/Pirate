using System.Collections;
using System.Collections.Generic;

using Hiker.Networks.Data.Shootero;

public partial class UnitStatUtils
{
    public const int LevelInc = 15;
    public static UnitStat GetStatAtLevel(UnitStat orgStat, int level)
    {
        UnitStat result = orgStat;
        if (level > 1)
        {
            result.HP += (level - 1) * LevelInc * orgStat.HP / 100;
            result.DMG += (level - 1) * LevelInc * orgStat.DMG / 100;
        }
        return result;
    }

    public static UnitStat ReplaceStatByStatMod(UnitStat origStat, StatMod replaceStat)
    {
        if (replaceStat.Mod == EStatModType.ADD) // only support replace with type mode == ADD
        {
            switch (replaceStat.Stat)
            {
                case EStatType.ATK:
                    origStat.DMG = (long)replaceStat.Val;
                    break;
                case EStatType.HP:
                    origStat.HP = (long)replaceStat.Val;
                    break;
                case EStatType.ATK_SPD:
                    origStat.ATK_SPD = (float)(replaceStat.Val / 100d);
                    break;
                case EStatType.PRJ_SPD:
                    origStat.PROJ_SPD = (float)replaceStat.Val;
                    break;
                case EStatType.CRIT:
                    origStat.CRIT = (int)replaceStat.Val;
                    break;
                case EStatType.CRIT_DMG:
                    origStat.CRIT_DMG = (int)replaceStat.Val;
                    break;
                case EStatType.ELE_DMG:
                    origStat.ELE_DMG = (int)replaceStat.Val;
                    break;
                case EStatType.EVASION:
                    origStat.EVASION = (int)replaceStat.Val;
                    break;
                case EStatType.KNOCKBACK:
                    origStat.KNOCK_BACK = (float)replaceStat.Val;
                    break;
                case EStatType.MOVE_SPD:
                    origStat.SPD = (float)replaceStat.Val;
                    break;
                default:
                    break;
            }
        }
        return origStat;
    }

    public static bool ModifyStat(ref UnitStat origStat, StatMod mod)
    {
        switch (mod.Stat)
        {
            case EStatType.ATK:
                if (mod.Mod == EStatModType.ADD)
                {
                    origStat.DMG += (long)mod.Val;
                }
                else if (mod.Mod == EStatModType.BUFF)
                {
                    origStat.DMG += (long)(origStat.DMG * mod.Val / 100);
                }
                else // Multiply EStatModType.MUL
                {
                    origStat.DMG = (long)(origStat.DMG * mod.Val / 100);
                }
                break;
            case EStatType.HP:
                if (mod.Mod == EStatModType.ADD)
                {
                    origStat.HP += (long)(mod.Val);
                }
                else if (mod.Mod == EStatModType.BUFF)
                {
                    origStat.HP += (long)(origStat.HP * mod.Val / 100);
                }
                else // Multiply EStatModType.MUL
                {
                    origStat.HP = (long)(origStat.HP * mod.Val / 100);
                }
                break;
            case EStatType.ATK_SPD:
                if (mod.Mod == EStatModType.ADD)
                {
                    origStat.ATK_SPD += (float)(mod.Val / 100d);
                }
                else if (mod.Mod == EStatModType.BUFF)
                {
                    origStat.ATK_SPD += (float)(origStat.ATK_SPD * mod.Val / 100);
                }
                else // Multiply EStatModType.MUL
                {
                    origStat.ATK_SPD = (float)(origStat.ATK_SPD * mod.Val / 100);
                }
                break;
            case EStatType.CRIT:
                if (mod.Mod == EStatModType.ADD)
                {
                    origStat.CRIT += (int)mod.Val;
                }
                else if (mod.Mod == EStatModType.BUFF)
                {
                    origStat.CRIT += (int)(origStat.CRIT * mod.Val / 100);
                }
                else // Multiply EStatModType.MUL
                {
                    origStat.CRIT = (int)(origStat.CRIT * mod.Val / 100);
                }
                break;
            case EStatType.CRIT_DMG:
                if (mod.Mod == EStatModType.ADD)
                {
                    origStat.CRIT_DMG += (int)mod.Val;
                }
                else if (mod.Mod == EStatModType.BUFF)
                {
                    origStat.CRIT_DMG += (int)(origStat.CRIT_DMG * mod.Val / 100);
                }
                else // Multiply EStatModType.MUL
                {
                    origStat.CRIT_DMG = (int)(origStat.CRIT_DMG * mod.Val / 100);
                }
                break;
            case EStatType.ELE_DMG:
                if (mod.Mod == EStatModType.ADD)
                {
                    origStat.ELE_DMG += (int)mod.Val;
                }
                else if (mod.Mod == EStatModType.BUFF)
                {
                    origStat.ELE_DMG += (int)(origStat.ELE_DMG * mod.Val / 100);
                }
                else // Multiply EStatModType.MUL
                {
                    origStat.ELE_DMG = (int)(origStat.ELE_DMG * mod.Val / 100);
                }
                break;
            case EStatType.EVASION:
                if (mod.Mod == EStatModType.ADD)
                {
                    origStat.EVASION += (int)mod.Val;
                }
                else if (mod.Mod == EStatModType.BUFF)
                {
                    origStat.EVASION += (int)(origStat.EVASION * mod.Val / 100);
                }
                else // Multiply EStatModType.MUL
                {
                    origStat.EVASION = (int)(origStat.EVASION * mod.Val / 100);
                }
                break;
            case EStatType.PRJ_SPD:
                if (mod.Mod == EStatModType.ADD)
                {
                    origStat.PROJ_SPD += (long)mod.Val;
                }
                else if (mod.Mod == EStatModType.BUFF)
                {
                    origStat.PROJ_SPD += origStat.PROJ_SPD * (long)mod.Val / 100;
                }
                else // Multiply EStatModType.MUL
                {
                    origStat.PROJ_SPD = origStat.PROJ_SPD * (long)mod.Val / 100;
                }
                break;
            case EStatType.MOVE_SPD:
                if (mod.Mod == EStatModType.ADD)
                {
                    origStat.SPD += (long)mod.Val;
                }
                else if (mod.Mod == EStatModType.BUFF)
                {
                    origStat.SPD += origStat.SPD * (long)mod.Val / 100;
                }
                else // Multiply EStatModType.MUL
                {
                    origStat.SPD = origStat.SPD * (long)mod.Val / 100;
                }
                break;
            default:
                return false;
        }

        return true;
    }

    public static UnitStat ModifyStat(UnitStat origStat, List<StatMod> mods, out List<StatMod> remainMods)
    {
        remainMods = new List<StatMod>();

        // process add mod first
        foreach (var m in mods)
        {
            if (m.Mod == EStatModType.ADD)
            {
                if (ModifyStat(ref origStat, m) == false)
                {
                    remainMods.Add(m);
                }
            }
        }

        // process mul mod first
        foreach (var m in mods)
        {
            if (m.Mod == EStatModType.MUL)
            {
                if (ModifyStat(ref origStat, m) == false)
                {
                    remainMods.Add(m);
                }
            }
        }

        // process mul mod first
        foreach (var m in mods)
        {
            if (m.Mod == EStatModType.BUFF)
            {
                if (ModifyStat(ref origStat, m) == false)
                {
                    remainMods.Add(m);
                }
            }
        }

        return origStat;
    }

    public static List<StatMod> GetColapseStatMods(List<StatMod> listMods)
    {
        List<StatMod> statMods = new List<StatMod>();
        for (int i = listMods.Count - 1; i >= 0; --i)
        {
            var mod = listMods[i];
            if (mod.Stat == EStatType.GETSKILL ||
                mod.Stat == EStatType.SKILLPLUS ||
                mod.Stat == EStatType.HEROPLUS ||
                mod.Stat == EStatType.DETU)
            {
                statMods.Add(mod);
            }
            else
            if (mod.Stat == EStatType.DEF_ON ||
                mod.Stat == EStatType.ATK_ON)
            {
                var curModIndex = statMods.FindIndex(e => e.Stat == mod.Stat && e.Mod == mod.Mod && e.Target == mod.Target);
                if (curModIndex < 0)
                {
                    statMods.Add(mod);
                }
                else
                {
                    var curMod = statMods[curModIndex];
                    curMod.Val += mod.Val;
                    statMods[curModIndex] = curMod;
                }
            }
            else
            {
                var curModIndex = statMods.FindIndex(e => e.Stat == mod.Stat && e.Mod == mod.Mod);
                if (curModIndex < 0)
                {
                    statMods.Add(mod);
                }
                else
                {
                    var curMod = statMods[curModIndex];
                    curMod.Val += mod.Val;
                    statMods[curModIndex] = curMod;
                }
            }
        }

        return statMods;
    }

    public static UnitStat BuildStatFromHeroEquipment(HeroData hero,
        List<TrangBiData> listItems,
        List<ArmoryData> listArmories,
        List<HeroData> listHeroes,
        List<CardData> listCards,
        out int vkAtkSpd,
        out List<StatMod> runTimeMods)
    {
        var baseStat = ConfigManager.GetUnitStat(hero.Name);

        List<TrangBiData> listEquipments = new List<TrangBiData>();
        List<CardData> listCardEquips = new List<CardData>();

        List<StatMod> listBasicVuKhiStat = new List<StatMod>();
        List<StatMod> listAdvceVuKhiStat = new List<StatMod>();
        TrangBiData vuKhi = null;

        foreach (var t in hero.ListSlots)
        {
            if (t.Slot == SlotType.VuKhi)
            {
                if (string.IsNullOrEmpty(t.TrangBi) == false)
                {
                    vuKhi = listItems.Find(e => e.ID == t.TrangBi);
                    if (vuKhi != null)
                    {
                        listBasicVuKhiStat.AddRange(GetBasicStatFromEquipment(vuKhi));
                        listAdvceVuKhiStat.AddRange(GetAdvanceStatFromEquipment(vuKhi));

                        if (vuKhi.CardSlots != null)
                        {
                            foreach (var c in vuKhi.CardSlots)
                            {
                                var card = listCards.Find(e => e.ID == c);
                                if (card != null)
                                {
                                    listCardEquips.Add(card);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(t.TrangBi) == false)
                {
                    var item = listItems.Find(e => e.ID == t.TrangBi);
                    if (item != null)
                    {
                        listEquipments.Add(item);

                        if (item.CardSlots != null)
                        {
                            foreach (var c in item.CardSlots)
                            {
                                var card = listCards.Find(e => e.ID == c);
                                if (card != null)
                                {
                                    listCardEquips.Add(card);
                                }
                            }
                        }
                    }
                }
            }
        }

        List<StatMod> listBasicMod = GetBasicStatFromEquipments(listEquipments);
        List<StatMod> listAdvceMod = GetAdvanceStatFromEquipments(listEquipments);

        List<StatMod> listArmStats = new List<StatMod>();
        foreach (var arm in listArmories)
        {
            if (arm != null && arm.Level > 0)
            {
                var armStats = ConfigManager.GetArmoryStat(arm.Name, arm.Level);
                listArmStats.AddRange(armStats);
            }
        }

        List<StatMod> listCardStats = GetStatsFromCards(listCardEquips);

        //listAdvceVuKhiStat.AddRange(listAdvceMod);
        //listAdvceMod = GetColapseStatMods(listAdvceVuKhiStat);
#region Buff Basic Stat of Equipments
        double buffStat = 0;
        listAdvceMod.ForEach(e =>
        {
            if (e.Stat == EStatType.BUFFSTAT)
            {
                buffStat += e.Val;
            }
        });
        listAdvceVuKhiStat.ForEach(e =>
        {
            if (e.Stat == EStatType.BUFFSTAT)
            {
                buffStat += e.Val;
            }
        });
        listArmStats.ForEach(e =>
        {
            if (e.Stat == EStatType.BUFFSTAT)
            {
                buffStat += e.Val;
            }
        });

        for (int i = 0; i < listBasicVuKhiStat.Count; ++i)
        {
            var mod = listBasicVuKhiStat[i];
            mod.Val += (mod.Val * buffStat / 100);
            listBasicVuKhiStat[i] = mod;
        }

        for (int i = 0; i < listBasicMod.Count; ++i)
        {
            var mod = listBasicMod[i];
            mod.Val += (mod.Val * buffStat / 100);
            listBasicMod[i] = mod;
        }
#endregion

        List<StatMod> listHeroesStats = new List<StatMod>();
        foreach (var h in listHeroes)
        {
            if (h != null && h.Level > 0)
            {
                var heroStats = ConfigManager.GetHeroStats(h.Name, h.Level);
                listHeroesStats.AddRange(heroStats);
            }
        }

        List<StatMod> listRemainMods = new List<StatMod>();
        listRemainMods.AddRange(listArmStats);
        listRemainMods.AddRange(listBasicVuKhiStat);
        listRemainMods.AddRange(listBasicMod);
        listRemainMods.AddRange(listHeroesStats);
        listRemainMods.AddRange(listCardStats);
        vkAtkSpd = ConfigManager.GetAtkSpdNormalize();
        if (listAdvceVuKhiStat.Count > 0)
        {
            foreach (var m in listAdvceVuKhiStat) // Replace atk speed from weapon
            {
                if (m.Stat == EStatType.ATK_SPD && m.Mod == EStatModType.ADD)
                {
                    vkAtkSpd = (int)m.Val;
                    baseStat = ReplaceStatByStatMod(baseStat, m);
                }
                else if (m.Stat == EStatType.PRJ_SPD && m.Mod == EStatModType.ADD)
                {
                    baseStat = ReplaceStatByStatMod(baseStat, m);
                }
                else if (m.Stat == EStatType.KNOCKBACK && m.Mod == EStatModType.ADD)
                {
                    baseStat = ReplaceStatByStatMod(baseStat, m);
                }
                else
                {
                    listRemainMods.Add(m);
                }
            }
        }

        listRemainMods.AddRange(listAdvceMod);

        var totalMods = GetColapseStatMods(listRemainMods);

        baseStat = ModifyStat(baseStat, totalMods, out listRemainMods);

        runTimeMods = listRemainMods;

        return baseStat;
    }

    public static StatMod[] GetBasicStatFromEquipment(string item, int level, ERarity rarity)
    {
        var itemConfig = ConfigManager.GetItemConfig(item);
        return itemConfig.GetBasicStat(level, (int)rarity);
    }

    public static StatMod[] GetBasicStatFromEquipment(TrangBiData it)
    {
        var itemConfig = ConfigManager.GetItemConfig(it);
        return itemConfig.GetBasicStat(it.Level, (int)it.Rarity);
    }

    public static StatMod[] GetAdvanceStatFromEquipment(TrangBiData it)
    {
        var itemConfig = ConfigManager.GetItemConfig(it);
        return itemConfig.GetAdvanceStat(it.Level, (int)it.Rarity);
    }

    public static StatMod[] GetStatsFromCard(CardData card)
    {
        var cfg = ConfigManager.GetCardConfig(card);
        return cfg.Stats;
    }
    public static List<StatMod> GetStatsFromCards(List<CardData> cards)
    {
        List<StatMod> listStartMods = new List<StatMod>();
        foreach (var it in cards)
        {
            listStartMods.AddRange(GetStatsFromCard(it));
        }
        return listStartMods;
    }

    public static List<StatMod> GetBasicStatFromEquipments(List<TrangBiData> items)
    {
        List<StatMod> listStartMods = new List<StatMod>();
        foreach (var it in items)
        {
            listStartMods.AddRange(GetBasicStatFromEquipment(it));
        }

        return listStartMods;
    }

    public static List<StatMod> GetAdvanceStatFromEquipments(List<TrangBiData> items)
    {
        List<StatMod> listStartMods = new List<StatMod>();
        foreach (var it in items)
        {
            listStartMods.AddRange(ConfigManager.GetItemConfig(it).GetAdvanceStat(it.Level, (int)it.Rarity));
        }

        return listStartMods;
    }


    //public static UnitStat BuildStatFromHeroEquipment(HeroData hero, List<ItemData> listItems)
    //{
    //    var baseStat = ConfigManager.GetUnitStat(hero.Name);
    //    var atk = System.Array.Find(hero.ListSlots, e => e.Slot == EquipmentSlot.SlotType.VuKhi); // default slot = ATK

    //    List<StatMod> listMods = new List<StatMod>();

    //    if (string.IsNullOrEmpty(atk.TrangBi) == false)
    //    {
    //        var vuKhi = listItems.Find(e => e.ID == atk.TrangBi);
    //        if (vuKhi != null)
    //        {
    //            var statMods = ConfigManager.GetStatModsByItems(vuKhi);

    //            baseStat = ReplaceStatByStatMod(baseStat, atkSpdMod);
    //        }
    //    }
    //}
}