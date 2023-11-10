using System.Collections;
using System.Collections.Generic;

#if ANTICHEAT
using Int64 = CodeStage.AntiCheat.ObscuredTypes.ObscuredLong;
using Int32 = CodeStage.AntiCheat.ObscuredTypes.ObscuredInt;
using Float = CodeStage.AntiCheat.ObscuredTypes.ObscuredFloat;
using Bool = CodeStage.AntiCheat.ObscuredTypes.ObscuredBool;
#else
using Int64 = System.Int64;
using Int32 = System.Int32;
using Float = System.Single;
using Bool = System.Boolean;
#endif

public struct UnitStat
{
    #region Attack System Stats
    // Basic Stat
    public Int64 HP;
    public Int64 DMG;

    public Float ATK_RANGE;
    public Float ATK_SPD;
    public Float PROJ_SPD;
    public Int32 CRIT;
    public Int32 CRIT_DMG;
    public Int32 ELE_DMG;
    public Int32 EVASION;

    public Int64 COLLISION_DMG;

    /// <summary>
    /// Knock back Resistance percent
    /// </summary>
    public Float KNOCK_BACK_RES;
    public Float KNOCK_BACK;
    #endregion

    #region Movement System Stats
    public Float BODY_RADIUS;
    public Float SPD;
    #endregion

    #region Other
    public string HitSound;
    public string Ava;
    public string Fire1Sound;
    public string Fire2Sound;
    public string Fire3Sound;
    public string Fire4Sound;
    #endregion

    public static UnitStat FromUnitStat(UnitStatWrapper stat)
    {
        return new UnitStat()
        {
            #region Attack System Stats
            HP = stat.HP,
            DMG = stat.DMG,
            ATK_RANGE = stat.ATK_RANGE,
            ATK_SPD = stat.ATK_SPD,
            PROJ_SPD = stat.PROJ_SPD,
            CRIT = stat.CRIT,
            CRIT_DMG = stat.CRIT_DMG,
            ELE_DMG = stat.ELE_DMG,
            EVASION = stat.EVASION,

            COLLISION_DMG = stat.COLLISION_DMG,
            KNOCK_BACK_RES = stat.KNOCK_BACK_RES,
            KNOCK_BACK = stat.KNOCK_BACK,
            #endregion

            #region Movement System Stats
            SPD = stat.SPD,
            #endregion

            Ava = stat.Ava,
        };
    }

    public UnitStatWrapper ToUnitStatWrapper()
    {
        var stat = this;

        return new UnitStatWrapper()
        {
            #region Attack System Stats
            HP = stat.HP,
            DMG = stat.DMG,
            ATK_RANGE = stat.ATK_RANGE,
            ATK_SPD = stat.ATK_SPD,
            PROJ_SPD = stat.PROJ_SPD,
            CRIT = stat.CRIT,
            CRIT_DMG = stat.CRIT_DMG,
            ELE_DMG = stat.ELE_DMG,
            EVASION = stat.EVASION,

            COLLISION_DMG = stat.COLLISION_DMG,
            KNOCK_BACK_RES = stat.KNOCK_BACK_RES,
            KNOCK_BACK = stat.KNOCK_BACK,
            #endregion

            #region Movement System Stats
            SPD = stat.SPD,
            #endregion
        };
    }

    public static UnitStat FromUnitStat(UnitStatConfig stat)
    {
        return new UnitStat()
        {
            #region Attack System Stats
            HP = stat.HP,
            DMG = stat.DMG,
            ATK_RANGE = stat.ATK_RANGE,
            ATK_SPD = stat.ATK_SPD,
            PROJ_SPD = stat.PROJ_SPD,
            CRIT = stat.CRIT,
            CRIT_DMG = stat.CRIT_DMG,
            ELE_DMG = stat.ELE_DMG,
            EVASION = stat.EVASION,

            COLLISION_DMG = stat.COLLISION_DMG,
            KNOCK_BACK_RES = stat.KNOCK_BACK_RES,
            KNOCK_BACK = stat.KNOCK_BACK,
            #endregion

            #region Movement System Stats
            BODY_RADIUS = stat.BODY_RADIUS,
            SPD = stat.SPD,
            #endregion

            #region Other
            HitSound = stat.HitSound,
            Fire1Sound = stat.Fire1Sound,
            Fire2Sound = stat.Fire2Sound,
            Fire3Sound = stat.Fire3Sound,
            Fire4Sound = stat.Fire4Sound,
            Ava = stat.Ava,
            #endregion
        };
    }
}

public class UnitStatWrapper
{
    #region Attack System Stats
    public long HP;
    public long DMG;
    public float ATK_RANGE;
    public float ATK_SPD;
    public float PROJ_SPD;
    public int CRIT;
    public int CRIT_DMG;
    public int ELE_DMG;
    public int EVASION;

    public long COLLISION_DMG;
    /// <summary>
    /// Knock back Resistance percent
    /// </summary>
    public float KNOCK_BACK_RES;
    public float KNOCK_BACK;
    #endregion

    #region Movement System Stats
    public float SPD;
    #endregion

    /// <summary>
    /// 0 is non-set
    /// 1 is true
    /// -1 is false
    /// </summary>
    public int IsBoss;
    /// <summary>
    /// 0 is non-set
    /// 1 is true
    /// -1 is false
    /// </summary>
    public int IsAir;
    public long STReceived;
    public string Ava;
}

public class WeaponStatConfig
{
    public string PROJ_N;
    public long DMG;
    public float ATK_SPD;
    public float PROJ_SPD;
    public float KNOCK_BACK;
    public float ATK_RANGE;
    public float CRIT;
    public float CRIT_DMG;
    public Dictionary<string, StatMod> StatLevels;

    public List<BuffStat> listBuff;
    public static WeaponStatConfig FromWeaponStat(string nameWeapon)
    {
        if (ConfigManager.WeaponStats.ContainsKey(nameWeapon)) return ConfigManager.WeaponStats[nameWeapon];
        return null;
    }

    public double GetStatLevel(int level, EStatType stat)
    {        
        if (level > 0 && StatLevels != null && StatLevels.ContainsKey(stat.ToString()))
        {
            var statLevel = StatLevels[stat.ToString()];
            int idxLevel = level - 1 >= statLevel.Inc.Length ? statLevel.Inc.Length - 1 : level - 1;
            return statLevel.Inc[idxLevel];
        }

        switch (stat)
        {
            case EStatType.ATK:
                return DMG;
            case EStatType.ATK_SPD:
                return ATK_SPD;
            case EStatType.CRIT:
                return CRIT;
            case EStatType.CRIT_DMG:
                return CRIT_DMG;
        }
        return 0;
    }

    public bool CheckHaveBuff(BuffType buffType)
    {
        if (listBuff != null)
        {
            for (int i = 0; i < listBuff.Count; i++)
            {
                if (listBuff[i].Type == buffType)
                {
                    return true;
                }
            }
        }

        return false;
    }
    public BuffStat GetBuff(BuffType buffType)
    {
        if (listBuff != null)
        {
            for (int i = 0; i < listBuff.Count; i++)
            {
                if (listBuff[i].Type == buffType)
                {
                    return listBuff[i];
                }
            }
        }

        return default;
    }
    public int GetStatBuff(BuffType buffType)
    {
        if(listBuff != null)
        {
            for(int i = 0; i < listBuff.Count; i++)
            {
                if(listBuff[i].Type == buffType)
                {
                    return (int)listBuff[i].Params[0];
                }
            }
        }

        return 0;
    }
}

public class UnitStatConfig
{
    #region Attack System Stats
    public long HP;
    public long DMG;
    public float ATK_RANGE;
    public float ATK_SPD;
    public float PROJ_SPD;
    public int CRIT;
    public int CRIT_DMG;
    public int ELE_DMG;
    public int EVASION;

    public long COLLISION_DMG;
    /// <summary>
    /// Knock back Resistance percent
    /// </summary>
    public float KNOCK_BACK_RES;
    public float KNOCK_BACK;
    #endregion

    #region Movement System Stats
    public float BODY_RADIUS;
    public float SPD;
    #endregion

    #region Other
    public string Ava;
    public string HitSound;
    public string Fire1Sound;
    public string Fire2Sound;
    public string Fire3Sound;
    public string Fire4Sound;
    #endregion
}

public enum EStatType
{
    #region Basic Stats
    HP,                                 // 0
    ATK,                                // 1
    #endregion
    /// <summary>
    /// ATK RUNTIME
    /// </summary>
    ATK_ON, // ATK RUNTIME ON Target    // 2
    DEF_ON, // DEF RUNTIME ON Target    // 3

    ATK_SPD,                            // 4
    PRJ_SPD,                            // 5
    CRIT,                               // 6
    CRIT_DMG,                           // 7
    EVASION,                            // 8

    HEALING,                            // 9

    ATK_ADDON,                          // 10
    HEALORB, // tang luong hoi Heal orb
    HPLEVELUP, // tang luong hoi khi len level trong battle
    BUFFSTAT, // tang % chi so co ban
    AFKREWARD,
    STARTSKILL,
    GETSKILL,
    EXP,
    KNOCKBACK,
    MOVE_SPD,
    ELE_DMG,
    SKILLPLUS,
    HEROPLUS,
    SKILLREROLL,
    BLESSINGPLUS,
    DETU,
    REGEN,
    DMG_ON_FREEZE
}

public enum EStatModType
{
    ADD,
    MUL, // Multiply or scale
    BUFF, // Increase by percent
}

public class StatModWraper
{
    public EStatType Stat;
    public EStatModType Mod;
    public string Target;
    public double Val;
    public static StatModWraper FromStatMod(StatMod mod)
    {
        return new StatModWraper()
        {
            Stat = mod.Stat,
            Mod = mod.Mod,
            Target = mod.Target,
            Val = mod.Val
        };
    }

    public static List<StatModWraper> FromStatMod(IEnumerable<StatMod> mods)
    {
        var result = new List<StatModWraper>();
        foreach (var m in mods)
        {
            result.Add(FromStatMod(m));
        }
        return result;
    }

    public StatMod ToStatMod()
    {
        return new StatMod()
        {
            Stat = this.Stat,
            Mod = this.Mod,
            Target = this.Target,
            Val = this.Val
        };
    }
}

public struct StatMod
{
    public EStatType Stat;
    public EStatModType Mod;
    public string Target;
    public double Val;
    public double[] Inc;

    public double GetTotalIncByLevel(int level)
    {
        double totalInc = 0;
        if (Inc != null)
        {
            foreach (var inc in Inc)
            {
                if (--level > 0)
                {
                    totalInc += inc;
                }
            }
        }

        return totalInc;
    }
    //public long GetIncByLevel(int level)
    //{
    //    double levelInc = 0;
    //    if (Inc != null && Inc.Length > level && level > 0)
    //    {
    //        levelInc = Inc[level - 1];
    //    }
    //    return (long)levelInc;
    //}
}

public enum ERarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legend,
    Mysthic
}

public enum SlotType
{
    VuKhi,
    Mu,
    Giap,
    OffHand,
    //Addon,
}

public class CardConfig
{
    public SlotType Slot;
    public ERarity Rarity;
    public string[] UnitsDrop;
    public StatMod[] Stats;
    public int MaxDrop;
}

public class ItemConfig
{
    public struct ItemStat
    {
        public int IncBuff;
        public StatMod[] Basic;
        public StatMod[] Advance;

        public int GetIncBuff() // DefaultIncBuff = 15% each rarity
        {
            if (IncBuff > 0) return IncBuff;

            return 15;
        }
    }

    public SlotType Slot;
    public Dictionary<string, ItemStat> Stats;
    public string VuKhiType;
    public float RandomRate = 1f;
    public int GoldCost = 0;
    public int GemCost = 0;

    public ItemStat GetStatByRarity(int rarity, out int cfgRarity)
    {
        /// maxRarity lower than rarity
        ERarity minRarity = ConfigManager.MinRarity;
        if (rarity > (int)ConfigManager.MaxRarity)
        {
            rarity = (int)ConfigManager.MaxRarity;
        }
        if (rarity < (int)ConfigManager.MinRarity)
        {
            rarity = (int)ConfigManager.MinRarity;
        }

        for (int i = rarity; i >= (int)ConfigManager.MinRarity; --i)
        {
            string rarityStr = ((ERarity)i).ToString();
            if (Stats.ContainsKey(rarityStr))
            {
                if ((int)minRarity < i)
                {
                    minRarity = (ERarity)i;
                }
            }
        }

        var rarityString = ((ERarity)rarity).ToString();

        ItemStat rarityStat;
        ItemStat minRarityStat = Stats[minRarity.ToString()];
        if (Stats.ContainsKey(rarityString))
        {
            cfgRarity = rarity;
            rarityStat = Stats[rarityString];
        }
        else
        {
            rarityStat = minRarityStat;
            cfgRarity = (int)minRarity;
        }

        return rarityStat;
    }

    public StatMod[] GetBasicStat(int level, int rarity)
    {
        var rarityStat = GetStatByRarity(rarity, out int cfgRarity);

        var baseRarity = ERarity.Common;
        for (int i = rarity; i >= (int)ConfigManager.MinRarity; --i)
        {
            string rarityStr = ((ERarity)i).ToString();
            if (Stats.ContainsKey(rarityStr))
            {
                var stat = Stats[rarityStr];
                if (stat.Basic != null && stat.Basic.Length > 0 && stat.Basic[0].Inc != null && stat.Basic[0].Inc.Length > 0)
                {
                    if ((int)baseRarity < i)
                    {
                        baseRarity = (ERarity)i;
                    }
                }
            }
        }

        double incScale = 1d;

        for (int i = rarity; i > (int)baseRarity; --i)
        {
            string rarityStr = ((ERarity)i).ToString();

            if (Stats.ContainsKey(rarityStr))
            {
                incScale *= (1d + Stats[rarityStr].GetIncBuff() / 100d);
            }
        }

        var baseStat = Stats[baseRarity.ToString()];
        var basicStats = new StatMod[rarityStat.Basic.Length];
        for (int i = 0; i < rarityStat.Basic.Length; ++i)
        {
            var mod = rarityStat.Basic[i];
            var baseMod = baseStat.Basic[i];
            mod.Val += baseMod.GetTotalIncByLevel(level) * incScale;
            //if (baseMod.Inc != null)
            //{
            //    mod.Inc = new double[baseMod.Inc.Length];
            //    for (int k = 0; k < baseMod.Inc.Length; ++k)
            //    {
            //        mod.Inc[k] = baseMod.Inc[k] * incScale;
            //    }
            //}
            //else
            {
                mod.Inc = null;
            }

            basicStats[i] = mod;
        }
        return basicStats;
    }
    public StatMod[] GetAdvanceStat(int level, int rarity)
    {
        var stat = GetStatByRarity(rarity, out int cfgRarity);

        return stat.Advance;
    }
}

public class HeroUpgradeConfig
{
    public int[] GoldCost;
    public int[] HeroStoneCost;
    public Dictionary<string, HeroStatsCfg> Heroes;

    public class HeroStatsCfg
    {
        public int LevelMax;
        public StatMod[] BasicStats;
        public AdvancedStatsCfg[] AdvancedStats;
    }
    public class AdvancedStatsCfg
    {
        public int unlockLevel;
        public StatMod Stat;
    }
}

